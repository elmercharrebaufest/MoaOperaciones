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
    }
}
