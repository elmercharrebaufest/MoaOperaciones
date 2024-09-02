using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Extensions
{
    public static class StringExtensions
    {
        public static string ToContratoSAP(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length >= 10)
            {
                return str;
            }
            return str.PadLeft(10, '0');
        }

        public static string ToCartaPorteSAP(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length >= 12)
            {
                return str;
            }
            return str.PadLeft(12, '0');
        }

        public static string ToFormatoRenspa(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length != 13)
            {
                return str;
            }
            return $"{str.Substring(0,2)}.{str.Substring(2, 3)}.{str.Substring(5, 1)}.{str.Substring(6, 5)}/{str.Substring(11, 2)}";
        }
    }
}
