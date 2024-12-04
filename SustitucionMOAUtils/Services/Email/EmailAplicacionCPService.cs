using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailAplicacionCPService : IEmailAplicacionCPService
    {
        private readonly string TEMPLATE_APLICACIONES_RECHAZADAS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AplicacionesCartaPorteRechazadas.html");

        private readonly IEmailService emailService;

        public EmailAplicacionCPService(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void EnviarMailAplicacionRechazada(IEnumerable<AplicacionCartaPorte> aplicaciones, string motivo, string destinatario)
        {
            var asunto = aplicaciones.Count() > 1 ? "Aplicaciones de carta de porte rechazadas" : "Aplicación de carta de porte rechazada";

            var mensaje = (aplicaciones.Count() > 1 ? "Las siguientes aplicaciones de carta de porte han sido rechazadas" : "La siguiente aplicación de carta de porte ha sido rechazada") +
                ". Motivo: " + motivo;

            var cuerpo = GenerarCuerpoConListadoAplicaciones(mensaje, aplicaciones);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { destinatario }),
                Asunto = asunto,
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        private string GenerarCuerpoConListadoAplicaciones(string mensaje, IEnumerable<AplicacionCartaPorte> aplicaciones)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_APLICACIONES_RECHAZADAS);
            var tablaAplicaciones = GenerarTablaAplicacionesANotificar(aplicaciones);
            var cuerpo = string.Format(cuerpoTemplate, mensaje, tablaAplicaciones);
            return cuerpo;
        }

        private StringBuilder GenerarTablaAplicacionesANotificar(IEnumerable<AplicacionCartaPorte> aplicaciones)
        {
            var aplicacionesStrBuilder = new StringBuilder();
            foreach (var aplicacion in aplicaciones)
            {
                aplicacionesStrBuilder.Append($"<tr>" +
                    $"<td>{aplicacion.CartaPorte}</td>" +
                    $"<td>{aplicacion.Contrato}</td>" +
                    $"</tr>");
            }
            return aplicacionesStrBuilder;
        }
    }
}
