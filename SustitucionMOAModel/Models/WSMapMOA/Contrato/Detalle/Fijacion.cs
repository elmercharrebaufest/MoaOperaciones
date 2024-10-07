using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Fijacion
    {
        public string fecha { get; set; }
        public string nroFija { get; set; }
        public decimal kilosFija { get; set; }
        public string unidad { get; set; }
        public decimal precio { get; set; }
        public string moneda { get; set; }
    }

    public class FijacionView : Fijacion
    {

        public string kilosFijaString { get; set; }

        public string precioString { get; set; }
    }
}
