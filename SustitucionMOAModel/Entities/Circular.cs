using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Circular
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? PlazoDeOferta { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public bool? RequiereCambioDeFechas { get; set; }
        public string Observaciones { get; set; }

        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario Usuario { get; set; }

        [InverseProperty("Circular")]
        public virtual ICollection<CircularPeticionDeOfertaUsuario> PeticionDeOfertaUsuarios { get; set; } = new List<CircularPeticionDeOfertaUsuario>();

        [InverseProperty("Circular")]
        public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    }
}
