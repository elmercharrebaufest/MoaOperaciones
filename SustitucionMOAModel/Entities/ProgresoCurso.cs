using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ProgresoCurso
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int CursoId { get; set; }
        public string DetalleProgreso { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaUltimoIntento { get; set; }
        public DateTime FechaCompletado { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Alumno { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso Curso { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 173752721;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alumno.Id.ToString());
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Curso.Id.ToString());
            return hashCode;
        }
    }
}
