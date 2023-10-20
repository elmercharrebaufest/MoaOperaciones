namespace SustitucionMOAModel.Enums
{
    namespace SustitucionMOAModel.Enums
    {
        public enum EstadoAplicacionCartaPorte
        {
            Pendiente,
            Aplicado,
            Error,
            SinEstado,
            Eliminado
        }

        public static class EstadoAplicacionCartaPorteExtensions
        {
            public static string ObtenerSemaforo(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Error:
                    case EstadoAplicacionCartaPorte.Eliminado:
                        return "red";
                    case EstadoAplicacionCartaPorte.Pendiente:
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
                    case EstadoAplicacionCartaPorte.Eliminado:
                        return "Eliminado";
                    case EstadoAplicacionCartaPorte.Aplicado:
                        return "Aplicado";
                    case EstadoAplicacionCartaPorte.Pendiente:
                        return "Pendiente";
                    default:
                        return "Sin estado";
                }
            }
            public static string ToFriendlyStringFromInt(int me)
            {
                switch (me)
                {
                    case 2:
                        return "Error";
                    case 1:
                        return "Aplicado";
                    case 0:
                        return "Pendiente";
                    case 4:
                        return "Eliminado";
                    default:
                        return "Sin estado";
                }
            }



            public static string ToUserFriendlyString(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Pendiente:
                        return "En proceso";
                    case EstadoAplicacionCartaPorte.Aplicado:
                        return "Aplicación aceptada";
                    case EstadoAplicacionCartaPorte.Error:
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
                    default:
                        return EstadoAplicacionCartaPorte.SinEstado;
                }
            }

        }
    }

}
