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
    [Authorize]
    public class ContratoController : BaseController
    {
        private readonly IContratoService contratoService;

        private const string _fijaciones = "FIJ";
        private const string _ampliaciones = "AMP";
        private const string _anulaciones = "ANU";

        public ContratoController(IContratoService contratoService)
        {
            this.contratoService = contratoService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO)]
        public ActionResult getVigentes(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.ObtenerVigentes(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO)]
        public ActionResult getFijaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.ObtenerContratosNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, new List<string>() { }, _fijaciones, "Fijaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO)]
        public ActionResult getAmpliaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.ObtenerContratosNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, new List<string>() { }, _ampliaciones, "Ampliaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO)]
        public ActionResult getAnulaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.ObtenerContratosNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, new List<string>() { }, _anulaciones, "Anulaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult getDetalle(string numeroContrato)
        {
            return JsonCustom(contratoService.ObtenerDetalleContrato(SessionPersister.Proveedor, numeroContrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult getDetalleFijacion(string numeroContrato, string fijacion)
        {
            return JsonCustom(contratoService.ObtenerDetalleFijacion(SessionPersister.Proveedor, numeroContrato, fijacion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult downloadBoletoFisico(string numeroContrato)
        {
            return JsonCustom(contratoService.DescargarBoletoFisico(SessionPersister.Proveedor, numeroContrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult downloadDetalle(string numeroContrato)
        {
            return JsonCustom(contratoService.DescargarDetalle(SessionPersister.Proveedor, numeroContrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult exportPDFCalidad(string numeroContrato)
        {
            return JsonCustom(contratoService.DescargarPDFCalidad(SessionPersister.Proveedor, numeroContrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CONTRATO)]
        public ActionResult downloadVigentes(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.DescargarVigentes(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CONTRATO)]
        public ActionResult downloadFijaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.DescargarNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, _fijaciones, "Reporte Fijaciones", "Fijaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CONTRATO)]
        public ActionResult downloadAmpliaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.DescargarNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, _ampliaciones, "Reporte Ampliaciones", "Ampliaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CONTRATO)]
        public ActionResult downloadAnulaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(contratoService.DescargarNoCumplidos(SessionPersister.Proveedor, fechaInicio, fechaFin, _anulaciones, "Reporte Anulaciones", "Anulaciones"));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult downloadDetalleFijacion(string numeroContrato, string fijacion)
        {
            return JsonCustom(contratoService.DescargarDetalleFijacion(SessionPersister.Proveedor, numeroContrato, fijacion));
        }

    }
}