using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SustitucionMOA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Config",
                url: "config.html",
                defaults: "~/config.html"
            );

            routes.MapRoute(
                name: "Documentos",
                url: "Documentos/{docName}",
                defaults: new { controller = "Documentos", action = "Index", docName = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Registro",
                url: "Registro",
                defaults: new { controller = "Home", action = "Registro" }
            );

            routes.MapRoute(
              name: "RedirectHome",
              url: "RedirectHome",
              defaults: new { controller = "Home", action = "RedirectHome" }
            );

            routes.MapRoute(
              name: "SignUpSignIn",
              url: "SignUpSignIn",
              defaults: new { controller = "AzureB2C", action = "SignUpSignIn" }
            );

            routes.MapRoute(
                name: "SignOut",
                url: "SignOut",
                defaults: new { controller = "Home", action = "SignOut" }
            );

            routes.MapRoute(
               name: "ResetPassword",
               url: "ResetPassword",
               defaults: new { controller = "Home", action = "ResetPassword" }
            );

            routes.MapRoute(
                name: "Default",
                url: "api/{controller}/{action}/{id}",
                defaults: new { action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "spa-fallback",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}

