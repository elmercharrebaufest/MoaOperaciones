using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class GestionImpuestosController : BaseController
    {
        private readonly IGestionImpuestosService gestionImpuestosService;
        private readonly IConsultaService consultaService;

        public GestionImpuestosController(IGestionImpuestosService gestionImpuestosService, IConsultaService consultaService)
        {
            this.gestionImpuestosService = gestionImpuestosService;
            this.consultaService = consultaService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult ListarCabeceras()
        {
            try
            {
                return JsonCustom(gestionImpuestosService.ListarCabeceras());
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult GetCombos()
        {
            try
            {
                return JsonCustom(new 
                { 
                    estados = gestionImpuestosService.ListarEstados(),
                    secuencias = gestionImpuestosService.ListarSecuenciaIngresosBrutosCoeficientesUnificador()
                });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult ListarDetalles(int idCabecera)
        {
            try
            {
                return JsonCustom(gestionImpuestosService.ListarDetalles(idCabecera));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult AutorizarCabecera(int idCabecera)
        {
            try
            {
                string mailUusario = SessionPersister.getUsername();
                return JsonCustom(gestionImpuestosService.AutorizarCabecera(idCabecera, mailUusario));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult EditarIngresosBrutosCoeficienteUnificadoDetalle(string detalleJson)
        {
            try
            {
                var detalle = JsonConvert.DeserializeObject<IngresosBrutosCoeficienteUnificadoDetalleDto>(detalleJson);

                return JsonCustom(gestionImpuestosService.EditarIngresosBrutosCoeficienteUnificadoDetalle(detalle));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult DescargarFormularioCM05(int idCabecera)
        {
            try
            {
                string rutaArchivo = gestionImpuestosService.ObtenerRutaArchivoFormularioCM05(idCabecera);

                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivo);
                string fileName = Path.GetFileName(rutaArchivo);
                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult EditarIngresosBrutosCoeficienteUnificado(string cabeceraJson)
        {
            try
            {
                IngresosBrutosCoeficienteUnificadoDto cabecera = JsonConvert.DeserializeObject<IngresosBrutosCoeficienteUnificadoDto>(cabeceraJson);

                return JsonCustom(gestionImpuestosService.EditarIngresosBrutosCoeficienteUnificado(cabecera));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult InsertarMovimientoIngresosBrutosCoeficienteUnificado(string cabeceraJson)
        {
            try
            {
                MovimientoIngresosBrutosCoeficienteUnificadoCustomDto cabecera = JsonConvert.DeserializeObject<MovimientoIngresosBrutosCoeficienteUnificadoCustomDto>(cabeceraJson);

                return JsonCustom(gestionImpuestosService.InsertarMovimientoIngresosBrutosCoeficienteUnificado(cabecera));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult ListarMovimientos(int idCabecera)
        {
            try
            {
                return JsonCustom(gestionImpuestosService.ListarMovimientos(idCabecera));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult CargarCM05()
        {
            try
            {
                var username = SessionPersister.getUsername();
                if (Request.Files.Count <= 0) return Json(new { info = "No se adjuntaron archivos" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new { Mensaje = consultaService.ProcesarCM05(Request.Files, username, null, true) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}