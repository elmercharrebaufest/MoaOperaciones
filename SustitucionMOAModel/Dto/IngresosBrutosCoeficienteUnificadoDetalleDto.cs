using System;

namespace SustitucionMOAModel.Dto
{
    public class IngresosBrutosCoeficienteUnificadoDetalleDto
    {
        public int Id { get; set; }

        public int NumeroJurisdiccion { get; set; }

        public string Jurisdiccion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaCese { get; set; }

        public decimal CoeficienteIngresos { get; set; }

        public decimal CoeficienteGastos { get; set; }

        public decimal CoeficienteUnificado { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }
        public bool Editar { get; set; }
    }
}