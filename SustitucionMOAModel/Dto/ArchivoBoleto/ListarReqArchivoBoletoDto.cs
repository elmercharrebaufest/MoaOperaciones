using System;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto.ArchivoBoleto
{
    public class ListarReqArchivoBoletoDto
    {
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        public string CUIT { get; set; }
        public int ProveedorId { get; set; }

        public DateTime FechaFinLimite { get {
                return FechaFin.AddDays(1);
            } }
    }
}
