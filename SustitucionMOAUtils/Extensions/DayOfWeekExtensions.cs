// Ignore Spelling: Dia

using System;

namespace SustitucionMOAUtils.Extensions
{
    public static class DayOfWeekExtensions
    {
        public static string GetDia(this DayOfWeek dia)
        {
            switch (dia)
            {
                case DayOfWeek.Sunday:
                    return "Domingo";
                case DayOfWeek.Monday:
                    return "Lunes";
                case DayOfWeek.Tuesday:
                    return "Martes";
                case DayOfWeek.Wednesday:
                    return "Miércoles";
                case DayOfWeek.Thursday:
                    return "Jueves";
                case DayOfWeek.Friday:
                    return "Viernes";
                case DayOfWeek.Saturday:
                    return "Sábado";
                default:
                    return string.Empty;
            }
        }
    }
}
