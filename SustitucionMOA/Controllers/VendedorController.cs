using SustitucionMOA.Utils;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class VendedorController : BaseController
    {
        private readonly IVendedorService _vendedorService;

        public VendedorController(IVendedorService vendedorService)
        {
            _vendedorService = vendedorService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_DATOS_FISCALES)]
        public ActionResult GetDatoFiscales(string vendedor)
        {
            if (vendedor == "" || vendedor == null)
            {
                vendedor = SessionPersister.Proveedor;
            }
            else
            {
                return JsonCustom(new { data = _vendedorService.GetDatosFiscales(vendedor, vendedor) });
            }
            return JsonCustom(new { data = _vendedorService.GetDatosFiscales(vendedor, SessionPersister.Proveedor) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDORES)]
        public ActionResult GetVendedores(string fechaInicio, string fechaFin)
        {
            return JsonCustom(new { data = _vendedorService.GetVendedores(SessionPersister.User.username, SessionPersister.Proveedor, fechaInicio, fechaFin) });
        }

        [HttpGet]
        public ActionResult GetAllClientsByType(int tipoProveedorId)
        {
            var response = _vendedorService.GetAllClientsByType(tipoProveedorId);
            return JsonCustom(response);
        }

        public ActionResult AutocompleteProveedores(string fechaInicio, string fechaFin, int tipoProveedorId)
        {
            var username = SessionPersister.User.username;
            var proveedor = SessionPersister.Proveedor;
            return JsonCustom(new { data = _vendedorService.AutocompleteProveedores(username, proveedor, fechaInicio, fechaFin, tipoProveedorId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDOR_STATUS)]
        public ActionResult GetVendedorStatus(string cuit)
        {
            cuit = cuit.Replace("-", "");
            return JsonCustom(new { data = _vendedorService.GetVendedorStatus(cuit, SessionPersister.Proveedor) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDOR_STATUS)]
        public ActionResult GetVariosVendedoresStatus(string cuitStr)
        {
            cuitStr = cuitStr.Replace("-", "");
            List<string> cuitVendedores = cuitStr.Split(',').ToList();

            return JsonCustom(new { data = _vendedorService.GetVariosVendedoresStatus(cuitVendedores, SessionPersister.Proveedor) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDOR_PENDIENTES)]
        public ActionResult GetVendedoresPendientes()
        {
            string mailUsuario = ClaimsPrincipalExtension.GetClaimValue("emails");
            var data = _vendedorService.GetVendedoresPendientes(mailUsuario, SessionPersister.Proveedor);
            foreach (var item in data)
            {
                if (item.EstadoAprobacion == EstadoAprobacion.AprobacionPendiente
                            || item.EstadoAprobacion == EstadoAprobacion.AnalisisDeNosis
                            || item.EstadoAprobacion == EstadoAprobacion.EtapaFinal)
                {
                    item.EstadoAprobacionDescripcion = "Alta en Gestión";
                }
            }
            return JsonCustom(new { data = data });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDOR_PENDIENTES)]
        public ActionResult AgregarVendedor(string cuit, int tipoProveedor)
        {

            string mailUsuario = ClaimsPrincipalExtension.GetClaimValue("emails");

            return JsonCustom(new { data = _vendedorService.AgregarVendedor(mailUsuario, cuit, tipoProveedor) });

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDOR_PENDIENTES)]
        public ActionResult EliminarVendedor(int proveedorId)
        {
            string mailUsuario = ClaimsPrincipalExtension.GetClaimValue("emails");

            return JsonCustom(new { data = _vendedorService.EliminarVendedor(mailUsuario, proveedorId) });
        }
    }
}