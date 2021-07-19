using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
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

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.c)]
        [HttpGet]
        public ActionResult GetListado()
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.Listar(mailUsuario) });
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

                return JsonCustom(new { data = ordenDeCargaService.Obtener(mailUsuario, ordenDeCargaId) });
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
                return JsonCustom(new { data = ordenDeCargaService.AnularOrden(ordenId) });
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
                var mailUsuario = SessionPersister.getUsername();

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

        [HttpGet]
        public ActionResult ObtenerCorredores(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.ObtenerCorredores(ordenId) });
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
        public ActionResult ObtenerContratosYCorredores(int ordenId)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.ObtenerContratosYCorredores(ordenId) });
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

                return JsonCustom(new { data = ordenDeCargaService.SeleccionarContrato(ordenId, contratoSAP) });
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
        public ActionResult SeleccionarCorredorContrato(int ordenId, string corredorContratoJson)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                var ordenDeCarga = JsonConvert.DeserializeObject<CorredorContratoDto>(corredorContratoJson);

                return JsonCustom(new { data = ordenDeCargaService.SeleccionarCorredorContrato(ordenId, ordenDeCarga) });
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
        public ActionResult SeleccionarCorredor(int ordenId, string corredor)
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.SeleccionarCorredor(ordenId, corredor) });
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
                return JsonCustom(new { data = consultaService.ObtenerMaterial() });
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
    }
}