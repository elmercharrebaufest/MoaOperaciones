using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [CustomPermisoAuthorize(Roles = Permiso.ABM_SOLP)]
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

        [HttpGet]
        public JsonResult GetSolpDisponiblesPliegosMultiple()
        {
            return JsonCustom(pliegoMultipleService.GetSolpDisponiblesPliegosMultiple());
        }
    }
}
