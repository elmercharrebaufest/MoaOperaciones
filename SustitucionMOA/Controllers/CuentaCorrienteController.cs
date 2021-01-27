using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class CuentaCorrienteController : BaseController
    {
        readonly ICuentaCorrienteService cuentaCorrienteService;

        public CuentaCorrienteController(ICuentaCorrienteService cartaPorteService)
        {
            this.cuentaCorrienteService = cartaPorteService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CUENTA_CORRIENTE)]
        public ActionResult GetCuentasCorrientes(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                return JsonCustom(cuentaCorrienteService.GetCuentasCorrientes(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CUENTA_CORRIENTE)]
        public ActionResult GetCuentasCorrientesAgrupadas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                return JsonCustom(cuentaCorrienteService.GetCuentasCorrientesAgrupadas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientes(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                return JsonCustom(cuentaCorrienteService.DownloadCuentaCorrientes(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientesAgrupadas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                return JsonCustom(cuentaCorrienteService.DownloadCuentaCorrientesAgrupadas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientesPartidasAbiertas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                return JsonCustom(cuentaCorrienteService.DownloadCuentasCorrientesPartidasAbiertas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}