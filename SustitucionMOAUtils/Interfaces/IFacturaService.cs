using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.Factura;
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
        Archivo ObtenerArchivo(int archivoId);
        object ObtenerReporteFacturasCertificaciones(
            string fechaInicio,
            string fechaFin,
            string mailUsuario,
            string ordenDeCompra = null,
            string proveedorRazonSocial = null,
            int? itemsPorPagina = null,
            int? pagina = null,
            string orden = null,
            string columna = null);
        
        List<CertificacionRegistrada> RegistrarCertificaciones(List<GrupoCertificaciones> gruposCertificaciones, string mailUsuario, int proveedorId, List<HttpPostedFileBase> archivos, string cuit, string codigoProveedor);
    }
}
