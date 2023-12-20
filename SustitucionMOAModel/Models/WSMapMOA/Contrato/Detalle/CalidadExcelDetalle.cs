using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class CalidadExcelDetalle
    {
        public string ccpp { get; set; }
        public string caract { get; set; }
        public decimal calaResul { get; set; }
        public decimal camaResul { get; set; }
        public string nroCert { get; set; }
        public decimal recResul { get; set; }
        public string recCert { get; set; }
        public decimal kgDto { get; set; }
        public decimal kgApli { get; set; }
        public decimal kgNetos { get; set; }
        public string unidad { get; set; }
        public decimal dto { get; set; }

    }
    public class CalidadContratoDetalleExcel {
        public string ccpp { get; set; }
        public string caract { get; set; }
        public string resultado { get; set; }
        public string nroCert { get; set; }
        public decimal recResul { get; set; }
        public string recCert { get; set; }
        public string kgDto { get; set; }
        public decimal kgApli { get; set; }
        public decimal kgNetos { get; set; }
        public string unidad { get; set; }
        public decimal dto { get; set; }
    }

    public class CalidadPDFDetalle
    {
        public string ccpp { get; set; }

        public string caract { get; set; }

        public string calaResul { get; set; }

        public string camaResul { get; set; }

        public string nroCert { get; set; }

        public string recResul { get; set; }

        public string recCert { get; set; }

        public string unidad { get; set; }

        public string kgNetos { get; set; }

        public string kgDto { get; set; }

        public string kgApli { get; set; }

        public string dto { get; set; }

    }
}
