using System;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using Model = SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using Entidades = SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System.Text;
using System.Linq;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOARepositorio;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using System.Web;
using System.Security.Claims;
using SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class AzureB2CController : Controller
    {
        LoginService _loginService = new LoginService();
        DataAgroService _dataAgroService = new DataAgroService();

        protected readonly IRepositorio repositorio;

        public AzureB2CController(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public ActionResult Login(string username, string pass)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    username = "poncedef";
                    pass = "prueba";
                }

                foreach (Claim claim in ClaimsPrincipal.Current.Claims)
                {
                    continue;
                }

                Entidades.Usuario usuarioLogeado = new Entidades.Usuario();

                string CUIT = GetClaimValue("extension_CUIT");

                //ValidarCUIT(CUIT);

                usuarioLogeado.Mail = GetClaimValue("emails");

                if(!ExisteUsuario(usuarioLogeado))
                    RegistrarUsuario(usuarioLogeado);
                
                if (username == "" || username == null)
                {
                    return Json(new { info = String.Format(InfoMsg.InputNoValido, "Usuario") }, JsonRequestBehavior.AllowGet);
                }

                if (pass == "" || pass == null)
                {
                    return Json(new { info = String.Format(InfoMsg.InputNoValido, "Contraseña") }, JsonRequestBehavior.AllowGet);
                }

                LoginWSMOAResponse result = _loginService.login(username, pass);

                if (result == null)
                {
                    return Json(new { info = ErrorMsg.ErrorLogin }, JsonRequestBehavior.AllowGet);
                }

                if (result.error != "00")
                {
                    return Json(new { info = result.texto }, JsonRequestBehavior.AllowGet);
                }

                if (result.proveedor == "" || result.proveedor == null)
                {
                    return Json(new { info = ErrorMsg.ErrorLogin }, JsonRequestBehavior.AllowGet);
                }

                if (result.permisos.Count() == 1 && result.permisos[0] == "DATAAGROLOGIN")
                {
                    SessionPersister.clear();
                    DataAgroAuthWSMOAResponse data = _dataAgroService.goToDataAgro(result.proveedor, result.nombre);
                    return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
                }

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


                return Redirect("/");

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

        private string GetClaimValue(string Type)
        {
            return ClaimsPrincipal.Current.Claims.Where(x => x.Type.Equals(Type)).Select(x => x.Value).FirstOrDefault(); 
        }


        public bool RegistrarUsuario(Entidades.Usuario usuario)
        {
            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool ExisteUsuario(Entidades.Usuario usuario)
        {
            return (repositorio.Existe<Entidades.Usuario>(u => u.Mail == usuario.Mail));
        }

        public bool ValidarCUIT(string CUIT)
        {
           return _dataAgroService.ValidarCUIT(CUIT);
        }
    }
}
