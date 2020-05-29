using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Pago
    {
        public string fecha { get; set; }
        public string idPago { get; set; }
        public string comprobante { get; set; }
        public decimal bruto { get; set; }
        public decimal iva { get; set; }
        public decimal retenciones { get; set; }
        public decimal neto { get; set; }
        public string moneda { get; set; }
    }

    public class PagoView : Pago
    {
        public string brutoString { get; set; }

        public string ivaString { get; set; }

        public string retencionesString { get; set; }

        public string netoString { get; set; }
    }
}
