using System.Configuration;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services.Email
{
    public abstract class EmailOrdenesCargaServiceBase : EmailService, IEmailOrdenesCargaServiceBase
    {
        private static readonly string DireccionToAltaIntermediarioFlete = ConfigurationManager.AppSettings["EmailAltaIntermediarioFleteTo"];
        private static readonly string DireccionCCAltaIntermediarioFlete = ConfigurationManager.AppSettings["EmailAltaIntermediarioFleteCC"];

        public abstract void EnviarMailAltaTempranaCuit(string cuit, string razonSocial);

        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionToAltaIntermediarioFlete }),
                Copias = ObtenerListaDestinatarios(new string[] { DireccionCCAltaIntermediarioFlete }),
                Asunto = GenerarAsunto("ALTA CUIT INTERMEDIARIO FLETE"),
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            EnviarMail(emailSenderData);
        }
    }
}
