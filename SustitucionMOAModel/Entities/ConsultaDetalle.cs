using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ConsultaDetalle
    {
        [Key, ForeignKey("Consulta")]
        public int Id { get; set; }
        public int Consulta_Id { get; set; }
        public DateTime? Fecha { get; set; }
        public string ComprobanteNo { get; set; }
        public string ContratoNo { get; set; }
        public Decimal? Importe { get; set; }
        public Decimal? Impuesto { get; set; }
        public int CausaConsulta_Id { get; set; }
        public string BolsaEmisoraOblea { get; set; }

        [Required]
        [ForeignKey("Consulta_Id")]
        public virtual Consulta Consulta { get; set; }

        [ForeignKey("CausaConsulta_Id")]
        public virtual CausaConsulta CausaConsulta { get; set; }
    }
}
