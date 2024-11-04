using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class DistanciaDomicilioReemplazos
    {
        [Key]
        public int Id { get; set; }

        public string Anterior { get; set; }

        public string Nuevo { get; set; }
    }
}
