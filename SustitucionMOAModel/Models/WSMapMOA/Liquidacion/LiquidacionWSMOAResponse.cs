using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion
{
    public class LiquidacionWSMOAResponse
    {
        public string error { get; set; }
        public List<LiquidacionView> liquidaciones { get; set; }

        public LiquidacionWSMOAResponse()
        {
            this.liquidaciones = new List<LiquidacionView>() { };
        }
    }

    public class LiquidacionExcelWSMOAResponse
    {
        public string error { get; set; }
        public List<Liquidacion> liquidaciones { get; set; }

        public LiquidacionExcelWSMOAResponse()
        {
            this.liquidaciones = new List<Liquidacion>() { };
        }
    }
}
