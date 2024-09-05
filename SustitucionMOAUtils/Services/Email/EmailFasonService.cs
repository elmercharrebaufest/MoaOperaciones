using SustitucionMOAUtils.Interfaces;
using System.Configuration;
using SustitucionMOAUtils.Email;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using DocumentFormat.OpenXml.Wordprocessing;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using iTextSharp.tool.xml.html;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasonService : IEmailFasonService
    {
        private static readonly string DireccionToAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonTo"];
        private static readonly string DireccionCCAltaTempranaCuitFason = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasonCC"];

        private static readonly string DireccionToAltaTransporteCuitFason = ConfigurationManager.AppSettings["EmailAltaTransporteFasonTo"];
        private static readonly string DireccionCCAltaTransporteCuitFason = ConfigurationManager.AppSettings["EmailAltaTransporteFasonCC"];

        private static readonly string DireccionComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        private static readonly string DireccionMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

        private readonly IEmailService emailService;

        public EmailFasonService(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        public void EnviarMailAltaTempranaCuit(OrdenDeCargaFason orden, string ordenId, bool gestionaDestino, bool gestionaDestinatario)
        {
            string cuerpoDestinatario = gestionaDestinatario ? $"CUIT DESTINATARIO: {orden.CUITDestinatario}, Razón social: {orden.RazonSocialDestinatario}\n" : "";
            string cuerpoDestino = gestionaDestino ? $"CUIT DESTINO: {orden.CUITDestino}, Razón social: {orden.RazonSocialDestino}\n" : "";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTransporteCuitFason, DireccionCCAltaTransporteCuitFason }),
                Asunto = $"ALTA TEMPRANA CLIENTE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Se solicita el alta temprana de: \n" + cuerpoDestinatario + cuerpoDestino
            };
            emailService.EnviarMail(emailSenderData);
        }
        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTransporteCuitFason, DireccionCCAltaTransporteCuitFason }),
                Asunto = $"ALTA CUIT INTERMEDIARIO FLETE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailTransporteNoExiste(OrdenDeCargaFason ordenDeCarga)
        {
            var cuerpo = $"Razón social: {ordenDeCarga.RazonSocialTransporte} <br> CUIT: {ordenDeCarga.CUITTransporte}";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTransporteCuitFason, DireccionCCAltaTransporteCuitFason }),
                Asunto = "ALTA TTE",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }
        public void EnviarMailIntentoEdicionActiva(OrdenDeCargaFason orden, OrdenDeCargaFasonRequest request)
        {
            var tablaInformacionOrden = CrearTablaDetalleOrden(
                orden,
                request
            );
            var cuerpo = CrearCuerpoMail("Se ha intentado editar una orden de carga fason activa", tablaInformacionOrden);
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionComerciales, DireccionMesaVentaFas }),
                Asunto = "INTENTO EDICIÓN ACTIVA - NRO ORDEN: " + orden.Id,
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }
        public void EnviarMailIntentoAnulacionActiva(OrdenDeCargaFason orden)
        {
            var tablaInformacionOrden = CrearTablaDetalleOrden(orden);
            var cuerpo = CrearCuerpoMail("Se ha intentado anular una orden de carga fason activa", tablaInformacionOrden);
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionComerciales, DireccionMesaVentaFas }),
                Asunto = "INTENTO ANULACIÓN ACTIVA - NRO ORDEN: " + orden.Id,
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        private string CrearCuerpoMail(string texto, string contenido)
        {
            string cuerpo = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<meta name = \"viewport\" content = \"width=device-width, initial-scale=1\" >" +
                "</head>" +
                "<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">" +
                "<p> Buenos d&iacute;as,</p>" +
                "<br />" +
               (!string.IsNullOrEmpty(texto) ? $"{texto} <br />" : "") +
               (!string.IsNullOrEmpty(contenido) ? $"{contenido} <br />" : "") +
                "<p > Saludos,</p>" +
                "<p > Moa Operaciones </p>" +
                "</body>\r\n</html>";
            return cuerpo;
        }

        private string CrearTablaDetalleOrden(OrdenDeCargaFason orden)
        {
            var detalleCorredor = orden.Corredor != null ? orden.Corredor.CUIT + " - " + orden.Corredor.RazonSocial : "---";
            return CrearTablaDetalleOrden(
                ordenId: orden.Id,
                cliente: $"{orden.Cliente.CUIT} - {orden.Cliente.RazonSocial}",
                corredor: $"{detalleCorredor}",
                chofer: $"{orden.CUILChofer} - {orden.ApellidoChofer}, {orden.NombreChofer}",
                transporte: $"{orden.CUITTransporte} - {orden.RazonSocialTransporte}",
                patenteChasis: orden.PatenteChasis,
                patenteAcoplado: orden.PatenteAcoplado,
                fecha: orden.FechaCreacion.ToString("dd/MM/yyyy")
                );
        }
        private string CrearTablaDetalleOrden(OrdenDeCargaFason orden, OrdenDeCargaFasonRequest request)
        {
            var detalleCorredor = orden.Corredor != null ? orden.Corredor.CUIT + " - " + orden.Corredor.RazonSocial : "---";
            return CrearTablaDetalleOrden(
                ordenId: orden.Id,
                cliente: $"{orden.Cliente.CUIT} - {orden.Cliente.RazonSocial}",
                corredor: $"{detalleCorredor}",
                chofer: $"{request.CUILChofer} - {request.ApellidoChofer}, {request.NombreChofer}",
                transporte: $"{request.CUITTransporte} - {request.RazonSocialTransporte}",
                patenteChasis: request.PatenteChasis,
                patenteAcoplado: request.PatenteAcoplado,
                fecha: orden.FechaCreacion.ToString("dd/MM/yyyy")
                );
        }
        private string CrearTablaDetalleOrden(
            long ordenId, string cliente, string corredor,
            string chofer, string transporte, string patenteChasis,
            string patenteAcoplado, string fecha)
        {
            var cuerpo = "<table cellspacing = \"5\" cellpadding = \"5\" border = \"3\">" +
              "<caption >Detalle de orden FASON</caption>" +
              "<thead style = \"background-color: #adacac;\">" +
              "<tr>" +
              "<td scope=\"col\">Número de orden</td>" +
              "<td scope=\"col\">Cliente</td>" +
              "<td scope=\"col\">Corredor</td>" +
              "<td scope=\"col\">Chofer</td>" +
              "<td scope=\"col\">Transporte</td>" +
              "<td scope=\"col\">Patente Chasis</td>" +
              "<td scope=\"col\">Patente acoplado</td>" +
              "<td scope=\"col\">Fecha carga</td>" +
              "</tr>" +
              "</thead>" +
              "<tbody>" +
              "<tr>" +
              $"<td>{ordenId}</td>" +
              $"<td>{cliente}</td>" +
              $"<td>{corredor}</td>" +
              $"<td>{chofer}</td>" +
              $"<td>{transporte}</td>" +
              $"<td>{patenteChasis}</td>" +
              $"<td>{patenteAcoplado}</td>" +
              $"<td>{fecha}</td>" +
              "</tr>" +
              "</tbody>" +
              "</table>";
            return cuerpo;
        }
    }
}
