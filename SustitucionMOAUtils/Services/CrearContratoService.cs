using Kendo.DynamicLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SustitucionMOAUtils.Services
{
    public class CrearContratoService : ICrearContratoService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;
        public CrearContratoService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }

        public string CrearContratoAPrecio(ContratoAPrecio contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAPrecio");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratoAPrecio);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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

        public string ObteneDatosContrato(int tiponegocio)
        {

            try
            {
                var url = string.Concat(DataAgroURL, "/Compranet/InicializarContrato");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };
                var content = JsonConvert.SerializeObject(new { tipoNegocioId = tiponegocio });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);

                    task.Wait();

                    var stringContent = task.Result.Content.ReadAsStringAsync();

                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    return scapedJson;

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

        public string ObtenerDatosCompraNet(int proveedorId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ObtenerDatosCompraNet");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { id = proveedorId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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

        public string CrearContratoAFijar(ContratoAFijar contratoAPrecio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoAFijar");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratoAPrecio);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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


        public string ValidarDirecto(string cuit)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/ValidarDirecto");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { cuit = cuit });

                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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
                Log.Debug(e.InnerException != null ? e.InnerException.Message : e.Message, cuit ?? "", "ValidarDirecto");
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string BuscarProveedoresConCorredor(string filtro, string cuitCorredor)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Proveedor/BuscarProveedoresConCorredor");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { filtroProveedor = filtro, filtro = cuitCorredor });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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

        public string HabilitarPizarra(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarPizarra");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { material = material, tiponegocio = tiponegocio });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string TraerPagosDiferido(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerPagosDiferido");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { tipoNegocio = tiponegocio, material = material });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string HabilitarCampaña(int material)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarCampaña");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { material = material });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string TraerPrecioMoa(int material, int tiponegocio)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerPrecioMoa");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { material = material, tiponegocio = tiponegocio });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ObtenerFijacionesAutomaticas");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { cuitProveedor, cuitCorredor, materialId, filtro, fijacionId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string CrearContratoFijacion(ContratoFijacion contratoFijacion)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarFijacion");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratoFijacion);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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
        public string GetContratos(DataSourceRequest request)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Contrato/BuscaDatosTabla");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(request);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return result;
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

        public string ValidarProveedor(string proveedorId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/ValidarProveedor");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { proveedorId = proveedorId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var result = stringContent.Result;
                    return scapedJson;
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

        public string TraerPrecioMoaMateriales(int tipoNegocioId)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/TraerPrecioMoa");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { tipoNegocioId = tipoNegocioId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public List<MaterialDto> BuscarMateriales()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Material/Buscar");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, null);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    JObject json = JObject.Parse(scapedJson);
                    var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<MaterialDto>>();

                    return data;
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

        public List<CentroDto> BuscarCentros()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Centro/Buscar");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, null);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    JObject json = JObject.Parse(scapedJson);
                    var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<CentroDto>>();

                    return data;
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

        public List<CampaniaDto> BuscarCampanias()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/Campana/Buscar");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };


                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, null);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var data = JsonConvert.DeserializeObject<List<CampaniaDto>>(scapedJson);

                    return data;
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

        public string ObteneContratosAcuerdo(int idDataAgro)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/TraerContratosAcuerdoPorCorredor");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };
                var content = JsonConvert.SerializeObject(new { corredorId = idDataAgro });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);

                    task.Wait();

                    var stringContent = task.Result.Content.ReadAsStringAsync();

                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    return scapedJson;

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

        public BasicoContrato TraerContratoCompleto(int id,string tipo)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNet/TraerContratoCompleto");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };
                var content = JsonConvert.SerializeObject(new { id = id, tipo = tipo });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);

                    task.Wait();

                    var stringContent = task.Result.Content.ReadAsStringAsync();

                    string scapedJson = stringContent.Result.Replace("ñ", "ni");

                    var data = JsonConvert.DeserializeObject<BasicoContrato>(scapedJson);

                    return data;
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

        public List<GrabarContratoResult> CrearContratoMasivo(List<BasicoContrato> contratos)
        {
            try
            {
                //var datatest = JsonConvert.DeserializeObject<List<GrabarContratoResult>>("[{\"ContratoId\":1,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},{\"ContratoId\":null,\"Errores\":[{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"El cuit existe.\",\"Source\":\"Proveedor\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"ListaErrores\":[{\"Item\":0,\"ErrorCode\":0,\"LogId\":0,\"Message\":\"El cuit existe.\",\"Source\":\"Proveedor\",\"LargeDescription\":\"\",\"Translate\":false,\"Format\":\"\",\"Args\":[]}],\"HayError\":true,\"HayErrores\":true},{\"ContratoId\":2,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false},{\"ContratoId\":3,\"Errores\":[],\"ListaErrores\":[],\"HayError\":false,\"HayErrores\":false}]");
                //return datatest;
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/GrabarContratoMasivo");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(contratos);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    var data = JsonConvert.DeserializeObject<List<GrabarContratoResult>>(scapedJson);

                    return data;
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


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { negocioId = negocioId, MotivoRechazo = motivo });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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
                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { id = negocioId });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public string ConfiguracionBolsaAutomatica()
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/ConfiguracionBolsa/DatosConfiguracion");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                var content = JsonConvert.SerializeObject(new { Page = 1, PageSize = 1000, Take = 1000, Skip = 0 });
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(url, byteContent);
                    task.Wait();
                    var stringContent = task.Result.Content.ReadAsStringAsync();
                    string scapedJson = stringContent.Result.Replace("ñ", "ni");
                    return scapedJson;
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

        public byte[] ExcelModeloAltaMasiva() {
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
    }
}
