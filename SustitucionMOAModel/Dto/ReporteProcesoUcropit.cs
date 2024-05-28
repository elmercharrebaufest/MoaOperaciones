using Newtonsoft.Json;

namespace SustitucionMOAModel.Dto
{
    public class ReporteProcesoUcropit
    {
        public int? IdCampo { get; set; }
        [JsonProperty("2Bsvs")]
        public Detalle2Bsvs Bsvs2 {get;set;}
        public double? ToneladasAprobadasEPA { get; set; }
        public string MotivoRechazo { get; set; }
    }

    public class Detalle2Bsvs
    {
        public double? ToneladasAprobadas { get; set; }
        public double? SuperficieTotalCampo { get; set; }
        public double? SuperficieElegible { get; set; }
        public double? HectareasDeclaradas { get; set; }
    }
}
