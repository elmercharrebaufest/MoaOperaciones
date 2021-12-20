using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EnumEstadoIngresosBrutosCoeficienteUnificado
    {
        Pendiente = 1,
        Autorizado = 2,
        Completado = 3,
        RechazadoPorUsuario = 4,
    }

    public static class EnumEstadoIngresosBrutosCoeficienteUnificadoExtensions
    {
        public static string ToFriendlyString(this EnumEstadoIngresosBrutosCoeficienteUnificado me)
        {
            switch (me)
            {
                case EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente:
                    return "Pendiente";
                case EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado:
                    return "Autorizado";
                case EnumEstadoIngresosBrutosCoeficienteUnificado.Completado:
                    return "Completado";
                case EnumEstadoIngresosBrutosCoeficienteUnificado.RechazadoPorUsuario:
                    return "RechazadoPorUsuario";
                default:
                    return "Estado desconocido";
            }
        }
    }
}
