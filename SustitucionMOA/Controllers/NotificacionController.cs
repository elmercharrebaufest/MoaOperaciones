using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
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

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.c)]
        public ActionResult GetListado()
        {
            return JsonCustom(new { data = notificacionService.Listar() });
        }

        public ActionResult GetNotificacion(int notificacionId)
        {
            return JsonCustom(new { data = notificacionService.ObtenerNotificacion(notificacionId) });
        }

        public ActionResult GetNotificaciones()
        {
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
            return JsonCustom(new { data = notificacionService.ObtenerNotificacionesUsuario(userMail) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Eliminar(int notificacionId)
        {
            return JsonCustom(new { data = notificacionService.Eliminar(notificacionId) });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Deshabilitar(int notificacionId)
        {
            return JsonCustom(new { data = notificacionService.Deshabilitar(notificacionId) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Habilitar(int notificacionId)
        {
            return JsonCustom(new { data = notificacionService.Habilitar(notificacionId) });
        }

        public ActionResult GetAllNotificacionPrioridad()
        {
            return JsonCustom(new { data = notificacionService.ObtenerTodosNotificacionPrioridad() });
        }

        /// <summary>
        /// Agrega marca de notificacion leida.
        /// </summary>
        /// <param name="notificacionId"></param>
        /// <returns></returns>
        public ActionResult PostNotificacionLeida(int notificacionId)
        {
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
            return JsonCustom(new { data = notificacionService.GrabarNotificacionComoLeida(notificacionId, userMail) });
        }

        public ActionResult GetListadoCompletoNotificacion()
        {
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
            return JsonCustom(new { data = notificacionService.ObtenerListadoCompletoNotificacion(userMail) });
        }


    }

}