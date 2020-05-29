using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente
{
    public class CuentaCorrienteAgrupada
    {
        public string agrupador { get; set; }
        public string total { get; set; }
        public List<CuentaCorriente> cuentasCorrientes { get; set; }

        public CuentaCorrienteAgrupada() {
            this.cuentasCorrientes = new List<CuentaCorriente>() { };
        }
    }

    public class CuentaCorrienteAgrupadaView
    {
        public string agrupador { get; set; }
        public string total { get; set; }
        public List<CuentaCorrienteView> cuentasCorrientes { get; set; }

        public CuentaCorrienteAgrupadaView()
        {
            this.cuentasCorrientes = new List<CuentaCorrienteView>() { };
        }
    }
}
