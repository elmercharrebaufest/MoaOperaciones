using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Comentario
    {
        [Key]
        public int Id { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha { get; set; }
        public int Consulta_Id { get; set; }
        public int Usuario_Id { get; set; }
        public bool? Recordado { get; set; }
        public DateTime? FechaRecordado { get; set; }

        [ForeignKey("Consulta_Id")]
        public virtual Consulta Consulta { get; set; }
        
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [InverseProperty("Comentarios")]
        public virtual ICollection<Archivo> Archivos { get; set; }

        public virtual ICollection<ComentarioRecordado> ComentarioRecordado { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone() as object;
        }
    }
}
