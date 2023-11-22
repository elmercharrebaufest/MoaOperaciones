using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class Partido
    {
        [Key]
        public int Id { get; set; }

        public string Descripcion { get; set; }

        public int ProvinciaID { get; set; }

    }
}
