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
    /*Modificamos toda esta clase para dejar de usar el session persister como HttpContext.Current.Session. Ahora solamente usamos los claims. 
    Esto lo tuvimos que cambiar porq ue no era compatible con el login con Azure
    */
    public static class SessionPersister
    {
        public static Usuario User
        {
            get
            {
                if (ClaimsPrincipal.Current.Claims.Any())
                {
                    return new Usuario
                    {
                        username = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsUserNameType).Value,
                        nombre = ClaimsPrincipal.Current.FindFirst(Globals.ClaimsNombreType).Value,
                        permisos = ClaimsPrincipal.Current.Claims.Where(c => c.Type.Equals(Globals.ClaimsPermisosType)).Select(c => c.Value).ToList()
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public static string Proveedor
        {
            get
            {
                if (ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorType) != null)
                {
                    return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorType).Value;
                }
                return null;
            }
            set
            {
                //TODO: Ver como cambiar el valor del proveedor en el claim. Esto es para cuando un corredor cambia de vendedor
                //HttpContext.Current.Session[proveedorSessionvar] = value;
            }
        }

        public static int ProveedorId
        {
            get
            {
                if (ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorId) != null)
                {
                    return int.Parse(ClaimsPrincipal.Current.FindFirst(Globals.ClaimsProveedorId).Value);
                }
                return 0;
            }
            set
            {
                //TODO: Ver como cambiar el valor del proveedor en el claim. Esto es para cuando un corredor cambia de vendedor
                //HttpContext.Current.Session[proveedorSessionvar] = value;
            }
        }

        public static string Sociedad
        {
            get
            {
                if (ClaimsPrincipal.Current.FindFirst(Globals.ClaimsSociedadType) != null)
                {
                    return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsSociedadType).Value;
                }

                return null;
            }
        }


        public static bool EsCodigoDeCorredor
        {
            get
            {
                if (ClaimsPrincipal.Current.FindFirst(Globals.ClaimsEsCodigoCorredorType) != null)
                {
                    return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsEsCodigoCorredorType).Value == "true";
                }
                return false;
            }
        }

        public static string getUsername()
        {
            if (ClaimsPrincipal.Current.FindFirst(Globals.ClaimsUserNameType) != null)
            {
                return ClaimsPrincipal.Current.FindFirst(Globals.ClaimsUserNameType).Value;
            }
            return "No User";
        }
    }
}
