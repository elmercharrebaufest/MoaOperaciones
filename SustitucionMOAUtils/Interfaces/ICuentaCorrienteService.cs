using SustitucionMOAModel.Models.ViewModel.CuentaCorriente;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICuentaCorrienteService
    {
        string DownloadCuentaCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        string DownloadCuentaCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        CuentaCorrienteViewModel GetCuentasCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        CuentaCorrienteAgrupadaViewModel GetCuentasCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
    }
}
