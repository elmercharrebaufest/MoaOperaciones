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
        Archivo ObtenerArchivo(int archivoId);
        object ObtenerReporteFacturasCertificaciones(
            string fechaInicio,
            string fechaFin,
            string ordenDeCompra = null,
            string proveedor = null,
            int? itemsPorPagina = null,
            int? pagina = null,
            string orden = null,
            string columna = null);
        
        void GuardarFacturaPorDiferenciaTasaDeCambio(HttpPostedFileBase archivoFactura, string cuit, string codigoProveedor, string mailUsuario);
    }
}
