using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class DistanciaDomicilio
    {
        [Key]
        public int Id { get; set; }

        public string DomicilioDescripcion { get; set; }

        public string DireccionBuscada { get; set; }

        public int? DistanciaKm { get; set; }

        public string JsonLugaresOSM { get; set; }

        public string JsonRutaOSRM { get; set; }
    }
}
