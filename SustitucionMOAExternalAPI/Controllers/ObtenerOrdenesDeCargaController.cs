using SustitucionMOAModel.Dto;
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
    public class ObtenerOrdenesDeCargaController : ApiController
    {
        private readonly IOrdenDeCargaApiService _ordenesCargaApi;
        public ObtenerOrdenesDeCargaController(IOrdenDeCargaApiService ordenesCargaApi)
        {
            _ordenesCargaApi = ordenesCargaApi;
        }

        [Authorize(Roles = "API ORDENES DE CARGA")]
        public IHttpActionResult Get(string patenteChasis = null, bool fason = true, bool fas = false)
        {
            try
            {
                var ordenes = _ordenesCargaApi.ObtenerOrdenes(patenteChasis, fason, fas);
                return Json(ordenes);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }


    }
}