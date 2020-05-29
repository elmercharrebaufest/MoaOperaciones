using System;
using System.IO;
using System.Net.Mail;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;

namespace SustitucionMOAUtils.Email
{
    public class EmailSender
    {
        public static void send(ContactoContenido contactoContenido, byte[] file, string fileName) {

            if (!EmailConfig.getEmailHab()) {
                return;
            }

            SmtpClient client = getSmtpClient();
            MailMessage mail = new MailMessage(EmailConfig.getEmailAddFrom(), EmailConfig.getEmailAddTo());
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
    }
}
