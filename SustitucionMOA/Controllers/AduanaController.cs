using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class AduanaController : BaseController
    {
        private readonly IDBService dBService;
        private readonly IAduanaService aduanaService;

        public AduanaController(IAduanaService aduanaService, IDBService dBService)
        {
            this.aduanaService = aduanaService;
            this.dBService = dBService;
        }



        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PESADAS)]
        public ActionResult getPesada(int centro, string fechaInicio, string fechaFin)
        {
            try
            {
                return JsonCustom(dBService.SqlSPReporte(centro, fechaInicio, fechaFin));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PESADA_DETALLE)]
        public ActionResult getPesadaDetalle(int centro, int nroOrden)
        {
            return JsonCustom(dBService.SqlSPDeltallePesada(centro, nroOrden));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CAMARAS_CONSOLIDACION)]
        public ActionResult obtenerImagenCamaraConsolidacion(string url, string nombre)
        {
            try
            {
                return JsonCustom(aduanaService.ObtenerImagen(url, nombre));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message, nombre = nombre }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message, nombre = nombre }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error, nombre = nombre }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}