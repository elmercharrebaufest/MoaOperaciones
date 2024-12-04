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
    public class CuentaCorrienteController : BaseController
    {
        readonly ICuentaCorrienteService cuentaCorrienteService;

        public CuentaCorrienteController(ICuentaCorrienteService cartaPorteService)
        {
            this.cuentaCorrienteService = cartaPorteService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CUENTA_CORRIENTE)]
        public ActionResult GetCuentasCorrientes(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            return JsonCustom(cuentaCorrienteService.GetCuentasCorrientes(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CUENTA_CORRIENTE)]
        public ActionResult GetCuentasCorrientesAgrupadas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            return JsonCustom(cuentaCorrienteService.GetCuentasCorrientesAgrupadas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientes(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            return JsonCustom(cuentaCorrienteService.DownloadCuentaCorrientes(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientesAgrupadas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            return JsonCustom(cuentaCorrienteService.DownloadCuentaCorrientesAgrupadas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CUENTA_CORRIENTE)]
        public ActionResult DownloadCuentasCorrientesPartidasAbiertas(string periodo, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            return JsonCustom(cuentaCorrienteService.DownloadCuentasCorrientesPartidasAbiertas(SessionPersister.Proveedor, SessionPersister.Sociedad, fechaInicio, fechaFin, contrato, pago, retencion));
        }
    }
}