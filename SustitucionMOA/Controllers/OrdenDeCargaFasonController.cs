using System;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOASecurity;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAUtils.Logger;
using Newtonsoft.Json;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using System.Collections.Generic;

namespace SustitucionMOA.Controllers
{
    public class OrdenDeCargaFasonController : BaseController
    {
        private readonly IOrdenDeCargaFasonService ordenDeCargaFasonService;
        private readonly IConsultaService consultaService;

        public OrdenDeCargaFasonController(IConsultaService consultaService, IOrdenDeCargaFasonService ordenDeCargaFasonService)
        {
            this.consultaService = consultaService;
            this.ordenDeCargaFasonService = ordenDeCargaFasonService;
        }

        [HttpGet]
        public ActionResult Listar(string fechaInicio, string fechaFin)
        {
            var response = new SustitucionMOAApiResponse<ListarOrdenDeCargaFasonResponse>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var request = new ListarOrdenDeCargaFasonRequest()
                {
                    MailUsuario = mailUsuario,
                    FechaDesde = fechaInicio,
                    FechaHasta = fechaFin
                };
                response.Data = ordenDeCargaFasonService.Listar(request);
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
        public ActionResult GetDetalle(int IdOrdenCargaFason)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var request = new DetalleOrdenDeCargaFasonRequest()
                {
                    MailUsuario = mailUsuario
                };

                return JsonCustom(new { data = ordenDeCargaFasonService.ObtenerDetalle(IdOrdenCargaFason, request) });
            }
            catch (InfoCustomException ex)
            {
                return Json(new { info = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        [HttpGet]
        public ActionResult VerificarTransporte(int IdOrdenCargaFason)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaFasonService.VerificarTransporte(IdOrdenCargaFason) });
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
        public ActionResult ObtenerDestinos(int clienteId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = ordenDeCargaFasonService.ObtenerDestinos(clienteId) });
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
        public ActionResult Agregar(string ordenDeCargaFasonJson)
        {
            try
            {
                var crearOrdenReq = JsonConvert.DeserializeObject<CrearOrdenDeCargaFasonRequest>(ordenDeCargaFasonJson);
                var mailUsuario = SessionPersister.getUsername();
                var resultado = ordenDeCargaFasonService.Crear(crearOrdenReq, mailUsuario);
                return JsonCustom(new { data = resultado });
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return JsonCustom(new { error = ErrorMsg.Error });
            }
        }

        [HttpPost]
        public ActionResult Editar(string ordenDeCargaJson)
        {
            try
            {
                var editarOrdenReq = JsonConvert.DeserializeObject<EditarOrdenDeCargaFasonRequest>(ordenDeCargaJson);
                var mailUsuario = SessionPersister.getUsername();
                var resultado = ordenDeCargaFasonService.Editar(editarOrdenReq, mailUsuario);
                return JsonCustom(new { data = resultado });
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return JsonCustom(new { error = ErrorMsg.Error });
            }
        }

        [HttpGet]
        public ActionResult ObtenerCorredores()
        {
            try
            {
                var corredores = ordenDeCargaFasonService.GetCorredores();
                return JsonCustom(new { corredores = corredores });
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return JsonCustom(new { error = ErrorMsg.Error });
            }
        }

        [HttpGet]
        public ActionResult ObtenerClientes(string codigoCorredor)
        {
            try
            {
                var clientes = ordenDeCargaFasonService.GetClientesDeCorredor(codigoCorredor);
                return JsonCustom(new { clientes = clientes });
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                return JsonCustom(new { error = ErrorMsg.Error });
            }
        }

        [HttpGet]
        public ActionResult Materiales()
        {
            try
            {
                return JsonCustom(new { data = consultaService.ObtenerMaterial(TablaSeccionMaterial.OrdenDeCargaFason) });
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
        public ActionResult GestionarAltaCuit(string cuit, string razonSocial, bool esIntermediarioFlete)
        {
            var response = new SustitucionMOAApiResponse();
            try
            {
                ordenDeCargaFasonService.EmailGestionarAlta(cuit, razonSocial, esIntermediarioFlete);
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
                response.Data = ordenDeCargaFasonService.ValidarIntermediarioFlete(cuit);
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
                response.Data = ordenDeCargaFasonService.ObtenerPlantasDestino(destinoCuit);
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
                response.Data = ordenDeCargaFasonService.ObtenerDomiciliosDestino(destinoCuit);
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
                response.Data = ordenDeCargaFasonService.ValidarCuitExisteScato(cuit);
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
        public ActionResult ValidarSisaCuit(string cuitDestinatario, string cuitDestino, string codigoMaterial)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarSisa(cuitDestinatario, cuitDestino, codigoMaterial);
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
                response.Data = ordenDeCargaFasonService.ValidarCuitRuca(cuit);
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
        [HttpPost]
        public ActionResult ActualizarSolicitudEdicion(int ordenId, bool aprobado)
        {
            var response = new SustitucionMOAApiResponse<OrdenDeCargaFasonDto>();
            try
            {
                var estadoSolicitud = new EstadoSolicitudEdicionFason(ordenId, SessionPersister.getUsername(), aprobado);
                response.Data = ordenDeCargaFasonService.ActualizarSolicitudEdicion(estadoSolicitud);
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
        [HttpPost]
        public ActionResult ActualizarSolicitudAnulacion(int ordenId, bool aprobado)
        {
            var response = new SustitucionMOAApiResponse<OrdenDeCargaFasonDto>();
            try
            {
                var estadoSolicitud = new EstadoSolicitudAnulacionFason(ordenId, SessionPersister.getUsername(), aprobado);
                response.Data = ordenDeCargaFasonService.ActualizarSolicitudAnulacion(estadoSolicitud);
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
        [HttpPost]
        public ActionResult SolicitarAnulacion(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<OrdenDeCargaFasonDto>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = ordenDeCargaFasonService.SolicitarAnulacion(ordenId, mailUsuario);
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
        [HttpPost]
        public ActionResult AnularOrden(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<OrdenDeCargaFasonDto>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = ordenDeCargaFasonService.AnularOrden(ordenId, mailUsuario);
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
