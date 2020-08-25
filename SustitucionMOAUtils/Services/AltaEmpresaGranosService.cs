using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz.Util;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
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
    public class AltaEmpresaGranosService : IAltaEmpresaGranosService
    {

        protected readonly IRepositorio repositorio;
        private readonly string DataAgroURL;

        public AltaEmpresaGranosService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }

        public byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario)
        {
            try
            {
                var urlInformeComercial = string.Concat(DataAgroURL, "/InformeComercial/Listar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

                ValidarEstadoSolicitud(usuario.ObtenerProveedorActual());

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

        public string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario)
        {
            try
            {
                string fileName = Path.GetFileName(fileSubido.FileName);

                string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

                ValidarEstadoSolicitud(usuario.ObtenerProveedorActual());

                string rutaCarpeta = string.Concat(rutaArchivosProveedores, "/", usuario.CUITRegistro, "/", usuario.Id, "/", fileKey);

                string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

                Directory.CreateDirectory(rutaCarpeta);

                switch (fileKey)
                {
                    case FileKeys.InformeComercialFirmado:
                        usuario.RutaInformeComercialFirmado = rutaArchivo;
                        break;
                    case FileKeys.ConstanciaCBU:
                        usuario.RutaConstanciaCBU = rutaArchivo;
                        break;
                    case FileKeys.ConstanciaCBUMercaderia:
                        usuario.RutaConstanciaCBUMercaderia = rutaArchivo;
                        break;
                    case FileKeys.ConstanciaCUIT:
                        usuario.RutaConstanciaCUIT = rutaArchivo;
                        break;
                    case FileKeys.InscripcionIIBB:
                        usuario.RutaInscripcionIIBB = rutaArchivo;
                        break;
                    case FileKeys.CertificadoExclusionIVA:
                        usuario.RutaCertificadoExclusionIVA = rutaArchivo;
                        break;
                    case FileKeys.CertificadoExclusionIIBB:
                        usuario.RutaCertificadoExclusionIIBB = rutaArchivo;
                        break;
                    case FileKeys.CertificadoExclusionSUSS:
                        usuario.RutaCertificadoExclusionSUSS = rutaArchivo;
                        break;
                    case FileKeys.CertificadoExclusionGanancias:
                        usuario.RutaCertificadoExclusionGanancias = rutaArchivo;
                        break;
                    case FileKeys.SIPER:
                        usuario.RutaSIPER = rutaArchivo;
                        break;
                    case FileKeys.DocumentacionEnBolsa:
                        usuario.RutaDocumentacionEnBolsa = rutaArchivo;
                        break;
                    default:
                        return ErrorMsg.ErrorFileKeyInvalido;
                }

                //Si subieron otros archivos anteriormente, los borramos
                DirectoryInfo carpeta = new DirectoryInfo(rutaCarpeta);

                foreach (FileInfo file in carpeta.GetFiles())
                {
                    file.Delete();
                }

                fileSubido.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();

                return SuccessMsg.ArchivoSubidoOK;
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

        public Dictionary<string, string> ObtenerArchivosSubidos(string mailUsuario)
        {
            var archivos = new Dictionary<string, string>();
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            archivos.Add(FileKeys.InformeComercialFirmado, ObtenerRuta(usuario.RutaInformeComercialFirmado));
            archivos.Add(FileKeys.ConstanciaCBU, ObtenerRuta(usuario.RutaConstanciaCBU));
            archivos.Add(FileKeys.ConstanciaCBUMercaderia, ObtenerRuta(usuario.RutaConstanciaCBUMercaderia));
            archivos.Add(FileKeys.ConstanciaCUIT, ObtenerRuta(usuario.RutaConstanciaCUIT));
            archivos.Add(FileKeys.InscripcionIIBB, ObtenerRuta(usuario.RutaInscripcionIIBB));
            archivos.Add(FileKeys.CertificadoExclusionIVA, ObtenerRuta(usuario.RutaCertificadoExclusionIVA));
            archivos.Add(FileKeys.CertificadoExclusionIIBB, ObtenerRuta(usuario.RutaCertificadoExclusionIIBB));
            archivos.Add(FileKeys.CertificadoExclusionSUSS, ObtenerRuta(usuario.RutaCertificadoExclusionSUSS));
            archivos.Add(FileKeys.CertificadoExclusionGanancias, ObtenerRuta(usuario.RutaCertificadoExclusionGanancias));
            archivos.Add(FileKeys.SIPER, ObtenerRuta(usuario.RutaSIPER));
            archivos.Add(FileKeys.DocumentacionEnBolsa, ObtenerRuta(usuario.RutaDocumentacionEnBolsa));

            return archivos;
        }

        public string ObtenerRuta(string ruta)
        {
            if (Path.GetFileName(ruta) != null)
                return Path.GetFileName(ruta);

            return "";
        }

        public string EnviarSolicitudUsuario(string mailUsuario)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            ValidarEstadoSolicitud(usuario.ObtenerProveedorActual());

            if (!ValidarArchivosSubidos(usuario))
            {
                return ErrorMsg.ErrorCompleteCampo;
            }

            var proveedor = usuario.ObtenerProveedorActual();

            proveedor.EstadoAprobacion = EstadoAprobacion.AprobacionPendiente;

            repositorio.GuardarCambios();

            return SuccessMsg.ValidacionPendienteOK;
        }

        private void ValidarEstadoSolicitud(Proveedor proveedor)
        {
            if (proveedor.EstadoAprobacion != EstadoAprobacion.DocumentacionPendiente && proveedor.EstadoAprobacion != EstadoAprobacion.EdicionRequerida)
            {
                throw new ValidationCustomException(ErrorMsg.EstadoIncorrectoSolicitud);
            }
        }

        private bool ValidarArchivosSubidos(UsuarioGranos usuario)
        {

            if (usuario.RutaInformeComercialFirmado.IsNullOrWhiteSpace())
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Informe comercial firmado"));
            }

            if (usuario.RutaConstanciaCBU.IsNullOrWhiteSpace())
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU"));
            }

            return true;
        }


        public string ObtenerArchivo(string mailUsuario, string fileKey)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);
            string rutaArchivo = "";
            switch (fileKey)
            {
                case FileKeys.InformeComercialFirmado:
                    rutaArchivo = usuario.RutaInformeComercialFirmado;
                    break;
                case FileKeys.ConstanciaCBU:
                    rutaArchivo = usuario.RutaConstanciaCBU;
                    break;
                case FileKeys.ConstanciaCBUMercaderia:
                    rutaArchivo = usuario.RutaConstanciaCBUMercaderia;
                    break;
                case FileKeys.ConstanciaCUIT:
                    rutaArchivo = usuario.RutaConstanciaCUIT;
                    break;
                case FileKeys.InscripcionIIBB:
                    rutaArchivo = usuario.RutaInscripcionIIBB;
                    break;
                case FileKeys.CertificadoExclusionGanancias:
                    rutaArchivo = usuario.RutaCertificadoExclusionGanancias;
                    break;
                case FileKeys.CertificadoExclusionIIBB:
                    rutaArchivo = usuario.RutaCertificadoExclusionIIBB;
                    break;
                case FileKeys.CertificadoExclusionIVA:
                    rutaArchivo = usuario.RutaCertificadoExclusionIVA;
                    break;
                case FileKeys.CertificadoExclusionSUSS:
                    rutaArchivo = usuario.RutaCertificadoExclusionSUSS;
                    break;
                case FileKeys.SIPER:
                    rutaArchivo = usuario.RutaSIPER;
                    break;
                case FileKeys.DocumentacionEnBolsa:
                    rutaArchivo = usuario.RutaDocumentacionEnBolsa;
                    break;
            }

            return rutaArchivo;
        }

        public string EliminarArchivo(string mailUsuario, string fileKey)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            ValidarEstadoSolicitud(usuario.ObtenerProveedorActual());

            string rutaArchivo = "";
            switch (fileKey)
            {
                case FileKeys.InformeComercialFirmado:
                    rutaArchivo = usuario.RutaInformeComercialFirmado;
                    usuario.RutaInformeComercialFirmado = null;
                    break;
                case FileKeys.ConstanciaCBU:
                    rutaArchivo = usuario.RutaConstanciaCBU;
                    usuario.RutaConstanciaCBU = null;
                    break;
                case FileKeys.ConstanciaCBUMercaderia:
                    rutaArchivo = usuario.RutaConstanciaCBUMercaderia;
                    usuario.RutaConstanciaCBUMercaderia = null;
                    break;
                case FileKeys.ConstanciaCUIT:
                    rutaArchivo = usuario.RutaConstanciaCUIT;
                    usuario.RutaConstanciaCUIT = null;
                    break;
                case FileKeys.InscripcionIIBB:
                    rutaArchivo = usuario.RutaInscripcionIIBB;
                    usuario.RutaInscripcionIIBB = null;
                    break;
                case FileKeys.CertificadoExclusionGanancias:
                    rutaArchivo = usuario.RutaCertificadoExclusionGanancias;
                    usuario.RutaCertificadoExclusionGanancias = null;
                    break;
                case FileKeys.CertificadoExclusionIIBB:
                    rutaArchivo = usuario.RutaCertificadoExclusionIIBB;
                    usuario.RutaCertificadoExclusionIIBB = null;
                    break;
                case FileKeys.CertificadoExclusionIVA:
                    rutaArchivo = usuario.RutaCertificadoExclusionIVA;
                    usuario.RutaCertificadoExclusionIVA = null;
                    break;
                case FileKeys.CertificadoExclusionSUSS:
                    rutaArchivo = usuario.RutaCertificadoExclusionSUSS;
                    usuario.RutaCertificadoExclusionSUSS = null;
                    break;
                case FileKeys.SIPER:
                    rutaArchivo = usuario.RutaSIPER;
                    usuario.RutaSIPER = null;
                    break;
                case FileKeys.DocumentacionEnBolsa:
                    rutaArchivo = usuario.RutaDocumentacionEnBolsa;
                    usuario.RutaDocumentacionEnBolsa = null;
                    break;
            }

            repositorio.GuardarCambios();
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            return "ok";
        }
    }
}
