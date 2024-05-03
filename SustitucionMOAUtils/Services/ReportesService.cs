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
using SustitucionMOAModel.Dto;
using System.Text;

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

            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReporteCamposSustentablesTSAJob").Habilitado == false)
                return;
            if (DateTime.Today.DayOfWeek != DayOfWeek.Tuesday && DateTime.Today.DayOfWeek != DayOfWeek.Friday)
            {
                return;
            }

            /*Mail del Martes: Se va a enviar los campos registrados los Viernes, Sábado, Domingo y Lunes anteriores
            Mail del Viernes: Se va a enviar los campos registrados los Martes, Miércoles y Jueves anteriores */

            var diasAtras = DateTime.Today.DayOfWeek == DayOfWeek.Tuesday ? 4 : 3;

            var dateToCompare = DateTime.Today.AddDays(-diasAtras);

            var camposAReportarPorCosecha = repositorio.ListarAgrupado<CampoProveedor, string, CampoReporteDto>(
                cp => cp.CampoCosecha.Cosecha.Nombre,
                cp => new CampoReporteDto
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
                },
                cp =>
                    cp.FechaCreacion.HasValue &&
                    DbFunctions.TruncateTime(cp.FechaCreacion.Value) >= DbFunctions.TruncateTime(dateToCompare) &&
                    DbFunctions.TruncateTime(cp.FechaCreacion.Value) < DbFunctions.TruncateTime(DateTime.Today) &&
                    cp.CampoCosecha.Cosecha.EnviarATSA
            );

            if (!camposAReportarPorCosecha.Any() || camposAReportarPorCosecha.All(list => !list.Any()))
            {
                throw new InfoCustomException("No se encontraron campos sustentables a reportar");
            }

            foreach (var camposAReportar in camposAReportarPorCosecha)
            {

                var excelFile = ExcelExport.ToExcel(camposAReportar, new string[] { "ID", "Codigo Operaciones", "Titular CCPP", "CUIT", "Nombre del Establecimiento", "Provincia", "Departamento", "Localidad", "Latitud", "Longitud", "Has de soja declaradas" }, string.Empty);

                Attachment archivoZip;
                var nombreArchivoXls = $"Listado campos {DateTime.Today:yyyy-MM-dd} - Cosecha {camposAReportar[0].NombreCosecha}.xls";
                var nombreArchivoZip = $"Campos sustentables{DateTime.Today:yyyy-MM-dd} - Cosecha {camposAReportar[0].NombreCosecha}.zip";

                var outputMemStream = new MemoryStream();


                var zipStream = new ZipOutputStream(outputMemStream);
                zipStream.SetLevel(3);

                foreach (var campo in camposAReportar)
                {
                    string rutaArchivoKmz = campo.RutaKmz;
                    string extension = Path.GetExtension(rutaArchivoKmz);

                    var kmzFileName = MakeValidFileName(string.Concat(campo.Id, "-", campo.Nombre,".", extension));

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
                }
                zipStream.IsStreamOwner = false;
                zipStream.Close();
                outputMemStream.Position = 0;

                archivoZip = new Attachment(outputMemStream, nombreArchivoZip);

                Attachment archivoExcel;
                MemoryStream streamExcel = new MemoryStream();
                var sw = new StreamWriter(streamExcel);

                sw.Write(excelFile);
                sw.Flush();
                streamExcel.Seek(0, SeekOrigin.Begin);

                archivoExcel = new Attachment(streamExcel, nombreArchivoXls);

                EmailSender.SendReporte(new ReporteCamposSustentables()
                {
                    Asunto = $"Reporte de Altas de Campos Sustentables - Cosecha {camposAReportar[0].NombreCosecha} - Resumen Diario {DateTime.Today:yyyy-MM-dd}",
                    CantidadCampos = camposAReportar.Count(),
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteCamposSustentables"],
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteCamposSustentables.html"),
                    Adjuntos = new List<Attachment>
                {
                    archivoExcel
                    ,
                    archivoZip
                }
                });

                sw.Dispose();
                outputMemStream.Dispose();

            }
        }

        public void EnviarReporteLiquidacionesInformadas()
        {
            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReporteLiquidacionesInformadasJob").Habilitado == false)
                return;

            var fechaAReportar = DateTime.Today.AddDays(-1);
            //Vamos a reportar las liquidaciones informadas del día de ayer
            var liquidacionesAReportar = repositorio.Listar<LiquidacionInformada>(li => li.FechaInformada == fechaAReportar);

            if (liquidacionesAReportar.Any())
            {
                EmailSender.SendReporte(new ReporteLiquidacionesInformadas()
                {
                    Asunto = $"Reporte de Liquidaciones Informadas - Resumen Diario",
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

        public void EnviarReporteLogin()
        {
            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReporteLoginsJob").Habilitado == false)
                return;

            var logins = repositorio.Listar<Usuario>().SelectMany(u =>
            {
                if (!u.Proveedores.Any())
                    return new List<ReporteLoginData>()
                        {
                            new ReporteLoginData(
                                u.Mail,
                                u.CUITRegistro,
                                u.UltimoLogin,
                                u.TipoUsuario.Nombre)
                     };

                return u.Proveedores.Select(p =>
                    new ReporteLoginData(
                        u.Mail,
                        u.CUITRegistro,
                        u.UltimoLogin,
                        u.TipoUsuario.Nombre,
                        p.CUIT,
                        p.RazonSocial
                    )).ToList();
            }).ToList();

            if (logins.Any())
            {

                MemoryStream streamExcel = ExcelExport.CreateExcelFileMs(logins, new string[] { "Mail", "CUIT Registro", "Ultimo Login", "Nombre", "CUIT Proveedor", "Razón Social" });
                Attachment archivoExcel;
                archivoExcel = new Attachment(streamExcel, "Usuarios MOA.xlsx");

                EmailSender.SendReporte(new ReporteLogin()
                {
                    Asunto = "Reporte de logins mensuales",
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteLogins"],
                    Adjuntos = new List<Attachment> { archivoExcel },
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteLogin.html")
                });
            }
            else
            {
                throw new InfoCustomException("No se encontraron logins a reportar");
            }
        }


        public void EnviarReporteConflictosCamposSustentables()
        {
            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReporteConflictosCamposSustentablesJob").Habilitado == false)
                return;

            var fechaAReportar = DateTime.Today.AddDays(-1);
            //Vamos a reportar las liquidaciones informadas del día de ayer
            var campoSustentablesAReportar = repositorio.Listar<ConflictoCampoSustentable>(cc => !cc.Notificado);

            if (campoSustentablesAReportar.Any())
            {
                EmailSender.SendReporte(new ReporteConflictoCampoSustentable()
                {
                    Asunto = "Conflictos en reporte de campos sustentables",
                    Destinatario = ConfigurationManager.AppSettings["EmailToReporteCamposSustentables"],
                    Campos = campoSustentablesAReportar,
                    Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteConflictosCamposSustentables.html")
                });
            }

            campoSustentablesAReportar.ForEach(x => x.Notificado = true);

            repositorio.GuardarCambios();

        }

        private string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
