using System;

namespace SustitucionMOAModel.Dto
{
    public class IngresosBrutosCoeficienteUnificadoDto
    {
        public int Id { get; set; }

        public int EstadoId { get; set; }

        public string CUIT { get; set; }

        public int Anticipo { get; set; }

        public int Sede { get; set; }

        public DateTime FechaCarga { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        public bool MalCargada { get; set; }

        public int? SecuenciaId { get; set; }

        public int? ConsultaId { get; set; }
    }
}
