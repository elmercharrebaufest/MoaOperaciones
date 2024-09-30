using System;

namespace SustitucionMOAModel.Enums
{
    [Flags]
    public enum EstadoListarTratamientoSolp : uint
    {
        None = 0,

        /* 0b -> Literal binario
         * _ -> Separador de dígitos para mejorar la legibilidad (se ignora)
         * se usa un _ extra al principio para que quede alineado
         */
        Pendientes = 0b0000_0001, // 1
        Completas = 0b_0000_0010, // 2

        Todas = Pendientes | Completas,
    }
}
