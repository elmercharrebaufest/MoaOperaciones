using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class Provincia
    {
        [Key]
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }

    }

}
