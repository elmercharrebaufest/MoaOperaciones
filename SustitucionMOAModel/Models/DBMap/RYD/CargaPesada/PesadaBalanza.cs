using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD.CargaPesada
{
    public class PesadaBalanza
    {
        public int numeroPesada { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public double pesoTara { get; set; }
        public double pesoBruto { get; set; }
        public double pesoNeto { get; set; }
    }

    public class PesadaBalanzaItemInforme
    {
        public int numeroPesada { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string pesoTara { get; set; }
        public string pesoBruto { get; set; }
        public string pesoNeto { get; set; }
    }
}
