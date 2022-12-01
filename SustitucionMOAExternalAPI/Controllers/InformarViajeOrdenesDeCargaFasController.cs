using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class InformarViajeOrdenesDeCargaFasController : ApiController
    {
        private readonly IOrdenDeCargaApiService _ordenesCargaApi;
        public InformarViajeOrdenesDeCargaFasController(IOrdenDeCargaApiService ordenesCargaApi)
        {
            _ordenesCargaApi = ordenesCargaApi;
        }

        [Authorize(Roles = "APIKEY")]
        public IHttpActionResult Post([FromBody] IngresosEgresosFas ingresosEgresosFas)
        {
            try
            {
                _ordenesCargaApi.InformarViajeOrdenesDeCargaFas(ingresosEgresosFas);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }

       
	}
}