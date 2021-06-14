using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOAExternalAPI.Managers
{
    public static class AuthenticationManager
    {
        public static List<string> ObtenerRoles(string token)
        {
            return new List<string>()
            {
                token == "1" ? "API" : "API2"
            };
        }
    }

}