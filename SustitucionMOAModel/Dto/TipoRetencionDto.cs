using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class TipoRetencionDto
    {
        public string Id { get; set; }
        public string DenominacionActual { get; set; }
        public string DescripcionWeb { get; set; }
    }
}
