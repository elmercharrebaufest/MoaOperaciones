using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaGranosService : IAltaEmpresaGranosService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;

        public AltaEmpresaGranosService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];

        }

        public byte[] GenerarInformeComercial(ParamInformeComercial informeComercial)
        {
            try
            {
                var urlInformeComercial = string.Concat(DataAgroURL, "/InformeComercial/Listar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                //var url = "http://localhost:58280/api/AltaEmpresa/TEST";

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                ObtenerCampaniaActual(out string Campania, out int CampaniaId);

                informeComercial.Campaña = Campania;
                informeComercial.CampañaId = CampaniaId;

                string downloadKey = "";

                httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(informeComercial); //myDetails is my class object.
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(urlInformeComercial, byteContent);

                    task.Wait();

                    var response = task.Result;

                    var stringContent = response.Content.ReadAsStringAsync();

                    dynamic jsonResult = JObject.Parse(stringContent.Result);

                    downloadKey = jsonResult.DownloadKey;


                    urlReporte = string.Concat(urlReporte, "?key=", downloadKey);
                    WebClient clienteDescarga = new WebClient();
                    clienteDescarga.Credentials = new NetworkCredential(userName, password, dominio);

                    byte[] InformeComercialPDF = clienteDescarga.DownloadData(urlReporte);

                    return InformeComercialPDF;

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

        public void ObtenerCampaniaActual(out string CampaniaActual, out int CampaniaIdActual)
        {
            var urlBusquedaMateriales = string.Concat(DataAgroURL, "/Material/Buscar");

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

                CampaniaActual = jsonResult.Datos[0].CampaniaActual.ToString();
                CampaniaIdActual = jsonResult.Datos[0].CampaniaIdActual;
            }


        }

        public string ObtenerMaterialesDataAgro()
        {
            try
            {
                var urlBusquedaMateriales = string.Concat(DataAgroURL, "/Material/Buscar");

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

                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    return stringContent.Result;
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

        public bool GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario)
        {
            try
            {
                string fileName = Path.GetFileName(fileSubido.FileName);

                string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

                string rutaCarpeta = string.Concat(rutaArchivosProveedores, "/", usuario.CUITRegistro, "/", usuario.Id, "/", fileKey);

                string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

                Directory.CreateDirectory(rutaCarpeta);

                switch (fileKey)
                {
                    case "informeComercialFirmado":
                        usuario.RutaInformeComercialFirmado = rutaArchivo;
                        break;
                    case "constanciaCBU":
                        usuario.RutaConstanciaCBU = rutaArchivo;
                        break;
                    case "constanciaCUIT":
                        usuario.RutaConstanciaCUIT = rutaArchivo;
                        break;
                    case "inscripcionIIBB":
                        usuario.RutaInscripcionIIBB = rutaArchivo;
                        break;
                    case "gananciasIVAIIBB":
                        usuario.RutaGananciasIVAIIBB = rutaArchivo;
                        break;
                    case "SIPER":
                        usuario.RutaSIPER = rutaArchivo;
                        break;
                    case "documentacionEnBolsa":
                        usuario.RutaDocumentacionEnBolsa = rutaArchivo;
                        break;
                    default:
                        return false;
                }

                fileSubido.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
