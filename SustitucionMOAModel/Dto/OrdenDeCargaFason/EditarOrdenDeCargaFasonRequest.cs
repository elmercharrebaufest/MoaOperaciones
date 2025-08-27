using SustitucionMOAModel.Dto.OrdenDeCargaCommon;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class EditarOrdenDeCargaFasonRequest : OrdenDeCargaFasonRequest
    {
        public UnidadTransporteCarga UnidadTransporte { get; set; }
    }
}
