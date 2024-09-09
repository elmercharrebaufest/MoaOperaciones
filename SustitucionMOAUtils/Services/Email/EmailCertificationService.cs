using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAUtils.Logger;
using SustitucionMOAModel.Dto.OrdenesCompra;
using System.Globalization;
using DocumentFormat.OpenXml.Bibliography;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailCertificationService : IEmailCertificationService
    {
        private static readonly string TEMPLATE_NOTIFICATION_CERTIFICATION_REJECTED = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacionRechazada.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_EXT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_PROVEEDOR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion-Proveedor.html");
        private readonly IEmailService emailService;
        protected readonly IAzureService azureService;

        public EmailCertificationService(IEmailService emailService, IAzureService azureService)
        {
            this.emailService = emailService;
            this.azureService = azureService;
        }

        private (List<string>, string, string) BuildEmail(EmailDetailCertificateDto emailDetail, string bodyTemplate, string subjectFormat, params object[] subjectArgs)
        {
            StringBuilder bodyTable = BuildTableDetailES(emailDetail);

            var addressee = new List<string>() { emailDetail.Destinatario };

            var subject = string.Format(subjectFormat, subjectArgs);

            var body = string.Format(bodyTemplate, subjectArgs.Concat(new object[] { bodyTable.ToString() }).ToArray());

            return (addressee, subject, body);
        }

        private (List<string>, string, string) BuildRejectedEmail(EmailDetailCertificateDto emailDetail, string bodyTemplate)
        {
            string subjectFormat = "Asunto: Rechazo de servicio - Certificación nro {0}";
            object[] subjectArgs = { emailDetail.NumeroCertificacion, emailDetail.MotivoRechazo,
                emailDetail.Proveedor, emailDetail.GeneradoPor, emailDetail.NumeroCertificacion, emailDetail.FechaCertificacion,
                emailDetail.Descripcion, emailDetail.MontoTotal, emailDetail.NroOC};

            return BuildEmail(emailDetail, bodyTemplate, subjectFormat, subjectArgs);
        }

        private (List<string>, string, string) BuildApprovedEmail(EmailDetailCertificateDto emailDetail, string bodyTemplate)
        {
            string subjectFormat = "Asunto: Aceptación de servicio - Certificación nro {0}";
           
            object[] subjectArgs = { emailDetail.NumeroCertificacion , emailDetail.NumeroCertificacion , emailDetail.NroOC, emailDetail.NumeroPosicion
                    , emailDetail.FechaCertificacion, emailDetail.Proveedor, emailDetail.Descripcion, emailDetail.MontoTotal, emailDetail.Aprobador};

            return BuildEmail(emailDetail, bodyTemplate, subjectFormat, subjectArgs);
        }

        public async Task SendNotifyRejectionEmail(EmailDetailCertificateDto emailDetailCertificateDto)
        {
            string bodyTemplate = File.ReadAllText(TEMPLATE_NOTIFICATION_CERTIFICATION_REJECTED);

            (List<string> emails, string subject, string body) emailParts = BuildRejectedEmail(emailDetailCertificateDto, bodyTemplate);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailParts.emails,
                Asunto = emailParts.subject,
                Cuerpo = emailParts.body,
            };

            Task emailSendTask = Task.Run(() => EmailSender.EnviarMail(emailSenderData));
            await emailSendTask;
        }

        public async Task SendAprobalProviderEmail(EmailDetailCertificateDto emailDetailCertificateDto, string reference)
        {
            string bodyTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_APROBACIONES_PROVEEDOR);

            (List<string> emails, string subject, string body) emailParts = BuildApprovedEmail(emailDetailCertificateDto, bodyTemplate);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailParts.emails,
                Asunto = emailParts.subject,
                Cuerpo = emailParts.body,
            };

            Task emailSendTask = Task.Run(() => EmailSender.EnviarMail(emailSenderData));
            await emailSendTask;
        }

        private StringBuilder BuildTableDetailES(EmailDetailCertificateDto emailDetailCertificateDto)
        {
            StringBuilder bodyTable = new StringBuilder();

            var moneda = emailDetailCertificateDto.Moneda == "ARP" ? "$ " : emailDetailCertificateDto.Moneda + " ";


            foreach (var servicio in emailDetailCertificateDto.DetalleServicio)
            {
                bodyTable.Append("<tr>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Descripcion}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Cantidad}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.UM}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Porcentaje}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{moneda}{servicio.Monto}</td>");
                bodyTable.Append("</tr>");
            }

            bodyTable.Append("<tr>");
            bodyTable.Append($"<td colspan='4' style='padding: 10px; border: 0px;'></td>");
            bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'><strong>{moneda}{emailDetailCertificateDto.MontoTotal}</strong></td>");
            bodyTable.Append("</tr>");

            return bodyTable;
        }

        public async Task EnviarMailAprobacion(List<Aprobaciones> apList, Proveedor prov, int userId, string destinatario, List<ReporteDto> reports)
        {
            var ms = new MemoryStream();

            try
            {
                string dateTimeFormat = "dd/MM/yyyy";
                List<string> dest = new List<string>();
                dest.Add(destinatario);

                string asunto = $" Aprobación de servicio - Certificación nro {apList[0].NRO_ES_LOCAL} ";

                try
                {
                    var report = await GetReportES(apList[0].NRO_ES_LOCAL).ConfigureAwait(false);

                    await report.CopyToAsync(ms);
                }
                catch(Exception e)
                {
                    Log.AzureError(e);
                    Log.Error("EnviarMailAprobacion: error al obtener archivo ",e);
                }

                //Leer Template - CertificacionesPendientesDeAprobacion.html
                var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_APROBACIONES_EXT);

                //Variables para completar el template
                string proveedor = string.IsNullOrEmpty(prov.RazonSocial) ? string.Empty : prov.RazonSocial;
                string usuario = string.Empty;
                string cert = string.Empty;
                string FechaCert = string.Empty;
                string desc = string.Empty;
                string importe = string.Empty;
                //MMSN-928 - Agregar OC al email.
                string OC = string.Empty;
                string NroPosicion = string.Empty;
                StringBuilder tabla = new StringBuilder();
                if (apList.Count > 0)
                {
                    usuario = apList[0].Ingresante_CDS;
                    cert = apList[0].NRO_ES_LOCAL;
                    DateTime fechaCarga = apList[0].Fecha_Carga_ES != null ? (DateTime)apList[0].Fecha_Carga_ES : DateTime.Now;
                    FechaCert = fechaCarga.ToString(dateTimeFormat);
                    desc = apList[0].Texto_breve_servicio;
                    importe = reports[0].Moneda == "ARP" ? "$ " + apList[0].Monto_total.ToString() : reports[0].Moneda + " " + apList[0].Monto_total.ToString();
                    OC = apList[0].NRO_OC;
                    tabla = GenerarTablaAprobaciones(reports);
                    NroPosicion = apList[0].NRO_POS;
                }


                string baseURL = ConfigurationManager.AppSettings["SpaUrl"];
                string approvalURL = "\"" + baseURL + "/aprobacion-externa/approve/" + apList[0].NRO_ES_LOCAL + "&" + userId + "\"";
                string rejectURL = "\"" + baseURL + "/aprobacion-externa/reject/" + apList[0].NRO_ES_LOCAL + "&" + userId + "\"";

                string _baseURL = "\"" + baseURL + "\"";
                var cuerpo = string.Format(cuerpoTemplate, proveedor, usuario, cert, FechaCert, desc, importe, tabla, approvalURL, rejectURL, OC, NroPosicion, _baseURL);

                ms.Position = 0;

                var emailSenderData = new EmailSenderData()
                {
                    Mails = dest,
                    Asunto = asunto,
                    Cuerpo = cuerpo,
                    Archivo = ms.Length > 0 ? ms.GetBuffer(): null,
                    NombreArchivo = "Reporte.pdf"
                };


                Task emailSendTask = Task.Run(() => EmailSender.EnviarMail(emailSenderData));
                await emailSendTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private StringBuilder GenerarTablaAprobaciones(List<ReporteDto> reports)
        {
            decimal montoTotal = 0;
            var aprStrBuilder = new StringBuilder();
            var moneda = reports[0].Moneda == "ARP" ? "$" : reports[0].Moneda;

            foreach (ReporteDto report in reports)
            {
                decimal porcentajeAnterior = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) / 100);
                decimal porcentajeAcertificar = (report.PorcentajeACertificar / 100);
                decimal porcentajeAcumulado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) + report.PorcentajeACertificar) / 100;

                string porcentajeAnteriorFormateado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture)).ToString().StartsWith("100") ?
                    porcentajeAnterior.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAnterior.ToString("P2", CultureInfo.GetCultureInfo("en-US"));

                string porcentajeACertificarFormateado = report.PorcentajeACertificar.ToString().StartsWith("100") ?
                    porcentajeAcertificar.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAcertificar.ToString("P2", CultureInfo.GetCultureInfo("en-US"));

                string porcentajeAcumuladoFormateado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) + report.PorcentajeACertificar).ToString().StartsWith("100") ?
                    porcentajeAcumulado.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAcumulado.ToString("P2", CultureInfo.GetCultureInfo("en-US"));

                aprStrBuilder.Append($"<tr>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.NumeroLinea.ToString()}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.ServicioNumero.ToString() ?? "N/A"}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.Descripcion ?? "N/A"}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.Cantidad.ToString("N2", CultureInfo.GetCultureInfo("en-US")) ?? "N/A"}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.UM ?? "N/A"}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{moneda} {report.Importe.ToString("N2", CultureInfo.GetCultureInfo("en-US")) ?? "N/A"}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{moneda} {(Convert.ToDecimal(report.Cantidad, CultureInfo.InvariantCulture) * report.Importe).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    // Anteriores
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.CantidadReal.ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{porcentajeAnteriorFormateado}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{moneda} {(report.CantidadReal * report.Importe).ToString("N", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    // A certificar
                    $"<td style='padding: 10px; border: 1px solid #333;'>{report.CantidadACertificar.ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{porcentajeACertificarFormateado}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{moneda} {(report.CantidadACertificar * report.Importe).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    // Acumulado
                    $"<td style='padding: 10px; border: 1px solid #333;'>{(report.CantidadReal + report.CantidadACertificar).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{porcentajeAcumuladoFormateado}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{moneda} {((report.CantidadReal * report.Importe) + (report.CantidadACertificar * report.Importe)).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>" +
                $"</tr>");
                montoTotal += (report.CantidadACertificar * report.Importe);
            }
            //Ultima fila - solo monto total
            aprStrBuilder.Append($"<tr>" +
                $"<td colspan='12' style='padding: 10px; border: 0px solid #333;'></td>" +
                $"<td style='padding: 10px; border: 1px solid #333;'><strong>{moneda} {montoTotal.ToString("N2")}</strong></td>" +
                $"</tr>");

            return aprStrBuilder;
        }

        private async Task<MemoryStream> GetReportES(string reference)
        {
            var blobResult = await azureService.ObtenerArchivoBlobStorageAsync(reference, "certificaciones").ConfigureAwait(false);
                
            return blobResult;
        }
    }
}
