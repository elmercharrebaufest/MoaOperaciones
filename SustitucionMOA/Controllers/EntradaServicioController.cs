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
using SustitucionMOAModel.Enums;
using SustitucionMOAUtils.Extensions;

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
        /// Servicio para obtener las entradas de servicios.
        /// </summary>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public ActionResult ObtenerESLocales(EntradaServicioParamsDto parametros)
        {
            UsuarioDto usuarioActual = ObtenerUsuarioActual();
            List<EntradaServicioCabeceraDto> result = EntradaServicioService.ServicioAprobaciones_EntradasServicioCabecera(parametros, usuarioActual);

            return JsonCustom(new { data = result });
        }

        public async Task<ActionResult> ListarEntradaServicio(EntradaServicioParamsDto parametros)
        {
            var result = new List<EntradaServicioCabeceraDto>();
            UsuarioDto usuarioActual = ObtenerUsuarioActual();

            if (parametros.Estado == EstadoCertificacionEnum.Aprobada.GetDescription())
            {
                result = await EntradaServicioService.ObtenerESAprobadasSAP(parametros, usuarioActual);
            }
            else
            {
                result = EntradaServicioService.ObtenerESLocales(parametros, usuarioActual);
            }
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
            SustitucionMOAWS.Logger.Log.Info("EntradaServicioController.CrearEntradaServicio");

            var payload = JsonConvert.DeserializeObject<CreateEntradaServicioDto>(request);

            var mailUsuario = ClaimsPrincipalExtension.GetClaimValue("emails");

            var response = EntradaServicioService.CrearEntradaServicio(payload, mailUsuario);

            return JsonCustom(new { data = response });
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