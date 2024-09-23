using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class OrdenesResiduosController: ApiController
    {
        private readonly IExternalApiOrdenesResiduosService ordenResiduoService;
        public OrdenesResiduosController(IExternalApiOrdenesResiduosService ordenesService)
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
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }

        [Authorize(Roles = "API ORDENES RESIDUOS")]
        [Route("external/api/ActualizarOrdenesResiduos")]
        [HttpPatch]
        public IHttpActionResult Actualizar([FromBody] ActualizarOrdenResiduosExternalDto datos)
        {
            try
            {
                ordenResiduoService.ActualizarOrden(datos);
                return Json(new { data = true });
            }
            catch (InfoCustomException ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }
        [Authorize(Roles = "API ORDENES RESIDUOS")]
        [Route("external/api/InformarViajeOrdenesResiduos")]
        [HttpPatch]
        public IHttpActionResult InformarViaje([FromBody] IngresosEgresosResiduos ingresosEgresosFasones)
        {
            try
            {
                ordenResiduoService.InformarViaje(ingresosEgresosFasones);
                return Json(new { data = true });
            }
            catch (InfoCustomException ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
                return InternalServerError(new Exception("Hubo un error al procesar la solicitud"));
            }
        }
    }
}