using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
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
                Log.Error($"Error id {errorId}", exception);
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
    }
}