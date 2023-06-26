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
        SinEnviarASAP,
        SinEstado,
        EntregaAnuladaPedidoPendienteAnulacion,
        PendienteCompensacion
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
                case EstadoOrdenDeCarga.ContratoVencido:
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                    return "orange";
                case EstadoOrdenDeCarga.Confirmado:
                case EstadoOrdenDeCarga.Vencida:
                case EstadoOrdenDeCarga.EntregaPendiente:
                case EstadoOrdenDeCarga.EdicionSolicitada:
                case EstadoOrdenDeCarga.AnulacionSolicitada:
                case EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion:
                    return "yellow";
                case EstadoOrdenDeCarga.EntregaGenerada:
                case EstadoOrdenDeCarga.Entregada:

                    return "green";
                case EstadoOrdenDeCarga.SinEnviarASAP:
                    return "bluesap";
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
                case EstadoOrdenDeCarga.SinEnviarASAP:
                    return "Sin Enviar a SAP";
                case EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion:
                    return "Entrega anulada, pedido pendiente de anulación";
                default:
                    return "Sin estado";
            }
        }



        public static string ToUserFriendlyString(this EstadoOrdenDeCarga me)
        {
            switch (me)
            {
                case EstadoOrdenDeCarga.ErrorDeCarga:
                case EstadoOrdenDeCarga.Pendiente:
                case EstadoOrdenDeCarga.Confirmado:
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                case EstadoOrdenDeCarga.EntregaPendiente:
                case EstadoOrdenDeCarga.ContratoVencido:
                case EstadoOrdenDeCarga.SinEnviarASAP:
                    return "En proceso";
                case EstadoOrdenDeCarga.Vencida:
                case EstadoOrdenDeCarga.EntregaGenerada:
                    return "OK";
                case EstadoOrdenDeCarga.AnulacionSolicitada:
                    return "Anulación Solicitada";
                case EstadoOrdenDeCarga.EdicionSolicitada:
                    return "Edición Solicitada";
                case EstadoOrdenDeCarga.Anulada:
                    return "Anulada";
                case EstadoOrdenDeCarga.AnuladaPorVencimiento:
                    return "Anulada por vencimiento";
                case EstadoOrdenDeCarga.EdicionRechazada:
                    return "Edición rechazada";
                case EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion:
                    return "Anulación en proceso";
                default:
                    return "Sin estado";
            }
        }

        public static EstadoOrdenDeCarga ObtenerDescripcionEstado(string estado)
        {

            switch (estado)
            {
                case "Error de datos":
                    return EstadoOrdenDeCarga.ErrorDeCarga;
                case "Pendiente":
                    return EstadoOrdenDeCarga.Pendiente;
                case "Vencida":
                    return EstadoOrdenDeCarga.Vencida;
                case "Confirmada":
                    return EstadoOrdenDeCarga.Confirmado;
                case "Pendiente aprobación crédito":
                    return EstadoOrdenDeCarga.PendienteAprobacionCredito;
                case "Entrega pendiente":
                    return EstadoOrdenDeCarga.EntregaPendiente;
                case "Entrega generada":
                    return EstadoOrdenDeCarga.EntregaGenerada;
                case "Entregada":
                    return EstadoOrdenDeCarga.Entregada;
                case "Anulada":
                    return EstadoOrdenDeCarga.Anulada;
                case "Anulada por vencimiento":
                    return EstadoOrdenDeCarga.AnuladaPorVencimiento;
                case "Anulación solicitada":
                    return EstadoOrdenDeCarga.AnulacionSolicitada;
                case "Edición solicitada":
                    return EstadoOrdenDeCarga.EdicionSolicitada;
                case "Contrato vencido":
                    return EstadoOrdenDeCarga.ContratoVencido;
                case "Edición rechazada":
                    return EstadoOrdenDeCarga.EdicionRechazada;
                case "Sin Enviar a SAP":
                    return EstadoOrdenDeCarga.SinEnviarASAP;
                case "Anulación en proceso":
                case "Entrega anulada, pedido pendiente de anulación":
                    return EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion;
                default:
                    return EstadoOrdenDeCarga.SinEstado;
            }
        }

    }
}
