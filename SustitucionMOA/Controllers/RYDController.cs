using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class RYDController : BaseController
    {
        protected readonly IRYDService rYDService;

        public RYDController(IRYDService rYDService)
        {
            this.rYDService = rYDService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REGISTRAR_PESADA)]
        public ActionResult getDataInputsCargaPesadas()
        {
            return JsonCustom(new { data = rYDService.ObtenerDataInputsCargaPesadas(1029) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_INFORME)]
        public ActionResult getFiltrosInforme()
        {
            return Json(new { data = rYDService.ObtenerFiltrosInforme(1029) }, JsonRequestBehavior.AllowGet);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LISTADO_PESADAS)]
        public ActionResult getFiltrosListadoPesadas()
        {
            return JsonCustom(new { data = rYDService.ObtenerFiltrosListadoPesadas() });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REGISTRAR_PESADA)]
        public ActionResult registrarPesada(string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto)
        {
            return JsonCustom(new { data = rYDService.RegistrarPesada(1029, balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado, numeroPesada, fechaPesada, pesoTara, pesoBruto) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REGISTRAR_PESADA)]
        public ActionResult finalizarCargaPesadas(string balanza, string fecha)
        {
            return JsonCustom(new { data = rYDService.FinalizarCargaPesadas(1029, balanza, fecha) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REGISTRAR_PESADA)]
        public ActionResult verificarBalanza(string balanza)
        {
            return JsonCustom(new { data = rYDService.VerificarBalanzaEnProceso(1029, balanza) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_INFORME)]
        public ActionResult getInforme(string balanza)
        {
            return JsonCustom(new { data = rYDService.ObtenerInforme(1029, balanza) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LISTADO_PESADAS)]
        public ActionResult getListadoPesadas(string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            return JsonCustom(new { data = rYDService.ObtenerListadoPesadas(1029, commodity, exportador, fechaInicio, fechaFin) });
        }

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_RYD)]
        public ActionResult downloadInforme(string balanza)
        {
            return JsonCustom(rYDService.DescargarInforme(1029, balanza));
        }

        public ActionResult downloadListadoPesada(string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            return JsonCustom(rYDService.DescargarListadoPesada(1029, commodity, exportador, fechaInicio, fechaFin));
        }
    }
}