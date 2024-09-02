using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailAplicacionCPService : IEmailAplicacionCPService
    {
        private readonly IEmailService emailService;

        public EmailAplicacionCPService(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void EnviarMailAplicacionRechazada(string cartaPorte, string contrato, string motivo, string destinatario)
        {
            var asunto = "Aplicación de carta de porte rechazada";

            var cuerpo = $"Carta de porte: {cartaPorte}. <br> Contrato: {contrato}. <br> La aplicación ha sido rechazada: {motivo}";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { destinatario }),
                Asunto = asunto,
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }
    }
}
