using System;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Logger;
using SustitucionMOAModel.Entities;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Configuration;
using Newtonsoft.Json;
using System.Text;
using SustitucionMOAFotmatter;

namespace SustitucionMOA.Jobs
{
    public interface IEnviarCamposUcropitJob : IHangfireJob { }
    public class EnviarCamposUcropitJob : IEnviarCamposUcropitJob
    {
        private readonly IRepositorio repositorio;
        public EnviarCamposUcropitJob(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "EnviarCamposUcropitJob");
                if (habilitacion == null || !habilitacion.Habilitado)
                {
                    return;
                }

                var cosechaAReportar = repositorio.Obtener<Configuracion>(c => c.Code== "CosechaParaEnvioUcropit").Value;
                var limiteCamposPorMail = int.Parse(repositorio.Obtener<Configuracion>(c => c.Code == "CosechaParaEnvioUcropitTope").Value);

                var fechaLimiteConfig = repositorio.Obtener<Configuracion>(c => c.Code == "FechaLimiteCamposSustentables");
                var fechaLimite = DataFormatter.StringToDateTime(fechaLimiteConfig.Value,"fechaLimiteCamposSustentables");

                var camposAReportar = repositorio.Listar<CampoProveedor, CampoReporteDTO>(
                    cp => new CampoReporteDTO
                    {
                        IdScato = cp.CampoCosecha.Campo.IdScato,
                        Id = cp.CampoCosecha.Campo.Id,
                        RazonSocial = cp.RazonSocial,
                        CUIT = cp.CUIT,
                        Nombre = cp.CampoCosecha.Campo.Nombre,
                        Provincia = cp.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                        Departamento = cp.CampoCosecha.Campo.Localidad.Partido.Descripcion,
                        Localidad = cp.CampoCosecha.Campo.Localidad.Nombre,
                        Latitud = cp.Latitud,
                        Longitud = cp.Longitud,
                        HectareasSoja = cp.HectareasSoja,
                        NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                        RutaKmz = cp.Archivo.Ruta,
                    },
                    cp =>
                        cp.CampoCosecha.ToneladasAprobadas == -1 &&
                        cp.CampoCosecha.Cosecha.Nombre == cosechaAReportar &&
                        cp.FechaCreacion >= fechaLimite
                    ).ToList();

                if (!camposAReportar.Any())
                {
                    throw new InfoCustomException("No se encontraron campos sustentables a reportar");
                }
                var fechaDeEnvio = DateTime.Now.ToString();
                var excelFile = ExcelExport.ToExcel(camposAReportar, new string[] { "ID Scato", "ID Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);

                var nombreArchivoZip = $"Campos sustentables{DateTime.Today:yyyy-MM-dd} - Cosecha {camposAReportar[0].NombreCosecha}.zip";


                var counter = 0;
                var camposAReportarExcel = new List<CampoReporteDTO>();

                var outputMemStream = new MemoryStream();
                var zipStream = new ZipOutputStream(outputMemStream);
                zipStream.SetLevel(3);

                foreach (var campo in camposAReportar)
                {
                    var fileName = ObtenerNombreArchivoDrive(campo);
                    try
                    {
                        CargarKmzEnZip(zipStream, fileName, campo);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        continue;
                    }
                    CargarJSONReporteCampoEnZip(zipStream, fileName,campo);
                    camposAReportarExcel.Add(campo);

                    counter += 1;
                    if(counter == limiteCamposPorMail)
                    {
                        counter = 0;
                        EnviarMail(excelFile,zipStream,nombreArchivoZip,outputMemStream, cosechaAReportar);
                        outputMemStream = new MemoryStream();
                        zipStream = new ZipOutputStream(outputMemStream);
                        zipStream.SetLevel(3);
                        camposAReportarExcel = new List<CampoReporteDTO>();
                    }
                }

                if(counter != 0)
                {
                    EnviarMail(excelFile, zipStream, nombreArchivoZip, outputMemStream, cosechaAReportar);
                }
                fechaLimiteConfig.Value = fechaDeEnvio;
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void EnviarMail(string excelFile, ZipOutputStream zipStream, string nombreArchivoZip, MemoryStream outputMemStream, string cosechaAReportar)
        {
            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;

            var archivoZip = new Attachment(outputMemStream, nombreArchivoZip);

            MemoryStream streamExcel = new MemoryStream();
            var sw = new StreamWriter(streamExcel);

            sw.Write(excelFile);
            sw.Flush();
            streamExcel.Seek(0, SeekOrigin.Begin);


            EmailSender.SendReporte(
                new EnvioCamposSustentablesUcropit()
                {
                    Asunto = $"Campos Sustentables en Gestion - Cosecha {cosechaAReportar}",
                    Cosecha= cosechaAReportar,
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteCamposSustentables"],
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EnvioCamposSustentablesUcropit.html"),
                    Adjuntos = new List<Attachment>
                    {
                        archivoZip
                    }
                });

            sw.Dispose();
            outputMemStream.Dispose();
        }

        private string ObtenerNombreArchivoDrive(CampoReporteDTO campoReporte)
        {
            return $"{campoReporte.CUIT}_{campoReporte.Id}_{campoReporte.NombreCosecha}";
        }
        private void CargarJSONReporteCampoEnZip(ZipOutputStream zipStream,string fileName ,CampoReporteDTO campo) {

            var reporteCertificadorJson = JsonConvert.SerializeObject(campo);
            var jsonBytes = Encoding.UTF8.GetBytes(reporteCertificadorJson);

            ZipEntry entry = new ZipEntry($"{fileName}.json")
            {
                DateTime = DateTime.Now,
            };

            CargarYCerrarZipEntry(zipStream,entry, new MemoryStream(jsonBytes));
        }
        private void CargarKmzEnZip(ZipOutputStream zipStream, string fileName, CampoReporteDTO campo)
        {
            string rutaArchivoKmz = campo.RutaKmz;
            string extension = Path.GetExtension(rutaArchivoKmz);
            campo.RutaKmz = null;
            if(!System.IO.File.Exists(rutaArchivoKmz)){
                throw new Exception($"El archivo {rutaArchivoKmz} no se ha encontrado, se omite este campo");
            }

            ZipEntry entry = new ZipEntry($"{fileName}.{extension}")
            {
                DateTime = DateTime.Now,
            };

            Stream stream = null;
            try
            {
                stream = new MemoryStream(File.ReadAllBytes(rutaArchivoKmz));
            }
            catch (Exception)
            {
                stream = new MemoryStream(File.ReadAllBytes(campo.RutaKmz));
            }

            CargarYCerrarZipEntry(zipStream, entry, stream);
        }
        private void CargarYCerrarZipEntry(ZipOutputStream zipStream, ZipEntry entry, Stream stream)
        {
            zipStream.PutNextEntry(entry);
            StreamUtils.Copy(stream, zipStream, new byte[4096]);
            zipStream.CloseEntry();
        }

        private string GetExcelFile(List<CampoReporteDTO> camposAReportar)
        {
            return ExcelExport.ToExcel(camposAReportar, new string[] { "ID Scato", "ID Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);
        } 
    }
}