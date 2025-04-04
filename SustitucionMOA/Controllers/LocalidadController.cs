using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
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