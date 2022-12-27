using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAplicacionCartaPorteService
    {
        Resultado Agregar(AplicacionCartaPorte aplicacionCCPP, string mailUsuario);
        List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        AplicacionCartaPorteDto Obtener(int aplicacionCCPPId,string mailUsuario);
    }
}
