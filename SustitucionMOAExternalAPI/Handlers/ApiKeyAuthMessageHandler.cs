using Ninject;
using SustitucionMOAExternalAPI.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace SustitucionMOAExternalAPI.Handlers
{
    public class ApiKeyAuthMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Cabecera http que indica que contiene la Key de la API
        /// </summary>
        private const string API_KEY = "x-api-key";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authManager = request.GetDependencyScope().GetService(typeof(IAuthenticationManager)) as IAuthenticationManager;

            IEnumerable<string> listaCabeceras;
            var existeCabeceraApiKey = request.Headers.TryGetValues(API_KEY, out listaCabeceras);
            if (existeCabeceraApiKey && listaCabeceras.Any())
            {
                List<string> permisos = authManager.ObtenerPermisos(listaCabeceras.First());

                if(permisos != null && permisos.Count > 0)
                {
                    var config = GlobalConfiguration.Configuration;
                    var controllerSelector = new DefaultHttpControllerSelector(config);
                    var controller = controllerSelector.SelectController(request);

                    var principal = new GenericPrincipal(new GenericIdentity("Auth_" + controller.ControllerName), permisos.ToArray());
                    AutorizarAccesoApi(principal);
                }
            }

            return base.SendAsync(request, cancellationToken);
        }

        private void AutorizarAccesoApi(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }
    }
}