using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class IngresosBrutosCoeficienteUnificadoDetalle
    {
        [Key]
        public int Id { get; set; }

        public int? NumeroJurisdiccion { get; set; }

        public string Jurisdiccion { get; set; }

        public DateTime? FechaInicio { get; set; }
        
        public DateTime? FechaCese { get; set; }

        public decimal? CoeficienteIngresos { get; set; }

        public decimal? CoeficienteGastos { get; set; }
        
        public decimal? CoeficienteUnificado { get; set; }

        public int IngresosBrutosCoeficienteUnificado_Id { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        [ForeignKey("IngresosBrutosCoeficienteUnificado_Id")]
        public virtual IngresosBrutosCoeficienteUnificado IngresosBrutosCoeficienteUnificado { get; set; }
    }
}