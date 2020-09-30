using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class PesificacionController : BaseController
    {
        PesificacionService _pesificacionService = new PesificacionService();

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult GetFechaPesificacion()
        {            
            try
            {
                return JsonCustom(_pesificacionService.GetFechaPesificacion("yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobante(string contrato)
        {
            try
            {
                var contratoJson = JsonConvert.DeserializeObject<ContratoContenido>(contrato);

                return JsonCustom(_pesificacionService.SetContrato(SessionPersister.Proveedor, contratoJson.Contrato, contratoJson.Fijacion, contratoJson.Cantidad));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobantes(HttpPostedFileBase file)
        {
            try
            {
                return JsonCustom(_pesificacionService.SetContratos(SessionPersister.Proveedor, file));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult GetContratos()
        {
            try
            {


                return JsonCustom(_pesificacionService.GetContratos(SessionPersister.Proveedor));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}