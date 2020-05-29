using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Home
{
    public class HomeWSMOAResponse
    {
        public List<ItemResumenHome> resumen { get; set; }

        public HomeWSMOAResponse()
        {
            this.resumen = new List<ItemResumenHome>() { };
        }
    }
}
