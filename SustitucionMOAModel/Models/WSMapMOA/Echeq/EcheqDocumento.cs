using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Echeq
{
    public class EcheqDocumento
    {
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string Sociedad { get; set; }
        public string Documento { get; set; }
        public string Ejercicio { get; set; }
        public string Fecha { get; set; }
        public string NumeroCOE { get; set; }
        public string Solapa { get; set; }
        public decimal ImporteMonedaDocumento { get; set; }
        public decimal ImporteEnPesos { get; set; }
        public string Moneda { get; set; }
        public bool DMBTRSpecified { get; set; }
        public bool WRBTRSpecified { get; set; }
    }
}
