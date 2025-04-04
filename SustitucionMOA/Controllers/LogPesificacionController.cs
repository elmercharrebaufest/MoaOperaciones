using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
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
            var idUsuario = ObtenerUsuarioActual().Id;
            var pesificaciones = logPesificacionService.ObtenerPesificacionesManuales(idUsuario);

            return JsonCustom(new
            {
                data = pesificaciones
            });
        }

        [ActionName("ListarAutomatica")]
        public JsonResult ListadoDePesificacionesAutomatica()
        {
            var idUsuario = ObtenerUsuarioActual().Id;
            var pesificaciones = logPesificacionService.ObtenerPesificacionesAutomaticas(idUsuario);

            return JsonCustom(new
            {
                data = pesificaciones
            });
        }

        [ActionName("ListarPorFiltros")]
        public JsonResult ListadoPorFiltros(string filtroJson)
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

        [ActionName("ListarPorFiltrosAutomaticas")]
        public JsonResult ListadoFiltroAutomaticas(string filtroJson)
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

        [ActionName("Agregar")]
        [HttpPost]
        public JsonResult GuardarLogPesificacion(LogPesificacionDto pesificacionDto)
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

        [ActionName("AgregarAutomatica")]
        [HttpPost]
        public ActionResult GuardarLogPesificacionAutomatico(HttpPostedFileBase file)
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

        [ActionName("DescargarArchivo")]
        public ActionResult DescargarArchivo(int idArchivo)
        {
            var archivo = logPesificacionService.ObtenerArchivoLogPesificacion(idArchivo);
            byte[] fileBytes = System.IO.File.ReadAllBytes(archivo.Ruta);
            string fileName = Path.GetFileName(archivo.Ruta);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.Mail;
            return usuarioService.GetUsuario(userMail);
        }
    }
}
