using System.Web;
using System.Web.Mvc;
using SustitucionMOA.Filter;
namespace SustitucionMOA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GZipOrDeflateAttribute());
            filters.Add(new CustomExceptionFilterAttribute());
        }
    }
}
