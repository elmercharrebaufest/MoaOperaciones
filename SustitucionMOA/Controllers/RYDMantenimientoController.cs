using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOACustomException;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class RYDMantenimientoController : BaseController
    {
        RYDMantenimientoService _rydMantenimientoService = new RYDMantenimientoService();

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult getInputDropDown()
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getInputDropDown(1029) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorCargaDropdown }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult getCommodities()
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getDataInputsCommoditie() });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorCargaDropdown }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult getExportador()
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getDataInputsExportador() });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorCargaDropdown }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult getFiltrosNroPuesto(int itcID)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getFiltrosNroPuesto(1029, itcID) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorCargaDropdown }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult getFiltrosExportador(string exportador)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getFiltrosExportador(exportador) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult getFiltrosCommodities(string commoditie)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.getFiltrosCommodities(commoditie) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult buscarBalanza(string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.buscarBalanza(1029, "", descripcion, tipoId, codigoCabezalId, codigoSAP) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult guardarBalanza(string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.guardarBalanza(1029, codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult actualizarBalanza(string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.actualizarBalanza(1029, codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult borrarBalanza(string codigo)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.borrarBalanza(1029, codigo) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult guardarCommodity(string materialSAP, string almacenOrigen, string descripcion)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.guardarCommodity(materialSAP, almacenOrigen, descripcion) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult actualizarCommodity(string materialSAP, string almacenOrigen, string descripcion, string commodityId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.actualizarCommodity(materialSAP, almacenOrigen, descripcion, commodityId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult borrarCommodity(string commodityId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.borrarCommodity(commodityId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult guardarExportador(string almacenSAP, string descripcion)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.guardarExportador(almacenSAP, descripcion) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult actualizarExportador(string almacenSAP, string descripcion, string exportadorId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.actualizarExportador(almacenSAP, descripcion, exportadorId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult borrarExportador(string exportadorId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.borrarExportador(exportadorId) });
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult busqueda()
        //{
        //        return Json(new { data = _rydMantenimientoService.busqueda() }, JsonRequestBehavior.AllowGet);
        //}

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult aplicar(string codigoId)
        {
            try
            {
                return JsonCustom(new { data = _rydMantenimientoService.buscarBalanzaAplicar(1029, codigoId, "", "", "", "") });
            }
            catch (ValidationCustomException e)
            {
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