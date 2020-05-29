using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class ListadoEncabezadoPesadas
    {
        public string fecha { get; set; }
        public string empresa { get; set; } 
        public string commodity { get; set; }
        public string exportador { get; set; }
        public string fechaInicioFin { get; set; }
        public List<EncabezadoPesadas> pesadas { get; set; }

        public ListadoEncabezadoPesadas() {
            this.pesadas = new List<EncabezadoPesadas>() { };
        }
    }
}
