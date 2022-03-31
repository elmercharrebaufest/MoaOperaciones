using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ILiquidacionService
    {
        LiquidacionViewModel getAprobadas(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionViewModel getObservadas(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionViewModel getPagas(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionViewModel getLiquidaciones(string proveedor, string tipo, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getAprobadasNG(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getObservadasNG(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getRegistradosNG(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getPendienteRegistroNG(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getPagasNG(string proveedor, string fechaInicio, string fechaFin);
        LiquidacionNGViewModel getLiquidacionesNG(string proveedor, string tipo, string fechaInicio, string fechaFin);
        string downloadAprobadas(string proveedor, string fechaInicio, string fechaFin);
        string downloadObservadas(string proveedor, string fechaInicio, string fechaFin);
        string downloadPagas(string proveedor, string fechaInicio, string fechaFin);
        string downloadAprobadasNG(string proveedor, string fechaInicio, string fechaFin);
        string downloadObservadasNG(string proveedor, string fechaInicio, string fechaFin);
        string downloadRegistradosNG(string proveedor, string fechaInicio, string fechaFin);
        string downloadPendienteRegistroNG(string proveedor, string fechaInicio, string fechaFin);
        string downloadPagasNG(string proveedor, string fechaInicio, string fechaFin);
        VinculaDetalleWSMOAResponse getVinculacion(string proveedor, string contrato, string secuencia);
        string descargaVinculacion(string proveedor, string contrato, string secuencia);
        DetalleCteWSMOAResponse getProforma(string proveedor, string fijacion);
        string descargaProforma(string proveedor, string fijacion);
        PDFResponse descargaProformaFinal(string contrato, string fijacion);
        ProcedenciaFleteWSMOAResponse getFleteProcedencia(string proveedor, string contrato);
        string descargarFleteProcedencia(string proveedor, string contrato);
        Task NotificarLiquidacionesAsync(HttpFileCollectionBase liquidaciones, string codigoProveedor);
        IList<LiquidacionInformada> GetLiquidacionInformadas(string codigoProveedor);
        LiquidacionViewModel TodasLiquidaciones(string proveedor, string fechaInicio, string fechaFin);
        ComprobantesNGWSMOAResponse getComprobantesNG(string proveedor, string fechaInicio, string fechaFin);

        }
}
