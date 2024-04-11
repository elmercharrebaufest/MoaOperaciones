using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailResiduosService: IEmailResiduosService
    {

        private readonly string DireccionMailAuditoriaOrdenesResiduosVencidas = ConfigurationManager.AppSettings["EmailOrdenesResiduosVencidas"];
        private readonly string TEMPLATE_NOTIFICACION_ORDENES_VENCIDAS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesResiduosVencidas.html");

        protected IEmailService emailService { get; set; }
        public EmailResiduosService(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void EnviarMailOrdenesVencidas(IEnumerable<OrdenResiduos> ordenes)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES_VENCIDAS);
            string descripcion;
            var tablaOrdenes = new StringBuilder(string.Empty);
            var hoy = DateTime.Now;
            var cabecera = string.Empty;

            if (ordenes.Any())
            {
                descripcion = $"Se informa que el día {hoy} se han vencido las siguientes órdenes de residuos:";
                cabecera = "Órdenes: ";
                tablaOrdenes = GenerarTablaOrdenesANotificar(ordenes);
            }
            else
            {
                descripcion = $"Se informa que para el día {hoy} no hay órdenes de residuos vencidas";
            }
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailAuditoriaOrdenesResiduosVencidas }),
                Asunto = $"Molinos Agro - Notificación de órdenes residuos vencidas",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        private StringBuilder GenerarTablaOrdenesANotificar(IEnumerable<OrdenResiduos> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{orden.Id}</td>" +
                    $"<td>{orden.Cliente.RazonSocial}</td>" +
                    $"<td>{orden.ChoferNombre}</td>" +
                    $"<td>{orden.PatenteChasis}</td>" +
                    $"<td>{orden.PatenteAcoplado}</td>" +
                    $"<td>{orden.FechaCreacion}</td>" +
                    $"</tr>");
            }
            return ordenesStrBuilder;
        }
    }
}
