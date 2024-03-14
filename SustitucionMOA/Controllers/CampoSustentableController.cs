using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Logger;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class CampoSustentableController : BaseController
    {
        readonly ICampoSustentableService campoSustentableService;
        private readonly IFileWrapper fileWrapper;
        private readonly IDataAgroService dataAgroService;

        public CampoSustentableController(ICampoSustentableService campoSustentableService, IFileWrapper fileWrapper, IDataAgroService dataAgroService)
        {
            this.campoSustentableService = campoSustentableService;
            this.fileWrapper = fileWrapper;
            this.dataAgroService = dataAgroService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult CampoProveedorAgregar(string campoProveedorJson, HttpPostedFileBase archivoKmz, bool UsarArchivoId)
        {
            try
            {
                var campoProveedor = JsonConvert.DeserializeObject<CampoProveedor>(campoProveedorJson);
                return JsonCustom(campoSustentableService.Agregar(SessionPersister.User.username, campoProveedor, archivoKmz, UsarArchivoId));
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

        [HttpPost]
        public JsonResult CampoProveedorEditar(string campoProveedorJson, HttpPostedFileBase archivoKmz)
        {
            try
            {
                var campoProveedor = JsonConvert.DeserializeObject<CampoProveedor>(campoProveedorJson);
                return JsonCustom(campoSustentableService.Editar(SessionPersister.User.username, campoProveedor, archivoKmz));
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.BORRAR_CAMPOS_CREADOS)]
        [HttpPost]
        public JsonResult CampoProveedorBorrar(int campoCosechaId, int proveedorId)
        {
            try
            {
                return JsonCustom(campoSustentableService.Borrar(SessionPersister.User.username, campoCosechaId, proveedorId));
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult CampoProveedor(int proveedorId, int campoCosechaId)
        {
            try
            {
                return JsonCustom(campoSustentableService.ObtenerCampo(SessionPersister.User.username, proveedorId, campoCosechaId));
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult CamposProveedores()
        {
            try
            {
                return JsonCustom(campoSustentableService.Listar(SessionPersister.User.username));
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
                Console.Write(e);
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult ExportarCamposProveedores()
        {
            try
            {
                return JsonCustom(campoSustentableService.ExportarCamposProveedores(SessionPersister.User.username));
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
                Console.Write(e);
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Cosechas(bool incluirInactivas)
        {
            try
            {
                return JsonCustom(campoSustentableService.ObtenerCosechas(incluirInactivas));
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

        [HttpGet]
        public JsonResult VerificarDeclaracion(int proveedorId, int cosechaId, string CUITDeclaracion)
        {
            try
            {
                return JsonCustom(campoSustentableService.VerificarDeclaracion(proveedorId, cosechaId, CUITDeclaracion));
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult GenerarDeclaracionProveedor(int proveedorId, int cosechaId, double hectareasTotales, string CUITDeclaracion, string razonSocialDeclaracion)
        {
            try
            {
                var fileArray = campoSustentableService.GenerarDeclaracionProveedor(SessionPersister.User.username, proveedorId, cosechaId, hectareasTotales, CUITDeclaracion, razonSocialDeclaracion);
                PDFResponse result = new PDFResponse
                {
                    Pdf = new Pdf()
                    {
                        data = fileArray
                    }
                };

                return JsonCustom(result.Pdf);
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult AdjuntarDeclaracionFirmada(int proveedorId, int cosechaId, string CUITDeclaracion, HttpPostedFileBase fileSubido)
        {
            try
            {
                return JsonCustom(campoSustentableService.AdjuntarDeclaracionFirmada(SessionPersister.User.username, proveedorId, cosechaId, CUITDeclaracion, fileSubido));
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult ImprimirDeclaracion(int proveedorId, int cosechaId, string CUIT)
        {
            try
            {
                var fileArray = campoSustentableService.ImprimirDeclaracion(proveedorId, cosechaId, CUIT);
                PDFResponse result = new PDFResponse
                {
                    Pdf = new Pdf()
                    {
                        data = fileArray
                    }
                };

                return JsonCustom(result.Pdf);
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult DescargarArchivoKMZ(int campoCosechaId, int proveedorId)
        {
            try
            {
                string rutaArchivo = campoSustentableService.ObtenerRutaArchivoKMZ(campoCosechaId, proveedorId);

                byte[] fileBytes = fileWrapper.ReadAllBytes(rutaArchivo);
                string fileName = Path.GetFileName(rutaArchivo);
                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
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