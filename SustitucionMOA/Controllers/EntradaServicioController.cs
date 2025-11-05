using System;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class EntradaServicioController : BaseController
    {
        private readonly IEntradaServicioService EntradaServicioService;
        private readonly IUsuarioService usuarioService;

        public EntradaServicioController(IEntradaServicioService entradaServicio, IUsuarioService usuarioService)
        {
            this.EntradaServicioService = entradaServicio;
            this.usuarioService = usuarioService;
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.Mail;
            return usuarioService.GetUsuario(userMail);
        }

        //[ValidateInput(false)]
        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
        public async Task<ActionResult> GetByProveedorAsync(EntradaServicioParamsDto parametros)
        {

            // Este debe combinarse con permisos de usuario.
            //if (parametros.vendedor == "" || parametros.vendedor == null)
            //{
            //    parametros.vendedor = SessionPersister.Proveedor;
            //}
            UsuarioDto usuarioActual = ObtenerUsuarioActual();
            List<EntradaServicioCabeceraDto> result = await EntradaServicioService.ObtenerEntradasServicioCompleta(parametros, usuarioActual);

            return JsonCustom(new { data = result });
        }

        /// <summary>
        /// Servicio para obtener las entradas de servicios guardadas en aprobaciones.
        /// </summary>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public ActionResult ObtenerESLocales(EntradaServicioParamsDto parametros)
        {
            UsuarioDto usuarioActual = ObtenerUsuarioActual();
            List<EntradaServicioCabeceraDto> result = EntradaServicioService.ServicioAprobaciones_EntradasServicioCabecera(parametros, usuarioActual);

            return JsonCustom(new { data = result });
        }


        public ActionResult DeleteById(EntradaServicioParamsDto parametros)
        {
            // Este debe combinarse con permisos de usuario.
            //if (parametros.vendedor == "" || parametros.vendedor == null)
            //{
            //    parametros.vendedor = SessionPersister.Proveedor;
            //}
            UsuarioDto usuarioActual = ObtenerUsuarioActual();
            string result = EntradaServicioService.BorrarEntradaServicio(parametros, usuarioActual);

            return JsonCustom(new { data = result });
        }

        [ValidateInput(false)]
        public ActionResult CrearEntradaServicio(string request)
        {
            try
            {
                var payload = JsonConvert.DeserializeObject<CreateEntradaServicioDto>(request);
                var mailUsuario = ClaimsPrincipalExtension.GetClaimValue("emails");

                var response = EntradaServicioService.CrearEntradaServicio(payload, mailUsuario);

                // Respuesta exitosa
                return JsonCustom(new { success = true, data = response });
            }
            catch (SustitucionMOAModel.CustomExceptions.ValidationCustomException vex)
            {
                // Error de validación conocido -> 400 Bad Request
                Response.StatusCode = 400;
                return JsonCustom(new { success = false, error = vex.Message });
            }
            catch (Exception ex)
            {
                // Error inesperado -> 500 Internal Server Error
                Response.StatusCode = 500;
                return JsonCustom(new { success = false, error = "Ocurrió un error al crear la entrada de servicio." });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RechazarEntradaDeServicio(string json)
        {
            var motivoRechazo = JsonConvert.DeserializeObject<EmailDetailCertificateDto>(json);
            var toRet = EntradaServicioService.RechazarEntradaDeServicio(motivoRechazo);
            return JsonCustom(new { data = toRet });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> AprobarEntradaDeServicio(string nro_es_local, string Moneda)
        {
            var result = await EntradaServicioService.AprobarEntradaDeServicio(nro_es_local, Moneda);
            return JsonCustom(new { data = result });
        }

        public ActionResult ReasignarSuplente(string nro_es_local, string suplente)
        {
            var result = EntradaServicioService.ReasignarSuplente(nro_es_local, suplente, true);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult ActualizarInformacionIngresante(IngresanteInfoEditableDto info)
        {
            var result = EntradaServicioService.ActualizarInformacionIngresante(info);
            return JsonCustom(new { data = result });
        }
    }
}