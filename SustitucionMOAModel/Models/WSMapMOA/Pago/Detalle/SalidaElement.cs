using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle
{
    public class SalidaElement
    {
        public string nroLegal { get; set; }
        public string caract { get; set; }
        public decimal bruto { get; set; }
        public decimal iva { get; set; }
        public decimal retIva { get; set; }
        public decimal impIibb { get; set; }
        public decimal impGanancias { get; set; }
        public string cbuIva { get; set; }
        public decimal remanenteIva { get; set; }
        public string brutoString { get; set; }
        public string ivaString { get; set; }
        public string retIvaString { get; set; }
        public string impIibbString { get; set; }
        public string impGananciasString { get; set; }
        public string remanenteIvaString { get; set; }
        public decimal mercCorredor { get; set; }
        public decimal ivaCorredor { get; set; }
        public decimal mercVendedor { get; set; }
        public decimal ivaVendedor { get; set; }
        public string mercCorredorString { get; set; }
        public string ivaCorredorString { get; set; }
        public string mercVendedorString { get; set; }
        public string ivaVendedorString { get; set; }
    }
}
