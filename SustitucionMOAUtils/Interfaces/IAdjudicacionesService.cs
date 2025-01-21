using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAdjudicacionesService
    {
        List<AdjudicacionDto> ListarAdjudicaciones(int solpId);

        AdjudicacionDto ObtenerAdjudicacion(int adjudicacionId);
    }
}
