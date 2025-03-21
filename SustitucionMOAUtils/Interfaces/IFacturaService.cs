using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFacturaService
    {
        void EliminarFacturasAntiguas();
        List<ValidationResult> SubirPDF(List<HttpPostedFileBase> files, string cuit, string codigo, string mail);
        List<CertificacionRegistrada> RegistrarCertificacion(List<CertificacionDto> certificaciones, string mail, int proveedorId, List<HttpPostedFileBase> files, string cuit, string codigo);
        string VerificarSiExisteRegistro(string NRO_Certificacion);
        CertificacionRegistrada ObtenerCertificacion(string NRO_Certificacion);
    }
}
