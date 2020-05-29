using System;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : BaseController
    {
        HomeService _homeService = new HomeService();
        // GET: Home

        public ActionResult Index()
        {
            if (Request.Url.AbsolutePath != "" && Request.Url.AbsolutePath != "/" && Request.Url.AbsolutePath != "/login") {
                return Redirect("/");
            }
            return new FilePathResult(Server.MapPath("~/index.html"), "text/html");
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_HOME)]
        public ActionResult getHomeInfo(string fechaInicio, string fechaFin)
        {
            try
            {
                HomeViewModel result = _homeService.getHomeInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
                return JsonCustom(new { data = result });
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_HOME_NG)]
        public ActionResult getHomeNGInfo(string fechaInicio, string fechaFin)
        {
            try
            {
                HomeViewModel result = _homeService.getHomeNGInfo(SessionPersister.Proveedor, fechaInicio, fechaFin, SessionPersister.Sociedad);
                return JsonCustom(new { data = result });
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}