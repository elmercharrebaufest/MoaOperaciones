using SustitucionMOAUtils.Interfaces;
using System;
using System.IO;

namespace SustitucionMOAUtils.Services
{
    public class HttpContextService : IHttpContextService
    {
        public HttpContextService()
        {

        }

        public string ObtenerPathLogoMail()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/header/logo_.png");
        }

        public string GetDirectory(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, path);
        }
    }
}