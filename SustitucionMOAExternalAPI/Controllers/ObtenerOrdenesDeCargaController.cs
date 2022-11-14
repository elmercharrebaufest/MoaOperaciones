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
        private readonly IOrdenesCargaApi _ordenesCargaApi;
        public ObtenerOrdenesDeCargaController(IOrdenesCargaApi ordenesCargaApi)
        {
            _ordenesCargaApi = ordenesCargaApi;
        }
        public IHttpActionResult Get()
        {
            try
            {
                var ordenes = _ordenesCargaApi.ObtenerOrdenes();
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