using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Extensiones
{
    public static class Extensiones
    {
        public static decimal ToDecimal(this string str)
        {
            // you can throw an exception or return a default value here
            if (string.IsNullOrEmpty(str))
                return 0;

            decimal d;

            // you could throw an exception or return a default value on failure
            if (!decimal.TryParse(str, out d))
                return 0;

            return d;
        }
    }
}
