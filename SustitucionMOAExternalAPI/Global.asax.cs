using SustitucionMOAExternalAPI.Handlers;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SustitucionMOAExternalAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Autenticación por Message Handler
            GlobalConfiguration.Configuration.MessageHandlers.Add(new ApiKeyAuthMessageHandler());

            // Configurar NLog con la cadena de conexión desde web.config
            LogConfig.ConfigureNLog();
        }
    }
}
