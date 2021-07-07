using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ComentarioRecordado
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaRecordado { get; set; }
        public int Comentario_Id { get; set; }

        [ForeignKey("Comentario_Id")]
        public virtual Comentario Comentario { get; set; }

    }
}
