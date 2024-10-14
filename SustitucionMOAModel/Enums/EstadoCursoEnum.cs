using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoCursoEnum
    {
        [Description("Sin iniciar")] SinIniciar = 0,
        [Description("Iniciado")] Iniciado = 1,
        [Description("En Progreso")] EnProgreso = 2,
        [Description("Completado")] Completado = 3,
    }
    public static class EnumExtensions
    {
        public static string ToDescription<TEnum>(this TEnum value) where TEnum : Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttributes = fieldInfo.GetCustomAttributes<DescriptionAttribute>();
            return descriptionAttributes.Any() ? descriptionAttributes.First().Description : value.ToString();
        }
    }
}
