using SustitucionMOAUtils.Interfaces;
using System.Configuration;
using SustitucionMOAUtils.Email;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using System.Collections.Generic;
using System;
using System.Text;
using SustitucionMOAModel.Util;
using System.Linq;
using System.IO;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasonService : IEmailFasonService
    {
        private static readonly string TEMPLATE_NOTIFICACION_AUTORIZACION_NOMINA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesAutorizacionNomina.html");

        private static readonly string DireccionToAltaTransporteCuitFason = ConfigurationManager.AppSettings["EmailAltaTransporteFasonTo"];
        private static readonly string DireccionCCAltaTransporteCuitFason = ConfigurationManager.AppSettings["EmailAltaTransporteFasonCC"];
        
        private static readonly string DireccionComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        private static readonly string DireccionMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
        private static readonly string DireccionAuditoriaOrdenesVencidas = ConfigurationManager.AppSettings["EmailToAuditoriaOrdenesVencidas"];
        private static readonly string DireccionToAutorizacionNomina = ConfigurationManager.AppSettings["EmailAutorizacionNominaTo"];
        private static readonly string DireccionToAutorizacionNominaInternoMoa = ConfigurationManager.AppSettings["EmailAutorizacionNominaInternoTo"];

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
        
        public void EnviarMailIntentoEdicionActiva(OrdenDeCargaFason orden, EditarOrdenDeCargaFasonRequest request)
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
        
        public void EnviarMailCamionAutorizadoEnVariasOrdenes(string patenteChasis, List<string> cuitsClientesOrdenes)
        {
            var cuerpo = $"El camión {patenteChasis} se encuentra autorizado en órdenes fason pendientes de las siguientes CUITs: {String.Join(", ", cuitsClientesOrdenes)}.";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionComerciales, DireccionMesaVentaFas }),
                Asunto = $"Camión {patenteChasis} autorizado en varias órdenes fason pendientes",
                Cuerpo = cuerpo
            };

            emailService.EnviarMail(emailSenderData);
        }
        
        public void EnviarMailVencieronOrdenesDeCarga(List<OrdenDeCargaFason> ordenes)
        {
            var tablaOrdenes = "";
            string descripcion;
            if (ordenes.Count > 0)
            {
                descripcion = $"Se informa que el día {DateTime.Now} se han vencido las siguientes órdenes de carga fason:";
                tablaOrdenes = CrearTablaDetalleOrden(ordenes);
            }
            else
            {
                descripcion = $"Se informa que para el día {DateTime.Now} no hay órdenes de carga fason vencidas";
            }
            var cuerpo = CrearCuerpoMail(descripcion, tablaOrdenes);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionComerciales, DireccionAuditoriaOrdenesVencidas }),
                Asunto = $"Molinos Agro - Notificación de órdenes fason vencidas",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }
        
        public void EnviarMailNotificacionEdicion(OrdenDeCargaFason orden, List<Variance> listaValoresDiferentes)
        {
            var descripcion = $"Se informa que el día {DateTime.Now} se han realizado las siguientes modificaciones para la orden fason {orden.Id}:";
            var tablaCambios = CrearTablaCambios(listaValoresDiferentes);
            var cuerpo = CrearCuerpoMail(descripcion, tablaCambios);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionComerciales, DireccionMesaVentaFas }),
                Asunto = $"Molinos Agro - Edición en su orden fason n°: {orden.Id}, {orden.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailAutorizacionDeNomina(IEnumerable<OrdenDeCargaFason> ordenes, bool esEdicionDeOrden)
        {
            if (!(ordenes?.Any() ?? false))
            {
                return;
            }

            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_AUTORIZACION_NOMINA);
            var tablaOrdenes = GenerarTablaOrdenesAutorizacionDeNomina(ordenes);
            var cuerpo = string.Format(cuerpoTemplate, ordenes.First().Observacion, tablaOrdenes);
            var destinatarios = esEdicionDeOrden ? DireccionToAutorizacionNominaInternoMoa : DireccionToAutorizacionNomina;

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { destinatarios }),
                Asunto = "Nómina de carga por cuenta de Molinos Agro SA" + (esEdicionDeOrden ? " - Orden modificada" : ""),
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
                "<p> Saludos,</p>" +
                "<p> Moa Operaciones </p>" +
                "</body>\r\n</html>";
            return cuerpo;
        }

        private string CrearTablaDetalleOrden(OrdenDeCargaFason orden)
        {
            var detalleCorredor = orden.Corredor != null ? orden.Corredor.CUIT + " - " + orden.Corredor.RazonSocial : "---";
            return InicioTablaDetalle() + CrearFilaTablaDetalleOrden(
                ordenId: orden.Id,
                cliente: $"{orden.Cliente.CUIT} - {orden.Cliente.RazonSocial}",
                corredor: $"{detalleCorredor}",
                chofer: $"{orden.CUILChofer} - {orden.ApellidoChofer}, {orden.NombreChofer}",
                transporte: $"{orden.CUITTransporte} - {orden.RazonSocialTransporte}",
                patenteChasis: orden.PatenteChasis,
                patenteAcoplado: orden.PatenteAcoplado,
                fecha: orden.FechaCreacion.ToString("dd/MM/yyyy")
                ) + FinalTabla();
        }
        
        private string CrearTablaDetalleOrden(List<OrdenDeCargaFason> ordenes)
        {
            var tablaBuilder = new StringBuilder();
            tablaBuilder.Append(InicioTablaDetalle());
            foreach (var orden in ordenes)
            {
                var detalleCorredor = orden.Corredor != null ? orden.Corredor.CUIT + " - " + orden.Corredor.RazonSocial : "---";
                tablaBuilder.Append(CrearFilaTablaDetalleOrden(
                    ordenId: orden.Id,
                    cliente: $"{orden.Cliente.CUIT} - {orden.Cliente.RazonSocial}",
                    corredor: $"{detalleCorredor}",
                    chofer: $"{orden.CUILChofer} - {orden.ApellidoChofer}, {orden.NombreChofer}",
                    transporte: $"{orden.CUITTransporte} - {orden.RazonSocialTransporte}",
                    patenteChasis: orden.PatenteChasis,
                    patenteAcoplado: orden.PatenteAcoplado,
                    fecha: orden.FechaCreacion.ToString("dd/MM/yyyy")
                    ));
            }
            tablaBuilder.Append(FinalTabla());
            return tablaBuilder.ToString();
        }
        
        private string CrearTablaDetalleOrden(OrdenDeCargaFason orden, EditarOrdenDeCargaFasonRequest request)
        {
            var detalleCorredor = orden.Corredor != null ? orden.Corredor.CUIT + " - " + orden.Corredor.RazonSocial : "---";
            return InicioTablaDetalle() + CrearFilaTablaDetalleOrden(
                ordenId: orden.Id,
                cliente: $"{orden.Cliente.CUIT} - {orden.Cliente.RazonSocial}",
                corredor: $"{detalleCorredor}",
                chofer: $"{request.UnidadTransporte.CUILChofer} - {request.UnidadTransporte.ApellidoChofer}, {request.UnidadTransporte.NombreChofer}",
                transporte: $"{request.UnidadTransporte.CUITTransporte} - {request.UnidadTransporte.RazonSocialTransporte}",
                patenteChasis: request.UnidadTransporte.PatenteChasis,
                patenteAcoplado: request.UnidadTransporte.PatenteAcoplado,
                fecha: orden.FechaCreacion.ToString("dd/MM/yyyy")
                ) + FinalTabla();
        }
        
        private string CrearFilaTablaDetalleOrden(
            long ordenId, string cliente, string corredor,
            string chofer, string transporte, string patenteChasis,
            string patenteAcoplado, string fecha)
        {
            return "<tr>" +
                $"<td>{ordenId}</td>" +
                $"<td>{cliente}</td>" +
                $"<td>{corredor}</td>" +
                $"<td>{chofer}</td>" +
                $"<td>{transporte}</td>" +
                $"<td>{patenteChasis}</td>" +
                $"<td>{patenteAcoplado}</td>" +
                $"<td>{fecha}</td>" +
                "</tr>";
        }
        
        private string InicioTablaDetalle()
        {
            return "<table cellspacing = \"5\" cellpadding = \"5\" border = \"3\">" +
              "<caption >Detalles</caption>" +
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
              "<tbody>";
        }
        
        private string FinalTabla()
        {
            return "</tbody>" +
            "</table>";
        }

        private string CrearTablaCambios(List<Variance> listaValoresDiferentes)
        {
            var tablaBuilder = new StringBuilder();
            var ahora = DateTime.Now;
            tablaBuilder.Append("<table cellspacing = \"5\" cellpadding = \"5\" border = \"3\">" +
              "<caption>Cambios</caption>" +
              "<thead style = \"background-color: #adacac;\">" +
              "<tr>" +
              "<td scope=\"col\">Nombre de la columna</td>" +
              "<td scope=\"col\">Antes del cambio</td>" +
              "<td scope=\"col\">Después del cambio</td>" +
              "<td scope=\"col\">Fecha</td>" +
              "</tr>" +
              "</thead>" +
              "<tbody>");

            // Filter out boolean values before processing
            var valoresNoBooleanos = listaValoresDiferentes.Where(diferencia =>
                !(diferencia.ValorAnterior is bool) && !(diferencia.ValorNuevo is bool)).ToList();
            foreach (var diferencia in valoresNoBooleanos)
            {
                tablaBuilder.Append("<tr>" +
                $"<td>{diferencia.PropertyName}</td>" +
                $"<td>{diferencia.ValorAnterior}</td>" +
                $"<td>{diferencia.ValorNuevo}</td>" +
                $"<td>{ahora}</td>" +
                "</tr>");
            }
            tablaBuilder.Append("</tbody></table>");
            return tablaBuilder.ToString();
        }

        private static StringBuilder GenerarTablaOrdenesAutorizacionDeNomina(IEnumerable<OrdenDeCargaFason> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{orden.Producto.Nombre}</td>" +
                    $"<td>{orden.Cliente.RazonSocial} ({orden.Cliente.CUIT})</td>" +
                    $"<td>{orden.PatenteChasis}</td>" +
                    $"<td>{orden.PatenteAcoplado}</td>" +
                    $"<td>{orden.NombreChofer} ({orden.CUILChofer})</td>" +
                    $"<td>{orden.RazonSocialTransporte} ({orden.CUITTransporte})</td>" +
                    $"<td>{orden.DestinoMercaderia}</td>" +
                    $"</tr>");
            }
            return ordenesStrBuilder;
        }
    }
}
