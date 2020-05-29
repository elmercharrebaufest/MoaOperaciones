using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Aplicacion
    {
        public string fecha { get; set; }
        public string ccpp { get; set; }
        public string descarga { get; set; }
        public decimal kgBrutos { get; set; }
        public string unidadBrutos { get; set; }
        public decimal kgNetos { get; set; }
        public string unidadNetos { get; set; }
        public decimal cantidad { get; set; }
        public string unidadCantidad { get; set; }
    }

    public class AplicacionView : Aplicacion 
    {
        public string descargaString { get; set; }
        public string kgBrutosString { get; set; }
        public string kgNetosString { get; set; }
        public string cantidadString { get; set; }
    }
}
