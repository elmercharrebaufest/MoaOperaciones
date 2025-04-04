using SustitucionMOAUtils.Interfaces;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class AdjuntosCertificacionesController : BaseController
    {
        private readonly IAdjuntosCertificacionesService _adjuntosCertificacionesService;

        public AdjuntosCertificacionesController(IAdjuntosCertificacionesService adjuntosCertificacionesService)
        {
            _adjuntosCertificacionesService = adjuntosCertificacionesService;
        }

        public async Task<ActionResult> Adjuntar()
        {
            var result = await _adjuntosCertificacionesService.AdjuntarAsync(Request.Files, "");

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }


        public async Task<ActionResult> GetAdjuntos(string idES)
        {

            var result = await _adjuntosCertificacionesService.GetAdjuntos(idES);

            return JsonCustom(new { data = result });

        }
    }
}
