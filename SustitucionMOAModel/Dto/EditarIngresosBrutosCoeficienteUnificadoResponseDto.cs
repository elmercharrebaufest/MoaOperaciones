using System;

namespace SustitucionMOAModel.Dto
{
    public class EditarIngresosBrutosCoeficienteUnificadoResponseDto
    {
        public string Mensaje { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        public EstadoIngresosBrutosCoeficienteUnificadoDto estadoCabecera { get; set; } 
    }
}
