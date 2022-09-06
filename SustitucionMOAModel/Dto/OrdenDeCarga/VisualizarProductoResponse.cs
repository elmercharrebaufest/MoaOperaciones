using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class VisualizarProductoResponse
	{
		public List<MaterialDto> Productos { get; set; }

		public VisualizarProductoResponse()
		{
			Productos = new List<MaterialDto>();
		}
	}
}
