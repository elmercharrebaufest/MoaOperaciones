using SustitucionMOAExternalAPI.Handlers;
using SustitucionMOAUtils.Logger;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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


            // Esto lo podés poner en Application_Start (Web) o en Main (Console/Service)
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true; // Ignora todos los errores de certificado
                };

            // Habilita TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            // Configurar NLog con la cadena de conexión desde web.config
            LogConfig.ConfigureNLog();
        }
    }
}
