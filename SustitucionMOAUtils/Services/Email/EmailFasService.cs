using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasService : EmailService, IEmailFasService
    {
        private static readonly string TEMPLATE_NOTIFICACION_ORDENES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesDeCarga.html");

        public void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            
            var titulo = "Contrato vencido Nro :" + ordenDeCarga.ContratoIngresado;
            var cabecera = "Orden :";
            var ordenVencidas = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now, ordenDeCarga.Id, ordenVencidas, titulo, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { mailsComerciales, mailsMesaVentaFas }),
                Asunto = GenerarAsunto($"Contrato Vencido - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
        }

        public void EnviarMailVariasFacturasPendientes(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            //TODO: Pendiente de consultar la diferencia entre estos comerciales y los otros (para ponerle un buen nombre a EmailToComercialesFFAA)
            var mailsComercialesFFAA = ConfigurationManager.AppSettings["EmailToComercialesFFAA"];

            var descripcion = "Hay más de una factura para seleccionar.";
            var cabecera = "Orden: " + ordenDeCarga.Id;
            var tablaOrdenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { mailsComercialesFFAA, mailsMesaVentaFas }),
                Asunto = GenerarAsunto($"Varias facturas pendientes - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
        }

        private StringBuilder GenerarTablaOrdenesANotificar(IEnumerable<OrdenDeCarga> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{orden.Id}</td>" +
                    $"<td>{orden.ContratoIngresado}</td>" +
                    $"<td>{orden.Cliente.RazonSocial}</td>" +
                    $"<td>{orden.CodigoCorredor}</td>" +
                    $"<td>{orden.NombreChofer}</td>" +
                    $"<td>{orden.ChasisAcoplado}</td>" +
                    $"<td>{orden.PatenteAcoplado}</td>" +
                    $"<td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td>" +
                    $"<td>{orden.NumeroEntrega}</td>" +
                    $"<td>{orden.FechaCarga}</td>" +
                    $"<td>{orden.FechaVencimiento}</td>" +
                    $"</tr>");
            }
            return ordenesStrBuilder;
        }
    }
}
