using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaCommon
{
    public class UnidadTransporteCarga
    {
        public string PatenteAcoplado { get; set; }

        [JsonProperty("patenteChasis")]
        public string PatenteChasis { get; set; }

        public string NombreChofer { get; set; }

        public string ApellidoChofer { get; set; }

        public string CUILChofer { get; set; }

        public string RazonSocialTransporte { get; set; }

        public string CUITTransporte { get; set; }

        public string CUITIntermediarioFlete { get; set; }

        public string RazonSocialIntermediarioFlete { get; set; }

        public int CantidadDeViajes { get; set; }
    }
}
