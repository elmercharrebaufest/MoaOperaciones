using SustitucionMOAModel.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class ActualizarOrdenResiduosExternalDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public FlujoActualizacionOrdenResiduos TipoActualizacion { get; set; }

        public string MotivoRechazo { get; set; }
    }
}
