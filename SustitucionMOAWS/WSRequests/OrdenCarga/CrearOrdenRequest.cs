namespace SustitucionMOAWS.WSRequests.OrdenCarga
{
    public class CrearOrdenRequest
    {
        public string Cliente { get; set; }
        public string Contrato { get; set; }
        public string Corredor { get; set; }
        public decimal Kilos { get; set; }
        public string Material { get; set; }
        public string PedidoInput { get; set; }
        public string UsuarioSAP { get; set; }
        public bool ValidaKg { get; set; }
        public string CuitDestino { get; set; }
        public string CuitDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public bool Reventa { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
    }
}
