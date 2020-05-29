using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente
{
    public class CuentaCorrienteAgrupadaWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public CuentaCorrienteAgrupadaView cuentasCorrientesSinAgrupar { get; set; }
        public List<CuentaCorrienteAgrupadaView> cuentasCorrientesAgrupadas { get; set; }
        public string total { get; set; }
        public string fechaSaldo { get; set; }
        public string msj { get; set; }

        public CuentaCorrienteAgrupadaWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cuentasCorrientesSinAgrupar = new CuentaCorrienteAgrupadaView() { };
            this.cuentasCorrientesAgrupadas = new List<CuentaCorrienteAgrupadaView>() { };
        }
    }

    public class CuentaCorrienteAgrupadaExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public CuentaCorrienteAgrupada cuentasCorrientesSinAgrupar { get; set; }
        public List<CuentaCorrienteAgrupada> cuentasCorrientesAgrupadas { get; set; }
        public string fechaSaldo { get; set; }

        public CuentaCorrienteAgrupadaExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cuentasCorrientesSinAgrupar = new CuentaCorrienteAgrupada() { };
            this.cuentasCorrientesAgrupadas = new List<CuentaCorrienteAgrupada>() { };
        }
    }
}
