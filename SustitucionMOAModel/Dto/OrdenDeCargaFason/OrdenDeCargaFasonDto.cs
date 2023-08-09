using Newtonsoft.Json;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
	public class OrdenDeCargaFasonDto
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("cliente")]
		
		public string CUITCliente { get; set; }

		[JsonProperty("estado")]
		public EstadoOrdenDeCargaFason Estado { get; set; }
		public string DescripcionEstado { get; set; }
		public string DescripcionEstadoListado { get; set; }
		public string ColorSemaforo { get; set; }
		public string Material { get; set; }
		public string Cliente { get; set; }
		public string RazonSocialCliente { get; set; }
		public string Corredor { get; set; }
		public string RazonSocialCorredor { get; set; }

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

		[JsonProperty("patenteChasis")]
		public string PatenteChasis { get; set; }

		public bool TransporteExiste { get; set; }
		public string Observacion { get; set; }
		public int LocalidadId { get; set; }
		public string LocalidadDescripcion { get; set; }
        public Models.DataAgro.MaterialDto Producto_Id { get; set; }
    }
}
