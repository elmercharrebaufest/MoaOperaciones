using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System.Security.Claims;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using System.Linq;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOA.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : BaseController
    {
        HomeService _homeService = new HomeService();
        LoginService _loginService = new LoginService();
        // GET: Home

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
                string redirectUrl = "/api/AzureB2C/Login";
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
                string redirectUrl = "/api/AzureB2C/Login";
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
            string redirectUrl = "/api/AzureB2C/Login";
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

        public bool LoginUser(string username, string pass)
        {
            try
            {

                if (username == "" || username == null)
                {
                    return false;
                }

                if (pass == "" || pass == null)
                {
                    return false;
                    //return Json(new { info = String.Format(InfoMsg.InputNoValido, "Contraseña") }, JsonRequestBehavior.AllowGet);
                }

                LoginWSMOAResponse result = _loginService.login(username, pass);

                if (result == null)
                {
                    return false;
                    //return Json(new { info = ErrorMsg.ErrorLogin }, JsonRequestBehavior.AllowGet);
                }

                if (result.error != "00")
                {
                    return false;
                    //return Json(new { info = result.texto }, JsonRequestBehavior.AllowGet);
                }

                if (result.proveedor == "" || result.proveedor == null)
                {
                    return false;
                    //return Json(new { info = ErrorMsg.ErrorLogin }, JsonRequestBehavior.AllowGet);
                }

               /* if (result.permisos.Count() == 1 && result.permisos[0] == "DATAAGROLOGIN")
                {
                    SessionPersister.clear();
                    DataAgroAuthWSMOAResponse data = _dataAgroService.goToDataAgro(result.proveedor, result.nombre);
                    return false;
                    //return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
                }*/

                SessionPersister.User = new Usuario()
                {
                    username = username,
                    nombre = result.nombre,
                    permisos = result.permisos
                };

                SessionPersister.Proveedor = result.proveedor;
                SessionPersister.GranosFlag = result.granosFlag;
                SessionPersister.Sociedad = "MOA";

                NoticiasDetallesWSMOAResponse noticias;

                try
                {

                    noticias = _loginService.getNoticias(result.proveedor);
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
                catch
                {
                    noticias = new NoticiasDetallesWSMOAResponse() { };
                }

                //LogFile(username, pass);
                return true;
                //return Json(new { success = SuccessMsg.LoginOk, username = username, nombre = result.nombre, proveedor = result.proveedor, granosFlag = result.granosFlag, tipoUsuario = result.tipoUsuario, permisos = result.permisos, noticias = noticias }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return false;
                //return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return false;
                //return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}