using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Text;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using SustitucionMOAWS.Logger;

namespace SustitucionMOAWS.WebApi
{
    public class ScatoRepositorioClient : IScatoRepositorioClient
    {
        private const string ObtenerProveedorPorCuil_TipoProveedor = "PR";
        private const string ObtenerProveedorPorCuil_TipoCorredor = "CM";

        private static readonly string ScatoRepositorioBaseAddress = ConfigurationManager.AppSettings["ScatoRepositorioBaseAddress"];
        private static readonly string Username = ConfigurationManager.AppSettings["ScatoRepositorioUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["ScatoRepositorioPassword"];
        private static readonly string ScatoApi = "ScatoApi";
        private static readonly string AfipApi = "AFIPApi";
        private readonly HttpClient cliente;

        public ScatoRepositorioClient()
        {
            Log.Info("Instancia e inicializa cliente API ScatoRepositorio");
            this.cliente = new HttpClient { BaseAddress = new Uri(ScatoRepositorioBaseAddress) };
            InicializarCliente();
        }

        public RespuestaListado<Planta> ObtenerPlantas(string cuitDestino)
        {
            var reqUri = $"{AfipApi}/ConsultarPlantasDGPorCUIT/{cuitDestino}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var plantasResponse = response.Content.ReadAsAsync<RespuestaListado<Planta>>().GetAwaiter().GetResult();
                return plantasResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
            }
        }

        public RespuestaListado<Domicilio> ObtenerDomicilios(string cuitDestino)
        {
            var reqUri = $"{AfipApi}/ConsultarDomiciliosPorCUIT/{cuitDestino}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var domiciliosResponse = response.Content.ReadAsAsync<RespuestaListado<Domicilio>>().GetAwaiter().GetResult();
                return domiciliosResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
            }
        }

        public Respuesta<Chofer> ObtenerChoferPorCuil(string cuilChofer)
        {
            var reqUri = $"{ScatoApi}/ObtenerChoferPorCuil/{cuilChofer}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var choferResponse = response.Content.ReadAsAsync<Respuesta<Chofer>>().GetAwaiter().GetResult();
                return choferResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
            }
        }

        public ObtenerProveedorPorCuilResponse ObtenerProveedorPorCuil(string cuil)
        {
            var tipo = ObtenerProveedorPorCuil_TipoProveedor;
            var cuilGuiones = DataFormatter.CuitConGuion(cuil);
            var jsonRes = string.Empty;

            try
            {
                var reqUri = $"{ScatoApi}/ObtenerProveedorPorCuil/{cuilGuiones}/{tipo}";
                HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    jsonRes = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var proveedorResponse = JsonConvert.DeserializeObject<ObtenerProveedorPorCuilResponse>(jsonRes);
                    return proveedorResponse;
                }
                else
                {
                    throw new Exception($"Respuesta API Scato: {response.StatusCode}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error en API Scato. CUIL: {cuil}. CUIL guiones: {cuilGuiones}. Respuesta: {jsonRes}.");
                throw new Exception("Error en API Scato ObtenerProveedorPorCuil");
            }
        }


        private void InicializarCliente()
        {
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{Username}:{Password}")));
        }
    }
}
