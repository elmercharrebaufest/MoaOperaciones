using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class NotificacionController : BaseController
    {
        readonly INotificacionService notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            this.notificacionService = notificacionService;
        }

        [ValidateInput(false)]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Grabar(List<HttpPostedFileBase> files, string notificacionJson)
        {
            try
            {
                var notificacion = JsonConvert.DeserializeObject<Notificacion>(notificacionJson);
                if (files != null)
                {
                    notificacion.ArchivosAdjuntos = new List<NotificacionAdjunto>();
                    foreach (var file in files)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            // Convertir el archivo a bytes
                            byte[] fileData;
                            using (var binaryReader = new BinaryReader(file.InputStream))
                            {
                                fileData = binaryReader.ReadBytes(file.ContentLength);
                            }
                            string base64Archivo = Convert.ToBase64String(fileData);
                            NotificacionAdjunto fileEntity = new NotificacionAdjunto()
                            {
                                AdjuntoNombre = file.FileName,
                                AdjuntoTipo = file.ContentType,
                                AdjuntoContenido = base64Archivo
                            };
                            
                            notificacion.ArchivosAdjuntos.Add(fileEntity);
                        }
                    }
                }
                return JsonCustom(new { data = notificacionService.GrabarNotificacion(notificacion) });
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

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.c)]
        public ActionResult GetListado()
        {
            try
            {
                return JsonCustom(new { data = notificacionService.Listar() });
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

        public ActionResult GetNotificacion(int notificacionId)
        {
            try
            {
                return JsonCustom(new { data = notificacionService.ObtenerNotificacion(notificacionId) });
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

        public ActionResult GetNotificaciones()
            {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(new { data = notificacionService.ObtenerNotificacionesUsuario(userMail) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Eliminar(int notificacionId)
        {
            try
            {
                return JsonCustom(new { data = notificacionService.Eliminar(notificacionId) });
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Deshabilitar(int notificacionId)
        {
            try
            {
                return JsonCustom(new { data = notificacionService.Deshabilitar(notificacionId) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Habilitar(int notificacionId)
        {
            try
            {
                return JsonCustom(new { data = notificacionService.Habilitar(notificacionId) });
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

        public ActionResult GetAllNotificacionPrioridad()
        {
            try
            {
                return JsonCustom(new { data = notificacionService.ObtenerTodosNotificacionPrioridad() });
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

        /// <summary>
        /// Agrega marca de notificacion leida.
        /// </summary>
        /// <param name="notificacionId"></param>
        /// <returns></returns>
        public ActionResult PostNotificacionLeida(int notificacionId)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                return JsonCustom(new { data = notificacionService.GrabarNotificacionComoLeida(notificacionId, userMail) });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetListadoCompletoNotificacion()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(new { data = notificacionService.ObtenerListadoCompletoNotificacion(userMail) });
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


    }

}