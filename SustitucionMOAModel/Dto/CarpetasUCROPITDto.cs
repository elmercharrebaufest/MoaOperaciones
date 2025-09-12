using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CarpetasUCROPITDto
    {
        public int Id { get; set; }
        public int Cosecha_Id { get; set; }
        public bool EPA { get; set; }
        public bool EUDER { get; set; }
        public bool BSVS2 { get; set; }
        public string UrlSubida { get; set; }
    }
}
