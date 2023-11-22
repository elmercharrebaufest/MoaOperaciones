using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IContactoMailService
    {
        string SendContactoMail(ContactoContenido contactoContenido, HttpPostedFileBase file);
        List<CategoriaContacto> ObtenerCategorias();   

    }
}
