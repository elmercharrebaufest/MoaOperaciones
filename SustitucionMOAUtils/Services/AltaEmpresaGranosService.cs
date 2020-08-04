using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        public byte[] GenerarInformeComercial(string mailUsuario, string EmplRelDep, string EmplRelDepCant, string Rodados, string RodadosOtros, string Chacra, string ChacraOtros, 
                                            string AntigActividad, string ActuacionProd, string ClienteAnt, string Comentarios, string Domicilio)
        {

            try
            {
                var urlInformeComercial = string.Concat(DataAgroURL, "/InformeComercial/Listar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                mailUsuario = mailUsuario.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mailUsuario;

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

                var proveedor = usuario.ObtenerProveedorActual();
                //var url = "http://localhost:58280/api/AltaEmpresa/TEST";

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };
                /*
                 *             string ProveedorId =  "1239";
                string CampañaId = "8";
                string Campaña = "19-20";
                string ComercialID = "10";

                 */

                ObtenerCampaniaActual(out string Campania, out string CampaniaId);

                string ProveedorId = proveedor.IdDataAgro.ToString();
                string ComercialID = proveedor.IdComercialDataAgro.ToString();

                /* EmplRelDep = EmplRelDep.IsNullOrWhiteSpace() ? "" : EmplRelDep;
                 EmplRelDepCant = EmplRelDepCant.IsNullOrWhiteSpace() ? "" : EmplRelDepCant;
                 Rodados = Rodados.IsNullOrWhiteSpace() ? "" : Rodados;
                 RodadosOtros = RodadosOtros.IsNullOrWhiteSpace() ? "" : RodadosOtros;
                 Chacra = Chacra.IsNullOrWhiteSpace() ? "" : Chacra;
                 ChacraOtros = ChacraOtros.IsNullOrWhiteSpace() ? "" : ChacraOtros;
                 AntigActividad = AntigActividad.IsNullOrWhiteSpace() ? "" : AntigActividad;
                 ActuacionProd = ActuacionProd.IsNullOrWhiteSpace() ? "" : ActuacionProd;
                 ClienteAnt = ClienteAnt.IsNullOrWhiteSpace() ? "" : ClienteAnt;
                 Comentarios = Comentarios.IsNullOrWhiteSpace() ? "" : Comentarios;
                 Domicilio = Domicilio.IsNullOrWhiteSpace() ? "" : Domicilio;*/

                var formContent = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("ProveedorId", ProveedorId),
                new KeyValuePair<string, string>("CampañaId", CampaniaId),
                new KeyValuePair<string, string>("Campaña", Campania),
                new KeyValuePair<string, string>("EmplRelDep", EmplRelDep),
                new KeyValuePair<string, string>("EmplRelDepCant", EmplRelDepCant),
                new KeyValuePair<string, string>("Rodados", Rodados),
                new KeyValuePair<string, string>("RodadosOtros", RodadosOtros),
                new KeyValuePair<string, string>("Chacra", Chacra),
                new KeyValuePair<string, string>("ChacraOtros", ChacraOtros),
                new KeyValuePair<string, string>("AntigActividad", AntigActividad),
                new KeyValuePair<string, string>("ActuacionProd", ActuacionProd),
                new KeyValuePair<string, string>("ClienteAnt", ClienteAnt),
                new KeyValuePair<string, string>("Comentarios", Comentarios),
                new KeyValuePair<string, string>("Domicilio", Domicilio),
                new KeyValuePair<string, string>("InformeComercialId", "0"),
                new KeyValuePair<string, string>("ComercialID", ComercialID)

            });

                string downloadKey = "";

                httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    try
                    {
                        var task = client.PostAsync(urlInformeComercial, formContent);

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
                    catch (Exception ex)
                    {

                    }
                }
                return null;
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

        public void ObtenerCampaniaActual(out string CampaniaActual, out string CampaniaIdActual)
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
                CampaniaIdActual = jsonResult.Datos[0].CampaniaIdActual.ToString();
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
            catch(Exception ex)
            {
                return false;
            }
        }

    }
}
