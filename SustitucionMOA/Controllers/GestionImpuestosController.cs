using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class GestionImpuestosController : BaseController
    {
        private readonly IGestionImpuestosService gestionImpuestosService;

        public GestionImpuestosController(IGestionImpuestosService gestionImpuestosService)
        {
            this.gestionImpuestosService = gestionImpuestosService;
        }

        public JsonResult ListarCabeceras()
        {
            return JsonCustom(gestionImpuestosService.ListarCabeceras());
        }

        public JsonResult ListarDetalles(int idCabecera)
        {
            return JsonCustom(gestionImpuestosService.ListarDetalles(idCabecera));
        }
    }
}