namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class CrearOrdenEnSAPRequest
	{
		public int IdOrdenDeCarga { get; set; }
		public string ClienteCodigo { get; set; }
		public string ContratoSAP { get; set; }
		public string CorredorCodigo { get; set; }
		public int Cantidad { get; set; }
		public string MaterialCodigoSAP { get; set; }
		public string NumeroPedidoIngresado { get; set; }
		//public string ValidarKg { get; set; }
		public string MailUsuarioSAP { get; set; }
	}
}
