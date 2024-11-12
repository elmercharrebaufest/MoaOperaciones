using Newtonsoft.Json;
using SustitucionMOACrypting;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOAUtils.Interfaces;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    //[System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [AllowAnonymous]
    public class ReportesController : BaseController
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public ActionResult LiquidacionesInformadas(string clave)
        {
            var claveDesencriptada = JsonConvert.DeserializeObject<ClaveReporte>(CryptoServiceProvider.Decrypt(clave));

            if (Request.Path.Contains(claveDesencriptada.Controller.ToLower()) && Request.Path.Contains(claveDesencriptada.Resource.ToLower()))
            {
                _reportesService.EnviarReporteLiquidacionesInformadas();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { info = "No pudo procesarse su solicitud. Intente nuevamente" }, JsonRequestBehavior.AllowGet);
        }
    }
}