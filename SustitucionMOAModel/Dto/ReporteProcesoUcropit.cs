using Newtonsoft.Json;

namespace SustitucionMOAModel.Dto
{
    public class ReporteProcesoUcropit
    {
        public int? IdCampo { get; set; }
        [JsonProperty("2Bsvs")]
        public Detalle2Bsvs Bsvs2 {get;set;}
        [JsonProperty("EPA")]
        public DetalleEpa Epa { get; set; }
        [JsonProperty("EUDR")]
        public DetalleEudr Eudr { get; set; }
        public double? ToneladasAprobadasEPA { get; set; }
    }

    public class Detalle2Bsvs
    {
        public double? ToneladasAprobadas { get; set; }
        public string MotivoRechazo { get; set; }
        public double? SuperficieTotalCampo { get; set; }
        public double? SuperficieElegible { get; set; }
        public double? HectareasDeclaradas { get; set; }
    }

    public class DetalleEpa
    {
        public double? ToneladasAprobadas { get; set; }
        public string MotivoRechazo { get; set; }
    }

    public class DetalleEudr
    {
        public double? ToneladasAprobadas { get; set; }
        public string MotivoRechazo { get; set; }
    }
}
