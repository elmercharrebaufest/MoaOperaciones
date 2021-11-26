using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTA_LOG_PESIFICACIONES)]
    public class LogPesificacionController : BaseController
    {

        private readonly ILogPesificacionService logPesificacionService;
        private readonly IUsuarioService usuarioService;

        public LogPesificacionController(ILogPesificacionService servicio, IUsuarioService usuarioService)
        {
            this.logPesificacionService = servicio;
            this.usuarioService = usuarioService;
        }

        [ActionName("Listar")]
        public JsonResult ListadoDePesificacionPorUsuario()
        {
            try
            {
                var idUsuario = ObtenerUsuarioActual().Id;
                var pesificaciones = logPesificacionService.ObtenerPesificacionesManuales(idUsuario);

                return JsonCustom(new
                {
                    data = pesificaciones
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
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("ListarAutomatica")]
        public JsonResult ListadoDePesificacionesAutomatica()
        {
            try
            {
                var idUsuario = ObtenerUsuarioActual().Id;
                var pesificaciones = logPesificacionService.ObtenerPesificacionesAutomaticas(idUsuario);

                return JsonCustom(new
                {
                    data = pesificaciones
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
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("ListarPorFiltros")]
        public JsonResult ListadoPorFiltros(string filtroJson)
        {
            try
            {
                var filtro = JsonConvert.DeserializeObject<FiltroDeBusquedaDto>(filtroJson);
                var idUsuario = ObtenerUsuarioActual().Id;
                filtro.IdUsuario = idUsuario;

                var pesificaciones = logPesificacionService.ObtenerPorFiltrosPesificacionesManuales(filtro);

                return JsonCustom(new
                {
                    data = pesificaciones
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
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("ListarPorFiltrosAutomaticas")]
        public JsonResult ListadoFiltroAutomaticas(string filtroJson)
        {
            try
            {
                var filtro = JsonConvert.DeserializeObject<FiltroDeBusquedaMasicoDto>(filtroJson);
                var idUsuario = ObtenerUsuarioActual().Id;
                filtro.IdUsuario = idUsuario;

                var pesificaciones = logPesificacionService.ObtenerPorFiltrosPesificacionesAutomaticas(filtro);

                return JsonCustom(new
                {
                    data = pesificaciones
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
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("Agregar")]
        [HttpPost]
        public JsonResult GuardarLogPesificacion(LogPesificacionDto pesificacionDto)
        {
            try
            {
                if (pesificacionDto != null)
                {
                    var nuevaPesificacion = new LogPesificacion
                    {
                        IdUsuario = ObtenerUsuarioActual().Id,
                        Fecha = DateTime.Now,
                        CodigoProveedor = SessionPersister.Proveedor,
                        Contrato = pesificacionDto.Contrato,
                        Fijacion = pesificacionDto.Fijacion,
                        CantidadKilos = pesificacionDto.CantidadKilos
                    };

                    var pesificacionAdd = logPesificacionService.GuardarPesificacion(nuevaPesificacion);

                    return JsonCustom(new
                    {
                        data = pesificacionAdd
                    });

                }

                return Json(new { info = "No se proporción los datos de la pesificación." });
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

        [ActionName("AgregarAutomatica")]
        [HttpPost]
        public ActionResult GuardarLogPesificacionAutomatico(HttpPostedFileBase file)
        {
            try
            {
                var nuevaPesificacion = new LogPesificacion
                {
                    IdUsuario = ObtenerUsuarioActual().Id,
                    Fecha = DateTime.Now,
                    CodigoProveedor = SessionPersister.Proveedor,
                };

                var pesificacionAdd = logPesificacionService.GuardarPesificacionAutomatica(nuevaPesificacion, file);

                return JsonCustom(new
                {
                    data = pesificacionAdd
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [ActionName("DescargarArchivo")]
        public ActionResult DescargarArchivo(int idArchivo)
        {
            try
            {
                var archivo = logPesificacionService.ObtenerArchivoLogPesificacion(idArchivo);
                byte[] fileBytes = System.IO.File.ReadAllBytes(archivo.Ruta);
                string fileName = Path.GetFileName(archivo.Ruta);
                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }
    }
}
