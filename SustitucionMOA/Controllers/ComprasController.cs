using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class ComprasController : BaseController
    {
        private readonly IComprasService service;
        private readonly IUsuarioService usuarioService;

        public ComprasController(IComprasService comprasService, IUsuarioService usuarioService)
        {
            service = comprasService;
            this.usuarioService = usuarioService;
        }

        [ValidateInput(false)]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
        public ActionResult GuardarSolp(string solpJson)
        {
            try
            {
                var solp = JsonConvert.DeserializeObject<SolpDto>(solpJson);
                solp.UsuarioActual = ObtenerUsuarioActual();
                var result = service.GuardarSolp(solp, Request.Files);

                try
                {
                    result.Solp.Pdf = Convert.ToBase64String(service.GenerarSolpPdf(result.Solp.Id.Value));
                }
                catch (Exception e)
                {
                    result.Solp.Pdf = string.Empty;
                }

                return JsonCustom(result);
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

        public ActionResult DescargarArchivo(int archivoId)
        {
            try
            {
                string rutaArchivoSubido = service.ObtenerRutaArchivo(archivoId);

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

        public ActionResult Combos()
        {
            try
            {
                return JsonCustom(new
                {
                    ClaseDocumento = service.ObtenerTablaSap(TablasSap.ClaseDocumento),
                    Centro = service.ObtenerTablaSap(TablasSap.Centro),
                    CentrosDireccion = service.ObtenerCentrosDireccion(),
                    Almacen = service.ObtenerTablaSap(TablasSap.Almacen),
                    GrupoCompras = service.ObtenerTablaSap(TablasSap.GrupoCompras),
                    GrupoArticulo = service.ObtenerTablaSap(TablasSap.GrupoArticulo),
                    Moneda = service.ObtenerTablaSap(TablasSap.Moneda).Where(a => a.Codigo != "USDM" && a.Codigo != "CLP").ToList(),
                    Unidades = service.ObtenerTablaSap(TablasSap.Unidad),
                    EstadosSolpSap = service.ObtenerTablaSap(TablasSap.EstadoSolpSap),
                    CentroBeneficio = service.ObtenerTablaSap(TablasSap.CentroBeneficio),
                    EstadoDocumento = service.ObtenerTablaEstado(TablasEstado.EstadoDocumento),
                    TipoPosicionSolp = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp),
                    TipoPosicion = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp),
                    TipoImputacion = service.ObtenerImputaciones(TablasGenerales.TipoImputacionSolp),

                    Usuarios = usuarioService.ListarUsuarioCreadorSolp(),
                    Regiones = service.ListarRegionesSap(),
                    CondicionesDeImportacion = service.ObtenerTablaSap(TablasSap.CondicionesDeImportacion),
                    CondicionesDePago = service.ObtenerTablaSap(TablasSap.CondicionesDePago),
                    CamposObligatoriosCabeceraSolp = service.ObtenerTablaGeneral(TablasGenerales.CamposObligatoriosCabeceraSolp).Where(x => x.IdPadre.HasValue).Select(x => new
                    {
                        ClaseDocumentoCodigo = x.Padre.Codigo,
                        Codigo = x.Codigo
                    }),

                    Provincia = service.ListarProvincia(),
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

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        [HttpGet]
        public ActionResult ListarSolp(int? pagina = null, int? itemsPorPagina = null, string orden = null, string columna = null, string nroSolp = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool sap = false, bool mantenimiento = false,
            bool web = false, bool repoAutomatica = false, bool contratoMarco = false, string estados = null, string usuarios = null, string centros = null, string grupoDeCompras = null, string claseDocumento = null, string tipoImputacion = null, string valorTipoImputacion = null)
        {
            try
            {
                var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
                var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : null), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : itemsPorPagina.Value);
                return JsonCustom(new
                {
                    data = service.ListarSolp(ObtenerUsuarioActual(), paginacion, nroSolp, fechaDesde, fechaHasta, sap, mantenimiento, web, repoAutomatica, contratoMarco, !string.IsNullOrEmpty(usuarios) ? usuarios.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                    !string.IsNullOrEmpty(estados) ? estados.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                    !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(), !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>())
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

        [HttpGet]
        public ActionResult ListarSolpComprador(int? pagina = null, int? itemsPorPagina = null, string orden = null, string columna = null, string nroSolp = null, string estados = null, string usuarios = null, string centros = null, string grupoDeCompras = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null,
            bool sap = false, bool mantenimiento = false, bool web = false, bool repoAutomatica = false, bool listarPendiente = false, bool contratoMarco = false, string claseDocumento = null, string tipoImputacion = null, string valorTipoImputacion = null)
        {
            try
            {
                //service.EditarOrdenDeCompra(new AdjudicacionEditarDto());
                var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
                var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : "Id"), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : (listarPendiente == false && itemsPorPagina.Value == 1) ? 20 : itemsPorPagina.Value);
                var usuario_Id = ObtenerUsuarioActual().Id;

                return JsonCustom(new
                {
                    data = service.ListarSolpComprador(usuario_Id, paginacion, nroSolp, fechaDesde, fechaHasta, sap, mantenimiento, web, repoAutomatica, listarPendiente, contratoMarco, !string.IsNullOrEmpty(usuarios) ? usuarios.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                    !string.IsNullOrEmpty(estados) ? estados.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                    !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(), !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>())
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

        [HttpGet]
        public ActionResult ListarUsuarioCompras()
        {
            try
            {
                return JsonCustom(new { data = service.ListarUsuarioCompras() });
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

        [HttpGet]
        public ActionResult TraerSolpId(int idSolp)
        {
            try
            {
                if (idSolp <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                var solp = service.TraerSolpId(idSolp);

                if (!string.IsNullOrEmpty(solp.EspecificacionesTecnicas) && System.IO.File.Exists(solp.EspecificacionesTecnicas))
                    solp.EspecificacionesTecnicas = System.IO.File.ReadAllText(solp.EspecificacionesTecnicas);
                else
                    solp.EspecificacionesTecnicas = null;

                return JsonCustom(new { data = solp });
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

        [HttpGet]
        public ActionResult BorrarSolp(int idSolp)
        {
            try
            {
                string userMail = SessionPersister.getUsername();

                if (idSolp <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new { data = service.BorrarSolp(idSolp) });
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

        [HttpGet]
        public JsonResult GenerarSolpPdf(int idSolp)
        {
            try
            {
                return JsonCustom(File(service.GenerarSolpPdf(idSolp), System.Net.Mime.MediaTypeNames.Application.Octet, "PliegoSolp" + idSolp + ".pdf"));
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

        [HttpGet]
        public JsonResult ObtenerServiciosSap()
        {
            try
            {
                return JsonCustom(new { data = service.ObtenerServiciosSap() });
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

        [HttpGet]
        public JsonResult AutocompleteTablaSap(string tabla, string valor)
        {
            try
            {
                return JsonCustom(service.AutocompleteTablaSap(tabla, valor));
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

        public ActionResult DescargarZipPliego(int solpId)
        {
            try
            {
                var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                Directory.CreateDirectory(path);

                string rutaZip = service.GenerarZipPliego(solpId, path);
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
                string fileName = Path.GetFileName(rutaZip);

                //Para evitar sobrecargar el server con zips, una vez cargado lo borro
                Directory.Delete(path, true);

                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult DescargarPliegoDesdeLink(int solpId, Guid? token)
        {
            SolpDescargaZipPorLink puedeDescargar = service.PuedeDescargarPliegoDesdeLink(solpId, token);
            if (puedeDescargar != SolpDescargaZipPorLink.PuedeDescargar)
            {
                string errorMsg = puedeDescargar == SolpDescargaZipPorLink.SolpIdNoExiste
                                                        ? "SOLP no disponible para descarga."
                                                        : "El token no coincide; no tiene permiso para realizar la descarga.";

                if (puedeDescargar == SolpDescargaZipPorLink.SinArchivos)
                {
                    errorMsg = "SOLP no disponible para descarga.";
                }

                return Json(new { error = errorMsg }, JsonRequestBehavior.AllowGet);

            }


            var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);

            string rutaZip = service.GenerarZipPliego(solpId, path);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);
            string fileExt = Path.GetExtension(fileName);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);
            string mimeType = fileExt.ToLower() == ".pdf" ? System.Net.Mime.MediaTypeNames.Application.Pdf : System.Net.Mime.MediaTypeNames.Application.Zip;

            return File(fileBytes, mimeType, fileName);
        }

        [ValidateInput(false)]
        public JsonResult ObtenerDatosPorCodigosSap(string codigosSap)
        {
            try
            {
                var codigos = JsonConvert.DeserializeObject<List<TablaSapDto>>(codigosSap);

                return JsonCustom(service.ObtenerDatosPorCodigosSap(codigos));
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

        [HttpGet]
        public JsonResult AutocompleteServicioSolp(string valor)
        {
            try
            {
                return JsonCustom(service.AutocompleteServicioSolp(valor));
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

        [HttpGet]
        public JsonResult AutocompleteCodigoServicioSolp(string valor)
        {
            try
            {
                return JsonCustom(service.AutocompleteCodigoServicioSolp(valor));
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

        [HttpGet]
        public JsonResult AutocompleteProveedor(string valor)
        {
            try
            {
                JsonResult result = JsonCustom(service.AutocompleteProveedor(valor));
                return result;
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

        [ValidateInput(false)]
        public JsonResult ObtenerDatosPorCodigosSapServicioSolp(string codigosSap)
        {
            try
            {
                var codigos = JsonConvert.DeserializeObject<List<string>>(codigosSap);

                return JsonCustom(service.ObtenerDatosPorCodigosSapServicioSolp(codigos));
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

        [HttpGet]
        public JsonResult AutocompleteMaterialSolp(string valor, int centroId)
        {
            try
            {
                return JsonCustom(service.AutocompleteMaterialSolp(valor, centroId));
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

        [HttpGet]
        public JsonResult AutocompleteCodigoMaterialSolp(string valor, int centroId)
        {
            try
            {
                return JsonCustom(service.AutocompleteCodigoMaterialSolp(valor, centroId));
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

        [HttpPost]
        public ActionResult EnviarEmail(string emailCompose)
        {
            try
            {
                var emailComposeDto = JsonConvert.DeserializeObject<EmailComposeDto>(emailCompose);
                service.EnviarEmailSolp(emailComposeDto);
                return JsonCustom(new { success = true });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress,
                    SessionPersister.getUsername(),
                    this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro)
        {
            try
            {
                if (string.IsNullOrEmpty(fechaEntregaPosicion)) return Json(new { info = "Fecha entrega posición inválido" }, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrEmpty(numeroMaterial)) return Json(new { info = "Número material inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new { data = service.ListarFuenteAprovisionamiento(fechaEntregaPosicion, numeroMaterial, centro) });
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

        [HttpGet]
        public ActionResult ObtenerContratoMarco(string numeroContrato, string centro)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroContrato)) return Json(new { info = "Número de contrato inválido" }, JsonRequestBehavior.AllowGet);

                return JsonCustom(new { data = service.ObtenerContratoMarco(numeroContrato, centro) });
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

        [HttpGet]
        public ActionResult ListarPOProveedor(int? pagina = null, int? itemsPorPagina = null, string orden = null, string columna = null, string nroSolp = null, string nroPo = null, string nombrePedido = null,
                    int? estadoLicitacion = null, int? estadoCotizacion = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
                var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : "Id"), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : itemsPorPagina.Value);

                return JsonCustom(new
                {
                    data = service.ListarPOProveedor(paginacion, nroSolp, nroPo, nombrePedido, SessionPersister.getUsername(), fechaDesde, fechaHasta, estadoLicitacion, estadoCotizacion)
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarOfertasComprador(int peticionOferta_Id)
        {
            try
            {
                var usuario = ObtenerUsuarioActual();
                return JsonCustom(new
                {
                    data = service.ListarOfertasComprador(peticionOferta_Id, usuario)
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


        [HttpPost]
        public ActionResult ListarAsociarContrato(string solpJson)
        {
            try
            {
                var posiciones = JsonConvert.DeserializeObject<List<SolpPosicionDto>>(solpJson);
                return JsonCustom(new { data = service.DevolverContratosAsociados(posiciones) });
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

        [HttpGet]
        public ActionResult ObtenerSolpCompras(int id)
        {
            try
            {
                return JsonCustom(new { data = service.ObtenerSolpCompras(id) });
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

        [HttpPost]
        public ActionResult GrabarPeticionDeOferta(string json)
        {
            try
            {
                var peticion = JsonConvert.DeserializeObject<GuardarPeticionDeOfertaDto>(json);
                peticion.UsuarioActual = ObtenerUsuarioActual();
                var result = service.GrabarPeticionDeOferta(peticion, Request.Files, true, null);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ListarProveedores(string filtro)
        {
            try
            {
                var result = usuarioService.ListarProveedores(filtro);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ObtenerLegajo(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor)
        {
            try
            {
                var result = service.ObtenerLegajo(peticionDeOfertaId, idPeticionDeOfertaUsuario, esProveedor);
                return JsonCustom(new { data = result });
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

        [ValidateInput(false)]
        public ActionResult GuardarAdjuntosPeticionDeOferta(string idPeticion)
        {
            try
            {
                Resultado result = service.GuardarAdjuntosPeticionDeOferta(int.Parse(idPeticion), Request.Files, ObtenerUsuarioActual());

                return JsonCustom(result);
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

        public ActionResult DescargarLegajo(int idPeticion, int? idPeticionDeOfertaUsuario)
        {
            try
            {
                var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                Directory.CreateDirectory(path);

                string rutaZip = service.DescargarLegajo(idPeticion, path, idPeticionDeOfertaUsuario);
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
                string fileName = Path.GetFileName(rutaZip);

                //Para evitar sobrecargar el server con zips, una vez cargado lo borro
                Directory.Delete(path, true);

                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario)
        {
            try
            {
                var pdf = service.GenerarPeticionDeOfertaUsuarioPdf(Math.Abs(idPeticionDeOfertaUsuario));
                return JsonCustom(File(pdf.data, System.Net.Mime.MediaTypeNames.Application.Octet, pdf.name));
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

        [HttpPost]
        public ActionResult GrabarCircular(string json)
        {
            try
            {
                var circular = JsonConvert.DeserializeObject<CircularDto>(json);
                circular.UsuarioId = ObtenerUsuarioActual().Id;
                var result = service.GrabarCircular(circular, Request.Files);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ObtenerPeticionDeOferta(int peticionDeOfertaId)
        {
            try
            {
                var result = service.ObtenerPeticionDeOfertaParaCircular(peticionDeOfertaId);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult GrabarProveedorEnPeticion(string json)
        {
            try
            {
                var peticion = JsonConvert.DeserializeObject<GuardarPeticionDeOfertaDto>(json);
                peticion.UsuarioActual = ObtenerUsuarioActual();
                var result = service.GrabarProveedoresEnPeticionDeOferta(peticion.UsuarioIds, peticion.Id);
                return JsonCustom(new { data = result });
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


        [HttpPost]
        public ActionResult CrearOrdenDeCompra(string json)
        {
            try
            {
                var adjudicacion = JsonConvert.DeserializeObject<AdjudicacionDto>(json);
                var result = service.GrabarAdjudicacion(adjudicacion, ObtenerUsuarioActual().Id, "");
                return JsonCustom(result);
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

        [HttpGet]
        public ActionResult DescargarAdjuntosCotizacion(int cotizacionId, bool desdeRevisionTecnica)
        {
            try
            {
                var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                Directory.CreateDirectory(path);

                string rutaZip = service.DescargarAdjuntosCotizacion(cotizacionId, path, desdeRevisionTecnica);
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
                string fileName = Path.GetFileName(rutaZip);

                //Para evitar sobrecargar el server con zips, una vez cargado lo borro
                Directory.Delete(path, true);

                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GrabarRevisionTecnica(string json, bool finalizar, string jsonRevision)
        {
            try
            {
                var peticionDeOfertaUsuarioDto = JsonConvert.DeserializeObject<List<PeticionDeOfertaUsarioDto>>(json);
                var revision = JsonConvert.DeserializeObject<PeticionDeOfertaRevisionTecnicaDto>(jsonRevision);

                var result = service.GrabarRevisionTecnica(peticionDeOfertaUsuarioDto, ObtenerUsuarioActual().Id, finalizar, revision);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ObtenerCotizacion(int peticionDeOfertaId)
        {
            try
            {
                var result = service.TraerCotizacion(peticionDeOfertaId);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult GrabarCotizacion(string json)
        {
            try
            {
                var cotizacion = JsonConvert.DeserializeObject<GuardarCotizacion>(json);
                var usuarioActual = ObtenerUsuarioActual();
                var result = service.GrabarCotizacion(cotizacion, Request.Files, cotizacion.EsFinalizado, usuarioActual.Id, true);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult ObtenerPrecioTotalPosicionProveedor(string json)
        {
            try
            {
                var cotizacion = JsonConvert.DeserializeObject<GuardarCotizacion>(json);
                var result = service.ObtenerPrecioTotalPosicionProveedor(cotizacion);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ObtenerAdjudicacion(string nroOC)
        {
            try
            {
                var result = service.ObtenerAdjudicacion(nroOC);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ListarAdjudicaciones(int solpId)
        {
            try
            {
                var result = service.ListarAdjudicaciones(solpId);
                return JsonCustom(new { data = result });
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


        [HttpGet]
        public ActionResult ListarPeticionesDeOferta(int solpId)
        {
            try
            {
                var result = service.ListarPeticionesDeOferta(solpId);
                return JsonCustom(new { data = result });
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

        static readonly object _lockObtenerOrdenDeCompra = new object();

        [HttpGet]
        public ActionResult ObtenerOrdenDeCompra(string nroOC)
        {
            lock (_lockObtenerOrdenDeCompra)
            {
                try
                {
                    var result = service.ObtenerOrdenDeCompra(nroOC);
                    return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult GuardarAdjudicacionAutomatica(string json)
        {
            try
            {
                //Falta probar el metodo
                var registrosInfo = JsonConvert.DeserializeObject<List<RegistroInfoDto>>(json);
                var usuarioActual = ObtenerUsuarioActual();
                var result = service.CrearOrdenDeCompraConRegistroInfo(registrosInfo, usuarioActual.Id);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult CerrarCotizacion(int peticionId, string observaciones)
        {
            try
            {
                var result = service.CerrarCotizacion(peticionId, ObtenerUsuarioActual().Id, observaciones);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ObtenerUltimaSolp()
        {
            try
            {
                var usuarioId = ObtenerUsuarioActual().Id;

                return JsonCustom(new
                {
                    data = service.ObtenerUltimaSolp(usuarioId)
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

        [HttpGet]
        public JsonResult AutocompleteMaterialRFC(string material, string centro, string grupoDeCompras)
        {
            try
            {
                return JsonCustom(service.ObtenerUltimoRegistroMaterial(material, centro, grupoDeCompras));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ObtenerLegajoParaExternos(int adjudicacionId, string token)
        {
            try
            {
                var result = service.ObtenerLegajoParaExternos(adjudicacionId, token);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult GrabarPeticionDeOfertaVisualizacionPrecio(string json)
        {
            try
            {
                var peticion = JsonConvert.DeserializeObject<PeticionDeOfertaVisualizacionPrecioDto>(json);
                peticion.UsuarioCreador_Id = ObtenerUsuarioActual().Id;
                var result = service.GrabarPeticionDeOfertaVisualizacionPrecio(peticion, Request.Files);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public JsonResult ObtenerReporteOrdenDeCompra(string nroOC, string fechaDesde, string fechaHasta, string codigoProveedor)
        {
            try
            {
                return JsonCustom(service.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerChat(int solpId)
        {
            try
            {
                return JsonCustom(service.ObtenerChat(solpId, ObtenerUsuarioActual().Id));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ObtenerChatProveedor(int peticionDeOfertaUsuarioId)
        {
            try
            {
                return JsonCustom(service.ObtenerChatProveedor(peticionDeOfertaUsuarioId, ObtenerUsuarioActual().Id));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GrabarMensajeChatInterno(string json)
        {
            try
            {
                var mensaje = JsonConvert.DeserializeObject<ChatInternoComprasDto>(json);
                mensaje.Usuario_Id = ObtenerUsuarioActual().Id;
                var result = service.GrabarMensajeChatInterno(mensaje);
                return JsonCustom(new { data = result });
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

        [HttpPost]
        public ActionResult GrabarMensajeChatExterno(string json)
        {
            try
            {
                var mensaje = JsonConvert.DeserializeObject<ChatExternoComprasDto>(json);
                mensaje.Usuario_Id = ObtenerUsuarioActual().Id;
                var result = service.GrabarMensajeChatExterno(mensaje);
                return JsonCustom(new { data = result });
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

        public ActionResult ObtenerYExportarChat(int solpId, int? peticionDeOfertaUsuarioId)
        {
            try
            {
                var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                Directory.CreateDirectory(path);

                string rutaTxt = service.ExportarChatInternoAtexto(solpId, path, peticionDeOfertaUsuarioId);
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaTxt);
                string fileName = Path.GetFileName(rutaTxt);

                //Para evitar sobrecargar el server con zips, una vez cargado lo borro
                Directory.Delete(path, true);

                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Text.Plain, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarRegionesSap()
        {
            try
            {
                var result = service.ListarRegionesSap();
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public JsonResult ValidarSolpTratada(string nroSolp)
        {
            try
            {
                return JsonCustom(service.ValidarSolpTratada(nroSolp));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DevolverMonedaProveedor(string codigoProveedor)
        {
            try
            {
                return JsonCustom(service.DevolverMonedaProveedor(codigoProveedor));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarLiberadorSap()
        {
            try
            {
                return JsonCustom(new { data = service.ListarLiberadorSap() });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarUsuarioCreadorSolp()
        {
            try
            {
                return JsonCustom(new { data = usuarioService.ListarUsuarioCreadorSolp() });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarUnidadesDeMedida(string material)
        {
            try
            {
                return JsonCustom(new { data = service.ListarUnidadesDeMedida(material) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ListarVisitasDeObra(List<VisitaObraDto> visitas)
        {
            try
            {
                var result = service.ListarVisitasDeObra(visitas);
                return JsonCustom(new { data = result });
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

        [HttpGet]
        public ActionResult ListarTablaSap(string codigos)
        {
            List<string> tablas = !string.IsNullOrEmpty(codigos) ? codigos.Split(',').ToList() : new List<string>();
            try
            {
                return JsonCustom(new { data = service.ListarTablaSap(tablas) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ModificarOrdenDeCompra(string json)
        {
            try
            {
                var adjudicacion = JsonConvert.DeserializeObject<AdjudicacionDto>(json);
                var result = service.EditarOrdenDeCompra(adjudicacion);
                return JsonCustom(result);
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

        [HttpGet]
        public JsonResult ListarClaseDocumento(int usuarioId)
        {
            try
            {
                var result = service.ListarClaseDocumento(usuarioId);
                return JsonCustom(result);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ActualizarProveedorVisibleEnSolicitante(int peticionDeOfertaUsuarioId, bool esVisible)
        {
            try
            {
                var result = service.ActualizarProveedorVisibleEnSolicitante(peticionDeOfertaUsuarioId, esVisible);
                return JsonCustom(result);
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

        [HttpGet]
        public ActionResult ObtenerHistorial(int id)
        {
            try
            {
                return JsonCustom(new { data = service.ObtenerHistorial(id) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ListarUsuarioSolicitante()
        {
            try
            {
                var result = service.ListarUsuarioSolicitante();
                return JsonCustom(result);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ListarPosicionesPOMultiple(bool? tratada, string centros = null, string grupoDeCompras = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null,
            bool sap = false, bool mantenimiento = false, bool web = false, bool repoAutomatica = false, bool contratoMarco = false, string claseDocumento = null, string tipoImputacion = null, string valorTipoImputacion = null)
        {
            try
            {
                return JsonCustom(new
                {
                    data = service.ListarPosicionesPOMultiple(fechaDesde, fechaHasta, sap, mantenimiento, web, repoAutomatica, tratada, contratoMarco,
                     !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                     !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(), !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>())
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

        [HttpGet]
        public ActionResult ObtenerPosicionesMultipleCompras(string ids)
        {
            try
            {
                List<int> listaId = ids.Replace("[", "").Replace("]", "").Split(',').Select(x => int.Parse(x)).ToList();
                SolpCompraDto resultado = service.ObtenerPosicionesMultipleCompras(listaId);

                return JsonCustom(new { data = resultado });
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

        [HttpGet]
        public ActionResult ListarHistorialDeFechas(int peticionId)
        {
            try
            {
                var resultado = service.ListarHistorialDeFechas(peticionId);
                return JsonCustom(new { data = resultado });
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

        [HttpPost]
        public ActionResult MarcarChatProveedorComoLeido(string json)
        {
            try
            {
                var proveedorChat = JsonConvert.DeserializeObject<ChatProveedoresDto>(json);
                service.MarcarChatProveedorComoLeido(proveedorChat);
                return JsonCustom(new { success = true });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress,
                    SessionPersister.getUsername(),
                    this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult ListarSolpCondicionEspecial(string filtroJson)
        {
            try
            {
                var filtro = JsonConvert.DeserializeObject<FiltroDto>(filtroJson);
                var ordenar = filtro.Orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
                var paginacion = new Paginacion((!string.IsNullOrEmpty(filtro.Columna) ? filtro.Columna : null), ordenar, (filtro.Pagina == null) ? 0 : filtro.Pagina.Value, (filtro.ItemsPorPagina == 0 || !filtro.ItemsPorPagina.HasValue) ? 10 : filtro.ItemsPorPagina.Value);
                var resultado = service.ListarSolpCondicionEspecial(filtro);
                return JsonCustom(new { data = resultado });                
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

        [HttpPost]
        public ActionResult AgruparPeticionesDeOferta(string peticionDeOfertaIds)
        {
            try
            {
                var usuario = ObtenerUsuarioActual();
                var result = service.AgruparPeticionesDeOferta(usuario.Id, peticionDeOfertaIds);
                return JsonCustom(new { data = result });
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