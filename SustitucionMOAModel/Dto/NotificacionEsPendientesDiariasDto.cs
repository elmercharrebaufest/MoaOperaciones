namespace SustitucionMOAModel.Dto
{
    public class NotificacionEsPendientesDiariasDto
    {
        public string NRO_ES_LOCAL { get; set; }
        public string Proveedor { get; set; }
        public string NRO_OC { get; set; }
        public string Texto_breve_servicio { get; set; }
        public string Cantidad_a_certificar { get; set; }
        public string UM { get; set; }
        public string Porcentaje_a_certificar { get; set; }
        public decimal? Monto_a_certificar { get; set; }
        public string Aprobador_CDS { get; set; }
        public string Moneda { get; set; }

    }
}
