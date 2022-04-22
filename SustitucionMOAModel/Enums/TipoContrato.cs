using System;

namespace SustitucionMOAModel.Enums
{
    public static class TipoContrato
    {
        public static readonly Tuple<string, string> ZCNV = new Tuple<string, string>("ZCNV", "Contrato Madre" );
        public static readonly Tuple<string, string> ZFAS = new Tuple<string, string>("ZFAS", "MP-Fason");
        public static readonly Tuple<string, string> ZFJ = new Tuple<string, string>("ZFJ$", "MP-Fason");
        public static readonly Tuple<string, string> ZHIJ = new Tuple<string, string>("ZHIJ", "Contrato Hijo");
        public static readonly Tuple<string, string> ZPAF = new Tuple<string, string>("ZPAF", "A Fijar");
        public static readonly Tuple<string, string> ZPDV = new Tuple<string, string>("ZPDV", "MP-Prest/Devolución");
        public static readonly Tuple<string, string> ZVEN = new Tuple<string, string>("ZVEN", "MP-Venta granos");
    }
}
