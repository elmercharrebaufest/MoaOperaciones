using SustitucionMOAModel.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public bool EsDerivadoGranario { get; set; }

        [InverseProperty("Materiales")]
        public virtual ICollection<Almacen> Almacenes { get; set; }
    }
}
