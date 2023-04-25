using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSRequests.OrdenCarga
{
    public class ControlCargaRequest
    {
        public string Cliente { get; set; }
        public string Contrato { get; set; }
        public string Corredor { get; set; }
        public string Cuit { get; set; }
        public string Material { get; set; }
        public string Pedido { get; set; }
        public bool SoloSisa { get; set; }
    }
}
