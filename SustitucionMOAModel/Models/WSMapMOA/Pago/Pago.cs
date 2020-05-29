using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago
{
    public class Pago
    {
        public string proveedor { get; set; }
        public string facreditacion { get; set; }
        public string idPago { get; set; }
        public string moneda { get; set; }
        public decimal totalMercaderia { get; set; }
        public decimal iva { get; set; }
        public decimal retencion { get; set; }
        public decimal monto { get; set; } 
        public string contrato { get; set; }
        public string contrProv { get; set; }
        public string fechaPago { get; set; }
        public string comprobante { get; set; }
        public string tipoComprobante { get; set; }
        public string concepto { get; set; }
    }

    public class PagoView : Pago
    {
        public DateTime facreditacionDate { get; set; }
        public DateTime fechaPagoDate { get; set; }
        public string montoString { get; set; }
        public string totalMercaderiaString { get; set; }
        public string ivaString { get; set; }
        public string retencionString { get; set; }
        public string vbeln { get; set; }
        public string pdf { get; set; }
        public string vblnr { get; set; }
        public string gjahr { get; set; }
        public string witht { get; set; }
    }
}
