using SustitucionMOAModel.Dto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class ComunicacionController : BaseController
    {
        readonly IComunicacionService comunicacionService;
        //private readonly ILiquidacionService liquidacionService;
        private readonly IUsuarioService usuarioService;

        public ComunicacionController(IComunicacionService comunicacionService, IUsuarioService usuarioService)
        {
            this.comunicacionService = comunicacionService;
            this.usuarioService = usuarioService;
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        public ActionResult GetAllByProveedor(string vendedor, string fechaInicio, string fechaFin)
        {
            if (vendedor == "" || vendedor == null)
            {
                vendedor = SessionPersister.Proveedor;
            }

            UsuarioDto usuarioActual = ObtenerUsuarioActual();
            bool obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);

            comunicacionService.ActualizarComunicacionesPorProveedor(vendedor, SessionPersister.Proveedor, fechaInicio, fechaFin, usuarioActual.Id, obtenerTodos);

            return JsonCustom(new { data = comunicacionService.ObtenerComunicacionesPorProveedor(vendedor, SessionPersister.Proveedor, fechaInicio, fechaFin, usuarioActual.Id, obtenerTodos) });
        }


        //[ValidateInput(false)]
        public ActionResult PostComunicacionLeida(ComunicacionListaIdDto comunicacionIds)
        {
            return JsonCustom(new { data = comunicacionService.GrabarComunicacionComoLeida(comunicacionIds) });
        }

        public ActionResult PostComunicacionNoLeida(ComunicacionListaIdDto comunicacionIds)
        {
            return JsonCustom(new { data = comunicacionService.GrabarComunicacionComoNoLeida(comunicacionIds) });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_DATOS_FISCALES)]
        public ActionResult GetCM05(string vendedor)
        {
            if (vendedor == "" || vendedor == null)
            {
                vendedor = SessionPersister.Proveedor;
            }
            return JsonCustom(new { data = comunicacionService.ProcesarCM05(vendedor, SessionPersister.Proveedor) });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_DATOS_FISCALES)]
        public ActionResult GetCuentasHabilitadas(string vendedor)
        {
            if (vendedor == "" || vendedor == null)
            {
                vendedor = SessionPersister.Proveedor;
            }
            return JsonCustom(new { data = comunicacionService.ProcesarCuentasHabilitadas(vendedor, SessionPersister.Proveedor) });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult GetLiquidacionesObservadas(string vendedor, string fechaInicio, string fechaFin)
        {
            return JsonCustom(comunicacionService.ProcesarLiquidacionesObservadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
        }


    }

}