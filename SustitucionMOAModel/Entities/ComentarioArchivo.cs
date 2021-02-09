using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ComentarioArchivo
    {
        [Key, Column(Order = 0)]
        public int Comentario_Id { get; set; }
        [Key, Column(Order = 1)]
        public int Archivo_Id { get; set; }

        [ForeignKey("Comentario_Id")]
        public virtual Comentario Comentario { get; set; }
        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }
    }
}
