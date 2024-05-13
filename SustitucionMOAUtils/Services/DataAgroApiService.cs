using Kendo.DynamicLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SustitucionMOAUtils.Services
{
    public class DataAgroApiService : IDataAgroApiService
    {

        protected readonly IRepositorio repositorio;
        protected readonly ICache cache;
        private readonly string DataAgroURL;
        public DataAgroApiService(IRepositorio repositorio, ICache cache)
        {
            this.repositorio = repositorio;
            this.cache = cache;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }

        public string CrearContratoAPrecio(ContratoAPrecio contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAPrecio");
                var content = JsonConvert.SerializeObject(contratoAPrecio);
                return ConsultarDataAaro(url, content);
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

        public string ObteneDatosContrato(int tiponegocio)
        {

            try
            {
                var url = string.Concat(DataAgroURL, "/Compranet/InicializarContrato");
                var content = JsonConvert.SerializeObject(new { tipoNegocioId = tiponegocio });
                return ConsultarDataAaro(url, content);                
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

        public string ObtenerDatosCompraNet(int proveedorId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ObtenerDatosCompraNet");
                var content = JsonConvert.SerializeObject(new { id = proveedorId });

                return ConsultarDataAaro(url, content);
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

        public string CrearContratoAFijar(ContratoAFijar contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAFijar");
                var content = JsonConvert.SerializeObject(contratoAPrecio);
                return ConsultarDataAaro(url, content);
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

        public string ValidarDirecto(string cuit)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/ValidarDirecto");
                var content = JsonConvert.SerializeObject(new { cuit = cuit });
                return ConsultarDataAaro(url, content);

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
                Log.Debug(e.InnerException != null ? e.InnerException.Message : e.Message, cuit ?? "", "ValidarDirecto");
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string BuscarProveedoresConCorredor(string filtro, string cuitCorredor)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Proveedor/BuscarProveedoresConCorredor");
                var content = JsonConvert.SerializeObject(new { filtroProveedor = filtro, filtro = cuitCorredor });

                return ConsultarDataAaro(url, content);
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

        public string HabilitarPizarra(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarPizarra");
                var content = JsonConvert.SerializeObject(new { material = material, tiponegocio = tiponegocio });
                return ConsultarDataAaro(url, content);

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

        public string TraerPagosDiferido(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerPagosDiferido");
                var content = JsonConvert.SerializeObject(new { tipoNegocio = tiponegocio, material = material });
                return ConsultarDataAaro(url, "");

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

        public string HabilitarCampaña(int material)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarCampaña");
                var content = JsonConvert.SerializeObject(new { material = material });
                return ConsultarDataAaro(url, content);

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

        public string TraerPrecioMoa(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerPrecioMoa");
                var content = JsonConvert.SerializeObject(new { material = material, tiponegocio = tiponegocio });
                return ConsultarDataAaro(url, content);
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

        public string ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ObtenerFijacionesAutomaticas");
                var content = JsonConvert.SerializeObject(new { cuitProveedor, cuitCorredor, materialId, filtro, fijacionId });
                return ConsultarDataAaro(url, content);
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

        public string CrearContratoFijacion(ContratoFijacion contratoFijacion)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarFijacion");
                var content = JsonConvert.SerializeObject(contratoFijacion);
                return ConsultarDataAaro(url, content);
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
        public string GetContratos(DataSourceRequest request)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Contrato/BuscaDatosTabla");
                var content = JsonConvert.SerializeObject(request);

                return ConsultarDataAaro(url, content);
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

        public string ValidarProveedor(string proveedorId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ValidarProveedor");
                var content = JsonConvert.SerializeObject(new { proveedorId = proveedorId });

                return ConsultarDataAaro(url, content);
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

        public string TraerPrecioMoaMateriales(int tipoNegocioId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/TraerPrecioMoa");
                var content = JsonConvert.SerializeObject(new { tipoNegocioId = tipoNegocioId });
                return ConsultarDataAaro(url, content);

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

        public List<MaterialDto> BuscarMateriales()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Material/Buscar");
                JObject json = JObject.Parse(ConsultarDataAaro(url, ""));
                var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<MaterialDto>>();

                return data;                
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

        public List<CentroDto> BuscarCentros()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Centro/Buscar");

                JObject json = JObject.Parse(ConsultarDataAaro(url, ""));
                var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<CentroDto>>();

                return data;
                
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

        public List<CampaniaDto> BuscarCampanias()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Campana/Buscar");
                string stringResult = ConsultarDataAaro(url, "");
                var data = JsonConvert.DeserializeObject<List<CampaniaDto>>(stringResult);

                return data;
                
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

        public string ObteneContratosAcuerdo(int idDataAgro)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerContratosAcuerdoPorCorredor");
                var content = JsonConvert.SerializeObject(new { corredorId = idDataAgro });
                return ConsultarDataAaro(url, content);
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

        private string ConsultarDataAaro(string url, string content)
        {
            CookiesDataAgro cookiesDataAgro = new CookiesDataAgro();
            if (cache.Existe("CookiesDataAgro"))
            {
                cookiesDataAgro = cache.Obtener<CookiesDataAgro>("CookiesDataAgro");
                if (!cookiesDataAgro.esValida)
                {
                    cache.Remover("CookiesDataAgro");
                    cookiesDataAgro = Retry.Do(generarCookies, TimeSpan.FromSeconds(1), 4);
                    cache.Agregar("CookiesDataAgro", cookiesDataAgro, DateTime.Now.AddHours(7));
                }
            }
            else
            {
                cookiesDataAgro = Retry.Do(generarCookies, TimeSpan.FromSeconds(1), 4);
                cache.Agregar("CookiesDataAgro", cookiesDataAgro, DateTime.Now.AddHours(7));
            }
            var responseCookies = new List<KeyValuePair<string, string>>();

            foreach (var item in cookiesDataAgro.Cookies)
            {
                responseCookies.Add(new KeyValuePair<string, string>(item.Key, item.Value));
            }
            var baseAddress = new Uri(url);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client2 = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                foreach (var cookie in responseCookies)
                {
                    cookieContainer.Add(baseAddress, new Cookie(cookie.Key, cookie.Value));
                }
                var buffer2 = Encoding.UTF8.GetBytes(content);
                var byteContent2 = new ByteArrayContent(buffer2);
                byteContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var task2 = client2.PostAsync(url, byteContent2);
                task2.Wait();
                var stringContent2 = task2.Result.Content.ReadAsStringAsync();
                string scapedJson2 = stringContent2.Result.Replace("ñ", "ni");
                return scapedJson2;
            }
        }

        private CookiesDataAgro generarCookies()
        {
            try
            {
                CookiesDataAgro cookiesDataAgro = new CookiesDataAgro();

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();
                var url = string.Concat(DataAgroURL, "/Material/Buscar");
                var httpClientHandler = new HttpClientHandler()
                {
                    UseDefaultCredentials = false,
                    PreAuthenticate = true,
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                CookieContainer cookies = new CookieContainer();
                httpClientHandler.CookieContainer = cookies;
                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, null);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    JObject json = JObject.Parse(scapedJson);
                    var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<MaterialDto>>();
                }
                Uri uri = new Uri(url);
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                foreach (var item in responseCookies)
                {
                    cookiesDataAgro.Cookies.Add(new KeyValuePair<string, string>(item.Name, item.Value));
                }
                cookiesDataAgro.Fecha = DateTime.Now;
                return cookiesDataAgro;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public BasicoContrato TraerContratoCompleto(int id, string tipo)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/TraerContratoCompleto");
                var content = JsonConvert.SerializeObject(new { id = id, tipo = tipo });
                var scapedJson = ConsultarDataAaro(url, content);
                var data = JsonConvert.DeserializeObject<BasicoContrato>(scapedJson);

                return data;

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

        public List<GrabarContratoResult> CrearContratoMasivo(List<BasicoContrato> contratos)
        {
            try
            {
                //var datatest = JsonConvert.DeserializeObject<List<GrabarContratoResult>>("[{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},{\"ContratoId\":null,\"Errores\":[{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"El cuit existe.\",\"Source\":\"Proveedor\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"ListaErrores\":[{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"El cuit existe.\",\"Source\":\"Proveedor\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"HayError\":true,\"HayErrores\":true},{\"ContratoId\":2,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},{\"ContratoId\":3,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false}]");
                //return datatest;
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoMasivo");
                var content = JsonConvert.SerializeObject(contratos);
                var scapedJson = ConsultarDataAaro(url, content);
                var data = JsonConvert.DeserializeObject<List<GrabarContratoResult>>(scapedJson);

                return data;
                
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

        public string AnularNegocio(int negocioId, int tipoNegocioId, string motivo)
        {
            try
            {
                var url = "";
                if (tipoNegocioId == 1 || tipoNegocioId == 2)
                {
                    url = string.Concat(DataAgroURL, "/CompraNetTercero/AnularContrato");
                }
                else if (tipoNegocioId == 3)
                {
                    url = string.Concat(DataAgroURL, "/CompraNetTercero/AnularFijacion");
                }
                else
                {
                    throw new ValidationCustomException("No se puede anular este tipo de negocios.");
                }
                var content = JsonConvert.SerializeObject(new { negocioId = negocioId, MotivoRechazo = motivo });
                return ConsultarDataAaro(url, content);

                
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

        public string TraerContratoCompleto(int negocioId, int tipoNegocioId)
        {
            try
            {
                var url = "";
                if (tipoNegocioId == 1 || tipoNegocioId == 2)
                {
                    url = string.Concat(DataAgroURL, "/CompraNet/TraerContratoCompleto");
                }
                else if (tipoNegocioId == 3)
                {
                    url = string.Concat(DataAgroURL, "/CompraNet/TraerFijacionCompleto");
                }
                else
                {
                    throw new NotImplementedException();
                }
                var content = JsonConvert.SerializeObject(new { id = negocioId });

                return ConsultarDataAaro(url, content);

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

        public string ConfiguracionBolsaAutomatica()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/ConfiguracionBolsa/DatosConfiguracion");
                var content = JsonConvert.SerializeObject(new { Page = 1, PageSize = 1000, Take = 1000, Skip = 0 });

                return ConsultarDataAaro(url, content);

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

        public byte[] ExcelModeloAltaMasiva()
        {
            byte[] reporte;
            var url = string.Concat(DataAgroURL, "/ReporteCompraNet/ExcelModeloAltaMasiva");
            string userName = DataAgroWSCredential.getUserName();
            string password = DataAgroWSCredential.getPassword();
            string dominio = DataAgroWSCredential.getDominio();
            using (WebClient clienteDescarga = new WebClient())
            {
                clienteDescarga.Credentials = new NetworkCredential(userName, password, dominio);
                reporte = clienteDescarga.DownloadData(url);
            }

            return reporte;
        }

        public string TraerHabilitarSustentable( )
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarSustentable");
                return ConsultarDataAaro(url, "");

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
        public List<LocalidadDto> ListarLocalidades()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Localidad/ListarLocalidades");
                var respuesta =  ConsultarDataAaro(url, "");


                return JsonConvert.DeserializeObject<List<LocalidadDto>>(respuesta);
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

        public List<PartidoDto> ListarPartidos()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Localidad/ListarPartidos");
                var respuesta = ConsultarDataAaro(url, "");


                return JsonConvert.DeserializeObject<List<PartidoDto>>(respuesta);
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
