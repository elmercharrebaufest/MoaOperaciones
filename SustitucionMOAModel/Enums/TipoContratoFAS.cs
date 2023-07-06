using System;

namespace SustitucionMOAModel.Enums
{
    public enum TipoContratoFAS
    {
        Normal,
        Anticipado
    }

    //public static class TipoContratoFASParser
    //{

    //    public static TipoContratoFAS Parse(string tipoContrato)
    //    {
    //        if (Enum.TryParse(tipoContrato, true, out TipoContratoFAS result))
    //        {
    //            return result;
    //        }
    //        else
    //        {
    //            return TipoContratoFAS.DESCONOCIDO;
    //        }
    //    }
    //    public static string IntoString(TipoContratoFAS tipoContrato)
    //    {
    //        var nombre = Enum.GetName(typeof(TipoContratoFAS), tipoContrato) ?? "";
    //        return nombre == "DESCONOCIDO" ? "" : nombre;
    //    }
    //}

}
