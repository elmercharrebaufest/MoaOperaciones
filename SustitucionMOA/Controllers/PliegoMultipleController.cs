using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class PliegoMultipleController : BaseController
    {
        private readonly IPliegoMultipleService pliegoMultipleService;

        public PliegoMultipleController(IPliegoMultipleService pliegoMultipleService)
        {
            this.pliegoMultipleService = pliegoMultipleService;
        }

        [HttpGet]
        public JsonResult GetPliegosMultiples(string nombrePliego = "")
        {
            return JsonCustom(pliegoMultipleService.GetPliegosMultiples(nombrePliego));
        }
    }
}
