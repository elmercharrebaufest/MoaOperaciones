using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CredentialService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class CampoSustentableService : ICampoSustentableService
    {
        private readonly IRepositorio repositorio;
        private readonly string DataAgroURL;
        private readonly IExcelExportWrapper excelExport;
        private readonly IDataAgroService dataAgroService;

        public CampoSustentableService(IRepositorio repositorio, IExcelExportWrapper excelExport, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
            this.excelExport = excelExport;
            this.dataAgroService = dataAgroService;
        }

        public Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz, bool UsarArchivoId)
        {
            string ruta = "";
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);
            ValidarCampo(usuario, campoProveedor, archivoKmz);
            var declaracion = repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == campoProveedor.CampoCosecha.Cosecha_Id && d.CUIT == campoProveedor.CUIT);
            campoProveedor.RazonSocial = declaracion.RazonSocial;
            campoProveedor.FechaCreacion = DateTime.Now;
            campoProveedor.Borrado = false;
            if (campoProveedor.Archivo_Id == 0)
            {
                campoProveedor.Archivo = (new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" });
            }
            else
            {
                var archivoCampo = repositorio.Obtener<Archivo>(a => a.Id == campoProveedor.Archivo_Id);
                ruta = archivoCampo.Ruta;
            }
            campoProveedor.CampoCosecha.ToneladasAprobadas = -1;

            campoProveedor.CampoCosecha.Campo.IdScato = ObtenerIdScato(campoProveedor);

            repositorio.Agregar(campoProveedor);

            repositorio.GuardarCambios();

            if (UsarArchivoId == false)
            {
                GuardarArchivoKMZ(campoProveedor, archivoKmz);
                repositorio.GuardarCambios();

            }
            var archivo = archivoKmz == null ? Convert.ToBase64String(System.IO.File.ReadAllBytes(ruta)) : ConvertirArchivo64(archivoKmz);
            InformarCampoSustentable(campoProveedor, archivo);
            return new Resultado { IdEntidad = campoProveedor.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableAgregado };
        }

        private void ValidarUsuario(Usuario usuario, int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            var esComercial = usuario.TienePermiso(PermisoEnum.ComercialCamposSustentables);
            var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodosCamposSustentable);
            if (!(esAdmin || esComercial))
            {
                if (!usuario.Proveedores.Any(p => p.CUIT == proveedor.CUIT))
                {
                    throw new ValidationCustomException("Su usuario no tiene habilitado el proveedor con el que intenta operar.");
                }
            }
        }

        public Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var editarAdmin = usuario.TienePermiso(PermisoEnum.EdicionCamposCreados);
            var editarComercial = usuario.TienePermiso(PermisoEnum.ComercialCamposSustentables);
            if (!(editarAdmin || editarComercial))
                throw new ValidationCustomException(ErrorMsg.ErrorSinPermiso);

            ValidarUsuario(usuario, campoProveedorObj.Proveedor_Id);

            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == campoProveedorObj.Proveedor_Id && cp.CampoCosecha_Id == campoProveedorObj.CampoCosecha_Id);

            if (campoProveedor == null)
            {
                throw new ValidationCustomException("No se encontró el campo sustentable para editar.");
            }

            campoProveedor.FechaModificacion = DateTime.Now;
            /*
            ValidarCampo(usuario, campoProveedor, archivoKmz);
            */

            campoProveedor.HectareasSoja = campoProveedorObj.HectareasSoja;
            campoProveedor.HectareasTotales = campoProveedorObj.HectareasTotales;
            campoProveedor.Longitud = campoProveedorObj.Longitud;
            campoProveedor.Latitud = campoProveedorObj.Latitud;
            campoProveedor.CampoCosecha.ToneladasAprobadas = campoProveedorObj.CampoCosecha.ToneladasAprobadas;
            campoProveedor.CampoCosecha.Campo.Nombre = campoProveedorObj.CampoCosecha.Campo.Nombre;
            campoProveedor.CampoCosecha.Campo.Localidad_Id = campoProveedorObj.CampoCosecha.Campo.Localidad_Id;

            repositorio.GuardarCambios();

            //GuardarArchivoKMZ(campoProveedor, archivoKmz);

            //repositorio.GuardarCambios();

            InformarCampoSustentable(campoProveedor, "");

            return new Resultado { IdEntidad = campoProveedorObj.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableActualizado };
        }

        public string Borrar(string mailUsuario, int campoCosechaId, int proveedorId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            if (campoProveedor.CampoCosecha.ToneladasAprobadas > 0)
            {
                throw new ValidationCustomException("No se puede eliminar el campo debido a que ya tiene toneladas aprobadas");
            }

            campoProveedor.Borrado = true;

            repositorio.GuardarCambios();

            return SuccessMsg.CampoSustentableBorrado;
        }

        public byte[] ImprimirDeclaracion(int proveedorId, int cosechaId, string CUIT)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            var cosecha = repositorio.Obtener<Cosecha>(cosechaId);

            var allCampos = repositorio.Listar<CampoProveedor, CamposSustentableReporte>
                (c => new CamposSustentableReporte
                {
                    HectareasSoja = c.HectareasSoja.ToString(),
                    HectareasTotales = c.HectareasTotales.ToString(),
                    Localidad = c.CampoCosecha.Campo.Localidad.Nombre,
                    Nombre = c.CampoCosecha.Campo.Nombre,
                    Pais = "Argentina",
                    Provincia = c.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    Coordenadas = string.Concat(c.Latitud, " ", c.Longitud),
                    Partido = c.CampoCosecha.Campo.Localidad.Partido.Descripcion
                },
                cp => cp.CUIT == CUIT && cp.CampoCosecha.Cosecha_Id == cosecha.Id);

            var declaracion = repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == cosechaId && d.CUIT == CUIT);

            DeclaracionCampoSustentableDto datos = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = declaracion.CUIT,
                RazonSocial = declaracion.RazonSocial,
                Fecha = declaracion.FechaFirma?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = declaracion.HectareasDeclaradas.Value,
                Campos = allCampos
            };

            var pdfCampos = GenerarPDFDeclaracion(datos);

            byte[] archivoResult;

            Document document = new Document();

            using (MemoryStream stream = new MemoryStream())
            {
                PdfCopy pdf = new PdfCopy(document, stream);
                document.Open();

                PdfReader pdfReaderCampos = new PdfReader(pdfCampos);

                pdfReaderCampos.SelectPages(string.Concat("2-", pdfReaderCampos.NumberOfPages));

                //var fileKey = string.Concat(FileKeys.DeclaracionCampoSustentable, "-", cosechaId);

                //var archivoDeclaracion = proveedor.Archivos.FirstOrDefault(a => a.FileKey == fileKey);
                byte[] fileBytes = File.ReadAllBytes(declaracion.Archivo.Ruta);

                PdfReader pdfReaderDeclaracion = new PdfReader(fileBytes);

                pdf.AddDocument(pdfReaderDeclaracion);
                pdfReaderDeclaracion.Close();

                pdf.AddDocument(pdfReaderCampos);
                pdfReaderCampos.Close();

                document.Close();

                archivoResult = stream.ToArray();
            }

            return archivoResult;
        }

        private byte[] GenerarPDFDeclaracion(DeclaracionCampoSustentableDto datos)
        {
            var urlReporteCampo = string.Concat(DataAgroURL, "/CamposSustentables/Generar");
            var urlReporte = string.Concat(DataAgroURL, "/Download/Reporte");

            string userName = DataAgroWSCredential.getUserName();
            string password = DataAgroWSCredential.getPassword();
            string dominio = DataAgroWSCredential.getDominio();

            var httpClientHandler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(userName, password, dominio),
            };
            var content = JsonConvert.SerializeObject(datos);

            Log.Error("", "", "CampoSustentableService", "GenerarPDFDeclaracion", content);

            var buffer = Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var client = new HttpClient(httpClientHandler, false))
            {
                var task = client.PostAsync(urlReporteCampo, byteContent);

                task.Wait();

                var response = task.Result;

                var stringContent = response.Content.ReadAsStringAsync();

                dynamic jsonResult = JObject.Parse(stringContent.Result);

                if (bool.Parse(jsonResult.HayErrores.ToString()))
                {
                    throw new InfoCustomException(jsonResult.Errores[0].Message);
                }

                string downloadKey = jsonResult.DownloadKey;
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

        private void ValidarCampo(Usuario usuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            if (!VerificarDeclaracion(campoProveedor.Proveedor_Id, campoProveedor.CampoCosecha.Cosecha_Id, campoProveedor.CUIT).DeclaracionFirmada)
            {
                throw new ValidationCustomException("El proveedor seleccionado no tiene firmada la declaración.");
            }

            if (campoProveedor.Archivo_Id == 0 && Path.GetExtension(archivoKmz.FileName).ToLower() != ".kmz")
            {
                throw new ValidationCustomException("El archivo debe tener formato KMZ.");
            }
        }

        private void GuardarArchivoKMZ(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {

            string fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, ".kmz");

            string rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campoProveedor.CUIT);

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }

            campoProveedor.Archivo.Ruta = rutaArchivo;

            archivoKmz.SaveAs(rutaArchivo);
        }

        public EstadoDeclaracionSustentableDto VerificarDeclaracion(int proveedorId, int cosechaId, string CUITDeclaracion)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            var cosecha = repositorio.Obtener<Cosecha>(cosechaId);

            string razonSocial = "";

            if (string.IsNullOrEmpty(CUITDeclaracion))
            {
                CUITDeclaracion = proveedor.CUIT;
                razonSocial = proveedor.RazonSocial;
            }

            var estado = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
                CosechaActual = cosecha.Nombre,
                CUIT = CUITDeclaracion,
                RazonSocial = razonSocial,
                HectareasDeclaracionCampoSustentable = 0,
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad
            };

            var declaracion = repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == cosechaId && d.CUIT == CUITDeclaracion);

            if (declaracion != null)
            {
                if (declaracion.FechaFirma > cosecha.Inicio)
                {
                    var fileKey = string.Concat(FileKeys.DeclaracionCampoSustentable, "-", cosechaId);
                    estado.DeclaracionFirmada = !string.IsNullOrEmpty(declaracion.Archivo?.Ruta);
                    estado.OpcionDeclaracionCampoSustentable = declaracion.OpcionDeclarada;
                    estado.HectareasDeclaracionCampoSustentable = declaracion.HectareasDeclaradas;
                    estado.CUIT = declaracion.CUIT;
                    estado.RazonSocial = declaracion.RazonSocial;
                }
            }

            return estado;
        }

        private Cosecha ObtenerCosechaActual()
        {
            return repositorio.Obtener<Cosecha>(c => DateTime.Now > c.Inicio && DateTime.Now < c.Fin);
        }

        public string AdjuntarDeclaracionFirmada(string mailUsuario, int proveedorId, int cosechaId, string CUITDeclaracion, HttpPostedFileBase fileSubido)
        {
            if (fileSubido == null)
            {
                throw new ValidationCustomException("Debe subir el archivo de declaración en formato PDF");
            }

            if (Path.GetExtension(fileSubido.FileName).ToLower() != ".pdf")
            {
                throw new ValidationCustomException("Debe subir el archivo de declaración en formato PDF");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            string fileName = Path.GetFileName(fileSubido.FileName);

            //var fileKey = FileKeys.DeclaracionCampoSustentable;
            var fileKey = string.Concat(FileKeys.DeclaracionCampoSustentable, "-", cosechaId);

            if (string.IsNullOrEmpty(CUITDeclaracion))
            {
                CUITDeclaracion = proveedor.CUIT;
            }

            var declaracion = repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == cosechaId && d.CUIT == CUITDeclaracion);

            if (declaracion == null)
            {
                throw new ValidationCustomException("Debe haber imprimido la declaración antes de adjuntarla.");
            }

            string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

            string rutaCarpeta = string.Concat(rutaArchivosProveedores, "/", CUITDeclaracion, "/", proveedor.Id, "/", fileKey);

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            declaracion.FechaFirma = DateTime.Now;

            Directory.CreateDirectory(rutaCarpeta);

            //Si subieron otros archivos anteriormente, los borramos
            DirectoryInfo carpeta = new DirectoryInfo(rutaCarpeta);

            foreach (FileInfo file in carpeta.GetFiles())
            {
                file.Delete();
            }

            //var archivoRemover = proveedor.Archivos.FirstOrDefault(a => a.FileKey == fileKey);

            //if (archivoRemover != null)
            //{
            //    repositorio.Remover(archivoRemover);
            //}

            declaracion.Archivo.Ruta = rutaArchivo;

            fileSubido.SaveAs(rutaArchivo);

            repositorio.GuardarCambios();

            return SuccessMsg.DeclaracionCampoSustentableFirmada;
        }

        public byte[] GenerarDeclaracionProveedor(string mailUsuario, int proveedorId, int cosechaId, double hectareasTotales, string CUITDeclaracion, string razonSocialDeclaracion)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            if (string.IsNullOrEmpty(CUITDeclaracion))
            {
                CUITDeclaracion = proveedor.CUIT;
                razonSocialDeclaracion = proveedor.RazonSocial;
            }

            var declaracion = repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == cosechaId && d.CUIT == CUITDeclaracion);

            if (declaracion != null)
            {
                declaracion.FechaFirma = DateTime.Now;
                declaracion.OpcionDeclarada = hectareasTotales > 0 ? OpcionesDeclaracionCampoSustentable.Parcial : OpcionesDeclaracionCampoSustentable.Totalidad;
                declaracion.HectareasDeclaradas = hectareasTotales;
                declaracion.Proveedor_Id = proveedorId;
                declaracion.RazonSocial = razonSocialDeclaracion;
            }
            else
            {
                var fileKey = string.Concat(FileKeys.DeclaracionCampoSustentable, "-", cosechaId);

                declaracion = new DeclaracionCampoSustentable
                {
                    Proveedor_Id = proveedorId,
                    Cosecha_Id = cosechaId,
                    FechaFirma = DateTime.Now,
                    OpcionDeclarada = hectareasTotales > 0 ? OpcionesDeclaracionCampoSustentable.Parcial : OpcionesDeclaracionCampoSustentable.Totalidad,
                    HectareasDeclaradas = hectareasTotales,
                    CUIT = CUITDeclaracion,
                    RazonSocial = razonSocialDeclaracion,
                    Archivo = new Archivo { FileKey = fileKey, Ruta = "" }
                };

                repositorio.Agregar(declaracion);
            }

            repositorio.GuardarCambios();

            var cosecha = repositorio.Obtener<Cosecha>(cosechaId);

            DeclaracionCampoSustentableDto datos = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = declaracion.CUIT,
                RazonSocial = declaracion.RazonSocial,
                Fecha = declaracion.FechaFirma?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = declaracion.HectareasDeclaradas.Value,
                Campos = null
            };

            var pdfBytes = GenerarPDFDeclaracion(datos);

            byte[] archivoResult;
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader pdfReader = new PdfReader(pdfBytes);
                pdfReader.SelectPages("1");

                PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);
                pdfStamper.Close();
                pdfReader.Close();

                archivoResult = stream.ToArray();
            }

            return archivoResult;
        }

        public List<Cosecha> ObtenerCosechas(bool incluirInactivas)
        {
            return repositorio.Listar<Cosecha>(c => incluirInactivas || c.PermitirAltas);
        }

        public List<CampoProveedorListadoDto> Listar(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var resultado = ListarCampos(usuario, cp => new CampoProveedorListadoDto()
            {
                IdScato = cp.CampoCosecha.Campo.IdScato,
                NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                HectareasSoja = cp.HectareasSoja,
                HectareasTotales = cp.HectareasTotales,
                NombreCampo = cp.CampoCosecha.Campo.Nombre,
                ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                CampoCosechaId = cp.CampoCosecha_Id,
                Proveedor = new ProveedorDto()
                {
                    Id = cp.Proveedor.Id,
                    CodigoProveedor = cp.Proveedor.CodigoProveedor,
                    RazonSocial = cp.Proveedor.RazonSocial
                },
                CodigoProveedor = cp.Proveedor.CodigoProveedor,
                CUITProveedor = cp.CUIT,
                RazonSocialProveedor = cp.RazonSocial,
                CosechaId = cp.CampoCosecha.Cosecha_Id,
                MotivoRechazo = cp.CampoCosecha.MotivoRechazo,
                FechaCreacion = cp.FechaCreacion
            });

            return resultado;
        }

        public CampoProveedorDto ObtenerCampo(string mailUsuario, int proveedorId, int campoCosechaId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var campo = repositorio.Obtener<CampoProveedor, CampoProveedorDto>(p => p.Proveedor_Id == proveedorId && p.CampoCosecha_Id == campoCosechaId,
                            cp => new CampoProveedorDto
                            {
                                NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                                HectareasSoja = cp.HectareasSoja,
                                HectareasTotales = cp.HectareasTotales,
                                NombreCampo = cp.CampoCosecha.Campo.Nombre,
                                Localidad_Id = cp.CampoCosecha.Campo.Localidad_Id,
                                ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                                Latitud = cp.Latitud,
                                Longitud = cp.Longitud,
                                CampoCosechaId = cp.CampoCosecha_Id,
                                ProveedorNombre = cp.RazonSocial,
                                LocalidadNombre = cp.CampoCosecha.Campo.Localidad.Nombre,
                                CampoSustentableId = cp.CampoCosecha.CampoSustentable_Id,
                                CosechaId = cp.CampoCosecha.Cosecha_Id,
                                CUIT = cp.CUIT,
                                Archivo_Id = cp.Archivo_Id,
                                Proveedor_Id = cp.Proveedor_Id,
                                CodigoProveedor = cp.Proveedor.CodigoProveedor
                            });

            return campo;
        }

        public string ExportarCamposProveedores(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esAdminCampos = usuario.TienePermiso(PermisoEnum.VerTodosCamposSustentable);
            var headersBase = new List<string>() { "Cosecha", "Campo", "Proveedor", "Cuit", "Estado", "Motivo", "Ha Totales", "Ha Soja", "Toneladas Aprobadas", "Razon Social", "Fecha Creacion" };
            dynamic listado;

            if (esAdminCampos)
            {
                headersBase.Insert(0, "Id Scato");
                listado = ListarCampos(usuario, cp => new CampoSustentableExportDTO()
                {
                    Campo = cp.CampoCosecha.Campo.Nombre,
                    ProveedorRazonSocial = cp.Proveedor.RazonSocial,
                    CuitProveedor = cp.Proveedor.CUIT,
                    Cosecha = cp.CampoCosecha.Cosecha.Nombre,
                    HectareasSoja = cp.HectareasSoja,
                    HectareasTotales = cp.HectareasTotales,
                    IdScato = cp.CampoCosecha.Campo.IdScato,
                    Motivo = cp.CampoCosecha.MotivoRechazo,
                    ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                    RazonSocial = cp.RazonSocial,
                    FechaCreacion = cp.FechaCreacion
                });
            }
            else
            {
                listado = ListarCampos(usuario, cp => new CampoSustentableExportBaseDTO()
                {
                    Campo = cp.CampoCosecha.Campo.Nombre,
                    ProveedorRazonSocial = cp.Proveedor.CodigoProveedor,
                    Cosecha = cp.CampoCosecha.Cosecha.Nombre,
                    HectareasSoja = cp.HectareasSoja,
                    HectareasTotales = cp.HectareasTotales,
                    Motivo = cp.CampoCosecha.MotivoRechazo,
                    ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                    RazonSocial = cp.RazonSocial,
                    FechaCreacion = cp.FechaCreacion
                });
            }

            return excelExport.ToExcel(listado, headersBase.ToArray(), "Reporte Campos Sustentables");
            //return ExcelExport.ToExcel(listado, headersBase.ToArray(), "Reporte Campos Sustentables");
        }

        private List<TProyeccion> ListarCampos<TProyeccion>(Usuario usuario, Expression<Func<CampoProveedor, TProyeccion>> proyeccion) where TProyeccion : class
        {
            var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodosCamposSustentable);
            var esComercial = usuario.TienePermiso(PermisoEnum.ComercialCamposSustentables);
            if (esAdmin || esComercial)
            {
                return repositorio.Listar(proyeccion, p => !p.Borrado, 0, "FechaCreacion", SustitucionMOAModel.Consultas.DirOrden.Desc);
            }
            else
            {
                var proveedoresIds = usuario.Proveedores.Select(pr => pr.Id);
                var proveedoresCuits = usuario.Proveedores.Select(pr => pr.CUIT);

                return repositorio.Listar(proyeccion, p =>
                    (proveedoresIds.Contains(p.Proveedor_Id) || proveedoresCuits.Contains(p.CUIT)) &&
                    !p.Borrado, 0, "FechaCreacion", SustitucionMOAModel.Consultas.DirOrden.Desc
                );
            }
        }

        public string ObtenerRutaArchivoKMZ(int campoCosechaId, int proveedorId)
        {
            CampoProveedor campoProveedor = repositorio.Obtener<CampoProveedor>(x => x.Proveedor_Id == proveedorId && x.CampoCosecha_Id == campoCosechaId);

            return campoProveedor.Archivo.Ruta;
        }

        internal void InformarCampoSustentable(CampoProveedor campoProveedor, string archivoKmz)
        {
            dataAgroService.AltaCampoSustentable(campoProveedor, archivoKmz);
        }

        private int ObtenerIdScato(CampoProveedor campoProveedor)
        {
            var campoNombre = campoProveedor.CampoCosecha.Campo.Nombre;
            var localidadId = campoProveedor.CampoCosecha.Campo.Localidad_Id;
            var cuitProveedor = campoProveedor.CUIT;

            var idsScato = repositorio.Listar<CampoProveedor, int>(
                cp => cp.CampoCosecha.Campo.IdScato,
                cp =>
                    cp.CUIT == cuitProveedor &&
                    cp.CampoCosecha.Campo.Localidad_Id == localidadId &&
                    cp.CampoCosecha.Campo.Nombre == campoNombre &&
                    cp.CampoCosecha.Campo.IdScato > 0
                );

            return idsScato != null && idsScato.Count > 0 ? idsScato.Max() : 0;
        }

        /// <summary>
        /// se Convierte un archivo a 64 bits
        /// </summary>
        /// <param name="archivoKmz"></param>
        /// <returns></returns>
        public string ConvertirArchivo64(HttpPostedFileBase archivoKmz)
        {
            string theFileName = Path.GetFileName(archivoKmz.FileName);
            byte[] thePictureAsBytes = new byte[archivoKmz.ContentLength];
            using (BinaryReader theReader = new BinaryReader(archivoKmz.InputStream))
            {
                thePictureAsBytes = theReader.ReadBytes(archivoKmz.ContentLength);
            }
            return Convert.ToBase64String(thePictureAsBytes);
        }
    }
}
