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
                Cuerpo = GenerarCuerpoMailDistanciaNoEncontrada(direccionDestino)
            };
            emailService.EnviarMail(emailSenderData);
        }

        private string GenerarCuerpoMailDistanciaNoEncontrada(string direccionDestino)
        {
            var cuerpo = new StringBuilder();

            cuerpo.AppendLine($"No se pudo encontrar la distancia en kms hasta el punto de destino. La dirección ingresada es {direccionDestino}. Ingrese manualmente la distancia en la tabla DistanciaDomicilio y en las órdenes que tienen ese domicilio. Además, verificar si se debe insertar un nuevo reemplazo en la tabla DistanciaDomicilioReemplazos.");
            cuerpo.AppendLine();
            cuerpo.AppendLine("Pasos para ingresar la distancia manualmente:");
            cuerpo.AppendLine("1) Medir la distancia en automóvil desde la planta de MOA hasta el destino, medida en kilómetros.");
            cuerpo.AppendLine("El origen es: https://maps.app.goo.gl/sbBXKAHGUByGgRZd6");
            cuerpo.AppendLine($"El destino para esta carga es: {direccionDestino}. (Buscar sólo con [Localidad], [Provincia], 'Argentina').");
            cuerpo.AppendLine("Registrar la distancia obtenida en kilómetros.");
            cuerpo.AppendLine();
            cuerpo.AppendLine("Verificar cuáles son los registros a actualizar, así además revisamos si hay algún otro destino al que le falte cargar la distancia. Podemos usar estas dos consultas. La primera muestra los domicilios para los que no se encontró distancia. Una vez que los actualicemos, cuando se cree una nueva orden que tenga ese mismo domicilio ya no se va a calcular la distancia, sino que se va a usar la que tengamos en esta tabla. La segunda consulta muestra todas las órdenes de carga que se crearon con los domicilios para los cuales no tenemos distancia. También deberemos actualizarlas.");
            cuerpo.AppendLine();
            cuerpo.AppendLine("SELECT * FROM DistanciaDomicilio DD WHERE DD.DistanciaKm is null ORDER BY DD.DomicilioDescripcion");
            cuerpo.AppendLine();
            cuerpo.AppendLine("SELECT DD.*, O.DomicilioDescr, O.KmsARecorrer FROM DistanciaDomicilio DD inner join OrdenDeCarga O on O.DomicilioDescr = DD.DomicilioDescripcion WHERE DD.DistanciaKm is null");
            cuerpo.AppendLine();
            cuerpo.AppendLine("2) Actualizar las distancias faltantes en la tabla DistanciaDomicilio. Se puede usar esta query como ejemplo, reemplazando los kilómetros calculados anteriormente.");
            cuerpo.AppendLine($"UPDATE DistanciaDomicilio SET DistanciaKm = 340 WHERE DomicilioDescripcion = '${direccionDestino}'");
            cuerpo.AppendLine();
            cuerpo.AppendLine("3) Actualizar las órdenes de carga que se crearon mientras no se tuvo distancias en la tabla. Se puede usar esta query como ejemplo, reemplazando los kilómetros calculados anteriormente.");
            cuerpo.AppendLine($"UPDATE OrdenDeCarga SET KmsARecorrer = 340 WHERE DomicilioDescr = '{direccionDestino}'");
            cuerpo.AppendLine();

            return cuerpo.ToString();
        }
    }
}
