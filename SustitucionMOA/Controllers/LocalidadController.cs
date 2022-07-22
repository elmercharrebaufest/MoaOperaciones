using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class LocalidadController : Controller
    {
        private readonly ILocalidadService localidadServices;

        public LocalidadController(ILocalidadService localidadService)
        {
            this.localidadServices = localidadService;
        }
        // GET: Localidad
        public ActionResult Index()
        {
            var localidades = localidadServices.GetLocalidades();
            return View();
        }
    }
}