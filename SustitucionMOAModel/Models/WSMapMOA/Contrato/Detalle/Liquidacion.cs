using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Liquidacion
    {
        public string fecha { get; set; }
        public string tipo { get; set; }
        public string comprobante { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public decimal precio { get; set; }
        public string monedaPrecio { get; set; }
        public decimal total { get; set; }
        public string monedaTotal { get; set; }
        public string pedido { get; set; }
        public string fijacion { get; set; }
    }

    public class LiquidacionView : Liquidacion
    { 
        public string cantidadString { get; set; }
        public string precioString { get; set; }
        public string totalString { get; set; }
        public bool verProforma { get; set; }
    }
}
