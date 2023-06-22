using SustitucionMOAUtils.Email;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public abstract class EmailService
    {
        protected string GenerarAsunto(string asunto)
        {
            var prefijoAsunto = ConfigurationManager.AppSettings["EmailAsuntoPrefijo"];
            return prefijoAsunto + asunto;
        }

        protected List<string> ObtenerListaDestinatarios(IEnumerable<string> gruposDeMails)
        {
            var destinatarios = new List<string>();
            foreach (var grupo in gruposDeMails.Where(x => !string.IsNullOrEmpty(x)))
            {
                destinatarios.AddRange(grupo.Split(';'));
            }
            return destinatarios.Distinct().ToList();
        }

        // Este método es para poder mockear el envío de mail al testear
        protected virtual void EnviarMail(EmailSenderData emailSenderData)
        {
            EmailSender.EnviarMail(emailSenderData);
        }
    }
}
