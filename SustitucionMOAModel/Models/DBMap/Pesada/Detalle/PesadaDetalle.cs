using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.Pesada.Detalle
{
    public class PesadaDetalle
    {
        public int centro { get; set; }
        public int nroOrden { get; set; }
        public int linea { get; set; } // No se realmente que es este valor
        public string pesoBruto { get; set; }
        public string pesoTara { get; set; }
        public string pesoNeto { get; set; }
        public string balanza { get; set; }
        public string fecha { get; set; }
    }
}
