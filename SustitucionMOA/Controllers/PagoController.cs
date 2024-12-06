using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class PagoController : BaseController
    {
        protected readonly IPDFService pDFService;
        protected readonly IPagoService pagoService;

        public PagoController(IPagoService pagoService, IPDFService pDFService)
        {
            this.pagoService = pagoService;
            this.pDFService = pDFService;
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGOS)]
        public ActionResult getEmitidos(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(pagoService.ObtenerEmitidos(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_PAGOS)]
        public ActionResult downloadEmitidos(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(pagoService.DescargarEmitidos(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGOS_NG)]
        public ActionResult getEmitidosNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(pagoService.ObtenerEmitidosNG(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGOS_NG)]
        public ActionResult getComprobantes(string documento, string fecha, string fiscalYear)
        {
            return JsonCustom(pagoService.ObtenerComprobantes(documento, fecha, SessionPersister.Sociedad, fiscalYear));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_PAGOS_NG)]
        public ActionResult downloadEmitidosNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(pagoService.DescargarEmitidosNG(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGO_DETALLE)]
        public ActionResult getDetalle(string numeroPago)
        {
            return JsonCustom(pagoService.ObtenerDetalle(SessionPersister.Proveedor, numeroPago));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGO_DETALLE)]
        public ActionResult downloadDetalle(string numeroPago)
        {
            return JsonCustom(pagoService.DescargarDetalle(SessionPersister.Proveedor, numeroPago));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_PAGO_DETALLE)]
        public ActionResult downloadDocumentPDF(string documento, string ejercicio)
        {
            return JsonCustom(pDFService.DescargarDocumentPDF(documento, ejercicio, SessionPersister.Proveedor, SessionPersister.Sociedad));
        }

    }
}