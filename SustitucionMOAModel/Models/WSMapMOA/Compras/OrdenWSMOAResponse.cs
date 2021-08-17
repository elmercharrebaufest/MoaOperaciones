using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class OrdenWSMOAResponse
    {
        public List<Orden> Ordenes { get; set; }
        public string Error { get; set; }
    }
}
