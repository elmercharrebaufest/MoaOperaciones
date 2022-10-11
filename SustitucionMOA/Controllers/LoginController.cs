using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Linq;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;
using Model = SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class LoginController : Controller
    {

        //LoginService _loginService = new LoginService();

        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly IRepositorio repositorio;

        public LoginController(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        /*
        public ActionResult login(string username, string pass)
        {
            try
            {
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

                SessionPersister.User = new Model.Usuario()
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

                LogFile(username, pass);

                return Json(new { success = SuccessMsg.LoginOk, username = username, nombre = result.nombre, proveedor = result.proveedor, granosFlag = result.granosFlag, tipoUsuario = result.tipoUsuario, permisos = result.permisos, noticias = noticias, esNuevoUsuario = false }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidarLoginAzure()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return Json(new { success = "Ok" }, JsonRequestBehavior.AllowGet);
                }

                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                string CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");

                string GranosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

                Entidades.Usuario usuario = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                if (GranosFlag.Equals("Granos"))
                {
                    UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT }; 

                    usuarioGranos = BuscarUsuarioGranos(usuarioGranos);
                    usuarioGranos.TipoUsuario = null;

                    usuario = usuarioGranos; ;
                }

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

                try
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
                catch 
                {

                }

                return Json(new { success = SuccessMsg.LoginOk, username = usuario.Mail, nombre = usuario.ObtenerRazonSocial(), proveedor = usuario.ObtenerCodigoProveedor(), granosFlag = usuario.TipoUsuario.NombreCorto, tipoUsuario = "PROV", permisos = usuario.ObtenerPermisos(), noticias = noticias, esNuevoUsuario = usuario.EsNuevoUsuario() }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new
                {
                    error = ErrorMsg.ErrorWS
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult logout()
        {
            SessionPersister.clear();
            return Json(new { success = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        private UsuarioGranos BuscarUsuarioGranos(UsuarioGranos usuarioGranos)
        {
            return repositorio.Obtener<UsuarioGranos>(u => u.Mail == usuarioGranos.Mail);
        }

        public bool ExisteUsuario(Entidades.Usuario usuario)
        {
            return (repositorio.Existe<Entidades.Usuario>(u => u.Mail == usuario.Mail));
        }

        public void LogFile(string username, string pass)
        {
            DateTime dateTime = DateTime.Now;
            string fecha = dateTime.ToString("yyyy/MM/dd");
            string hora = dateTime.ToString("hh:mm:ss");
            fecha = fecha.Replace("/", "");
            hora = hora.Replace(":", "");

            string path = Server.MapPath("/") + "Logs\\";
            string fileName = "Log.txt";
            string text = fecha + ";" + hora + ";" + username + ";" + pass;

            System.IO.StreamWriter file = new System.IO.StreamWriter(path + fileName, true);
            file.WriteLine(text);
            file.Close();
        }*/
    }
}