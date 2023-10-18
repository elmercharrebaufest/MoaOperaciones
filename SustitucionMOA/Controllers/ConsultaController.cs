using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class ConsultaController : BaseController
    {
        private readonly IConsultaService consultaService;
        private readonly IUsuarioService usuarioService;
        private readonly IOrdenDeCargaService ordenDeCargaService;

        public ConsultaController(IConsultaService consultaService, IUsuarioService usuarioService,
            IOrdenDeCargaService ordenDeCargaService)
        {
            this.consultaService = consultaService;
            this.usuarioService = usuarioService;
            this.ordenDeCargaService = ordenDeCargaService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost, ValidateInput(false)]
        public JsonResult Comentarios(int consultaId, string comentarioJson)
        {
            try
            {
                if (consultaId <= 0) return Json(new { info = "Id de consulta inválido" }, JsonRequestBehavior.AllowGet);
                var comentarioDto = JsonConvert.DeserializeObject<ComentarioDto>(comentarioJson);
                comentarioDto.UsuarioId = ObtenerUsuarioActual().Id;

                return JsonCustom(consultaService.AgregarComentario(consultaId, comentarioDto, Request.Files));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult Consulta(string consultaJson, string comentarioJson)
        {
            try
            {
               
                var consulta = JsonConvert.DeserializeObject<Consulta>(consultaJson);
                var comentario = JsonConvert.DeserializeObject<Comentario>(comentarioJson);
                comentario.Fecha = DateTime.Now;
                consulta.Usuario_Id = ObtenerUsuarioActual().Id;

                return JsonCustom(consultaService.AgregarConsulta(consulta, comentario, Request.Files));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                if (e.LoguearExcepcion)
                {
                    Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.InnerException ?? e);
                }

                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult RecordarComentario(int consultaId)
        {
            try
            {
                return JsonCustom(consultaService.RecordarComentario(consultaId));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult GenerarReclamoImpositivoPdf(string reclamoImpositivoJson)
        {
            try
            {
                var reclamoImpositivo = JsonConvert.DeserializeObject<ReclamoImpositivo>(reclamoImpositivoJson);
                
                string rutaArchivoSubido = consultaService.GenerarReclamoImpositivoPdf(reclamoImpositivo);
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
                string fileName = Path.GetFileName(rutaArchivoSubido);

                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Consultas()
        {
            try
            {
                var usuarioActual = ObtenerUsuarioActual();
                var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);
                var consultas = consultaService.ListarConsultas(usuarioActual.Id, obtenerTodos);
                var categorias = consultaService.ObtenerCategorias(false, usuarioActual);
                var estados = consultaService.ObtenerEstados();

                categorias.ForEach(x => x.Cantidad = consultas.Count(c => c.CategoriaId == x.Id));
                estados.ForEach(x => x.Cantidad = consultas.Count(c => c.EstadoConsultaId == x.Id));

                return JsonCustom(new { data = new
                {
                    consultas,
                    categorias,
                    estados
                }
            });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = e + "-" + (e.InnerException != null ? e.InnerException.Message : string.Empty);
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
                var detalle = consultaService.ObtenerConsulta(consultaId);
                detalle.UsuarioActualId = ObtenerUsuarioActual().Id;

                return JsonCustom(detalle);
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DescargarArchivo(int archivoId)
        {
            try
            {
                string rutaArchivoSubido = consultaService.ObtenerRutaArchivo(archivoId);

                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
                string fileName = Path.GetFileName(rutaArchivoSubido);
                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPatch]
        public JsonResult Recategorizar(int consultaId, int categoriaId, int? subCategoriaId)
        {
            try
            {
                if (consultaId <= 0 || categoriaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                consultaService.RecategorizarConsulta(consultaId, categoriaId, subCategoriaId);

                return JsonCustom(new { });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
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
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Combos(Boolean? excluir)
        {
            try
            {
                var usuarioActual = ObtenerUsuarioActual();
                var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);

                return JsonCustom(new { 
                    categorias = consultaService.ObtenerCategorias(excluir, usuarioActual),
                    subcategorias = consultaService.ObtenerSubCategorias(usuarioActual),
                    estados = consultaService.ObtenerEstados(),
                    causas = consultaService.ObtenerCausas(),
                    materiales = consultaService.ObtenerMaterial(TablaSeccionMaterial.Contacto),
                    isExternal = !obtenerTodos,
                    proveedorId = SessionPersister.ProveedorId
                });;
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public ActionResult ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId, int? causaConsultaId)
        {
            try
            {
                if (consultaId <= 0 || estadoConsultaId <= 0 || categoriaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new
                {
                    data = consultaService.ActualizarCombos(consultaId, estadoConsultaId, categoriaId, subcategoriaId, causaConsultaId)
                });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTA_AMB)]
        [HttpGet]
        public ActionResult RecordatorioComentarioMail(int consulta_Id)
        {
            try
            {
                return JsonCustom(consultaService.EnviarMailRecordatorio(consulta_Id));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult AnularConsulta(int consultaId, string motivoRechazo)
        {
            try
            {
                int usuarioId = ObtenerUsuarioActual().Id;
                return JsonCustom(new { Mensaje = consultaService.AnularConsulta(consultaId, usuarioId, motivoRechazo) });
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

        public ActionResult CombosConsultaInterna(int ordenId)
        {
            try
            {
                var usuarioActual = ObtenerUsuarioActual();
                DateTime fechaFin = DateTime.Now;
                DateTime fechaInicio = fechaFin.AddMonths(-6);
                return JsonCustom(new
                {
                    categorias = consultaService.ObtenerCategorias(false, usuarioActual),
                    subcategorias = consultaService.ObtenerSubCategorias(usuarioActual),
                    ordenes = ordenDeCargaService.Listar(usuarioActual.Mail, fechaInicio.ToString("dd/MM/yyyy"), fechaFin.ToString("dd/MM/yyyy")),
                    proveedorId = SessionPersister.ProveedorId,
                }); 
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetDestinatariosConsulta(int ordenId)
        {
            try
            {
                return JsonCustom(new
                {
                    destinatarios = ordenDeCargaService.ObtenerDestinatariosConsulta(ordenId)
                });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
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

    }
}