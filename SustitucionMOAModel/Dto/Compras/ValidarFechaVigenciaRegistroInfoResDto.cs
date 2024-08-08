using System;
namespace SustitucionMOAModel.Dto.Compras
{
    public class ValidarFechaVigenciaRegistroInfoResDto
    {
        public int CotizacionPosicionId { get; set; }
        public bool EstaVigente { get; set; }
        public string FechaVigencia { get; set; }
    }
}
