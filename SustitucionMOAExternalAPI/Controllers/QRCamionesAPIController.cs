using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities.QRCamiones;
using SustitucionMOAUtils.Interfaces.QRCamiones;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    [RoutePrefix("api/qrcamiones")]
    public class QRCamionesAPIController : ApiController
    {
        private readonly IQRCamionesAPIService _qrCamionesService;
        private readonly IScatoConsumer _scatoConsumer;

        public QRCamionesAPIController(IQRCamionesAPIService qrCamionesService, IScatoConsumer scatoConsumer)
        {
            _qrCamionesService = qrCamionesService;
            _scatoConsumer = scatoConsumer;
        }

        [HttpGet]
        [Route("search")]
        // [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult Search([FromUri] TrackingRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Ctg) || string.IsNullOrWhiteSpace(request.Patente))
                {
                    return Ok(new TrackingResponseDto
                    {
                        Resultado = false,
                        Mensaje = "Los parámetros CTG y Patente son requeridos"
                    });
                }

                Log.ExternalAPIInfo($"QRCamionesAPI - Search: CTG={request.Ctg}, Patente={request.Patente}");

                var trackingDataScato = _scatoConsumer.ObtenerTrackingDataQRCamiones(request.Ctg, request.Patente);

                if (trackingDataScato == null)
                {
                    return Ok(new TrackingResponseDto
                    {
                        Resultado = true,
                        Mensaje = "Datos encontrados",
                        Data = null
                    });
                }

                var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow("Granos");

                var trackingDto = ConvertirScatoTrackingDataADto(trackingDataScato, configuraciones);

                return Ok(new TrackingResponseDto
                {
                    Resultado = true,
                    Mensaje = "Datos encontrados",
                    Data = trackingDto
                });
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                
                return Ok(new TrackingResponseDto
                {
                    Resultado = false,
                    Mensaje = $"Error al obtener los datos: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("estadoEtapas")]
        // [Authorize(Roles = "ABM SOLP")]
        public IHttpActionResult EstadoEtapas([FromUri] TrackingRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Ctg) || string.IsNullOrWhiteSpace(request.Patente))
                {
                    return Ok(new EstadoEtapasResponseDto
                    {
                        Resultado = false,
                        Mensaje = "Los parámetros CTG y Patente son requeridos"
                    });
                }

                Log.ExternalAPIInfo($"QRCamionesAPI - EstadoEtapas: CTG={request.Ctg}, Patente={request.Patente}");

                var trackingDataScato = _scatoConsumer.ObtenerTrackingDataQRCamiones(request.Ctg, request.Patente);

                if (trackingDataScato == null)
                {
                    return Ok(new EstadoEtapasResponseDto
                    {
                        Resultado = true,
                        Mensaje = "Datos encontrados",
                        Data = null
                    });
                }

                var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow("Granos");

				var estadoEtapasDto = new EstadoEtapasQRCamionesDto
                {
                    DatosAdicionales = ConvertirDatosAdicionalesScatoADto(trackingDataScato.DatosAdicionales),
                    Etapas = ConvertirEtapasScatoADto(trackingDataScato.Etapas, configuraciones)
                };

                return Ok(new EstadoEtapasResponseDto
                {
                    Resultado = true,
                    Mensaje = "Datos encontrados",
                    Data = estadoEtapasDto
                });
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                
                return Ok(new EstadoEtapasResponseDto
                {
                    Resultado = false,
                    Mensaje = $"Error al obtener los datos: {ex.Message}"
                });
            }
        }

        #region Metodos Privados de Conversion

        private TrackingDataDto ConvertirScatoTrackingDataADto(
            TrackingDataQRCamiones trackingDataScato,
            List<QRCamionesConfiguracion> configuraciones)
        {
            return new TrackingDataDto
            {
                Workflow = trackingDataScato.Workflow,
                Ctg = trackingDataScato.CTG,
                FechaHoraIngreso = trackingDataScato.FechaHoraIngreso,
                TitularCartaPorte = trackingDataScato.TitularCartaPorte,
                RemitenteComercial = trackingDataScato.RemitenteComercial,
                RemitenteComercialVtaPrim = trackingDataScato.RemitenteComercialVtaPrim,
                Entregador = trackingDataScato.Entregador,
                Transportista = trackingDataScato.Transportista,
                Material = trackingDataScato.Material,
                Rechazado = trackingDataScato.Rechazado,
                Camion = new CamionQRCamionesDto
                {
                    Patente = trackingDataScato.Camion?.Patente,
                    PatenteAcoplado = trackingDataScato.Camion?.PatenteAcoplado
                },
                Chofer = new ChoferQRCamionesDto
                {
                    Cuil = trackingDataScato.Chofer?.CUIL,
                    TipoDocumento = trackingDataScato.Chofer?.TipoDocumento,
                    NumeroDocumento = trackingDataScato.Chofer?.NumeroDocumento,
                    Extranjero = trackingDataScato.Chofer?.Extranjero.ToString() ?? "false",
                    NombreApellido = trackingDataScato.Chofer?.NombreApellido
                },
                DatosAdicionales = ConvertirDatosAdicionalesScatoADto(trackingDataScato.DatosAdicionales),
                Etapas = ConvertirEtapasScatoADto(trackingDataScato.Etapas, configuraciones)
            };
        }

        private DatosAdicionalesQRCamionesDto ConvertirDatosAdicionalesScatoADto(DatosAdicionalesQRCamiones datosAdicionales)
        {
            if (datosAdicionales == null)
            {
                return new DatosAdicionalesQRCamionesDto();
            }

            return new DatosAdicionalesQRCamionesDto
            {
                PreCaladoFila = datosAdicionales.PreCaladoFila,
                PostCaladoFila = datosAdicionales.PostCaladoFila,
                CaladoEstado = datosAdicionales.CaladoEstado,
                PesadaBruto = datosAdicionales.PesadaBruto,
                PesadaTara = datosAdicionales.PesadaTara,
                PesadaDescargado = datosAdicionales.PesadaDescargado
            };
        }

        private List<EtapaQRCamionesDto> ConvertirEtapasScatoADto(
            EtapaQRCamiones[] etapas,
            List<QRCamionesConfiguracion> configuraciones)
        {
            var etapasDto = new List<EtapaQRCamionesDto>();

            if (etapas == null || etapas.Length == 0)
            {
                return etapasDto;
            }

            foreach (var etapa in etapas)
            {
                var config = configuraciones?.FirstOrDefault(c =>
                    c.NombreEtapa.Equals(etapa.Nombre, StringComparison.OrdinalIgnoreCase));

                etapasDto.Add(new EtapaQRCamionesDto
                {
                    Nombre = etapa.Nombre,
                    Fecha = etapa.Fecha,
                    TiempoEstimado = config?.TiempoEstimado.ToString() ?? etapa.TiempoEstimado,
                    Estado = ConvertirEstadoEtapa(etapa.Estado)
                });
            }

            return etapasDto;
        }

        private string ConvertirEstadoEtapa(EstadoEtapaQRCamiones estado)
        {
            switch (estado)
            {
                case EstadoEtapaQRCamiones.Completado:
                    return "completado";
                case EstadoEtapaQRCamiones.EnProceso:
                    return "en-proceso";
                case EstadoEtapaQRCamiones.Pendiente:
                    return "pendiente";
                default:
                    return "pendiente";
            }
        }

        #endregion
    }
}