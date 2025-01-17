using SustitucionMOAUtils.Helpers;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace SustitucionMOA.Filter
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class LoggingActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var logInfo = new ActionLogInfo
            {
                Method = filterContext.ActionDescriptor.ActionName,
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                User = filterContext.HttpContext.User.Identity.IsAuthenticated
                    ? filterContext.HttpContext.User.Identity.Name
                    : "Anonymous",
                StartTime = DateTime.UtcNow,
                Stopwatch = Stopwatch.StartNew()
            };

            filterContext.HttpContext.Items["ActionLogInfo"] = logInfo;

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Items["ActionLogInfo"] is ActionLogInfo logInfo)
            {
                logInfo.Stopwatch.Stop();
                logInfo.EndTime = DateTime.UtcNow;
                logInfo.DurationMilliseconds = logInfo.Stopwatch.ElapsedMilliseconds;
                if (logInfo.Method != "VerificarEstadoSesion")
                {
                    SustitucionMOAWS.Logger.Log.LogRequest(logInfo.ToJson());
                }
            }

            base.OnActionExecuted(filterContext);
        }

    }
    public class ActionLogInfo
    {
        public string Method { get; set; }
        public string Controller { get; set; }
        public string User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? DurationMilliseconds { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Stopwatch Stopwatch { get; set; }
    }
}