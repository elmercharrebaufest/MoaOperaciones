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
            string userMail = SessionPersister.getUsername();
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


        //public ActionResult Create(EntradaServicioCreateParamsDto parametros)
        //{
        //    try
        //    {
        //        // Este debe combinarse con permisos de usuario.
        //        //if (parametros.vendedor == "" || parametros.vendedor == null)
        //        //{
        //        //    parametros.vendedor = SessionPersister.Proveedor;
        //        //}

        //        string result = EntradaServicioService.CrearEntradaServicio(parametros);

        //        return JsonCustom(new { data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonCustom(new { error = ex.Message });
        //    }
        //}

        [ValidateInput(false)]
        public async Task<ActionResult> CreateAsync(string request)
        {
            SustitucionMOAWS.Logger.Log.Info("EntradaServicioController.CreateAsync");

            var payload = JsonConvert.DeserializeObject<CreateEntradaServicioDto>(request);

            // Este debe combinarse con permisos de usuario.
            //if (parametros.vendedor == "" || parametros.vendedor == null)
            //{
            //    parametros.vendedor = SessionPersister.Proveedor;
            //}
            //MMSN-601
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
            List<EntradaServicioCreateRespuestaDto> ret = new List<EntradaServicioCreateRespuestaDto>();


            foreach (EntradaServicioCreateParamsDto posicion in payload.Posiciones)
            {
                string solpedNumber = posicion.EntrySheetHeader.SolPedNumber;

                var validacion = EntradaServicioService.ValidarIngresante(posicion, userMail, solpedNumber);
                var result = new EntradaServicioCreateRespuestaDto();
                if (validacion.Message == "Auto")
                {
                    result = await EntradaServicioService.CrearEntradaServicioAsync(posicion, userMail, payload.report, payload.IdAdjuntos, solpedNumber, posicion.EntrySheetHeader.Proveedor);
                }
                else if (validacion.Message == "Temporal")
                {
                    result = EntradaServicioService.CrearEntradaServicioTemporal(posicion, userMail, payload.report, payload.IdAdjuntos, solpedNumber, posicion.EntrySheetHeader.Proveedor);
                }
                else
                {
                    result = validacion;
                }
                ret.Add(result);
            }


            return JsonCustom(new { data = ret });
        }

        [HttpPost]
        public ActionResult RechazarEntradaDeServicio(string json)
        {
            var motivoRechazo = JsonConvert.DeserializeObject<EmailDetailCertificateDto>(json);
            var toRet = EntradaServicioService.RechazarEntradaDeServicio(motivoRechazo);
            return JsonCustom(new { data = toRet });
        }

        [HttpPost]
        public async Task<ActionResult> AprobarEntradaDeServicio(string nro_es_local, string Moneda)
        {
            var result = await EntradaServicioService.AprobarEntradaDeServicio(nro_es_local, Moneda);
            return JsonCustom(new { data = result });
        }

        public ActionResult ReasignarSuplente(string nro_es_local, string suplente)
        {
            var result = EntradaServicioService.ReasignarSuplente(nro_es_local, suplente);
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