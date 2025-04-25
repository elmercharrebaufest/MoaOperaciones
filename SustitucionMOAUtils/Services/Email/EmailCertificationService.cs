using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services.Email.Dto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailCertificationService : IEmailCertificationService
    {
        private static readonly string TEMPLATE_NOTIFICATION_CERTIFICATION_REJECTED = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacionRechazada.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_EXT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_EXT_POSICION = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion_Posicion.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_PROVEEDOR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion-Proveedor.html");
        private static readonly string TEMPLATE_NOTIFICACION_DIARIA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionEsPendientesDeAprobacion.html");


        private readonly IEmailService emailService;
        protected readonly IAzureService azureService;

        public EmailCertificationService(IEmailService emailService, IAzureService azureService)
        {
            this.emailService = emailService;
            this.azureService = azureService;
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

        public void SendDailyNotification(string to, List<NotificacionEsPendientesDiariasDto> aprobaciones)
        {
            string bodyTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_DIARIA);

            string body = BuildDailyNotification(aprobaciones, bodyTemplate);


            var emailSenderData = new EmailSenderData
            {
                Mails = new List<string> { to },
                Asunto = "Certificaciones pendientes de aprobación",
                Cuerpo = body,
            };

            var emailSendTask = Task.Run(() => EmailSender.EnviarMail(emailSenderData));
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

        public void EnviarMailCertificacionAutomatica(string nroOC, string nroSolp, string mensaje, IEnumerable<string> destinatarios)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(destinatarios),
                Asunto = $"Certificación automática. OC: {nroOC} - SOLP: {nroSolp}",
                Cuerpo = mensaje
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailAprobacion(MailAprobacionESRequest request)
        {
            try
            {
                var baseURL = ConfigurationManager.AppSettings["SpaUrl"];
                var asunto = "Aprobación de servicio - Certificaciones: ";
                var adjuntosMail = new List<EmailAttachment>();
                var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_APROBACIONES_EXT);
                var cuerpoTemplatePosiciones = File.ReadAllText(TEMPLATE_NOTIFICACION_APROBACIONES_EXT_POSICION);
                var contenidoHtmlPosiciones = string.Empty;

                foreach (var posicion in request.Posiciones)
                {
                    var certificacionNro = posicion.Aprobacion.NRO_ES_LOCAL;
                    var reporteMemStream = new MemoryStream();
                    try
                    {
                        var reporteES = GetReportES(certificacionNro).ConfigureAwait(false).GetAwaiter().GetResult();
                        reporteES.CopyTo(reporteMemStream);
                    }
                    catch (Exception ex)
                    {
                        Log.AzureError(ex);
                        Log.Error("Error al obtener archivo para " + certificacionNro, ex);
                    }

                    var proveedorRazonSocial = posicion.ProveedorRazonSocial ?? string.Empty;
                    var usuario = posicion.Aprobacion.Ingresante_CDS;
                    var fechaCarga = posicion.Aprobacion.Fecha_Carga_ES ?? DateTime.Now;
                    var fechaCertificacion = fechaCarga.ToString("dd/MM/yyyy");
                    var servicioDescripcion = posicion.Aprobacion.Texto_breve_servicio;
                    var importe = posicion.Reportes[0].Moneda == "ARP" ?
                        "$ " + Convert.ToDecimal(posicion.Aprobacion.Monto_total).ToString("N2", CultureInfo.GetCultureInfo("en-US")) :
                        posicion.Reportes[0].Moneda + " " + Convert.ToDecimal(posicion.Aprobacion.Monto_total).ToString("N2", CultureInfo.GetCultureInfo("en-US"));
                    var ordenCompraNro = posicion.Aprobacion.NRO_OC;
                    var tabla = GenerarTablaAprobaciones(posicion.Reportes);
                    var nroPosicion = posicion.Aprobacion.NRO_POS;

                    asunto += $"{certificacionNro}, {proveedorRazonSocial}, {posicion.Aprobacion.Texto_breve_servicio}. ";

                    var approvalURL = "\"" + $"{baseURL}/aprobacion-externa/approve/{certificacionNro}&{request.UsuarioId}" + "\"";
                    var rejectURL = "\"" + $"{baseURL}/aprobacion-externa/reject/{certificacionNro}&{request.UsuarioId}" + "\"";

                    var cuerpoPosicion = string.Format(cuerpoTemplatePosiciones, proveedorRazonSocial, request.UsuarioId, certificacionNro, fechaCertificacion, servicioDescripcion,
                        importe, tabla, approvalURL, rejectURL, ordenCompraNro, nroPosicion);

                    contenidoHtmlPosiciones += cuerpoPosicion;

                    if (reporteMemStream.Length > 0)
                    {
                        adjuntosMail.Add(new EmailAttachment(reporteMemStream, $"Reporte_{certificacionNro}.pdf"));
                    }

                    foreach (var adjuntoPosicion in posicion.Adjuntos)
                    {
                        var adjuntoMemStream = azureService.ObtenerArchivoBlobStorageAsync(adjuntoPosicion.NombreEnBlob, "certificaciones").ConfigureAwait(false).GetAwaiter().GetResult();
                        adjuntoMemStream.Position = 0;

                        if (adjuntoMemStream.Length > 0)
                        {
                            adjuntosMail.Add(new EmailAttachment(adjuntoMemStream, adjuntoPosicion.NombreArchivo));
                        }
                    }
                }

                var urlOperaciones = "\"" + baseURL + "\"";
                var cuerpo = string.Format(cuerpoTemplate, contenidoHtmlPosiciones, urlOperaciones);

                var emailSenderData = new EmailSenderData
                {
                    Mails = emailService.ObtenerListaDestinatarios(new[] { request.DestinatarioMail }),
                    Asunto = asunto,
                    Cuerpo = cuerpo,
                    Adjuntos = adjuntosMail
                };
                emailService.EnviarMail(emailSenderData);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
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

            var montoTotal = GetImporte(emailDetail.MontoTotal);

            object[] subjectArgs = { emailDetail.NumeroCertificacion, emailDetail.MotivoRechazo,
            emailDetail.Proveedor, emailDetail.GeneradoPor, emailDetail.NumeroCertificacion, emailDetail.FechaCertificacion,
            emailDetail.Descripcion, montoTotal, emailDetail.NroOC};

            return BuildEmail(emailDetail, bodyTemplate, subjectFormat, subjectArgs);
        }

        private (List<string>, string, string) BuildApprovedEmail(EmailDetailCertificateDto emailDetail, string bodyTemplate)
        {
            string subjectFormat = "Asunto: Aceptación de servicio - Certificación nro {0}";

            var importe = GetImporte(emailDetail.MontoTotal);

            object[] subjectArgs = { emailDetail.NumeroCertificacion , emailDetail.NumeroCertificacion , emailDetail.NroOC, emailDetail.NumeroPosicion
                    , emailDetail.FechaCertificacion, emailDetail.Proveedor, emailDetail.Descripcion, importe, emailDetail.Aprobador};

            return BuildEmail(emailDetail, bodyTemplate, subjectFormat, subjectArgs);
        }

        private string GetImporte(string importe)
        {
            string pattern = @"([^\d]+)\s*([\d,]+(?:\.\d+)?)";

            Match match = Regex.Match(importe, pattern);
            if (match.Success)
            {
                string currency = match.Groups[1].Value;
                string numberString = match.Groups[2].Value;
                string formattedNumberString = numberString.Replace(',', '.');
                if (decimal.TryParse(formattedNumberString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
                {
                    // Comprobar si ya está bien formado
                    string formattedImporte = currency + " " + number.ToString("N2", CultureInfo.GetCultureInfo("en-US"));

                    if (importe.Trim() == formattedImporte.Trim())
                    {
                        return importe;
                    }

                    return formattedImporte;
                }
            }

            return importe;
        }

        private string BuildDailyNotification(List<NotificacionEsPendientesDiariasDto> aprobaciones, string bodyTemplate)
        {
            var bodyTable = BuildTableDailyNotification(aprobaciones);

            var body = string.Format(bodyTemplate, bodyTable.ToString());

            return body;
        }

        private StringBuilder BuildTableDetailES(EmailDetailCertificateDto emailDetailCertificateDto)
        {
            StringBuilder bodyTable = new StringBuilder();

            var moneda = emailDetailCertificateDto.Moneda == "ARP" ? "$ " : emailDetailCertificateDto.Moneda + " ";

            var montoTotal = GetImporte(emailDetailCertificateDto.MontoTotal);


            foreach (var servicio in emailDetailCertificateDto.DetalleServicio)
            {
                var monto = GetImporte(servicio.Monto);

                bodyTable.Append("<tr>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Descripcion}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Cantidad}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.UM}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Porcentaje}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{monto}</td>");
                bodyTable.Append("</tr>");
            }

            bodyTable.Append("<tr>");
            bodyTable.Append($"<td colspan='4' style='padding: 10px; border: 0px;'></td>");
            bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'><strong>{montoTotal}</strong></td>");
            bodyTable.Append("</tr>");

            return bodyTable;
        }

        private StringBuilder BuildTableDailyNotification(List<NotificacionEsPendientesDiariasDto> aprobaciones)
        {
            StringBuilder bodyTable = new StringBuilder();

            foreach (var item in aprobaciones)
            {
                var moneda = "$ ";

                bodyTable.Append("<tr>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.NRO_ES_LOCAL}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.Proveedor}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.NRO_OC}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.Texto_breve_servicio}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.Cantidad_a_certificar}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.UM}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{item.Porcentaje_a_certificar}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{moneda}{" "}{((decimal)item.Monto_a_certificar).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");
                bodyTable.Append("</tr>");
            }

            return bodyTable;
        }

        private static StringBuilder GenerarTablaAprobaciones(List<ReporteDto> reports)
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
                $"<td style='padding: 10px; border: 1px solid #333;'><strong>{moneda} {montoTotal.ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</strong></td>" +
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
