using System.ComponentModel;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoCursoEnum
    {
        [Description("Sin iniciar")] SinIniciar = 0,
        [Description("Iniciado")] Iniciado = 1,
        [Description("En Progreso")] EnProgreso = 2,
        [Description("Completado")] Completado = 3,
    }
}
