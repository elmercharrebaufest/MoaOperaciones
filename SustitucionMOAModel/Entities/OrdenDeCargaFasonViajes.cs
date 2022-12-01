using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCargaFasonViajes
    {
        [Key]
        public int Id { get; set; }
        public long OrdenDeCargaFason_Id { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaEgreso { get; set; }
        public int Cantidad { get; set; }
        public string NroRemito { get; set; }
        public string UniMedCant { get; set; }

        [ForeignKey("OrdenDeCargaFason_Id")]
        public virtual OrdenDeCargaFason OrdenDeCargaFason { get; set; }
    }
}
