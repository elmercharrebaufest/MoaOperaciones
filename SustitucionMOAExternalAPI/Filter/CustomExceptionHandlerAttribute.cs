using SustitucionMOAUtils.Logger;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Web.Mvc;

namespace SustitucionMOAExternalAPI.Filter
{
    public class CustomExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                try
                {
                    Log.ExternalAPIError(filterContext.Exception.GetOriginalException());
                }
                catch (Exception ex)
                {
                    Log.ExternalAPIError(ex);
                }

                filterContext.ExceptionHandled = true;
            }
            
        }

    }

    public static class ExceptionExtensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }

        public static string GetAllFootprints(this Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = "";
            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString +=
                    "File: " + frame.GetFileName() +
                    ", Method:" + frame.GetMethod().Name +
                    ", LineNumber: " + frame.GetFileLineNumber();

                traceString += "  -->  ";
            }

            return traceString;
        }
    }
}