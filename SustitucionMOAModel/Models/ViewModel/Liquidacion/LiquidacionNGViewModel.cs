using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;

namespace SustitucionMOAModel.Models.ViewModel.Liquidacion
{
    public class LiquidacionNGViewModel
    {
        public LiquidacionNGWSMOAResponse data { get; set; }
        public DropdownContent filtroObservacion { get; set; }

        public ComprobantesNGWSMOAResponse comprobantes { get; set; }
    }
}
