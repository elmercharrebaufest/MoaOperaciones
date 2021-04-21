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
    public class CampoSustentableService : ICampoSustentableService
    {
        private readonly IRepositorio repositorio;
        private readonly string DataAgroURL;

        public CampoSustentableService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
        }

        public Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            ValidarCampo(usuario, campoProveedor, archivoKmz);

            campoProveedor.FechaCreacion = DateTime.Now;
            campoProveedor.Borrado = false;
            campoProveedor.Archivo = (new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" });
            campoProveedor.CampoCosecha.ToneladasAprobadas = -1;

            repositorio.Agregar(campoProveedor);

            repositorio.GuardarCambios();

            GuardarArchivoKMZ(campoProveedor, archivoKmz);

            repositorio.GuardarCambios();

            return new Resultado { IdEntidad = campoProveedor.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableAgregado };
        }

        private void ValidarUsuario(Usuario usuario, int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            if (!usuario.ObtenerPermisos().Contains("VER TODOS CAMPOS SUSTENTABLE"))
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

            ValidarUsuario(usuario, campoProveedorObj.Proveedor_Id);

            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == campoProveedorObj.Proveedor_Id && cp.CampoCosecha_Id == campoProveedorObj.CampoCosecha_Id);

            if (campoProveedor == null)
            {
                throw new ValidationCustomException("No se encontró el campo sustentable para editar.");
            }

            campoProveedor.FechaModificacion = DateTime.Now;
            /*
            ValidarCampo(usuario, campoProveedor, archivoKmz);

            campoProveedor.HectareasSoja = campoProveedorObj.HectareasSoja;
            campoProveedor.HectareasTotales = campoProveedorObj.HectareasTotales;
            campoProveedor.Longitud = campoProveedorObj.Longitud;
            campoProveedor.Latitud = campoProveedorObj.Latitud;*/
            campoProveedor.CampoCosecha.Campo.Nombre = campoProveedorObj.CampoCosecha.Campo.Nombre;

            repositorio.GuardarCambios();

            //GuardarArchivoKMZ(campoProveedor, archivoKmz);

            //repositorio.GuardarCambios();

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

        public byte[] ImprimirDeclaracion(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            var cosecha = ObtenerCosechaActual();

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
                cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha.Cosecha_Id == cosecha.Id);

            DeclaracionCampoSustentableDto datos = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = proveedor.CUIT,
                RazonSocial = proveedor.RazonSocial,
                Fecha = proveedor.FechaFirmaDeclaracionCampoSustentable?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = proveedor.HectareasDeclaracionCampoSustentable.Value,
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

                pdfReaderCampos.SelectPages("2");

                var archivoDeclaracion = proveedor.Archivos.FirstOrDefault(a => a.FileKey == FileKeys.DeclaracionCampoSustentable);
                byte[] fileBytes = File.ReadAllBytes(archivoDeclaracion.Ruta);

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
            if (!VerificarDeclaracion(campoProveedor.Proveedor_Id).DeclaracionFirmada)
            {
                throw new ValidationCustomException("El proveedor seleccionado no tiene firmada la declaración.");
            }

            if (Path.GetExtension(archivoKmz.FileName).ToLower() != ".kmz")
            {
                throw new ValidationCustomException("El archivo debe tener formato KMZ.");
            }
        }

        private void GuardarArchivoKMZ(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {

            string fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, ".kmz");

            string rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campoProveedor.Proveedor.CUIT);

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }

            campoProveedor.Archivo.Ruta = rutaArchivo;

            archivoKmz.SaveAs(rutaArchivo);
        }

        public EstadoDeclaracionSustentableDto VerificarDeclaracion(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            Cosecha cosechaActual = ObtenerCosechaActual();

            var estado = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
                CosechaActual = cosechaActual.Nombre,
                CUIT = proveedor.CUIT,
                RazonSocial = proveedor.RazonSocial,
                HectareasDeclaracionCampoSustentable = 0,
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad
            };

            if (proveedor.FechaFirmaDeclaracionCampoSustentable != null)
            {
                if (proveedor.FechaFirmaDeclaracionCampoSustentable > cosechaActual.Inicio)
                {
                    estado.DeclaracionFirmada = proveedor.Archivos.Any(a => a.FileKey == FileKeys.DeclaracionCampoSustentable);
                    estado.OpcionDeclaracionCampoSustentable = proveedor.OpcionDeclaracionCampoSustentable;
                    estado.HectareasDeclaracionCampoSustentable = proveedor.HectareasDeclaracionCampoSustentable;
                }
            }

            return estado;
        }

        private Cosecha ObtenerCosechaActual()
        {
            return repositorio.Obtener<Cosecha>(c => DateTime.Now > c.Inicio && DateTime.Now < c.Fin);
        }

        public string FirmarDeclaracion(string mailUsuario, int proveedorId, double hectareasTotales)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            proveedor.FechaFirmaDeclaracionCampoSustentable = DateTime.Now;

            proveedor.OpcionDeclaracionCampoSustentable = hectareasTotales > 0 ? OpcionesDeclaracionCampoSustentable.Parcial : OpcionesDeclaracionCampoSustentable.Totalidad;

            proveedor.HectareasDeclaracionCampoSustentable = hectareasTotales;

            repositorio.GuardarCambios();

            return SuccessMsg.DeclaracionCampoSustentableFirmada;
        }

        public string AdjuntarDeclaracionFirmada(string mailUsuario, int proveedorId, HttpPostedFileBase fileSubido)
        {
            if (Path.GetExtension(fileSubido.FileName).ToLower() != ".pdf")
            {
                throw new ValidationCustomException("Debe subir el archivo de declaración en formato PDF");
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            string fileName = Path.GetFileName(fileSubido.FileName);

            var fileKey = FileKeys.DeclaracionCampoSustentable;

            string rutaArchivosProveedores = ConfigurationManager.AppSettings["RutaArchivosProveedores"];

            string rutaCarpeta = string.Concat(rutaArchivosProveedores, "/", proveedor.CUIT, "/", proveedor.Id, "/", fileKey);

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            proveedor.FechaFirmaDeclaracionCampoSustentable = DateTime.Now;

            Directory.CreateDirectory(rutaCarpeta);

            //Si subieron otros archivos anteriormente, los borramos
            DirectoryInfo carpeta = new DirectoryInfo(rutaCarpeta);

            foreach (FileInfo file in carpeta.GetFiles())
            {
                file.Delete();
            }

            var archivoRemover = proveedor.Archivos.FirstOrDefault(a => a.FileKey == fileKey);

            if (archivoRemover != null)
            {
                repositorio.Remover(archivoRemover);
            }

            proveedor.Archivos.Add(new Archivo { FileKey = fileKey, Ruta = rutaArchivo });

            fileSubido.SaveAs(rutaArchivo);

            repositorio.GuardarCambios();

            return SuccessMsg.DeclaracionCampoSustentableFirmada;
        }

        public byte[] GenerarDeclaracionProveedor(string mailUsuario, int proveedorId, double hectareasTotales)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            proveedor.FechaFirmaDeclaracionCampoSustentable = DateTime.Now;

            proveedor.OpcionDeclaracionCampoSustentable = hectareasTotales > 0 ? OpcionesDeclaracionCampoSustentable.Parcial : OpcionesDeclaracionCampoSustentable.Totalidad;

            proveedor.HectareasDeclaracionCampoSustentable = hectareasTotales;

            repositorio.GuardarCambios();

            var cosecha = ObtenerCosechaActual();

            DeclaracionCampoSustentableDto datos = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = proveedor.CUIT,
                RazonSocial = proveedor.RazonSocial,
                Fecha = proveedor.FechaFirmaDeclaracionCampoSustentable?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = proveedor.HectareasDeclaracionCampoSustentable.Value,
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

        public List<Cosecha> ObtenerCosechas()
        {
            return repositorio.Listar<Cosecha>();
        }

        public List<CampoProveedorListadoDto> Listar(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            List<CampoProveedorListadoDto> listado;

            if (usuario.ObtenerPermisos().Contains("VER TODOS CAMPOS SUSTENTABLE"))
            {
                listado = repositorio
                               .Listar<CampoProveedor>(p => !p.Borrado)
                               .Select(cp => new CampoProveedorListadoDto
                               {
                                   IdScato = cp.CampoCosecha.Campo.IdScato,
                                   NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                                   HectareasSoja = cp.HectareasSoja,
                                   HectareasTotales = cp.HectareasTotales,
                                   NombreCampo = cp.CampoCosecha.Campo.Nombre,
                                   ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                                   CampoCosechaId = cp.CampoCosecha_Id,
                                   Proveedor = new ProveedorDto(cp.Proveedor),
                                   CodigoProveedor = cp.Proveedor.CodigoProveedor
                               }).ToList();
            }
            else
            {
                var proveedoresIds = usuario.Proveedores.Select(pr => pr.Id);
                listado = repositorio
                         .Listar<CampoProveedor>(p => proveedoresIds.Contains(p.Proveedor_Id) && !p.Borrado)
                         .Select(cp => new CampoProveedorListadoDto
                         {
                             IdScato = cp.CampoCosecha.Campo.IdScato,
                             NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                             HectareasSoja = cp.HectareasSoja,
                             HectareasTotales = cp.HectareasTotales,
                             NombreCampo = cp.CampoCosecha.Campo.Nombre,
                             ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                             CampoCosechaId = cp.CampoCosecha_Id,
                             Proveedor = new ProveedorDto(cp.Proveedor),
                             CodigoProveedor = cp.Proveedor.CodigoProveedor
                         }).ToList();
            }
            return listado;
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
                                ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                                Latitud = cp.Latitud,
                                Longitud = cp.Longitud,
                                CampoCosechaId = cp.CampoCosecha_Id,
                                ProveedorNombre = cp.Proveedor.RazonSocial,
                                LocalidadNombre = cp.CampoCosecha.Campo.Localidad.Nombre,
                                CampoSustentableId = cp.CampoCosecha.CampoSustentable_Id,
                                CosechaId = cp.CampoCosecha.Cosecha_Id,
                            });

            return campo;
        }
    }
}
