using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;

namespace SustitucionMOAModel.Models.ViewModel.CuentaCorriente
{
    public class CuentaCorrienteViewModel
    {
        public CuentaCorrienteWSMOAResponse data { get; set; }
        public DropdownContent filtroConcepto { get; set; }
        
    }

    public class CuentaCorrienteAgrupadaViewModel
    {
        public CuentaCorrienteAgrupadaWSMOAResponse data { get; set; }
        public DropdownContent filtroConcepto { get; set; }
    }
}
