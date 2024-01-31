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

namespace SustitucionMOA.Jobs
{
    public interface IEnviarCamposUcropitJob : IHangfireJob { }
    public class EnviarCamposUcropitJob : IEnviarCamposUcropitJob
    {
        private readonly IRepositorio repositorio;
        private readonly int LIMITE_CAMPOS_POR_MAIL = 200;
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
                var camposAReportar = repositorio.Listar<CampoProveedor>(
                    cp =>
                        cp.CampoCosecha.ToneladasAprobadas == -1 &&
                        cp.CampoCosecha.Cosecha.Nombre == "22-23"

                    ).Select(cp => new CampoReporteDTO
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
                        RutaKmz = cp.Archivo.Ruta
                    }).ToList();

                if (!camposAReportar.Any())
                {
                    throw new InfoCustomException("No se encontraron campos sustentables a reportar");
                }
                var excelFile = ExcelExport.ToExcel(camposAReportar, new string[] { "ID", "Codigo Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);

                var nombreArchivoXls = $"Listado campos {DateTime.Today:yyyy-MM-dd} - Cosecha {camposAReportar[0].NombreCosecha}.xls";
                var nombreArchivoZip = $"Campos sustentables{DateTime.Today:yyyy-MM-dd} - Cosecha {camposAReportar[0].NombreCosecha}.zip";


                var counter = 0;

                var outputMemStream = new MemoryStream();
                var zipStream = new ZipOutputStream(outputMemStream);
                zipStream.SetLevel(3);

                foreach (var campo in camposAReportar)
                {

                    string rutaArchivoKmz = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campo.CUIT, "/", campo.Id, ".kmz");

                    var kmzFileName = MakeValidFileName(string.Concat(string.Concat(campo.Id, "-", campo.Nombre, ".kmz")));

                    ZipEntry entry = new ZipEntry(kmzFileName)
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

                    zipStream.PutNextEntry(entry);
                    StreamUtils.Copy(stream, zipStream, new byte[4096]);
                    zipStream.CloseEntry();
                    counter += 1;
                    if(counter == LIMITE_CAMPOS_POR_MAIL)
                    {
                        counter = 0;
                        EnviarMail(excelFile,zipStream,nombreArchivoZip,nombreArchivoXls,outputMemStream);
                        outputMemStream = new MemoryStream();
                        zipStream = new ZipOutputStream(outputMemStream);
                        zipStream.SetLevel(3);
                    }
                }

                if(counter != 0)
                {
                    EnviarMail(excelFile, zipStream, nombreArchivoZip, nombreArchivoXls, outputMemStream);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        private string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private void EnviarMail(string excelFile, ZipOutputStream zipStream, string nombreArchivoZip, string nombreArchivoXls, MemoryStream outputMemStream)
        {
            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;

            var archivoZip = new Attachment(outputMemStream, nombreArchivoZip);

            Attachment archivoExcel;
            MemoryStream streamExcel = new MemoryStream();
            var sw = new StreamWriter(streamExcel);

            sw.Write(excelFile);
            sw.Flush();
            streamExcel.Seek(0, SeekOrigin.Begin);

            archivoExcel = new Attachment(streamExcel, nombreArchivoXls);

            EmailSender.SendReporte(
                new EnvioCamposSustentablesUcropit()
                {
                    Asunto = $"Campos Sustentables en Gestion - Cosecha 22-23",
                    Cosecha="22-23",
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteCamposSustentables"],
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "EnvioCamposSustentablesUcropit.html"),
                    Adjuntos = new List<Attachment>
                    {
                        archivoExcel,
                        archivoZip
                    }
                });

            sw.Dispose();
            outputMemStream.Dispose();
        }
    }
}