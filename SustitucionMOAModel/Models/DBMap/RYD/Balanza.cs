using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class Balanza : DbElement
    {
        public string centro { get; set; }
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public bool automatico { get; set; }
        public string toleria { get; set; }
        public string centroEmisor { get; set; }
        public string tolerX { get; set; }
        public string tipo { get; set; }
        public string pesoMaximo { get; set; }
        public string codigoSAP { get; set; }
        public string codigoCabezal { get; set; }
        public string itc { get; set; }
        public string nroPuesto { get; set; }
        public string tipoAcceso { get; set; }
        public string nombrePc { get; set; }
        public string tipoDesc { get; set; }
        public string cabezalDesc { get; set; }
    }
}
