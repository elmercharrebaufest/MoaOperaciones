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
		// [Authorize(Roles = "API QR CAMIONES")]
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

				var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow("Granos");

				Log.ExternalAPIInfo($"QRCamionesAPI - Search: existe TrackingDataScato: {trackingDataScato != null}, existe configuraciones: {configuraciones != null}");

				if (trackingDataScato == null || configuraciones == null)
                {
                    return Ok(new TrackingResponseDto
                    {
                        Resultado = true,
                        Mensaje = "Datos encontrados",
                        Data = null
                    });
                }                

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
		// [Authorize(Roles = "API QR CAMIONES")]
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

				var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow("Granos");

				Log.ExternalAPIInfo($"QRCamionesAPI - Search: existe TrackingDataScato: {trackingDataScato != null}, existe configuraciones: {configuraciones != null}");

				if (trackingDataScato == null || configuraciones == null)
                {
                    return Ok(new EstadoEtapasResponseDto
                    {
                        Resultado = true,
                        Mensaje = "Datos encontrados",
                        Data = null
                    });
                }                

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
            EtapaQRCamiones[] etapasArray,
            List<QRCamionesConfiguracion> configuraciones)
		{
			var result = new List<EtapaQRCamionesDto>();

			bool encontradoPrimerPendiente = false;
			EtapaQRCamionesDto ultimaEtapaCompleta = null;

			foreach (var config in configuraciones.OrderBy(c => c.Id))
			{
				// Check if etapa exists in the API list
				var etapaApi = etapasArray
					.Where(x => 
                        string.Equals(x.Nombre, config.FinEtapa)
                    )
					.OrderByDescending(e => e.Fecha)
					.FirstOrDefault();

				bool esFinDeEtapa = etapaApi != null;

				var dto = new EtapaQRCamionesDto
				{
					Nombre = config.NombreEtapa,
					TiempoEstimado = config.TiempoEstimado.ToString(),
					Fecha = etapaApi?.Fecha ?? default,
					Estado = esFinDeEtapa ? "completado" : "pendiente"
				};

				result.Add(dto);

				if (!encontradoPrimerPendiente && !esFinDeEtapa)
				{
					encontradoPrimerPendiente = true;

					if (ultimaEtapaCompleta != null) ultimaEtapaCompleta.Estado = "en-proceso";
				}

				if (esFinDeEtapa)
					ultimaEtapaCompleta = dto;
			}

			// Todas las etapas completas, ultima etapa se encuentra "en-proceso"
			if (!encontradoPrimerPendiente && ultimaEtapaCompleta != null)
				ultimaEtapaCompleta.Estado = "en-proceso";

			// Ninguna etapa esta completa, primer etapa se pone en "en-proceso"
			if (ultimaEtapaCompleta == null && result.Any())
				result.First().Estado = "en-proceso";

			return result;
		}

        #endregion
    }
}