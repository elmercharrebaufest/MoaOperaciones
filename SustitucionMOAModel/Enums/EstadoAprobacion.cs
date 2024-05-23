namespace SustitucionMOAModel.Enums
{
    public enum EstadoAprobacion
    {
        Aprobado = 0,
        DocumentacionPendiente,
        AprobacionPendiente,
        DeshabilitadoEnDataAgro,
        EdicionRequerida,
        Rechazado,
        AunNoImplementado,
        AnalisisDeNosis,
        EtapaFinal,
        Deshabilitado,
        AnularRechazo,
        SinAlta,
        AnularObservacion,
        PendienteAprobacionCompras,
        RechazadoPorCompras,
        AltaIncompleta,
        AnularAprobacion

    }

    public static class EstadoDeAprobacionExtensions
    {
        public static string ToFriendlyString(this EstadoAprobacion me)
        {
            switch (me)
            {
                case EstadoAprobacion.Aprobado:
                    return "Alta aceptada";
                case EstadoAprobacion.DocumentacionPendiente:
                    return "Documentación pendiente";
                //case EstadoAprobacion.AprobacionPendiente:
                //return "Aprobación pendiente";
                case EstadoAprobacion.AprobacionPendiente:
                    return "Alta solicitada";
                case EstadoAprobacion.DeshabilitadoEnDataAgro:
                    return "Deshabilitado en Data Agro";
                case EstadoAprobacion.SinAlta:
                    return "Proveedor sin alta";
                case EstadoAprobacion.EdicionRequerida:
                    return "Solicitud de información";
                case EstadoAprobacion.Rechazado:
                    return "Alta rechazada";
                case EstadoAprobacion.AunNoImplementado:
                    return "Tipo de usuario no implementado";
                case EstadoAprobacion.Deshabilitado:
                    return "Deshabilitado";
                case EstadoAprobacion.AnalisisDeNosis:
                    return "Analisis de Nosis";
                case EstadoAprobacion.EtapaFinal:
                    return "Etapa Final";
                case EstadoAprobacion.PendienteAprobacionCompras:
                    return "Pendiente aprobacion compras";
                case EstadoAprobacion.RechazadoPorCompras:
                    return "Rechazado por compras";
                case EstadoAprobacion.AltaIncompleta:
                    return "CUIT no habilitado";
                case EstadoAprobacion.AnularAprobacion:
                    return "Anular Aprobacion";
                default:
                    return "Estado desconocido";
            }
        }
    }
}