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
        
        private static readonly string DireccionMailAlimentacionAnimal = ConfigurationManager.AppSettings["EmailToComercialesAlimAnimal"];
        private static readonly string DireccionMailAuditoriaOrdenesVencidas = ConfigurationManager.AppSettings["EmailToAuditoriaOrdenesVencidas"];
        private static readonly string DireccionMailCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
        private static readonly string DireccionMailComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
        private static readonly string DireccionMailGestionAltaCuit = ConfigurationManager.AppSettings["EmailToGestionAltaCuit"];
        private static readonly string DireccionMailGestionAltaCuitCopia = ConfigurationManager.AppSettings["CopiaEmailToGestionAltaCuit"];
        private static readonly string DireccionMailMesaEntrSanLorenzo = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
        private static readonly string DireccionMailMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

        public void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = GenerarAsunto("ALTA CUIT INTERMEDIARIO FLETE"),
                Cuerpo = $"Razón social: {razonSocial}, CUIT: {cuit}"
            };
            EmailSender.EnviarMail(emailSenderData);
        }

        public void EnviarMailAltaTempranaCuit(string cuit, string razonSocial)
        {
            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailGestionAltaCuit, DireccionMailGestionAltaCuitCopia }),
                Asunto = GenerarAsunto("ALTA TEMPRANA CUIT"),
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = GenerarAsunto($"Faltan cargar los Km en el contrato, Orden de carga N° {ordenDeCarga.Id}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = GenerarAsunto($"Contrato Vencido - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { ordenDeCarga.Cliente.Mail, DireccionMailComerciales, DireccionMailMesaVentaFas }),
                Asunto = GenerarAsunto($"Molinos Agro - Notificación de orden vencida - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
        }

        public void EnviarMailTransporteNoExiste(OrdenDeCarga ordenDeCarga)
        {
            var cuerpo = $"Razón social: {ordenDeCarga.RazonSocialTransporte} <br> CUIT: {ordenDeCarga.CUITTransporte}";

            var emailSenderData = new EmailSenderData
            {
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailMesaEntrSanLorenzo }),
                Asunto = GenerarAsunto("ALTA TTE"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailMesaVentaFas, DireccionMailCobranzas }),
                Asunto = GenerarAsunto($"Orden de carga #{ordenDeCarga.Id}  Pedido Bloqueado {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailAlimentacionAnimal, DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = GenerarAsunto($"Varias facturas pendientes - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailMesaVentaFas, DireccionMailComerciales }),
                Asunto = GenerarAsunto($"Varios ctto pendientes - {ordenDeCarga.Cliente.RazonSocial}"),
                Cuerpo = cuerpo
            };
            EnviarMail(emailSenderData);
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
                Mails = ObtenerListaDestinatarios(new string[] { DireccionMailComerciales, DireccionMailAuditoriaOrdenesVencidas }),
                Asunto = GenerarAsunto($"Molinos Agro - Notificación de órdenes vencidas"),
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
    }
}
