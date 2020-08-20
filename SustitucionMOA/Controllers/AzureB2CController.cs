using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;
using Model = SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class AzureB2CController : Controller
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAzureB2CService azureB2CService;
        protected readonly IDataAgroService dataAgroService;


        public AzureB2CController(IRepositorio repositorio, IAzureB2CService azureB2CService, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.azureB2CService = azureB2CService;
            this.dataAgroService = dataAgroService;
        }

        public void SignUpSignIn()
        {
            string redirectUrl = "/RedirectHome";

            // Use the default policy to process the sign up / sign in flow
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUrl });
            return;
        }

        public ActionResult Login()
        {
            ValidarLogin();

            return Redirect("/");

        }

        private void ValidarLogin()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                string CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");

                CUIT = CUIT.Replace("-", string.Empty);
                string GranosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

                Entidades.Usuario usuario = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                usuario = azureB2CService.LoguearUsuario(mail, CUIT, GranosFlag);

                SessionPersister.User = new Model.Usuario()
                {
                    username = mail,
                    nombre = usuario.ObtenerRazonSocial(),
                    permisos = usuario.ObtenerPermisos()
                };

                SessionPersister.Proveedor = usuario.ObtenerCodigoProveedor();
                SessionPersister.GranosFlag = usuario.TipoUsuario.NombreCorto;
                SessionPersister.Sociedad = "MOA";

                NoticiasDetallesWSMOAResponse noticias = new NoticiasDetallesWSMOAResponse() { };

                if (!Globals.EsLocal)
                {
                    if (usuario.EstaHabilitado())
                    {
                        noticias = azureB2CService.getNoticias(usuario.ObtenerCodigoProveedor());
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
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }


        public ActionResult ValidarLoginAzure()
        {
      
            if (SessionPersister.User == null)
            {
                ValidarLogin();
            }

            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            string granosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

            Usuario usuario = azureB2CService.ObtenerUsuario(mail, granosFlag);

            if (usuario.ObtenerPermisos().Count() == 1 && usuario.ObtenerPermisos().Contains("DATAAGROLOGIN"))
            {
                DataAgroAuthWSMOAResponse data = dataAgroService.goToDataAgro(usuario.ObtenerCodigoProveedor(), usuario.ObtenerRazonSocial());

                SessionPersister.clear();

                return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = SuccessMsg.LoginOk,
                username = SessionPersister.User.username,
                nombre = SessionPersister.User.nombre,
                proveedor = SessionPersister.Proveedor,
                granosFlag = SessionPersister.GranosFlag,
                tipoUsuario = usuario.TipoUsuario,
                permisos = SessionPersister.User.permisos,
                noticias = SessionPersister.Notificaciones,
                esNuevoUsuario = usuario.EsNuevoUsuario()
            }, JsonRequestBehavior.AllowGet); ;
        }


    }
}
