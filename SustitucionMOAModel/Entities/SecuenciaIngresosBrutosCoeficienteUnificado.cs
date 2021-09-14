using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class SecuenciaIngresosBrutosCoeficienteUnificado
    {
        [Key]
        public int Id { get; set; }

        public string Descripcion { get; set; }
    }
}
