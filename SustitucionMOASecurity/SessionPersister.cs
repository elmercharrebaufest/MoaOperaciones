using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOASecurity
{
    public static class SessionPersister
    {
        static string usernameSessionvar = "username";

        static string proveedorSessionvar = "proveedor";


        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                var sessionVar = HttpContext.Current.Session[usernameSessionvar];

                if (sessionVar != null)
                    return sessionVar as string;

                return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionvar] = value;
            }
        }


        public static string Proveedor
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;

                var sessionVar = HttpContext.Current.Session[proveedorSessionvar];

                if (sessionVar != null)
                    return sessionVar as string;

                return null;
            }
            set
            {
                HttpContext.Current.Session[proveedorSessionvar] = value;
            }
        }


        public static void clear()
        {
            Username = string.Empty;
            Proveedor = string.Empty;
        }
    }
}
