using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente
{
    public class Movimiento
    {
        public string docDate { get; set; }
        public string fecVto { get; set; }
        public string docNo { get; set; }
        public string descripcion { get; set; }
        public string contrato { get; set; }
        public decimal ukurs { get; set; }
        public decimal debe { get; set; }
        public decimal haber { get; set; }
        public string moneda { get; set; }
        public decimal importeArg { get; set; }

        public decimal saldo { get; set; }
        public string agrupador { get; set; }
        public string augbl { get; set; }
        public string xblnr { get; set; }
        public string fiscYear { get; set; }
    }

    public class MovimientoView : Movimiento
    {
        public int orden { get; set; }
        public string debeString { get; set; }
        public DateTime docDateDate { get; set; }
        public DateTime fecVtoDate { get; set; }
        public string haberString { get; set; }
        public string importeArgString { get; set; }
        public string saldoString { get; set; }
        public string ukursString { get; set; }

    }
    
}
