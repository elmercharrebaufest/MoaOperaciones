using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi
{
    public class ScatoRepositorioClient : IScatoRepositorioClient
    {
        private static readonly string ScatoRepositorioBaseAddress = ConfigurationManager.AppSettings["ScatoRepositorioBaseAddress"];
        private static readonly string Username = ConfigurationManager.AppSettings["ScatoRepositorioUsername"];
        private static readonly string Password = ConfigurationManager.AppSettings["ScatoRepositorioPassword"];
        private static HttpClient cliente = new HttpClient { BaseAddress = new Uri(ScatoRepositorioBaseAddress) };


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

        private void InicializarCliente()
        {
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "U2NhdG9Mb2dpc3RpY2E6U2VydmljaW9FeHRlcm5vUGFzcw==");

            //request.DefaultRequestHeaders.Authorization = 
            //  new AuthenticationHeaderValue(
            //    "Basic", Convert.ToBase64String(
            //        System.Text.ASCIIEncoding.ASCII.GetBytes(
            //           $"{yourusername}:{yourpwd}")));

            cliente.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        ASCIIEncoding.ASCII.GetBytes($"{Username}:{Password}")));
        }
    }
}
