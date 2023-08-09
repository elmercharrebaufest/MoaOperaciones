namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class ValidarCorredorClienteContratoProductoRequest
	{
		public string ClienteCuit { get; set; }
		public string ClienteCodigo { get; set; }
		public string Contrato { get; set; }
		public string Corredor { get; set; }
		public string UsuarioEmail { get; set; }
		public string FechaInicio { get; set; }
		public string FechaFin { get; set; }
		public string ProductoId { get; set; }
		public bool Pendiente { get; set; }
	}
}
