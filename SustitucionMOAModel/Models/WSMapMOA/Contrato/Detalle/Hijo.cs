using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Hijo
    {
        public string fecha { get; set; }
        public string contrMadre { get; set; }
        public string contrMolinos { get; set; }
        public string contrProve { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public decimal precio { get; set; }
        public string moneda { get; set; }
    }

    public class HijoView : Hijo
    {
        public string cantidadString { get; set; }
        public string precioString { get; set; }
    }
}
