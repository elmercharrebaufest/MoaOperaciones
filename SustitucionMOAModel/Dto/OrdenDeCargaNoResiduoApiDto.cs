using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public abstract class OrdenDeCargaNoResiduoApiDto : OrdenDeCargaApiDtoBase
    {
        public int Cantidad { get; set; }
        public string CUITCorredor { get; set; }
    }
}
