using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;

namespace SustitucionMOAUtils.Email
{
    public class EmailSender
    {
        public static void send(ContactoContenido contactoContenido, byte[] file, string fileName) {

            if (!EmailConfig.getEmailHab()) {
                return;
            }

            SmtpClient client = getSmtpClient();
            var to = EmailConfig.getEmailAddTo();

            if (new string[] { "RETENCIONES", "ACTUALIZACIONES" }.Any(c => c.Equals(contactoContenido.categoria, StringComparison.OrdinalIgnoreCase))) to = EmailConfig.getEmailAddToDocumentacion();

            MailMessage mail = new MailMessage(EmailConfig.getEmailAddFrom(), to);
            mail.Subject = contactoContenido.categoria;
            mail.Body = bodyBuilder(contactoContenido);
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

            client.Send(mail);
        }


        public static void sendFleteEmail(string nroProveedor, string nroFactura, string nroProforma, string importe, byte[] file, string fileName)
        {
            if (!EmailConfig.getEmailHab())
            {
                return;
            }

            SmtpClient client = getSmtpClient();
            MailMessage mail = new MailMessage(EmailConfig.getEmailAddFrom(), EmailConfig.getEmailAddToFletes());
            mail.Subject = nroProveedor + "-" + nroFactura + "-" + nroProforma;
            mail.Body = bodyBuilderFletes(nroProveedor, nroFactura, nroProforma, importe);

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

            client.Send(mail);

        }

        public static void sendReporte(ReporteBase reporte)
        {
            SmtpClient client = getSmtpClient();
            var template = File.ReadAllText(reporte.Template);
            var cuerpo = string.Format(template, reporte.GetFecha(), reporte.GetBody());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(EmailConfig.getEmailAddFrom());
            mail.Subject = reporte.Asunto;
            mail.Body = cuerpo;
            mail.IsBodyHtml = true;

            //Mas de un destinatario
            if (reporte.Destinatario.Contains(",")){
                foreach (var destinatario in reporte.Destinatario.Split(','))
                {
                    mail.To.Add(destinatario);
                }
            }
            //Solo un destinatario
            else
            {
                mail.To.Add(reporte.Destinatario);
            }

            client.Send(mail);
        }

        private static SmtpClient getSmtpClient() {
            SmtpClient client = new SmtpClient();
            client.Port = EmailConfig.getEmailPort();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = EmailConfig.getEmailHost();
            return client;
        }

        private static string bodyBuilder(ContactoContenido contactoContenido) {

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
            else {
                return "Nro. Proveedor: " + contactoContenido.proveedor + "\n\n"
                        + "Contacto: " + contactoContenido.nombre + "\n\n"
                        + "Mail de Contacto: " + contactoContenido.email + "\n\n"
                        + "Teléfono: " + contactoContenido.telefono + "\n\n"
                        + "Reclamo: " + contactoContenido.comentario;
            }

        }

        private static string bodyBuilderFletes(string nroProveedor, string nroFactura, string nroProforma, string importe)
        {
            return "Nro. de Proveedor: " + nroProveedor + "\n\n"
                    + "Nro. de Factura: " + nroFactura + "\n\n"
                    + "Nro. de Proforma: " + nroProforma + "\n\n"
                    + "Importe: $" + importe;
        }

        public static void EnviarMail(List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null)
        {
            try
            {
                MailMessage oMensaje = new MailMessage
                {
                    From = new MailAddress(EmailConfig.getEmailAddFrom()),
                    Body = cuerpo,
                    Subject = asunto,
                    IsBodyHtml = true,
                };
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
                SmtpClient oCliente = getSmtpClient();
                oCliente.Send(oMensaje);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
      

    }
}
