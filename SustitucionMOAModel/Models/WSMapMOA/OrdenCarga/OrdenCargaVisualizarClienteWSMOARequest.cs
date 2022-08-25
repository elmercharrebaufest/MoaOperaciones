using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.OrdenCarga
{
	public class OrdenCargaVisualizarClienteWSMOARequest
	{
		public string Cliente { get; set; }
		public string Contrato { get; set; }
		public string Corredor { get; set; }
		public List<FechaWS> Fechas { get; set; }
		public string Material { get; set; }
		public string Pendiente { get; set; }
		public string TipoContrato { get; set; }

		public OrdenCargaVisualizarClienteWSMOARequest()
		{
			Cliente = string.Empty;
			Contrato = string.Empty;
			Corredor = string.Empty;
			Material = string.Empty;
			Pendiente = string.Empty;
			TipoContrato = string.Empty;
		}
	}
}
