using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.IO;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
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
            return JsonCustom(gestionImpuestosService.ListarCabeceras());
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult GetCombos()
        {
            return JsonCustom(new
            {
                estados = gestionImpuestosService.ListarEstados(),
                secuencias = gestionImpuestosService.ListarSecuenciaIngresosBrutosCoeficientesUnificador()
            });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult ListarDetalles(int idCabecera)
        {
            return JsonCustom(gestionImpuestosService.ListarDetalles(idCabecera));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult AutorizarCabecera(int idCabecera)
        {
            string mailUusario = SessionPersister.getUsername();
            return JsonCustom(gestionImpuestosService.AutorizarCabecera(idCabecera, mailUusario));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult EditarIngresosBrutosCoeficienteUnificadoDetalle(string detalleJson)
        {
            var detalle = JsonConvert.DeserializeObject<IngresosBrutosCoeficienteUnificadoDetalleDto>(detalleJson);
            return JsonCustom(gestionImpuestosService.EditarIngresosBrutosCoeficienteUnificadoDetalle(detalle));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult DescargarFormularioCM05(int idCabecera)
        {
            string rutaArchivo = gestionImpuestosService.ObtenerRutaArchivoFormularioCM05(idCabecera);

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivo);
            string fileName = Path.GetFileName(rutaArchivo);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult EditarIngresosBrutosCoeficienteUnificado(string cabeceraJson)
        {
            IngresosBrutosCoeficienteUnificadoDto cabecera = JsonConvert.DeserializeObject<IngresosBrutosCoeficienteUnificadoDto>(cabeceraJson);

            return JsonCustom(gestionImpuestosService.EditarIngresosBrutosCoeficienteUnificado(cabecera));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult InsertarMovimientoIngresosBrutosCoeficienteUnificado(string cabeceraJson)
        {
            MovimientoIngresosBrutosCoeficienteUnificadoCustomDto cabecera = JsonConvert.DeserializeObject<MovimientoIngresosBrutosCoeficienteUnificadoCustomDto>(cabeceraJson);
            return JsonCustom(gestionImpuestosService.InsertarMovimientoIngresosBrutosCoeficienteUnificado(cabecera));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpGet]
        public JsonResult ListarMovimientos(int idCabecera)
        {
            return JsonCustom(gestionImpuestosService.ListarMovimientos(idCabecera));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.GESTION_IMPUESTOS_CM05)]
        [HttpPost]
        public JsonResult CargarCM05()
        {
            var username = SessionPersister.getUsername();
            if (Request.Files.Count <= 0) return Json(new { info = "No se adjuntaron archivos" }, JsonRequestBehavior.AllowGet);
            return JsonCustom(new { Mensaje = consultaService.ProcesarCM05(Request.Files, username, null, true) });
        }
    }
}