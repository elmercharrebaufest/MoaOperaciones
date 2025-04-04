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
    [Authorize]
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
            var response = new SustitucionMOAApiResponse<ProgresoResDto>();
            try
            {
                var emailUsuario = SessionPersister.Mail;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                var emailUsuario = SessionPersister.Mail;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REALIZAR_CURSOS)]
        [HttpPatch]
        public ContentResult ActualizarProgreso([WebHttp.FromBody] ActualizarProgresoReqDto actualizarCursoReq)
        {
            var response = new SustitucionMOAApiResponse<bool>();
            try
            {
                var emailUsuario = SessionPersister.Mail;
                actualizarCursoReq.EmailUsuario = emailUsuario;
                cursoService.ActualizarProgreso(actualizarCursoReq);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ADMINISTRAR_CURSOS)]
        [HttpPost]
        public ContentResult Asignar([WebHttp.FromBody] AsignarReqDto asignarReqDto)
        {
            var response = new SustitucionMOAApiResponse<List<AsignarAlumnosResDto>> ();
            try
            {
                response.Data = cursoService.Asignar(asignarReqDto);
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ADMINISTRAR_CURSOS)]
        [HttpGet]
        public ContentResult ObtenerProgresoAlumnos(int cursoId)
        {
            var response = new SustitucionMOAApiResponse<List<ProgresoAlumnoEnCursoDto>>();
            try
            {
                response.Data = cursoService.ObtenerProgresoAlumnos(cursoId);
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
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.REALIZAR_CURSOS)]
        [HttpGet]
        public ContentResult ObtenerProgresoAlumno(int cursoId)
        {
            var response = new SustitucionMOAApiResponse<ProgresoAlumnoEnCursoDto>();
            try
            {
                var emailUsuario = SessionPersister.Mail;
                response.Data = cursoService.ObtenerProgresoAlumno(cursoId, emailUsuario);
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