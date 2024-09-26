using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ScatoWS = SustitucionMOAWS.ScatoWebService;

namespace SustitucionMOAUtils.Services
{
    public class OrdenResiduosService : IOrdenResiduosService
    {
        private readonly IRepositorioOrdenResiduos repositorio;
        private readonly IScatoRepositorioClient scatoRepositorioClient;
        private readonly IOrdenCargaConsumerMOA ordenCargaConsumer;
        private readonly IFeriadoService feriadoService;
        private readonly IEmailResiduosService emailResiduosService;
        private readonly IScatoConsumer scatoConsumer;

        public OrdenResiduosService(
            IRepositorioOrdenResiduos repositorio,
            IScatoRepositorioClient scatoRepositorioClient,
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IFeriadoService feriadoService,
            IEmailResiduosService emailResiduosService,
            IScatoConsumer scatoConsumer)
        {
            this.repositorio = repositorio;
            this.scatoRepositorioClient = scatoRepositorioClient;
            this.ordenCargaConsumer = ordenCargaConsumer;
            this.feriadoService = feriadoService;
            this.emailResiduosService = emailResiduosService;
            this.scatoConsumer = scatoConsumer;
        }

        public List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientes()
        {
            return repositorio.ObtenerClientesResiduos();
        }

        public SustitucionMOAModel.Dto.ProveedorDto ObtenerProveedor(int idProveedor)
        {
            var proveedor = repositorio.ObtenerProveedor(idProveedor) ??
                throw new Exception("No se encontró el proveedor con ID " + idProveedor);
            return new SustitucionMOAModel.Dto.ProveedorDto(proveedor);
        }

        public MaterialDto[] ObtenerMateriales()
        {
            return repositorio.ObtenerMateriales();
        }

        public ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicioStr, string fechaFinStr)
        {
            var fechaInicio = DataFormatter.StringToDateTime(fechaInicioStr, "");
            var fechaFin = DataFormatter.StringToDateTime(fechaFinStr, "");

            var listado = repositorio.ObtenerListadoOrdenes(fechaInicio, fechaFin);

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
            return repositorio.ObtenerLocalidades();
        }

        public PatentesClienteDto ObtenerPatentes(int clienteId)
        {
            return repositorio.ObtenerPatentesDeOrdenes(clienteId);
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
            return repositorio.ObtenerIdsTransportes(clienteId, patenteAcoplado);
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
            var producto = repositorio.Obtener<Material>(ordenDto.Producto.MaterialId);
            var localidad = ObtenerLocalidadDeLaOrden(ordenDto, producto);
            for (int i = 0; i < ordenDto.CantidadDeViajes; i++)
            {
                var ordenEntity = ordenDto.ToEntity();
                ordenEntity.DomicilioDescr = ordenEntity.DomicilioDescr.Substring(ordenEntity.DomicilioDescr.IndexOf(" ") + 1);
                ordenEntity.LocalidadScatoId = localidad.Id;
                ordenEntity.LocalidadScatoDescripcion = localidad.LocalidadDescripcion;
                ordenEntity.KmsARecorrer = localidad.KmARecorrer;
                ordenEntity.FechaCreacion = DateTime.Now;
                repositorio.Agregar(ordenEntity);
            }

            repositorio.GuardarCambios();

            if (!existeTransporte)
            {
                emailResiduosService.EnviarMailTransporteNoExiste(ordenDto.RazonSocialTransporte, ordenDto.CUITTransporte);
            }

            return new GrabarOrdenResponse { Mensaje = SuccessMsg.OrdenDeCargaAgregada };
        }

        public GrabarOrdenResponse EditarOrden(OrdenResiduosDto ordenDto, string mailUsuario)
        {
            Log.Info($"Editar orden residuos: [{ordenDto.ToJson()}], usuario: [{mailUsuario}]");
            ValidarOrden(ordenDto);

            var ordenEntity = repositorio.ObtenerOrdenResiduos(ordenDto.Id);

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

            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            if (!usuario.TieneRol(RolEnum.ResiduosAdmin))
            {
                ordenEntity.EstadoId = (int)EstadoOrdenResiduosEnum.EdicionSolicitada;
            }
            else
            {
                if (ordenEntity.EstadoId != (int)EstadoOrdenResiduosEnum.OrdenVencida)
                {
                    var existeTransporte = ExisteTransporte(ordenEntity.TransporteCuit);
                    ordenEntity.EstadoId = (int)(existeTransporte ? EstadoOrdenResiduosEnum.OrdenGenerada : EstadoOrdenResiduosEnum.Pendiente);
                }
            }

            repositorio.GuardarCambios();

            return new GrabarOrdenResponse { IdOrden = ordenEntity.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
        }

        public OrdenResiduosDto ObtenerOrden(int idOrden)
        {
            var ordenDto = new OrdenResiduosDto().FromEntity(repositorio.ObtenerOrdenResiduos(idOrden));

            return ordenDto;
        }

        public OrdenResiduosDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorio.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            if (orden.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenEntregada)
            {
                throw new InfoCustomException("Esta orden no puede ser anulada, ya fue entregada");
            }

            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            if (!usuario.TieneRol(RolEnum.ResiduosAdmin))
            {
                throw new InfoCustomException("Usuario sin permiso para realizar esta acción");
            }

            orden.EstadoId = (int)EstadoOrdenResiduosEnum.Anulada;
            repositorio.GuardarCambios();

            return new OrdenResiduosDto().FromEntity(orden);
        }

        public OrdenResiduosDto SolicitarAnulacion(int ordenId, string mailUsuario)
        {
            var orden = repositorio.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            var estadosPermitenAnulacion = new EstadoOrdenResiduosEnum[]
            {
                EstadoOrdenResiduosEnum.OrdenGenerada,
                EstadoOrdenResiduosEnum.Pendiente,
                EstadoOrdenResiduosEnum.OrdenVencida
            };
            if (!estadosPermitenAnulacion.Contains((EstadoOrdenResiduosEnum)orden.EstadoId))
            {
                throw new InfoCustomException("Esta orden no puede anularse en el estado actual");
            }
            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            if (usuario.TieneRol(RolEnum.ResiduosAdmin))
            {
                throw new InfoCustomException("Usuario sin permiso para realizar esta acción");
            }

            orden.EstadoId = (int)EstadoOrdenResiduosEnum.AnulacionSolicitada;
            repositorio.GuardarCambios();

            return new OrdenResiduosDto().FromEntity(orden);
        }

        public OrdenResiduosDto ActualizarSolicitudAnulacion(int ordenId, string mailUsuario, bool aprobarSolicitud)
        {
            var orden = repositorio.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            if (orden.EstadoId != (int)EstadoOrdenResiduosEnum.AnulacionSolicitada)
            {
                throw new InfoCustomException("Esta orden no está en estado de anulación solicitada");
            }

            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            if (!usuario.TieneRol(RolEnum.ResiduosAdmin))
            {
                throw new InfoCustomException("Usuario sin permiso para realizar esta acción");
            }

            if (aprobarSolicitud)
            {
                orden.EstadoId = (int)EstadoOrdenResiduosEnum.Anulada;
            }
            else
            {
                var existeTransporte = ExisteTransporte(orden.TransporteCuit);
                orden.EstadoId = existeTransporte ? (int)EstadoOrdenResiduosEnum.OrdenGenerada : (int)EstadoOrdenResiduosEnum.Pendiente;
            }
            repositorio.GuardarCambios();
            return new OrdenResiduosDto().FromEntity(orden);
        }

        public OrdenResiduosDto ActualizarSolicitudEdicion(int ordenId, string mailUsuario, bool aprobarSolicitud)
        {
            var orden = repositorio.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            if (orden.EstadoId != (int)EstadoOrdenResiduosEnum.EdicionSolicitada)
            {
                throw new InfoCustomException("Esta orden no está en estado de edición solicitada");
            }

            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            if (!usuario.TieneRol(RolEnum.ResiduosAdmin))
            {
                throw new InfoCustomException("Usuario sin permiso para realizar esta acción");
            }

            if (aprobarSolicitud)
            {
                var existeTransporte = ExisteTransporte(orden.TransporteCuit);
                orden.EstadoId = existeTransporte ? (int)EstadoOrdenResiduosEnum.OrdenGenerada : (int)EstadoOrdenResiduosEnum.Pendiente;
            }
            else
            {
                orden.EstadoId = (int)EstadoOrdenResiduosEnum.EdicionRechazada;
            }
            repositorio.GuardarCambios();
            return new OrdenResiduosDto().FromEntity(orden);
        }

        public OrdenResiduosDto VerificarTransporte(int ordenId)
        {
            var orden = repositorio.ObtenerOrdenResiduos(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            var existeTransporte = ExisteTransporte(orden.TransporteCuit);
            orden.EstadoId = existeTransporte ? (int)EstadoOrdenResiduosEnum.OrdenGenerada : (int)EstadoOrdenResiduosEnum.Pendiente;
            repositorio.GuardarCambios();
            return new OrdenResiduosDto().FromEntity(orden);
        }

        public void VerificarVencimientoOrdenesResiduos()
        {
            var fechaActual = DateTime.Now;
            var dayOfWeek = fechaActual.DayOfWeek;
            if ((dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday))
            {
                return;
            }
            var feriados = feriadoService.ObtenerFeriados();
            if (feriados.Any(feriado => feriado.Date == fechaActual.Date))
            {
                return;
            }

            var habilitacion = repositorio.Obtener<SustitucionMOAModel.Entities.HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesResiduosJob");
            if (!habilitacion.Habilitado)
            {
                return;
            }

            var ordenes =
                repositorio.Listar<SustitucionMOAModel.Entities.OrdenResiduos>(o => o.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenGenerada)
                .Where(orden => orden.FechaVencimiento(feriados) < fechaActual);

            foreach (var orden in ordenes)
            {
                orden.EstadoId = (int)EstadoOrdenResiduosEnum.OrdenVencida;
            }
            repositorio.GuardarCambios();

            emailResiduosService.EnviarMailOrdenesVencidas(ordenes);
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
                .Sum(x => x.item * char.GetNumericValue(cuit[x.index]));

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

        private void LogMensajesScato(RespuestaScatoBase respuestaScato)
        {
            foreach (var err in respuestaScato.Messages)
            {
                Log.Info($"Error Scato código {err.MessageCode}, descripción: {err.Message}");
            }
        }
        private List<ScatoWS.KmPorProveedorDto> ObtenerDestinos(string cuit)
        {
            if (cuit.Length != 11)
            {
                throw new ValidationCustomException("El cuit no tiene el formato correcto.");
            }
            return scatoConsumer.BuscarDestinos(cuit);
        }

        private ScatoWS.KmPorProveedorDto ObtenerLocalidadDeLaOrden(OrdenResiduosDto request, Material producto)
        {
            var localidades = producto.EsDerivadoGranario ?
                ObtenerDestinos(request.Cliente.CUIT) :
                ObtenerDestinos(request.Cliente.CUIT);

            if (localidades.Count == 0)
            {
                throw new ValidationCustomException("El cliente/destino no cuenta con ninguna localidad, imposible continuar con la carga.");
            }
            return localidades.First();
        }
    }
}
