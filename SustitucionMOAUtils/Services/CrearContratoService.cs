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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/GrabarContratoAPrecio";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNet/InicializarContrato";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNet/ObtenerDatosCompraNet";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/GrabarContratoAFijar";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/ValidarDirecto";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/Proveedor/BuscarProveedoresConCorredor";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/HabilitarPizarra";
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

        public string HabilitarCampaña(int material)
        {
            try
            {
                var url = string.Concat(DataAgroURL, "/CompraNetTercero/HabilitarCampaña");


                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/HabilitarCampaña";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/TraerPrecioMoa";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNet/ObtenerFijacionesAutomaticas";
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

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //url = "http://localhost:52498/CompraNetTercero/GrabarFijacion";
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
    }
}
