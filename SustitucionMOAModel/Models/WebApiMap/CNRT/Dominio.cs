using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.CNRT
{
    public class Dominio
    {
        public string dominio { get; set; }

        public Rto Rto { get; set; }

        public Ruta Ruta { get; set; }
    }
}
