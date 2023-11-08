using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;
using System.Collections.Generic;

namespace SustitucionMOAWS.Interfaces
{
    public interface IAplicacionCartaPorteConsumer
    {
        ZMPES7070[] ObtenerAplicacionesPendientes(AppCartasPortePendienteRequest request);
    }
}
