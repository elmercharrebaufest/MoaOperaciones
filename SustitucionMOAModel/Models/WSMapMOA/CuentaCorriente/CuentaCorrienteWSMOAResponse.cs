using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente
{
    public class CuentaCorrienteWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<MovimientoView> cuentasCorrientes { get; set; }
        public string msj { get; set; }

        public CuentaCorrienteWSMOAResponse() {
            this.error = new ErrorWS();
            this.cuentasCorrientes = new List<MovimientoView>() { };
        }
    }

    public class CuentaCorrienteExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<Movimiento> cuentasCorrientes { get; set; }

        public CuentaCorrienteExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cuentasCorrientes = new List<Movimiento>() { };
        }
    }
}
