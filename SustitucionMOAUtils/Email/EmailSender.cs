using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Email
{
    public class EmailSender
    {
        protected static string GenerarAsunto(string asunto)
        {
            var prefijoAsunto = ConfigurationManager.AppSettings["EmailAsuntoPrefijo"];
            return prefijoAsunto + asunto;
        }
        public static void send(ContactoContenido contactoContenido, byte[] file, string fileName)
        {

            if (!EmailConfig.getEmailHab())
            {
                return;
            }

            SmtpClient client = GetSmtpClient();
            var to = EmailConfig.getEmailAddTo();

            if (new string[] { "RETENCIONES", "ACTUALIZACIONES" }.Any(c => c.Equals(contactoContenido.categoria, StringComparison.OrdinalIgnoreCase))) to = EmailConfig.getEmailAddToDocumentacion();

            MailMessage mail = new MailMessage(EmailConfig.getEmailAddFrom(), to);
            mail.Subject = contactoContenido.categoria;
            mail.Body = BodyBuilder(contactoContenido);
            mail.ReplyToList.Add(contactoContenido.email);

            if (file.Length != 0)
            {
                try
                {
                    Stream fileStream = new MemoryStream(file);
                    Attachment attachment;
                    attachment = new Attachment(fileStream, fileName);
                    mail.Attachments.Add(attachment);
                }
                catch { }
            }

            SendMail(mail, client);
        }

        public static void sendFleteEmail(string nroProveedor, string nroFactura, string nroProforma, string importe, byte[] file, string fileName)
        {
            if (!EmailConfig.getEmailHab())
            {
                return;
            }

            SmtpClient client = GetSmtpClient();
            MailMessage mail = new MailMessage(EmailConfig.getEmailAddFrom(), EmailConfig.getEmailAddToFletes());
            mail.Subject = nroProveedor + "-" + nroFactura + "-" + nroProforma;
            mail.Body = BodyBuilderFletes(nroProveedor, nroFactura, nroProforma, importe);

            if (file.Length != 0)
            {
                try
                {
                    Stream fileStream = new MemoryStream(file);
                    Attachment attachment;
                    attachment = new Attachment(fileStream, fileName);
                    mail.Attachments.Add(attachment);
                }
                catch { }
            }

            SendMail(mail, client);

        }

        public static void SendReporte(ReporteBase reporte)
        {
            SmtpClient client = GetSmtpClient();
            var template = File.ReadAllText(reporte.Template);
            var cuerpo = string.Format(template, reporte.GetFecha(), reporte.GetBody());

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(EmailConfig.getEmailAddFrom()),
                Subject = reporte.Asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };

            if (reporte.Adjuntos != null)
            {
                foreach (Attachment attachment in reporte.Adjuntos)
                {
                    //attachment.ContentStream.Seek(0, SeekOrigin.Begin);
                    mail.Attachments.Add(attachment);
                    //attachment.ContentStream.Flush();
                }
            }

            //Mas de un destinatario
            if (reporte.Destinatario.Contains(","))
            {
                foreach (var destinatario in reporte.Destinatario.Split(','))
                {
                    if (EsCorreoValido(destinatario))
                    {
                        mail.To.Add(destinatario);
                    }
                }
            }
            //Solo un destinatario
            else if (EsCorreoValido(reporte.Destinatario))
            {
                mail.To.Add(reporte.Destinatario);
            }
            SendMail(mail, client);

        }

        private static SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient
            {
                Port = EmailConfig.getEmailPort(),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                //Credentials = new System.Net.NetworkCredential("moaoperaciones@molinosagro.com.ar","", ConfigurationManager.AppSettings["HostEmail"]),
                Host = EmailConfig.getEmailHost()
            };
            return client;
        }

        private static string BodyBuilder(ContactoContenido contactoContenido)
        {

            if (contactoContenido.camposAdicionales == "A")
            {
                return "Nro. Proveedor: " + contactoContenido.proveedor + "\n\n"
                        + "Razón Social: " + contactoContenido.razonSocial + "\n\n"
                        + "Contacto: " + contactoContenido.nombre + "\n\n"
                        + "Mail de Contacto: " + contactoContenido.email + "\n\n"
                        + "Teléfono: " + contactoContenido.telefono + "\n\n"
                        + "Nombre Vendedor: " + contactoContenido.nombreVendedor + "\n\n"
                        + "Contrato: " + contactoContenido.contrato + "\n\n"
                        + "CUIT: " + contactoContenido.cuit + "\n\n"
                        + "Comprobante: " + contactoContenido.comprobante + "\n\n"
                        + "Fecha Pago: " + contactoContenido.fechaPago + "\n\n"
                        + "Importe: " + contactoContenido.importe + "\n\n"
                        + "Impuesto: " + contactoContenido.impuesto + "\n\n"
                        + "Inscripción: " + contactoContenido.inscripcion + "\n\n"
                        + "Motivo: " + contactoContenido.motivo + "\n\n"
                        + "Reclamo: " + contactoContenido.comentario;
            }
            else
            {
                return "Nro. Proveedor: " + contactoContenido.proveedor + "\n\n"
                        + "Contacto: " + contactoContenido.nombre + "\n\n"
                        + "Mail de Contacto: " + contactoContenido.email + "\n\n"
                        + "Teléfono: " + contactoContenido.telefono + "\n\n"
                        + "Reclamo: " + contactoContenido.comentario;
            }

        }

        private static string BodyBuilderFletes(string nroProveedor, string nroFactura, string nroProforma, string importe)
        {
            return "Nro. de Proveedor: " + nroProveedor + "\n\n"
                    + "Nro. de Factura: " + nroFactura + "\n\n"
                    + "Nro. de Proforma: " + nroProforma + "\n\n"
                    + "Importe: $" + importe;
        }

        public static void EnviarMail(List<string> enviarA,
            string asunto,
            string cuerpo,
            List<string> copia = null,
            AlternateView vistaAlternativa = null,
            byte[] archivo = null,
            string nombreArchivo = null,
            string enviarDesde = null,
            List<string> copiaOculta = null,
            Dictionary<string, byte[]> archivos = null)
        {
            try
            {
                MailMessage oMensaje = new MailMessage
                {
                    From = new MailAddress(string.IsNullOrEmpty(enviarDesde) ? EmailConfig.getEmailAddFrom() : enviarDesde),
                    Body = cuerpo,
                    Subject = asunto,
                    IsBodyHtml = true,
                };
                foreach (string mail in enviarA)
                {
                    if (!string.IsNullOrEmpty(mail) && EsCorreoValido(mail))
                    {
                        oMensaje.To.Add(mail);
                    }
                }
                if (enviarA == null || !enviarA.Any())
                {
                    oMensaje.To.Add(EmailConfig.getEmailAddFrom());
                }

                if (copia != null)
                {
                    foreach (string mail in copia)
                    {
                        if (EsCorreoValido(mail))
                        {
                            oMensaje.CC.Add(mail);
                        }
                    }
                }
                if (copiaOculta != null)
                {
                    foreach (string mail in copiaOculta)
                    {
                        oMensaje.Bcc.Add(mail);
                    }
                }
                if (vistaAlternativa != null)
                {
                    oMensaje.AlternateViews.Add(vistaAlternativa);
                }

                oMensaje.BodyEncoding = Encoding.UTF8;
                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                if (archivo != null)
                {
                    Attachment data = new Attachment(new MemoryStream(archivo), nombreArchivo);
                    oMensaje.Attachments.Add(data);
                }

                if (archivos != null)
                {
                    foreach (var archi in archivos)
                    {
                        Attachment data = new Attachment(new MemoryStream(archi.Value), archi.Key);
                        oMensaje.Attachments.Add(data);
                    }

                }

                //if (archivo != null)
                //{
                //    using (var stream = new MemoryStream(archivo))
                //    {
                //        Attachment data = new Attachment(stream, nombreArchivo);
                //        oMensaje.Attachments.Add(data);
                //    }
                //}
                SmtpClient client = GetSmtpClient();
                SendMail(oMensaje, client);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public static void EnviarMail(EmailSenderData emailSenderData)
        {
            try
            {
                MailMessage oMensaje = new MailMessage
                {
                    From = new MailAddress(EmailConfig.getEmailAddFrom()),
                    Body = emailSenderData.Cuerpo,
                    Subject = emailSenderData.Asunto,
                    IsBodyHtml = true,
                };
                foreach (string mail in emailSenderData.Mails)
                {
                    if (!string.IsNullOrEmpty(mail) && EsCorreoValido(mail))
                    {
                        oMensaje.To.Add(mail);
                    }
#if DEBUG
                    if (!EsCorreoValido(mail))
                    {
                        Logger.Log.Info($"Correo no válido: {mail}, no se intenta continuar por ser ambiente DEBUG");
                        return;
                    }
#endif
                }
                if (emailSenderData.Mails == null || emailSenderData.Mails.Count() == 0)
                {
                    oMensaje.To.Add(EmailConfig.getEmailAddFrom());
                }

                if (emailSenderData.Copias != null)
                {
                    foreach (string copia in emailSenderData.Copias)
                    {
                        if (EsCorreoValido(copia))
                        {
                            oMensaje.CC.Add(copia);
                        }
#if DEBUG
                        if (!EsCorreoValido(copia))
                        {
                            Logger.Log.Info($"Correo no válido: {copia}, no se intenta continuar por ser ambiente DEBUG");
                            return;
                        }
#endif
                    }
                }
                if (emailSenderData.VistaAlternativa != null)
                {
                    oMensaje.AlternateViews.Add(emailSenderData.VistaAlternativa);
                }

                oMensaje.BodyEncoding = Encoding.UTF8;
                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                foreach (var adjunto in emailSenderData.Adjuntos)
                {
                    Attachment data = new Attachment(adjunto.Archivo, adjunto.Nombre);
                    oMensaje.Attachments.Add(data);
                }

                SmtpClient oCliente = GetSmtpClient();
                SendMail(oMensaje, oCliente);
            }
            catch (SmtpException ex)
            {
                Logger.Log.Info(ex.Message);
                if (ex.InnerException != null)
                {
                    Logger.Log.Info($"{ex.InnerException.Message}");
                }
                Logger.Log.Info("Stack: ");
                Logger.Log.Info(ex.StackTrace);
#if !DEBUG
                throw;
#endif
            }
            catch (Exception ex)
            {
                Logger.Log.Info(ex.Message);
                if (ex.InnerException != null)
                {
                    Logger.Log.Info($"{ex.InnerException.Message}");
                }
                Logger.Log.Info("Stack: ");
                Logger.Log.Info(ex.StackTrace);

                throw;
            }
        }

        public static async Task EnviarMailAsync(List<string> enviarA,
            string asunto,
            string cuerpo,
            List<string> copia = null,
            AlternateView vistaAlternativa = null,
            byte[] archivo = null,
            string nombreArchivo = null,
            string enviarDesde = null,
            List<string> copiaOculta = null,
            Dictionary<string, byte[]> archivos = null)
        {
            try
            {
                MailMessage oMensaje = new MailMessage
                {
                    From = new MailAddress(string.IsNullOrEmpty(enviarDesde) ? EmailConfig.getEmailAddFrom() : enviarDesde),
                    Body = cuerpo,
                    Subject = asunto,
                    IsBodyHtml = true,
                };

                enviarA.RemoveAll(correo => !EsCorreoValido(correo));
                copia.RemoveAll(correo => !EsCorreoValido(correo));

                foreach (string mail in enviarA)
                {
                    if (!string.IsNullOrEmpty(mail))
                    {
                        oMensaje.To.Add(mail);
                    }
                }
                if (enviarA == null || enviarA.Count() == 0)
                {
                    oMensaje.To.Add(EmailConfig.getEmailAddFrom());
                }

                if (copia != null)
                {
                    foreach (string mail in copia)
                    {
                        oMensaje.CC.Add(mail);
                    }
                }
                if (copiaOculta != null)
                {
                    foreach (string mail in copiaOculta)
                    {
                        oMensaje.Bcc.Add(mail);
                    }
                }
                if (vistaAlternativa != null)
                {
                    oMensaje.AlternateViews.Add(vistaAlternativa);
                }

                oMensaje.BodyEncoding = Encoding.UTF8;
                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                if (archivo != null)
                {
                    Attachment data = new Attachment(new MemoryStream(archivo), nombreArchivo);
                    oMensaje.Attachments.Add(data);
                }

                if (archivos != null)
                {
                    foreach (var archi in archivos)
                    {
                        Attachment data = new Attachment(new MemoryStream(archi.Value), archi.Key);
                        oMensaje.Attachments.Add(data);
                    }

                }

                //if (archivo != null)
                //{
                //    using (var stream = new MemoryStream(archivo))
                //    {
                //        Attachment data = new Attachment(stream, nombreArchivo);
                //        oMensaje.Attachments.Add(data);
                //    }
                //}
                SmtpClient oCliente = GetSmtpClient();
                await SendMailAsync(oMensaje, oCliente);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private static bool EsCorreoValido(string correo)
        {
            try
            {

                MailAddress mailAddress = new MailAddress(correo);
                Regex regex = new Regex(EmailConfig.getEmailRegexFormato());

                return regex.IsMatch(correo);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static async Task EnviarMailAsync(EmailSenderData emailSenderData)
        {
            try
            {
                MailMessage oMensaje = new MailMessage
                {
                    From = new MailAddress(EmailConfig.getEmailAddFrom()),
                    Body = emailSenderData.Cuerpo,
                    IsBodyHtml = true,
                };

                foreach (string mail in emailSenderData.Mails)
                {
                    if (!string.IsNullOrEmpty(mail) && EsCorreoValido(mail))
                    {
                        oMensaje.To.Add(mail);
                    }
                }

                if (emailSenderData.Mails == null || !emailSenderData.Mails.Any())
                {
                    oMensaje.To.Add(EmailConfig.getEmailAddFrom());
                }

                if (emailSenderData.Copias != null)
                {
                    foreach (string copia in emailSenderData.Copias)
                    {
                        if (!string.IsNullOrEmpty(copia) && EsCorreoValido(copia))
                        {
                            oMensaje.CC.Add(copia);
                        }
                    }
                }

                if (emailSenderData.VistaAlternativa != null)
                {
                    oMensaje.AlternateViews.Add(emailSenderData.VistaAlternativa);
                }

                oMensaje.BodyEncoding = Encoding.UTF8;
                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                foreach (EmailAttachment adjunto in emailSenderData.Adjuntos)
                {
                    Attachment data = new Attachment(adjunto.Archivo, adjunto.Nombre);
                    oMensaje.Attachments.Add(data);
                }

                oMensaje.Subject = emailSenderData.Asunto ?? "";

                SmtpClient oCliente = GetSmtpClient();

                // Enviar el correo de forma asíncrona
                await SendMailAsync(oMensaje, oCliente);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException);
                }
                Log.Error("Stack: " + ex.StackTrace, ex);

                throw;
            }
        }

        private static async Task SendMailAsync(MailMessage oMensaje, SmtpClient oCliente)
        {
            oMensaje.Subject = GenerarAsunto(oMensaje.Subject);
            LogMail(oMensaje);
            await oCliente.SendMailAsync(oMensaje);
        }

        private static void SendMail(MailMessage mail, SmtpClient client)
        {
            mail.Subject = GenerarAsunto(mail.Subject);
            LogMail(mail);
            client.Send(mail);
        }
        private static void LogMail(MailMessage mail)
        {
            try
            {
                Log.Info($"SendMail Subject: {mail.Subject}");
                Log.Info($"SendMail To: {mail.To}");
                Log.Info($"SendMail Cc: {mail.CC}");
                Log.Info($"SendMail Bcc: {mail.Bcc}");
                Log.Info($"SendMail BodyIsHtml: {mail.IsBodyHtml}");
                Log.Info($"SendMail Body: {mail.Body}");
                Log.Info($"SendMail HasAttachments: {mail.Attachments?.Count > 0}");
                Log.Info($"SendMail Attachments: {string.Join(",", mail.Attachments?.Select(a => a.Name).ToList())}");

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }

    public class EmailSenderData
    {
        public List<string> Mails { get; set; } = new List<string>();
        public string Asunto { get; set; }
        public string Cuerpo { get; set; }
        public List<string> Copias { get; set; } = null;
        public AlternateView VistaAlternativa { get; set; } = null;
        public IEnumerable<EmailAttachment> Adjuntos { get; set; } = Enumerable.Empty<EmailAttachment>();
    }

    public class EmailAttachment
    {
        public string Nombre { get; }
        public Stream Archivo { get; }

        public EmailAttachment(Stream archivo, string nombre)
        {
            Archivo = archivo;
            Archivo.Seek(0, SeekOrigin.Begin);
            Nombre = nombre;
        }
    }
}
