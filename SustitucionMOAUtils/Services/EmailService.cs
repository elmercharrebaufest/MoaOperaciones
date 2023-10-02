using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace SustitucionMOAUtils.Services
{
    public class EmailService: IEmailService
    {
        public EmailService()
        {

        }
        public List<string> ObtenerListaDestinatarios(IEnumerable<string> gruposDeMails)
        {
            var destinatarios = new List<string>();
            foreach (var grupo in gruposDeMails.Where(x => !string.IsNullOrEmpty(x)))
            {
                destinatarios.AddRange(grupo.Split(';'));
            }
            return destinatarios.Distinct().ToList();
        }

        // Este método es para poder mockear el envío de mail al testear
        public void EnviarMail(EmailSenderData emailSenderData)
        {
            EmailSender.EnviarMail(emailSenderData);
        }

        public void EnviarMail(List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null, string enviarDesde = null, List<string> copiaOculta = null, Dictionary<string, byte[]> archivos = null)
        {
            EmailSender.EnviarMail(enviarA,  asunto,  cuerpo,  copia ,  vistaAlternativa ,  archivo ,  nombreArchivo ,  enviarDesde,  copiaOculta ,archivos);
        }
    }
}
