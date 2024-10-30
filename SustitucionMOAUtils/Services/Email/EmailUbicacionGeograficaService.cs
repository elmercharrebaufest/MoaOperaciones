using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailUbicacionGeograficaService : IEmailUbicacionGeograficaService
    {
        private static readonly string DireccionToDistanciaNoEncontrada = ConfigurationManager.AppSettings["EmailDistanciaNoEncontrada"];

        private readonly IEmailService emailService;

        public EmailUbicacionGeograficaService(IEmailService emailService)
        {
            this.emailService = emailService;
        }


        public void EnviarMailDistanciaNoEncontrada(string direccionDestino, string direccionABuscar)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToDistanciaNoEncontrada }),
                Asunto = "No se encontró distancia a la localidad " + direccionABuscar,
                Cuerpo = "No se pudo encontrar la distancia en kms hasta el punto de destino. La dirección ingresada es " + direccionDestino + ". Ingrese manualmente la distancia en la tabla DistanciaDomicilio y en las órdenes que tienen ese domicilio. Además, verificar si se debe insertar un nuevo reemplazo en la tabla DistanciaDomicilioReemplazos."
            };
            emailService.EnviarMail(emailSenderData);
        }
    }
}
