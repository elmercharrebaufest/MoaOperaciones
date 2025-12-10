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
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CredentialService;
//using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AltaEmpresaGranosService : IAltaEmpresaGranosService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        protected readonly IAltaEmpresaService AltaEmpresaService;
        private readonly string DataAgroURL;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EstadoAlta.html");
        private static readonly string EMAIL_TEMPLATE_AUDITORIA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoAuditoria.html");


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

                //ValidarEstadoSolicitud(proveedor);

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler
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

                    byte[] InformeComercialPDF;
                    urlReporte = string.Concat(urlReporte, "?key=", downloadKey);
                    using (WebClient clienteDescarga = new WebClient())
                    {
                        clienteDescarga.Credentials = new NetworkCredential(userName, password, dominio);

                        InformeComercialPDF = clienteDescarga.DownloadData(urlReporte);
                    }

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

                var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);
                var infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);

                cartadePresentacion.vendedorCuit = proveedor.CUIT;
                cartadePresentacion.vendedorRazonSocial = proveedor.RazonSocial;
                cartadePresentacion.vendedorActividad = infoProveedor.ProveedorClasificacion;

                //ValidarEstadoSolicitud(proveedor);

                string userName = DataAgroWSCredential.getUserName();
                string password = DataAgroWSCredential.getPassword();
                string dominio = DataAgroWSCredential.getDominio();

                var httpClientHandler = new HttpClientHandler
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

                    byte[] InformeComercialPDF;

                    using (WebClient clienteDescarga = new WebClient())
                    {
                        clienteDescarga.Credentials = new NetworkCredential(userName, password, dominio);
                        InformeComercialPDF = clienteDescarga.DownloadData(urlReporte);
                    }
                    ;

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

        public string GuardarArchivo(HttpPostedFileBase fileSubido, string fileKey, string mailUsuario, int proveedorId)
        {
            try
            {
                string fileName = Path.GetFileName(fileSubido.FileName);

                string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                if (fileKey != FileKeys.ArchivosInternos)
                {
                    ValidarEstadoSolicitud(proveedor);
                }

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

                    var archivoRemover = proveedor.Archivos.FirstOrDefault(a => a.FileKey == fileKey);

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

        public Localidad GetLocalidad(int localidadId)
        {
            if (localidadId <= 0)
            {
                throw new InfoCustomException("Id de localidad invalido");
            }

            var localidad = repositorio.Obtener<Localidad>(l => l.LocalidadId == localidadId);

            if (localidad == null) throw new InfoCustomException("No existe la localidad");

            //localidad.Nombre = localidad.Nombre + " (" + localidad.Provincia.Nombre + ")";

            return localidad;
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

            proveedor = proveedorId > 0 ? repositorio.Obtener<Proveedor>(proveedorId) : usuario.ObtenerProveedorPorId(proveedorId);

            if (proveedor.TipoProveedor.Nombre != "No Granos")
            {
                infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);
            }

            //Para contemplar validar los archivos, tenemos que verificar el usuario del proveedor. Esto es necesario hacerlo así para cuando se hacen altas internas
            var usuarioDeProveedor = repositorio.Obtener<Usuario>(u => u.Mail == proveedor.Mail);

            var tipoDeProveedor = "";

            //Si le están haciendo el alta interna, y todavía no se registró el usuario, lo tenemos en cuenta
            if (usuarioDeProveedor == null)
            {
                tipoDeProveedor = proveedor.TipoProveedor.Nombre;
            }
            else
            {
                tipoDeProveedor = usuarioDeProveedor.TipoUsuario.Nombre;
            }

            Log.Info("ValidarEstadoSolicitud");

            ValidarEstadoSolicitud(proveedor);

            Log.Info("If para validar archivos. Tipo: " + proveedor.TipoProveedor.Nombre);

            if (tipoDeProveedor == "Corredor")
            {
                if (!ValidarArchivosSubidosCorredor(proveedor, infoProveedor))
                {
                    return ErrorMsg.ErrorCompleteCampo;
                }
            }
            else if (tipoDeProveedor == "No Granos")
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

                bool pasoPorEdicionRequerida = proveedor.HistorialAprobaciones.Any(h => h.EstadoAprobacion == EstadoAprobacion.EdicionRequerida);
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
                    proveedor.EstadoAprobacion = (proveedor.RequiereVerificacionCompras ?? false) ? EstadoAprobacion.PendienteAprobacionCompras : EstadoAprobacion.AprobacionPendiente;
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
                var estadoAprobacion = EstadoAprobacion.DocumentacionPendiente;

                if (proveedor.CorrespondeAltaSolicitada())
                {
                    estadoAprobacion = EstadoAprobacion.AprobacionPendiente;
                }
                if (proveedor.CorrespondeEstadoPrevio())
                {
                    var estadoPrevioAActual = proveedor.EstadoPrevioAActual();
                    estadoAprobacion = estadoPrevioAActual != null ? (EstadoAprobacion)estadoPrevioAActual : estadoAprobacion;
                }

                proveedor.EstadoAprobacion = estadoAprobacion;

                proveedor.HistorialAprobaciones.Add(
                    new ProveedorHistorialAprobacion
                    {
                        Fecha = DateTime.Now,
                        EstadoAprobacion = estadoAprobacion,
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

            Log.Info("Validamos DDJJ y enviamos mail.");

            if (ValidarDDJJ(altaEmpresa))
            {
                EnviarMailAuditoria(altaEmpresa, proveedor);
            }

            Log.Info("Fin");

            return SuccessMsg.ValidacionPendienteOK;
        }

        private void EnviarMailAuditoria(AltaEmpresaViewModel altaEmpresa, Proveedor proveedor)
        {
            try
            {

                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_AUDITORIA);
                string asunto = "MOA Operaciones - Conflicto de Interés Declarado en Alta de Proveedor";
                var vinculoEmpleadoMolinos = new StringBuilder();
                var Vinculofuncionarios = new StringBuilder();

                foreach (var empleado in altaEmpresa.Empleados)
                {
                    vinculoEmpleadoMolinos.AppendLine($"<tr><td>{empleado.NombreProveedora}</td><td>{empleado.CargoProveedora}</td><td>{empleado.NombreMolinos}</td><td>{empleado.Vinculo}</td></tr>");
                }

                foreach (var funcionario in altaEmpresa.Funcionarios)
                {
                    Vinculofuncionarios.AppendLine($"<tr><td>{funcionario.NombreFirma}</td><td>{funcionario.CargoFirma}</td><td>{funcionario.NombreFuncionario}</td><td>{funcionario.CargoFuncionario}</td><td>{funcionario.Vinculo}</td></tr>");
                }

                var cuit = ReformatearCUIT(proveedor.CUIT);

                var cuerpo = string.Format(cuerpoTemplate, proveedor.FechaSolicitud, proveedor.TipoProveedor.Nombre, proveedor.RazonSocial, cuit, vinculoEmpleadoMolinos, Vinculofuncionarios);
                var Destinatario = ConfigurationManager.AppSettings["EmailToAuditoria"];

                EmailSender.EnviarMail(new List<string> { Destinatario }, asunto, cuerpo, null, null, null, null);
            }
            catch (Exception)
            {
            }

        }

        private void EnviarMailEdicionRequerida(Proveedor proveedor, string observacionParaElProveedor, List<string> copia)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, proveedor.RazonSocial, "observada", !string.IsNullOrWhiteSpace(observacionParaElProveedor) ? observacionParaElProveedor : "-");
                string asunto = "Molinos Agro - Edición Requerida";

                if (proveedor.AltaInterna ?? false)
                {
                    if (proveedor.TipoProveedor.NombreCorto == "G")
                    {
                        var usuario = repositorio.Obtener<Usuario>(U => U.Id == proveedor.IdSolicitanteInternoAltaGranos);
                        copia.Add(usuario.Mail);
                    }
                }

                EmailSender.EnviarMail(new List<string> { proveedor.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private bool ValidarDDJJ(AltaEmpresaViewModel altaEmpresa)
        {
            var vinculoEmpleados = altaEmpresa.VinculoConEmpleadosDeMolinos.HasValue ? altaEmpresa.VinculoConEmpleadosDeMolinos ?? true : false;
            var vinculoFuncionarios = altaEmpresa.VinculoConFuncionariosPublicos.HasValue ? altaEmpresa.VinculoConFuncionariosPublicos ?? true : false;

            if (vinculoEmpleados || vinculoFuncionarios) return true;

            return false;
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

            if (infoProveedor.EstadoSISA != "1" && !proveedor.Archivos.Any(f => f.FileKey == FileKeys.SIPER))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER"));
            }

            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.DDJJ) && (proveedor.AltaInterna.HasValue && proveedor.AltaInterna == true) && proveedor.TipoProveedor.Nombre == "Granos")
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "DDJJ"));
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

            if (infoProveedor.EstadoSISA != "1" && !proveedor.Archivos.Any(f => f.FileKey == FileKeys.SIPER))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER"));
            }

            return true;
        }
        public bool ValidarArchivosSubidosNoGranos(Proveedor proveedor, AltaEmpresaViewModel altaEmpresa)
        {
            if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.ConstanciaCUIT))
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CUIT"));
            }
            if ((altaEmpresa.IdIngresoBruto == (int)IngresosBrutos.Local || altaEmpresa.IdIngresoBruto == (int)IngresosBrutos.ConvenioMultilateral)
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
            if (proveedor.AltaInterna ?? false)
            {
                if (!proveedor.Archivos.Any(f => f.FileKey == FileKeys.DeclaracionVinculosAltaInterna))
                {
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorArchivoRequerido, "Declaración vinculos"));
                }
            }

            return true;
        }

        public string GrabarProveedorAltaInternaGranos(string cuit, string mailUsuario, string mailVendedor)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);
            if (usuario == null) throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Usuario", mailUsuario));

            if (repositorio.Existe<Proveedor>(p => p.CUIT == cuit && p.Mail == mailVendedor))
                throw new InfoCustomException("El mail ya tiene registrado esta CUIT");

            if (cuit == null || cuit == "") throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT"));
            if (usuario.Proveedores.Where(x => x.CUIT == cuit).Any()) throw new ValidationCustomException(ErrorMsg.ErrorVendedorRepetido);

            if (mailVendedor == null || mailVendedor == "") throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Mail"));
            if (!IsValidEmail(mailVendedor)) throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorIncorrecto, "Mail"));
            if (mailVendedor.ToLower().Contains("@molinosagro.com.ar")) throw new ValidationCustomException(string.Format(ErrorMsg.MailMolinosAgro));

            var infoDA = dataAgroService.ObtenerValidarCUITProveedorGranos(cuit, false);
            if (infoDA.HayError) throw new ValidationCustomException(infoDA.ListaErrores[0].Message);

            if (infoDA.ProveedorMails.Contains(mailVendedor, StringComparer.OrdinalIgnoreCase) || bool.Parse(ConfigurationManager.AppSettings["EsLocal"]))
            {
                var proveedorComercial = usuario.ObtenerProveedor();

                var proveedor = new Proveedor
                {
                    CUIT = cuit.Trim(),
                    Mail = mailVendedor.Trim(),
                    EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                    CodigoProveedor = FormatearCodigoProveedor(cuit),
                    TipoProveedor = ObtenerTipoPorNombreCorto("G"),
                    FechaSolicitud = DateTime.Now,
                    Comercial = string.Concat(infoDA.ComercialNombres, " ", infoDA.ComercialApellido),
                    AltaInterna = true,
                    IdSolicitanteInternoAltaGranos = usuario.Id
                };
                var result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                if (result != null)
                {
                    proveedor.EstadoSISA = result.ProveedorSISAEstadoCuit;
                }
                var hist = new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    EstadoAprobacion = proveedor.EstadoAprobacion,
                    Observacion = "Alta interna - Proveedor habilitado en DataAgro"
                };

                proveedor.RazonSocial = infoDA.ProveedorRazonSocial;
                proveedor.IdComercialDataAgro = infoDA.ComercialId;
                proveedor.IdDataAgro = infoDA.ProveedorId;
                proveedor.IdSolicitanteInternoAltaGranos = usuario.Id;
                proveedor.HistorialAprobaciones.Add(hist);

                repositorio.Agregar(proveedor);

                //Le agrego el proveedor al comercial
                usuario.Proveedores.Add(proveedor);
                repositorio.GuardarCambios();

                return SuccessMsg.AltaVendedorOK;
            }
            else
            {
                throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorIncorrecto, "Mail"));
            }
        }

        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto) => repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);

        private string FormatearCodigoProveedor(string CUIT)
        {
            return string.Concat("00", CUIT.Substring(2, 8));
        }



        public List<ArchivoDto> ObtenerArchivosSubidos(string mailUsuario, int proveedorId, bool esOperador)
        {
            List<ArchivoDto> archivos = new List<ArchivoDto>();

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

            var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);

            return proveedor.Archivos.FirstOrDefault(f => f.Id.Equals(archivoID)).Ruta;
        }

        public string EliminarArchivo(string mailUsuario, int archivoID, int proveedorId)
        {
            var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);

            string rutaArchivo = "";

            var archivoEliminar = proveedor.Archivos.FirstOrDefault(f => f.Id.Equals(archivoID));

            if (archivoEliminar.FileKey != FileKeys.ArchivosInternos)
            {
                ValidarEstadoSolicitud(proveedor);
            }

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

            SustitucionMOAModel.Entities.Proveedor proveedor;

            proveedor = proveedorId > 0 ? repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId) : usuario.ObtenerProveedor();

            var CUITProveedor = ReformatearCUIT(proveedor.CUIT);

            SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial result = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);

            var info = new InfoProveedorDataAgroDto
            {
                ProveedorCBU = result.ProveedorCBU,
                ProveedorClasificacion = result.ProveedorClasificacion,
                EstadoSISA = result.ProveedorSISAEstadoCuit,
                ProveedorCUIT = CUITProveedor,
                RazonSocial = proveedor.RazonSocial,
                AltaInterna = proveedor.AltaInterna ?? false,
                Observacion = proveedor.Observaciones,
            };

            return info;
        }

        public AltaEmpresaViewModel CargarSolicitudUsuario(string mailUsuario, int proveedorId)
        {
            AltaEmpresaViewModel altaEmpresa = new AltaEmpresaViewModel();

            var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);

            altaEmpresa.VinculoConEmpleadosDeMolinos = proveedor.VinculoConEmpleadosDeMolinos;
            altaEmpresa.VinculoConFuncionariosPublicos = proveedor.VinculoConFuncionariosPublicos;

            altaEmpresa.Empleados = proveedor.RelacionConEmpleados
                                                .Select(a => new AltaEmpresaEmpleadosViewModel
                                                {
                                                    CargoProveedora = a.CargoProveedora,
                                                    NombreMolinos = a.NombreMolinos,
                                                    NombreProveedora =
                                                    a.NombreProveedora,
                                                    Vinculo = a.Vinculo
                                                })
                                                .ToList();

            altaEmpresa.Funcionarios = proveedor.RelacionConFuncionarios
                                                .Select(a => new AltaEmpresaFuncionariosViewModel
                                                {
                                                    CargoFirma = a.CargoFirma,
                                                    CargoFuncionario = a.CargoFuncionario,
                                                    NombreFirma = a.NombreFirma,
                                                    NombreFuncionario = a.NombreFuncionario,
                                                    Vinculo = a.Vinculo
                                                })
                                                .ToList();

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

                var httpClientHandler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(userName, password, dominio),
                };

                using (var client = new HttpClient(httpClientHandler, false))
                {
                    var task = await client.PostAsync(urlBusquedaMateriales, null).ConfigureAwait(false);

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

        public SustitucionMOAModel.Entities.Proveedor ObtenerRazonSocialProveedor(int proveedorId)
        {
            var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);

            return proveedor;
        }
        public string ObtenerCUITUsuario(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            return usuario.CUITRegistro;
        }

        public string ObtenerArchivos(string mail, int proveedorId, string pathBase)
        {
            var proveedor = repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId);
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

        public string SolicitudAltaInterna(string mailUsuario, int proveedorId, AltaEmpresaViewModel altaEmpresa)
        {
            InfoProveedorDataAgroDto infoProveedor = null;
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var proveedor = proveedorId > 0 ? repositorio.Obtener<SustitucionMOAModel.Entities.Proveedor>(proveedorId) : usuario.ObtenerProveedorPorId(proveedorId);

            if (proveedor.TipoProveedor.Nombre != "Granos")
            {
                return ErrorMsg.ErrorSinPermiso;
            }

            ValidarEstadoSolicitud(proveedor);
            infoProveedor = ObtenerInfoProveedor(mailUsuario, proveedorId);

            if (!ValidarArchivosSubidos(proveedor, infoProveedor))
            {
                return ErrorMsg.ErrorCompleteCampo;
            }
            ;

            if (proveedor.HistorialAprobaciones == null)
            {
                proveedor.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();
            }

            proveedor.EstadoAprobacion = EstadoAprobacion.AprobacionPendiente;
            proveedor.VinculoConEmpleadosDeMolinos = (altaEmpresa.VinculoConEmpleadosDeMolinos.HasValue
                && altaEmpresa.VinculoConEmpleadosDeMolinos == true) ? true : false;
            proveedor.VinculoConFuncionariosPublicos = (altaEmpresa.VinculoConFuncionariosPublicos.HasValue
                && altaEmpresa.VinculoConFuncionariosPublicos == true) ? true : false;

            proveedor.HistorialAprobaciones.Add(
                new ProveedorHistorialAprobacion
                {
                    Fecha = DateTime.Now,
                    EstadoAprobacion = EstadoAprobacion.AprobacionPendiente,
                    Observacion = "Envía solicitud",
                    Proveedor_Id = proveedorId,
                    Usuario_Id = usuario.Id,
                }
            );

            repositorio.GuardarCambios();

            if (ValidarDDJJ(altaEmpresa))
            {
                EnviarMailAuditoria(altaEmpresa, proveedor);
            }

            return SuccessMsg.ValidacionPendienteOK;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    }
}
