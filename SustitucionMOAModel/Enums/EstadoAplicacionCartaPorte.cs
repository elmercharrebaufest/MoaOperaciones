namespace SustitucionMOAModel.Enums
{
    namespace SustitucionMOAModel.Enums
    {
        public enum EstadoAplicacionCartaPorte
        {
            Pendiente = 0,
            Aplicado = 1,
            Error = 2,
            SinEstado = 3,
            Eliminado = 4,
            EnProceso = 5,
            PendienteAprobacion = 6,
            Rechazada = 7
        }

        public static class EstadoAplicacionCartaPorteExtensions
        {
            public static string ObtenerSemaforo(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Error:
                    case EstadoAplicacionCartaPorte.Rechazada:
                        return "red";
                    case EstadoAplicacionCartaPorte.Pendiente:
                    case EstadoAplicacionCartaPorte.EnProceso:
                    case EstadoAplicacionCartaPorte.PendienteAprobacion:
                        return "orange";
                    case EstadoAplicacionCartaPorte.Aplicado:
                        return "green";
                    default:
                        return "white";
                }
            }

            public static string ToFriendlyString(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Error:
                        return "Error";                   
                    case EstadoAplicacionCartaPorte.Aplicado:
                        return "Aplicado";
                    case EstadoAplicacionCartaPorte.Pendiente:
                        return "Pendiente";
                    case EstadoAplicacionCartaPorte.EnProceso:
                        return "En proceso";
                    case EstadoAplicacionCartaPorte.PendienteAprobacion:
                        return "Pendiente aprobación";
                    case EstadoAplicacionCartaPorte.Rechazada:
                        return "Rechazada";
                    default:
                        return "Sin estado";
                }
            }

            public static string ToUserFriendlyString(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Pendiente:
                    case EstadoAplicacionCartaPorte.EnProceso:
                    case EstadoAplicacionCartaPorte.PendienteAprobacion:
                        return "En proceso";
                    case EstadoAplicacionCartaPorte.Aplicado:
                        return "Aplicación aceptada";
                    case EstadoAplicacionCartaPorte.Error:
                    case EstadoAplicacionCartaPorte.Rechazada:
                        return "Aplicación rechazada";
                    case EstadoAplicacionCartaPorte.Eliminado:
                        return "Aplicación eliminada";
                    default:
                        return "Sin estado";
                }
            }

            public static EstadoAplicacionCartaPorte ObtenerDescripcionEstado(string estado)
            {
                switch (estado)
                {
                    case "Pendiente":
                        return EstadoAplicacionCartaPorte.Pendiente;
                    case "Aplicado":
                        return EstadoAplicacionCartaPorte.Aplicado;
                    case "Error":
                        return EstadoAplicacionCartaPorte.Error;
                    case "Eliminado":
                        return EstadoAplicacionCartaPorte.Eliminado;
                    case "En proceso":
                        return EstadoAplicacionCartaPorte.EnProceso;
                    default:
                        return EstadoAplicacionCartaPorte.SinEstado;
                }
            }
        }
    }
}
