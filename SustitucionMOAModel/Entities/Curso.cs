using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Acceso { get; set; }
        public int MinimosMinutosCursada { get; set; }

        [InverseProperty("Curso")]
        public virtual ICollection<ProgresoCurso> ProgresosDelCurso { get; set; }
        public override int GetHashCode()
        {
            int hashCode = 173752721;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nombre);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FechaCreacion.ToString());
            return hashCode;
        }
    }
}
