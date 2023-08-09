using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class FuenteAprovisionamientoWSMOAResponse
    {
        public List<FuenteAprovisionamiento> ContratosAprovisionamiento { get; set; }

        public string error { get; set; }
    }
}
