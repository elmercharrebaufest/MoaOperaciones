using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class PesificacionController : BaseController
    {
        PesificacionService _pesificacionService = new PesificacionService();
        private readonly ILogPesificacionService logPesificacionService;
        private readonly IUsuarioService usuarioService;


        public PesificacionController(ILogPesificacionService servicio, IUsuarioService usuarioService)
        {
            this.logPesificacionService = servicio;
            this.usuarioService = usuarioService;
        }

        public ActionResult GetFechaPesificacion()
        {
            try
            {
                return JsonCustom(_pesificacionService.GetFechaPesificacion("yyyy-MM-dd"));
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

                var respuestaDeContrato = _pesificacionService.SetContrato(SessionPersister.Proveedor, contratoJson.Contrato, contratoJson.Fijacion, contratoJson.Cantidad);

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
                var envioSap = _pesificacionService.SetContratos(SessionPersister.Proveedor, file);

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


                return JsonCustom(_pesificacionService.GetContratos(SessionPersister.Proveedor));
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


        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        private int ParseContrato(string contrato)
        {
            int valor = 0;
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

            int valor = 0;
            if (int.TryParse(fijacion, out valor))
            {
                return valor;
            }
            return 0;
        }
    }
}