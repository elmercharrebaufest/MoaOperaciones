using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class PesificacionController : BaseController
    {
        private readonly IPesificacionService pesificacionService;
        private readonly ILogPesificacionService logPesificacionService;
        private readonly IUsuarioService usuarioService;


        public PesificacionController(IPesificacionService pesificacionService, ILogPesificacionService servicio, IUsuarioService usuarioService)
        {
            this.pesificacionService = pesificacionService;
            this.logPesificacionService = servicio;
            this.usuarioService = usuarioService;
        }

        public JsonResult GetFechaPesificacion()
        {
            try
            {
                return JsonCustom(pesificacionService.GetFechaPesificacion("yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSoja200()
        {
            try
            {
                return JsonCustom(pesificacionService.GetSoja200());
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetDolarGirasol()
        {
            try
            {
                var data = pesificacionService.GetDolarGirasol();
                return JsonCustom(new { data });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetDolarMaiz()
        {
            try
            {
                var data = pesificacionService.GetDolarMaiz();
                return JsonCustom(new { data });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobante(string contrato)
        {
            try
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "SetComprobante(string contrato) " + (contrato ?? "null"));
                var contratoJson = JsonConvert.DeserializeObject<ContratoContenido>(contrato);

                //registro MOAOperaciones el alta de una pesificacion
                var nuevaPesificacion = new LogPesificacion
                {
                    IdUsuario = ObtenerUsuarioActual().Id,
                    Fecha = DateTime.Now,
                    CodigoProveedor = SessionPersister.Proveedor,
                    Contrato = this.ParseContrato(contratoJson.Contrato),
                    Fijacion = this.ParseFijacion(contratoJson.Fijacion),
                    CantidadKilos = contratoJson.Cantidad
                };

                var logPesificacion = logPesificacionService.GuardarPesificacion(nuevaPesificacion);

                var respuestaDeContrato = pesificacionService.SetContrato(SessionPersister.Proveedor, contratoJson.Contrato, contratoJson.Fijacion, contratoJson.Cantidad);

                //si todo el proceso fue exitoso actualizo en MOAOperaciones el exitoso en el log
                logPesificacionService.ActualizarEstadoLogPesificacion(new LogPesificacion { Id = logPesificacion.Id, EnvioExitoso = true });

                return JsonCustom(respuestaDeContrato);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.PESIFICACION)]
        public ActionResult SetComprobantes(HttpPostedFileBase file)
        {
            try
            {
                var nuevaPesificacion = new LogPesificacion
                {
                    IdUsuario = ObtenerUsuarioActual().Id,
                    Fecha = DateTime.Now,
                    CodigoProveedor = SessionPersister.Proveedor,
                };

                var logPesificacion = logPesificacionService.GuardarPesificacionAutomatica(nuevaPesificacion, file);
                var envioSap = pesificacionService.SetContratos(SessionPersister.Proveedor, file);

                logPesificacionService.ActualizarEstadoLogPesificacion(new LogPesificacion { Id = logPesificacion.Id, EnvioExitoso = true });
                return JsonCustom(envioSap);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetContratos()
        {
            try
            {
                return JsonCustom(pesificacionService.GetContratos(SessionPersister.Proveedor));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public JsonResult PesificacionesSAP()
        {
            try
            {
                return JsonCustom(new { data = pesificacionService.GetPesificacionesSAP(SessionPersister.Proveedor) });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        private int ParseContrato(string contrato)
        {
            int valor;
            if (int.TryParse(contrato, out valor))
            {
                return valor;
            }
            return 0;
        }

        private int? ParseFijacion(string fijacion)
        {
            if (string.IsNullOrEmpty(fijacion))
                return null;

            int valor;
            if (int.TryParse(fijacion, out valor))
            {
                return valor;
            }
            return 0;
        }
    }
}