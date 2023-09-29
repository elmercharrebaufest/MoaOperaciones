using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System.Configuration;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasonService : EmailOrdenesCargaServiceBase, IEmailFasonService
    {
        private static readonly string DireccionToAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonTo"];
        private static readonly string DireccionCCAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonCC"];

        public void EnviarMailAltaTempranaCuit(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionToAltaTempranaCuitFason }),
                Copias = ObtenerListaDestinatarios(new string[] { DireccionCCAltaTempranaCuitFason }),
                Asunto = "ALTA TEMPRANA CUIT",
                Cuerpo = $"Se solicita el alta temprana del CUIT: {cuit}, Razón social: {razonSocial}"
            };
            EnviarMail(emailSenderData);
        }
    }
}
