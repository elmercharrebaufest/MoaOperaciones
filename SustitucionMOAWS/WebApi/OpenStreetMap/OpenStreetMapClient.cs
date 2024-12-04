using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.WebApi.OpenStreetMap.Response;

namespace SustitucionMOAWS.WebApi.OpenStreetMap
{
    public class OpenStreetMapClient : IOpenStreetMapClient
    {
        private static readonly string ConsultaOpenStreetMapHabilitada = ConfigurationManager.AppSettings["ConsultaOpenStreetMapHabilitada"];
        private static readonly string OpenStreetMapApiBaseAddress = ConfigurationManager.AppSettings["OpenStreetMapApiBaseAddress"];

        private readonly HttpClient _clienteHttp;

        public OpenStreetMapClient()
        {
            _clienteHttp = new HttpClient { BaseAddress = new Uri(OpenStreetMapApiBaseAddress) };
            _clienteHttp.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
        }

        public OpenStreetMapClient(HttpClient httpClient)
        {
            this._clienteHttp = httpClient;
        }

        public async Task<LugaresOSMResponse> BuscarLugaresSegunDireccionAsync(string direccion)
        {
            try
            {
                if (!bool.TryParse(ConsultaOpenStreetMapHabilitada, out bool habilitado) || !habilitado)
                {
                    return null;
                }
                var rutaApi = $"search?q={direccion}&format=json";

                var responseMessage = await _clienteHttp.GetAsync(rutaApi).ConfigureAwait(false);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
                    var lugaresOSM = JsonConvert.DeserializeObject<List<OSMPlace>>(jsonResponse);

                    return new LugaresOSMResponse
                    {
                        JsonResponseRaw = jsonResponse,
                        Lugares = lugaresOSM
                    };
                }
                else
                {
                    throw new Exception($"Error en respuesta OpenStreetMap. StatusCode={responseMessage.StatusCode}. ReasonPhrase={responseMessage.ReasonPhrase}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al obtener lugares desde OpenStreetMap con dirección " + direccion);
                return null;
            }
            
        }
    }
}
