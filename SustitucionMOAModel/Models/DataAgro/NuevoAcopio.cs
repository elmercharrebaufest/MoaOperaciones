using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class NuevoAcopio
    {
        public float Toneladas { get; set; }
        public int LocalidadID { get; set; }
        public bool ArrendaPropia { get; set; }
        public int CampañaID { get; set; }

    }
}
