using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using ScatoWS = SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOARepositorio;
using CNRTModel = SustitucionMOAModel.Models.WebApiMap.CNRT;
using ModelScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;

namespace SustitucionMOAUtils.Services
{
    public abstract class OrdenDeCargaServiceBase : IOrdenDeCargaServiceBase
    {
        protected readonly IOrdenCargaConsumerMOA ordenCargaConsumer;
        protected readonly IScatoConsumer scatoConsumer;
        protected readonly IScatoRepositorioClient scatoRepositorioClient;
        protected readonly IFeriadoService feriadoService;
        protected readonly IRepositorio repositorio;
        protected readonly ICNRTClient cNRTClient;
        private readonly IUbicacionGeograficaService ubicacionGeograficaService;

        protected OrdenDeCargaServiceBase(
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IScatoRepositorioClient scatoRepositorioClient,
            IRepositorio repositorio,
            ICNRTClient cNRTClient,
            IFeriadoService feriadoService,
            IUbicacionGeograficaService ubicacionGeograficaService
            )
        {
            this.scatoConsumer = scatoConsumer;
            this.ordenCargaConsumer = ordenCargaConsumer;
            this.scatoRepositorioClient = scatoRepositorioClient;
            this.repositorio = repositorio;
            this.cNRTClient = cNRTClient;
            this.feriadoService = feriadoService;
            this.ubicacionGeograficaService = ubicacionGeograficaService;
        }

        public ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit)
        {
            var clientesScato = scatoConsumer.ObtenerClientesPorCuit(cuit);
            var clientesValidos = clientesScato.Where(c => !c.Bloqueado && c.Activo).ToArray();

            return new ValidarCuitExisteScatoResponse
            {
                Existe = clientesValidos.Length > 0,
                RazonSocial = clientesValidos.Length > 0 ? clientesValidos.First().Descripcion : ""
            };
        }

        public bool ValidarSisa(string cuitDestinatario, string cuitDestino, string codigoMaterial)
        {
            if (string.IsNullOrEmpty(cuitDestinatario) && string.IsNullOrEmpty(cuitDestino))
            {
                return false;
            }
            var controlarCargaReq = new ControlCargaRequest
            {
                SoloSisa = true,
                Material = codigoMaterial,
                CuitDestinatario = cuitDestinatario,
                CuitDestino = cuitDestino
            };
            var responseHandler = ordenCargaConsumer.ControlarCarga(controlarCargaReq);
            if (!string.IsNullOrEmpty(cuitDestinatario))
            {
                return !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinatarioInhabilitadoEnSisa);
            }
            else
            {
                return !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinoInhabilitadoEnSisa);
            }
        }

        public List<PlantaDto> ObtenerPlantasDestino(string destinoCuit)
        {
            var plantasRes = scatoRepositorioClient.ObtenerPlantas(destinoCuit);
            if (!plantasRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Scato con CUIT " + destinoCuit);
                foreach (var err in plantasRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                if (plantasRes.Messages.Any(mensaje => mensaje.MessageType == 1 &&
                    mensaje.Message == "800 - No existen solicitudes para los parámetros indicados."))
                {
                    throw new InfoCustomException("Sin plantas habilitadas.");
                }
                throw new ValidationCustomException("Error al obtener Plantas.");
            }
            else
            {
                return plantasRes.Data
                    .Select(x =>
                        new PlantaDto
                        {
                            Actividad = x.Actividad,
                            Codigo = x.NroPlanta
                        })
                    .ToList();
            }
        }

        public List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit)
        {
            var domiciliosRes = scatoRepositorioClient.ObtenerDomicilios(destinoCuit);
            if (!domiciliosRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Domicilios con CUIT " + destinoCuit);
                foreach (var err in domiciliosRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                throw new ValidationCustomException("Error al obtener Domicilios");
            }
            else
            {
                return domiciliosRes.Data
                    .Select(x =>
                        new DomicilioDto
                        {
                            Descripcion = x.Descripcion,
                            Orden = x.Orden,
                            Tipo = x.Tipo
                        })
                    .ToList();
            }
        }

        public bool ValidarCuitRuca(string cuit)
        {
            var tienePlanta = ObtenerPlantasDestino(cuit).Any();
            if (tienePlanta)
            {
                var tieneDomicilio = ObtenerDomiciliosDestino(cuit).Any();
                return tieneDomicilio;
            }
            else
            {
                return false;
            }
        }

        public ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit)
        {
            var scatoRes = scatoRepositorioClient.ObtenerProveedorPorCuil(cuit);
            if (scatoRes.IsValid)
            {
                return new ValidarIntermediarioFleteResponse
                {
                    EsCuitValido = true,
                    ExisteIntermediario = true,
                    RazonSocial = scatoRes.Data.RazonSocial
                };
            }

            var response = new ValidarIntermediarioFleteResponse();
            if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.DigitoVerificadorNoValido))
            {
                response.EsCuitValido = false;
            }
            else
            {
                if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.ProveedorNoEncontrado))
                {
                    response.EsCuitValido = true;
                    response.ExisteIntermediario = false;
                }
                else
                {
                    throw new Exception("Error en ValidarIntermediarioFlete. Validación inesperada con cuit " + cuit);
                }
            }
            return response;
        }

        public (bool, ScatoRepo.Chofer) ValidarCuilChofer(string cuilChofer)
        {
            var choferRes = scatoRepositorioClient.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer));
            var chofer = choferRes.Data;
            var cuitValido = ValidarDigitoCuit(cuilChofer);
            if (!choferRes.IsValid)
            {
                Log.Info("Error al obtener chofer de Scato " + cuilChofer);
                LogMensajesScato(choferRes);
            }
            return (cuitValido, chofer);
        }

        public bool ValidarCuilChoferDigito(string cuilChofer)
        {
            return ValidarCuilChofer(cuilChofer).Item1;
        }

        public bool ValidarCuitTransporteDigito(string cuitTransporte)
        {
            return ValidarCuitTransporte(cuitTransporte).Item1;
        }

        public ProveedorDto ObtenerProveedor(int idProveedor)
        {
            var proveedor = repositorio.Obtener<Proveedor>(idProveedor) ??
                throw new Exception("No se encontró el proveedor con ID " + idProveedor);
            return new ProveedorDto(proveedor);
        }

        public ValidarCamionResponse ValidarCamion(string patenteChasis, string patenteAcoplado)
        {
            try
            {
                var cnrtResponse = cNRTClient.ObtenerEquipos(patenteChasis, patenteAcoplado);
                var dominios = cnrtResponse.Data.Dominios;

                if (dominios == null || !dominios.Any())
                {
                    return new ValidarCamionResponse { ExisteCamion = false };
                }

                var tipoVehiculo = cnrtResponse.TipoVehiculoCNRTSegunCategoriaEscalado;

                if (dominios.Any(d => d.Ruta == null || d.Ruta.CantEjes <= 0) && (
                        tipoVehiculo == null ||
                        tipoVehiculo == CNRTModel.TipoVehiculoCNRT.CamionBitren))
                {
                    return new ValidarCamionResponse { ExisteCamion = false };
                }
                else
                {
                    return new ValidarCamionResponse
                    {
                        ExisteCamion = true,
                        EsCamionEscalable = (
                            tipoVehiculo == CNRTModel.TipoVehiculoCNRT.CamionC ||
                            tipoVehiculo == CNRTModel.TipoVehiculoCNRT.CamionD ||
                            tipoVehiculo == CNRTModel.TipoVehiculoCNRT.CamionE)
                    };
                }
            }
            catch (InfoCustomException ice) { throw ice; }
            catch (ValidationCustomException vce) { throw vce; }
            catch (Exception ex)
            {
                Log.Error($"Error al validar camión con patentes: {patenteChasis} y {patenteAcoplado}.", ex);
                throw;
            }
        }

        protected DateTime CalcularFechaVencimiento(DateTime fechaOrigen)
        {
            var feriados = feriadoService.ObtenerFeriados();
            return CalcularFechaVencimiento(fechaOrigen, feriados);
        }

        protected DateTime CalcularFechaVencimiento(DateTime fechaOrigen, List<DateTime> feriados)
        {
            var dayOfWeek = fechaOrigen.DayOfWeek;
            var cantidadDiasDeMargen = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Thursday) ? 4 : 2;

            var fechaFinal = fechaOrigen.AddDays(cantidadDiasDeMargen);

            foreach (var fechaFeriado in feriados)
            {
                if (fechaFeriado.DayOfWeek != DayOfWeek.Saturday &&
                    fechaFeriado.DayOfWeek != DayOfWeek.Sunday &&
                    fechaFeriado.Date >= fechaOrigen &&
                    fechaFeriado.Date <= fechaFinal)
                {
                    cantidadDiasDeMargen++;
                }
            }

            var fechaVencimiento = fechaOrigen.AddDays(cantidadDiasDeMargen);
            return fechaVencimiento;
        }

        protected void LogMensajesScato(ModelScatoRepo.RespuestaScatoBase respuestaScato)
        {
            foreach (var err in respuestaScato.Messages)
            {
                Log.Info($"Error Scato código {err.MessageCode}, descripción: {err.Message}");
            }
        }

        protected bool ValidarDigitoCuit(string cuit)
        {
            if (cuit.Length != 11)
            {
                throw new ValidationCustomException($"La CUIT/CUIL {cuit} no tiene un formato válido");
            }

            var BASES_VALIDACION_CUIT = new int[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

            var auxiliar = BASES_VALIDACION_CUIT
                .WithIndex()
                .Sum(b =>
                    b.item * Char.GetNumericValue(cuit[b.index])
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
            var ultimoDigito = Char.GetNumericValue(cuit.Last());
            return auxiliar == ultimoDigito;
        }

        protected List<ScatoWS.KmPorProveedorDto> ObtenerDestinos(string cuit)
        {
            if (cuit.Length != 11)
            {
                throw new ValidationCustomException($"CUIT {cuit} no tiene el formato correcto.");
            }
            return scatoConsumer.BuscarDestinos(cuit);
        }

        protected int? ObtenerDistanciaARecorrer(string domicilioDescripcion)
        {
            if (string.IsNullOrEmpty(domicilioDescripcion))
            {
                return null;
            }
            var distanciaDomicilio = ubicacionGeograficaService.ObtenerDistanciaDePlantaMoaADestino(domicilioDescripcion);
            return distanciaDomicilio?.DistanciaKm;
        }

        private (bool, ScatoRepo.Chofer) ValidarCuitTransporte(string cuitTransporte)
        {
            var transporteRes = scatoRepositorioClient.ObtenerTransportePorCuit(DataFormatter.CuitConGuion(cuitTransporte));
            var transporte = transporteRes.Data;
            var cuitValido = ValidarDigitoCuit(cuitTransporte);
            if (!transporteRes.IsValid)
            {
                Log.Info("Error al obtener transporte de Scato " + cuitTransporte);
                LogMensajesScato(transporteRes);
            }
            return (cuitValido, transporte);
        }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
           => self.Select((item, index) => (item, index));
    }

}
