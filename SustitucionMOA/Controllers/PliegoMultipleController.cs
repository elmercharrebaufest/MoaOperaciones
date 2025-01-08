using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<int> creadorList = string.IsNullOrWhiteSpace(creador)
                ? Enumerable.Empty<int>()
                : creador.Split(',').Select(x => int.Parse(x));

            IEnumerable<string> fiscalList = string.IsNullOrWhiteSpace(fiscal)
                ? Enumerable.Empty<string>()
                : fiscal.Split(',');

            return JsonCustom(pliegoMultipleService.GetSolpDisponiblesPliegosMultiple(numeroSolp, fechaInicio, fechaFin, creadorList, fiscalList, sap, mantenimiento));
        }
    }
}
