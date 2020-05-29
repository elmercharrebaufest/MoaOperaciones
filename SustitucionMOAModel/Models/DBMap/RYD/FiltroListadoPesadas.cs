using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class FiltroListadoPesadas
    {
        public List<DbElement> commodities { get; set; }
        public List<DbElement> exportadores { get; set; }

        public FiltroListadoPesadas()
        {
            this.commodities = new List<DbElement>() { };
            this.exportadores = new List<DbElement>() { };
        }
        
    }
}
