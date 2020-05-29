using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class AmpliacionAnulacion
    {
        public string tipo { get; set; }
        public string fecha { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public decimal importe { get; set; }
        public string moneda { get; set; }
    }

    public class AmpliacionAnulacionView : AmpliacionAnulacion 
    {
        public string cantidadString { get; set; }
        public string importeString { get; set; }
    }
}
