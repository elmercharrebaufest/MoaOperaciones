using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class ObtenerLegajoResponse
    {
        public List<LegajoDto> LegajoFilas { get; set; }

        public bool PuedeVerPrecios { get; set; }
    }
}
