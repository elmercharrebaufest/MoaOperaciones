using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Dto.Scato;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenResiduosService : OrdenDeCargaServiceBase, IOrdenResiduosService
    {
        private readonly IRepositorioOrdenResiduos repositorioResiduos;
        private readonly IEmailResiduosService emailResiduosService;

        public OrdenResiduosService(
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IScatoRepositorioClient scatoRepositorioClient,
            IRepositorioOrdenResiduos repositorioResiduos,
            ICNRTClient cNRTClient,
            IFeriadoService feriadoService,
            IEmailResiduosService emailResiduosService,
            IUbicacionGeograficaService ubicacionGeograficaService
            ) : base(ordenCargaConsumer, scatoConsumer, scatoRepositorioClient, repositorioResiduos, cNRTClient, feriadoService, ubicacionGeograficaService)
        {
            this.repositorioResiduos = repositorioResiduos;
            this.emailResiduosService = emailResiduosService;
        }

        public List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientes()
        {
            return repositorioResiduos.ObtenerClientesResiduos();
        }

        public MaterialDto[] ObtenerMateriales()
        {
            return repositorioResiduos.ObtenerMateriales();
        }

        public ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicioStr, string fechaFinStr, string mailUsuario)
        {
            var fechaInicio = DataFormatter.StringToDateTime(fechaInicioStr, "");
            var fechaFin = DataFormatter.StringToDateTime(fechaFinStr, "");

            var usuario = repositorioResiduos.ObtenerUsuarioSegunMail(mailUsuario);
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaResiduosAdmin);

            var listado = repositorioResiduos.ObtenerListadoOrdenes(fechaInicio, fechaFin, esInterno);

            if (listado == null || listado.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga de residuos e insumos"));
            }

            return new ListarOrdenesResiduosResponse
            {
                ListaOrdenes = listado
            };
        }

        public LocalidadDto[] ObtenerLocalidades()
        {
            return repositorioResiduos.ObtenerLocalidades();
        }

        public PatentesClienteDto ObtenerPatentes(int clienteId)
        {
            return repositorioResiduos.ObtenerPatentesDeOrdenes(clienteId);
        }

        public List<SustitucionMOAModel.Dto.OrdenDeCarga.PlantaDto> ObtenerPlantas(string cuit)
        {
            var plantasRes = scatoRepositorioClient.ObtenerPlantas(cuit);
            if (!plantasRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Scato con CUIT " + cuit);
                LogMensajesScato(plantasRes);
                throw new ValidationCustomException("Error al obtener Plantas");
            }
            else
            {
                return plantasRes.Data
                    .Select(x =>
                        new SustitucionMOAModel.Dto.OrdenDeCarga.PlantaDto
                        {
                            Actividad = x.Actividad,
                            Codigo = x.NroPlanta
                        })
                    .ToList();
            }
        }

        public List<SustitucionMOAModel.Dto.OrdenDeCarga.DomicilioDto> ObtenerDomicilios(string cuit)
        {
            var domiciliosRes = scatoRepositorioClient.ObtenerDomicilios(cuit);
            if (!domiciliosRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Domicilios con CUIT " + cuit);
                LogMensajesScato(domiciliosRes);
                throw new ValidationCustomException("Error al obtener Domicilios");
            }
            else
            {
                return domiciliosRes.Data
                    .Select(x =>
                        new SustitucionMOAModel.Dto.OrdenDeCarga.DomicilioDto
                        {
                            Descripcion = x.Descripcion,
                            Orden = x.Orden,
                            Tipo = x.Tipo
                        })
                    .ToList();
            }
        }

        public bool EsCuilCuitValido(string cuilCuit)
        {
            if (!EsDigitoCuilCuitValido(cuilCuit))
            {
                return false;
            }
            return true;
        }

        public TransportesIds ObtenerIdsTransportes(int clienteId, string patenteAcoplado)
        {
            return repositorioResiduos.ObtenerIdsTransportes(clienteId, patenteAcoplado);
        }

        public GrabarOrdenResponse CrearNuevaOrden(OrdenResiduosDto ordenDto, string mailUsuario)
        {
            Log.Info($"Crear nueva orden residuos: [{ordenDto.ToJson()}], usuario: [{mailUsuario}]");
            ValidarOrden(ordenDto);

            var existeTransporte = ExisteTransporte(ordenDto.CUITTransporte);
            ordenDto.Estado = new EstadoOrdenResiduosDto
            {
                Id = existeTransporte ? (int)EstadoOrdenResiduosEnum.OrdenGenerada : (int)EstadoOrdenResiduosEnum.Pendiente
            };

            CompletarDatosDestino(ordenDto);

            for (int i = 0; i < ordenDto.CantidadDeViajes; i++)
            {
                var ordenEntity = ordenDto.ToEntity();
                ordenEntity.DomicilioDescr = ordenEntity.DomicilioDescr?.Substring(ordenEntity.DomicilioDescr.IndexOf(" ") + 1);
                ordenEntity.FechaCreacion = DateTime.Now;
                repositorioResiduos.Agregar(ordenEntity);
            }

            repositorioResiduos.GuardarCambios();

            if (!existeTransporte)
            {
                emailResiduosService.EnviarMailTransporteNoExiste(ordenDto.RazonSocialTransporte, ordenDto.CUITTransporte);
            }

            return new GrabarOrdenResponse { Mensaje = SuccessMsg.OrdenDeCargaAgregada };
        }

        public GrabarOrdenResponse EditarOrden(OrdenResiduosDto ordenDto, string mailUsuario)
        {
            Log.Info($"Editar orden residuos: [{ordenDto.ToJson()}]");
            ValidarOrden(ordenDto);

            var usuario = repositorioResiduos.ObtenerUsuarioSegunMail(mailUsuario);
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaResiduosAdmin);
            var ordenEntity = repositorioResiduos.ObtenerOrdenResiduos(ordenDto.Id);

            if (!esInterno)
            {
                var camionEstaEnPlanta = OrdenEstaActivaEnScato(ordenDto.Id);
                if (camionEstaEnPlanta)
                {
                    emailResiduosService.EnviarMailIntentoEdicionOrdenActiva(ordenEntity);
                    throw new ValidationCustomException("La orden no se puede editar por estar el camión en planta");
                }
            }

            var estadosPermitenEdicion = new EstadoOrdenResiduosEnum[]
            {
                EstadoOrdenResiduosEnum.OrdenGenerada,
                EstadoOrdenResiduosEnum.Pendiente,
                EstadoOrdenResiduosEnum.OrdenVencida
            };
            if (!estadosPermitenEdicion.Contains((EstadoOrdenResiduosEnum)ordenEntity.EstadoId))
            {
                throw new InfoCustomException("Esta orden no puede anularse en el estado actual");
            }

            ordenEntity = ordenDto.ToEntity(ordenEntity);

            if (ordenEntity.EstadoId != (int)EstadoOrdenResiduosEnum.OrdenVencida)
            {
                var existeTransporte = ExisteTransporte(ordenEntity.TransporteCuit);
                ordenEntity.EstadoId = (int)(existeTransporte ? EstadoOrdenResiduosEnum.OrdenGenerada : EstadoOrdenResiduosEnum.Pendiente);
                if (!existeTransporte)
                {
                    emailResiduosService.EnviarMailTransporteNoExiste(ordenEntity.TransporteRazonSocial, ordenEntity.TransporteCuit);
                }
            }

            repositorioResiduos.GuardarCambios();

            return new GrabarOrdenResponse { IdOrden = ordenEntity.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
        }

        public OrdenResiduosDto ObtenerOrden(int idOrden)
        {
            var ordenDto = new OrdenResiduosDto().FromEntity(repositorioResiduos.ObtenerOrdenResiduos(idOrden));

            return ordenDto;
        }

        public OrdenResiduosDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorioResiduos.ObtenerOrdenResiduos(ordenId) ?? throw new ValidationCustomException("Orden no encontrada");
            if (orden.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenEntregada)
            {
                throw new ValidationCustomException("Esta orden no puede ser anulada, ya fue entregada");
            }

            var usuario = repositorioResiduos.ObtenerUsuarioSegunMail(mailUsuario);
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaResiduosAdmin);
            if (!esInterno)
            {
                var camionEstaEnPlanta = OrdenEstaActivaEnScato(ordenId);
                if (camionEstaEnPlanta)
                {
                    emailResiduosService.EnviarMailIntentoAnulacionOrdenActiva(orden);
                    throw new ValidationCustomException("La orden no se puede anular por estar el camión en planta");
                }
            }
            orden.EstadoId = (int)EstadoOrdenResiduosEnum.Anulada;
            repositorioResiduos.GuardarCambios();

            return new OrdenResiduosDto().FromEntity(orden);
        }

        public OrdenResiduosDto VerificarTransporte(int ordenId)
        {
            var orden = repositorioResiduos.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            var existeTransporte = ExisteTransporte(orden.TransporteCuit);
            orden.EstadoId = existeTransporte ? (int)EstadoOrdenResiduosEnum.OrdenGenerada : (int)EstadoOrdenResiduosEnum.Pendiente;
            repositorioResiduos.GuardarCambios();
            return new OrdenResiduosDto().FromEntity(orden);
        }

        public void VerificarVencimientoOrdenesResiduos()
        {
            var habilitacion = repositorioResiduos.Obtener<SustitucionMOAModel.Entities.HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesResiduosJob");
            if (!habilitacion.Habilitado)
            {
                return;
            }

            var tresDiasAtras = DateTime.Today.AddDays(-3);

            var ordenes =
                repositorioResiduos.Listar<SustitucionMOAModel.Entities.OrdenResiduos>(o => o.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenGenerada
                && DbFunctions.TruncateTime(o.FechaCreacion) <= tresDiasAtras);

            foreach (var orden in ordenes)
            {
                orden.EstadoId = (int)EstadoOrdenResiduosEnum.OrdenVencida;
            }
            repositorioResiduos.GuardarCambios();

            emailResiduosService.EnviarMailOrdenesVencidas(ordenes);
        }

        public IList<DestinoScato> ObtenerDestinosMercaderia(string cuit)
        {
            var destinosScato = ObtenerDestinos(cuit);
            var destinosMercaderia = destinosScato
                .Select(x =>
                    new DestinoScato
                    {
                        LocalidadDescripcion = x.LocalidadDescripcion,
                        LocalidadId = x.LocalidadId,
                        ProvinciaDescripcion = x.ProvinciaDescripcion,
                        ProvinciaId = x.ProvinciaId,
                        KmsARecorrer = x.KmARecorrer
                    })
                .ToList();
            return destinosMercaderia;
        }

        public bool ValidarCamionEstaEnPlantaParaEditarOrden(int ordenId)
        {
            var camionEstaEnPlanta = OrdenEstaActivaEnScato(ordenId);
            if (camionEstaEnPlanta)
            {
                var orden = repositorioResiduos.ObtenerOrdenResiduos(ordenId);
                emailResiduosService.EnviarMailIntentoEdicionOrdenActiva(orden);
            }
            return camionEstaEnPlanta;
        }

        private void ValidarOrden(OrdenResiduosDto ordenDto)
        {
            if (ordenDto.Id == 0 && (ordenDto.CantidadDeViajes is null || ordenDto.CantidadDeViajes < 1 || ordenDto.CantidadDeViajes > 3))
            {
                throw new InfoCustomException("Revisar campo cantidad de viajes.");
            }
            if (!ordenDto.Producto.ValidaSisaRuca)
            {
                ordenDto.Domicilio = null;
                ordenDto.Planta = null;
            }
        }

        private bool ExisteTransporte(string cuit)
        {
            var estadoTransportista = ordenCargaConsumer.GetOrdenCargaControlEstadoTransportista(cuit);
            return estadoTransportista == SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS.ControlEstadoResEnum.TransportistaOK;
        }

        private bool EsDigitoCuilCuitValido(string cuit)
        {
            if (cuit.Length != 11)
            {
                throw new ValidationCustomException($"{cuit} no es un CUIT válido");
            }

            var basesValidacionCuit = new int[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

            var auxiliar = basesValidacionCuit
                .WithIndex()
                .Sum(x =>
                    x.item * char.GetNumericValue(cuit[x.index])
                );

            auxiliar = 11 - (auxiliar % 11);

            if (auxiliar == 11)
            {
                auxiliar = 0;
            }
            if (auxiliar == 10)
            {
                auxiliar = 9;
            }
            var ultimoDigito = char.GetNumericValue(cuit.Last());
            return auxiliar == ultimoDigito;
        }

        private bool OrdenEstaActivaEnScato(int ordenId)
        {
            var recorridoScato = scatoConsumer.ObtenerRecorridoOrdenResiduos(ordenId);
            return recorridoScato != null && !recorridoScato.Terminado;
        }

        private void CompletarDatosDestino(OrdenResiduosDto ordenDto)
        {
            ordenDto.DestinoMercaderia = ordenDto.DestinoMercaderia ?? new DestinoScato();

            if (ordenDto.Producto.ValidaSisaRuca && ordenDto.Domicilio != null)
            {
                var distanciaARecorrer = ObtenerDistanciaARecorrer(ordenDto.Domicilio.Descripcion);
                ordenDto.DestinoMercaderia.KmsARecorrer = distanciaARecorrer.HasValue ? distanciaARecorrer.ToString() : null;
            }
        }
    }
}
