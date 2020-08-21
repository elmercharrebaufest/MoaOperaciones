using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;
using Model = SustitucionMOAModel.Models;



namespace SustitucionMOA.Controllers
{
    //[System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : BaseController
    {
        HomeService _homeService = new HomeService();
        LoginService _loginService = new LoginService();

        protected readonly IRepositorio repositorio;
        protected readonly IAzureB2CService azureB2CService;
        protected readonly IDataAgroService dataAgroService;

        // GET: Home

        public HomeController(IRepositorio repositorio, IAzureB2CService azureB2CService, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.azureB2CService = azureB2CService;
            this.dataAgroService = dataAgroService;
        }

        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                if (Request.Url.AbsolutePath != "" && Request.Url.AbsolutePath != "/" && Request.Url.AbsolutePath != "/login")
                {
                    return Redirect("/");
                }

                return new FilePathResult(Server.MapPath("~/index.html"), "text/html");
            }
            else
            {
                string redirectUrl = "/";
                //Este try catch lo ignoramos porque son las excepciones cuando carga componentes nuevos 
                try
                {
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });

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
            if (Request.IsAuthenticated)
            {

                if (Request.Url.AbsolutePath != "" && Request.Url.AbsolutePath != "/" && Request.Url.AbsolutePath != "/login")
                {
                    return Redirect("/");
                }

                return new FilePathResult(Server.MapPath("~/index.html"), "text/html");
            }
            else
            {
                string redirectUrl = "/";
                //Este try catch lo ignoramos porque son las excepciones cuando carga componentes nuevos 
                try
                {
                    HttpContext.GetOwinContext().Set("Policy", Globals.SignUpPolicyId);
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl, });
                }
                //Ignoramos esta excepción porque la da cuando carga recursos
                catch
                {

                }
                return null;
            }
        }

        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                SessionPersister.clear();
                await MsalAppBuilder.ClearUserTokenCache();
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
        }

        public ActionResult ResetPassword()
        {
            string redirectUrl = "/";
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

        public ActionResult ValidarLoginAzure()
        {
            if (!Request.IsAuthenticated)
            {
                return Redirect("/");
            }
            string test = SessionPersister.getUsername();

            string username = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsUserNameType).Value;
            string nombre = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsNombreType).Value;
            string proveedor = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorType).Value;
            string granosFlag = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsGranosFlagType).Value;
            string tipoUsuario = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsTipoUsuarioType).Value;
            string esNuevoUsuarioStr = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsEsNuevoUsuarioType).Value;

            bool esNuevoUsuario = bool.Parse(esNuevoUsuarioStr);

            List<string> permisos = ClaimsPrincipal.Current.Claims.Where(c => c.Type.Equals(Globals.ClaimsPermisosType)).Select(c => c.Value).ToList();

            if (permisos.Count() == 1 && permisos.Contains("DATAAGROLOGIN"))
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                string granosFlagAzure = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

                Entidades.Usuario usuario = azureB2CService.ObtenerUsuario(mail, granosFlagAzure);

                DataAgroAuthWSMOAResponse data = dataAgroService.goToDataAgro(usuario.ObtenerCodigoProveedor(), usuario.ObtenerRazonSocial());

                SessionPersister.clear();

                return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            }

            NoticiasDetallesWSMOAResponse noticias = new NoticiasDetallesWSMOAResponse() { };

            if (!Globals.EsLocal)
            {
                if (!esNuevoUsuario)
                {
                    noticias = _loginService.getNoticias(proveedor);
                    noticias.cantidad = 0;
                    if (noticias != null && noticias.noticias != null)
                    {
                        SessionPersister.Noticias = noticias.noticias;
                        noticias.cantidad += noticias.noticias.Count;
                    }
                    if (noticias != null && noticias.notificaciones != null)
                    {
                        SessionPersister.Notificaciones = noticias.notificaciones;
                        noticias.cantidad += noticias.notificaciones.Count;
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
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getHomeInfo(string fechaInicio, string fechaFin)
        {
            try
            {
                HomeViewModel result = _homeService.getHomeInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
                return JsonCustom(new { data = result });
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_HOME_NG)]
        public ActionResult getHomeNGInfo(string fechaInicio, string fechaFin)
        {
            try
            {
                HomeViewModel result = _homeService.getHomeNGInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
                return JsonCustom(new { data = result });
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}