using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Export.CampoSustentable;
using SustitucionMOAUtils.Extensions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
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
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class CampoSustentableService : ICampoSustentableService
    {
        private readonly IRepositorioCampoSustentable repositorio;
        private readonly string DataAgroURL;
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
            this.DataAgroURL = ConfigurationManager.AppSettings["DataAgroURL"];
            this.excelExport = excelExport;
            this.dataAgroService = dataAgroService;
            this.campoSustentableGoogleDrive = campoSustentableGoogleDrive;
            this.campoSustentablePdfGenerator = campoSustentablePdfGenerator;
        }

        public Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz, bool UsarArchivoId, HttpPostedFileBase archivoEPA)
        {
            var ruta = "";
            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            SustentableRenspaExisteDto renspaExisteDto =
                RenspaExiste(campoProveedor.CampoCosecha.Campo.Renspa,
                             campoProveedor.CUIT,
                             campoProveedor.CampoCosecha.Cosecha_Id,
                             out CampoCosecha campoCosechaExistente);
            if (renspaExisteDto.RenspaExiste)
            {
                if (renspaExisteDto.MismoCuit)
                {
                    throw new ValidationCustomException("Este campo ya fue presentado.");
                }
                Proveedor proveedor = repositorio.Obtener<Proveedor>(p => p.CUIT.Equals(campoProveedor.CUIT, StringComparison.OrdinalIgnoreCase));
                campoCosechaExistente.Proveedores.Add(proveedor);
                repositorio.GuardarCambios();
                return new Resultado { IdEntidad = campoCosechaExistente.Id, Mensaje = SuccessMsg.CampoSustentableAgregado };
            }

            ValidarCampo(campoProveedor, archivoKmz);

            var declaracion = repositorio.ObtenerDeclaracionDeProveedor(campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id);

            if(campoProveedor.BSVS2 && declaracion != null)
            {
                campoProveedor.RazonSocial = declaracion.RazonSocial;
            }

            campoProveedor.FechaCreacion = DateTime.Now;
            campoProveedor.Borrado = false;
            
            if (campoProveedor.Archivo_Id == 0)
            {
                campoProveedor.Archivo = (new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" });
            }
            else
            {
                var archivoCampo = repositorio.ObtenerArchivo(campoProveedor.Archivo_Id);
                ruta = archivoCampo.Ruta;
            }

            if (campoProveedor.EPA && archivoEPA != null)
            {
                campoProveedor.EvidenciaEPA = (new Archivo { FileKey = FileKeys.ArchivoEPA, Ruta = "" });
                campoProveedor.EvidenciaEPA.Ruta = GuardarArchivoCampoSustentable(campoProveedor, archivoEPA, rutaArch => campoProveedor.EvidenciaEPA.Ruta = rutaArch);
            }

            //TODO linea 113 borrar dps
            campoProveedor.CampoCosecha.ToneladasAprobadas = -1;

            campoProveedor.CampoCosecha.Campo.IdScato = ObtenerIdScato(campoProveedor);
            campoProveedor.CampoCosecha.Cosecha = repositorio.Obtener<Cosecha>(campoProveedor.CampoCosecha.Cosecha_Id);

            repositorio.Agregar(campoProveedor);

            this.AgregarNormativas(campoProveedor);

            repositorio.GuardarCambios();
            if (!UsarArchivoId)
            {
                ruta = GuardarArchivoCampoSustentable(campoProveedor, archivoKmz, rutaArch => campoProveedor.Archivo.Ruta = rutaArch);

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
                normativas.Add("BSVS2");

            if (campoProveedor.EPA)
                normativas.Add("EPA");

            if (campoProveedor.EUDER)
                normativas.Add("EUDER");

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

            campoProveedor.HectareasSoja = campoProveedorObj.HectareasSoja;
            campoProveedor.HectareasTotales = campoProveedorObj.HectareasTotales;
            campoProveedor.Longitud = campoProveedorObj.Longitud;
            campoProveedor.Latitud = campoProveedorObj.Latitud;
            campoProveedor.CampoCosecha.ToneladasAprobadas = campoProveedorObj.CampoCosecha.ToneladasAprobadas;
            campoProveedor.CampoCosecha.Campo.Nombre = campoProveedorObj.CampoCosecha.Campo.Nombre;
            campoProveedor.CampoCosecha.Campo.Renspa = campoProveedorObj.CampoCosecha.Campo.Renspa;
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
                    HectareasSoja = c.HectareasSojaUcropit != null ? c.HectareasSojaUcropit.ToString() : c.HectareasSoja.ToString(),
                    HectareasTotales = c.HectareasTotalesUcropit != null ? c.HectareasTotalesUcropit.ToString() : c.HectareasTotales.ToString(),
                    Localidad = c.CampoCosecha.Campo.Localidad.Nombre,
                    Nombre = c.CampoCosecha.Campo.Nombre,
                    Renspa = c.CampoCosecha.Campo.Renspa,
                    Pais = "Argentina",
                    Provincia = c.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    Coordenadas = string.Concat(c.Latitud, " ", c.Longitud),
                    ToneladasAprobadas = c.CampoCosecha.ToneladasAprobadas.ToString(),
                    Partido = c.CampoCosecha.Campo.Localidad.Partido.Descripcion
                },
                cp => cp.CUIT == CUIT && cp.CampoCosecha.Cosecha_Id == cosecha.Id && cp.CampoCosecha.ToneladasAprobadas != -1);

            var declaracion = ObtenerDeclaracion(cosechaId, CUIT);

            var datosDeclaracionJurada = new DeclaracionCampoSustentableDto
            {
                Cosecha = cosecha.Nombre,
                CUIT = declaracion.CUIT,
                RazonSocial = declaracion.RazonSocial,
                Fecha = declaracion.FechaFirma?.ToString("dd/MM/yyyy"),
                CantidadParteSoja = declaracion.HectareasDeclaradas.Value,
                Campos = allCampos
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
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad
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
                Campos = null
            };

            var pdfBytes = GenerarPDFDeclaracion(datos);
            pdfBytes = ReemplazarTextoEnPdf(pdfBytes, @"2018/2001/EC \(RED II\)", @"2023/2413/EC \(RED III\)");

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

            var campos = ListarCampos(usuario, cp => cp); // Trae los CampoProveedor completos

            var resultado = new List<CampoProveedorListadoDto>();

            foreach (var cp in campos)
            {
                var normativas = new List<(bool flag, string descripcion)>
                {
                    (cp.BSVS2, "BSVS2"),
                    (cp.EPA, "EPA"),
                    (cp.EUDER, "EUDER")
                };

                foreach (var (flag, descripcion) in normativas.Where(n => n.flag))
                {
                    // Busca la normativa correspondiente
                    var normativa = cp.CampoCosecha.CampoCosechaNormativas?
                        .FirstOrDefault(n => n.TipoNormativa.Descripcion == descripcion);

                    if (normativa != null)
                    {
                        resultado.Add(new CampoProveedorListadoDto
                        {
                            IdScato = cp.CampoCosecha.Campo.IdScato,
                            NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                            HectareasSoja = cp.HectareasSoja,
                            HectareasTotales = cp.HectareasTotales,
                            NombreCampo = cp.CampoCosecha.Campo.Nombre,
                            ToneladasAprobadas = normativa.ToneladasAprobadas,
                            CampoCosechaId = cp.CampoCosecha_Id,
                            Proveedor = new ProveedorDto
                            {
                                Id = cp.Proveedor.Id,
                                CodigoProveedor = cp.Proveedor.CodigoProveedor,
                                RazonSocial = cp.Proveedor.RazonSocial
                            },
                            CodigoProveedor = cp.Proveedor.CodigoProveedor,
                            CUITProveedor = cp.CUIT,
                            RazonSocialProveedor = cp.RazonSocial,
                            CosechaId = cp.CampoCosecha.Cosecha_Id,
                            MotivoRechazo = normativa.MotivoRechazo,
                            FechaCreacion = cp.FechaCreacion,
                            TipoNormativa = normativa.TipoNormativa.Descripcion,
                            Validado = normativa.Validado,
                            ValidadoPor = normativa.ValidadoPor,
                        });
                    }
                }
            }

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
            var headersBase = new List<string>() { "Cosecha", "Campo", "RENSPA", "Proveedor", "Cuit", "Estado", "Motivo", "Ha Totales", "Ha Soja", "Toneladas Aprobadas", "Razon Social", "Fecha Creacion" };
            dynamic listado;

            if (esAdminCampos)
            {
                headersBase.Insert(0, "Id Scato");
                listado = ListarCampos(usuario, cp => new CampoSustentableExportDTO()
                {
                    Campo = cp.CampoCosecha.Campo.Nombre,
                    Renspa = cp.CampoCosecha.Campo.Renspa,
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
                ((List<CampoSustentableExportDTO>)listado).ForEach(x => x.Renspa = x.Renspa.ToFormatoRenspa());
            }
            else
            {
                listado = ListarCampos(usuario, cp => new CampoSustentableExportBaseDTO()
                {
                    Campo = cp.CampoCosecha.Campo.Nombre,
                    Renspa = cp.CampoCosecha.Campo.Renspa,
                    ProveedorRazonSocial = cp.Proveedor.CodigoProveedor,
                    Cosecha = cp.CampoCosecha.Cosecha.Nombre,
                    HectareasSoja = cp.HectareasSoja,
                    HectareasTotales = cp.HectareasTotales,
                    Motivo = cp.CampoCosecha.MotivoRechazo,
                    ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas,
                    RazonSocial = cp.RazonSocial,
                    FechaCreacion = cp.FechaCreacion
                });
                ((List<CampoSustentableExportBaseDTO>)listado).ForEach(x => x.Renspa = x.Renspa.ToFormatoRenspa());
            }

            return excelExport.ToExcel(listado, headersBase.ToArray(), "Reporte Campos Sustentables");
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
            archivoSinDescargar.CampoCosecha.ToneladasAprobadas = resultadoProcesadoUcropit.Bsvs2 != null ?
                    Math.Round(resultadoProcesadoUcropit.Bsvs2.ToneladasAprobadas ?? 0, 2) : 0;
            archivoSinDescargar.CampoCosecha.MotivoRechazo = resultadoProcesadoUcropit.MotivoRechazo;
            campoProveedor.HectareasSojaUcropit = resultadoProcesadoUcropit.Bsvs2?.SuperficieElegible;
            campoProveedor.HectareasTotalesUcropit = resultadoProcesadoUcropit.Bsvs2?.SuperficieTotalCampo;

            repositorio.GuardarCambios();
        }

        public SustentableRenspaExisteDto RenspaExiste(string renspa, string cuit, int cosechaId, out CampoCosecha campoCosecha)
        {
            SustentableRenspaExisteDto result = new SustentableRenspaExisteDto();

            campoCosecha = repositorio.Obtener<CampoCosecha>(
                new List<Expression<Func<CampoCosecha, object>>> { c => c.CamposProveedor },
                c => c.Campo.Renspa == renspa && c.Cosecha_Id == cosechaId);

            if (campoCosecha == null || campoCosecha.CamposProveedor == null) { return result; }

            result.RenspaExiste = campoCosecha.CamposProveedor.Any(cp => !cp.Borrado);

            if (campoCosecha.CamposProveedor.Any(campoProv => campoProv.CUIT.Equals(cuit, StringComparison.OrdinalIgnoreCase)))
            {
                result.MismoCuit = true;
            }

            return result;
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

        public void AgregarCamposSugeridos(List<SugerenciaCampoDto> camposSugeridosDto, List<HttpPostedFileBase> archivosKmz, string mailUsuario)
        {
            var usuario = repositorio.ObtenerUsuarioPorMail(mailUsuario);
            foreach (var proveedorId in camposSugeridosDto.Select(x => x.Proveedor_Id).Distinct())
            {
                ValidarUsuario(usuario, proveedorId);
            }

            foreach (var campoSugeridoDto in camposSugeridosDto)
            {
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
                        ToneladasAprobadas = -1,
                        Campo = new CampoSustentable
                        {
                            Nombre = campoSugeridoDto.NombreCampo,
                            Localidad_Id = campoSugeridoDto.Localidad_Id,
                            Renspa = campoSugeridoDto.Renspa
                        }
                    },
                    Archivo_Id = campoSugeridoDto.Archivo_Id,
                    FechaCreacion = DateTime.Now,
                    Borrado = false
                };

                var renspaExisteDto = RenspaExiste(campoProveedor.CampoCosecha.Campo.Renspa, campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id, out CampoCosecha campoCosechaExistente);
                if (renspaExisteDto.RenspaExiste)
                {
                    if (!renspaExisteDto.MismoCuit)
                    {
                        Log.Info($"Se agrega para la cosecha id {campoProveedor.CampoCosecha.Cosecha_Id} el campo sugerido con renspa {campoProveedor.CampoCosecha.Campo.Renspa} al proveedor CUIT {campoProveedor.CUIT}");
                        var proveedor = repositorio.Obtener<Proveedor>(p => p.CUIT == campoProveedor.CUIT);
                        campoCosechaExistente.Proveedores.Add(proveedor);
                        repositorio.GuardarCambios();
                    }
                    else
                    {
                        Log.Info($"No se guarda el campo sugerido con renspa {campoProveedor.CampoCosecha.Campo.Renspa} porque ya existe para el mismo CUIT");
                    }
                }
                else
                {
                    Log.Info($"Se guarda para la cosecha id {campoProveedor.CampoCosecha.Cosecha_Id} el campo sugerido con renspa {campoProveedor.CampoCosecha.Campo.Renspa}");

                    var archivoNuevoKmz = !string.IsNullOrEmpty(campoSugeridoDto.NombreNuevoKmz) ? archivosKmz.FirstOrDefault(x => x.FileName == campoSugeridoDto.NombreNuevoKmz) : null;
                    ValidarCampo(campoProveedor, archivoNuevoKmz);

                    var declaracion = repositorio.ObtenerDeclaracionDeProveedor(campoProveedor.CUIT, campoProveedor.CampoCosecha.Cosecha_Id);

                    campoProveedor.RazonSocial = declaracion.RazonSocial;
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
                            rutaArchivo = GuardarArchivoCampoSustentable(campoProveedor, archivoNuevoKmz, rutaArch => campoProveedor.Archivo.Ruta = rutaArch);
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

            Log.Info($"CampoSustentableService, GenerarPDFDeclaracion, {content}");

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


        private string GuardarArchivoCampoSustentable(CampoProveedor campoProveedor, HttpPostedFileBase archivo, Action<string> setRutaArch)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, ".", extension);
            var rutaCarpeta = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campoProveedor.CUIT);
            var rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }
            archivo.SaveAs(rutaArchivo);

            setRutaArch(rutaArchivo);
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
            return $"{cuit}_{campoCosecha.Campo.Id}_{campoCosecha.Cosecha.Nombre}";
        }
        private void SubirArchivosAGoogleDrive(string rutaArchivo, CampoProveedor campoProveedor)
        {
            var extension = Path.GetExtension(rutaArchivo);
            var reporteACertificadorDto = repositorio.ObtenerReporteCertificador(
                campoProveedor.CampoCosecha_Id, campoProveedor.Proveedor_Id);

            var reporteCertificadorJson = JsonConvert.SerializeObject(reporteACertificadorDto);
            var jsonBytes = Encoding.UTF8.GetBytes(reporteCertificadorJson);

            var nombreArchivo = ObtenerNombreArchivoDrive(campoProveedor);

            var uploadFileKMZ = new GoogleDriveFileUploadRequest()
                .WithFileUploadName($"{nombreArchivo}{extension}")
                .WithFilePath(rutaArchivo);

            campoSustentableGoogleDrive.UploadFile(uploadFileKMZ);

            var uploadFileJSON = new GoogleDriveFileUploadRequest()
                .WithFileUploadName($"{nombreArchivo}.json")
                .WithMimeType("applications/json")
                .WithBytes(jsonBytes);

            campoSustentableGoogleDrive.UploadFile(uploadFileJSON);
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

            AddTextosSegundaPagina(writer, baseFontBold, fontSizeNormal, fontSize, xPosition, xMargenBase, xMargenTexto, yPosition);

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
        private void AddTextosSegundaPagina(PdfWriter writer, BaseFont baseFontBold, float fontSizeNormal, float fontSize, float xPosition, float xMargenBase, float xMargenTexto, float yPosition)
        {

            PdfContentByte under = writer.DirectContentUnder;
            under.BeginText();
            under.SetColorFill(BaseColor.BLACK);
            under.SetFontAndSize(baseFontBold, fontSizeNormal);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Sres: Molinos Agro S.A.", xPosition + xMargenBase, yPosition, 0);
            under.SetFontAndSize(baseFontBold, fontSize);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Declaración de Conformidad según criterios de sustentabilidad para la producción de Biomasa, de acuerdo con los"
                , xPosition + xMargenTexto, yPosition - (15f * 2), 0);
            under.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "requisitos de la Directiva 2023/2413/EC (RED III)"
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
    }
}
