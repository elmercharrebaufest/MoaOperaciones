using SustitucionMOAUtils.Interfaces;
using System.Configuration;
using SustitucionMOAUtils.Email;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasonService : IEmailFasonService
    {
        private static readonly string DireccionToAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonTo"];
        private static readonly string DireccionCCAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonCC"];

        private readonly IEmailService emailService;

        public EmailFasonService(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        public void EnviarMailAltaTempranaCuit(OrdenDeCargaFason orden, string ordenId, bool gestionaDestino, bool gestionaDestinatario)
        {
            string cuerpoDestinatario = gestionaDestinatario ? $"CUIT DESTINATARIO: {orden.CUITDestinatario}, Razón social: {orden.RazonSocialDestinatario}\n" : "";
            string cuerpoDestino = gestionaDestino ? $"CUIT DESTINO: {orden.CUITDestino}, Razón social: {orden.RazonSocialDestino}\n" : "";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTempranaCuitFason, DireccionCCAltaTempranaCuitFason }),
                Asunto = $"ALTA TEMPRANA CLIENTE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Se solicita el alta temprana de: \n" + cuerpoDestinatario + cuerpoDestino
            };
            emailService.EnviarMail(emailSenderData);
        }
        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTempranaCuitFason, DireccionCCAltaTempranaCuitFason }),
                Asunto = $"ALTA CUIT INTERMEDIARIO FLETE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            emailService.EnviarMail(emailSenderData);
        }
    }
}
