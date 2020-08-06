using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class NuevoProduccion
    {
        public int MaterialId { get; set; }
        public int Hectareas { get; set; }
        public int Toneladas { get; set; }
        public int LocalidadId { get; set; }
        public bool ArrendaPropia { get; set; }
        public int CampañaID { get; set; }

    }
}
