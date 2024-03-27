using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto.ArchivoBoleto
{
    public class ActualizarArchivoBoletoDto
    {
        public string Mensaje { get; set; }
        [Required]
        public int EstadoFinal { get; set; }
    }
}
