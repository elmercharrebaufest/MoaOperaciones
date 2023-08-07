
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasonService : EmailFasService, IEmailFasonService
    {
        public new void EnviarMailAltaTempranaCuit(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] {
                    DireccionMailMesaVentaFas, DireccionMailMesaEntrSanLorenzo, DireccionMailComerciales }),
                Asunto = GenerarAsunto("ALTA TEMPRANA CUIT"),
                Cuerpo = $"Se solicita el alta temprana del CUIT: {cuit}, Razón social: {razonSocial}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }
    }
}
