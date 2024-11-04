using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Util;
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
                Log.ExternalAPIInfo("InformarViajeOrdenesDeCargaFas: " + ingresosEgresosFas.ToJson());
                var result = _ordenesCargaApi.InformarViajeOrdenesDeCargaFas(ingresosEgresosFas);
                if (result.Errores.Count > 0)
                {
                    var ex = new Exception(string.Format("Se dieron los siguientes errores al procesar los elementos: {0}", string.Join(" | ", result.Errores.Select(a => a.Message).ToArray())));
                    Log.ExternalAPIError(ex);
                    return InternalServerError(ex);
                }
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