using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos
{
    public class PagoNG
    {
        public string fechaPago { get; set; }
        public string numeroPago { get; set; } 
        public string fiscYear { get; set; }
        public string viaPago { get; set; }
        public string moneda { get; set; }
        public decimal totalMercaderia { get; set; }
        public decimal retencion { get; set; }
        public decimal monto { get; set; }
    }

    public class PagoNGView : PagoNG {
        public DateTime fechaPagoDate { get; set; }
        public string montoString { get; set; }
        public string totalMercaderiaString { get; set; }
        public string retencionString { get; set; }
    }
}
