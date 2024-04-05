using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class OrdenesResiduoController: ApiController
    {
        private readonly IExternalApiOrdenesResiduosService ordenResiduoService;
        public OrdenesResiduoController(IExternalApiOrdenesResiduosService ordenesService)
        {
            ordenResiduoService = ordenesService;
        }

        [Authorize(Roles = "API ORDENES RESIDUOS")]
        [HttpGet]
        public IHttpActionResult Obtener([FromUri]string patenteChasis = null)
        {
            try
            {
                var ordenes = ordenResiduoService.ObtenerOrdenes(patenteChasis);
                return Json(ordenes);
            }
            catch(InfoCustomException ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex); ;
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }
    }
}