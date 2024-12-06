using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;

namespace SustitucionMOA.Controllers
{
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