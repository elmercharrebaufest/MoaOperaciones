using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    namespace SustitucionMOAModel.Enums
    {
        public enum EstadoAplicacionCartaPorte
        {
            Pendiente,
            Aplicado,
            Error,
            SinEstado
        }

        public static class EstadoAplicacionCartaPorteExtensions
        {
            public static string ObtenerSemaforo(this EstadoAplicacionCartaPorte me)
            {
                switch (me)
                {
                    case EstadoAplicacionCartaPorte.Error:
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
                    default:
                        return EstadoAplicacionCartaPorte.SinEstado;
                }
            }

        }
    }

}
