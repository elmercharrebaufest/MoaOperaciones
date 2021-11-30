using System;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto
{
    public class MovimientoIngresosBrutosCoeficienteUnificadoDto
    {
        public int Id { get; set; }

        public IngresosBrutosCoeficienteUnificadoDto IngresosBrutosCoeficienteUnificado { get; set; }

        public string Observaciones { get; set; }

        public DateTime Fecha { get; set; }

        public int TipoId { get; set; }

        public int OrigenId { get; set; }

        public int EstadoAnteriorId { get; set; }

        public int EstadoPosteriorId { get; set; }
    }
}