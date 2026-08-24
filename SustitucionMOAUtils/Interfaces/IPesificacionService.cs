using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public interface IPesificacionService
    {
        List<Contrato> GetContratos(string proveedor);
        Fecha GetFechaPesificacion(string formatoFecha);
        List<PesificacionSapDto> GetPesificacionesSAP(string proveedor);
        PesificacionSetContratosWSMOAResponse SetContrato(string proveedor, string contrato, string fijacion, decimal cantidad);
        string SetContratos(string proveedor, HttpPostedFileBase file);
        PesificacionSetContratosWSMOAResponse SetContratos(string proveedor, List<ContratoContenido> contratos);
        DolarMaterialDto GetSoja200();
        DolarMaterialDto GetDolarGirasol();
        DolarMaterialDto GetDolarMaiz();
        List<ContratoContenido> LeerContratosCSV(HttpPostedFileBase file);
    }
}