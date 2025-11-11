using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Consulta;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class ConsultaController : BaseController
    {
        private readonly IConsultaService consultaService;
        private readonly IUsuarioService usuarioService;
        private readonly IVendedorService vendedorService;
        private readonly IOrdenDeCargaService ordenDeCargaService;

        public ConsultaController(IConsultaService consultaService, IUsuarioService usuarioService,
            IVendedorService vendedorService, IOrdenDeCargaService ordenDeCargaService)
        {
            this.consultaService = consultaService;
            this.usuarioService = usuarioService;
            this.vendedorService = vendedorService;
            this.ordenDeCargaService = ordenDeCargaService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost, ValidateInput(false)]
        public JsonResult Comentarios(int consultaId, string comentarioJson)
        {
            if (consultaId <= 0) return Json(new { info = "Id de consulta inválido" }, JsonRequestBehavior.AllowGet);
            var comentarioDto = JsonConvert.DeserializeObject<ComentarioDto>(comentarioJson);
            comentarioDto.UsuarioId = ObtenerUsuarioActual().Id;

            return JsonCustom(consultaService.AgregarComentario(consultaId, comentarioDto, Request.Files));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult Consulta(string consultaJson, string comentarioJson)
        {
            var consulta = JsonConvert.DeserializeObject<Consulta>(consultaJson);
            var comentario = JsonConvert.DeserializeObject<Comentario>(comentarioJson);
            comentario.Fecha = DateTime.Now;
            consulta.Usuario_Id = ObtenerUsuarioActual().Id;
            return JsonCustom(consultaService.AgregarConsulta(consulta, comentario, Request.Files));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult RecordarComentario(int consultaId)
        {
            return JsonCustom(consultaService.RecordarComentario(consultaId));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult GenerarReclamoImpositivoPdf(string reclamoImpositivoJson)
        {

            var reclamoImpositivo = JsonConvert.DeserializeObject<ReclamoImpositivo>(reclamoImpositivoJson);

            string rutaArchivoSubido = consultaService.GenerarReclamoImpositivoPdf(reclamoImpositivo);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
            string fileName = Path.GetFileName(rutaArchivoSubido);

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        [Route("/{consultaId}/Comentario/{comentarioId}/Adjuntos")]
        public JsonResult Adjuntos(int consultaId, int comentarioId)
        {
            if (consultaId <= 0 || comentarioId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);
            if (Request.Files.Count <= 0) return Json(new { info = "No se adjuntaron archivos" }, JsonRequestBehavior.AllowGet);

            return JsonCustom(new { data = consultaService.AgregarAdjuntoComentario(consultaId, comentarioId, Request.Files) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Consultas(ReqListadoConsultaDto reqListadoConsultaDto)
        {
            var usuarioActual = ObtenerUsuarioActual();
            var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);
            var paginacion = new Paginacion(
                reqListadoConsultaDto.OrderBy,
                reqListadoConsultaDto.DirOrden,
                reqListadoConsultaDto.Page,
                reqListadoConsultaDto.PageSize);
            FiltrosConsultaDto filtros = null;
            if (!string.IsNullOrEmpty(reqListadoConsultaDto.FiltrosURIEncoded))
            {
                var jsonDecode = DataFormatter.FormatEncodedURI(reqListadoConsultaDto.FiltrosURIEncoded);
                filtros = DataFormatter.GetDtoFromJsonString<FiltrosConsultaDto>(jsonDecode);
            }


            var consultas = consultaService.ListarConsultas(usuarioActual.Id, obtenerTodos, paginacion, filtros);
            var categorias = consultaService.ObtenerCategorias(false, usuarioActual, true);
            var categoriasParaFiltrar = obtenerTodos ? consultaService.ObtenerCategoriasInterno(false, usuarioActual)
                .Select(x => x.Id) : null;
            var estados = consultaService.ObtenerEstados(categoriasParaFiltrar);

            return JsonCustom(new
            {
                data = new
                {
                    consultas,
                    categorias,
                    estados,
                    totalConsultas = consultas.ItemsTotales,
                    pageItem = consultas.ItemsPorPagina,
                    page = consultas.Pagina
                }
            });
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult ListaExportacionConsultas(ReqListadoConsultaDto reqListadoConsultaDto)
        {
            var usuarioActual = ObtenerUsuarioActual();
            var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);

            FiltrosConsultaDto filtros = null;
            if (!string.IsNullOrEmpty(reqListadoConsultaDto.FiltrosURIEncoded))
            {
                var jsonDecode = DataFormatter.FormatEncodedURI(reqListadoConsultaDto.FiltrosURIEncoded);
                filtros = DataFormatter.GetDtoFromJsonString<FiltrosConsultaDto>(jsonDecode);
            }

            var consultas = consultaService.ListarConsultasSinPaginar(usuarioActual.Id, obtenerTodos, filtros);

            return JsonCustom(new
            {
                data = consultas
            });
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult ObtenerConsultaDisconformidad(string numeroCCPP)
        {
            var usuarioActual = ObtenerUsuarioActual();
            var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);
            var consulta = consultaService.ObtenerConsultaDisconformidad(numeroCCPP, usuarioActual.Id, obtenerTodos);

            return JsonCustom(new
            {
                data = consulta
            });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public JsonResult Detalle(int consultaId)
        {
            if (consultaId <= 0) return Json(new { info = "Id de consulta inválido" }, JsonRequestBehavior.AllowGet);
            var detalle = consultaService.ObtenerConsulta(consultaId);
            detalle.UsuarioActualId = ObtenerUsuarioActual().Id;

            return JsonCustom(detalle);

        }

        public ActionResult DescargarArchivo(int archivoId)
        {

            string rutaArchivoSubido = consultaService.ObtenerRutaArchivo(archivoId);

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
            string fileName = Path.GetFileName(rutaArchivoSubido);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPatch]
        public JsonResult Recategorizar(int consultaId, int categoriaId, int? subCategoriaId)
        {
            if (consultaId <= 0 || categoriaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

            consultaService.RecategorizarConsulta(consultaId, categoriaId, subCategoriaId);

            return JsonCustom(new { });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult ActualizarEstado(int consultaId, int estadoConsultaId)
        {
            if (consultaId <= 0 || estadoConsultaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);
            consultaService.ActualizarEstadoConsulta(consultaId, estadoConsultaId);
            return JsonCustom(new { });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        public ActionResult Combos(Boolean? excluir, Boolean? mostrarCategoriaInterno)
        {
            var usuarioActual = ObtenerUsuarioActual();
            var obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);

            return JsonCustom(new
            {
                categorias = consultaService.ObtenerCategorias(excluir, usuarioActual, mostrarCategoriaInterno),
                subcategorias = consultaService.ObtenerSubCategorias(usuarioActual),
                estados = consultaService.ObtenerEstados(),
                causas = consultaService.ObtenerCausas(),
                materiales = consultaService.ObtenerMaterial(TablaSeccionMaterial.Contacto),
                isExternal = !obtenerTodos,
                proveedorId = SessionPersister.ProveedorId
            }); ;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpGet]
        public ActionResult ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId, int? causaConsultaId)
        {
            if (consultaId <= 0 || estadoConsultaId <= 0 || categoriaId <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

            return JsonCustom(new
            {
                data = consultaService.ActualizarCombos(consultaId, estadoConsultaId, categoriaId, subcategoriaId, causaConsultaId)
            });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTA_AMB)]
        [HttpGet]
        public ActionResult RecordatorioComentarioMail(int consulta_Id)
        {
            return JsonCustom(consultaService.EnviarMailRecordatorio(consulta_Id));
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.Mail;
            return usuarioService.GetUsuario(userMail);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost]
        public JsonResult AnularConsulta(int consultaId, string motivoRechazo)
        {
            int usuarioId = ObtenerUsuarioActual().Id;
            return JsonCustom(new { Mensaje = consultaService.AnularConsulta(consultaId, usuarioId, motivoRechazo) });
        }

        public ActionResult CombosConsultaInterna(int ordenId)
        {
            var usuarioActual = ObtenerUsuarioActual();
            DateTime fechaFin = DateTime.Now;
            DateTime fechaInicio = fechaFin.AddMonths(-6);
            return JsonCustom(new
            {
                categorias = consultaService.ObtenerCategoriasInterno(false, usuarioActual),
                subcategorias = consultaService.ObtenerSubCategorias(usuarioActual),
                ordenes = ordenDeCargaService.Listar(usuarioActual.Mail, fechaInicio.ToString("dd/MM/yyyy"), fechaFin.ToString("dd/MM/yyyy"), "Normal"),
                proveedorId = SessionPersister.ProveedorId,
                materiales = consultaService.ObtenerMaterial(TablaSeccionMaterial.Contacto),
            });
        }

        public ActionResult GetDestinatariosConsultaFas(int ordenId)
        {

            return JsonCustom(new
            {
                destinatarios = ordenDeCargaService.ObtenerDestinatariosConsultaFas(ordenId)
            });
        }

        public ActionResult GetDestinatario(int proveedorId)
        {
            return JsonCustom(new
            {
                destinatarios = usuarioService.ObtenerDestinatariosConsulta(proveedorId)
            });
        }

        public ActionResult GetVendedoresUsuario()
        {
            return JsonCustom(new
            {
                vendedores = vendedorService.GetVendedoresRaw()
            });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONTACTO_MAIL)]
        [HttpPost, ValidateInput(false)]
        public JsonResult AgregarConsultaInterna(string consultaJson, string comentarioJson, string destinatariosJson)
        {
            var consulta = JsonConvert.DeserializeObject<Consulta>(consultaJson);
            var comentario = JsonConvert.DeserializeObject<Comentario>(comentarioJson);
            var destinatarios = JsonConvert.DeserializeObject<List<DestinatarioDto>>(destinatariosJson);

            comentario.Fecha = DateTime.Now;
            consulta.UsuarioInterno_Id = ObtenerUsuarioActual().Id;

            return JsonCustom(consultaService.AgregarConsultaInterna(consulta, comentario, Request.Files, destinatarios));
        }

        public JsonResult ReabrirConsulta(int consultaId)
        {
            var usuarioActual = ObtenerUsuarioActual();
            consultaService.ReabrirConsulta(consultaId, usuarioActual);
            return JsonCustom(new { });
        }

    }
}