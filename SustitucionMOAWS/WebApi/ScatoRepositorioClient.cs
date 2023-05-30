using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SustitucionMOAWS.WebApi
{
    public class ScatoRepositorioClient : IScatoRepositorioClient
    {
        private static readonly string ScatoRepositorioBaseAddress = ConfigurationManager.AppSettings["ScatoRepositorioBaseAddress"];
        private static readonly string Username = ConfigurationManager.AppSettings["ScatoRepositorioUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["ScatoRepositorioPassword"];
        private static HttpClient cliente = new HttpClient { BaseAddress = new Uri(ScatoRepositorioBaseAddress) };
        private static readonly string ScatoApi = "ScatoApi";


        public ConsultaListado<Planta> ObtenerPlantas(string cuitDestino)
        {
            InicializarCliente();

            var reqUri = $"AFIPApi/ConsultarPlantasDGPorCUIT/{cuitDestino}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var plantasResponse = response.Content.ReadAsAsync<ConsultaListado<Planta>>().GetAwaiter().GetResult();
                return plantasResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
            }
        }

        public ConsultaListado<Domicilio> ObtenerDomicilios(string cuitDestino)
        {
            InicializarCliente();

            var reqUri = $"AFIPApi/ConsultarDomiciliosPorCUIT/{cuitDestino}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var domiciliosResponse = response.Content.ReadAsAsync<ConsultaListado<Domicilio>>().GetAwaiter().GetResult();
                return domiciliosResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
            }
        }

        public ConsultaListado<ChoferDto> ObtenerChoferPorCuil(string cuilChofer)
        {
            InicializarCliente();

            var reqUri = $"{ScatoApi}/ObtenerChoferPorCuit/{cuilChofer}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var choferResponse = response.Content.ReadAsAsync<ConsultaListado<ChoferDto>>().GetAwaiter().GetResult();
                return choferResponse;
            }
            else
            {
                throw new Exception("Error en api Scato " + response.StatusCode);
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
                        System.Text.ASCIIEncoding.ASCII.GetBytes($"{Username}:{Password}")));
        }
    }
}
