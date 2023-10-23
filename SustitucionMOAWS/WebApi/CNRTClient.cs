using SustitucionMOAModel.Models.WebApiMap.CNRT;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi
{
    public class CNRTClient : ICNRTClient
    {
        private static readonly string CNRTBaseAddress = "https://api.cnrt.gob.ar/dut/v1/public/";
        private readonly HttpClient cliente;

        public CNRTClient()
        {
            Log.Info("Instancia e inicializa cliente API ScatoRepositorio");
            this.cliente = new HttpClient { BaseAddress = new Uri(CNRTBaseAddress) };
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public EquiposResponse ObtenerEquipos(string patenteChasis, string patenteAcoplado)
        {
            var reqUri = $"equipos?dominios={patenteChasis},{patenteAcoplado}";
            HttpResponseMessage response = cliente.GetAsync(reqUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var equiposResponse = response.Content.ReadAsAsync<EquiposResponse>().GetAwaiter().GetResult();
                if (equiposResponse.Result != "ok")
                {
                    Log.Error($"Error en API CNRT con patentes {patenteChasis} y {patenteAcoplado}. Respuesta CNRT: {equiposResponse.ToJson()}.");
                    throw new Exception("Error al consultar CNRT");
                }
                return equiposResponse;
            }
            else
            {
                throw new Exception("Error en api CNRT " + response.StatusCode);
            }
        }
    }
}
