using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.Pesada
{
    public class Pesada
    {
        public int centro { get; set; }
        public int id { get; set; }
        public string fechaIncio { get; set; }
        public string balanza { get; set; }
        public double totalEmbarcado { get; set; }
        public string commodity { get; set; }
        public string bodega { get; set; }
        public string destino { get; set; }
        public string exportador { get; set; }
        public string vapor { get; set; }
        public string pesoProgramado { get; set; }
    }
}
