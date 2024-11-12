using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_NOTIFICACONES)]
        public ActionResult Grabar(string notificacionJson)
        {
            var notificacion = JsonConvert.DeserializeObject<Notificacion>(notificacionJson);

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
            //se comenta hasta volver a implementar
            //string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

            //return JsonCustom(new { data = notificacionService.ObtenerNotificacionesUsuario(userMail) });
            return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
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
    }
}