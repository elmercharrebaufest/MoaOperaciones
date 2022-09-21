using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoOrdenDeCarga
    {
        Pendiente,
        Confirmado,
        PendienteAprobacionCredito,
        EntregaGenerada,
        Anulada,
        Entregada,
        Vencida,
        EntregaPendiente,
        AnuladaPorVencimiento,
        ErrorDeCarga,
        EdicionSolicitada,
        AnulacionSolicitada,
        ContratoVencido, 
        EdicionRechazada,
        //TransporteNoExiste
    }

    public static class EstadoOrdenDeCargaExtensions
    {
        public static string ObtenerSemaforo(this EstadoOrdenDeCarga me)
        {
            switch (me)
            {
                case EstadoOrdenDeCarga.ErrorDeCarga:
                case EstadoOrdenDeCarga.AnuladaPorVencimiento:
                case EstadoOrdenDeCarga.Anulada:
                case EstadoOrdenDeCarga.EdicionRechazada:
                    return "red";
                case EstadoOrdenDeCarga.Pendiente:
                case EstadoOrdenDeCarga.Vencida:
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                    return "orange";
                case EstadoOrdenDeCarga.Confirmado:
                case EstadoOrdenDeCarga.EntregaPendiente:
                case EstadoOrdenDeCarga.ContratoVencido:
                //case EstadoOrdenDeCarga.TransporteNoExiste:
                case EstadoOrdenDeCarga.EdicionSolicitada:
                case EstadoOrdenDeCarga.AnulacionSolicitada:
                    return "yellow";
                case EstadoOrdenDeCarga.EntregaGenerada:
                case EstadoOrdenDeCarga.Entregada:
                    return "green";
                default:
                    return "white";
            }
        }

        public static string ToFriendlyString(this EstadoOrdenDeCarga me)
        {
            switch (me)
            {
                case EstadoOrdenDeCarga.ErrorDeCarga:
                    return "Error de datos";
                case EstadoOrdenDeCarga.Pendiente:
                    return "Pendiente";
                case EstadoOrdenDeCarga.Vencida:
                    return "Vencida";
                case EstadoOrdenDeCarga.Confirmado:
                    return "Confirmada";
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                    return "Pendiente aprobación crédito";
                case EstadoOrdenDeCarga.EntregaPendiente:
                    return "Entrega pendiente"; 
                case EstadoOrdenDeCarga.EntregaGenerada:
                    return "Entrega generada";
                case EstadoOrdenDeCarga.Entregada:
                    return "Entregada";
                case EstadoOrdenDeCarga.Anulada:
                    return "Anulada";
                case EstadoOrdenDeCarga.AnuladaPorVencimiento:
                    return "Anulada por vencimiento";
                case EstadoOrdenDeCarga.AnulacionSolicitada:
                    return "Anulación solicitada";
				case EstadoOrdenDeCarga.EdicionSolicitada:
                    return "Edición solicitada";
                case EstadoOrdenDeCarga.ContratoVencido:
                    return "Contrato vencido";
                case EstadoOrdenDeCarga.EdicionRechazada:
                    return "Edición rechazada";
                //case EstadoOrdenDeCarga.TransporteNoExiste:
                //    return "Transporte no existe";
                default:
                    return "Sin estado";
            }
        }

        public static string ToUserFriendlyString(this EstadoOrdenDeCarga me)
        {
            switch (me)
            {
                //case EstadoOrdenDeCarga.Vencida:
                //    return "Vencida";
                case EstadoOrdenDeCarga.ErrorDeCarga:
                case EstadoOrdenDeCarga.Pendiente:
                case EstadoOrdenDeCarga.Confirmado:
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                case EstadoOrdenDeCarga.EntregaPendiente:
                case EstadoOrdenDeCarga.ContratoVencido:
                //case EstadoOrdenDeCarga.TransporteNoExiste:
                    return "En proceso";
                case EstadoOrdenDeCarga.Vencida:
                case EstadoOrdenDeCarga.EntregaGenerada:
                    return "OK";
                case EstadoOrdenDeCarga.AnulacionSolicitada:
                    return "Anulación Solicitada";
                case EstadoOrdenDeCarga.EdicionSolicitada:
                    return "Edición Solicitada";
                //case EstadoOrdenDeCarga.Entregada:
                //    return "Completada";
                case EstadoOrdenDeCarga.Anulada:
                    return "Anulada";
                case EstadoOrdenDeCarga.AnuladaPorVencimiento:
                    return  "Anulada por vencimiento";
                case EstadoOrdenDeCarga.EdicionRechazada:
                    return "Edición rechazada";
                default:
                    return "Sin estado";
            }
        }
    }
}
