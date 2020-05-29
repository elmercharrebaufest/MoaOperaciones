using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Proforma
{
    public class Salida
    {
        public string contrato { get; set; }
        public string caracteristica { get; set; }
        public string moneda { get; set; }
        public decimal importe { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }
    }

    public class SalidaView : Salida
    {
        public string importeString { get; set; }
        public string ivaString { get; set; }
        public string totalString { get; set; }
    }
}
