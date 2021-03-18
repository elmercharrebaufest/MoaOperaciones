using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CampoProveedorDto
    {
        public string NombreCampo { get; set; }

        public string NombreCosecha { get; set; }

        public double HectareasTotales { get; set; }

        public double HectareasSoja { get; set; }

        public double ToneladasAprobadas { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}
