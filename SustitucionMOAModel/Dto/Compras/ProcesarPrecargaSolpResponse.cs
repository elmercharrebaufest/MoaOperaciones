using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class ProcesarPrecargaSolpResponse
    {
        public List<string> ErroresValidacion { get; set; } = new List<string>();

        public List<SolpPosicionDto> Posiciones { get; set; } = new List<SolpPosicionDto>();
    }
}
