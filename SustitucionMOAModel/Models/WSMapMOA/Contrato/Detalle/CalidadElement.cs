using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class CalidadElement
    {
        public string caract { get; set; }
        public decimal calaResul { get; set; }
        public decimal camaResul { get; set; }
        public string kgDto { get; set; }
        public decimal kgDtoValor { get; set; }
        public decimal dto { get; set; }
        public string certificado { get; set; }

        public string nroCert { get; set; }
        public decimal recResul { get; set; }
        public string recCert { get; set; }
        public decimal kgApli { get; set; }
        public decimal kgNetos { get; set; }
        public string unidad { get; set; }
    }

}
