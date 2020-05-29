using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class Commodity : DbElement
    {
        public string MaterialSap { get; set; }
        public string AlmacenOrigen { get; set; }
    }
}
