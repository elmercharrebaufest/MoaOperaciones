using Entities =  SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class DetallesActualizarOrdenDeCargaFason
    {
        public Entities.OrdenDeCargaFason orden { get; set; }
        public bool existeTransporte { get; set; }
        public bool existeIntermediarioFlete { get; set; }

        public bool existeTransporteEIntermediario { get
            {
                return existeTransporte && existeIntermediarioFlete;
            } }
    }
}
