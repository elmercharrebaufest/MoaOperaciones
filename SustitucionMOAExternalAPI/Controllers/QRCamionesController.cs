using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject.Activation;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.QRCamiones;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    [RoutePrefix("api/qrcamiones")]
    public class QRCamionesController : ApiController
    {
        private readonly IQRCamionesAPIService _qrCamionesService;
        private readonly IScatoConsumer _scatoConsumer;
		private readonly ITicketPesadaService _ticketPesadaService;

		public QRCamionesController(IQRCamionesAPIService qrCamionesService, IScatoConsumer scatoConsumer, 
			ITicketPesadaService ticketPesadaService)
		{
			_qrCamionesService = qrCamionesService;
			_scatoConsumer = scatoConsumer;
			_ticketPesadaService = ticketPesadaService;
		}

		#region GET
		[HttpGet]
		[Route("files")]
		public IHttpActionResult Files([FromUri] TrackingRequestDto request)
		{
			var listadoArchivos = _ticketPesadaService.ObtenerTicket(request);
			return JsonCamelCase(new { data = listadoArchivos });
		}

		[HttpGet]
		[Route("search")]
		[Authorize(Roles = "API QR CAMIONES")]
		public IHttpActionResult Search([FromUri] TrackingRequestDto request)
		{
			try
			{
				if (request == null || string.IsNullOrWhiteSpace(request.Ctg) || string.IsNullOrWhiteSpace(request.Patente))
				{
					return JsonCamelCase(new TrackingResponseDto
					{
						Resultado = false,
						Mensaje = "Los parámetros CTG y Patente son requeridos"
					});
				}

				Log.ExternalAPIInfo($"QRCamionesAPI - Search: CTG={request.Ctg}, Patente={request.Patente}");

				var trackingDataScato = _scatoConsumer.ObtenerTrackingDataQRCamiones(request.Ctg, request.Patente);
				var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow(request.TipoWorkflow);

				Log.ExternalAPIInfo($"QRCamionesAPI - Search: existe TrackingDataScato: {trackingDataScato != null}, existe configuraciones: {configuraciones != null}");
				Log.ExternalAPIInfo($"Existe etapas SCATO: {trackingDataScato.Etapas.Length > 0}");

				if (trackingDataScato == null || configuraciones == null)
				{
					return JsonCamelCase(new TrackingResponseDto
					{
						Resultado = true,
						Mensaje = "Datos encontrados",
						Data = null
					});
				}

				var trackingDto = ConvertirScatoTrackingDataADto(trackingDataScato, configuraciones);

				return JsonCamelCase(new TrackingResponseDto
				{
					Resultado = true,
					Mensaje = "Datos encontrados",
					Data = trackingDto
				});
			}
			catch (Exception ex)
			{
				Log.ExternalAPIError(ex);

				return JsonCamelCase(new TrackingResponseDto
				{
					Resultado = false,
					Mensaje = $"Error al obtener la informacion requerido con los datos ingresados. Comuniquese con el administrador del sistema."
				});
			}
		}

		[HttpGet]
		[Route("estadoEtapas")]
		[Authorize(Roles = "API QR CAMIONES")]
		public IHttpActionResult EstadoEtapas([FromUri] TrackingRequestDto request)
		{
			try
			{
				if (request == null || string.IsNullOrWhiteSpace(request.Ctg) || string.IsNullOrWhiteSpace(request.Patente))
				{
					return JsonCamelCase(new EstadoEtapasResponseDto
					{
						Resultado = false,
						Mensaje = "Los parámetros CTG y Patente son requeridos"
					});
				}

				Log.ExternalAPIInfo($"QRCamionesAPI - EstadoEtapas: CTG={request.Ctg}, Patente={request.Patente}");

				var trackingDataScato = _scatoConsumer.ObtenerTrackingDataQRCamiones(request.Ctg, request.Patente);
				var configuraciones = _qrCamionesService.ObtenerConfiguracionesPorTipoWorkflow(request.TipoWorkflow);

				Log.ExternalAPIInfo($"QRCamionesAPI - EstadoEtapas: existe TrackingDataScato: {trackingDataScato != null}, existe configuraciones: {configuraciones != null}");

				if (trackingDataScato == null || configuraciones == null)
				{
					return JsonCamelCase(new EstadoEtapasResponseDto
					{
						Resultado = true,
						Mensaje = "Datos encontrados",
						Data = null
					});
				}

				var estadoEtapasDto = new EstadoEtapasQRCamionesDto
				{
					DatosAdicionales = ConvertirDatosAdicionalesScatoADto(trackingDataScato.DatosAdicionales, trackingDataScato.Rechazado),
					Etapas = ConvertirEtapasScatoADto(trackingDataScato.Etapas, configuraciones)
				};

				return JsonCamelCase(new EstadoEtapasResponseDto
				{
					Resultado = true,
					Mensaje = "Datos encontrados",
					Data = estadoEtapasDto
				});
			}
			catch (Exception ex)
			{
				Log.ExternalAPIError(ex);

				return JsonCamelCase(new EstadoEtapasResponseDto
				{
					Resultado = false,
					Mensaje = $"Error al obtener la informacion requerido con los datos ingresados. Comuniquese con el administrador del sistema."
				});
			}
		}
		#endregion

		#region POST
		[HttpPost]
		[Route("log")]
		public IHttpActionResult LogIntoExternalApi([FromBody] LogRequestDto request)
		{
			try
			{
				if (request == null)
				{
					return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
				}

				if (request.IsError)
				{
					var ex = new Exception(request.Log);
					Log.ExternalAPIError(ex);
				}
				else
				{
					Log.ExternalAPIInfo($"QRCamionesAPI Info - Log: {request.Log}");
				}

				return Ok();
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}
		#endregion

		#region Metodos Privados de Conversion

		private IHttpActionResult JsonCamelCase(object data)
		{
			var jsonFormatter = new JsonMediaTypeFormatter
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				}
			};

			return Content(System.Net.HttpStatusCode.OK, data, jsonFormatter);
		}

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
                RemitenteComercialProd = trackingDataScato.RemitenteComercialProd,
                RemitenteComercialVtaPrim = trackingDataScato.RemitenteComercialVtaPrim,
                Entregador = trackingDataScato.Entregador,
                Transportista = trackingDataScato.Transportista,
                Material = trackingDataScato.Material,
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
                DatosAdicionales = ConvertirDatosAdicionalesScatoADto(trackingDataScato.DatosAdicionales, trackingDataScato.Rechazado),
                Etapas = ConvertirEtapasScatoADto(trackingDataScato.Etapas, configuraciones)
            };
        }

        private DatosAdicionalesQRCamionesDto ConvertirDatosAdicionalesScatoADto(DatosAdicionalesQRCamiones datosAdicionales, bool rechazado = false)
        {
            if (datosAdicionales == null)
            {
                return new DatosAdicionalesQRCamionesDto();
            }

            return new DatosAdicionalesQRCamionesDto
            {
				Rechazado = rechazado,
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
			int lastFoundIndex = -1;

			foreach (var config in configuraciones.OrderBy(c => c.Id))
			{
				int searchStartIndex = lastFoundIndex + 1;
				EtapaQRCamiones etapaApi = null;
				int foundIndex = -1;

				for (int i = searchStartIndex; i < etapasArray.Length; i++)
				{
					if (string.Equals(etapasArray[i].Nombre, config.FinEtapa) && 
							(EsControlRecorrido(etapasArray[i].NombreTabla, config.FinEtapaEsControlRecorrido) ||
							EsLogActividad(etapasArray[i].NombreTabla, config.FinEtapaEsControlRecorrido))
					   )
					{
						Log.ExternalAPIInfo($"Etapa: {config.NombreEtapa}, Nombre SCATO: {etapasArray[i].Nombre}, Texto FinEtapa: {config.FinEtapa}");
						Log.ExternalAPIInfo($"EsControlRecorrido: {EsControlRecorrido(etapasArray[i].NombreTabla, config.FinEtapaEsControlRecorrido)}, EsLogActividad: {EsLogActividad(etapasArray[i].NombreTabla, config.FinEtapaEsControlRecorrido)}");
						etapaApi = etapasArray[i];
						foundIndex = i;
						break;
					}
				}

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
					dto.Estado = "en-proceso";
					dto.Fecha = DateTime.Now;
				}

				if (esFinDeEtapa)
				{
					ultimaEtapaCompleta = dto;
					lastFoundIndex = foundIndex;
				}
			}

			// Ninguna etapa esta completa, primer etapa se pone en "en-proceso"
			if (ultimaEtapaCompleta == null && result.Any())
				result.First().Estado = "en-proceso";

			return result;
		}

		private bool EsControlRecorrido(string nombreTabla, bool provieneDeControlRecorrido)
		{
			return nombreTabla == "ControlRecorrido" && provieneDeControlRecorrido;
		}

		private bool EsLogActividad(string nombreTabla, bool provieneDeControlRecorrido)
		{
			return nombreTabla == "LogActividad" && !provieneDeControlRecorrido;
		}

		#endregion
	}
}