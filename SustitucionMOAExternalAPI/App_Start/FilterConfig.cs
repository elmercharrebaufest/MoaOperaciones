using SustitucionMOAExternalAPI.Filter;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOAExternalAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomExceptionHandlerAttribute());

        }
    }
}
