using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOAModel.Models.ViewModel.Flete;
using SustitucionMOAModel.Models.WSMapMOA;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFleteService
    {
        FleteViewModel ObtenerViajesPendientes(string proveedor, string fechaInicio, string fechaFin);
        string DescargarViajesPendientes(string proveedor, string fechaInicio, string fechaFin);
        FleteAgrupadosViewModel ObtenerViajesAFacturar(string proveedor, string fechaInicio, string fechaFin);
        string DescargarViajesAFacturar(string proveedor, string fechaInicio, string fechaFin);
        FleteAgrupadosViewModel ObtenerViajesFacturados(string proveedor, string fechaInicio, string fechaFin);
        string DescargarViajesFacturados(string proveedor, string fechaInicio, string fechaFin);
        Pdf ExportarPDFAFacturar(string proveedor, string fechaInicio, string fechaFin, string proforma);
        Pdf ExportarPDFFacturado(string proveedor, string fechaInicio, string fechaFin, string proforma);
        string ValidarImporte(decimal importe, string proforma, string proveedor);
        string ObtenerRelacion(string factura, string fechaEmision, decimal importe, byte[] PDF, string pdfName, string proforma, string proveedor);
    }
}
