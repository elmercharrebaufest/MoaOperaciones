using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        LoginService _loginService = new LoginService();

        public AzureB2CController(IRepositorio repositorio, IAzureB2CService azureB2CService)
        {
            this.repositorio = repositorio;
            this.azureB2CService = azureB2CService;
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
                        noticias = _loginService.getNoticias(usuario.ObtenerCodigoProveedor());
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
            catch (Exception ex)
            {

            }

        }


        public ActionResult ValidarLoginAzure()
        {
            //if (result.permisos.Count() == 1 && result.permisos[0] == "DATAAGROLOGIN")
            //{
            //    SessionPersister.clear();
            //    DataAgroAuthWSMOAResponse data = _dataAgroService.goToDataAgro(result.proveedor, result.nombre);
            //    return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            //}

            if (SessionPersister.User == null)
            {
                ValidarLogin();
            }

            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            string granosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

            Usuario usuario = azureB2CService.ObtenerUsuario(mail, granosFlag);

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
