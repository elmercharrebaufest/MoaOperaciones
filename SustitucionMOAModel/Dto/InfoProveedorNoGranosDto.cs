using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class InfoProveedorNoGranosDto
    {
        public string ProveedorCUIT { get; set; }
        public string RazonSocial { get; set; }

        public bool IngresoAPlanta { get; set; }
        public bool SiperObligatorio { get; set; }
        public bool DeclaracionVinculosObligatorio { get; set; }
    }
}
