using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class HistorialCambiosOrdenDeCarga
    {
        [Key]
        public int Id { get; set; }
        public int OrdenDeCarga_Id { get; set; }
        public string Cambio { get; set; }
        public DateTime FechaCambio { get; set; }
        public int Cliente_Id { get; set; }
        public string NombreColumnaCambio { get; set; } 

        [ForeignKey("Cliente_Id")]
        public virtual Proveedor Cliente { get; set; }

        [ForeignKey("OrdenDeCarga_Id")]
        public virtual OrdenDeCarga OrdenDeCarga { get; set; }
    }
}
