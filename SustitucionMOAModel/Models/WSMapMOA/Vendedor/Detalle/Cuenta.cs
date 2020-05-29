using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle
{
    public class Cuenta
    {
        public string proveedor { get; set; }

        public string banco { get; set; }

        public string cuenta { get; set; }

        public string cbu { get; set; }

        public string tipoCta { get; set; }
    }
}
