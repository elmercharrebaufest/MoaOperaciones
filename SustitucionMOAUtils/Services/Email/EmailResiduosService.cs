using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailResiduosService : IEmailResiduosService
    {
        private static readonly string DireccionToAltaTransporteCuitResiduos = ConfigurationManager.AppSettings["EmailAltaTransporteResiduosTo"];
        private static readonly string DireccionCCAltaTransporteCuitResiduos = ConfigurationManager.AppSettings["EmailAltaTransporteResiduosCC"];

        private readonly string DireccionMailAuditoriaOrdenesResiduosVencidas = ConfigurationManager.AppSettings["EmailOrdenesResiduosVencidas"];
        private readonly string DireccionMailIntentoEdicionAnulacionActiva = ConfigurationManager.AppSettings["DestinatariosEmailOrdenResiduosIntentoAnulacionActiva"];
        private readonly string TEMPLATE_NOTIFICACION_ORDENES_RESIDUOS = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesResiduos.html");
        
        private readonly IEmailService emailService;

        public EmailResiduosService(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void EnviarMailIntentoAnulacionOrdenActiva(OrdenResiduos orden)
        {
            var cuerpo = GenerarCuerpoConListadoOrdenes("Se ha intentado anular una orden de residuos e insumos activa.", "Orden:", new List<OrdenResiduos> { orden });

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailIntentoEdicionAnulacionActiva }),
                Asunto = $"Intento anulación orden residuos activa - Nro. orden: {orden.Id} - {orden.Cliente?.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailIntentoEdicionOrdenActiva(OrdenResiduos orden)
        {
            var cuerpo = GenerarCuerpoConListadoOrdenes("Se ha intentado editar una orden de residuos e insumos activa.", "Orden:", new List<OrdenResiduos> { orden });

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailIntentoEdicionAnulacionActiva }),
                Asunto = $"Intento edición orden residuos activa - Nro. orden: {orden.Id} - {orden.Cliente?.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailTransporteNoExiste(string razonSocialTransporte, string cuitTransporte)
        {
            var cuerpo = $"Razón social: {razonSocialTransporte} <br> CUIT: {cuitTransporte}";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTransporteCuitResiduos, DireccionCCAltaTransporteCuitResiduos }),
                Asunto = "ALTA TTE",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailOrdenesVencidas(IEnumerable<OrdenResiduos> ordenes)
        {
            string mensaje;
            var cabeceraTabla = string.Empty;

            if (ordenes.Any())
            {
                mensaje = $"Se informa que el día {DateTime.Now} se han vencido las siguientes órdenes de residuos:";
                cabeceraTabla = "Órdenes: ";
            }
            else
            {
                mensaje = $"Se informa que para el día {DateTime.Now} no hay órdenes de residuos vencidas";
            }
            var cuerpo = GenerarCuerpoConListadoOrdenes(mensaje, cabeceraTabla, ordenes);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailAuditoriaOrdenesResiduosVencidas }),
                Asunto = $"Molinos Agro - Notificación de órdenes residuos vencidas",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        private string GenerarCuerpoConListadoOrdenes(string mensaje, string cabeceraTabla, IEnumerable<OrdenResiduos> ordenes)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES_RESIDUOS);
            var tablaOrdenes = GenerarTablaOrdenesANotificar(ordenes);
            var cuerpo = string.Format(cuerpoTemplate, mensaje, cabeceraTabla, tablaOrdenes);
            return cuerpo;
        }

        private StringBuilder GenerarTablaOrdenesANotificar(IEnumerable<OrdenResiduos> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{orden.Id}</td>" +
                    $"<td>{orden.Cliente.RazonSocial}</td>" +
                    $"<td>{orden.ChoferApellido}, {orden.ChoferNombre}</td>" +
                    $"<td>{orden.PatenteChasis}</td>" +
                    $"<td>{orden.PatenteAcoplado}</td>" +
                    $"<td>{orden.FechaCreacion}</td>" +
                    $"</tr>");
            }
            return ordenesStrBuilder;
        }
    }
}
