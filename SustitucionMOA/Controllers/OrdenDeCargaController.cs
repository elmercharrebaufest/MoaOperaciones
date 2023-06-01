using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class OrdenDeCargaController : BaseController
    {
        readonly IOrdenDeCargaService ordenDeCargaService;
        private readonly IConsultaService consultaService;

        public OrdenDeCargaController(IConsultaService consultaService, IOrdenDeCargaService ordenDeCargaService)
        {
            this.consultaService = consultaService;
            this.ordenDeCargaService = ordenDeCargaService;
        }

        [HttpPost]
        public ActionResult Agregar(string ordenDeCargaJson)
        {
            try
            {
                var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaService.Agregar(ordenDeCarga, mailUsuario) });
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

        [HttpPost]
        public ActionResult Editar(string ordenDeCargaJson)
        {
            try
            {
                var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);

                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.Editar(ordenDeCarga, mailUsuario) });
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

        [HttpGet]
        public ActionResult GetListado(string fechaInicio, string fechaFin)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var result = ordenDeCargaService.Listar(mailUsuario, fechaInicio, fechaFin);
                return JsonCustom(result);
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

        [HttpPost]
        public ActionResult CrearOrdenEnSAP(CrearOrdenEnSAPRequest request)
        {
            try
            {
                var response = ordenDeCargaService.CrearOrdenEnSAP(request, true);
                return JsonCustom(response);
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
        [HttpPost]
        public ActionResult EnviarOrdenesASAP(System.Collections.Generic.List<int> ordenesIds)
        {
            try
            {

                var mailUsuario = SessionPersister.getUsername();

                var data = ordenDeCargaService.EnviarOrdenesASAP(ordenesIds, mailUsuario);
                return JsonCustom(new { data });
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

        [HttpGet]
        public ActionResult Get(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.Obtener(mailUsuario, ordenDeCargaId), historial = ordenDeCargaService.ObtenerEditarHistorial(mailUsuario, ordenDeCargaId) });
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

        [HttpGet]
        public ActionResult GetEditar(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.ObtenerEditar(mailUsuario, ordenDeCargaId) });
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

        [HttpGet]
        public ActionResult AnularOrden(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var response = ordenDeCargaService.AnularOrden(ordenId, mailUsuario);
                return JsonCustom(new { data = response });
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

        [HttpGet]
        public ActionResult EdicionFinalizada(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaService.EdicionFinalizada(ordenId, mailUsuario) });
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

        [HttpGet]
        public ActionResult NotificarTransporte(int ordenId)
        {
            try
            {

                return JsonCustom(new { data = ordenDeCargaService.NotificarTransporte(ordenId) });
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

        [HttpGet]
        public ActionResult ObtenerContratos(int ordenId)
        {
            try
            {
                return JsonCustom(new { data = ordenDeCargaService.ObtenerContratos(ordenId) });
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

        [HttpPost]
        public ActionResult SeleccionarContrato(int ordenId, string contratoSAP)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.SeleccionarContrato(ordenId, contratoSAP, mailUsuario) });
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

        [HttpGet]
        public ActionResult ObtenerPedidos(int ordenId)
        {
            try
            {
                return JsonCustom(new { data = ordenDeCargaService.ObtenerPedidos(ordenId) });
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

        [HttpPost]
        public ActionResult SeleccionarPedido(int ordenId, string pedido)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.SeleccionarPedido(ordenId, pedido, mailUsuario) });
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


        [HttpGet]
        public ActionResult VerificarSituacionCrediticia(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.VerificarSituacionCrediticia(ordenId) });
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

        [HttpGet]
        public ActionResult VerificarTransporte(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.VerificarTransporte(ordenId) });
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


        [HttpGet]
        public ActionResult Materiales()
        {
            try
            {
                return JsonCustom(new { data = consultaService.ObtenerMaterial(TablaSeccionMaterial.OrdenDeCarga) });
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

        [HttpPost]
        public ActionResult ForzarCreacionOrden(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.ForzarCreacionOrden(ordenId, mailUsuario) });
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

        [HttpGet]
        public ActionResult SolicitarAnulacion(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var message = ordenDeCargaService.SolicitarAnulacionOrden(ordenDeCargaId, mailUsuario);
                return JsonCustom(message);
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

        [HttpGet]
        public ActionResult RechazarSolicitudAnulacion(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var message = ordenDeCargaService.RechazarSolicitudAnulacion(ordenDeCargaId, mailUsuario);
                return JsonCustom(message);
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

        [HttpGet]
        public ActionResult SolicitarEdicion(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var message = ordenDeCargaService.SolicitarEdicionOrden(ordenDeCargaId, mailUsuario);
                return JsonCustom(message);
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

        [HttpGet]
        public ActionResult RechazarSolicitudEdicion(int ordenDeCargaId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var message = ordenDeCargaService.RechazarSolicitudEdicion(ordenDeCargaId, mailUsuario);
                return JsonCustom(message);
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

        public ActionResult ObtenerPatentes(string ordenDeCargaJson)
        {
            try
            {
                var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);
                var mailUsuario = SessionPersister.getUsername();
                return Json(ordenDeCargaService.ObtenerPatentes(ordenDeCarga, mailUsuario), JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult VisualizarCliente(string corredor, string fechaInicio, string fechaFin, string pendiente)
        {
            try
            {
                //var response = ordenDeCargaService.VisualizarClienteProducto(corredor, fechaInicio, fechaFin, pendiente);
                var request = new VisualizarClienteRequest()
                {
                    Corredor = corredor,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Pendiente = pendiente
                };
                var response = ordenDeCargaService.VisualizarCliente(request);
                return JsonCustom(response);
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

        [HttpGet]
        public ActionResult VisualizarProducto(string contrato, string fechaInicio, string fechaFin, string pendiente)
        {
            try
            {
                var request = new VisualizarProductoRequest()
                {
                    //ClienteCuit = clienteCuit,
                    Contrato = contrato,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    Pendiente = pendiente
                };
                var response = ordenDeCargaService.VisualizarProducto(request);
                return JsonCustom(response);
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

        [HttpGet]
        public ActionResult ValidarCorredorClienteContratoProducto(string clienteCuit, string clienteCodigo, string contrato, string corredor, string usuarioEmail, string fechaInicio, string fechaFin, string productoId, string pendiente)
        {
            try
            {
                var request = new ValidarCorredorClienteContratoProductoRequest()
                {
                    ClienteCuit = clienteCuit,
                    ClienteCodigo = clienteCodigo,
                    Contrato = contrato,
                    Corredor = corredor,
                    UsuarioEmail = usuarioEmail,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    ProductoId = productoId,
                    Pendiente = pendiente
                };
                var response = ordenDeCargaService.ValidarCorredorClienteContratoProducto(request);
                return JsonCustom(response);
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

        [HttpGet]
        public ActionResult AnularOrdenPorVencimiento(int ordenId)
        {
            try
            {

                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaService.NotificarVencimientoOrdenCarga(ordenId, mailUsuario) });
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

        [HttpGet]
        public ActionResult ActivarOC(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaService.ActivarOC(ordenId, mailUsuario) });
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

        [HttpGet]
        public ActionResult ObtenerContratosDisponibles(string clienteCodigo, string fechaDesde, string fechaHasta, string corredorCodigo)
        {
            var response = new ObtenerContratosDisponiblesResponse();
            try
            {

                var mailUsuario = SessionPersister.getUsername();
                var req = new ObtenerContratosDisponiblesRequest
                {
                    ClienteCodigo = clienteCodigo,
                    CorredorCodigo = corredorCodigo ?? "",
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta
                };
                response = ordenDeCargaService.ObtenerContratosDisponibles(req, mailUsuario);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return JsonCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo)
        {
            var response = new SustitucionMOAApiResponse<ValidarSisaCorredorClienteResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarSisaCorredorCliente(corredorCodigo, clienteCodigo);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitExisteScato(string cuit)
        {
            var response = new SustitucionMOAApiResponse<ValidarCuitExisteScatoResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuitExisteScato(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [HttpGet]
        public ActionResult ValidarSisaCuit(string cuit, string campo)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarSisaCuit(cuit, campo);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [HttpGet]
        public ActionResult GestionarAltaCuit(string cuit, string razonSocial, bool esIntermediarioFlete)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.EmailGestionarAlta(cuit, razonSocial, esIntermediarioFlete);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [HttpGet]
        public ActionResult ObtenerPlantasDestino(string destinoCuit)
        {
            var response = new SustitucionMOAApiResponse<List<PlantaDto>>();
            try
            {
                response.Data = ordenDeCargaService.ObtenerPlantasDestino(destinoCuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ObtenerDomiciliosDestino(string destinoCuit)
        {
            var response = new SustitucionMOAApiResponse<List<DomicilioDto>>();
            try
            {
                response.Data = ordenDeCargaService.ObtenerDomiciliosDestino(destinoCuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitRuca(string cuit)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuitRuca(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ContentResult ValidarIntermediarioFlete(string cuit)
        {
            var response = new SustitucionMOAApiResponse<ValidarIntermediarioFleteResponse>();
            try
            {
                response.Data = ordenDeCargaService.ValidarIntermediarioFlete(cuit);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        public ActionResult ValidarCuilChofer(string cuilChofer)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaService.ValidarCuilChoferValido(cuilChofer);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
    }
}
