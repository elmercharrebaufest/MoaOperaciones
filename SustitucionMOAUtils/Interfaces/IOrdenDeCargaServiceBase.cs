using SustitucionMOAModel.Dto.OrdenDeCarga;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaServiceBase
    {
        ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit);

        bool ValidarSisa(string cuitDestinatario, string cuitDestino, string codigoMaterial);

        List<PlantaDto> ObtenerPlantasDestino(string destinoCuit);

        List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit);

        bool ValidarCuitRuca(string cuit);
        bool EmailGestionarAlta(string cuit, string razonSocial, bool esIntermediarioFlete);
        ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit);
    }
}
