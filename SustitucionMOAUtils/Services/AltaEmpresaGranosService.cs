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
using System.Threading.Tasks;
using System.IO.Compression;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Email;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaGranosService : IAltaEmpresaGranosService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        private readonly string DataAgroURL;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EstadoAlta.html");


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

                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
                var infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);

                cartadePresentacion.vendedorCuit = proveedor.CUIT;
                cartadePresentacion.vendedorRazonSocial = proveedor.RazonSocial;
                cartadePresentacion.vendedorActividad = infoProveedor.ProveedorClasificacion;

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

        public async Task<string> ObtenerMaterialesDataAgro()
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
                    var task = await client.PostAsync(urlBusquedaMateriales, null);

                    //task.Wait();

                    var stringContent = task.Content.ReadAsStringAsync();

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

                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                if (fileKey != FileKeys.ArchivosInternos)
                    ValidarEstadoSolicitud(proveedor);

                string rutaCarpeta = ArmarRutaCarpeta(fileKey, rutaArchivosProveedores, proveedor);

                string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    return ErrorMsg.ErrorArchivoRepetido;
                }

                Directory.CreateDirectory(rutaCarpeta);

                //Si subieron otros archivos anteriormente, los borramos
                DirectoryInfo carpeta = new DirectoryInfo(rutaCarpeta);

                if (fileKey != FileKeys.CertificadoExclusionIIBB && fileKey != FileKeys.OtrosArchivos && fileKey != FileKeys.ArchivosInternos)
                {
                    foreach (FileInfo file in carpeta.GetFiles())
                    {
                        file.Delete();
                    }

                    var archivoRemover = proveedor.Archivos.Where(a => a.FileKey == fileKey).FirstOrDefault();

                    if (archivoRemover != null)
                    {
                        repositorio.Remover(archivoRemover);
                    }
                }

                proveedor.Archivos.Add(new Archivo { FileKey = fileKey, Ruta = rutaArchivo });

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

        public string EnviarSolicitudUsuario(string mailUsuario, int proveedorId, bool esGuardarYNotificar, AltaEmpresaViewModel altaEmpresa)
        {

            Log.Info("Inicio enviar solicitud");

            Log.Info("Obtener Usuario");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            InfoProveedorDataAgroDto infoProveedor = null;

            Proveedor proveedor;

            Log.Info("Obtener Proveedor. Proveedor ID: " + proveedorId.ToString());

            if (proveedorId > 0)
                proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            else
                proveedor = usuario.ObtenerProveedorPorId(proveedorId);

            if (proveedor.TipoProveedor.Nombre != "No Granos")
                infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);


            Log.Info("ValidarEstadoSolicitud");

            ValidarEstadoSolicitud(proveedor);


            Log.Info("If para validar archivos. Tipo: " + proveedor.TipoProveedor.Nombre);

            if (proveedor.TipoProveedor.Nombre == "Corredor")
            {
                if (!ValidarArchivosSubidosCorredor(proveedor, infoProveedor))
                {
                    return ErrorMsg.ErrorCompleteCampo;
                }
            }
            else if (proveedor.TipoProveedor.Nombre == "No Granos")
            {
                if (!ValidarArchivosSubidosNoGranos(proveedor, altaEmpresa))
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


            if (!esGuardarYNotificar)
            {
                Log.Info("If para chequear historial anterior");

                var historialAnterior = proveedor.HistorialAprobaciones.Where(h => h.EstadoAprobacion != EstadoAprobacion.EdicionRequerida).OrderByDescending(x => x.Fecha).FirstOrDefault();

                bool pasoPorEdicionRequerida = proveedor.HistorialAprobaciones.Where(h => h.EstadoAprobacion == EstadoAprobacion.EdicionRequerida).Any();
                //Si existe un historial le ponemos el anterior antes de ser observado. Si no, lo ponemos en el estado inicial del flujo de alta
                //Ademas, nos fijamos que lo hallan mandado a observar
                if (historialAnterior != null && pasoPorEdicionRequerida)
                {
                    if (historialAnterior.EstadoAprobacion == EstadoAprobacion.AnularAprobacion)
                        historialAnterior.EstadoAprobacion = EstadoAprobacion.EtapaFinal;

                    proveedor.EstadoAprobacion = historialAnterior.EstadoAprobacion;
                }
                else
                {
                    Log.Info("If para chequear verificacion compras");
                    if (proveedor.RequiereVerificacionCompras ?? false)
                    {
                        proveedor.EstadoAprobacion = EstadoAprobacion.PendienteAprobacionCompras;
                    }
                    else
                    {
                        proveedor.EstadoAprobacion = EstadoAprobacion.AprobacionPendiente;
                    }
                }


                proveedor.VinculoConEmpleadosDeMolinos = altaEmpresa.VinculoConEmpleadosDeMolinos;
                proveedor.VinculoConFuncionariosPublicos = altaEmpresa.VinculoConFuncionariosPublicos;
            }
            proveedor.CBU = altaEmpresa.CBU;
            proveedor.IdIngresoBruto = altaEmpresa.IdIngresoBruto;
            proveedor.IdSituacionIVA = altaEmpresa.IdSituacionIVA;


            if (!esGuardarYNotificar)
            {
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

                Log.Info("Actualizamos historial");

                if (proveedor.HistorialAprobaciones == null)
                {
                    proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }

                proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = proveedor.EstadoAprobacion,
                        Observacion = "Envía solicitud",
                        Proveedor_Id = proveedorId,
                        Usuario_Id = usuario.Id,
                        ObservacionParaProveedor = altaEmpresa.Comentarios
                    }
                );
            }
            else
            {

                if (proveedor.HistorialAprobaciones == null)
                {
                    proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
                }

                proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                        Observacion = "Guardado y notificado al proveedor",
                        Proveedor_Id = proveedorId,
                        Usuario_Id = usuario.Id
                    }
                );

                string mensaje = "Alta en proceso. Ya puede ingresar a aceptar el código de conducta.";

                EnviarMailEdicionRequerida(proveedor, mensaje, null);
            }

            proveedor.FechaSolicitud = DateTime.Now;

            Log.Info("Guardamos");

            repositorio.GuardarCambios();

            Log.Info("Fin");

            return SuccessMsg.ValidacionPendienteOK;
        }

        private void EnviarMailEdicionRequerida(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "observada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
                string asunto = "Molinos Agro - Edición Requerida";

                EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
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
        public bool ValidarArchivosSubidosNoGranos(Proveedor proveedor, AltaEmpresaViewModel altaEmpresa)
        {
            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.ConstanciaCUIT))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CUIT"));
            }
            if ((altaEmpresa.IdIngresoBruto == (int)IngresosBrutos.Local || altaEmpresa.IdIngresoBruto ==  (int)IngresosBrutos.ConvenioMultilateral) 
                && !proveedor.Archivos.Any(f => f.FileKey == FileKeys.InscripcionIIBB))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Inscripcion IIBB"));
            }
            if (altaEmpresa.IdIngresoBruto == (int)IngresosBrutos.ConvenioMultilateral && !proveedor.Archivos.Any(f => f.FileKey == FileKeys.FormularioCM05))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Convenio (CM05 vigente)"));
            }

            if (proveedor.IngresoAPlanta ?? false)
            {
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.NotaSiniestralidadART))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Nota Siniestralidad ART"));
                }
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.ProtocoloSanitarioCovid))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Protocolo Sanitario Covid"));
                }
            }

            if (proveedor.SiperObligatorio ?? false)
            {
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.SIPER))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER"));
                }
            }

            return true;
        }

        public List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario, int proveedorId, bool esOperador)
        {
            List<ArchivoDto> archivos = new List<ArchivoDto>();

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);


            foreach (var archivo in proveedor.Archivos.Where(a => a.FileKey != (!esOperador ? FileKeys.ArchivosInternos : "")))
            {
                archivos.Add(new ArchivoDto(archivo));
            }

            return archivos;
        }

        public string ObtenerArchivo(string mailUsuario, int archivoID, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            return proveedor.Archivos.Where(f => f.Id.Equals(archivoID)).FirstOrDefault().Ruta;
        }

        public string EliminarArchivo(string mailUsuario, int archivoID, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            string rutaArchivo = "";

            var archivoEliminar = proveedor.Archivos.Where(f => f.Id.Equals(archivoID)).FirstOrDefault();

            if (archivoEliminar.FileKey != FileKeys.ArchivosInternos)
                ValidarEstadoSolicitud(proveedor);

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

        public string ReformatearCUIT(string CUIT)
        {
            var CUITFormateado = (CUIT.Insert(2, "-")).Insert(11, "-");

            return CUITFormateado;
        }

        public InfoProveedorDataAgroDto ObtenerInfoProveedor(string mailUsuario, int proveedorId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Proveedor proveedor;

            if (proveedorId > 0)
                proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            else
                proveedor = usuario.ObtenerProveedor();

            var CUITProveedor = ReformatearCUIT(proveedor.CUIT);


            ResultadoValidarProveedorComercial result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
            //result = new ResultadoValidarProveedorComercial {

            //};
            var info = new InfoProveedorDataAgroDto
            {
                ProveedorCBU = result.ProveedorCBU,
                ProveedorClasificacion = result.ProveedorClasificacion,
                EstadoSISA = result.ProveedorSISAEstadoCuit,
                ProveedorCUIT = CUITProveedor,
                RazonSocial = proveedor.RazonSocial
            };

            return info;
        }

        public AltaEmpresaViewModel CargarSolicitudUsuario(string mail, int proveedorId)
        {
            AltaEmpresaViewModel altaEmpresa = new AltaEmpresaViewModel();
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mail);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            altaEmpresa.VinculoConEmpleadosDeMolinos = proveedor.VinculoConEmpleadosDeMolinos;
            altaEmpresa.VinculoConFuncionariosPublicos = proveedor.VinculoConFuncionariosPublicos;
            altaEmpresa.Empleados = proveedor.RelacionConEmpleados.Select(a => new AltaEmpresaEmpleadosViewModel { CargoProveedora = a.CargoProveedora, NombreMolinos = a.NombreMolinos, NombreProveedora = a.NombreProveedora, Vinculo = a.Vinculo }).ToList();
            altaEmpresa.Funcionarios = proveedor.RelacionConFuncionarios.Select(a => new AltaEmpresaFuncionariosViewModel { CargoFirma = a.CargoFirma, CargoFuncionario = a.CargoFuncionario, NombreFirma = a.NombreFirma, NombreFuncionario = a.NombreFuncionario, Vinculo = a.Vinculo }).ToList();
            altaEmpresa.IdIngresoBruto = proveedor.IdIngresoBruto;
            altaEmpresa.IdSituacionIVA = proveedor.IdSituacionIVA;
            altaEmpresa.CBU = proveedor.CBU;

            return altaEmpresa;
        }

        public async Task<string> ObtenerCampañasDataAgroAsync()
        {
            try
            {
                var urlBusquedaMateriales = string.Concat(DataAgroURL, "/Campana/Buscar");

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                //userName = "emartin";
                //password = "eugeniomartin2";
                //dominio = "baunet";
                //urlBusquedaMateriales = "http://localhost:52498/Campana/Buscar";

                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = await client.PostAsync(urlBusquedaMateriales, null);

                    //task.Wait();

                    var stringContent = task.Content.ReadAsStringAsync();

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

        public Proveedor ObtenerRazonSocialProveedor(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            return proveedor;
        }
        public string ObtenerCUITUsuario(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            return usuario.CUITRegistro;
        }

        public string ObtenerArchivos(string mail, int proveedorId, string pathBase)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            var zipFilename = $"Proveedor-{proveedor.CUIT}-Documentacion.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (var archivoSubido in proveedor.Archivos)
                    {
                        //Hay casos en prod de archivos que no están fisicamente pero si en la tabla. Mejor chequeemos que exista y si no seguimos con otro
                        if (File.Exists(archivoSubido.Ruta))
                        {
                            string fileName = Path.GetFileName(archivoSubido.Ruta);
                            archivo.CreateEntryFromFile(archivoSubido.Ruta, fileName);
                        }

                    }
                }
            }

            return filePath;
        }
    }
}
