using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
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
        public JsonResult GetSolpDisponiblesPliegosMultiple(string numeroSolp,
                                                            DateTime? fechaInicio,
                                                            DateTime? fechaFin,
                                                            string creador,
                                                            string fiscal,
                                                            bool sap = false,
                                                            bool mantenimiento = false)
        {
            return JsonCustom(pliegoMultipleService.GetSolpDisponiblesPliegosMultiple(numeroSolp, fechaInicio, fechaFin, creador, fiscal, sap, mantenimiento));
        }
    }
}
