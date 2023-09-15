using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasService : IEmailFasService
    {
        private readonly IEmailService emailService;


        private static readonly string TEMPLATE_NOTIFICACION_ORDENES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesDeCarga.html");
        private static readonly string TEMPLATE_NOTIFICACION_SOLICITUD_EDICION_ORDEN = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoEdicionOrdenDeCarga.html");

        private static readonly string DireccionMailAlimentacionAnimal = ConfigurationManager.AppSettings["EmailToComercialesAlimAnimal"];
        private static readonly string DireccionMailAuditoriaOrdenesVencidas = ConfigurationManager.AppSettings["EmailToAuditoriaOrdenesVencidas"];
        private static readonly string DireccionMailCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
        private static readonly string DireccionMailComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        private static readonly string DireccionMailGestionAltaCuit = ConfigurationManager.AppSettings["EmailToGestionAltaCuit"];
        private static readonly string DireccionMailGestionAltaCuitCopia = ConfigurationManager.AppSettings["CopiaEmailToGestionAltaCuit"];
        private static readonly string DireccionMailMesaEntrSanLorenzo = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
        private static readonly string DireccionMailMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = $"ALTA CUIT INTERMEDIARIO FLETE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }

        public void EnviarMailAltaTempranaCuit(List<GestionCuitDto> cuits)
        {
            var destinatario = cuits.FirstOrDefault(c => c.campo == "CUITDestinatario");
            var destino = cuits.FirstOrDefault(c => c.campo == "CUITDestino");

            string cuerpoDestinatario = destinatario != null ? $"CUIT DESTINATARIO: {destinatario.cuit}, Razón social: {destinatario.razonSocial}\n" : "";
            string cuerpoDestino = destino != null ? $"CUIT DESTINO: {destino.cuit}, Razón social: {destino.razonSocial}\n" : "";

            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = "ALTA TEMPRANA CUIT",
                Cuerpo = $"Se solicita el alta temprana del CUIT: {cuit}, Razón social: {razonSocial}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }

        public void EnviarMailContratoSinKm(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = $"Se informa que el día {DateTime.Now} el contrato de la siguiente ordenDeCarga no tiene los Km cargados:";
            var cabecera = "Orden: ";
            var ordenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", ordenes, titulo, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = $"Faltan cargar los Km en el contrato, Orden de carga N° {ordenDeCarga.Id}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = "Contrato vencido Nro :" + ordenDeCarga.ContratoIngresado;
            var cabecera = "Orden :";
            var ordenVencidas = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now, ordenDeCarga.Id, ordenVencidas, titulo, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = $"Contrato Vencido - {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailOrdenDeCargaVencida(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = $"Se informa que el día {DateTime.Now} se ha vencido la siguiente orden de carga:";
            var cabecera = "Orden: ";
            var tablaOrden = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now, ordenDeCarga.Id, tablaOrden, titulo, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { ordenDeCarga.Cliente.Mail, DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = $"Molinos Agro - Notificación de orden vencida - {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailTransporteNoExiste(OrdenDeCarga ordenDeCarga)
        {
            var cuerpo = $"Razón social: {ordenDeCarga.RazonSocialTransporte} <br> CUIT: {ordenDeCarga.CUITTransporte}";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailMesaEntrSanLorenzo }),
                Asunto = "ALTA TTE",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailValidacionesCrediticias(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = $"Se informa que la siguiente ordenDeCarga de carga no pasó las validaciones crediticias.";
            var cabecera = "Orden: ";
            var ordenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", ordenes, titulo, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas, DireccionMailCobranzas }),
                Asunto = $"Orden de carga #{ordenDeCarga.Id}  Pedido Bloqueado {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailVariasFacturasPendientes(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var descripcion = "Hay más de una factura para seleccionar.";
            var cabecera = "Orden: " + ordenDeCarga.Id;
            var tablaOrdenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailAlimentacionAnimal, DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = $"Varias facturas pendientes - {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailVariosContratos(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var descripcion = "Se encontraron varios contratos para el mismo cliente";
            var cabecera = "Orden: ";
            var tablaOrdenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = $"Varios ctto pendientes - {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailVencieronOrdenesDeCarga(List<OrdenDeCarga> ordenesDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);
            string descripcion;
            var tablaOrdenes = new StringBuilder(string.Empty);
            var cabecera = string.Empty;

            if (ordenesDeCarga.Count > 0)
            {
                descripcion = $"Se informa que el día {DateTime.Now} se han vencido las siguientes órdenes de carga:";
                cabecera = "Órdenes: ";
                tablaOrdenes = GenerarTablaOrdenesANotificar(ordenesDeCarga);
            }
            else
            {
                descripcion = $"Se informa que para el día {DateTime.Now} no hay órdenes de carga vencidas";
            }
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailAuditoriaOrdenesVencidas }),
                Asunto = $"Molinos Agro - Notificación de órdenes vencidas",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }


        private StringBuilder GenerarTablaOrdenesANotificar(IEnumerable<OrdenDeCarga> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{orden.Id}</td>" +
                    $"<td>{(!string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoSAP.Trim() : orden.ContratoIngresado)}</td>" +
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
        public void EnviarMailSolicitudEdicion(OrdenDeCarga ordenDeCarga, List<OrdenDeCargaCambiosHistorial> historialCambios)
        {
            try
            {
                if (historialCambios.Count == 0)
                {
                    return;
                }
                var destinatarios = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas });

                if (destinatarios.Count == 0)
                {
                    return;
                }
                var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_SOLICITUD_EDICION_ORDEN);

                var contrato = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                var numeroEntregaLabel = string.IsNullOrEmpty(ordenDeCarga.NumeroEntrega) ? "N/G" : ordenDeCarga.NumeroEntrega;
                var numeroPedidoLabel = string.IsNullOrEmpty(ordenDeCarga.NumeroPedido) ? "N/G" : ordenDeCarga.NumeroPedido;

                var cambios = new StringBuilder();
                foreach (var cambio in historialCambios
                    .OrderByDescending(x => x.NombreColumnaCambio == "ChasisAcoplado")
                    .ThenByDescending(x => x.NombreColumnaCambio == "PatenteAcoplado"))
                {
                    var nombreColumna = cambio.NombreColumnaCambio == "ChasisAcoplado" ? "PatenteChasis" : cambio.NombreColumnaCambio;
                    cambios.AppendLine($"<tr><td>{(nombreColumna)}</td><td>{cambio.Antes}</td><td>{cambio.Despues}</td><td>{cambio.FechaCambio}</td></tr>");
                }

                var cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenDeCarga.Id, numeroEntregaLabel, numeroPedidoLabel, cambios);
                var emailSenderData = new EmailSenderData()
                {
                    Mails = destinatarios,
                    Asunto = $"Molinos Agro - Edición en su orden de carga n°: {ordenDeCarga.Id}, {ordenDeCarga.Cliente.RazonSocial}, {contrato}",
                    Cuerpo = cuerpo
                };

                emailService.EnviarMail(emailSenderData);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public void EnviarMailSolicitudAnulacion(OrdenDeCarga orden)
        {

            var detallesOrden = new StringBuilder();
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            string titulo = $"Se informa que el día {DateTime.Now.ToString()} se ha solicitado la anulación de la siguiente orden de carga:";
            var cabecera = "Orden :";
            detallesOrden.Append(
                $"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>"
                );
            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, detallesOrden, titulo, cabecera);

            var emailSenderData = new EmailSenderData()
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = $"Solicitud de anulación, Orden de carga N° {orden.Id}",
                Cuerpo = cuerpo
            };

            emailService.EnviarMail(emailSenderData);
        }
    }
}
