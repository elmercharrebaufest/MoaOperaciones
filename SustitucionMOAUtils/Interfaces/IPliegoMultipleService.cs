using SustitucionMOAModel.Dto.PliegoMultiple;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IPliegoMultipleService
    {
        List<PliegoDto> GetPliegosMultiples(string nombrePliego);

        List<SolpDto> GetSolpDisponiblesPliegosMultiple(string numeroSolp,
                                                        DateTime? fechaInicio,
                                                        DateTime? fechaFin,
                                                        string creador,
                                                        string fiscal,
                                                        bool sap,
                                                        bool mantenimiento);
    }
}
