using Newtonsoft.Json;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
	public class OrdenDeCargaFasonDto
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("cliente")]
		public string Cliente { get; set; }

		[JsonProperty("estado")]
		public EstadoOrdenDeCargaFason Estado { get; set; }

		[JsonProperty("fechaCreacion")]
		public string FechaCreacion { get; set; }

		[JsonProperty("fechaRetiro")]
		public string FechaRetiro { get; set; }

		public int Cantidad { get; set; }
		public string PatenteAcoplado { get; set; }
		public string NombreChofer { get; set; }
		public string CUILChofer { get; set; }
		public string RazonSocialTransporte { get; set; }
		public string CUITTransporte { get; set; }

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

		public string ColorSemaforo { get; set; }
		public string DescripcionEstado { get; set; }
		public bool TransporteExiste { get; set; }
	}
}
