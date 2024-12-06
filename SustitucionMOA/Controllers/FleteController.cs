using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Services;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class FleteController : BaseController
    {
        private FleteService _fleteService = new FleteService();

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult getViajesPendientes(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.ObtenerViajesPendientes(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult downloadViajesPendientes(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.DescargarViajesPendientes(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult getViajesAFacturar(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.ObtenerViajesAFacturar(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult downloadViajesAFacturar(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.DescargarViajesAFacturar(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult getViajesFacturados(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.ObtenerViajesFacturados(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult downloadViajesFacturados(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_fleteService.DescargarViajesFacturados(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult exportarPDFAFacturar(string periodo, string fechaInicio, string fechaFin, string proforma)
        {
            return JsonCustom(_fleteService.ExportarPDFAFacturar(SessionPersister.Proveedor, fechaInicio, fechaFin, proforma));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult exportarPDFFacturado(string periodo, string fechaInicio, string fechaFin, string proforma)
        {
            return JsonCustom(_fleteService.ExportarPDFFacturado(SessionPersister.Proveedor, fechaInicio, fechaFin, proforma));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult validarImporte(decimal importe, string proforma)
        {
            return JsonCustom(_fleteService.ValidarImporte(importe, proforma, SessionPersister.Proveedor));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_FLETE)]
        public ActionResult guardarDatosProforma(string proforma, HttpPostedFileBase file)
        {

            var pdfBytes = new byte[0];
            try
            {
                string extension = System.IO.Path.GetExtension(file.FileName);
                if (extension.ToUpper() == ".PDF")
                {
                    var streamLength = file.InputStream.Length;
                    pdfBytes = new byte[streamLength];
                    file.InputStream.Read(pdfBytes, 0, pdfBytes.Length);
                }
                else
                {
                    throw new ValidationCustomException(ErrorMsg.ErrorArchivoFormato);
                }
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch { throw new ValidationCustomException(InfoMsg.NoPDF); }
            var viajeAgrupado = JsonConvert.DeserializeObject<ViajeAgrupado>(proforma);

            return Json(_fleteService.ObtenerRelacion(viajeAgrupado.factura, viajeAgrupado.fechaEmision, viajeAgrupado.totalImporte, pdfBytes, file.FileName, viajeAgrupado.proforma, SessionPersister.Proveedor), JsonRequestBehavior.AllowGet);

        }

    }
}