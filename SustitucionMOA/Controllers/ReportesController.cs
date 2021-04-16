using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOACrypting;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    //[System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [AllowAnonymous]
    public class ReportesController: BaseController
    {
        private readonly IReportesService _reportesService;

        public ReportesController(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public ActionResult LiquidacionesInformadas(string clave)
        {
            try
            {
                var claveDesencriptada = JsonConvert.DeserializeObject<ClaveReporte>(CryptoServiceProvider.Decrypt(clave));
                
                if(Request.Path.Contains(claveDesencriptada.Controller.ToLower()) && Request.Path.Contains(claveDesencriptada.Resource.ToLower()))
                {
                    _reportesService.EnviarReporteLiquidacionesInformadas();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { info = "No pudo procesarse su solicitud. Intente nuevamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}