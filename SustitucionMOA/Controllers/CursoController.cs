using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Curso;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using WebHttp=System.Web.Http;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class CursoController : BaseController
    {
        readonly ICursoService cursoService;

        public CursoController(ICursoService cursoService)
        {
            this.cursoService = cursoService;
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REALIZAR_CURSOS)]
        [HttpGet]
        public ContentResult Progreso(int cursoId)
        {
            var response = new SustitucionMOAApiResponse<string>();
            try
            {
                var emailUsuario = SessionPersister.getUsername();
                response.Data = cursoService.ObtenerProgreso(cursoId, emailUsuario);
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REALIZAR_CURSOS)]
        [HttpGet]
        public ContentResult AsignadosAUsuario()
        {
            var response = new SustitucionMOAApiResponse<List<CursoUsuarioDto>>();
            try
            {
                var emailUsuario = SessionPersister.getUsername();
                response.Data = cursoService.AsignadosAUsuario(emailUsuario);
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ADMINISTRAR_CURSOS)]
        [HttpGet]
        public ContentResult Disponibles()
        {
            var response = new SustitucionMOAApiResponse<List<CursoDto>>();
            try
            {
                response.Data = cursoService.Disponibles();
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REALIZAR_CURSOS)]
        [HttpPatch]
        public ContentResult ActualizarEstado([WebHttp.FromBody] ActualizarProgresoReqDto actualizarCursoReq)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                var emailUsuario = SessionPersister.getUsername();
                actualizarCursoReq.EmailUsuario = emailUsuario;
                cursoService.ActualizarEstado(actualizarCursoReq);
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ADMINISTRAR_CURSOS)]
        [HttpPatch]
        public ContentResult Asignar([WebHttp.FromBody] AsignarReqDto asignarReqDto)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                cursoService.Asignar(asignarReqDto);
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