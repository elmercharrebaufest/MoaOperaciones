using Newtonsoft.Json;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Services;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class ContactoMailController : BaseController
    {
        ContactoMailService _contactoMailService = new ContactoMailService();

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult sendContactoMail(string contacto, HttpPostedFileBase file)
        {
            var contactoContenido = JsonConvert.DeserializeObject<ContactoContenido>(contacto);

            return JsonCustom(new { data = _contactoMailService.SendContactoMail(contactoContenido, file) });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult getCategorias()
        {
            return JsonCustom(new { data = _contactoMailService.ObtenerCategorias() });
        }
    }
}