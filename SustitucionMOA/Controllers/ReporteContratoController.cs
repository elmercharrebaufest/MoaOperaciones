using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
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
                var proveedor = SessionPersister.Proveedor;
                if (string.IsNullOrEmpty(proveedor))
                {
                    return JsonCustom(new { logout = true });
                }

                var contratos = reporteContratoService.GetContratosReporte(proveedor, fechaInicio, fechaFin, mostrarPendientes, null);
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
                var proveedor = SessionPersister.Proveedor;
                if (string.IsNullOrEmpty(proveedor))
                {
                    return JsonCustom(new { logout = true });
                }

                var dataFiltro = JsonConvert.DeserializeObject<ReporteContratoWSMOAResponse>(dataContrato);
                var contratos = reporteContratoService.GetContratosReporte(proveedor, fechaInicio, fechaFin, mostrarPendientes, dataFiltro);
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

        public ActionResult ObtenerDetalleContrato(string contrato, string fechaInicio, string fechaFin)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var detalleContrato = reporteContratoService.GetContratosDetalle(contrato, SessionPersister.Proveedor, fechaInicio, fechaFin);
                
                Result result = new Result();
                foreach (var det in detalleContrato.data.Resultados)
                {
                    result.Detalles = det.Detalles;

                    foreach(var item in result.Detalles)
                    {
                        if (!item.Entrega.Equals(string.Empty))
                        {
                            try
                            {
                                var orden = ordenDeCargaService.ObtenerPorNroEntrega(mailUsuario, item.Entrega);
                                item.OrdenCargaId = orden.Id.ToString();
                            }
                            catch(Exception ex)
                            {
                                item.OrdenCargaId= string.Empty;
                            }
                        }
                    }
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
        public ActionResult ObtenerOrdenDeCarga(string nroEntrega)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.ObtenerPorNroEntrega(mailUsuario, nroEntrega) });
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