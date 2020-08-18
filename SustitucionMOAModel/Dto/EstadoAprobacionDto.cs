using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoAprobacionDto
    {
        public EstadoAprobacion Estado { get; set; }
        public string EstadoDescripcion { get; set; }
        public string Observaciones { get; set; }
    }
}
