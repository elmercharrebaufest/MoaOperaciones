namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class VisualizarClienteRequest
	{
		public string Corredor { get; set; }
		public string FechaInicio { get; set; }
		public string FechaFin { get; set; }
		public bool Pendiente { get; set; }
	}
}
