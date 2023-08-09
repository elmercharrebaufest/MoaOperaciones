using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAplicacionCartaPorteService
    {
        Resultado Agregar(AplicacionCartaPorte aplicacionCCPP, string mailUsuario);
        List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones);
        AplicacionCartaPorteDto Obtener(int aplicacionCCPPId,string mailUsuario);
    }
}
