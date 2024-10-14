using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

           // Habilita TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Configurar NLog con la cadena de conexión desde web.config
            LogConfig.ConfigureNLog();
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            var request = HttpContext.Current.Request.RequestContext.HttpContext.Request;

            if (httpException != null)
            {
                var statusCode = httpException.GetHttpCode();

                if (request.AppRelativeCurrentExecutionFilePath.Contains("api"))
                {
                    // Manejar errores para peticiones AJAX
                    Response.Clear();
                    Response.StatusCode = statusCode;
                    Response.ContentType = "application/json";
                    var errorResponse = new { message = "Error " + statusCode + " occurred." };
                    Response.Write(JsonConvert.SerializeObject(errorResponse));
                    Response.End();
                }
                else
                {
                    // Manejar errores para peticiones HTTP normales
                    if (statusCode == 403)
                    {
                        Response.Redirect("~/");
                    }
                    else if (statusCode == 404)
                    {
                        Response.Redirect("~/");
                    }
                    else
                    {
                        // Redirigir a una página de error general para otros casos
                        Response.Redirect("~/");
                    }
                }
            }
        }
    }
}
