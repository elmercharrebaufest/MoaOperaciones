using Newtonsoft.Json;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WebApi.OSRM.Common;
using SustitucionMOAWS.WebApi.OSRM.Response;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM
{
    public class OsrmApiClient : IOsrmApiClient
    {
        //private static readonly string Service = "route"; // Service: route, nearest, table, match, trip, tile
        //private static readonly string Version = "v1";
        //private static readonly string Profile = "car"; // car, bike, foot
        //private static readonly string Formato = "json";
        //private static readonly string Options = null;

        private readonly HttpClient _clienteHttp;

        public OsrmApiClient()
        {
            _clienteHttp = new HttpClient { BaseAddress = new Uri("http://router.project-osrm.org/") };
        }

        public OsrmApiClient(HttpClient httpClient)
        {
            this._clienteHttp = httpClient;
        }

        
        public RutaOSRMResponse ObtenerRuta(GeoCoordenada coordOrigen, GeoCoordenada coordDestino) //public async Task<RutaOSRMResponse> ObtenerRutaAsync(GeoCoordenada coordOrigen, GeoCoordenada coordDestino)
        {
            try
            {
                var rutaApi = $"route/v1/driving/{coordOrigen.Valor};{coordDestino.Valor}.json?overview=false";

                var responseMessage = _clienteHttp.GetAsync(rutaApi).ConfigureAwait(false).GetAwaiter().GetResult(); //var responseMessage = await _clienteHttp.GetAsync(rutaApi).ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonResponse = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult(); //var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    var rutaOSRMResponse = JsonConvert.DeserializeObject<RouteResponse<GeoJsonGeometry>>(jsonResponse);

                    return new RutaOSRMResponse
                    {
                        JsonResponseRaw = jsonResponse,
                        RouteResponse = rutaOSRMResponse
                    };
                }
                else
                {
                    throw new Exception($"Error en respuesta OpenSourceRoutingMachine. StatusCode={responseMessage.StatusCode}. ReasonPhrase={responseMessage.ReasonPhrase}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al obtener rutas desde OpenSourceRoutingMachine con coordenadas {coordOrigen.Valor};{coordDestino.Valor}");
                return null;
            }
        }
    }
}
