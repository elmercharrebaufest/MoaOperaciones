using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.Pesada
{
    public class PesadaWSResponse
    {
        public List<Pesada> pesadas { get; set; }
        public PesadaWSResponse()
        {
            this.pesadas = new List<Pesada>() { };
        }
    }
}
