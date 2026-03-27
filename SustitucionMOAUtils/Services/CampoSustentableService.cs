using iTextSharp.text;
using iTextSharp.text.pdf;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SharpKml.Base;
using SharpKml.Engine;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export.CampoSustentable;
using SustitucionMOAUtils.Extensions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace SustitucionMOAUtils.Services
{
    public class CampoSustentableService : ICampoSustentableService
    {
        private readonly IRepositorioCampoSustentable repositorio;
        private readonly IExcelExportWrapper excelExport;
        private readonly IDataAgroService dataAgroService;
        private readonly ICampoSustentableGoogleDrive campoSustentableGoogleDrive;
        private readonly ICampoSustentablePdfGenerator campoSustentablePdfGenerator;

        public CampoSustentableService(
            IRepositorioCampoSustentable repositorio,
            IExcelExportWrapper excelExport,
            IDataAgroService dataAgroService,
            ICampoSustentableGoogleDrive campoSustentableGoogleDrive,
            ICampoSustentablePdfGenerator campoSustentablePdfGenerator
            )
        {
            this.repositorio = repositorio;
            this.excelExport = excelExport;
            this.dataAgroService = dataAgroService;
            this.campoSustentableGoogleDrive = campoSustentableGoogleDrive;
            this.campoSustentablePdfGenerator = campoSustentablePdfGenerator;
        }

        public Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz, bool UsarArchivoId, HttpPostedFileBase archivoEPA)
        {
            var ruta = "";
            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);

            if (archivoKmz != null)
                ValidarCampoPoligonoKmz(campoProveedor, archivoKmz);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            bool renspaExiste =
                RenspaExiste(campoProveedor.CampoCosecha.Campo.Renspa,
                             campoProveedor.CUIT,
                             campoProveedor.CampoCosecha.Cosecha_Id,
                             campoProveedor.EPA,
                             campoProveedor.BSVS2,
                             campoProveedor.EUDR);

            if (renspaExiste)
            {
                throw new ValidationCustomException("Este campo ya fue presentado.");
            }

            ValidarCampo(campoProveedor, archivoKmz);

            if (!ExisteCarpetaUcropIt(campoProveedor))
            {
                EnviarMailCarpetasUCROPIT(campoProveedor);
                throw new ValidationCustomException("No se encontró la configuración de carpeta para subir el archivo KMZ. Contacte al administrador.");
            }

            var declaracion = repositorio.ObtenerDeclaracionDeProveedor(campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id);

            if (campoProveedor.BSVS2 && declaracion != null)
            {
                campoProveedor.RazonSocial = declaracion.RazonSocial;
            }
            else
            {
                var proveedor = this.repositorio.Obtener<Proveedor>(p => p.Id == campoProveedor.Proveedor_Id);
                campoProveedor.RazonSocial = proveedor.RazonSocial ?? string.Empty;
            }

            campoProveedor.FechaCreacion = DateTime.Now;
            campoProveedor.Borrado = false;

            if (campoProveedor.Archivo_Id == 0)
            {
                var archivoKmzEntidad = new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" };
                repositorio.Agregar(archivoKmzEntidad);
                campoProveedor.Archivo = archivoKmzEntidad;
            }
            else
            {
                var archivoCampo = repositorio.ObtenerArchivo(campoProveedor.Archivo_Id);
                ruta = archivoCampo.Ruta;
            }

            //Guardado Evidencia EPA

            if (campoProveedor.EPA)
            {
                if (archivoEPA != null && !campoProveedor.EvidenciaPresentada)
                {
                    var archivoEPAEntidad = new Archivo { FileKey = FileKeys.ArchivoEPA, Ruta = "" };
                    repositorio.Agregar(archivoEPAEntidad);
                    campoProveedor.EvidenciaEPA = archivoEPAEntidad;
                    campoProveedor.EvidenciaEPA.Ruta = GuardarArchivoEPA(campoProveedor, archivoEPA);
                }
                else if (campoProveedor.EvidenciaPresentada && archivoKmz != null)
                {
                    archivoKmz.InputStream.Position = 0;
                    var archivoEPAExistente = this.ObtenerEPAExistenteCampo(campoProveedor.Proveedor_Id, campoProveedor.CUIT, archivoKmz);
                    campoProveedor.EvidenciaEPA = archivoEPAExistente;
                    campoProveedor.EvidenciaEPA_Id = archivoEPAExistente?.Id;
                }
                else
                {
                    campoProveedor.EvidenciaEPA = null;
                    campoProveedor.EvidenciaEPA_Id = null;
                }
            }
            else
            {
                campoProveedor.EvidenciaEPA = null;
                campoProveedor.EvidenciaEPA_Id = null;
            }

            campoProveedor.CampoCosecha.Campo.IdScato = ObtenerIdScato(campoProveedor);
            campoProveedor.CampoCosecha.Cosecha = repositorio.Obtener<Cosecha>(campoProveedor.CampoCosecha.Cosecha_Id);

            repositorio.Agregar(campoProveedor);

            this.AgregarNormativas(campoProveedor);

            repositorio.GuardarCambios();
            if (!UsarArchivoId)
            {
                ruta = GuardarArchivoKMZ(campoProveedor, archivoKmz);

                repositorio.GuardarCambios();
            }

            EnviarCampoACertificadorDeSustentables(ruta, campoProveedor);

            var archivo = archivoKmz == null ? Convert.ToBase64String(File.ReadAllBytes(ruta)) : ConvertirArchivo64(archivoKmz);
            InformarCampoSustentable(campoProveedor, archivo);
            return new Resultado { IdEntidad = campoProveedor.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableAgregado };
        }

        private void AgregarNormativas(CampoProveedor campoProveedor)
        {
            var normativas = new List<string>();

            if (campoProveedor.BSVS2)
                normativas.Add("2BSVS");

            if (campoProveedor.EPA)
                normativas.Add("EPA");

            if (campoProveedor.EUDR)
                normativas.Add("EUDR");

            foreach (var normativa in normativas)
            {
                var tipoNormativa = this.repositorio.Obtener<TipoNormativa>(n => n.Descripcion == normativa);
                this.repositorio.Agregar(new CampoCosechaNormativa
                {
                    CampoCosecha = campoProveedor.CampoCosecha,
                    TipoNormativa = tipoNormativa,
                    ToneladasAprobadas = -1,
                });
            }
        }


        public Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz, HttpPostedFileBase archivoEPA)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedorObj.Proveedor_Id);

            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == campoProveedorObj.Proveedor_Id && cp.CampoCosecha_Id == campoProveedorObj.CampoCosecha_Id);

            if (campoProveedor == null)
            {
                throw new ValidationCustomException("No se encontró el campo sustentable para editar.");
            }

            campoProveedor.FechaModificacion = DateTime.Now;

            campoProveedor.HectareasSoja = campoProveedorObj.HectareasSoja;
            campoProveedor.HectareasTotales = campoProveedorObj.HectareasTotales;
            campoProveedor.Longitud = campoProveedorObj.Longitud;
            campoProveedor.Latitud = campoProveedorObj.Latitud;
            campoProveedor.CampoCosecha.Campo.Nombre = campoProveedorObj.CampoCosecha.Campo.Nombre;
            campoProveedor.CampoCosecha.Campo.Renspa = campoProveedorObj.CampoCosecha.Campo.Renspa;
            campoProveedor.CampoCosecha.Campo.Localidad_Id = campoProveedorObj.CampoCosecha.Campo.Localidad_Id;

            ActualizarEPA(campoProveedor, archivoEPA);

            repositorio.GuardarCambios();

            InformarCampoSustentable(campoProveedor, "");

            return new Resultado { IdEntidad = campoProveedorObj.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableActualizado };
        }

        private void ActualizarEPA(CampoProveedor campoProveedor, HttpPostedFileBase archivoEPA)
        {
            // Si EPA está seleccionada y hay archivo parametro, actualizar la evidencia
            if (campoProveedor.EPA && archivoEPA != null && !campoProveedor.EvidenciaPresentada)
            {
                if (campoProveedor.EvidenciaEPA == null)
                    campoProveedor.EvidenciaEPA = new Archivo { FileKey = FileKeys.ArchivoEPA, Ruta = "" };

                campoProveedor.EvidenciaEPA.Ruta = GuardarArchivoEPA(campoProveedor, archivoEPA);
            }
        }

        private void ActualizarNormativas(CampoProveedor campoProveedor, HttpPostedFileBase archivoEPA)
        {
            var normativasSeleccionadas = new List<string>();
            if (campoProveedor.EPA) normativasSeleccionadas.Add("EPA");
            if (campoProveedor.EUDR) normativasSeleccionadas.Add("EUDR");
            if (campoProveedor.BSVS2) normativasSeleccionadas.Add("2BSVS");

            var tiposNormativas = repositorio.Listar<TipoNormativa>().ToList();
            var normativasCampo = campoProveedor.CampoCosecha.CampoCosechaNormativas?.ToList()
                                     ?? new List<CampoCosechaNormativa>();

            var normativasCampoAgregar = new List<TipoNormativa>();
            var normativasCampoEliminar = new List<CampoCosechaNormativa>();

            foreach (var tipo in tiposNormativas)
            {
                bool estaSeleccionada = normativasSeleccionadas.Contains(tipo.Descripcion);
                var existente = normativasCampo.FirstOrDefault(n =>
                    n?.TipoNormativa?.Descripcion == tipo.Descripcion);

                if (estaSeleccionada && existente == null)
                {
                    normativasCampoAgregar.Add(tipo);
                }
                else if (!estaSeleccionada && existente != null)
                {
                    normativasCampoEliminar.Add(existente);
                }
            }

            // Agregar las nuevas normativas
            foreach (var tipoNormativa in normativasCampoAgregar)
            {
                var nuevaNormativa = new CampoCosechaNormativa
                {
                    CampoCosecha = campoProveedor.CampoCosecha,
                    TipoNormativa = tipoNormativa,
                    ToneladasAprobadas = -1
                };
                repositorio.Agregar(nuevaNormativa);
            }

            // Eliminar las normativas desmarcadas
            foreach (var normativa in normativasCampoEliminar)
            {
                if (normativa.TipoNormativa?.Descripcion == "EPA")
                {
                    campoProveedor.EvidenciaEPA = null;
                    campoProveedor.EvidenciaEPA_Id = null;
                }
                repositorio.Remover(normativa);
            }
        }

        public string Borrar(string mailUsuario, int campoCosechaId, int proveedorId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            if (campoProveedor.CampoCosecha.CampoCosechaNormativas.Any(n => n.ToneladasAprobadas > 0))
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
                    HectareasSoja = c.HectareasSojaUcropit != null ? c.HectareasSojaUcropit.ToString() : c.HectareasSoja.ToString(),
                    HectareasTotales = c.HectareasTotalesUcropit != null ? c.HectareasTotalesUcropit.ToString() : c.HectareasTotales.ToString(),
                    Localidad = c.CampoCosecha.Campo.Localidad.Nombre,
                    Nombre = c.CampoCosecha.Campo.Nombre,
                    Renspa = c.CampoCosecha.Campo.Renspa,
                    Pais = "Argentina",
                    Provincia = c.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    Coordenadas = string.Concat(c.Latitud, " ", c.Longitud),
                    ToneladasAprobadas = c.CampoCosecha.CampoCosechaNormativas.FirstOrDefault(x => x.TipoNormativa.Descripcion == "2BSVS").ToneladasAprobadas.ToString(),
                    Partido = c.CampoCosecha.Campo.Localidad.Partido.Descripcion
                },
                cp => cp.CUIT == CUIT && cp.CampoCosecha.Cosecha_Id == cosecha.Id && cp.CampoCosecha.CampoCosechaNormativas.Any(x => x.TipoNormativa.Descripcion == "2BSVS" && x.ToneladasAprobadas != -1));

            var declaracion = ObtenerDeclaracion(cosechaId, CUIT);

            var datosDeclaracionJurada = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = declaracion.CUIT,
                RazonSocial = declaracion.RazonSocial,
                Fecha = declaracion.FechaFirma?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = declaracion.HectareasDeclaradas.Value,
                Campos = allCampos,
                DirectivaDDJJCampoSustentable = cosecha.DirectivaDDJJCampoSustentable
            };

            //var pdfDeclaracionJurada = GenerarPDFDeclaracion(datosDeclaracionJurada);

            var pdfListaCampos = campoSustentablePdfGenerator.GenerarDeclaracionJuradaListaCampos(datosDeclaracionJurada);

            byte[] archivoResult;

            var document = new Document();

            using (MemoryStream stream = new MemoryStream())
            {
                var pdfCopy = new PdfCopy(document, stream);
                document.Open();

                var declaracionCargadaBytes = File.ReadAllBytes(declaracion.Archivo.Ruta);
                var declaracionCargadaPdfReader = new PdfReader(declaracionCargadaBytes);
                pdfCopy.AddDocument(declaracionCargadaPdfReader);
                declaracionCargadaPdfReader.Close();

                var pdfReaderListaCampos = new PdfReader(pdfListaCampos);
                pdfCopy.AddDocument(pdfReaderListaCampos);
                pdfReaderListaCampos.Close();

                document.Close();

                archivoResult = stream.ToArray();
            }

            return archivoResult;
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
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad,
                DirectivaDDJJCampoSustentable = cosecha.DirectivaDDJJCampoSustentable
            };

            var declaracion = repositorio.ObtenerDeclaracionDeProveedor(CUITDeclaracion, cosechaId);

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

            var datos = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = declaracion.CUIT,
                RazonSocial = declaracion.RazonSocial,
                Fecha = declaracion.FechaFirma?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = declaracion.HectareasDeclaradas.Value,
                Campos = null,
                DirectivaDDJJCampoSustentable = cosecha.DirectivaDDJJCampoSustentable
            };

            var pdfBytes = GenerarPDFDeclaracion(datos);
            pdfBytes = ReemplazarTextoEnPdf(pdfBytes, @"2018/2001/EC \(RED II\)", datos.DirectivaDDJJCampoSustentable);

            byte[] archivoResult;
            using (MemoryStream stream = new MemoryStream())
            {
                ActualizarPdf(pdfBytes, stream, datos);

                archivoResult = stream.ToArray();
            }

            //archivoResult = campoSustentablePdfGenerator.GenerarDeclaracionJurada(datos);

            return archivoResult;
        }

        public List<Cosecha> ObtenerCosechas(bool incluirInactivas)
        {
            return repositorio.Listar<Cosecha>(c => incluirInactivas || c.PermitirAltas);
        }

        public List<CampoProveedorListadoDto> Listar(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var campos = ListarCampos(usuario);

            var resultado = campos.Select(cp => new CampoProveedorListadoDto
            {
                IdScato = cp.IdScato,
                NombreCosecha = cp.NombreCosecha,
                HectareasSoja = cp.HectareasSoja ?? 0,
                HectareasTotales = cp.HectareasTotales ?? 0,
                NombreCampo = cp.NombreCampo,
                ToneladasAprobadas = cp.ToneladasAprobadas ?? 0,
                CampoCosechaId = cp.CampoCosecha_Id,
                Proveedor = new ProveedorDto
                {
                    Id = cp.IdProveedor,
                    CodigoProveedor = cp.CodigoProveedor,
                    RazonSocial = cp.RazonSocialProveedor
                },
                CodigoProveedor = cp.CodigoProveedor,
                CUITProveedor = cp.CUITCampoProveedor,
                RazonSocialProveedor = cp.RazonSocialCampoProveedor,
                CosechaId = cp.CosechaId,
                MotivoRechazo = cp.MotivoRechazo,
                FechaCreacion = cp.FechaCreacion,
                TipoNormativa = cp.TipoNormativa,
                TipoNormativaId = cp.TipoNormativaId,
                Validado = cp.Validado ?? false,
                ValidadoPor = cp.ValidadoPor,
                EvidenciaEPA_Id = cp.EvidenciaEPA_Id
            }
            ).ToList();

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
                                Renspa = cp.CampoCosecha.Campo.Renspa,
                                Localidad_Id = cp.CampoCosecha.Campo.Localidad_Id,
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
                                CodigoProveedor = cp.Proveedor.CodigoProveedor,
                                BSVS2 = cp.BSVS2,
                                EPA = cp.EPA,
                                EUDR = cp.EUDR,
                                EvidenciaEPA_Id = cp.EvidenciaEPA_Id,
                            });

            if (campo.EPA && campo.EvidenciaEPA_Id != null)
            {
                var archivo = this.repositorio.Obtener<Archivo>(a => a.Id == campo.EvidenciaEPA_Id);
                campo.NombreArchivoEPA = Path.GetFileName(archivo.Ruta);
                campo.ArchivoEPA = File.ReadAllBytes(archivo.Ruta);
            }

            return campo;
        }

        public string ExportarCamposProveedores(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esAdminCampos = usuario.TienePermiso(PermisoEnum.VerTodosCamposSustentable);
            var headersBase = new List<string> { "Cosecha", "Campo", "RENSPA", "Proveedor", "Cuit", "Estado", "Motivo", "Ha Totales", "Ha Soja", "Toneladas Aprobadas", "Normativa", "Razon Social", "Fecha Creacion" };
            var campos = ListarCampos(usuario);

            if (esAdminCampos)
                headersBase.Insert(0, "Id Scato");


            if (esAdminCampos)
            {
                var listado = campos
                        .Select(cp =>
                            new CampoSustentableExportDTO
                            {
                                Campo = cp.NombreCampo,
                                Renspa = cp.Renspa,
                                ProveedorRazonSocial = cp.RazonSocialProveedor,
                                CuitProveedor = cp.CUITProveedor,
                                Cosecha = cp.NombreCosecha,
                                HectareasSoja = cp.HectareasSoja ?? 0,
                                HectareasTotales = cp.HectareasTotales ?? 0,
                                IdScato = cp.IdScato,
                                Motivo = cp.MotivoRechazo,
                                ToneladasAprobadas = cp.ToneladasAprobadas ?? 0,
                                Normativa = cp.TipoNormativa,
                                RazonSocial = cp.RazonSocialCampoProveedor,
                                FechaCreacion = cp.FechaCreacion,
                            }).ToList();

                listado.ForEach(x => x.Renspa = x.Renspa.ToFormatoRenspa());
                return excelExport.ToExcel(listado, headersBase.ToArray(), "Reporte Campos Sustentables");
            }
            else
            {
                var listado = campos.Select(cp =>
                         new CampoSustentableExportBaseDTO
                         {
                             Campo = cp.NombreCampo,
                             Renspa = cp.Renspa,
                             ProveedorRazonSocial = cp.RazonSocialProveedor,
                             Cosecha = cp.NombreCosecha,
                             HectareasSoja = cp.HectareasSoja ?? 0,
                             HectareasTotales = cp.HectareasTotales ?? 0,
                             Motivo = cp.MotivoRechazo,
                             ToneladasAprobadas = cp.ToneladasAprobadas ?? 0,
                             Normativa = cp.TipoNormativa,
                             RazonSocial = cp.RazonSocialCampoProveedor,
                             FechaCreacion = cp.FechaCreacion,
                         }).ToList();

                listado.ForEach(x => x.Renspa = x.Renspa.ToFormatoRenspa());
                return excelExport.ToExcel(listado, headersBase.ToArray(), "Reporte Campos Sustentables");
            }
        }

        public string ObtenerRutaArchivoKMZ(int campoCosechaId, int proveedorId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(x => x.Proveedor_Id == proveedorId && x.CampoCosecha_Id == campoCosechaId);

            return campoProveedor.Archivo.Ruta;
        }

        public async Task DescargarArchivosDeGoogleDrive(ArchivoCampoSustentable archivoSinDescargar)
        {
            if (archivoSinDescargar.ProcesadoUcropit)
            {
                return;
            }
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.CampoCosecha_Id == archivoSinDescargar.CampoCosechaId);
            var cuit = campoProveedor.CUIT;

            var nombreArchivoBase = ObtenerNombreArchivoDrive(cuit, archivoSinDescargar.CampoCosecha);
            var rutaCarpeta = $"{ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"]}/{cuit}";

            async Task<ReporteProcesoUcropit> IntentarDescarga(string nombreArchivo)
            {
                return await campoSustentableGoogleDrive.DownloadFileAs<ReporteProcesoUcropit>(
                           new GoogleDriveFileDownloadRequest()
                               .WithFilePath($"{rutaCarpeta}/{nombreArchivo}")
                               .WithFileName(nombreArchivo)
                       );
            }

            ReporteProcesoUcropit resultadoProcesadoUcropit = null;
            string rutaGuardado = $"{rutaCarpeta}/{nombreArchivoBase}.json";

            try
            {
                resultadoProcesadoUcropit = await IntentarDescarga($"{nombreArchivoBase}.json");
            }
            catch (FileNotFoundException)
            {
                try
                {
                    rutaGuardado = $"{rutaCarpeta}/{nombreArchivoBase}..json";
                    resultadoProcesadoUcropit = await IntentarDescarga($"{nombreArchivoBase}..json");
                }
                catch (FileNotFoundException)
                {
                    return;
                }
            }

            var nuevoArchivo = new Archivo { FileKey = FileKeys.CampoSustentableAnalisisUcrop, Ruta = rutaGuardado };

            repositorio.Agregar(nuevoArchivo);

            archivoSinDescargar.Archivo = nuevoArchivo;
            archivoSinDescargar.ProcesadoUcropit = true;

            var normativas = new Dictionary<string, (bool flag, dynamic resultado)>
            {
                 { "2BSVS", (campoProveedor.BSVS2, resultadoProcesadoUcropit.Bsvs2) },
                 { "EPA",   (campoProveedor.EPA,   resultadoProcesadoUcropit.Epa) },
                 { "EUDR",  (campoProveedor.EUDR,  resultadoProcesadoUcropit.Eudr) }
            };

            foreach (var normativa in normativas)
            {
                if (normativa.Value.flag)
                {
                    var campoNormativa = archivoSinDescargar.CampoCosecha.CampoCosechaNormativas
                        .FirstOrDefault(x => x.TipoNormativa.Descripcion == normativa.Key);

                    if (campoNormativa != null)
                    {
                        campoNormativa.ToneladasAprobadas = normativa.Value.resultado != null
                            ? Math.Round(normativa.Value.resultado.ToneladasAprobadas ?? 0, 2)
                            : 0;

                        campoNormativa.MotivoRechazo = normativa.Value.resultado?.MotivoRechazo ?? null;
                    }
                }
            }

            campoProveedor.HectareasSojaUcropit = resultadoProcesadoUcropit.Bsvs2?.SuperficieElegible;
            campoProveedor.HectareasTotalesUcropit = resultadoProcesadoUcropit.Bsvs2?.SuperficieTotalCampo;

            repositorio.GuardarCambios();
        }

        public bool RenspaExiste(string renspa, string cuit, int cosechaId, bool epa, bool bsvs2, bool eudr)
        {
            var result = repositorio.Existe<CampoProveedor>(cp =>
            cp.CUIT == cuit &&
            !cp.Borrado &&
            cp.CampoCosecha.Campo.Renspa == renspa &&
            cp.CampoCosecha.Cosecha_Id == cosechaId &&
            ((epa && cp.EPA) || (eudr && cp.EUDR) || (bsvs2 && cp.BSVS2))
            );

            return result;
        }

        private void EnviarMailCarpetasUCROPIT(CampoProveedor campoProveedor)
        {
            try
            {
                var normativas = new List<string>();
                if (campoProveedor.EPA) normativas.Add("EPA");
                if (campoProveedor.BSVS2) normativas.Add("2BSVS");
                if (campoProveedor.EUDR) normativas.Add("EUDR");
                string normativasTexto = string.Join(", ", normativas);

                var cosecha = this.repositorio.Obtener<Cosecha>(c => c.Id == campoProveedor.CampoCosecha.Cosecha_Id);
                var cuerpo = $"Estimados, falta configurar carpeta drive, para la cosecha {cosecha} y las siguientes normativas: {normativasTexto}.";
                string asunto = "Molinos Agro - Falta configurar carpeta UCROPIT";
                string destinatarios = ConfigurationManager.AppSettings["EmailCarpetasUcropIt"];
                List<string> listaCorreos = destinatarios.Split(',').Select(x => x.Trim()).ToList();

                EmailSender.EnviarMail(listaCorreos, asunto, cuerpo, null, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public List<SugerenciaCampoDto> ObtenerSugerenciaCamposNuevaCosecha(int proveedorId, int cosechaId, string cuitTitularCP)
        {
            return repositorio.ObtenerSugerenciaCamposNuevaCosecha(proveedorId, cosechaId, cuitTitularCP);
        }

        public string ExportarCamposSugeridos(int proveedorId, int cosechaId, string cuitTitularCP)
        {
            var camposSugeridos = repositorio.ObtenerSugerenciaCamposNuevaCosecha(proveedorId, cosechaId, cuitTitularCP);
            var headers = new string[] { "Cosecha", "Nombre campo", "Localidad", "RENSPA", "Hectáreas totales", "Hectáreas soja", "Toneladas aprobadas", "Presentado en nueva cosecha" };
            var listadoExport = camposSugeridos.Select(x => new SugerenciaCampoExportDto
            {
                CampoNombre = x.NombreCampo,
                Cosecha = x.NombreCosecha,
                HectareasSoja = x.HectareasSoja,
                HectareasTotales = x.HectareasTotales,
                LocalidadNombre = x.LocalidadNombre,
                Presentado = x.CampoYaPresentado ? "SI" : "NO",
                Renspa = x.Renspa.ToFormatoRenspa(),
                ToneladasAprobadas = x.ToneladasAprobadas
            });
            return excelExport.ToExcel(listadoExport, headers, "Sugerencias campos nueva cosecha");
        }

        public void AgregarCamposSugeridos(List<SugerenciaCampoDto> camposSugeridosDto, List<HttpPostedFileBase> archivosKmz, List<HttpPostedFileBase> archivosEPA, string mailUsuario)
        {
            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            foreach (var proveedorId in camposSugeridosDto.Select(x => x.Proveedor_Id).Distinct())
            {
                ValidarUsuario(usuario, proveedorId);
            }

            foreach (var campoSugeridoDto in camposSugeridosDto)
            {
                var cosecha = this.repositorio.Obtener<Cosecha>(c => c.Id == campoSugeridoDto.CosechaId);
                var campoProveedor = new CampoProveedor
                {
                    HectareasTotales = campoSugeridoDto.HectareasTotales,
                    HectareasSoja = campoSugeridoDto.HectareasSoja,
                    CUIT = campoSugeridoDto.CUIT,
                    Latitud = campoSugeridoDto.Latitud,
                    Longitud = campoSugeridoDto.Longitud,
                    Proveedor_Id = campoSugeridoDto.Proveedor_Id,
                    CampoCosecha = new CampoCosecha
                    {
                        Cosecha_Id = campoSugeridoDto.CosechaId,
                        Campo = new CampoSustentable
                        {
                            Nombre = campoSugeridoDto.NombreCampo,
                            Localidad_Id = campoSugeridoDto.Localidad_Id,
                            Renspa = campoSugeridoDto.Renspa
                        },
                        Cosecha = cosecha
                    },
                    Archivo_Id = campoSugeridoDto.Archivo_Id,
                    FechaCreacion = DateTime.Now,
                    Borrado = false,
                    BSVS2 = campoSugeridoDto.BSVS2,
                    EPA = campoSugeridoDto.EPA,
                    EUDR = campoSugeridoDto.EUDR,
                    EvidenciaPresentada = campoSugeridoDto.EvidenciaPresentada
                };

                this.AgregarNormativas(campoProveedor);
                var fileEPA = archivosEPA?.FirstOrDefault(x => x.FileName == campoSugeridoDto.NombreNuevaEvidenciaEPA);
                if (campoSugeridoDto.EPA)
                {
                    if (fileEPA != null)
                    {
                        campoProveedor.EvidenciaEPA = (new Archivo { FileKey = FileKeys.ArchivoEPA, Ruta = "" });
                        campoProveedor.EvidenciaEPA.Ruta = GuardarArchivoEPA(campoProveedor, fileEPA);
                    }
                    else
                    {
                        //Asociamos la existente en la ruta
                        var epaExistente = this.repositorio.Obtener<Archivo>(a => a.Id == campoSugeridoDto.EvidenciaEPA_Id);
                        campoProveedor.EvidenciaEPA = epaExistente;
                    }
                }

                var renspaExisteDto = RenspaExiste(campoProveedor.CampoCosecha.Campo.Renspa, campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id, campoProveedor.EPA, campoProveedor.BSVS2, campoProveedor.EUDR);
                if (renspaExisteDto)
                {
                    Log.Info($"No se guarda el campo sugerido con renspa {campoProveedor.CampoCosecha.Campo.Renspa} porque ya existe para el mismo CUIT");
                }
                else
                {
                    Log.Info($"Se guarda para la cosecha id {campoProveedor.CampoCosecha.Cosecha_Id} el campo sugerido con renspa {campoProveedor.CampoCosecha.Campo.Renspa}");

                    var archivoNuevoKmz = !string.IsNullOrEmpty(campoSugeridoDto.NombreNuevoKmz) ? archivosKmz.FirstOrDefault(x => x.FileName == campoSugeridoDto.NombreNuevoKmz) : null;
                    ValidarCampo(campoProveedor, archivoNuevoKmz);

                    var declaracion = repositorio.ObtenerDeclaracionDeProveedor(campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id);

                    if (declaracion != null)
                    {
                        campoProveedor.RazonSocial = declaracion.RazonSocial;
                    }
                    else
                    {
                        var proveedor = this.repositorio.Obtener<Proveedor>(p => p.Id == campoProveedor.Proveedor_Id);
                        campoProveedor.RazonSocial = proveedor.RazonSocial ?? string.Empty;
                    }

                    campoProveedor.CampoCosecha.Campo.IdScato = ObtenerIdScato(campoProveedor);

                    var rutaArchivo = "";
                    if (string.IsNullOrEmpty(campoSugeridoDto.NombreNuevoKmz) && campoProveedor.Archivo_Id != 0)
                    {
                        var archivoCampo = repositorio.ObtenerArchivo(campoProveedor.Archivo_Id);
                        rutaArchivo = archivoCampo.Ruta;
                        repositorio.Agregar(campoProveedor);
                        repositorio.GuardarCambios();
                    }
                    else
                    {
                        campoProveedor.Archivo = new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" };
                        if (archivoNuevoKmz != null)
                        {
                            rutaArchivo = GuardarArchivoKMZ(campoProveedor, archivoNuevoKmz);
                        }
                        repositorio.Agregar(campoProveedor);
                        repositorio.GuardarCambios();
                    }

                    EnviarCampoACertificadorDeSustentables(rutaArchivo, campoProveedor);

                    var archivo = Convert.ToBase64String(File.ReadAllBytes(rutaArchivo));
                    InformarCampoSustentable(campoProveedor, archivo);
                }
            }
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
                    throw new ValidationCustomException($"Su usuario no tiene habilitado el proveedor con el que intenta operar ({proveedor.CUIT}).");
                }
            }
        }

        private byte[] GenerarPDFDeclaracion(DeclaracionCampoSustentableDto datos)
        {

            var content = JsonConvert.SerializeObject(datos);
            Log.Info($"CampoSustentableService, GenerarPDFDeclaracion, {content}");

            var dto = new SustitucionMOAWS.DataAgroServices.DeclaracionCampoSustentable
            {
                RazonSocial = datos.RazonSocial,
                Campos = datos.Campos?.Select(x => new SustitucionMOAWS.DataAgroServices.CamposSustentableReporte
                {
                    Nombre = x.Nombre,
                    Coordenadas = x.Coordenadas,
                    HectareasSoja = x.HectareasSoja,
                    HectareasTotales = x.HectareasTotales,
                    Localidad = x.Localidad,
                    Pais = x.Pais,
                    Partido = x.Partido,
                    Provincia = x.Provincia
                }).ToArray(),
                CantidadParteSoja = datos.CantidadParteSoja,
                Cosecha = datos.Cosecha,
                CUIT = datos.CUIT,
                Fecha = datos.Fecha
            };
            var RespuestaArchivoDto = dataAgroService.CamposSustentables(dto);
            if (RespuestaArchivoDto.Errores.Any())
            {
                throw new InfoCustomException(RespuestaArchivoDto.Errores[0]);
            }
            return RespuestaArchivoDto.Contenido;
        }

        private void ValidarCampo(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            if (campoProveedor.BSVS2 && !VerificarDeclaracion(campoProveedor.Proveedor_Id, campoProveedor.CampoCosecha.Cosecha_Id, campoProveedor.CUIT).DeclaracionFirmada)
            {
                throw new ValidationCustomException("El proveedor seleccionado no tiene firmada la declaración.");
            }

            if (campoProveedor.Archivo_Id == 0 && Path.GetExtension(archivoKmz.FileName).ToLower() != ".kmz")
            {
                throw new ValidationCustomException("El archivo debe tener formato KMZ.");
            }
        }

        private string GuardarArchivoKMZ(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            var extension = Path.GetExtension(archivoKmz.FileName);
            var fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, ".", extension);
            var rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campoProveedor.CUIT);
            var rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            archivoKmz.SaveAs(rutaArchivo);

            campoProveedor.Archivo.Ruta = rutaArchivo;
            return rutaArchivo;
        }

        private string GuardarArchivoEPA(CampoProveedor campoProveedor, HttpPostedFileBase archivo)
        {
            var fileName = Path.GetFileName(archivo.FileName);
            var rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campoProveedor.CUIT);
            var rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            archivo.SaveAs(rutaArchivo);

            campoProveedor.EvidenciaEPA.Ruta = rutaArchivo;
            return rutaArchivo;
        }

        private void EnviarCampoACertificadorDeSustentables(string rutaArchivo, CampoProveedor campoProveedor)
        {
            Log.Info($"EnviarCampoACertificadorDeSustentables archivo {rutaArchivo} proveedor id {campoProveedor.Proveedor_Id}");

            var archivoCampoSustentable = new ArchivoCampoSustentable
            {
                CampoCosechaId = campoProveedor.CampoCosecha_Id,
                IdArchivoRecepcion = 0,
                ProcesadoUcropit = false,
                ProveedorId = campoProveedor.Proveedor_Id
            };
            repositorio.Agregar(archivoCampoSustentable);

            SubirArchivosAGoogleDrive(rutaArchivo, campoProveedor);
            repositorio.GuardarCambios();
        }

        public void ReenviarCamposACertificadorDeSustentables()
        {
            var cosechaId = this.repositorio.Obtener<Cosecha>(c => c.Nombre == "25-26").Id;
            var archivosCamposAnalizados = this.repositorio.Listar<ArchivoCampoSustentable>(a => a.CampoCosecha.Cosecha_Id == cosechaId)
                .Select(x => x.CampoCosechaId).ToList();
            var camposAReenviar = this.repositorio.Listar<CampoProveedor>(x => x.CampoCosecha.Cosecha_Id == cosechaId && !archivosCamposAnalizados.Contains(x.CampoCosecha_Id));
            
            Log.Info($"Ejecucion Job ReenviarCamposACertificadorDeSustentables camposCosecha IDs: {string.Join(",", camposAReenviar.Select(c => c.CampoCosecha.Id))}");

            foreach (var campo in camposAReenviar)
            {
                try
                {
                    var rutaArchivo = this.repositorio.Obtener<Archivo>(a => a.Id == campo.Archivo_Id).Ruta;
                    EnviarCampoACertificadorDeSustentables(rutaArchivo, campo);
                }
                catch(Exception ex)
                {
                    Log.Error($"Error ejecucion ReenviarCamposACertificadorDeSustentables campoCosechaId {campo.CampoCosecha_Id}", ex);
                }
            }
        }

        private List<SP_CampoProveedorListadoDto> ListarCampos(Usuario usuario)
        {
            var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodosCamposSustentable);
            var esComercial = usuario.TienePermiso(PermisoEnum.ComercialCamposSustentables);
            if (esAdmin || esComercial)
            {

                return repositorio.SelStore<SP_CampoProveedorListadoDto>("sp_ListarCampoProveedor", 0, "", "");
            }
            else
            {
                var proveedoresIds = usuario.Proveedores.Select(pr => pr.Id.ToString());
                var proveedoresCuits = usuario.Proveedores.Select(pr => pr.CUIT);

                return repositorio.SelStore<SP_CampoProveedorListadoDto>("sp_ListarCampoProveedor", 0, string.Join(",", proveedoresIds), string.Join(",", proveedoresCuits));
            }
        }

        private void InformarCampoSustentable(CampoProveedor campoProveedor, string archivoKmz)
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
        private string ConvertirArchivo64(HttpPostedFileBase archivoKmz)
        {
            string theFileName = Path.GetFileName(archivoKmz.FileName);
            byte[] thePictureAsBytes = new byte[archivoKmz.ContentLength];
            using (BinaryReader theReader = new BinaryReader(archivoKmz.InputStream))
            {
                thePictureAsBytes = theReader.ReadBytes(archivoKmz.ContentLength);
            }
            return Convert.ToBase64String(thePictureAsBytes);
        }
        private string ObtenerNombreArchivoDrive(CampoProveedor campoProveedor)
        {
            return ObtenerNombreArchivoDrive(campoProveedor.CUIT, campoProveedor.CampoCosecha);
        }
        private string ObtenerNombreArchivoDrive(string cuit, CampoCosecha campoCosecha)
        {
            var cosecha = this.repositorio.Obtener<Cosecha>(c => c.Id == campoCosecha.Cosecha_Id);
            return $"{cuit}_{campoCosecha.Campo.Id}_{cosecha.Nombre}";
        }
        private void SubirArchivosAGoogleDrive(string rutaArchivo, CampoProveedor campoProveedor)
        {
            var extension = Path.GetExtension(rutaArchivo);
            var reporteACertificadorDto = repositorio.ObtenerReporteCertificador(
                campoProveedor.CampoCosecha_Id, campoProveedor.Proveedor_Id);

            var reporteCertificadorJson = JsonConvert.SerializeObject(reporteACertificadorDto);
            var jsonBytes = Encoding.UTF8.GetBytes(reporteCertificadorJson);

            var nombreArchivo = ObtenerNombreArchivoDrive(campoProveedor);
            var inputFolderId = ObtenerInputFolderSegunNormativa(campoProveedor);

            var uploadFileKMZ = new GoogleDriveFileUploadRequest()
                .WithFileUploadName($"{nombreArchivo}{extension}")
                .WithFilePath(rutaArchivo);

            campoSustentableGoogleDrive.UploadFile(uploadFileKMZ, inputFolderId);

            var uploadFileJSON = new GoogleDriveFileUploadRequest()
                .WithFileUploadName($"{nombreArchivo}.json")
                .WithMimeType("applications/json")
                .WithBytes(jsonBytes);

            campoSustentableGoogleDrive.UploadFile(uploadFileJSON, inputFolderId);
        }

        private string ObtenerInputFolderSegunNormativa(CampoProveedor campoProveedor)
        {
            var carpetaUcropIt = this.repositorio.Obtener<CarpetasUCROPIT>(c => c.Cosecha_Id == campoProveedor.CampoCosecha.Cosecha_Id
            && c.EPA == campoProveedor.EPA && c.EUDR == campoProveedor.EUDR && c.BSVS2 == campoProveedor.BSVS2);
            if (carpetaUcropIt == null)
            {
                throw new ValidationCustomException("No se encontró la configuración de carpeta para subir el archivo KMZ. Contacte al administrador.");
            }

            var match = System.Text.RegularExpressions.Regex.Match(carpetaUcropIt.UrlSubida, @"folders\/([a-zA-Z0-9\-_]+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        private bool ExisteCarpetaUcropIt(CampoProveedor campoProveedor)
        {
            return this.repositorio.Existe<CarpetasUCROPIT>(c => c.EUDR == campoProveedor.EUDR && c.EPA == campoProveedor.EPA &&
            c.BSVS2 == campoProveedor.BSVS2 && c.Cosecha_Id == campoProveedor.CampoCosecha.Cosecha_Id);
        }

        private void ActualizarPdf(byte[] pdfBytes, MemoryStream stream, DeclaracionCampoSustentableDto datos)
        {
            // open the reader
            PdfReader pdfReader = new PdfReader(pdfBytes);
            Rectangle size = pdfReader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // open the writer
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            document.Open();
            // the pdf content
            PdfContentByte cb = writer.DirectContent;

            // write the older pdf information in the pdf content
            PdfImportedPage page = writer.GetImportedPage(pdfReader, 1);
            cb.AddTemplate(page, 0, 0);
            float fontSizeNormal = 9.5f;
            BaseFont baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.EMBEDDED);
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);

            LimpiarFirmaAclaracionPrevios(cb);
            AddTextosPrimeraPagina(cb, baseFont, baseFontBold, fontSizeNormal);
            document.NewPage();

            float fontSize = 10f;
            float xMargenBase = -22.5f;
            float xMargenTexto = 15f;
            float xPosition = iTextSharp.text.PageSize.A4.Width / 10;
            float yPosition = iTextSharp.text.PageSize.A4.Height - ((iTextSharp.text.PageSize.A4.Height - 140f) / 5);

            AddTextosSegundaPagina(writer, baseFontBold, fontSizeNormal, fontSize, xPosition, xMargenBase, xMargenTexto, yPosition, datos.DirectivaDDJJCampoSustentable);

            var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "images", "header", "logo_.png");
            var logoImg = iTextSharp.text.Image.GetInstance(logoPath);
            logoImg.ScaleToFit(200f, 150f);
            document.Add(logoImg);

            AddTablaDatos(cb, datos, xPosition, xMargenTexto, yPosition);
            // close the streams and voilá the file should be changed :)
            document.Close();
            writer.Close();
            pdfReader.Close();
        }
        private void LimpiarFirmaAclaracionPrevios(PdfContentByte cb)
        {
            cb.SetColorFill(new CMYKColor(0f, 0f, 0f, 0f));

            cb.MoveTo(0, 220);
            cb.LineTo(600, 220);
            cb.LineTo(600, 300);
            cb.LineTo(0, 300);

            cb.Fill();

            cb.MoveTo(55, 340);
            cb.LineTo(500, 340);
            cb.LineTo(500, 355);
            cb.LineTo(55, 355);

            cb.Fill();
        }
        private void AddTextosPrimeraPagina(PdfContentByte cb, BaseFont baseFont, BaseFont baseFontBold, float fontSizeNormal)
        {
            cb.BeginText();
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(baseFontBold, fontSizeNormal);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Esquema de Certificación 2BSvs", 300, 740, 0);
            var baseTexto = 280f;
            cb.SetFontAndSize(baseFont, fontSizeNormal);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "RED III"
                , 55f, baseTexto + (11 * 6) + 0.75f, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Con esta declaración, el agricultor reconoce que los auditores de los organismos de certificación o de 2BSvs o de un Estado miembro"
                , 15f, baseTexto, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "pueden venir a verificar in situ si se han cumplido los requisitos pertinentes estipulados en la Directiva (UE) 2023/2413. Las pruebas de"
                , 15f, baseTexto - (11 * 1), 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "los requisitos   mencionados estarán disponibles y se facilitarán durante la auditoría y/o previa solicitud."
                , 15f, baseTexto - (11 * 2), 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "En caso de que se indique que no se cumplen los requisitos (por ejemplo, si los documentos no están disponibles o son incompletos), el "
                , 15f, baseTexto - (11 * 3), 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "agricultor se expone a que se rebaje la categoría de sus suministros."
                , 15f, baseTexto - (11 * 4), 0);

            cb.SetFontAndSize(baseFontBold, fontSizeNormal);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Firma: "
                , 15f, baseTexto - (11 * 14), 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Aclaración y DNI: "
                , iTextSharp.text.PageSize.A4.Width / 2, baseTexto - (11 * 14), 0);

            cb.EndText();

        }
        private void AddTextosSegundaPagina(PdfWriter writer, BaseFont baseFontBold, float fontSizeNormal, float fontSize, float xPosition, float xMargenBase, float xMargenTexto, float yPosition, string directiva)
        {

            PdfContentByte under = writer.DirectContentUnder;
            under.BeginText();
            under.SetColorFill(BaseColor.BLACK);
            under.SetFontAndSize(baseFontBold, fontSizeNormal);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Sres: Molinos Agro S.A.", xPosition + xMargenBase, yPosition, 0);
            under.SetFontAndSize(baseFontBold, fontSize);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Declaración de Conformidad según criterios de sustentabilidad para la producción de Biomasa, de acuerdo con los"
                , xPosition + xMargenTexto, yPosition - (15f * 2), 0);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "requisitos de la Directiva " + directiva
                , xPosition + xMargenTexto, yPosition - (15f * 3), 0);
            under.SetFontAndSize(baseFontBold, fontSizeNormal);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "De mi mayor consideración:"
                , xPosition + xMargenBase, yPosition - (15f * 5), 0);

            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Me dirijo a Uds. Para presentar la documentación requerida, para dar cumplimiento a la normativa"
                , xPosition + xMargenTexto, yPosition - (15f * 6), 0);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Internacional vigente (Reglamento EU 2023/1115), sus políticas y procesos internos."
                , xPosition + xMargenTexto, yPosition - (15f * 7), 0);


            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Para ello, acompañamos a la presente, la siguiente documentación, la cual se declara bajo"
                , xPosition + xMargenTexto, yPosition - (15f * 8), 0);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "juramento, que es fiel a la original y se encuentra plenamente vigente:"
                , xPosition + xMargenTexto, yPosition - (15f * 9), 0);


            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "1-  Declaración de sustentabilidad completa"
                , xPosition + xMargenTexto, yPosition - (15f * 11), 0);

            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "2-  Copia del Estatuto (última versión vigente)"
                , xPosition + xMargenTexto, yPosition - (15f * 12), 0);

            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "3-  Copia del poder de representación legal a nombre del firmante de la DDJJ"
                , xPosition + xMargenTexto, yPosition - (15f * 13), 0);

            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "4-  En caso de persona física solo adjuntar copia del DNI en lugar de los puntos 2 y 3"
                , xPosition + xMargenTexto, yPosition - (15f * 14), 0);

            under.EndText();

        }
        private void AddTablaDatos(PdfContentByte cb, DeclaracionCampoSustentableDto datos, float xPosition, float xMargenTexto, float yPosition)
        {

            PdfPTable informacionADeclararEnTabla = new PdfPTable(3);

            var fechaCell = new PdfPCell(new Phrase("Fecha"));
            fechaCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(fechaCell);
            var dateCell = new PdfPCell(new Phrase(DateTime.Now.ToString("d"))) { Colspan = 2 };
            informacionADeclararEnTabla.AddCell(dateCell);

            var razonSocialCell = new PdfPCell(new Phrase("Razón Social"));
            razonSocialCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(razonSocialCell);
            var razonSocialInfoCell = new PdfPCell(new Phrase(datos.RazonSocial)) { Colspan = 2 };
            informacionADeclararEnTabla.AddCell(razonSocialInfoCell);

            var cuitCell = new PdfPCell(new Phrase("CUIT"));
            cuitCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(cuitCell);
            var cuitInfoCell = new PdfPCell(new Phrase(datos.CUIT)) { Colspan = 2 };
            informacionADeclararEnTabla.AddCell(cuitInfoCell);

            var nombreApellidoCell = new PdfPCell(new Phrase("Nombre y Apellido"));
            nombreApellidoCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(nombreApellidoCell);
            var emptyCell = new PdfPCell(new Phrase("")) { Colspan = 2 };
            informacionADeclararEnTabla.AddCell(emptyCell);

            var dniCell = new PdfPCell(new Phrase("DNI"));
            dniCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(dniCell);
            informacionADeclararEnTabla.AddCell(emptyCell);

            var cargoCell = new PdfPCell(new Phrase("Cargo"));
            cargoCell.PaddingBottom = 15f;
            informacionADeclararEnTabla.AddCell(cargoCell);
            informacionADeclararEnTabla.AddCell(emptyCell);

            var firmaCell = new PdfPCell(new Phrase("Firma"));
            firmaCell.PaddingBottom = 60f;
            informacionADeclararEnTabla.AddCell(firmaCell);
            informacionADeclararEnTabla.AddCell(emptyCell);
            informacionADeclararEnTabla.TotalWidth = 400f;
            informacionADeclararEnTabla.WriteSelectedRows(0, -1, xPosition + (xMargenTexto * 3), yPosition - (15f * 16), cb);

        }

        private DeclaracionCampoSustentable ObtenerDeclaracion(int cosechaId, string CUIT)
        {
            return repositorio.Obtener<DeclaracionCampoSustentable>(d => d.Cosecha_Id == cosechaId && d.CUIT == CUIT);
        }

        private byte[] ReemplazarTextoEnPdf(byte[] pdfBytes, string textoOriginal, string textoNuevo)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                var pdfReader = new PdfReader(pdfBytes);
                var pdfStamper = new PdfStamper(pdfReader, outputStream);

                for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                {
                    PdfDictionary pageDict = pdfReader.GetPageN(i);
                    PdfObject contentObject = pageDict.GetDirectObject(PdfName.CONTENTS);

                    if (contentObject is PRStream contentStream)
                    {
                        var contentBytes = PdfReader.GetStreamBytes(contentStream);
                        var content = Encoding.Default.GetString(contentBytes);

                        if (content.Contains(textoOriginal))
                        {
                            content = content.Replace(textoOriginal, textoNuevo);
                            contentStream.SetData(Encoding.Default.GetBytes(content));
                        }
                    }
                }

                pdfStamper.Close();
                pdfReader.Close();

                return outputStream.ToArray();
            }
        }

        public List<TipoNormativa> ObtenerNormativas()
        {
            return repositorio.Listar<TipoNormativa>();
        }

        public string ObtenerRutaArchivoEPA(int campoCosechaId, int proveedorId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(x => x.Proveedor_Id == proveedorId && x.CampoCosecha_Id == campoCosechaId);
            return campoProveedor.EvidenciaEPA.Ruta;
        }

        public string Rechazar(string mailUsuario, int campoCosechaId, int proveedorId, int tipoNormativaId, string motivoRechazo)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            var campoCosechaNormativa = campoProveedor.CampoCosecha.CampoCosechaNormativas.FirstOrDefault(c => c.TipoNormativa_Id == tipoNormativaId);

            if (campoCosechaNormativa == null)
            {
                throw new ValidationCustomException("No existe el campo seleccionado.");
            }

            campoCosechaNormativa.MotivoRechazo = motivoRechazo;

            repositorio.GuardarCambios();

            return SuccessMsg.CampoSustentableRechazado;
        }

        public string Aprobar(string mailUsuario, int campoCosechaId, int proveedorId, int tipoNormativaId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            var campoCosechaNormativa = campoProveedor.CampoCosecha.CampoCosechaNormativas.FirstOrDefault(c => c.TipoNormativa_Id == tipoNormativaId);

            if (campoCosechaNormativa == null)
            {
                throw new ValidationCustomException("No existe el campo seleccionado.");
            }

            campoCosechaNormativa.Validado = true;
            campoCosechaNormativa.ValidadoPor = usuario.Id;
            campoCosechaNormativa.ValidadoFecha = DateTime.Now;

            repositorio.GuardarCambios();

            return SuccessMsg.CampoSustentableAprobado;
        }

        private void ValidarCampoPoligonoKmz(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            if (Path.GetExtension(archivoKmz.FileName).ToLower() != ".kmz")
            {
                throw new ValidationCustomException("El archivo debe tener formato KMZ.");
            }

            // Validar que no sea solo un punto
            if (archivoKmz.ContentLength == 0)
            {
                throw new ValidationCustomException("El archivo KMZ está vacío.");
            }

            using (var memoryStream = new MemoryStream())
            {
                archivoKmz.InputStream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                if (!ContienePoligonoEnKmz(memoryStream))
                    throw new ValidationCustomException("El archivo KMZ debe contener al menos un polígono.");
            }

            // Volver a dejar el InputStream original en el inicio
            archivoKmz.InputStream.Position = 0;
        }

        private bool ContienePoligonoEnKmz(Stream kmzStream)
        {
            using (var zip = new ZipArchive(kmzStream, ZipArchiveMode.Read, true))
            {
                var kmlEntry = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase));
                if (kmlEntry == null)
                    return false;

                using (var kmlStream = kmlEntry.Open())
                {
                    var parser = new Parser();
                    parser.Parse(kmlStream);
                    var kml = parser.Root as SharpKml.Dom.Kml;
                    if (kml == null)
                        return false;

                    return BuscarPoligonoEnKml(kml);
                }
            }
        }

        private bool BuscarPoligonoEnKml(SharpKml.Dom.Kml kml)
        {
            // Recorrer todos los elementos y buscar al menos un Polígono
            foreach (var placemark in kml.Flatten().OfType<SharpKml.Dom.Placemark>())
            {
                if (placemark.Geometry is SharpKml.Dom.Polygon)
                    return true;
                // También puede haber MultiGeometry con polígonos
                if (placemark.Geometry is SharpKml.Dom.MultipleGeometry multi)
                {
                    if (multi.Geometry.Any(g => g is SharpKml.Dom.Polygon))
                        return true;
                }
            }
            return false;
        }

        public string AdjuntarEPAValidado(string mailUsuario, int campoCosechaId, int proveedorId, HttpPostedFileBase archivoEPA)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            campoProveedor.EvidenciaEPA = (new Archivo { FileKey = FileKeys.ArchivoEPA, Ruta = "" });
            campoProveedor.EvidenciaEPA.Ruta = GuardarArchivoEPA(campoProveedor, archivoEPA);
            repositorio.GuardarCambios();

            return SuccessMsg.CampoSustentableActualizado;
        }

        public Archivo ObtenerEPAExistenteCampo(int proveedorId, string cuit, HttpPostedFileBase archivoKmz)
        {
            // Leer el archivo KMZ subido a memoria para reutilizarlo varias veces
            byte[] kmzBytes;
            using (var memoryStream = new MemoryStream())
            {
                archivoKmz.InputStream.CopyTo(memoryStream);
                kmzBytes = memoryStream.ToArray();
            }

            //Campos que ya hayan presentado evidencia para ser reutilizada.
            var campos = repositorio.Listar<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CUIT == cuit && cp.EvidenciaEPA != null);

            foreach (var campo in campos)
            {
                var rutaKmzExistente = campo.Archivo.Ruta;
                if (!string.IsNullOrEmpty(rutaKmzExistente) && File.Exists(rutaKmzExistente))
                {
                    using (var streamExistente = File.OpenRead(rutaKmzExistente))
                    using (var kmzStream = new MemoryStream(kmzBytes)) // nuevo stream limpio cada vez
                    {
                        if (SonKmzIgualesPorPoligono(streamExistente, kmzStream))
                        {
                            return campo.EvidenciaEPA;
                        }
                    }
                }
            }

            return null;
        }

        private bool SonKmzIgualesPorPoligono(Stream kmzStream1, Stream kmzStream2)
        {
            // Reiniciar los streams antes de leerlos
            if (kmzStream1.CanSeek) kmzStream1.Position = 0;
            if (kmzStream2.CanSeek) kmzStream2.Position = 0;

            var coords1 = ExtraerCoordenadasPoligonoKmz(kmzStream1);
            var coords2 = ExtraerCoordenadasPoligonoKmz(kmzStream2);

            if (coords1 == null || coords2 == null)
                return false;

            if (coords1.Count != coords2.Count)
                return false;

            for (int i = 0; i < coords1.Count; i++)
            {
                if (!coords1[i].Equals(coords2[i]))
                    return false;
            }

            return true;
        }

        private List<SharpKml.Base.Vector> ExtraerCoordenadasPoligonoKmz(Stream kmzStream)
        {
            if (kmzStream.CanSeek)
                kmzStream.Position = 0;

            using (var zip = new System.IO.Compression.ZipArchive(kmzStream, System.IO.Compression.ZipArchiveMode.Read, leaveOpen: false))
            {
                var kmlEntry = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase));
                if (kmlEntry == null)
                    return null;

                using (var kmlStream = kmlEntry.Open())
                {
                    var parser = new Parser();
                    parser.Parse(kmlStream);
                    var kml = parser.Root as SharpKml.Dom.Kml;
                    if (kml == null)
                        return null;

                    var poligono = kml.Flatten()
                        .OfType<SharpKml.Dom.Placemark>()
                        .Select(p => p.Geometry)
                        .OfType<SharpKml.Dom.Polygon>()
                        .FirstOrDefault();

                    if (poligono == null)
                        return null;

                    return poligono.OuterBoundary?.LinearRing?.Coordinates?.ToList();
                }
            }
        }

        public CampoProveedor ObtenerCampoSuperposicion(CampoProveedor cp, string rutaKmz)
        {
            var campos = this.repositorio.Listar<CampoProveedor>(c => c.CUIT == cp.CUIT
            && c.Proveedor_Id == cp.Proveedor_Id && c.EPA == cp.EPA && c.BSVS2 == cp.BSVS2 &&
            c.EUDR == cp.EUDR && c.CampoCosecha_Id != cp.CampoCosecha_Id);

            if (campos == null || !campos.Any())
                return null;

            Polygon polygonKmzNuevo = CargarPoligonoDesdeKMZ(rutaKmz);

            foreach (var campo in campos)
            {
                Polygon polygonKmzExistente = CargarPoligonoDesdeKMZ(campo.Archivo.Ruta);
                var porcentajeSuperposicion = CalcularSuperposicion(polygonKmzNuevo, polygonKmzExistente);
                if (porcentajeSuperposicion > 70)
                {
                    return campo;
                }
            }

            return null;
        }

        public static double CalcularSuperposicion(Polygon polygon1, Polygon polygon2)
        {
            // Revisar si son idénticos
            if (polygon1.EqualsExact(polygon2))
            {
                Console.WriteLine("Los polígonos son idénticos.");
                return 100.0;
            }

            // Calcular la intersección
            var interseccion = polygon1.Intersection(polygon2);

            // Calcular el porcentaje de superposición
            double areaInterseccion = interseccion.Area;
            double areaUnion = polygon1.Union(polygon2).Area;

            double porcentajeSuperposicion = (areaInterseccion / areaUnion) * 100.0;
            //Console.WriteLine($"Porcentaje de superposición: {porcentajeSuperposicion}%");
            return porcentajeSuperposicion;
        }

        private static Polygon CargarPoligonoDesdeKMZ(string kmzPath)
        {
            // Extraer el archivo KML desde el KMZ
            string kmlPath = ExtraerKmlDesdeKmz(kmzPath);
            // Leer y procesar el archivo KML para obtener un polígono
            return LeerPoligonoDesdeKml(kmlPath);
        }

        private static string ExtraerKmlDesdeKmz(string kmzPath)
        {
            // Si es un archivo .kml directo, devolverlo
            if (Path.GetExtension(kmzPath).Equals(".kml", StringComparison.OrdinalIgnoreCase))
                return kmzPath;

            if (!File.Exists(kmzPath))
                throw new FileNotFoundException($"El archivo {kmzPath} no existe.");

            var fileInfo = new FileInfo(kmzPath);
            if (fileInfo.Length == 0)
                throw new InvalidDataException($"El archivo {kmzPath} está vacío o dañado.");

            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                using (var zip = ZipFile.OpenRead(kmzPath))
                {
                    foreach (var entry in zip.Entries)
                    {
                        // Ignorar directorios
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        if (entry.FullName.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                        {
                            string kmlPath = Path.Combine(tempDir, Path.GetFileName(entry.FullName));
                            entry.ExtractToFile(kmlPath, true);
                            return kmlPath;
                        }
                    }
                }

                throw new FileNotFoundException("No se encontró un archivo .kml dentro del KMZ.");
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidOperationException("El archivo KMZ está corrupto o no es un ZIP válido.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al extraer el KML desde el KMZ.", ex);
            }
        }


        private static Polygon LeerPoligonoDesdeKml(string kmlPath)
        {
            var doc = XDocument.Load(kmlPath);
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            var coordinatesElement = doc.Descendants(ns + "coordinates").FirstOrDefault();
            if (coordinatesElement == null)
            {
                throw new InvalidOperationException("No se encontraron coordenadas en el archivo KML.");
            }

            string coordinatesText = coordinatesElement.Value.Trim();
            var coordinates = ParseCoordinates(coordinatesText);

            var geometryFactory = new GeometryFactory();
            var linearRing = geometryFactory.CreateLinearRing(coordinates);
            return geometryFactory.CreatePolygon(linearRing);
        }

        private static Coordinate[] ParseCoordinates(string coordinatesText)
        {
            var coordinateStrings = coordinatesText.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var coordinates = new Coordinate[coordinateStrings.Length + 1];

            for (int i = 0; i < coordinateStrings.Length; i++)
            {
                var parts = coordinateStrings[i].Split(',');
                if (parts.Length < 2)
                {
                    throw new FormatException("Formato de coordenadas inválido.");
                }

                double lon = double.Parse(parts[0], CultureInfo.InvariantCulture);
                double lat = double.Parse(parts[1], CultureInfo.InvariantCulture);

                coordinates[i] = new Coordinate(lon, lat);
            }

            // Aseguramos que el último punto sea igual al primero
            coordinates[coordinates.Length - 1] = coordinates[0];

            return coordinates;
        }



    }


}


