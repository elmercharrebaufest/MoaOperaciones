using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAplicacionCartaPorteService
    {
        List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin);
        AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones);
        AplicacionCartaPorteDto Obtener(int aplicacionCCPPId,string mailUsuario);
        void EliminarAplicacion(int aplicacionId);
        List<ContratoParaAplicacionCartaPorte> ObtenerContratos(string mailUsuario);
        List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorte(string numeroContrato, string mailUsuario);
        void GuardarAplicacion(CrearAplicacionCartaPorte aplicacionACrear, string mailUsuario);
    }
}
