using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
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

            var mailUsuario = SessionPersister.Mail;
            var request = new ListarOrdenDeCargaFasonRequest()
            {
                MailUsuario = mailUsuario,
                FechaDesde = fechaInicio,
                FechaHasta = fechaFin,
                EsCorredor = SessionPersister.EsCodigoDeCorredor
            };
            response.Data = ordenDeCargaFasonService.Listar(request);

            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult GetDetalle(int IdOrdenCargaFason)
        {
            var mailUsuario = SessionPersister.Mail;
            var request = new DetalleOrdenDeCargaFasonRequest()
            {
                MailUsuario = mailUsuario
            };

            return JsonCustom(new { data = ordenDeCargaFasonService.ObtenerDetalle(IdOrdenCargaFason, request) });
        }


        [HttpGet]
        public ActionResult VerificarTransporte(int IdOrdenCargaFason)
        {
            var response = new SustitucionMOAApiResponse<OrdenDeCargaFasonDto>();
            try
            {
                var mailUsuario = SessionPersister.Mail;
                response.Data = ordenDeCargaFasonService.VerificarTransporte(IdOrdenCargaFason, mailUsuario);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ObtenerDestinos(int clienteId)
        {
            return JsonCustom(new { data = ordenDeCargaFasonService.ObtenerDestinos(clienteId) });
        }

        [HttpPost]
        public ActionResult Agregar(string ordenDeCargaFasonJson)
        {
            var response = new SustitucionMOAApiResponse<Resultado>();
            try
            {
                var crearOrdenReq = JsonConvert.DeserializeObject<CrearOrdenDeCargaFasonRequest>(ordenDeCargaFasonJson);
                var mailUsuario = SessionPersister.Mail;

                response.Data = ordenDeCargaFasonService.Crear(crearOrdenReq, mailUsuario);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult Editar(string ordenDeCargaJson)
        {
            var editarOrdenReq = JsonConvert.DeserializeObject<EditarOrdenDeCargaFasonRequest>(ordenDeCargaJson);
            var mailUsuario = SessionPersister.Mail;
            var resultado = ordenDeCargaFasonService.Editar(editarOrdenReq, mailUsuario);
            return JsonCustom(new { data = resultado });
        }

        [HttpGet]
        public ActionResult ObtenerCorredores()
        {
            var corredores = ordenDeCargaFasonService.GetCorredores();
            return JsonCustom(new { corredores = corredores });
        }

        [HttpGet]
        public ActionResult ObtenerClientes(string codigoCorredor)
        {
            var clientes = ordenDeCargaFasonService.GetClientesDeCorredor(codigoCorredor);
            return JsonCustom(new { clientes = clientes });
        }

        [HttpGet]
        public ActionResult Materiales()
        {
            return JsonCustom(new { data = consultaService.ObtenerMaterial(TablaSeccionMaterial.OrdenDeCargaFason) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                var mailUsuario = SessionPersister.Mail;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuilChofer(string cuilChofer)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarCuilChoferDigito(cuilChofer);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarCuitTransporte(string cuitTransporte)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarCuitTransporteDigito(cuitTransporte);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ObtenerProveedor(int idProveedor)
        {
            var response = new SustitucionMOAApiResponse<ProveedorDto>();
            try
            {
                response.Data = ordenDeCargaFasonService.ObtenerProveedor(idProveedor);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult ObtenerCuilsChofer(string ordenDeCargaFasonJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<CrearOrdenDeCargaFasonRequest>(ordenDeCargaFasonJson);
            var mailUsuario = SessionPersister.Mail;
            return Json(new { cuils = ordenDeCargaFasonService.ObtenerCuilsChofer(ordenDeCarga, mailUsuario) }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult ObtenerCuitsTransporte(string ordenDeCargaFasonJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<CrearOrdenDeCargaFasonRequest>(ordenDeCargaFasonJson);
            var mailUsuario = SessionPersister.Mail;
            return Json(new { cuits = ordenDeCargaFasonService.ObtenerCuitsTransporte(ordenDeCarga, mailUsuario) }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ObtenerPatentes(string ordenDeCargaFasonJson)
        {
            var ordenDeCarga = JsonConvert.DeserializeObject<CrearOrdenDeCargaFasonRequest>(ordenDeCargaFasonJson);
            var mailUsuario = SessionPersister.Mail;
            return Json(ordenDeCargaFasonService.ObtenerPatentes(ordenDeCarga, mailUsuario), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ContentResult ValidarCamion(string patenteChasis, string patenteAcoplado)
        {
            var response = new SustitucionMOAApiResponse<ValidarCamionResponse>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarCamion(patenteChasis, patenteAcoplado);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult EnviarMailAltaCuitTerceros(bool gestionaFlete, bool gestionaDestino, bool gestionaDestinatario,
            string ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.EnviarMailAltaCuitTerceros(gestionaFlete, gestionaDestino, gestionaDestinatario, ordenId);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult VerificarCuitsTerceros(int ordenId)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(new { data = ordenDeCargaFasonService.VerificarCuitsTerceros(ordenId, mailUsuario) });
        }

        [HttpGet]
        public ActionResult ValidarOrdenActivaScato(long ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarOrdenActivaScato(ordenId);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult ValidarSisaCliente(string codigoCliente, string codigoMaterial)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarSisaCliente(codigoCliente, codigoMaterial);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [HttpGet]
        public ContentResult ValidarExistenciaPatentes(string patenteChasis, string cuitCliente)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenDeCargaFasonService.ValidarExistenciaPatente(patenteChasis, cuitCliente);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
    }
}
