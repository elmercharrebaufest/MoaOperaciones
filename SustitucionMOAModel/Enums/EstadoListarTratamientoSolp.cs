using System;
using System.ComponentModel;

namespace SustitucionMOAModel.Enums
{
    [Flags]
    public enum EstadoListarTratamientoSolp
    {
        NotConfigured = 0,

        /* 0b -> Literal binario
         * _ -> Separador de dígitos para mejorar la legibilidad (se ignora)
         * se usa un _ extra al principio para que quede alineado
         */

        [Description("Ver Pendientes")]
        Pendientes = 0b0000_0001, // 1

        [Description("Ver Completas")]
        Completas = 0b_0000_0010, // 2

        [Description("Ver Todas")]
        Todas = Pendientes | Completas,
    }
}
