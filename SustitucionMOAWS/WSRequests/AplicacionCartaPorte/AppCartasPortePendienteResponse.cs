using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSRequests.AplicacionCartaPorte
{
    public class AppCartasPortePendienteResponse
    {
        public List<AplicacionPendienteCartaPorte> CartasDePorte { get; set; }

        public List<AplicacionPendienteContrato> Contratos { get; set; }
    }

    public class AplicacionPendienteCartaPorte
    {
        public string NumeroCartaPorte { get; set; }

        public decimal Cantidad { get; set; }

        public string Material { get; set; }
        public string Centro { get; internal set; }
    }

    public class AplicacionPendienteContrato
    {
        public string NumeroContrato { get; set; }

        public string Material { get; set; }
        
        public string CodigoProveedor { get; set; }
        
        public bool TieneAnticipo { get; set; }
        public string Centro { get; internal set; }
    }
}
