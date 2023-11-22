using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Fijacion.Detalle;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IContratoService
    {
        ContratoViewModel ObtenerVigentes(string proveedor, string fechaInicio, string fechaFin);
        ContratosNoCumplidosViewModel ObtenerContratosNoCumplidos(string proveedor, string fechaInicio, string fechaFin, List<string> contratos, string tipoOperacion, string tipoOperacionMsj);
        ContratoDetalleWSMOAResponse ObtenerDetalleContrato(string proveedor, string numeroContrato);
        FijacionDetalleWSMOAResponse ObtenerDetalleFijacion(string proveedor, string numeroContrato, string fijacion);
        string DescargarVigentes(string proveedor, string fechaInicio, string fechaFin);
        string DescargarNoCumplidos(string proveedor, string fechaInicio, string fechaFin, string tipoOperacion, string nombreArchivo, string tipoOperacionMsj);
        string DescargarDetalle(string proveedor, string numeroContrato);
        Pdf DescargarPDFCalidad(string proveedor, string numeroContrato);
        Pdf DescargarBoletoFisico(string proveedor, string contrato);
        string DescargarDetalleFijacion(string proveedor, string numeroContrato, string fijacion);


            }
}
