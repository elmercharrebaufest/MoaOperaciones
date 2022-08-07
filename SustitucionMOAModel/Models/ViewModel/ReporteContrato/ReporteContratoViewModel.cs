using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.ViewModel.ReporteContrato
{
    public class ReporteContratoViewModel
    {
        public ReporteContratoWSMOAResponse data { get; set; }
        public DropdownContent filtroProducto { get; set; }
        public DropdownContent filtroCliente { get; set; }
        public DropdownContent filtroNroContrato { get; set; }
        public DropdownContent filtroTipoContrato { get; set; }

        public List<Totales> totales { get; set; }
    }
}
