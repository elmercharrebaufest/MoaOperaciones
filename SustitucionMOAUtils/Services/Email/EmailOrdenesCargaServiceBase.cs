using System.Configuration;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailOrdenesCargaServiceBase : EmailService, IEmailOrdenesCargaServiceBase
    {
        private static readonly string DireccionMailGestionAltaCuit = ConfigurationManager.AppSettings["EmailToGestionAltaCuit"];
        private static readonly string DireccionMailGestionAltaCuitCopia = ConfigurationManager.AppSettings["CopiaEmailToGestionAltaCuit"];
        public static readonly string DireccionMailComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        public static readonly string DireccionMailMesaEntrSanLorenzo = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
        public static readonly string DireccionMailMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = GenerarAsunto("ALTA CUIT INTERMEDIARIO FLETE"),
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }

        public void EnviarMailAltaTempranaCuit(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = GenerarAsunto("ALTA TEMPRANA CUIT"),
                Cuerpo = $"Se solicita el alta temprana del CUIT: {cuit}, Razón social: {razonSocial}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }
    }
}
