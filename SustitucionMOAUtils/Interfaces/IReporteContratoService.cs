using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IReporteContratoService
    {
        ReporteContratoViewModel GetContratosReporte(string mailUsuario,string proveedor, string fechaInicio, string fechaFin, bool mostrarPendientes, ReporteContratoWSMOAResponse dataFiltro);
        ReporteContratoViewModel obtenerAgrupadoProducto(ReporteContratoViewModel view);
        ReporteContratoViewModel GetContratoDetalle(string contrato, string fechaInicio, string fechaFin);
    }
}
