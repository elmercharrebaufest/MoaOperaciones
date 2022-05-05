using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos
{
    public class Salida
    {
        public string id { get; set; }
        public string vencimiento { get; set; }
        public string tipo { get; set; }
        public string comprobante { get; set; }
        public decimal importe { get; set; }
        public string moneda { get; set; }
        public string compra { get; set; }
        public string observaciones { get; set; }
        public string fechaDocumento { get; set; }
        public DateTime fechaComprobanteDate { get; set; }

    }

    public class SalidaView : Salida
    {
        public DateTime vencimientoDate { get; set; }
        public string importeString { get; set; }
        public string estadoRegistradas { get { return observaciones == "" ? "Aprobada" : "Observada"; } }
        public DateTime fechaDocFiltro { get; set; }
    }
}
