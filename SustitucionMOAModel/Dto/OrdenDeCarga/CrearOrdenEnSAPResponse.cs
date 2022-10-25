using Newtonsoft.Json;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
	public class CrearOrdenEnSAPResponse
	{
		[JsonProperty("resultCreation")]
		public bool ResultCreation { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("logout")]
		public bool Logout { get; set; }
	}
}
