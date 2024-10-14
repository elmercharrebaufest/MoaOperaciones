using System;
using System.Globalization;
using System.Resources;
using System.Text;

namespace SustitucionMOAFotmatter
{
    public class SAPFormatter
    {
        private static ResourceManager currencyManager;
        public static ResourceManager CurrencyManager
        {
            get
            {
                if (currencyManager == null)
                    currencyManager = SustitucionMOAAssets.Currency.ResourceManager;
                return currencyManager;
            }
        }

        #region Entradas a SAP

        public static string PrepararString(string s)
        {
            return s == null ? "" : s.ToUpper();
        }

        public static string PrepararFecha(DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Salidas de SAP

        public static DateTime GetDateTime(string sapDate)
        {
            if (sapDate.Substring(0, 4) == "0000")
            {
                return new DateTime();
            }

            return new DateTime(
                Convert.ToInt32(sapDate.Substring(0, 4)),
                Convert.ToInt32(sapDate.Substring(5, 2)),
                Convert.ToInt32(sapDate.Substring(8, 2)));
        }

        public static string FormatearFecha(string sapDate)
        {
            DateTime fecha = GetDateTime(sapDate);
            return FormatearFecha(fecha);
        }

        public static string FormatearFecha(DateTime fecha)
        {
            if (fecha.Year == 1)
                return "";
            else
                return fecha.ToString("d MMM yyyy");
        }

        public static string FormatearPorcentaje(decimal porcentaje)
        {
            return FormatearCantidad(porcentaje, "%", 1);
        }

        public static string FormatearCantidad(decimal cantidad, string unidad)
        {
            return FormatearCantidad(cantidad, unidad, 2);
        }

        public static string FormatearCantidad(decimal cantidad, string unidad, uint decimales)
        {
            StringBuilder fmt = new StringBuilder("#,#0");

            if (decimales > 0 && (Math.Abs((cantidad - Convert.ToInt32(cantidad))) > 0))
            {
                fmt.Append(".");

                for (int i = 0; i < decimales; i++)
                {
                    fmt.Append("0");
                }
            }

            if (unidad != "")
            {
                if (cantidad.ToString(fmt.ToString()) == "")
                    return string.Format("{0} {1}", "0", unidad);
                else
                    return string.Format("{0} {1}", cantidad.ToString(fmt.ToString()), unidad);
            }
            else
            {
                if (cantidad.ToString(fmt.ToString()) == "")
                    return string.Format("0");
                else
                    return string.Format(cantidad.ToString(fmt.ToString()));
            }

        }

        public static string FormatearCantidadDouble(double cantidad, string unidad)
        {
            StringBuilder fmt = new StringBuilder("#,#0");

            if (unidad != "")
            {
                if (cantidad.ToString(fmt.ToString()) == "")
                    return string.Format("{0} {1}", "0", unidad);
                else
                    return string.Format("{0} {1}", cantidad.ToString(fmt.ToString()), unidad);
            }
            else
            {
                if (cantidad.ToString(fmt.ToString()) == "")
                    return string.Format("0");
                else
                    return string.Format(cantidad.ToString(fmt.ToString()));
            }
        }

        public static string FormatearCantidadInt(int cantidad, string unidad)
        {
            decimal cantidadDecimal;
            try
            {
                cantidadDecimal = Decimal.Parse(cantidad.ToString());
            }
            catch
            {
                cantidadDecimal = 0;
            }
            return FormatearCantidad(cantidadDecimal, unidad, 2);
        }

        public static string FormatearMoneda(string moneda)
        {
            if (moneda == null)
                return "";

            string currency = CurrencyManager.GetString(moneda);
            return !string.IsNullOrEmpty(currency) ? currency : moneda;
        }

        public static string FormatearMonto(decimal monto, string moneda)
        {
            if (monto == 0)
                return string.Format("{0}{1}", FormatearMoneda(moneda), "0,00");
            else
            {
                string formatoDecimal = (moneda.ToUpper() == "USD" || moneda.ToUpper() == "USDM") ? "N3" : "N2";
                return string.Format("{0}{1}", FormatearMoneda(moneda), monto.ToString(formatoDecimal, new CultureInfo("is-IS")));
            }
        }

        public static string FormatearMonto(decimal montoNumerador, decimal montoDenominador, string moneda)
        {
            if (montoDenominador > 0 && montoNumerador > 0)
                return string.Format("{0}{1}", FormatearMoneda(moneda), (montoNumerador / montoDenominador).ToString("N", new CultureInfo("is-IS")));
            else
                return string.Format("{0}{1}", FormatearMoneda(moneda), "0");
        }

        public static string FormatearMontoTarifas(decimal monto, string moneda)
        {
            if (monto == 0)
                return string.Format("{0}{1}", "0,00", " " + FormatearMoneda(moneda));
            else
                return string.Format("{0}{1}", monto.ToString("N", new CultureInfo("is-IS")), " " + FormatearMoneda(moneda));
        }

        public static string FormatearMonto(decimal monto)
        {
            return removeZeroDecimal(monto);
        }

        public static string FormatearMonto(string monto)
        {
            decimal montoDecimal = 0;
            try { montoDecimal = Convert.ToDecimal(monto); } catch { }
            return removeZeroDecimal(montoDecimal);
        }

        private static string removeZeroDecimal(decimal monto)
        {
            string montoString = monto.ToString("N", new CultureInfo("is-IS"));
            string[] montoArray = montoString.Split(',');
            try
            {
                if (montoArray[1] == "00")
                {
                    return montoArray[0];
                }
            }
            catch { }
            return montoString;
        }

        public static string FormatearTipoVehiculo(string tipo)
        {
            if (tipo.ToUpper() == "C")
            {
                return "Camión";
            }
            else if (tipo.ToUpper() == "T")
            {
                return "Tren";
            }
            else
                return tipo;

        }

        public static string FormatearTipoCambio(decimal tipoCambio)
        {
            return string.Format(tipoCambio.ToString("F3"));
        }

        public static string FormatearCCPP(string ccpp)
        {
            try
            {
                return ccpp.Replace("-", "");
            }
            catch
            {
                return ccpp;
            }
        }

        public static string FormatearBooleano(bool value)
        {
            return value ? "X" : string.Empty;
        }
        #endregion
    }
}
