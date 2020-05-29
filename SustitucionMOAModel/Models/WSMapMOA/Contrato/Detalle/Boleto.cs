using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Boleto
    {
        public string fechaRecepcion { get; set; }

        public string estado { get; set; }

        public string devAcop { get; set; }

        public string tipoBoleto { get; set; }

        public string bolsa { get; set; }

        public string envioBolsa { get; set; }

        public string vueltaBolsa { get; set; }

        public string obleaBolsa { get; set; }

        public string envioAfip { get; set; }

        public string vueltaAfip { get; set; }

        public string obleaAfip { get; set; }

        public string envioSellado { get; set; }

        public string vueltaSellado { get; set; }

        public string provPlanCanje { get; set; }

        public string observacion { get; set; }
    }
}
