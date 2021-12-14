using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class MovimientoIngresosBrutosCoeficienteUnificado
    {
        [Key]
        public int Id { get; set; }

        public int IngresosBrutosCoeficienteUnificado_Id { get; set; }

        public string Observaciones { get; set; }

        public DateTime Fecha { get; set; }

        public int TipoMovimientoIngresosBrutosCoeficienteUnificado_Id { get; set; }

        public int OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id { get; set; }

        public int EstadoAnterior_Id { get; set; }

        public int EstadoPosterior_Id { get; set; }

        [ForeignKey("IngresosBrutosCoeficienteUnificado_Id")]
        public virtual IngresosBrutosCoeficienteUnificado IngresosBrutosCoeficienteUnificado { get; set; }

        [ForeignKey("TipoMovimientoIngresosBrutosCoeficienteUnificado_Id")]
        public virtual TipoMovimientoIngresosBrutosCoeficienteUnificado TipoMovimientoIngresosBrutosCoeficienteUnificado { get; set; }

        [ForeignKey("OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id")]
        public virtual OrigenMovimientoIngresosBrutosCoeficienteUnificado OrigenMovimientoIngresosBrutosCoeficienteUnificado { get; set; }

        [ForeignKey("EstadoAnterior_Id")]
        public virtual EstadoIngresosBrutosCoeficienteUnificado EstadoAnterior { get; set; }

        [ForeignKey("EstadoPosterior_Id")]
        public virtual EstadoIngresosBrutosCoeficienteUnificado EstadoPosterior { get; set; }
    }
}
