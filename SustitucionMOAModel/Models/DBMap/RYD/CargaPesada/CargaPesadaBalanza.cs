using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;

namespace SustitucionMOAModel.Models.DBMap.RYD.CargaPesada
{
    public class CargaPesadaBalanza
    {
        public List<PesadaBalanza> pesadas { get; set; }
        public string balanza { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string bodega { get; set; }
        public string commodity { get; set; }
        public string destino { get; set; }
        public string exportador { get; set; }
        public string vapor { get; set; }
        public int pesoProgramado { get; set; }
        public int pesoAcumulado { get; set; }

        public CargaPesadaBalanza() {
            this.pesadas = new List<PesadaBalanza>();
        }
    }

    public class PesadaBalanzaInforme
    {
        public List<PesadaBalanzaItemInforme> pesadas { get; set; }
        public string balanza { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string bodega { get; set; }
        public string commodity { get; set; }
        public string destino { get; set; }
        public string exportador { get; set; }
        public string vapor { get; set; }
        public string pesoProgramado { get; set; }
        public string pesoAcumulado { get; set; }

        public PesadaBalanzaInforme()
        {
            this.pesadas = new List<PesadaBalanzaItemInforme>();
        }
    }
}
