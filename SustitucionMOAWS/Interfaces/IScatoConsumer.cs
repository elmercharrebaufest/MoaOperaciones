using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.ScatoWebService;
using System.Collections.Generic;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoConsumer
    {
        List<CartaPorteFoto> ObtenerFotoCartaPorte(string cartaPorteId);
        List<CartaPorteFoto> ObtenerFotoCartasPorte(List<string> cartaPorteIds);
        List<LocalidadDto> ObtenerLocalidades();
        List<ProvinciaDto> ObtenerProvincias();
        List<KmPorProveedorDto> BuscarDestinos(string cuit);
        bool CuilChoferExiste(string cuil, bool logger = true);
        ValidarCuitExisteScatoResponse ExisteCuitDestinoDestinatario(string cuit, bool logger = true);
        ProveedorDto ObtenerProveedorPorCuit(string cuit);
        RecorridoDto[] ObtenerRecorridoNoRechazadoPorNumeroDocumento(string cuit);
    }
}
