using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOferta
    {
        [Key]
        public int Id { get; set; }
        public int Solp_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public string Observaciones { get; set; }

        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario Usuario { get; set; }
        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }

        [InverseProperty("Peticiones")]
        public virtual ICollection<SolpPosicion> Posiciones { get; set; }

        [InverseProperty("Peticiones")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        [InverseProperty("Peticiones")]
        public virtual ICollection<Archivo> Archivos { get; set; }

    }
}
