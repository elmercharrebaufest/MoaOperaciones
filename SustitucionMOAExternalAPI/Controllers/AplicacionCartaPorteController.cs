using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class AplicacionCartaPorteController : ApiController
    {
        private readonly IAplicacionCartaPorteApiService aplicacionCartaPorteApiService;

        public AplicacionCartaPorteController(IAplicacionCartaPorteApiService aplicacionCartaPorteApiService)
        {
            this.aplicacionCartaPorteApiService = aplicacionCartaPorteApiService;
        }

        [HttpGet]
        [Authorize(Roles = "APIKEY")]
        public IHttpActionResult ObtenerAplicacionesAProcesar()
        {
            try
            {
                var aplicaciones = aplicacionCartaPorteApiService.ObtenerAplicacionesAProcesar();
                return Json(aplicaciones);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud", ex));
            }
        }

        [HttpPut]
        [Authorize(Roles = "APIKEY")]
        public IHttpActionResult ActualizarEstadoAplicacion(int id, int nuevoEstado, string error)
        {
            try
            {
                aplicacionCartaPorteApiService.ActualizarEstadoAplicacion(id, nuevoEstado, error);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud", ex));
            }
        }
    }
}