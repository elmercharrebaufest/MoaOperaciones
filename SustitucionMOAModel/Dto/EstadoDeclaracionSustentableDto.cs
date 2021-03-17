using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoDeclaracionSustentableDto
    {
        public bool DeclaracionFirmada { get; set; }
        public string CosechaActual { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }

    }
}
