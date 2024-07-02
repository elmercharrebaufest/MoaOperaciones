using DocumentFormat.OpenXml.Office2010.Excel;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
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

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailCertificationService : IEmailCertificationService
    {
        private static readonly string TEMPLATE_NOTIFICATION_CERTIFICATION_REJECTED = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacionRechazada.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_EXT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion.html");
        private static readonly string TEMPLATE_NOTIFICACION_APROBACIONES_PROVEEDOR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "CertificacionesPendientesDeAprobacion-Proveedor.html");

        private readonly IEmailService emailService;

        public EmailCertificationService(IEmailService emailService)
        {
            this.emailService = emailService;
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
                emailDetail.Descripcion, emailDetail.Importe};

            return BuildEmail(emailDetail, bodyTemplate, subjectFormat, subjectArgs);
        }

        private (List<string>, string, string) BuildApprovedEmail(EmailDetailCertificateDto emailDetail, string bodyTemplate)
        {
            string subjectFormat = "Asunto: Aceptación de servicio - Certificación nro {0}";
            object[] subjectArgs = { emailDetail.NumeroCertificacion, emailDetail.NumeroCertificacion, emailDetail.FechaCertificacion,
                emailDetail.Descripcion, emailDetail.Importe};

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

        public async Task SendAprobalProviderEmail(EmailDetailCertificateDto emailDetailCertificateDto)
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

            foreach (var servicio in emailDetailCertificateDto.DetalleServicio)
            {
                bodyTable.Append("<tr>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Descripcion}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Cantidad}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.UM}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Porcetaje}</td>");
                bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'>{servicio.Monto}</td>");
                bodyTable.Append("</tr>");
            }

            bodyTable.Append("<tr>");
            bodyTable.Append($"<td colspan='4' style='padding: 10px; border: 0px;'></td>");
            bodyTable.Append($"<td style='padding: 10px; border: 1px solid #333;'><strong>{emailDetailCertificateDto.MontoTotal}</strong></td>");
            bodyTable.Append("</tr>");

            return bodyTable;
        }

        public async Task EnviarMailAprobacion(List<Aprobaciones> apList, Proveedor prov, int userId, string destinatario)
        {
            try
            {
                string dateTimeFormat = "dd/MM/yyyy";
                List<string> dest = new List<string>();
                dest.Add(destinatario);

                string asunto = $" Aprobación de servicio - Certificación nro {apList[0].NRO_ES_LOCAL} ";


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
                    desc = apList[0].Descripcion_ES;
                    importe = "$ " + apList[0].Monto_total.ToString();
                    OC = apList[0].NRO_OC;
                    tabla = GenerarTablaAprobaciones(apList);
                    NroPosicion = apList[0].NRO_POS;
                }


                string baseURL = ConfigurationManager.AppSettings["SpaUrl"];
                string approvalURL = "\"" + baseURL + "/aprobacion-externa/approve/" + apList[0].NRO_ES_LOCAL + "&" + userId + "\"";
                string rejectURL = "\"" + baseURL + "/aprobacion-externa/reject/" + apList[0].NRO_ES_LOCAL + "&" + userId + "\"";

                var cuerpo = string.Format(cuerpoTemplate, proveedor, usuario, cert, FechaCert, desc, importe, tabla, approvalURL, rejectURL, OC, NroPosicion);


                var emailSenderData = new EmailSenderData()
                {
                    Mails = dest,
                    Asunto = asunto,
                    Cuerpo = cuerpo
                };


                Task emailSendTask = Task.Run(() => EmailSender.EnviarMail(emailSenderData));
                await emailSendTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private StringBuilder GenerarTablaAprobaciones(List<Aprobaciones> apList)
        {
            var aprStrBuilder = new StringBuilder();
            foreach (Aprobaciones ap in apList)
            {
                aprStrBuilder.Append($"<tr>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{(!string.IsNullOrEmpty(ap.Descripcion_ES) ? ap.Descripcion_ES : ap.Texto_breve_servicio)}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{(!string.IsNullOrEmpty(ap.Cantidad_a_certificar) ? ap.Cantidad_a_certificar : "0")}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{ap.UM}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>{ap.Porcentaje_a_certificar}</td>" +
                    $"<td style='padding: 10px; border: 1px solid #333;'>$ {ap.Monto_a_certificar}</td>" +
                    $"</tr>");
            }
            //Ultima fila - solo monto total
            aprStrBuilder.Append($"<tr>" +
                $"<td colspan='4' style='padding: 10px; border: 0px solid #333;'></td>" +
                $"<td style='padding: 10px; border: 1px solid #333;'><strong>$ {apList[0].Monto_total}</strong></td>" +
                $"</tr>");

            return aprStrBuilder;
        }
    }
}
