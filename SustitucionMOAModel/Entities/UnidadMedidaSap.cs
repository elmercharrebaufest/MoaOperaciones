using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class UnidadMedidaSap
    {
        [Key]
        public int Id { get; set; }

        public string UM { get; set; }
        public string Comercial { get; set; }
        public string Tecnica { get; set; }
        public string TextoUM { get; set; }
        public string TextoUM2 { get; set; }
    }
}
