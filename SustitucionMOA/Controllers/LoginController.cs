using System;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System.Text;
using System.Linq;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;

namespace SustitucionMOA.Controllers
{
    public class LoginController : Controller
    {
        LoginService _loginService = new LoginService();
        DataAgroService _dataAgroService = new DataAgroService();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                    if (noticias != null && noticias.notificaciones != null) {
                        SessionPersister.Notificaciones = noticias.notificaciones;
                        noticias.cantidad += noticias.notificaciones.Count;
                    }
                }
                catch
                {
                    noticias = new NoticiasDetallesWSMOAResponse() { };
                }

                LogFile(username,pass);

                return Json(new { success = SuccessMsg.LoginOk, username = username, nombre = result.nombre, proveedor = result.proveedor, granosFlag = result.granosFlag, tipoUsuario = result.tipoUsuario, permisos = result.permisos, noticias = noticias }, JsonRequestBehavior.AllowGet);
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

        public ActionResult logout()
        {
            SessionPersister.clear();
            return Json(new { success = "Ok" }, JsonRequestBehavior.AllowGet);
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
        }
    }
}