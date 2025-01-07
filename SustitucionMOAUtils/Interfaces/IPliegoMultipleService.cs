using SustitucionMOAModel.Dto.PliegoMultiple;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IPliegoMultipleService
    {
        List<PliegoDto> GetPliegosMultiples(string nombrePliego);

        List<SolpDto> GetSolpDisponiblesPliegosMultiple(string numeroSolp);
    }
}
