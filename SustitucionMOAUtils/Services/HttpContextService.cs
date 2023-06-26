using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class HttpContextService
    {
        public HttpContextService()
        {

        }

        public string ObtenerPathLogoMail()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/header/logo_.png");
        }
    }
}
