namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class VisualizarProductoRequest
	{
		public string ClienteCuit { get; set; }
		public string Contrato { get; set; }
		public string FechaInicio { get; set; }
		public string FechaFin { get; set; }
		public string Pendiente { get; set; }
	}
}
