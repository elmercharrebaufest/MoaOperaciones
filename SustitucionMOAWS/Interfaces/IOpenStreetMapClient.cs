using System.Threading;
using System.Threading.Tasks;
using SustitucionMOAWS.WebApi.OpenStreetMap.Response;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOpenStreetMapClient
    {
        Task<LugaresOSMResponse> BuscarLugaresSegunDireccionAsync(string direccion);
    }
}
