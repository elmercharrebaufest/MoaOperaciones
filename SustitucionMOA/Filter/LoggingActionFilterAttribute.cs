using SustitucionMOASecurity;
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
                User = SessionPersister.getUsername(),
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
                if (MetodosAceptados())
                {
                    SustitucionMOAWS.Logger.Log.LogRequest(logInfo.ToJson());
                }
            }

            base.OnActionExecuted(filterContext);

            bool MetodosAceptados()
            {
                return logInfo.Method != "VerificarEstadoSesion"
                    && !(logInfo.Method == "Index" && logInfo.Controller == "Home")
                    && !(logInfo.Method == "Ping" && logInfo.Controller == "Home")
                    && !(logInfo.Method == "getHomeInfo" && logInfo.Controller == "Home")
                    && !(logInfo.Method == "getHomeNGInfo" && logInfo.Controller == "Home")
                    && !(logInfo.Method == "Front" && logInfo.Controller == "Logger")
                    && !(logInfo.Method == "SignOut" && logInfo.Controller == "Home")
                    && !(logInfo.Method == "ValidarLoginAzure" && logInfo.Controller == "Home");
            }
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