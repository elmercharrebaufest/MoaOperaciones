using Newtonsoft.Json;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
	public class OrdenDeCargaFasonDto
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("cliente")]
		public string Cliente { get; set; }

		[JsonProperty("estado")]
		public string Estado { get; set; }

		[JsonProperty("fechaCreacion")]
		public string FechaCreacion { get; set; }

		[JsonProperty("fechaRetiro")]
		public string FechaRetiro { get; set; }


		//public int Cantidad { get; set; }
		//public string PatenteAcoplado { get; set; }
		//public string NombreChofer { get; set; }
		//public string CUILChofer { get; set; }
		//public string RazonSocialTransporte { get; set; }
		//public string CUITTransporte { get; set; }

		[JsonProperty("cantidadDeViajesRealizados")]
		public short CantidadDeViajesRealizados { get; set; }

		[JsonProperty("cantidadDeViajesEsperados")]
		public short CantidadDeViajesEsperados { get; set; }

		[JsonProperty("producto")]
		public string Producto { get; set; }

		[JsonProperty("patenteChasis")]
		public string PatenteChasis { get; set; }

		[JsonProperty("destino")]
		public string Destino { get; set; }
	}

	public class OrdenDeCargaFasonDetalleDto
    {
		public long Id { get; set; }
		public string EstadoDescripcion { get; set; }
		public string FechaCreacion { get; set; }
		public string FechaRetiro { get; set; }
		public int Cantidad { get; set; }
		public string PatenteChasis { get; set; }
		public string PatenteAcoplado { get; set; }
		public string NombreChofer { get; set; }
		public string CUILChofer { get; set; }
		public string RazonSocialTransporte { get; set; }
		public string CUITTransporte { get; set; }
		public string Destino { get; set; }
		public short CantidadDeViajesRealizados { get; set; }
		public short CantidadDeViajesEsperados { get; set; }
		public string Observacion { get; set; }
		public string Cliente { get; set; }
		public bool TransporteExiste { get; set; }
	}
}
