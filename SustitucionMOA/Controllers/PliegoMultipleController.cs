// Ignore Spelling: Sustitucion solps

using Newtonsoft.Json;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        [HttpDelete]
        public JsonResult EliminarPliegoMultiple(int idPliego)
        {
            pliegoMultipleService.EliminarPliegoMultiple(idPliego);
            return JsonCustom(new { });
        }

        [HttpGet]
        public ActionResult DescargarZipPliego(int pliegoId)
        {
            var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);

            string rutaZip = pliegoMultipleService.GenerarZipPliego(pliegoId, path, out string mimeType);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            return JsonCustom(File(fileBytes, mimeType, fileName));
        }

        public ActionResult TraerPliegoId(int idPliego)
        {
            if (idPliego <= 0) { throw new ArgumentException("El id del pliego no puede ser menor o igual a 0"); }

            return JsonCustom(pliegoMultipleService.TraerPliegoId(idPliego));
        }

        private ComprasDto.UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }
    }
}
