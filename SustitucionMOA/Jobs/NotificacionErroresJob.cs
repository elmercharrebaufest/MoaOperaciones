using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Log = SustitucionMOAUtils.Logger.Log;

namespace SustitucionMOA.Jobs
{
    public interface INotificacionErroresJob : IHangfireJob { }
    public class NotificacionErroresJob : INotificacionErroresJob
    {
        private readonly IRepositorio repositorio;
        private readonly ILogTableService logTableService;
        private readonly IEmailService emailService;
        private readonly IHttpContextService httpContextService;

        public NotificacionErroresJob(IRepositorio repositorio, ILogTableService logTableService, IEmailService emailService, IHttpContextService httpContextService)
        {
            this.repositorio = repositorio;
            this.logTableService = logTableService;
            this.logTableService = logTableService;
            this.emailService = emailService;
            this.httpContextService = httpContextService;
        }

        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "NotificacionErroresJob");
                if (habilitacion == null || !habilitacion.Habilitado)
                {
                    return;
                }
                int minutos = int.Parse(repositorio.Obtener<Configuracion, string>(hj => hj.Code == "NotificacionErroresMinutos", x => x.Value));
                int cantidad = int.Parse(repositorio.Obtener<Configuracion, string>(hj => hj.Code == "NotificacionErroresCantidad", x => x.Value));
                List<string> enviarA = repositorio.Obtener<Configuracion, string>(hj => hj.Code == "NotificacionErroresEnviarA", x => x.Value).Replace(" ", "").Split(';').ToList();
                DateTime desde = DateTime.Now.AddMinutes(minutos * -1);

                var agrupado = logTableService.ObtenerLogs(desde, true)
                    .Where(x => x.Count >= cantidad).ToList();

                if (agrupado.Any())
                {
                    emailService.EnviarMail(enviarA, "Notificacion Errores", "", null, CuerpoEnviarMail(agrupado, minutos));

                }

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private AlternateView CuerpoEnviarMail(List<LogTableCountErrors> agrupado, int minutos)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();

            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();

            // Construcción del cuerpo del mail con el HTML
            string htmlBody = "<h2>Reporte de Errores</h2>";
            htmlBody += $"<p>A continuación se detalla la cantidad de errores agrupados por Logger y Level en los últimos {minutos} minutos:</p>";

            // Itera sobre la lista de errores agrupados
            foreach (var grupo in agrupado)
            {
                htmlBody += $"<h3>Logger: {grupo.Logger}, Level: {grupo.Level}, Cantidad: {grupo.Count}</h3>";
                htmlBody += "<ul>";

                // Detalles de cada error
                foreach (var error in grupo.Errors)
                {
                    htmlBody += $"<li><strong>Fecha:</strong> {error.Date} <br />";
                    htmlBody += $"<strong>Mensaje:</strong> {error.Message} <br />";
                    htmlBody += $"<strong>Excepción:</strong> {error.Exception} </li><br /><br />";
                }
                htmlBody += "</ul>";
            }

            htmlBody += "<br/><br/>";

            // Firma y despedida
            htmlBody += "En caso de tener alguna consulta, ingresar a <a href='http://www.moaoperaciones.com.ar'>www.moaoperaciones.com.ar</a> " +
                  "<br/><br/>Saludos Cordiales<br/>" +
                  "Molinos Agro S.A. <br/><br/>" +
                   @"<img width='15%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

    }
}