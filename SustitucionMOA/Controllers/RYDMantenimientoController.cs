using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class RYDMantenimientoController : BaseController
    {
        private readonly IRYDMantenimientoService mantenimientoService;

        public RYDMantenimientoController(IRYDMantenimientoService mantenimientoService)
        {
            this.mantenimientoService = mantenimientoService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult getInputDropDown()
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerInputDropDown(1029) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult getCommodities()
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerDataInputsCommoditie() });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult getExportador()
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerDataInputsExportador() });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult getFiltrosNroPuesto(int itcID)
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerFiltrosNroPuesto(1029, itcID) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult getFiltrosExportador(string exportador)
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerFiltrosExportador(exportador) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult getFiltrosCommodities(string commoditie)
        {
            return JsonCustom(new { data = mantenimientoService.ObtenerFiltrosCommodities(commoditie) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult buscarBalanza(string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            return JsonCustom(new { data = mantenimientoService.BuscarBalanza(1029, "", descripcion, tipoId, codigoCabezalId, codigoSAP) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult guardarBalanza(string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            return JsonCustom(new { data = mantenimientoService.GuardarBalanza(1029, codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult actualizarBalanza(string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            return JsonCustom(new { data = mantenimientoService.ActualizarBalanza(1029, codigo, descripcion, automatico, toleria, centroEmisor, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult borrarBalanza(string codigo)
        {
            return JsonCustom(new { data = mantenimientoService.BorrarBalanza(1029, codigo) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult guardarCommodity(string materialSAP, string almacenOrigen, string descripcion)
        {
            return JsonCustom(new { data = mantenimientoService.GuardarCommodity(materialSAP, almacenOrigen, descripcion) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult actualizarCommodity(string materialSAP, string almacenOrigen, string descripcion, string commodityId)
        {
            return JsonCustom(new { data = mantenimientoService.ActualizarCommodity(materialSAP, almacenOrigen, descripcion, commodityId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_COMMODITIES)]
        public ActionResult borrarCommodity(string commodityId)
        {
            return JsonCustom(new { data = mantenimientoService.BorrarCommodity(commodityId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult guardarExportador(string almacenSAP, string descripcion)
        {
            return JsonCustom(new { data = mantenimientoService.GuardarExportador(almacenSAP, descripcion) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult actualizarExportador(string almacenSAP, string descripcion, string exportadorId)
        {
            return JsonCustom(new { data = mantenimientoService.ActualizarExportador(almacenSAP, descripcion, exportadorId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EXPORTADORES)]
        public ActionResult borrarExportador(string exportadorId)
        {
            return JsonCustom(new { data = mantenimientoService.BorrarExportador(exportadorId) });
        }

        //public ActionResult busqueda()
        //{
        //        return Json(new { data = mantenimientoService.busqueda() }, JsonRequestBehavior.AllowGet);
        //}

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_BALANZAS)]
        public ActionResult aplicar(string codigoId)
        {
            return JsonCustom(new { data = mantenimientoService.BuscarBalanzaAplicar(1029, codigoId, "", "", "", "") });
        }
    }
}