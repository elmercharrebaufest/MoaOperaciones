namespace SustitucionMOAModel.Enums
{
    public enum EstadoOrdenDeCargaFason
    {
        Generada = 0,
        Pendiente = 1,
        Vencida = 2,
        Entregada = 3,
        SinEstado = 4,
        PendienteCompensacion = 5,
        PendienteContabilizacion = 6,
        //EdicionSolicitada = 7,
        //EdicionRechazada = 8,
        AnulacionSolicitada = 9,
        Anulada = 10,
    }

    public static class EstadoOrdenDeCargaFasonExtensions
    {
        public static string ToFriendlyStringInterno(this EstadoOrdenDeCargaFason me)
        {
            switch (me)
            {
                case EstadoOrdenDeCargaFason.Generada:
                    return "Orden generada";
                case EstadoOrdenDeCargaFason.Pendiente:
                    return "Pendiente";
                case EstadoOrdenDeCargaFason.Vencida:
                    return "Orden vencida";
                case EstadoOrdenDeCargaFason.Entregada:
                    return "Orden entregada";
                case EstadoOrdenDeCargaFason.PendienteCompensacion:
                    return "Pendiente compensación";
                case EstadoOrdenDeCargaFason.PendienteContabilizacion:
                    return "Pendiente contabilizar";
                case EstadoOrdenDeCargaFason.AnulacionSolicitada:
                    return "Anulación solicitada";
                case EstadoOrdenDeCargaFason.Anulada:
                    return "Anulada";
                default:
                    return "Sin estado";
            }
        }

        public static string ToFriendlyStringExterno(this EstadoOrdenDeCargaFason me)
        {
            switch (me)
            {
                case EstadoOrdenDeCargaFason.Generada:
                case EstadoOrdenDeCargaFason.PendienteContabilizacion:
                    return "OK";
                case EstadoOrdenDeCargaFason.Pendiente:
                case EstadoOrdenDeCargaFason.PendienteCompensacion:
                    return "En proceso";
                case EstadoOrdenDeCargaFason.Vencida:
                    return "Orden vencida";
                case EstadoOrdenDeCargaFason.Entregada:
                    return "Orden entregada";
                case EstadoOrdenDeCargaFason.AnulacionSolicitada:
                    return "Anulación solicitada";
                case EstadoOrdenDeCargaFason.Anulada:
                    return "Anulada";

                default:
                    return "Sin estado";
            }
        }

        public static string ObtenerSemaforo(this EstadoOrdenDeCargaFason me)
        {
            switch (me)
            {
                case EstadoOrdenDeCargaFason.Vencida:
                case EstadoOrdenDeCargaFason.Anulada:
                    return "red";
                case EstadoOrdenDeCargaFason.AnulacionSolicitada:
                case EstadoOrdenDeCargaFason.Pendiente:
                    return "yellow";
                case EstadoOrdenDeCargaFason.Generada:
                    return "green";
                case EstadoOrdenDeCargaFason.Entregada:
                    return "green_entregada";
                case EstadoOrdenDeCargaFason.PendienteCompensacion:
                case EstadoOrdenDeCargaFason.PendienteContabilizacion:
                    return "";
                default:
                    throw new System.Exception("Semáforo no mapeado");
            }
        }
    }
}
