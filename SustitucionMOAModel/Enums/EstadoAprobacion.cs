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
        SentenciaFinal
    }

    public static class EstadoDeAprobacionExtensions
    {
        public static string ToFriendlyString(this EstadoAprobacion me)
        {
            switch (me)
            {
                case EstadoAprobacion.Aprobado:
                    return "Aprobado";
                case EstadoAprobacion.DocumentacionPendiente:
                    return "Documentación pendiente";
                case EstadoAprobacion.AprobacionPendiente:
                    return "Aprobación pendiente";
                case EstadoAprobacion.DeshabilitadoEnDataAgro:
                    return "Deshabilitado en DataAgro";
                case EstadoAprobacion.EdicionRequerida:
                    return "Edicion requerida";
                case EstadoAprobacion.Rechazado:
                    return "Rechazado";
                case EstadoAprobacion.AunNoImplementado:
                    return "Tipo de usuario no implementado";
                default:
                    return "Estado desconocido";
            }
        }
    }
}