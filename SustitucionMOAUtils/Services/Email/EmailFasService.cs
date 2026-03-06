using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailFasService : IEmailFasService
    {

        private static readonly string TEMPLATE_NOTIFICACION_ORDENES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesDeCarga.html");
        private static readonly string TEMPLATE_NOTIFICACION_SOLICITUD_EDICION_ORDEN = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoEdicionOrdenDeCarga.html");
        private static readonly string TEMPLATE_NOTIFICACION_AUTORIZACION_NOMINA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesAutorizacionNomina.html");

        private static readonly string DireccionMailAlimentacionAnimal = ConfigurationManager.AppSettings["EmailToComercialesAlimAnimal"];
        private static readonly string DireccionMailAuditoriaOrdenesVencidas = ConfigurationManager.AppSettings["EmailToAuditoriaOrdenesVencidas"];
        private static readonly string DireccionMailCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
        private static readonly string DireccionMailComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        private static readonly string DireccionMailMesaEntrSanLorenzo = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
        private static readonly string DireccionMailMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
        private static readonly string DireccionToAltaTempranaCuitFas = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasTo"];
        private static readonly string DireccionCCAltaTempranaCuitFas = ConfigurationManager.AppSettings["EmailAltaTempranaCuitFasCC"];
        private static readonly string DireccionToKgsMenos15TNFas = ConfigurationManager.AppSettings["EmailKgsMenos15TNFasTo"];
        private static readonly string DireccionToAutorizacionNomina = ConfigurationManager.AppSettings["EmailAutorizacionNominaTo"];
        private static readonly string DireccionToAutorizacionNominaInternoMoa = ConfigurationManager.AppSettings["EmailAutorizacionNominaInternoTo"];

        private readonly IEmailService emailService;

        public EmailFasService(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, int ordenId)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTempranaCuitFas, DireccionCCAltaTempranaCuitFas }),
                Asunto = $"ALTA CUIT INTERMEDIARIO FLETE - NRO ORDEN: {ordenId}",
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailAltaTempranaCuit(OrdenDeCarga ordenDeCarga, int ordenId, bool gestionaDestino, bool gestionaDestinatario)
        {
            string cuerpoDestinatario = gestionaDestinatario ? $"CUIT DESTINATARIO: {ordenDeCarga.CUITDestinatario}, Razón social: {ordenDeCarga.RazonSocialDestinatario}\n" : "";
            string cuerpoDestino = gestionaDestino ? $"CUIT DESTINO: {ordenDeCarga.CUITDestino}, Razón social: {ordenDeCarga.RazonSocialDestino}\n" : "";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToAltaTempranaCuitFas, DireccionCCAltaTempranaCuitFas }),
                Asunto = $"ALTA TEMPRANA CLIENTE SCATO - NRO ORDEN: {ordenId}",
                Cuerpo = $"Se solicita el alta temprana de: \n" + cuerpoDestinatario + cuerpoDestino
            };
            emailService.EnviarMail(emailSenderData);
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

        public void EnviarMailSolicitudAnulacionCamionEnPlanta(OrdenDeCarga orden)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = $"Se informa que el día {DateTime.Now} se ha intentado solicitar la anulación de una orden de carga estando el camión en planta:";
            var cabecera = "Orden:";
            var ordenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { orden });

            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now, orden.Id, ordenes, titulo, cabecera);

            var emailSenderData = new EmailSenderData()
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = $"Intento de solicitud de anulación. Orden de carga N° {orden.Id}",
                Cuerpo = cuerpo
            };

            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailSolicitudEdicionCamionEnPlanta(OrdenDeCarga orden)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var titulo = $"Se informa que el día {DateTime.Now} se ha intentado solicitar la edición de una orden de carga estando el camión en planta:";
            var cabecera = "Orden:";
            var ordenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { orden });

            var cuerpo = string.Format(cuerpoTemplate, DateTime.Now, orden.Id, ordenes, titulo, cabecera);

            var emailSenderData = new EmailSenderData()
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = $"Intento de solicitud de edición. Orden de carga N° {orden.Id}",
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

            var titulo = $"Se informa que la siguiente orden de carga no pasó las validaciones crediticias.";
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

            var descripcion = "El cliente ha seleccionado un pedido con menos de 15 tn disponibles.";
            var cabecera = "Orden: " + ordenDeCarga.Id;
            var tablaOrdenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailAlimentacionAnimal, DireccionToKgsMenos15TNFas, DireccionMailComerciales }),
                Asunto = $"Factura con menos de 15 tn - {ordenDeCarga.Cliente.RazonSocial}",
                Cuerpo = cuerpo
            };
            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailVariosContratos(OrdenDeCarga ordenDeCarga)
        {
            var cuerpoTemplate = File.ReadAllText(TEMPLATE_NOTIFICACION_ORDENES);

            var descripcion = "El cliente ha seleccionado un contrato con menos de 15 tn disponibles.";
            var cabecera = "Orden: " + ordenDeCarga.Id;
            var tablaOrdenes = GenerarTablaOrdenesANotificar(new OrdenDeCarga[] { ordenDeCarga });
            var cuerpo = string.Format(cuerpoTemplate, "", "", tablaOrdenes, descripcion, cabecera);

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionToKgsMenos15TNFas, DireccionMailComerciales }),
                Asunto = $"Contrato con menos de 15 tn - {ordenDeCarga.Cliente.RazonSocial}",
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
                    bool antesEsBool = string.Equals(cambio.Antes, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(cambio.Antes, "false", StringComparison.OrdinalIgnoreCase);
                    bool despuesEsBool = string.Equals(cambio.Despues, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(cambio.Despues, "false", StringComparison.OrdinalIgnoreCase);

                    if (antesEsBool || despuesEsBool)
                        continue;

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

        public void EnviarMailCamionAutorizadoEnVariasOrdenes(string patenteChasis, List<string> cuitsClientesOrdenes)
        {
            var cuerpo = $"El camión {patenteChasis} se encuentra autorizado en órdenes pendientes de las siguientes CUITs: {String.Join(", ", cuitsClientesOrdenes)}.";

            var emailSenderData = new EmailSenderData
            {
                Mails = emailService.ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = $"Camión {patenteChasis} autorizado en varias órdenes pendientes",
                Cuerpo = cuerpo
            };

            emailService.EnviarMail(emailSenderData);
        }

        public void EnviarMailAutorizacionDeNomina(IEnumerable<OrdenDeCarga> ordenes, bool esEdicionDeOrden)
        {
            if (!ordenes?.Any() ?? false)
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

        private static StringBuilder GenerarTablaOrdenesANotificar(IEnumerable<OrdenDeCarga> ordenes)
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

        private static StringBuilder GenerarTablaOrdenesAutorizacionDeNomina(IEnumerable<OrdenDeCarga> ordenes)
        {
            var ordenesStrBuilder = new StringBuilder();
            foreach (var orden in ordenes)
            {
                ordenesStrBuilder.Append($"<tr>" +
                    $"<td>{ orden.Producto.Nombre }</td>" +
                    $"<td>{ orden.Cliente.RazonSocial } ({ orden.CUITCliente })</td>" +
                    $"<td>{ orden.ChasisAcoplado }</td>" +
                    $"<td>{ orden.PatenteAcoplado }</td>" +
                    $"<td>{ orden.NombreChofer } ({ orden.CUITChofer })</td>" +
                    $"<td>{ orden.RazonSocialTransporte } ({ orden.CUITTransporte })</td>" +
                    $"<td>{ orden.DestinoMercaderia }</td>" +
                    $"</tr>");
            }
            return ordenesStrBuilder;
        }
    }
}
