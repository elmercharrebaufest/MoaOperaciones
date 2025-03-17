using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;



namespace SustitucionMOA.Controllers
{
    //[System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : BaseController
    {

        readonly IHomeService _homeService;
        protected readonly IRepositorio repositorio;
        protected readonly IAzureB2CService azureB2CService;
        protected readonly IDataAgroService dataAgroService;
        protected readonly ILoginService loginService;

        private static readonly string redirectUrl = ConfigurationManager.AppSettings["SpaUrl"];

        // GET: Home

        public HomeController(IRepositorio repositorio, IAzureB2CService azureB2CService, IDataAgroService dataAgroService,
            IHomeService _homeService, ILoginService loginService)
        {
            this.repositorio = repositorio;
            this._homeService = _homeService;
            this.azureB2CService = azureB2CService;
            this.dataAgroService = dataAgroService;
            this.loginService = loginService;
        }

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return Redirect(redirectUrl);
            }
            else
            {
                try
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl, ExpiresUtc = DateTime.Now.AddMinutes(1) });

                }
                //Ignoramos esta excepción porque la da cuando carga recursos
                catch
                {

                }
                return null;
            }
        }

        public ActionResult Registro()
        {
            //Este try catch lo ignoramos porque son las excepciones cuando carga componentes nuevos 
            try
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl, });
            }
            //Ignoramos esta excepción porque la da cuando carga recursos
            catch
            {

            }
            return null;
        }

        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                await MsalAppBuilder.ClearUserTokenCache();
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
            else
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });
            }
        }

        public ActionResult ResetPassword()
        {
            //Este try catch lo ignoramos porque son las excepciones cuando carga componentes nuevos 
            try
            {
                HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl, });
            }
            //Ignoramos esta excepción porque la da cuando carga recursos
            catch
            {

            }
            return null;
        }

        public ActionResult VerificarEstadoSesion()
        {
            return Json(new { tieneSesion = true }, JsonRequestBehavior.AllowGet);

            try
            {
                if (!Request.IsAuthenticated)
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });

                    return null;
                    //return Json(new { tieneSesion = false }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { tieneSesion = true }, JsonRequestBehavior.AllowGet);
            }
            catch (InfoCustomException e)
            {
                return Json(new
                {
                    info = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidarLoginAzure()
        {
            if (!Request.IsAuthenticated)
            {
                throw new ValidationCustomException("Su sesión ha expirado. Por favor, ingrese nuevamente.");
            }

            string username = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsUserNameType).Value;
            string nombre = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsNombreType).Value;
            string proveedor = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorType).Value;
            string granosFlag = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsGranosFlagType).Value;
            string tipoUsuario = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsTipoUsuarioType).Value;
            string esNuevoUsuarioStr = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsEsNuevoUsuarioType).Value;
            string seccionesVisitadas = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsSeccionesVisitadas).Value;
            string cuit = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsCuit).Value;
            string proveedorId = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorId).Value;



            bool esNuevoUsuario = bool.Parse(esNuevoUsuarioStr);
            bool aceptoTyC = false;
            List<string> permisos = ClaimsPrincipal.Current.Claims.Where(c => c.Type.Equals(Globals.ClaimsPermisosType)).Select(c => c.Value).ToList();

            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            string granosFlagAzure = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

            string apikey = string.Empty;

            Entidades.Usuario usuario = azureB2CService.ObtenerUsuario(mail, granosFlagAzure);

            string usuarioId = usuario.Id.ToString();

            aceptoTyC = usuario.AceptoTyC;
            apikey = usuario.ApiKey ?? string.Empty;

            seccionesVisitadas = usuario.SeccionesVisitadas;

            if (permisos.Count() == 1 && permisos.Contains("DATAAGROLOGIN"))
            {
                DataAgroAuthWSMOAResponse data = dataAgroService.goToDataAgro(usuario.ObtenerCodigoProveedor(), usuario.ObtenerRazonSocial());

                return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            }

            NoticiasDetallesWSMOAResponse noticias = new NoticiasDetallesWSMOAResponse() { };

            try
            {
                if (!Globals.EsLocal)
                {
                    if (!esNuevoUsuario)
                    {
                        noticias = loginService.ObtenerNoticias(proveedor);
                        noticias.cantidad = 0;
                        if (noticias != null && noticias.noticias != null)
                        {
                            noticias.cantidad += noticias.noticias.Count;
                        }
                        if (noticias != null && noticias.notificaciones != null)
                        {
                            noticias.cantidad += noticias.notificaciones.Count;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
            }

            string redirectURL = "";

            if (esNuevoUsuario)
            {
                if (usuario.TipoUsuario.Nombre == "Corredor")
                {
                    if (usuario.ObtenerProveedor().EstadoAprobacion == EstadoAprobacion.Aprobado)
                    {
                        redirectURL = "/dato-fiscal/vendedores-pendientes";
                    }
                    else
                    {
                        redirectURL = "/estado-solicitud";
                    }
                }
                else
                {
                    if (usuario.ObtenerProveedor().EstadoAprobacion == EstadoAprobacion.DocumentacionPendiente)
                    {
                        if (granosFlag == "G")
                        {
                            redirectURL = "/alta-empresa-granos";
                        }
                        else
                        {
                            redirectURL = "/alta-empresa-no-granos";
                        }
                    }
                    else
                    {
                        redirectURL = "/estado-solicitud";
                    }
                }
            }
            else
            {
                if ((string.IsNullOrEmpty(proveedor) || proveedor == "-") && usuario.Roles.Any(r => r.Codigo == "COMERCIAL"))
                {
                    redirectURL = "/usuario/cambio-vendedor";
                }
                else
                {
                    if (tipoUsuario == "ADMP" || tipoUsuario == "ADNA" || tipoUsuario == "RYDD")
                    {
                        redirectURL = "/aduana/pesada-online";
                    }
                    else if (tipoUsuario == "CLIE")
                    {
                        redirectURL = "/cuenta-corriente/simple";
                    }
                    else
                    {
                        if (granosFlag == "A" || granosFlag == "G")
                        {
                            redirectURL = "/home";
                        }
                        else
                        {
                            redirectURL = "/home-ngs";
                        }
                    }
                }
            }

            return Json(new
            {
                success = SuccessMsg.LoginOk,
                username,
                nombre,
                proveedor,
                granosFlag,
                permisos,
                tipoUsuario,
                noticias,
                esNuevoUsuario,
                redirectURL,
                seccionesVisitadas,
                aceptoTyC,
                apikey,
                cuit,
                proveedorId,
                usuarioId
            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult getHomeInfo(string fechaInicio, string fechaFin)
        {
            HomeViewModel result = _homeService.getHomeInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
            return JsonCustom(new { data = result });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_HOME_NG)]
        public ActionResult getHomeNGInfo(string fechaInicio, string fechaFin)
        {

            HomeViewModel result = _homeService.getHomeNGInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
            return JsonCustom(new { data = result });

        }

        public ActionResult AceptarTyC()
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(x => x.Mail == SessionPersister.User.username);
            usuario.AceptoTyC = true;
            usuario.AceptoTyCFecha = DateTime.Now;
            repositorio.GuardarCambios();
            return JsonCustom(new { data = true });
        }

        public ActionResult BuscardorInteligente(string PalabraABuscar)
        {
            return JsonCustom(new { data = _homeService.getBusqueda(PalabraABuscar, SessionPersister.getUsername(), SessionPersister.Proveedor) });
        }

        [AllowAnonymous]
        public ActionResult Ping()
        {
            return new HttpStatusCodeResult(200);
        }
    }
}