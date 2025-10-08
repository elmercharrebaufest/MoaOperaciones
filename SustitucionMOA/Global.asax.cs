using Newtonsoft.Json;
using SustitucionMOAUtils.Logger;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
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

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            if (Request.IsAuthenticated)
            {
                HttpCookie authCookie = Response.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    authCookie.HttpOnly = true;
                    authCookie.Secure = true; // Esto requiere HTTPS habilitado
                }
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            foreach (string key in Response.Cookies.AllKeys)
            {
                HttpCookie cookie = Response.Cookies[key];
                if (cookie != null)
                {
                    string cookieText = $"{cookie.Name}={cookie.Value}; Path={cookie.Path};";

                    if (cookie.HttpOnly)
                    {
                        cookieText += " HttpOnly;";
                    }
                    if (cookie.Secure)
                    {
                        cookieText += " Secure;";
                    }

                    // Agrega SameSite de forma manual
                    cookieText += " SameSite=Strict;";

                    // Usa AppendHeader para evitar sobrescribir el encabezado Set-Cookie
                    Response.Headers.Add("Set-Cookie", cookieText);
                }
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            try
            {
                // Verificar si el usuario no está autenticado
                if (!(HttpContext.Current.User?.Identity?.IsAuthenticated ?? false))
                {

                    Regex regex = new Regex(@"/api/(?<controller>\w+)/(?<action>\w+)", RegexOptions.IgnoreCase);
                    Match match = regex.Match(HttpContext.Current.Request.RawUrl);

                    if (match.Success)
                    {
                        string controllerName = match.Groups["controller"].Value;
                        string actionName = match.Groups["action"].Value;

                        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
                        {
                            return; // Evitar procesar si no hay controlador o acción definidos
                        }

                        // Obtener todos los controladores de la aplicación (sin importar mayúsculas/minúsculas)
                        var controllers = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .Where(t => t.IsClass && t.IsSubclassOf(typeof(Controller)) && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        // Buscar el controlador de forma case-insensitive
                        var controllerType = controllers.FirstOrDefault(c => c.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
                        if (controllerType == null)
                        {
                            return; // Si no se encuentra el controlador, salir
                        }

                        var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
                        var actionDescriptor = controllerDescriptor.GetCanonicalActions()
                            .FirstOrDefault(a => a.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));

                        if (actionDescriptor == null)
                        {
                            return; // Si no se encuentra la acción, salir
                        }


                        // Verificar si la acción o el controlador tiene el atributo [Authorize]
                        bool isAuthorizedAction = actionDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any();
                        bool isAuthorizedController = controllerDescriptor.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any();

                        // Verificar si el controlador o la acción tienen [AllowAnonymous]
                        bool isAnonymousAction = actionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
                        bool isAnonymousController = controllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

                        // Si la acción o el controlador tienen [AllowAnonymous], no aplicar restricción
                        if (isAnonymousAction || isAnonymousController)
                        {
                            return;
                        }

                        // Si la acción o el controlador tiene [Authorize] y el usuario no está autenticado, devolver un 401
                        if ((isAuthorizedAction || isAuthorizedController))
                        {
                            HttpContext.Current.Response.ContentType = "application/json";
                            HttpContext.Current.Response.StatusCode = 401;  // Código de no autorizado
                            HttpContext.Current.Response.Write("{\"success\":false, \"message\":\"Su sesión ha expirado. Por favor, ingrese nuevamente..\"}");
                            HttpContext.Current.Response.End();
                        }
                    }
                }
            }
            catch
            {
                // No hacer nada en el caso que de un error
            }
        }
    }
}
