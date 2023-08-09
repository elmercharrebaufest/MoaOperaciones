using SustitucionMOAModel.Models.DataAgro;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class VisualizarClienteResponse
	{
		public List<ProveedorDto> Clientes { get; set; }
		//public List<MaterialDto> Productos { get; set; }

		public VisualizarClienteResponse()
		{
			Clientes = new List<ProveedorDto>();
			//Productos = new List<MaterialDto>();
		}
	}
}
