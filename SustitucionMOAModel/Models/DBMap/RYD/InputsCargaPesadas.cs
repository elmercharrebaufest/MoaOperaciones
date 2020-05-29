using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class InputsCargaPesadas
    {
        public List<DbElement> balanzas { get; set; }
        public List<DbElement> bodegas { get; set; }
        public List<DbElement> commodities { get; set; }
        public List<DbElement> destinos { get; set; }
        public List<DbElement> exportadores { get; set; }
        public List<DbElement> vapores { get; set; }

        public InputsCargaPesadas() {
            this.balanzas = new List<DbElement>() { };
            this.bodegas = new List<DbElement>() { };
            this.commodities = new List<DbElement>() { };
            this.destinos = new List<DbElement>() { };
            this.exportadores = new List<DbElement>() { };
            this.vapores = new List<DbElement>() { };
        }
    }
}
