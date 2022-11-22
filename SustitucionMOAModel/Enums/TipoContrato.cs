using System;

namespace SustitucionMOAModel.Enums
{
    public static class TipoContrato
    {
        public static readonly Tuple<string, string> ZCNV = new Tuple<string, string>("ZCNV", "Contrato Madre" );
        public static readonly Tuple<string, string> ZFAS = new Tuple<string, string>("ZFAS", "MP-Fason");
        public static readonly Tuple<string, string> ZFJ = new Tuple<string, string>("ZFJ$", "A Precio");
        public static readonly Tuple<string, string> ZHIJ = new Tuple<string, string>("ZHIJ", "Contrato Hijo");
        public static readonly Tuple<string, string> ZPAF = new Tuple<string, string>("ZPAF", "A Fijar");
        public static readonly Tuple<string, string> ZPDV = new Tuple<string, string>("ZPDV", "MP-Prest/Devolución");
        public static readonly Tuple<string, string> ZVEN = new Tuple<string, string>("ZVEN", "MP-Venta granos");

        public static string GetTipoContrato(string key, string pagoDiferido, string dolarExpress,  string dolarCorredor, string pagoDiferidoArp, string canje, string cesion, string compensacion, string dolarizado)
        {
            var result = string.Empty;
            if (key.Equals(ZCNV.Item1))
            {
                result = ZCNV.Item2;
            }
            else if (key.Equals(ZFAS.Item1))
            {
                result = ZFAS.Item2;
            }
            else if (key.Equals(ZFJ.Item1))
            {
                result = ZFJ.Item2;
            }
            else if (key.Equals(ZHIJ.Item1))
            {
                result = ZHIJ.Item2;
            }
            else if (key.Equals(ZPAF.Item1))
            {
                result = ZPAF.Item2;
            }
            else if (key.Equals(ZPDV.Item1))
            {
                result = ZPDV.Item2;
            }
            else if (key.Equals(ZVEN.Item1))
            {
                result = ZVEN.Item2;
            }

            if (pagoDiferido == "X") 
            {
                result = "Dolarizado";
            }

            if (dolarExpress == "X")
            {
                result = "Dolarizado";
            }

            if (dolarCorredor == "X")
            {
                result = "Dolarizado";
            }

            if (pagoDiferidoArp == "X")
            {
                result = "Pago Dif en ARP";
            }

            if (canje == "X")
            {
                result = "Canje";
            }

            if (cesion == "X")
            {
                result = "Cesión";
            }

            if (compensacion == "X")
            {
                result = "Compensación";
            }

            if (dolarizado == "X")
            {
                result = "Dolarizado";
            }



            return result;
        }
    }
}
