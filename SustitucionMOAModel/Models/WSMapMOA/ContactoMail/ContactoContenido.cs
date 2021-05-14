using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.ContactoMail
{
    public class ContactoContenido
    {
        public string asunto { get; set; }
        public string proveedor { get; set; }
        public int proveedor_id { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string categoria { get; set; }
        public string camposAdicionales { get; set; }
        public string comentario { get; set; }
        public string contrato { get; set; }
        public string razonSocial { get; set; }
        public string cuit { get; set; }
        public string nombreVendedor { get; set; }
        public string comprobante { get; set; }
        public string fechaPago { get; set; }
        public string importe { get; set; }
        public decimal importeDecimal { get; set; }
        public string impuesto { get; set; }
        public decimal impuestoDecimal { get; set; }
        public string inscripcion { get; set; }
        public string motivo { get; set; }
    }
}
