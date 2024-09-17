using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle
{
    public class Calidad
    {
        public string caracteristica { get; set; }
        public decimal resultadoCalado { get; set; }
        public decimal resultadoCamara { get; set; }
        public string certificado { get; set; }
        public decimal resultadoReconsideracion { get; set; }
        public string certificadoReconsideracion { get; set; }
        public decimal kgNetos { get; set; }
        public string unidadNetos { get; set; }
        public decimal kgDescuento { get; set; }
        public string unidadDescuento { get; set; }
        public decimal kgAplicados { get; set; }
        public string unidadAplicados { get; set; }
        public decimal porcentajeDescuento { get; set; }
        public string camaraAPresent { get; set; }
    }

    public class CalidadView : Calidad
    {
        public string kgNetosString { get; set; }
        public string kgDescuentoString { get; set; }
        public string kgAplicadosString { get; set; }
    }

    public class CalidadPDF : Calidad
    {
        public string resultadoCaladoString { get; set; }
        public string resultadoCamaraString { get; set; }
        public string kgNetosString { get; set; }
        public string kgDescuentoString { get; set; }
        public string kgAplicadosString { get; set; }
        public string porcentajeDescuentoString { get; set; }
    }
}
