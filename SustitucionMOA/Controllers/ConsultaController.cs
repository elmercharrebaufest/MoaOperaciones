using System;
using System.Web.Mvc;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class ConsultaController : BaseController
    {
        private readonly IConsultaService consultaService;

        public ConsultaController(IConsultaService consultaService)
        {
            this.consultaService = consultaService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult Comentarios(int consultaId, Comentario comentario)
        {
            try
            {
                if (consultaId <= 0) return Json(new { info = "Id de consulta inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(consultaService.AgregarComentario(consultaId, comentario));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        [Route("/{consultaId}/Comentario/{comentarioId}/Adjuntos")]
        public JsonResult Adjuntos(int consultaId, int comentarioId)
        {
            try
            {
                if (consultaId <= 0 || comentarioId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);
                if (Request.Files.Count <= 0) return Json(new { info = "No se adjuntaron archivos" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new { data = consultaService.AgregarAdjuntoComentario(consultaId, comentarioId, Request.Files) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Consultas()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                return JsonCustom(new { data = consultaService.ListarConsultas(userMail) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = e.Message + "-" + (e.InnerException != null ? e.InnerException.Message : string.Empty);
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult Detalle(int consultaId)
        {
            try
            {
                if (consultaId <= 0) return Json(new { info = "Id de consulta inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(consultaService.ObtenerConsulta(consultaId));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPatch]
        public JsonResult Recategorizar(int consultaId, int categoriaId)
        {
            try
            {
                if (consultaId <= 0 || categoriaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                consultaService.RecategorizarConsulta(consultaId, categoriaId);

                return JsonCustom(new { });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPatch]
        public JsonResult ActualizarEstado(int consultaId, int estadoConsultaId)
        {
            try
            {
                if (consultaId <= 0 || estadoConsultaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                consultaService.ActualizarEstadoConsulta(consultaId, estadoConsultaId);

                return JsonCustom(new { });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Combos()
        {
            try
            {
                return JsonCustom(new { 
                    categorias = consultaService.ObtenerCategorias(),
                    subcategorias = consultaService.ObtenerSubCategorias(),
                    estados = consultaService.ObtenerEstados()
                });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}