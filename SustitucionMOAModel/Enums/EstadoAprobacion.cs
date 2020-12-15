namespace SustitucionMOAModel.Enums
{
    public enum EstadoAprobacion
    {
        Aprobado,
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
        SinAlta
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
                default:
                    return "Estado desconocido";
            }
        }
    }
}