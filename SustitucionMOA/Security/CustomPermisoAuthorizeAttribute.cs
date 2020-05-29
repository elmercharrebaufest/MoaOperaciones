using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SustitucionMOAAssets;
using SustitucionMOAModel.Models;

namespace SustitucionMOASecurity
{
    public class CustomPermisoAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //Si no está el usuario en Session redirecciona a página Home
            if (SessionPersister.User == null)
            {
                filterContext.Result = new JsonResult() { Data = new { error = ErrorMsg.ErrorUserNoLogueado, logout = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));

            }
            else if (string.IsNullOrEmpty(SessionPersister.Proveedor))
            {
                filterContext.Result = new JsonResult() { Data = new { error = ErrorMsg.ErrorUserNoLogueado, logout = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
            else
            {
                //Usuario user = db.Usuarios.Where(u => u.usuarioID.Equals(SessionPersister.Username)).Include(u => u.Perfiles.Select(p => p.Permisos)).Include(u => u.Clientes).SingleOrDefault();
                //Se reemplaza linea anterior por la siguiente,
                //De lo contrario, por más que no tuviera permiso a funcionalidad, escribiendo URL en sitio subido a Azure (ej. /Facturado) a veces lo dejaba entrar. 
                //También se agregó en web.config: <remove name="RoleManager" />
                Usuario user = (Usuario)SessionPersister.User;

                CustomPrincipal mp = new CustomPrincipal(user);

                //Chequeo que el usuario tenga el rol/permiso
                if (!mp.IsInRole(Roles))
                {
                    filterContext.Result = new JsonResult() { Data = new { error = ErrorMsg.ErrorSinPermiso }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AccessDenied", action = "Index" }));
                }

            }

        }

    }
}
