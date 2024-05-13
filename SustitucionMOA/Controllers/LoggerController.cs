using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class LoggerController: BaseController
    {
        public LoggerController() { }
        
        [HttpPost]
        public ActionResult Front(FrontLoggerRequestDto data)
        {
            try
            {
                Log.FrontError(data);
                return JsonCustom(true);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}