using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.ViewModel
{
    public class ContratosNoCumplidosViewModel
    {
        public ContratosNoCumplidosWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroVendedor { get; set; }
    }
}
