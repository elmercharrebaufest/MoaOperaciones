namespace SustitucionMOAModel.Dto.Compras
{
    public class ValidarFechaVigenciaRegistroInfoReqDto
    {
        public string CentroCodigoSap { get; set; }
        public string GrupoComprasCodigoSap { get; set; }
        public string MaterialCodigoSap { get; set; }
        public int CotizacionPosicionId { get; set; }
    }
}
