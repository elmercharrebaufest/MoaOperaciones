using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class LiquidacionInformada
    {
        [Key]
        public Guid Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string COE { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FechaComprobante { get; set; }
        [Column(TypeName = "date")]
        public DateTime FechaInformada { get; set; }
    }
}
