using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class ReporteContratoController : BaseController
    {
        private readonly IOrdenDeCargaService ordenDeCargaService;
        protected readonly IReporteContratoService reporteContratoService;

        public ReporteContratoController(IOrdenDeCargaService ordenDeCargaService, IReporteContratoService reporteContratoService)
        {
            this.ordenDeCargaService = ordenDeCargaService;
            this.reporteContratoService = reporteContratoService;
        }
        // GET: ReporteContrato
        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetContratos()
        {
            try
            {
                var contratos = reporteContratoService.GetContratosReporte(SessionPersister.Proveedor);
                return JsonCustom(contratos);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

       
    }
}