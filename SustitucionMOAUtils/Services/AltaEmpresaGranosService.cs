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
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaGranosService : IAltaEmpresaGranosService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        private readonly string DataAgroURL;

        public AltaEmpresaGranosService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
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

                string rutaCarpeta = ArmarRutaCarpeta(fileKey, rutaArchivosProveedores, usuario);

                string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    return ErrorMsg.ErrorArchivoRepetido;
                }    

                Directory.CreateDirectory(rutaCarpeta);

                usuario.Archivos.Add(new Archivo { FileKey = fileKey, Ruta = rutaArchivo });

                //Si subieron otros archivos anteriormente, los borramos
                DirectoryInfo carpeta = new DirectoryInfo(rutaCarpeta);

                if (fileKey != FileKeys.CertificadoExclusionIIBB || fileKey != FileKeys.OtrosArchivos)
                {
                    foreach (FileInfo file in carpeta.GetFiles())
                    {
                        file.Delete();
                    }
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

        private string ArmarRutaCarpeta(string fileKey, string rutaArchivosProveedores, UsuarioGranos usuario)
        {
            return string.Concat(rutaArchivosProveedores, "/", usuario.CUITRegistro, "/", usuario.Id, "/", fileKey);
        }

        public List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario)

        {
            List<ArchivoDto> archivos = new List<ArchivoDto>();
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            foreach (var archivo in usuario.Archivos)
            {
                archivos.Add(new ArchivoDto(archivo));
            }

            return archivos;
        }

        public string ObtenerCBUSISA(string mailUsuario)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            var proveedor = usuario.ObtenerProveedorActual();

            string CBU = dataAgroService.ObtenerCBUProveedor(proveedor.CUIT);

            return CBU;
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

            if (usuario.Archivos.Any(f => f.FileKey == FileKeys.InformeComercialFirmado))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Informe comercial firmado"));
            }

            if (usuario.Archivos.Any(f => f.FileKey == FileKeys.ConstanciaCBU))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU"));
            }

            return true;
        }

        public string ObtenerArchivo(string mailUsuario, string fileKey, int fileID)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            return usuario.Archivos.Where(f => f.Id.Equals(fileID)).FirstOrDefault().Ruta; ;
        }

        public string EliminarArchivo(string mailUsuario, string fileKey, int archivoID)
        {
            var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

            ValidarEstadoSolicitud(usuario.ObtenerProveedorActual());
            string rutaArchivo = "";
        
            var archivoEliminar = usuario.Archivos.Where(f => f.Id.Equals(archivoID)).FirstOrDefault();
            rutaArchivo = archivoEliminar.Ruta;
            usuario.Archivos.Remove(archivoEliminar);

            repositorio.Remover(archivoEliminar);
       
            repositorio.GuardarCambios();
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            return SuccessMsg.ArchivoBorrado;
        }
    }
}
