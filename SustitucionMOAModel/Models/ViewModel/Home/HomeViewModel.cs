using System.Collections.Generic;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAModel.Models.WSMapMOA.Pago;

namespace SustitucionMOAModel.Models.ViewModel.Home
{
    public class HomeViewModel
    {
        public List<ItemResumenHome> resumen { get; set; }
        public string msjCtaCte { get; set; }
        public List<MovimientoView> cuentasCorrientes { get; set; }
    }
}
