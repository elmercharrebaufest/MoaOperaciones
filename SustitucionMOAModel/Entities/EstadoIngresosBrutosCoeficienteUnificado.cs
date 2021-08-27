using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class EstadoIngresosBrutosCoeficienteUnificado
    {
        [Key]
        public int Id { get; set; }

        public string Descripcion { get; set; }
    }
}