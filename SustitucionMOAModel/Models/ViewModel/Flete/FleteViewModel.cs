using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Flete;

namespace SustitucionMOAModel.Models.ViewModel.Flete
{
    public class FleteViewModel
    {
        public FletesWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
    }

    public class FleteAgrupadosViewModel
    {
        public FletesAgrupadosWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
    }
}
