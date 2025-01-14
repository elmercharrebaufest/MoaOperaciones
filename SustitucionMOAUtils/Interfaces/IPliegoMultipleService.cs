using SustitucionMOAModel.Dto.PliegoMultiple;
using System;
using System.Collections.Generic;
using System.Web;
using ComprasDto = SustitucionMOAModel.Dto;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IPliegoMultipleService
    {
        List<PliegoDto> GetPliegosMultiples(string nombrePliego);

        List<SolpDto> GetSolpDisponiblesPliegosMultiple(string numeroSolp,
                                                        DateTime? fechaInicio,
                                                        DateTime? fechaFin,
                                                        IEnumerable<int> creador,
                                                        IEnumerable<string> fiscal,
                                                        bool sap,
                                                        bool mantenimiento);

        void CrearPliegoMultiple(ComprasDto.SolpDto pliegoData, HttpFileCollectionBase adjuntos, IEnumerable<int> solpsAsociar);
    }
}
