using SustitucionMOAModel.Dto;
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
        
        ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit);
        
        bool ValidarCuilChoferDigito(string cuilChofer);
        
        bool ValidarCuitTransporteDigito(string cuitTransporte);
        
        ProveedorDto ObtenerProveedor(int idProveedor);
        
        ValidarCamionResponse ValidarCamion(string patenteChasis, string patenteAcoplado);
    }
}
