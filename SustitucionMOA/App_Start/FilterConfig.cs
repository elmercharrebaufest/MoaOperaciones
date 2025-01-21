using SustitucionMOA.Filter;
using System.Web.Mvc;
namespace SustitucionMOA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GZipOrDeflateAttribute());
            filters.Add(new CustomExceptionFilterAttribute());
            filters.Add(new LoggingActionFilterAttribute());
        }
    }
}
