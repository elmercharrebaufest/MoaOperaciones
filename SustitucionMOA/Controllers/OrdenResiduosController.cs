using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Dto.Scato;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class OrdenResiduosController : BaseController
    {
        private readonly IOrdenResiduosService ordenResiduosService;

        public OrdenResiduosController(IOrdenResiduosService ordenResiduosService)
        {
            this.ordenResiduosService = ordenResiduosService;
        }

        [HttpGet]
        public ActionResult ObtenerClientes()
        {
            var response = new SustitucionMOAApiResponse<List<ProveedorDto>>();
            try
            {
                response.Data = ordenResiduosService.ObtenerClientes();
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
        public ActionResult ObtenerProveedor(int idProveedor)
        {
            var response = new SustitucionMOAApiResponse<ProveedorDto>();
            try
            {
                response.Data = ordenResiduosService.ObtenerProveedor(idProveedor);
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
        public ActionResult ObtenerMateriales()
        {
            var response = new SustitucionMOAApiResponse<MaterialDto[]>();
            try
            {
                response.Data = ordenResiduosService.ObtenerMateriales();
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
        public ActionResult ObtenerListadoOrdenes(string fechaInicio, string fechaFin)
        {
            var response = new SustitucionMOAApiResponse<ListarOrdenesResiduosResponse>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = ordenResiduosService.ObtenerListadoOrdenes(fechaInicio, fechaFin, mailUsuario);
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
        public ActionResult ObtenerLocalidades()
        {
            var response = new SustitucionMOAApiResponse<SustitucionMOAModel.Dto.OrdenResiduos.LocalidadDto[]>();
            try
            {
                response.Data = ordenResiduosService.ObtenerLocalidades();
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
        public ActionResult ObtenerPatentes(int clienteId)
        {
            var response = new SustitucionMOAApiResponse<PatentesClienteDto>();
            try
            {
                response.Data = ordenResiduosService.ObtenerPatentes(clienteId);
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
        public ActionResult ObtenerPlantas(string cuit)
        {
            var response = new SustitucionMOAApiResponse<List<SustitucionMOAModel.Dto.OrdenDeCarga.PlantaDto>>();
            try
            {
                response.Data = ordenResiduosService.ObtenerPlantas(cuit);
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
        public ActionResult ObtenerDomicilios(string cuit)
        {
            var response = new SustitucionMOAApiResponse<List<SustitucionMOAModel.Dto.OrdenDeCarga.DomicilioDto>>();
            try
            {
                response.Data = ordenResiduosService.ObtenerDomicilios(cuit);
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
        public ActionResult ObtenerIdsTransportes(int clienteId, string patenteAcoplado)
        {
            var response = new SustitucionMOAApiResponse<TransportesIds>();
            try
            {
                response.Data = ordenResiduosService.ObtenerIdsTransportes(clienteId, patenteAcoplado);
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
        public ActionResult EsCuilCuitValido(string cuilCuit)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenResiduosService.EsCuilCuitValido(cuilCuit);
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
        public ActionResult AgregarOrden(string ordenResiduosJson)
        {
            var response = new SustitucionMOAApiResponse<GrabarOrdenResponse>();
            try
            {
                var ordenDto = JsonConvert.DeserializeObject<OrdenResiduosDto>(ordenResiduosJson);
                var mailUsuario = SessionPersister.getUsername();

                response.Data = ordenResiduosService.CrearNuevaOrden(ordenDto, mailUsuario);
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
        public ActionResult EditarOrden(string ordenResiduosJson)
        {
            var response = new SustitucionMOAApiResponse<GrabarOrdenResponse>();
            try
            {
                var ordenDto = JsonConvert.DeserializeObject<OrdenResiduosDto>(ordenResiduosJson);
                var mailUsuario = SessionPersister.getUsername();
                response.Data = ordenResiduosService.EditarOrden(ordenDto, mailUsuario);
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
        public ActionResult ObtenerOrden(int idOrden)
        {
            var response = new SustitucionMOAApiResponse<OrdenResiduosDto>();
            try
            {
                response.Data = ordenResiduosService.ObtenerOrden(idOrden);
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
        public ActionResult AnularOrden(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<OrdenResiduosDto>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = ordenResiduosService.AnularOrden(ordenId, mailUsuario);
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
        public ActionResult VerificarTransporte(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<OrdenResiduosDto>();
            try
            {
                response.Data = ordenResiduosService.VerificarTransporte(ordenId);
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
        public ActionResult ObtenerDestinosMercaderia(string cuit)
        {
            var response = new SustitucionMOAApiResponse<IList<DestinoScato>>();
            try
            {
                response.Data = ordenResiduosService.ObtenerDestinosMercaderia(cuit);
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
        public ActionResult ValidarCamionEstaEnPlantaParaEditarOrden(int ordenId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                response.Data = ordenResiduosService.ValidarCamionEstaEnPlantaParaEditarOrden(ordenId);
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