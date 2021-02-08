using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ComentarioArchivo
    {
        [Key]
        public int Comentario_Id { get; set; }
        [Key]
        public int Archivo_Id { get; set; }
        public virtual Comentario Comentario { get; set; }
        public virtual Archivo Archivo { get; set; }
    }
}
