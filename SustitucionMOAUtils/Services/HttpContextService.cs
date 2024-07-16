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
            return GetDirectory("Templates/images/logo.png");
        }

        public string GetDirectory(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, path);
        }

        public iTextSharp.text.Image ObtenerLogoImagen()
        {
            return iTextSharp.text.Image.GetInstance(@"https://b2cmoagro.blob.core.windows.net/moaoperaciones/logo.png");
        }
    }
}