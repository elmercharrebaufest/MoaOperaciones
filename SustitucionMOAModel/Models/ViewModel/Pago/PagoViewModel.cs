using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;

namespace SustitucionMOAModel.Models.ViewModel.Pago
{
    public class PagoViewModel
    {
        public PagosWSMOAReponse data { get; set; }
        public DropdownContent filtroID { get; set; }
        public DropdownContent filtroTitular { get; set; }
        public DropdownContent filtroContratoMolinos { get; set; }
        public DropdownContent filtroContratoProveedores { get; set; }

    }

    public class PagoNGViewModel {
        public PagosNGWSMOAResponse data { get; set; }
        public DropdownContent filtroID { get; set; }
        public DropdownContent filtroTitular { get; set; }
        public DropdownContent filtroContratoMolinos { get; set; }
        public DropdownContent filtroContratoProveedores { get; set; }
    }
}
