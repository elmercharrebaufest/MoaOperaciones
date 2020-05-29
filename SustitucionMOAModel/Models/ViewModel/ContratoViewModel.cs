using SustitucionMOAModel.Models.WSMapMOA.Contrato;

namespace SustitucionMOAModel.Models.ViewModel
{
    public class ContratoViewModel
    {
        public ContratosWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroVendedor { get; set; }
    }
}
