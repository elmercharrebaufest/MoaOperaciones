using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class CuentaWSMOAResponse
    {
        public string error { get; set; }
        public List<Cuenta> cuentas { get; set; }
    }
}
