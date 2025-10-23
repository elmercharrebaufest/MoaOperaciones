using SustitucionMOAModel.Dto.PliegoMultiple;
using System;
using System.Collections.Generic;
using System.Web;
using ComprasDto = SustitucionMOAModel.Dto;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IPliegoMultipleService
    {
        List<PliegoPMDto> GetPliegosMultiples(string nombrePliego);

        List<SolpPMDto> GetSolpDisponiblesPliegosMultiple(string numeroSolp,
                                                        string nombrePliego,
                                                        DateTime? fechaInicio,
                                                        DateTime? fechaFin,
                                                        IEnumerable<int> creador,
                                                        IEnumerable<string> fiscal,
                                                        bool sap,
                                                        bool mantenimiento,
                                                        bool web = false,
                                                        bool repoAutomatica = false,
                                                        bool contratoMarco = false,
                                                        bool incluirGuardadas = false,
                                                        int? pliegoId = null);

        void CrearPliegoMultiple(ComprasDto.SolpDto pliegoData, HttpFileCollectionBase adjuntos, IEnumerable<int> solpsAsociar);

        void EliminarPliegoMultiple(int idPliego);

        string GenerarZipPliego(int idPliego, string pathBase, out string mimeType);

        TraerPliegoPMDto TraerPliegoId(int idPliego);
    }
}
