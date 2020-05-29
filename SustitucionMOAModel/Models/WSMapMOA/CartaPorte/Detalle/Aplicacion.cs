using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle
{
    public class Aplicacion
    {
        public string fecha { get; set; }
        public string contrato { get; set; }
        public decimal kgAplicados { get; set; }
        public string unidadKgAplicados { get; set; }
    }

    public class AplicacionView : Aplicacion {
        public string kgAplicadosString { get; set; }
    }
}
