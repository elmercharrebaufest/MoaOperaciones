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
        protected readonly IOrdenCargaConsumerMOA consumer;

        public ReporteContratoController(IOrdenDeCargaService ordenDeCargaService, IOrdenCargaConsumerMOA consumer)
        {
            this.ordenDeCargaService = ordenDeCargaService;
            this.consumer = consumer;
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
                var request = new OrdenCargaVisualizarClienteWSMOARequest()
                {
                    Cliente = "4922730000",
                    Pendiente= "X"
                };

                return JsonCustom(new { data = consumer.OrdenCargaVisualizarClienteExecute(request) });
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