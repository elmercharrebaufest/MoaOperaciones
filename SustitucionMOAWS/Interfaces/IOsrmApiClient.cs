using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.WebApi.OSRM.Response;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOsrmApiClient
    {
        //Task<RutaOSRMResponse> ObtenerRutaAsync(GeoCoordenada origen, GeoCoordenada destino);
        RutaOSRMResponse ObtenerRuta(GeoCoordenada origen, GeoCoordenada destino);
    }
}
