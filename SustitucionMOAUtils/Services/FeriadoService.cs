using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;

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
                var feriados = new List<DateTime>();
              
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
