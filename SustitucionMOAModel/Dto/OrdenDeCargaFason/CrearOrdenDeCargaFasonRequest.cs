
using SustitucionMOAModel.Dto.OrdenDeCargaCommon;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class CrearOrdenDeCargaFasonRequest : OrdenDeCargaFasonRequest
    {
        public List<UnidadTransporteCarga> UnidadesTransporte { get; set; }
    }
}
