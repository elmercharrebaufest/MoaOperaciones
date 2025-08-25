using System.Collections.Generic;
using SustitucionMOAModel.Dto.OrdenDeCargaCommon;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class CrearOrdenDeCargaRequest
    {
        public Entities.OrdenDeCarga OrdenDeCarga { get; set; }

        public GestionAltasFAS GestionAltasFAS { get; set; }

        public List<UnidadTransporteCarga> UnidadesTransporte { get; set; }
    }
}
