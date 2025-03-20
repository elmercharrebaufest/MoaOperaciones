using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class CampoSustentableController : BaseController
    {
        readonly ICampoSustentableService campoSustentableService;
        private readonly IFileWrapper fileWrapper;

        public CampoSustentableController(ICampoSustentableService campoSustentableService, IFileWrapper fileWrapper)
        {
            this.campoSustentableService = campoSustentableService;
            this.fileWrapper = fileWrapper;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult CampoProveedorAgregar(string campoProveedorJson, HttpPostedFileBase archivoKmz, bool UsarArchivoId)
        {
            var campoProveedor = JsonConvert.DeserializeObject<CampoProveedor>(campoProveedorJson);
            return JsonCustom(campoSustentableService.Agregar(SessionPersister.User.username, campoProveedor, archivoKmz, UsarArchivoId));
        }

        [HttpPost]
        public JsonResult CampoProveedorEditar(string campoProveedorJson, HttpPostedFileBase archivoKmz)
        {
            var campoProveedor = JsonConvert.DeserializeObject<CampoProveedor>(campoProveedorJson);
            return JsonCustom(campoSustentableService.Editar(SessionPersister.User.username, campoProveedor, archivoKmz));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.BORRAR_CAMPOS_CREADOS)]
        [HttpPost]
        public JsonResult CampoProveedorBorrar(int campoCosechaId, int proveedorId)
        {
            return JsonCustom(campoSustentableService.Borrar(SessionPersister.User.username, campoCosechaId, proveedorId));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult CampoProveedor(int proveedorId, int campoCosechaId)
        {
            return JsonCustom(campoSustentableService.ObtenerCampo(SessionPersister.User.username, proveedorId, campoCosechaId));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult CamposProveedores()
        {
            return JsonCustom(campoSustentableService.Listar(SessionPersister.User.username));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult ExportarCamposProveedores()
        {
            return JsonCustom(campoSustentableService.ExportarCamposProveedores(SessionPersister.User.username));
        }

        [HttpGet]
        public JsonResult Cosechas(bool incluirInactivas)
        {
            return JsonCustom(campoSustentableService.ObtenerCosechas(incluirInactivas));
        }

        [HttpGet]
        public JsonResult VerificarDeclaracion(int proveedorId, int cosechaId, string CUITDeclaracion)
        {
            return JsonCustom(campoSustentableService.VerificarDeclaracion(proveedorId, cosechaId, CUITDeclaracion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult GenerarDeclaracionProveedor(int proveedorId, int cosechaId, double hectareasTotales, string CUITDeclaracion, string razonSocialDeclaracion)
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public JsonResult AdjuntarDeclaracionFirmada(int proveedorId, int cosechaId, string CUITDeclaracion, HttpPostedFileBase fileSubido)
        {
            return JsonCustom(campoSustentableService.AdjuntarDeclaracionFirmada(SessionPersister.User.username, proveedorId, cosechaId, CUITDeclaracion, fileSubido));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult ImprimirDeclaracion(int proveedorId, int cosechaId, string CUIT)
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult DescargarArchivoKMZ(int campoCosechaId, int proveedorId)
        {
            string rutaArchivo = campoSustentableService.ObtenerRutaArchivoKMZ(campoCosechaId, proveedorId);
            byte[] fileBytes = fileWrapper.ReadAllBytes(rutaArchivo);
            string fileName = Path.GetFileName(rutaArchivo);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        [CustomPermisoAuthorize(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public JsonResult RenspaExiste(string renspa, string cuit, int cosechaId)
        {
            if (string.IsNullOrWhiteSpace(renspa)) { throw new ArgumentNullException(nameof(renspa), "El RENSPA es requerido."); }
            if (string.IsNullOrWhiteSpace(cuit)) { throw new ArgumentNullException(nameof(cuit), "El CUIT es requerido."); }

            CampoCosecha discardUnderscoreIsNotAvailable;
            return JsonCustom(campoSustentableService.RenspaExiste(renspa, cuit, cosechaId, out discardUnderscoreIsNotAvailable));
        }

        [CustomPermisoAuthorize(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public ActionResult ObtenerSugerenciaCamposNuevaCosecha(int proveedorId, int cosechaId, string cuitTitularCP)
        {
            var response = new SustitucionMOAApiResponse<List<SugerenciaCampoDto>>();
            response.Data = campoSustentableService.ObtenerSugerenciaCamposNuevaCosecha(proveedorId, cosechaId, cuitTitularCP);
            return ContentCustom(response);
        }

        [CustomPermisoAuthorize(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpGet]
        public ActionResult ExportarCamposSugeridos(int proveedorId, int cosechaId, string cuitTitularCP)
        {
            var response = new SustitucionMOAApiResponse<string> { Data = campoSustentableService.ExportarCamposSugeridos(proveedorId, cosechaId, cuitTitularCP) };
            return ContentCustom(response);
        }

        [CustomPermisoAuthorize(Roles = Permiso.ABM_CAMPOS_SUSTENTABLE)]
        [HttpPost]
        public ActionResult GuardarSugerenciasCamposNuevaCosecha(string camposJson, List<HttpPostedFileBase> archivosKmz)
        {
            var campos = JsonConvert.DeserializeObject<List<SugerenciaCampoDto>>(camposJson);
            var mailUsuario = SessionPersister.User.username;
            campoSustentableService.AgregarCamposSugeridos(campos, archivosKmz, mailUsuario);
            return ContentCustom(new SustitucionMOAApiResponse<string> { Data = "Los campos se han guardado correctamente" });
        }
    }
}
