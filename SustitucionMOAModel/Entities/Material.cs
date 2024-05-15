using SustitucionMOAModel.Enums;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class Material
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CodigoSap { get; set; }
        public TablaSeccionMaterial TablaSeccionMaterial { get; set; }
        public bool ValidaSisaRuca { get; set; }
        public string Abreviacion { get; set; }
    }
}
