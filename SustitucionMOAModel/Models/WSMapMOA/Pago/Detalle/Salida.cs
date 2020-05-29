using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle
{
    public class Salida
    {
        public string contrato { get; set; }
        public string tipo { get; set; }
        public decimal total { get; set; }
        public string totalString { get; set; }
        public decimal subtotalIvaVendedor { get; set; }
        public decimal subtotalIvaCorredor { get; set; }
        public decimal subtotalMercVendedor { get; set; }
        public decimal subtotalMercCorredor { get; set; }
        public string subtotalIvaVendedorString { get; set; }
        public string subtotalIvaCorredorString { get; set; }
        public string subtotalMercVendedorString { get; set; }
        public string subtotalMercCorredorString { get; set; }
        public List<SalidaElement> registros { get; set; }

        public Salida() {
            this.registros = new List<SalidaElement>() { };
            this.subtotalIvaVendedor = 0;
            this.subtotalIvaCorredor = 0;
            this.subtotalMercVendedor = 0;
            this.subtotalMercCorredor = 0;
        }
    }
}
