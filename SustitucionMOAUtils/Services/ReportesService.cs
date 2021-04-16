using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Data.Entity;

namespace SustitucionMOAUtils.Services
{
    public class ReportesService : IReportesService
    {
        private readonly IRepositorio repositorio;

        public ReportesService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void EnviarReporteCamposSustentablesTSA()
        {

            var dateToCompare = DateTime.Today;


            var camposAReportar = repositorio.Listar<CampoProveedor>(cp => cp.FechaCreacion.HasValue 
            && DbFunctions.TruncateTime(cp.FechaCreacion.Value) == DbFunctions.TruncateTime(dateToCompare)
            )
                .Select(cp => new
                {
                    cp.CampoCosecha.Campo.IdScato,
                    cp.CampoCosecha.Campo.Id,
                    cp.Proveedor.RazonSocial,
                    cp.Proveedor.CUIT,
                    cp.CampoCosecha.Campo.Nombre,
                    Provincia = cp.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    Departamento = cp.CampoCosecha.Campo.Localidad.Partido.Descripcion,
                    Localidad = cp.CampoCosecha.Campo.Localidad.Nombre,
                    cp.Latitud,
                    cp.Longitud,
                    cp.HectareasSoja
                });

            if (camposAReportar.Any())
            {
                var excelFile = ExcelExport.ToExcel(camposAReportar, new string[] { "ID", "Codigo Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);

                Attachment archivoZip;
                var nombreArchivoXls = $"Listado campos.xls";
                var nombreArchivoZip = $"Campos sustentables{DateTime.Today:yyyy-MM-dd}.zip";

                var outputMemStream = new MemoryStream();

                using (var zipStream = new ZipOutputStream(outputMemStream))
                {
                    zipStream.SetLevel(3);


                    foreach (var campo in camposAReportar)
                    {
                        string rutaArchivoKmz = string.Concat(ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"], "/", campo.CUIT, "/", campo.Id, ".kmz");

                        var kmzFileName = MakeValidFileName(string.Concat(string.Concat(campo.Id, "-", campo.Nombre, ".kmz")));

                        ZipEntry entry = new ZipEntry(kmzFileName)
                        {
                            DateTime = DateTime.Now,
                        };

                        Stream stream = new MemoryStream(File.ReadAllBytes(rutaArchivoKmz));
                        zipStream.PutNextEntry(entry);
                        StreamUtils.Copy(stream, zipStream, new byte[4096]);
                        zipStream.CloseEntry();
                    }
                    zipStream.IsStreamOwner = false;
                }

                outputMemStream.Position = 0;

                archivoZip = new Attachment(outputMemStream, nombreArchivoZip);

                Attachment archivoExcel;
                using (MemoryStream streamExcel = new MemoryStream())
                {
                    var sw = new StreamWriter(streamExcel);
                    try
                    {
                        sw.Write(excelFile);
                        sw.Flush();
                        streamExcel.Seek(0, SeekOrigin.Begin);

                        archivoExcel = new Attachment(streamExcel, nombreArchivoXls);
                    }
                    finally
                    {
                        sw.Dispose();
                    }
                   
                }

                EmailSender.SendReporte(new ReporteCamposSustentables()
                {
                    Asunto = "Reporte de Liquidaciones Informadas - Resumen Diario",
                    CantidadCampos = camposAReportar.Count(),
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteLiquidacion"],
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteCamposSustentables.html"),
                    Adjuntos = new List<Attachment>
                    {
                       archivoZip,
                       archivoExcel
                    }
                });

            }
            else
            {
                throw new InfoCustomException("No se encontraron campos sustentables a reportar");
            }
        }

        public void EnviarReporteLiquidacionesInformadas()
        {
            var fechaAReportar = DateTime.Today.AddDays(-1);
            //Vamos a reportar las liquidaciones informadas del día de ayer
            var liquidacionesAReportar = repositorio.Listar<LiquidacionInformada>(li => li.FechaInformada == fechaAReportar);

            if (liquidacionesAReportar.Any())
            {
                EmailSender.SendReporte(new ReporteLiquidacionesInformadas()
                {
                    Asunto = "Reporte de Liquidaciones Informadas - Resumen Diario",
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteLiquidacion"],
                    Liquidaciones = liquidacionesAReportar,
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteLiquidacionesInformadas.html")
                });
            }
            else
            {
                throw new InfoCustomException("No se encontraron liquidaciones a reportar");
            }
        }

        private string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
