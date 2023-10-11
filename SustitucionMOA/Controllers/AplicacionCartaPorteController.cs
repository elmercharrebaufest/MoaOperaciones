using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class AplicacionCartaPorteController : BaseController
    {
        readonly IAplicacionCartaPorteService aplicacionCCPPService;

        public AplicacionCartaPorteController(IAplicacionCartaPorteService aplicacionCCPPService)
        {
            this.aplicacionCCPPService = aplicacionCCPPService;
        }

        [HttpPost]
        public ActionResult Agregar(string aplicacionCCPPJson)
        {
            try
            {
                var aplicacionCCPP = JsonConvert.DeserializeObject<AplicacionCartaPorte>(aplicacionCCPPJson);
                var mailUsuario = SessionPersister.getUsername();
                return JsonCustom(new { data = aplicacionCCPPService.Agregar(aplicacionCCPP, mailUsuario) });
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
                var data = aplicacionCCPPService.Listar(mailUsuario, fechaInicio, fechaFin);
                if (! (data.Count > 0))
                    throw new InfoCustomException("No se han encontrado aplicaciones cargadas");
                var filtros = aplicacionCCPPService.ObtenerFiltros(data);
                return JsonCustom(new { data, filtros });
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
        public ActionResult Get(int aplicacionCCPPId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = aplicacionCCPPService.Obtener(aplicacionCCPPId, mailUsuario) });
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
        public ContentResult EliminarAplicacion(int aplicacionId)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
               aplicacionCCPPService.EliminarAplicacion(aplicacionId);
               response.Data = true;
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

        public ContentResult ObtenerContratos()
        {
            var response = new SustitucionMOAApiResponse<ContratoParaAplicacionCartaPorteResponse>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = new ContratoParaAplicacionCartaPorteResponse { Contratos = aplicacionCCPPService.ObtenerContratos(mailUsuario) };
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
        public ContentResult ObtenerCartasPorte(string numeroContrato)
        {
            var response = new SustitucionMOAApiResponse<CartaPorteParaAplicacionCartaPorteResponse>();
            try
            {
                var mailUsuario = SessionPersister.getUsername();
                response.Data = new CartaPorteParaAplicacionCartaPorteResponse { CartasPorte = aplicacionCCPPService.ObtenerCartasPorte(numeroContrato, mailUsuario) };
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
        public ContentResult GuardarAplicacion(string aplicacionCCPPJSON)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {

                var aplicacionACrear = JsonConvert.DeserializeObject<CrearAplicacionCartaPorte>(aplicacionCCPPJSON);
                var mailUsuario = SessionPersister.getUsername();
                aplicacionCCPPService.GuardarAplicacion(aplicacionACrear, mailUsuario);
                response.Data = true;
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