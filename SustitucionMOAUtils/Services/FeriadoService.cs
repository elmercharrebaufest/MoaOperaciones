using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class FeriadoService : IFeriadoService
    {

        private readonly string DataAgroURL;

        public FeriadoService()
        {

            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }


        public List<DateTime> ObtenerFeriados()
        {
            try
            {
                if (DataAgroURL.Contains("test"))
                {
                    var feriados = new List<DateTime>();
                    string result = "{\"Datos\":[{\"Id\":1,\"Feriado\":\"\\/Date(1577847600000)\\/\"},{\"Id\":2,\"Feriado\":\"\\/Date(1583031600000)\\/\"},{\"Id\":3,\"Feriado\":\"\\/Date(1585710000000)\\/\"},{\"Id\":5,\"Feriado\":\"\\/Date(1588215600000)\\/\"},{\"Id\":4,\"Feriado\":\"\\/Date(1588302000000)\\/\"}],\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false}";
                    dynamic json = JObject.Parse(result);
                    for (int i = 0; i < json.Datos.Count; i++)
                    {
                        DateTime fecha = json.Datos[i].Feriado;
                        feriados.Add(fecha);
                    }
                    return feriados;
                }


                var urlBusquedaMateriales = string.Concat(DataAgroURL, "/FechaFeriado/Buscar");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(urlBusquedaMateriales, null);

                    task.Wait();

                    var stringContent = task.Result.Content.ReadAsStringAsync();

                    dynamic jsonResult = JObject.Parse(stringContent.Result);


                    for (int i = 0; i < jsonResult.Datos.Count; i++)
                    {
                        DateTime fecha = jsonResult.Datos[i].Feriado;
                        feriados.Add(fecha);
                    }
                    return feriados;
                }
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }


    }
}
