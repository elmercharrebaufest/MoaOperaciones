using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class LiquidacionController : BaseController
    {
        private readonly ILiquidacionService _liquidacionService;

        public LiquidacionController(ILiquidacionService liquidacionService)
        {
            _liquidacionService = liquidacionService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult getAprobadas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getAprobadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult getObservadas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getObservadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult getPagas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getPagas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getAprobadasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getAprobadasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getComprobantesNG(string proveedor, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getComprobantesNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getObservadasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getObservadasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getRegistradosNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getRegistradosNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getPendienteRegistroNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getPendienteRegistroNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES_NG)]
        public ActionResult getPagasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.getPagasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES)]
        public ActionResult downloadAprobadas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadAprobadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES)]
        public ActionResult downloadObservadas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadObservadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES)]
        public ActionResult downloadPagas(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadPagas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES_NG)]
        public ActionResult downloadAprobadasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadAprobadasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }



        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES_NG)]
        public ActionResult downloadObservadasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadObservadasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES_NG)]
        public ActionResult downloadRegistradosNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadRegistradosNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        public ActionResult downloadPendienteRegistroNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadPendienteRegistroNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_LIQUIDACIONES_NG)]
        public ActionResult downloadPagasNG(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_liquidacionService.downloadPagasNG(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult getVinculacion(string contrato, string secuencia)
        {
            return JsonCustom(new { data = _liquidacionService.getVinculacion(SessionPersister.Proveedor, contrato, secuencia) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult downloadVinculacion(string contrato, string secuencia)
        {
            return JsonCustom(_liquidacionService.descargaVinculacion(SessionPersister.Proveedor, contrato, secuencia));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult getProforma(string fijacion)
        {
            return JsonCustom(new { data = _liquidacionService.getProforma(SessionPersister.Proveedor, fijacion) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult descargarProforma(string fijacion)
        {
            return JsonCustom(_liquidacionService.descargaProforma(SessionPersister.Proveedor, fijacion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult descargarProformaFinal(string fijacion)
        {
            //Formato de Fijación entrante: 0[nro de contrato][nro de pedido]. La RFC espera dos parametros: [nro de contrato] (sin ceros) y como fijación: [nro de contrato][nro de pedido]
            return JsonCustom(_liquidacionService.descargaProformaFinal(fijacion.Substring(1, fijacion.Length - 3), fijacion.Substring(1, fijacion.Length - 1)));
        }

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult descargaComprobantesNG(string CodigoProveedorSAP, string FechaDocumento, string NumeroLegalDocumento)
        {
            return JsonCustom(_liquidacionService.descargaComprobantesNG(CodigoProveedorSAP, FechaDocumento, NumeroLegalDocumento).Pdf);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult getFleteProcedencia(string contrato)
        {
            return JsonCustom(_liquidacionService.getFleteProcedencia(SessionPersister.Proveedor, contrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CONTRATO_DETALLE)]
        public ActionResult descargarFleteProcedencia(string contrato)
        {
            return JsonCustom(_liquidacionService.descargarFleteProcedencia(SessionPersister.Proveedor, contrato));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public async Task<ActionResult> notificar()
        {
            try
            {
                await _liquidacionService.NotificarLiquidacionesAsync(Request.Files, SessionPersister.Proveedor);

                return Json(new { data = SuccessMsg.ArchivoSubidoOK }, JsonRequestBehavior.AllowGet);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult getInformadas()
        {
            return Json(new { data = _liquidacionService.GetLiquidacionInformadas(SessionPersister.Proveedor) }, JsonRequestBehavior.AllowGet);
        }
    }
}