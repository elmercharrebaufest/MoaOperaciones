using System;

namespace SustitucionMOAModel.Enums
{
    [Flags]
    public enum TipoPliego
    {
        None = 0,
        PliegoUnico = 1,
        PliegoMultiple = 2,

        All = PliegoMultiple | PliegoUnico,
    }
}
