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
using SustitucionMOAUtils.Services;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOA.Controllers
{
    public class OrdenDeCargaFasonController : BaseController
    {
        private readonly IOrdenDeCargaFasonService _ordenDeCargaFasonService;
        private readonly IConsultaService consultaService;

        public OrdenDeCargaFasonController(IConsultaService consultaService, IOrdenDeCargaFasonService ordenDeCargaFasonService)
        {
            this.consultaService = consultaService;
            _ordenDeCargaFasonService = ordenDeCargaFasonService;
        }

        [HttpGet]
        public ActionResult Listar(string fechaInicio, string fechaFin)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var request = new ListarOrdenDeCargaFasonRequest()
                {
                    MailUsuario = mailUsuario,
                    FechaDesde = fechaInicio,
                    FechaHasta = fechaFin
                };
                var result = _ordenDeCargaFasonService.Listar(request);
                return JsonCustom(result);
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
        public ActionResult GetDetalle(int IdOrdenCargaFason)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                var request = new DetalleOrdenDeCargaFasonRequest()
                {
                    MailUsuario = mailUsuario
                };

                return JsonCustom(new { data = _ordenDeCargaFasonService.ObtenerDetalle(IdOrdenCargaFason, request) });
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
                return JsonCustom(new { data = _ordenDeCargaFasonService.VerificarTransporte(IdOrdenCargaFason) });
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
                return JsonCustom(new { data = _ordenDeCargaFasonService.ObtenerDestinos(clienteId) });
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
                var resultado = _ordenDeCargaFasonService.Crear(crearOrdenReq, mailUsuario);
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
                var resultado = _ordenDeCargaFasonService.Editar(editarOrdenReq, mailUsuario);
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
                var corredores = _ordenDeCargaFasonService.GetCorredores();
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
                var clientes = _ordenDeCargaFasonService.GetClientesDeCorredor(codigoCorredor);
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
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = _ordenDeCargaFasonService.EmailGestionarAlta(cuit, razonSocial, esIntermediarioFlete);
                response.Data = false;
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
                response.Data = _ordenDeCargaFasonService.ValidarIntermediarioFlete(cuit);
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
