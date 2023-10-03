using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.CNRT
{
    public class Equipo
    {
        public string CategoriaEscalado { get; set; }

        public decimal Pbtc { get; set; }

        public List<Dominio> Dominios { get; set; }
    }
}
