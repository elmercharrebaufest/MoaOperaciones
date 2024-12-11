using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOARepositorio;
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
                throw new ValidationCustomException("No files provided.");

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
            string directorioUsuario = Path.Combine(folderBase, usuarioId.ToString());

            if (!Directory.Exists(directorioUsuario))
            {
                Directory.CreateDirectory(directorioUsuario);
            }

            string rutaArchivo = Path.Combine(directorioUsuario, fileName);

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

            var OrdenDeCompraEncontrada = resultadoAnalisis.Find(a => a.IsValid && a.ValidataionType == typeof(OrdenCompraValidationCommand).Name);
            if (OrdenDeCompraEncontrada != null)
            {
                var ordenDeCompraSAP = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra(OrdenDeCompraEncontrada.Value);
                if (ordenDeCompraSAP.Cabecera.CodigoProveedor != codigoProveedor)
                {
                    result.Add(new ValidationResult(false, "La orden de compra pertenece a otro proveedor", typeof(OrdenCompraValidationCommand).Name, "", ""));
                    return result;
                }

                if (ordenDeCompraSAP.Cabecera.SaldoDisponible <= 0 && ordenDeCompraSAP.Posiciones[0].TipoPosicion == "SERVICIOS")
                {
                    result.Add(new ValidationResult(false, "La orden de compra no tiene saldo disponible.", typeof(OrdenCompraValidationCommand).Name, "", ""));
                    return result;
                }

            }

            result.Add(new ValidationResult(true, "El documento se envió a para su análisis.", "OCR", "", ""));

            return result;
        }
    }
}
