using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
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


        public JsonResult GetContratos(string fechaInicio, string fechaFin, bool mostrarPendientes, string contrato)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var contratos = reporteContratoService.GetContratosReporte(SessionPersister.Proveedor, fechaInicio, fechaFin, mostrarPendientes, mailUsuario, null);
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

        public ActionResult ObtenerContratosFiltro(string fechaInicio, string fechaFin, string cliente, string producto, string tipoContrato, bool mostrarPendientes, string dataContrato)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var dataFiltro = JsonConvert.DeserializeObject<ReporteContratoWSMOAResponse>(dataContrato);
                var contratos = reporteContratoService.GetContratosReporte(SessionPersister.Proveedor, fechaInicio, fechaFin, mostrarPendientes, mailUsuario, dataFiltro);
                return JsonCustom(contratos);
            }
            catch (InfoCustomException e)
            {
                return Json(new
                {
                    info = e.Message
                }, JsonRequestBehavior.AllowGet);
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

        public ActionResult ObtenerDetalleContrato(string contrato)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var detalleContrato = reporteContratoService.GetContratosDetalle(contrato, SessionPersister.Proveedor);
                Result result = new Result();
                foreach (var det in detalleContrato.data.Resultados)
                {
                    result.Detalles = det.Detalles;
                }
                return JsonCustom(new { data = result.Detalles });
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