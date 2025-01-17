// Ignore Spelling: Sustitucion solps

using Newtonsoft.Json;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ComprasDto = SustitucionMOAModel.Dto;

namespace SustitucionMOA.Controllers
{
    [CustomPermisoAuthorize(Roles = Permiso.ABM_SOLP)]
    public class PliegoMultipleController : BaseController
    {
        private readonly IPliegoMultipleService pliegoMultipleService;
        private readonly IUsuarioService usuarioService;

        public PliegoMultipleController(IPliegoMultipleService pliegoMultipleService,
            IUsuarioService usuarioService)
        {
            this.pliegoMultipleService = pliegoMultipleService;
            this.usuarioService = usuarioService;
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CrearPliegoMultiple(string pliegoData, string solpsAsociar)
        {
            ComprasDto.SolpDto pliegoDataDto = JsonConvert.DeserializeObject<ComprasDto.SolpDto>(pliegoData);

            List<int> solpsAsociarList = JsonConvert.DeserializeObject<List<int>>(solpsAsociar);

            pliegoDataDto.UsuarioActual = ObtenerUsuarioActual();

            pliegoMultipleService.CrearPliegoMultiple(pliegoDataDto, Request.Files, solpsAsociarList);

            return JsonCustom(new { });
        }

        private ComprasDto.UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }
    }
}
