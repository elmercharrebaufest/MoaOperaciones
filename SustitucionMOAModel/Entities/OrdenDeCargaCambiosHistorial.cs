using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCargaCambiosHistorial
    {
        [Key]
        public int Id { get; set; }
        public int OrdenDeCarga_Id { get; set; }
        public string Antes { get; set; }
        public string Despues { get; set; }
        public DateTime? FechaCambio { get; set; }
        public int Usuario_Id { get; set; }
        public string NombreColumnaCambio { get; set; } 

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("OrdenDeCarga_Id")]
        public virtual OrdenDeCarga OrdenDeCarga { get; set; }
    }
}
