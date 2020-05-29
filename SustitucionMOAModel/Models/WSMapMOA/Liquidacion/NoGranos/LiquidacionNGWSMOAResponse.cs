using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos
{
    public class LiquidacionNGWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<SalidaView> liquidaciones { get; set; }

        public LiquidacionNGWSMOAResponse() {
            this.error = new ErrorWS();
            this.liquidaciones = new List<SalidaView>() { };
        }
    }

    public class LiquidacionExcelNGWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<Salida> liquidaciones { get; set; }

        public LiquidacionExcelNGWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.liquidaciones = new List<Salida>() { };
        }
    }
}
