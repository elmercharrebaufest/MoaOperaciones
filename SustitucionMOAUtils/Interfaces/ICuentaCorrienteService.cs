using SustitucionMOAModel.Models.ViewModel.CuentaCorriente;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICuentaCorrienteService
    {
        string downloadCuentaCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        string downloadCuentaCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        CuentaCorrienteViewModel getCuentasCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
        CuentaCorrienteAgrupadaViewModel getCuentasCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion);
    }
}
