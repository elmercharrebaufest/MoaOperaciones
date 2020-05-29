using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;

namespace SustitucionMOAModel.Models.ViewModel.Liquidacion
{
    public class LiquidacionViewModel
    {
        public LiquidacionWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroObservacion { get; set; }
    }
}
