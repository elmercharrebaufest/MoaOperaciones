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
        AnularAprobacion,
        AnalisisInterno

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
                case EstadoAprobacion.AnalisisInterno:
                    return "Análisis Interno";
                default:
                    return "Estado desconocido";
            }
        }
    }
    public static class EstadoAprobacionHelper
    {
        public static EstadoAprobacion FromStr(string estado)
        {
            switch (estado)
            {
                case "Alta aceptada":
                    return EstadoAprobacion.Aprobado;
                case "Documentación pendiente":
                    return EstadoAprobacion.DocumentacionPendiente;
                //case EstadoAprobacion.AprobacionPendiente:
                //return "Aprobación pendiente";
                case "Alta solicitada":
                    return EstadoAprobacion.AprobacionPendiente;
                case "Deshabilitado en Data Agro":
                    return EstadoAprobacion.DeshabilitadoEnDataAgro;
                case "Proveedor sin alta":
                    return EstadoAprobacion.SinAlta;
                case "Solicitud de información":
                    return EstadoAprobacion.EdicionRequerida;
                case "Alta rechazada":
                    return EstadoAprobacion.Rechazado;
                case "Tipo de usuario no implementado":
                    return EstadoAprobacion.AunNoImplementado;
                case "Deshabilitado":
                    return EstadoAprobacion.Deshabilitado;
                case "Analisis de Nosis":
                    return EstadoAprobacion.AnalisisDeNosis;
                case "Etapa Final":
                    return EstadoAprobacion.EtapaFinal;
                case "Pendiente aprobacion compras":
                    return EstadoAprobacion.PendienteAprobacionCompras;
                case "Rechazado por compras":
                    return EstadoAprobacion.RechazadoPorCompras;
                case "CUIT no habilitado":
                    return EstadoAprobacion.AltaIncompleta;
                case "Anular Aprobacion":
                    return EstadoAprobacion.AnularAprobacion;
                case "Análisis Interno":
                    return EstadoAprobacion.AnalisisInterno;
            }
            return EstadoAprobacion.SinAlta;
        }
    }
}