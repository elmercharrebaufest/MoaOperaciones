using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Filter
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            // Obtén la excepción lanzada
            var exception = filterContext.Exception;



            // Define el tipo de respuesta en función de la excepción
            if (exception is InfoCustomException)
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { info = exception.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else if (exception is ValidationCustomException)
            {
                Log.Error(HttpContext.Current.Request.UserHostAddress,
                          SessionPersister.getUsername(),
                          filterContext.Controller.GetType().Name,
                          filterContext.RouteData.Values["action"].ToString(),
                          exception);
                filterContext.Result = new JsonResult
                {
                    Data = new { error = exception.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else if (exception is WSCustomException)
            {
                Log.Error(HttpContext.Current.Request.UserHostAddress,
                          SessionPersister.getUsername(),
                          filterContext.Controller.GetType().Name,
                          filterContext.RouteData.Values["action"].ToString(),
                          exception);
                filterContext.Result = new JsonResult
                {
                    Data = new { error = ErrorMsg.ErrorWS },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                var errorId = Guid.NewGuid();
                var requestData = string.Empty;

                try
                {
                    var request = HttpContext.Current.Request;
                    var method = request.HttpMethod;

                    if (method == "GET")
                    {
                        requestData = string.Join("&", request.QueryString.AllKeys
                            .Select(key => $"{key}={SanitizeHtml(request.QueryString[key])}"));
                    }
                    else if (method == "POST")
                    {
                        requestData = string.Join("&", request.Unvalidated.Form.AllKeys
                            .Where(key => !request.Files.AllKeys.Contains(key))
                            .Select(key => $"{key}={SanitizeHtml(request.Unvalidated.Form[key])}"));
                    }
                }
                catch (Exception)
                {
                    //Se agrega para prevenir un error por no tener data o error al obtenerla.
                }


                Log.Debug($"ID Error: {errorId}, Data: {requestData}");
                Log.Error(HttpContext.Current.Request.UserHostAddress,
                          SessionPersister.getUsername(),
                          filterContext.Controller.GetType().Name,
                          filterContext.RouteData.Values["action"].ToString(),
                          exception);
                // Para excepciones generales
                filterContext.Result = new JsonResult
                {
                    Data = new { error = ErrorMsg.Error + $" (ID Error: {errorId})" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            filterContext.ExceptionHandled = true;
        }

        /// <summary>
        /// Limpia el contenido HTML para evitar errores de validación de solicitudes peligrosas.
        /// </summary>
        /// <param name="input">El texto que necesita ser sanitizado.</param>
        /// <returns>Texto sanitizado.</returns>
        private static string SanitizeHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Escapar caracteres especiales para evitar problemas
            var data = HttpUtility.HtmlEncode(input);
            data = data.Replace("&quot;", "\"");
            return data;
        }
    }
}