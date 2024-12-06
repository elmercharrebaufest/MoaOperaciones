using SustitucionMOAModel.Models;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFacturaService
    {
        List<ValidationResult> SubirPDF(List<HttpPostedFileBase> files, string cuit, string codigo, string mail);
    }
}
