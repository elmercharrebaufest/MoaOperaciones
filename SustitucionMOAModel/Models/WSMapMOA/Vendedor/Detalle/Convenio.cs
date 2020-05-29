using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle
{
    public class Convenio
    {
        public string proveedor { get; set; }

        public string provincia { get; set; }

        public decimal coeficiente { get; set; }

        public string descripcion { get; set; }
    }
}
