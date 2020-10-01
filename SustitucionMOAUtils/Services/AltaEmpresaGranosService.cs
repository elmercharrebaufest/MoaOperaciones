using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;
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
        public byte[] GenerarInformeComercial(ParamInformeComercial informeComercial, string mailUsuario, int proveedorId)
        {
            try
            {
                var urlInformeComercial = string.Concat(DataAgroURL, "/InformeComercial/Listar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mailUsuario);

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                ValidarEstadoSolicitud(proveedor);

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                string downloadKey = "";

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

                    if (bool.Parse(jsonResult.HayErrores.ToString()))
                    {
                        throw new InfoCustomException(jsonResult.Errores[0].Message);
                    }

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

        public byte[] GenerarCartaDePresentacion(RptCartaDePresentacionInfo cartadePresentacion, string mailUsuario, int proveedorId)
        {
            try
            {
                var urlCartaPresentacion = string.Concat(DataAgroURL, "/CartaDePresentacion/Generar");
                var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                ValidarEstadoSolicitud(proveedor);

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                string downloadKey = "";

                var content = JsonConvert.SerializeObject(cartadePresentacion);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = client.PostAsync(urlCartaPresentacion, byteContent);

                    task.Wait();

                    var response = task.Result;

                    var stringContent = response.Content.ReadAsStringAsync();

                    dynamic jsonResult = JObject.Parse(stringContent.Result);

                    if (bool.Parse(jsonResult.HayErrores.ToString()))
                    {
                        throw new InfoCustomException(jsonResult.Errores[0].Message);
                    }

                    downloadKey = jsonResult.DownloadKey;


                    urlReporte = string.Concat(urlReporte, "?key=", downloadKey);

                    WebClient clienteDescarga = new WebClient
                    {
                        Credentials = new NetworkCredential(userName, password, dominio)
                    };

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

        public string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario, int proveedorId)
        {
            try
            {
                string fileName = Path.GetFileName(fileSubido.FileName);

                string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                ValidarEstadoSolicitud(proveedor);

                string rutaCarpeta = ArmarRutaCarpeta(fileKey, rutaArchivosProveedores, proveedor);

                string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    return ErrorMsg.ErrorArchivoRepetido;
                }

                Directory.CreateDirectory(rutaCarpeta);

                proveedor.Archivos.Add(new Archivo { FileKey = fileKey, Ruta = rutaArchivo });

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

        public string ArmarRutaCarpeta(string fileKey, string rutaArchivosProveedores, Proveedor proveedor)
        {
            return string.Concat(rutaArchivosProveedores, "/", proveedor.CUIT, "/", proveedor.Id, "/", fileKey);
        }

        public string EnviarSolicitudUsuario(string mailUsuario, int proveedorId, AltaEmpresaViewModel altaEmpresa)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);

            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            ValidarEstadoSolicitud(proveedor);

            if (usuario.TipoUsuario.Nombre == "Corredor")
            {
                if (!ValidarArchivosSubidosCorredor(proveedor, infoProveedor))
                {
                    return ErrorMsg.ErrorCompleteCampo;
                }
            }
            else
            {
                if (!ValidarArchivosSubidos(proveedor, infoProveedor))
                {
                    return ErrorMsg.ErrorCompleteCampo;
                }
            }

            proveedor.EstadoAprobacion = EstadoAprobacion.AprobacionPendiente;

            proveedor.VinculoConEmpleadosDeMolinos = altaEmpresa.VinculoConEmpleadosDeMolinos;
            proveedor.VinculoConFuncionariosPublicos = altaEmpresa.VinculoConFuncionariosPublicos;

            repositorio.RemoverTodos(proveedor.RelacionConEmpleados.ToList());
            repositorio.RemoverTodos(proveedor.RelacionConFuncionarios.ToList());
            foreach (var item in altaEmpresa.Empleados)
            {
                proveedor.RelacionConEmpleados.Add(
                    new ProveedorRelacionConEmpleados
                    {
                        CargoProveedora = item.CargoProveedora,
                        NombreMolinos = item.NombreMolinos,
                        NombreProveedora = item.NombreProveedora,
                        Proveedor_Id = usuario.Id,
                        Vinculo = item.Vinculo
                    });
            }
            foreach (var item in altaEmpresa.Funcionarios)
            {
                proveedor.RelacionConFuncionarios.Add(
                    new ProveedorRelacionConFuncionarios
                    {
                        CargoFirma = item.CargoFirma,
                        CargoFuncionario = item.CargoFuncionario,
                        NombreFirma = item.NombreFirma,
                        NombreFuncionario = item.NombreFuncionario,
                        Proveedor_Id = usuario.Id,
                        Vinculo = item.Vinculo
                    });
            }
            repositorio.GuardarCambios();

            return SuccessMsg.ValidacionPendienteOK;
        }

        public bool ValidarEstadoSolicitud(Proveedor proveedor)
        {
            if (proveedor.EstadoAprobacion != EstadoAprobacion.DocumentacionPendiente && proveedor.EstadoAprobacion != EstadoAprobacion.EdicionRequerida)
            {
                throw new ValidationCustomException(ErrorMsg.EstadoIncorrectoSolicitud);
            }

            return true;
        }

        public bool ValidarArchivosSubidos(Proveedor proveedor, InfoProveedorDataAgroDto infoProveedor)
        {
            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.InformeComercialFirmado))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Informe comercial firmado"));
            }

            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.ConstanciaCBU))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU"));
            }

            if (infoProveedor.EstadoSISA != "1")
            {
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.SIPER))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER"));
                }
            }

            return true;
        }

        public bool ValidarArchivosSubidosCorredor(Proveedor proveedor, InfoProveedorDataAgroDto infoProveedor)
        {
            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.CartaDePresentacionFirmada))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Carta de presentación firmada"));
            }

            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.ConstanciaCBU))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU"));
            }

            if (infoProveedor.EstadoSISA != "1")
            {
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.SIPER))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER"));
                }
            }

            return true;
        }

        public List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario, int proveedorId)
        {
            List<ArchivoDto> archivos = new List<ArchivoDto>();

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            foreach (var archivo in proveedor.Archivos)
            {
                archivos.Add(new ArchivoDto(archivo));
            }

            return archivos;
        }

        public string ObtenerArchivo(string mailUsuario, int archivoID, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            return proveedor.Archivos.Where(f => f.Id.Equals(archivoID)).FirstOrDefault().Ruta;
        }

        public string EliminarArchivo(string mailUsuario, int archivoID, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            ValidarEstadoSolicitud(proveedor);
            string rutaArchivo = "";


            var archivoEliminar = proveedor.Archivos.Where(f => f.Id.Equals(archivoID)).FirstOrDefault();
            rutaArchivo = archivoEliminar.Ruta;
            proveedor.Archivos.Remove(archivoEliminar);

            repositorio.Remover(archivoEliminar);

            repositorio.GuardarCambios();
            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            return SuccessMsg.ArchivoBorrado;
        }

        public InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                ResultadoValidarProveedorComercial result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);

                var info = new InfoProveedorDataAgroDto
                {
                    ProveedorCBU = result.ProveedorCBU,
                    ProveedorClasificacion = result.ProveedorClasificacion,
                    EstadoSISA = result.ProveedorSISACodCategoria
                };

                return info;
            }
            catch
            {
                return null;
            }
        }

        public AltaEmpresaViewModel CargarSolicitudUsuario(string mail, int proveedorId)
        {
            AltaEmpresaViewModel altaEmpresa = new AltaEmpresaViewModel();
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mail);

            var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            altaEmpresa.VinculoConEmpleadosDeMolinos = proveedor.VinculoConEmpleadosDeMolinos;
            altaEmpresa.VinculoConFuncionariosPublicos = proveedor.VinculoConFuncionariosPublicos;
            altaEmpresa.Empleados = proveedor.RelacionConEmpleados.Select(a => new AltaEmpresaEmpleadosViewModel { CargoProveedora = a.CargoProveedora, NombreMolinos = a.NombreMolinos, NombreProveedora = a.NombreProveedora, Vinculo = a.Vinculo }).ToList();
            altaEmpresa.Funcionarios = proveedor.RelacionConFuncionarios.Select(a => new AltaEmpresaFuncionariosViewModel { CargoFirma = a.CargoFirma, CargoFuncionario = a.CargoFuncionario, NombreFirma = a.NombreFirma, NombreFuncionario = a.NombreFuncionario, Vinculo = a.Vinculo }).ToList();

            return altaEmpresa;
        }
    }
}
