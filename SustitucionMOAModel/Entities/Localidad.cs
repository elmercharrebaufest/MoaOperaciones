using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Localidad
    {
        [Key]
        public int LocalidadId { get; set; }

        public int CodLocalidad { get; set; }

        public string Nombre { get; set; }

        public int ProvinciaId { get; set; }

        public int PartidoId { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }

        [ForeignKey("PartidoId")]
        public virtual Partido Partido { get; set; }
    }
}
