using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOA.Utils;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;

namespace SustitucionMOASecurity
{
    public static class SessionPersister
    {
        static string userSessionvar = "user";

        static string proveedorSessionvar = "proveedor";

        static string granosFlagSessionvar = "granosFlag";

        static string noticiasSessionvar = "noticias";

        static string notificacionesSessionvar = "notificaciones";

        static string sociedadSessionvar = "sociedad";

        static string B2CClaimsvar = "B2CClaims";


        public static Usuario User
        {
            get
            {
                //if (HttpContext.Current == null)
                //    return null;

                //var sessionVar = HttpContext.Current.Session[userSessionvar];

                //if (sessionVar != null)
                //    return sessionVar as Usuario;

                return null;
            }
            set
            {
                //HttpContext.Current.Session[userSessionvar] = value;
            }
        }

        public static string Proveedor
        {
            get
            {
                if (ClaimsPrincipal.Current != null)
                {
                    return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorType).Value;
                }

                return null;
            }
            set
            {
                //HttpContext.Current.Session[proveedorSessionvar] = value;
            }
        }

        public static string GranosFlag
        {
            get
            {
                /*if (HttpContext.Current == null)
                    return string.Empty;

                var sessionVar = HttpContext.Current.Session[granosFlagSessionvar];

                if (sessionVar != null)
                    return sessionVar as string;*/

                return null;
            }
            set
            {
                //HttpContext.Current.Session[granosFlagSessionvar] = value;
            }
        }

        public static List<Noticia> Noticias
        {
            get
            {
                /*if (HttpContext.Current == null)
                    return new List<Noticia>() { };

                var sessionVar = HttpContext.Current.Session[noticiasSessionvar];

                if (sessionVar != null)
                    return sessionVar as List<Noticia>;*/

                return null;
            }
            set
            {
               // HttpContext.Current.Session[noticiasSessionvar] = value;
            }
        }

        public static List<Notificacion> Notificaciones
        {
            get
            {
               /* if (HttpContext.Current == null)
                    return new List<Notificacion>() { };

                var sessionVar = HttpContext.Current.Session[notificacionesSessionvar];

                if (sessionVar != null)
                    return sessionVar as List<Notificacion>;*/

                return null;
            }
            set
            {
                //HttpContext.Current.Session[notificacionesSessionvar] = value;
            }
        }

        public static string Sociedad
        {
            get
            {
                if (ClaimsPrincipal.Current != null)
                {
                    return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsSociedadType).Value;
                }
                /*
                if (HttpContext.Current == null)
                    return string.Empty;

                var sessionVar = HttpContext.Current.Session[sociedadSessionvar];

                if (sessionVar != null)
                    return sessionVar as string;
                */
                return null;
            }
            set
            {
             //   HttpContext.Current.Session[sociedadSessionvar] = value;
            }
        }

        public static string getUsername() {
            if (ClaimsPrincipal.Current != null) {
                return ClaimsPrincipal.Current.FindFirst("username").Value;
            }
            return "No User";
        }

        public static void clear()
        {
            /*User = null;
            Noticias = null;
            Notificaciones = null;
            Proveedor = string.Empty;
            GranosFlag = string.Empty;
            Sociedad = string.Empty;*/
        }
    }
}
