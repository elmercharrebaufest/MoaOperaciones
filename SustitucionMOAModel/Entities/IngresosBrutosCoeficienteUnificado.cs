using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class IngresosBrutosCoeficienteUnificado
    {
        [Key]
        public int Id { get; set; }

        public string CUIT { get; set; }

        public int Anticipo { get; set; }

        public int Sede { get; set; }

        public DateTime FechaCarga { get; set; }

        public int EstadoIngresosBrutosCoeficienteUnificado_Id { get; set; }

        public int? Consulta_Id { get; set; }
        
        public int Archivo_Id { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        [ForeignKey("EstadoIngresosBrutosCoeficienteUnificado_Id")]
        public virtual EstadoIngresosBrutosCoeficienteUnificado EstadoIngresosBrutosCoeficienteUnificado { get; set; }

        [ForeignKey("Consulta_Id")]
        public virtual Consulta Consulta { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }

        [InverseProperty("IngresosBrutosCoeficienteUnificado")]
        public virtual ICollection<IngresosBrutosCoeficienteUnificadoDetalle> Detalle { get; set; }
    }
}