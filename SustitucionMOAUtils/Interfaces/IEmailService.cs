using SustitucionMOAUtils.Email;
using System.Collections.Generic;
using System.Net.Mail;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailService
    {
        List<string> ObtenerListaDestinatarios(IEnumerable<string> gruposDeMails);
        void EnviarMail(EmailSenderData emailSenderData);
        void EnviarMail(List<string> enviarA,
            string asunto,
            string cuerpo,
            List<string> copia = null,
            AlternateView vistaAlternativa = null,
            byte[] archivo = null,
            string nombreArchivo = null,
            string enviarDesde = null,
            List<string> copiaOculta = null,
            Dictionary<string, byte[]> archivos = null);
    }
}