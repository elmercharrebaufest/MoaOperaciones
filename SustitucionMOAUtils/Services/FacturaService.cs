using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.Factura;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services.AnalisisDocumentoServiceValidation;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Usuario = SustitucionMOAModel.Entities.Usuario;

namespace SustitucionMOAUtils.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IAzureService azureService;
        private readonly IAnalisisDocumentoService analisisDocumentoService;
        private readonly IRepositorio repositorio;
        private readonly IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA;
        private readonly IEmailService emailService;
        private readonly string EmailFacturasES = ConfigurationManager.AppSettings["EmailFacturasES"];


        public FacturaService(IAzureService azureService, IAnalisisDocumentoService analisisDocumentoService, IRepositorio repositorio,
            IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA, IEmailService emailService)
        {
            this.azureService = azureService ?? throw new ArgumentNullException(nameof(azureService));
            this.analisisDocumentoService = analisisDocumentoService ?? throw new ArgumentNullException(nameof(analisisDocumentoService));
            this.repositorio = repositorio;
            this.obtenerOrdenDeCompraConsumerMOA = obtenerOrdenDeCompraConsumerMOA;
            this.emailService = emailService;
        }

        public List<ValidationResult> SubirPDF(List<HttpPostedFileBase> files, string cuit, string codigo, string mail)
        {
            Log.Info("files: " + files.Count);
            if (files == null || !files.Any())
                throw new ValidationCustomException("No se adjuntó ningun archivo.");

            List<ValidationResult> results = new List<ValidationResult>();
            List<ValidationResult> resultadoOcrs = new List<ValidationResult>();
            foreach (var file in files)
            {
                Log.Info("AnalizarImagenAsync: " + file.FileName);
                var operacionOCRId = Task.Run(async () => await azureService.AnalizarImagenAsync(file)).Result;
                Thread.Sleep(2000);
                Log.Info("ObtenerResultadoOCRAsync: " + file.FileName);
                var elementosLeidos = Task.Run(async () => await azureService.ObtenerResultadoOCRAsync(operacionOCRId)).Result;
                resultadoOcrs.AddRange(elementosLeidos.Select(a => new ValidationResult { Input = a, FileName = file.FileName }));
                Log.Info("Fin ObtenerResultadoOCRAsync: " + file.FileName);
            }

            int usuarioId = repositorio.Obtener<Usuario>(u => u.Mail == mail).Id;
            foreach (var file in files)
            {
                try
                {
                    Log.Info("Procesando el documento " + file.FileName);
                    List<string> elementosLeidos = resultadoOcrs.Where(a => a.FileName == file.FileName).Select(a => a.Input).ToList();
                    List<ValidationResult> resultadoAnalisis = analisisDocumentoService.AnalizarFacturaCertificacionServicios(elementosLeidos, cuit, file.FileName);
                    List<ValidationResult> resultado = AnalizarResultados(resultadoAnalisis, codigo);
                    // Flujo nuevo
                    if (resultado[0].IsValid)
                    {
                        var ordenDeCompraValidationResult = resultadoAnalisis.Find(a => a.IsValid && a.ValidataionType == typeof(OrdenCompraValidationCommand).Name);
                        var certificacionesRegistradasOC = repositorio.Listar<CertificacionRegistrada>(cr => cr.NRO_OC == ordenDeCompraValidationResult.Value);

                        if (resultado[0].Certificaciones?.Count > 0)
                        {
                            resultado.ForEach(r => r.FileName = file.FileName);
                            // Buscamos los archivos relacionados a ese nro de certificacion
                            resultado[0].Certificaciones.ForEach((certificacion) =>
                            {
                                var certificacionesRegistradas = certificacionesRegistradasOC.Where(c => c.NRO_Certificacion == certificacion.NroCertificacion).ToList();
                                if (certificacionesRegistradas != null && certificacionesRegistradas.Any())
                                {
                                    certificacionesRegistradas.ForEach(certificacionRegistrada =>
                                    {
                                        certificacion.Archivo.Add(certificacionRegistrada.Archivo);
                                    });
                                }
                            });
                        }

                        resultado[0].Certificaciones = resultado[0].Certificaciones ?? new List<SustitucionMOAModel.Dto.OrdenDeCompraSAPCertificacion>();
                        if (certificacionesRegistradasOC.Any(x => x.Archivo.FileKey == FileKeys.FacturaDiferenciaTasaDeCambio))
                        {
                            resultado[0].Certificaciones.Add(new SustitucionMOAModel.Dto.OrdenDeCompraSAPCertificacion
                            {
                                NroCertificacion = "Factura por diferencia de tasa de cambio",
                                Archivo = new List<Archivo>(certificacionesRegistradasOC.Where(cr => cr.Archivo.FileKey == FileKeys.FacturaDiferenciaTasaDeCambio).Select(x => x.Archivo))
                            });
                        }
                    }
                    // Flujo anterior a registro de certificaciones
                    else
                    {
                        string ruta = GenerarRutaArchivo(ConfigurationManager.AppSettings["FolderFacturasES"], usuarioId, file.FileName);
                        file.SaveAs(ruta);
                        Archivo archivo = new Archivo { Ruta = ruta, FileKey = FileKeys.FacturaEntradaDeServicios };
                        repositorio.Agregar(archivo);
                        repositorio.GuardarCambios();
                        resultado.ForEach(r => r.FileName = file.FileName);
                        resultadoAnalisis.ForEach(r => r.Archivo_Id = archivo.Id);
                        if (resultado.Exists(r => r.IsValid))
                        {
                            EnviarMail(file);
                        }

                        GuardarResultadosYArchivo(elementosLeidos, resultadoAnalisis, ruta, usuarioId);
                    }

                    results.AddRange(resultado);
                }
                catch (Exception e)
                {
                    var error = new ValidationResult(false, "El documento no se envió a para su análisis.", "OCR", "", "");
                    error.FileName = file.FileName;
                    results.Add(error);
                    Log.Error("Error al procesar el documento " + file.FileName, e);
                }
            }
            return results;
        }

        public List<CertificacionRegistrada> RegistrarCertificaciones(List<GrupoCertificaciones> gruposCertificaciones, string mailUsuario, int proveedorId, List<HttpPostedFileBase> archivos, string cuit, string codigoProveedor)
        {
            var certificacionesRegistradas = new List<CertificacionRegistrada>();
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            foreach (var grupo in gruposCertificaciones)
            {
                var archivo = archivos.First(x => x.FileName == grupo.NombreArchivo);

                if (grupo.EsFacturaPorDiferenciaTasaDeCambio)
                {
                    certificacionesRegistradas.Add(GuardarFacturaPorDiferenciaTasaDeCambio(grupo, archivo, proveedorId, cuit, codigoProveedor, usuario));
                }
                else
                {
                    certificacionesRegistradas.AddRange(RegistrarCertificacion(grupo.Items, usuario, proveedorId, archivos, cuit, codigoProveedor));
                }
            }

            return certificacionesRegistradas;
        }

        private List<CertificacionRegistrada> RegistrarCertificacion(List<CertificacionDto> certificaciones, Usuario usuario, int proveedorId, List<HttpPostedFileBase> files, string cuit, string codigo)
        {
            List<ValidationResult> resultadoOcrs = new List<ValidationResult>();
            foreach (var file in files)
            {
                resultadoOcrs.AddRange(ObtenerElementosArchivoPorOCR(file));
            }

            var fechaRegistro = DateTime.Now;
            var usuarioId = usuario.Id;
            List<CertificacionRegistrada> certificacionRegistradas = certificaciones.Select(certificacion => new CertificacionRegistrada
            {
                NombreDeArchivo = certificacion.NombreDeArchivo,
                NRO_OC = certificacion.NRO_OC,
                NRO_Certificacion = certificacion.NRO_Certificacion,
                Importe = certificacion.Importe,
                Moneda = certificacion.Moneda,
                FechaDeRegistro = fechaRegistro,
                UsuarioId = usuarioId,
                ProveedorId = proveedorId
            }).ToList();
            // Verify that the filename is present on the list of certificacionesRegistradas
            foreach (var file in files)
            {
                Log.Info("Procesando el documento " + file.FileName);
                List<string> elementosLeidos = resultadoOcrs.Where(a => a.FileName == file.FileName).Select(a => a.Input).ToList();
                List<ValidationResult> resultadoAnalisis = analisisDocumentoService.AnalizarFacturaCertificacionServicios(elementosLeidos, cuit, file.FileName);
                List<ValidationResult> resultado = AnalizarResultados(resultadoAnalisis, codigo);
                // Upload the file
                string ruta = GenerarRutaArchivo(ConfigurationManager.AppSettings["FolderFacturasES"], usuarioId, file.FileName);
                file.SaveAs(ruta);
                Archivo archivo = new Archivo { Ruta = ruta, FileKey = FileKeys.FacturaEntradaDeServicios };
                repositorio.Agregar(archivo);
                repositorio.GuardarCambios();
                // Add the ArchivoId to the CertificacionRegistrada element
                certificacionRegistradas
                    .Where(c => c.NombreDeArchivo == file.FileName)
                    .ToList()
                    .ForEach(c => c.ArchivoId = archivo.Id);
                resultadoAnalisis.ForEach(r => r.Archivo_Id = archivo.Id);
                if (resultado.Exists(r => r.IsValid))
                {
                    EnviarMail(file);
                }
                GuardarResultadosYArchivo(elementosLeidos, resultadoAnalisis, ruta, usuarioId);
            }
            repositorio.AgregarTodos(certificacionRegistradas);
            repositorio.GuardarCambios();
            return certificacionRegistradas;
        }

        private CertificacionRegistrada GuardarFacturaPorDiferenciaTasaDeCambio(GrupoCertificaciones grupoCertificaciones, HttpPostedFileBase archivoFactura, int proveedorId, string cuit, string codigoProveedor, Usuario usuario)
        {
            var resultadoOcrs = ObtenerElementosArchivoPorOCR(archivoFactura).ToList();

            var usuarioId = usuario.Id;

            Log.Info("Procesando documento por diferencia de tasa de cambio: " + archivoFactura.FileName);

            var elementosLeidos = resultadoOcrs.Where(a => a.FileName == archivoFactura.FileName).Select(a => a.Input).ToList();
            List<ValidationResult> resultadoAnalisis = analisisDocumentoService.AnalizarFacturaCertificacionServicios(elementosLeidos, cuit, archivoFactura.FileName);
            List<ValidationResult> resultado = AnalizarResultados(resultadoAnalisis, codigoProveedor);

            var rutaArchivo = GenerarRutaArchivo(ConfigurationManager.AppSettings["FolderFacturasES"], usuarioId, archivoFactura.FileName);
            archivoFactura.SaveAs(rutaArchivo);

            var archivo = new Archivo { Ruta = rutaArchivo, FileKey = FileKeys.FacturaDiferenciaTasaDeCambio };
            repositorio.Agregar(archivo);

            var certificacionRegistrada = new CertificacionRegistrada
            {
                Archivo = archivo,
                FechaDeRegistro = DateTime.Now,
                NombreDeArchivo = archivoFactura.FileName,
                NRO_OC = grupoCertificaciones.OrdenDeCompra,
                ProveedorId = proveedorId,
                UsuarioId = usuarioId,
                NRO_Certificacion = string.Empty,
                Importe = 0,
                Moneda = string.Empty
            };
            repositorio.Agregar(certificacionRegistrada);
            repositorio.GuardarCambios();

            if (resultado.Exists(r => r.IsValid))
            {
                EnviarMail(archivoFactura);
            }
            GuardarResultadosYArchivo(elementosLeidos, resultadoAnalisis, rutaArchivo, usuarioId);
            return certificacionRegistrada;
        }

        private void GuardarResultadosYArchivo(List<string> elementosLeidos, List<ValidationResult> resultadoAnalisis, string ruta, int usuarioId)
        {
            try
            {
                var resultadosORC = elementosLeidos.Select(a => new ResultadoOcr
                {
                    Archivo_Id = resultadoAnalisis[0].Archivo_Id,
                    Texto = a,
                    Usuario_Id = usuarioId,
                    FechaAlta = DateTime.Now
                }).ToList();
                repositorio.AgregarTodos(resultadosORC);


                var resultadosAnalisisOcr = resultadoAnalisis.Select(item => new ResultadoAnalisisOcr
                {
                    Archivo_Id = item.Archivo_Id,
                    Usuario_Id = usuarioId,
                    FechaAlta = DateTime.Now,
                    IsValid = item.IsValid,
                    Message = item.Message,
                    ValidataionType = item.ValidataionType,
                    Value = item.Value,
                    Input = item.Input
                }).ToList();
                repositorio.AgregarTodos(resultadosAnalisisOcr);
            }
            catch (Exception e)
            {
                Log.Error("Error al guardar los resultados para el documento " + ruta, e);
            }
        }

        private static string GenerarRutaArchivo(string folderBase, int usuarioId, string fileName)
        {
            folderBase = folderBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string directorioUsuario = Path.Combine(folderBase, usuarioId.ToString().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));


            if (!Directory.Exists(directorioUsuario))
            {
                Directory.CreateDirectory(directorioUsuario);
            }

            string rutaArchivo = Path.Combine(directorioUsuario, fileName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));


            int contador = 1;
            string nombreArchivo = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            while (File.Exists(rutaArchivo))
            {
                rutaArchivo = Path.Combine(directorioUsuario, $"{nombreArchivo}_{contador}{extension}");
                contador++;
            }

            return rutaArchivo;
        }


        private void EnviarMail(HttpPostedFileBase file)
        {
            var clonedStream = new MemoryStream();
            file.InputStream.CopyTo(clonedStream);
            clonedStream.Position = 0;

            emailService.EnviarMail(new EmailSenderData
            {
                Asunto = "Envio Factura " + file.FileName,
                Mails = new List<string> { EmailFacturasES },
                Adjuntos = new List<EmailAttachment>
                {
                    new EmailAttachment(clonedStream, file.FileName)
                }
            });

        }

        private List<ValidationResult> AnalizarResultados(List<ValidationResult> resultadoAnalisis, string codigoProveedor)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            if (!resultadoAnalisis.Any())
            {
                result.Add(new ValidationResult(false, "No se pudo procesar el documento", "OCR", "", ""));
                return result;
            }

            var CuitNoEncontrado = resultadoAnalisis.Where(a => !a.IsValid && a.ValidataionType == typeof(CuitValidationCommand).Name);
            if (CuitNoEncontrado.Any())
            {
                result.AddRange(CuitNoEncontrado.ToList());
                return result;
            }

            var FacturaNoEncontrada = resultadoAnalisis.Where(a => !a.IsValid && (a.ValidataionType == typeof(NumeroFacturaValidationCommand).ToString() || a.ValidataionType == typeof(CodigoFacturaValidationCommand).ToString()));
            if (FacturaNoEncontrada.Any())
            {
                result.AddRange(FacturaNoEncontrada.ToList());
                return result;
            }

            //Verify that only one file must have one oc
            if (resultadoAnalisis.Where(a => a.IsValid && a.ValidataionType == typeof(OrdenCompraValidationCommand).Name).Select(a => a.Value).Distinct().Count() > 1)
            {
                // Get the list of OCs
                var ocs = resultadoAnalisis.Where(a => a.IsValid && a.ValidataionType == typeof(OrdenCompraValidationCommand).Name).Select(a => a.Value).Distinct().ToList();
                string texto = "No se puede procesar el documento ya que tiene más de una orden de compra: ";
                ocs.ForEach(oc => texto += oc + ", ");
                texto = texto.Substring(0, texto.Length - 2) + ".";

                result.Add(new ValidationResult(false, texto, typeof(OrdenCompraValidationCommand).Name, "", ""));
                return result;
            }

            var ordenDeCompraEncontrada = resultadoAnalisis.Find(a => a.IsValid && a.ValidataionType == typeof(OrdenCompraValidationCommand).Name);
            if (ordenDeCompraEncontrada != null)
            {
                var ordenDeCompraSAP = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra(ordenDeCompraEncontrada.Value);

                if (ordenDeCompraSAP.Cabecera == null)
                {
                    result.Add(new ValidationResult(false, $"Orden de compra {ordenDeCompraEncontrada.Value} no encontrada", typeof(OrdenCompraValidationCommand).Name, "", ""));
                    return result;
                }

                if (ordenDeCompraSAP.Cabecera.CodigoProveedor != codigoProveedor)
                {
                    result.Add(new ValidationResult(false, "La orden de compra pertenece a otro proveedor", typeof(OrdenCompraValidationCommand).Name, "", ""));
                    return result;
                }

                if (ordenDeCompraSAP.Posiciones[0].TipoPosicion == "SERVICIO")
                {
                    if (ordenDeCompraSAP.Cabecera.SaldoDisponible <= 0)
                    {
                        result.Add(new ValidationResult(false, $"La orden de compra {ordenDeCompraEncontrada.Value} no tiene saldo disponible. Factura no enviada. Deberá certificar y volver a cargarla nuevamente.", typeof(OrdenCompraValidationCommand).Name, "", ordenDeCompraEncontrada.Value));
                        return result;
                    }

                    // Validar si tiene certificaciones
                    if (ordenDeCompraSAP.Certificaciones.Count == 0)
                    {
                        result.Add(new ValidationResult(false, "La orden de compra no tiene certificaciones pendientes de facturar", typeof(OrdenCompraValidationCommand).Name, "", ""));
                        return result;
                    }

                    if (ordenDeCompraSAP.Cabecera.SaldoDisponible > 0)
                    {
                        result.Add(new ValidationResult
                        {
                            IsValid = true,
                            Message = $"Seleccione las certificaciones para la orden de compra {ordenDeCompraEncontrada.Value}.",
                            ValidataionType = typeof(OrdenCompraValidationCommand).Name,
                            Value = ordenDeCompraEncontrada.Value,
                            Certificaciones = ordenDeCompraSAP.Certificaciones,
                            EsMonedaExtranjera = ordenDeCompraSAP.Cabecera.Moneda != nameof(Currency.ARP)
                        });
                        return result;
                    }
                }
            }

            result.Add(new ValidationResult(true, "El documento se envió a para su análisis.", "OCR", "", ""));

            return result;
        }

        public void EliminarFacturasAntiguas()
        {
            try
            {
                DateTime fechaLimite = DateTime.Now.AddDays(-60);
                var resultadosOcr = repositorio.Listar<ResultadoOcr>(a => a.FechaAlta <= fechaLimite);
                var resultadosAnalisisOcr = repositorio.Listar<ResultadoAnalisisOcr>(r => r.FechaAlta <= fechaLimite);
                var archivosAntiguosIds = resultadosOcr.Select(a => a.Archivo_Id).ToList();
                archivosAntiguosIds.AddRange(resultadosAnalisisOcr.Select(a => a.Archivo_Id).ToList());
                archivosAntiguosIds = archivosAntiguosIds.Distinct().ToList();
                var archivosAntiguos = repositorio.Listar<Archivo>(a => archivosAntiguosIds.Contains(a.Id));

                foreach (var archivo in archivosAntiguos)
                {
                    if (File.Exists(archivo.Ruta))
                    {
                        File.Delete(archivo.Ruta);
                    }
                }
                repositorio.RemoverTodos(resultadosOcr);
                repositorio.RemoverTodos(resultadosAnalisisOcr);
                repositorio.RemoverTodos(archivosAntiguos);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error("Error al eliminar facturas antiguas", e);
            }
        }

        public Archivo ObtenerArchivo(int archivoId)
        {
            return repositorio.Obtener<Archivo>(a => a.Id == archivoId);
        }

        public object ObtenerReporteFacturasCertificaciones(
            string fechaInicio,
            string fechaFin,
            string ordenDeCompra = null,
            string proveedor = null,
            int? itemsPorPagina = null,
            int? pagina = null,
            string orden = null,
            string columna = null
            )
        {
            try
            {
                // Preparar objeto de paginación si se especifican los parámetros
                Paginacion paginacion = null;
                if (pagina.HasValue || itemsPorPagina.HasValue)
                {
                    var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
                    paginacion = new Paginacion(
                    (!string.IsNullOrEmpty(columna) ? columna : null),
                    ordenar,
                        (pagina == null) ? 0 : pagina.Value,
                        (itemsPorPagina == null || itemsPorPagina == 0) ? 10 : itemsPorPagina.Value
                    );
                }

                // Validar las fechas
                var fechaInicioParsed = DateTime.Parse(fechaInicio);
                var fechaFinParsed = DateTime.Parse(fechaFin);
                var pag = new Paginacion("FechaDeRegistro", DirOrden.Desc, 1, 10);
                var consulta = new ListarCertificacionRegistradaConsulta(paginacion, ordenDeCompra, proveedor, fechaInicioParsed, fechaFinParsed);
                var resultado = repositorio.ListarConsultaPaginada(consulta);
                var certificacionesSinAreas = resultado.Select(c => new
                {
                    c.Id,
                    c.Proveedor.RazonSocial,
                    c.NRO_OC,
                    NRO_Certificacion = c.Archivo.FileKey != FileKeys.FacturaDiferenciaTasaDeCambio ? c.NRO_Certificacion : "Por diferencia de tasa de cambio",
                    c.Importe,
                    c.Archivo,
                    c.FechaDeRegistro,
                    c.Usuario.Mail,
                    c.Moneda
                }).ToList();

                int totalItems = resultado.Items.Count;
                itemsPorPagina = paginacion?.ItemsPorPagina ?? totalItems;
                int paginaActual = paginacion?.Pagina ?? 1;
                int totalPaginas = (int)Math.Ceiling((decimal)totalItems / itemsPorPagina.Value);

                return new
                {
                    certificaciones = certificacionesSinAreas,
                    totalItems = totalItems,
                    totalPaginas = totalPaginas,
                    paginaActual = paginaActual
                };

            }
            catch (Exception ex)
            {
                Log.Error($"Error en ObtenerReporteFacturasCertificaciones: {ex.Message}", ex);
                throw;
            }
        }

        private IEnumerable<ValidationResult> ObtenerElementosArchivoPorOCR(HttpPostedFileBase archivo)
        {
            Log.Info("AnalizarImagenAsync: " + archivo.FileName);

            var operacionOCRId = Task.Run(async () => await azureService.AnalizarImagenAsync(archivo)).Result;
            Thread.Sleep(2000);

            Log.Info("ObtenerResultadoOCRAsync: " + archivo.FileName);

            var elementosLeidos = Task.Run(async () => await azureService.ObtenerResultadoOCRAsync(operacionOCRId)).Result;
            var resultadoOcrs = elementosLeidos.Select(a => new ValidationResult { Input = a, FileName = archivo.FileName });

            Log.Info("Fin ObtenerResultadoOCRAsync: " + archivo.FileName);

            return resultadoOcrs;
        }
    }
}
