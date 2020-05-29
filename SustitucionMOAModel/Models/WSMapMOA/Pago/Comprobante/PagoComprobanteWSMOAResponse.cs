using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.Comprobante
{
    public class PagoComprobanteWSMOAResponse
    {
        public List<Comprobante> comprobantes { get; set; }
        public decimal totalRetenciones { get; set; }
        public string fiscalYear { get; set; }

        public PagoComprobanteWSMOAResponse() {
            this.comprobantes = new List<Comprobante>() { };
        } 
    }
}
