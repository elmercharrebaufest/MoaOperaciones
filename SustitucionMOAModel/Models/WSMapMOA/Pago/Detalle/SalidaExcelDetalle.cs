using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle
{
    public class SalidaExcelDetalle
    {
        public string contrato { get; set; }
        public string tipo { get; set; }
        public decimal total { get; set; }
        public decimal totalIva { get; set; }
        public string moneda { get; set; }
        public string nroLegal { get; set; }
        public string caract { get; set; }
        public decimal bruto { get; set; }
        public decimal iva { get; set; }
        public decimal retIva { get; set; }
        public decimal impIibb { get; set; }
        public decimal impGanancias { get; set; }
        public string cbuIva { get; set; }
        public decimal remanenteIva { get; set; }
    }
}
