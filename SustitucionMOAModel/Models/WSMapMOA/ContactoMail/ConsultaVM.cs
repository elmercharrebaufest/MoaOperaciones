using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.ContactoMail
{
    public class ConsultaVM
    {
        public int id { get; set; }
        public string asunto { get; set; }
        public string estado { get; set; }
        public int idEstado { get; set; }
        public string categoria { get; set; }
        public int idCategoria { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaUltimaModificacion { get; set; }

        //datos extendidos
        //retenciones
        public string contrato { get; set; }
        public string razonSocial { get; set; }
        public string cuit { get; set; }
        public string comprobante { get; set; }
        public string inscripcion { get; set; }
        public string fechaPago { get; set; }
        public string importe { get; set; }
        public decimal importeDecimal { get; set; }
        public string impuesto { get; set; }
        public decimal impuestoDecimal { get; set; }
    }
}
