using Newtonsoft.Json;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.PrecargaSolp;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HttpHelper = System.Web.Http;


namespace SustitucionMOA.Controllers
{
    [Authorize]
    public class ComprasController : BaseController
    {
        private readonly IComprasService service;
        private readonly IFacturaService facturaService;
        private readonly IComprasSapService comprasSapService;
        private readonly IComprasSolicitanteService comprasSolicitanteService;
        private readonly IUsuarioService usuarioService;
        private readonly IAdjudicacionesService adjudicacionesService;
        private readonly ITablaSapService tablaSapService;

        public ComprasController(IComprasService comprasService,
                                 IFacturaService facturaService,
                                 IComprasSapService comprasSapService,
                                 IComprasSolicitanteService comprasSolicitanteService,
                                 IUsuarioService usuarioService,
                                 IAdjudicacionesService adjudicacionesService,
                                 ITablaSapService tablaSapService)
        {
            this.service = comprasService;
            this.facturaService = facturaService;
            this.comprasSapService = comprasSapService;
            this.comprasSolicitanteService = comprasSolicitanteService;
            this.usuarioService = usuarioService;
            this.adjudicacionesService = adjudicacionesService;
            this.tablaSapService = tablaSapService;
        }

        [ValidateInput(false)]
        [CustomPermisoAuthorize(Roles = Permiso.ABM_SOLP)]
        public ActionResult GuardarSolp(string solpJson)
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

        public ActionResult DescargarArchivo(int archivoId)
        {
            string rutaArchivoSubido = service.ObtenerRutaArchivo(archivoId);

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
            string fileName = Path.GetFileName(rutaArchivoSubido);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        public ActionResult Combos()
        {
            var tiposPosicionSolp = service.ObtenerTiposPosicionSolp();

            return JsonCustom(new
            {
                ClaseDocumento = comprasSapService.ObtenerTablaSap(TablasSap.ClaseDocumento),
                Centro = service.ObtenerCentros(),
                CentrosDireccion = service.ObtenerCentrosDireccion(),
                Almacen = service.ObtenerAlmacenes(),
                GrupoCompras = service.ObtenerGrupoCompras(),
                GrupoArticulo = service.ObtenerGrupoArticulos(),
                Moneda = service.ObtenerMonedas(),
                Unidades = service.ObtenerUnidades(),
                EstadosSolpSap = comprasSapService.ObtenerTablaSap(TablasSap.EstadoSolpSap),
                CentroBeneficio = comprasSapService.ObtenerTablaSap(TablasSap.CentroBeneficio),
                EstadoDocumento = service.ObtenerTablaEstado(TablasEstado.EstadoDocumento),
                TipoPosicionSolp = tiposPosicionSolp,
                TipoPosicion = tiposPosicionSolp,
                TipoImputacion = service.ObtenerTiposImputaciones(),

                Usuarios = usuarioService.ListarUsuarioCreadorSolp(ObtenerUsuarioActual()),
                Regiones = service.ListarRegionesSap(),
                CondicionesDeImportacion = comprasSapService.ObtenerTablaSap(TablasSap.CondicionesDeImportacion),
                CondicionesDePago = comprasSapService.ObtenerTablaSap(TablasSap.CondicionesDePago),
                CamposObligatoriosCabeceraSolp = service.ObtenerTablaGeneral(TablasGenerales.CamposObligatoriosCabeceraSolp).Where(x => x.IdPadre.HasValue).Select(x => new
                {
                    ClaseDocumentoCodigo = x.Padre.Codigo,
                    x.Codigo
                }),

                Provincia = service.ListarProvincia(),

                ListarPendienteList = service.ListarPendienteListComboOptions(),

                // ----- crear PO Múltiple -----
                DefaultTipoPosicionSolpCrearPoMultiple = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp)
                    .Find(x => x.Codigo.StartsWith("material", StringComparison.InvariantCultureIgnoreCase))
                    .Codigo,

                showNombrePliegoConditionList = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp)
                    // por ahora, sólo servicio. Se retorna como lista
                    .Where(x => x.Codigo.StartsWith("servicio", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Codigo),
                // ----- FIN crear PO Múltiple -----

                showTipoPliegoMultipleConditionList = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp)
                    // a la fecha, igual a showNombrePliegoConditionList
                    .Where(x => x.Codigo.StartsWith("servicio", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Codigo),

                TipoPliego = new List<object>
                {
                    new {Id = (int)TipoPliego.PliegoUnico, Descripcion = "Pliego única SOLP"},
                    new {Id = (int)TipoPliego.PliegoMultiple, Descripcion = "Pliego múltiple SOLP"},
                },
            });
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.Mail;
            return usuarioService.GetUsuario(userMail);
        }

        [HttpGet]
        public ActionResult ListarSolp(int? pagina = null, int? itemsPorPagina = null, string orden = null, string columna = null, string nroSolp = null, string nombrePedido = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null, bool sap = false, bool mantenimiento = false,
            bool web = false, bool repoAutomatica = false, bool contratoMarco = false, string estados = null, string usuarios = null, string centros = null, string grupoDeCompras = null, string claseDocumento = null, string tipoImputacion = null, string valorTipoImputacion = null)
        {
            var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
            var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : null), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : itemsPorPagina.Value);
            return JsonCustom(new
            {
                data = comprasSolicitanteService.ListarSolp(ObtenerUsuarioActual(), paginacion, nroSolp, nombrePedido, fechaDesde, fechaHasta, sap, mantenimiento, web, repoAutomatica, contratoMarco, !string.IsNullOrEmpty(usuarios) ? usuarios.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                !string.IsNullOrEmpty(estados) ? estados.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(), !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>())
            });
        }

        [HttpGet]
        public ActionResult ListarSolpComprador(int? pagina = null,
                                                int? itemsPorPagina = null,
                                                string orden = null,
                                                string columna = null,
                                                string nroSolp = null,
                                                string nombrePedido = null,
                                                string estados = null,
                                                string usuarios = null,
                                                string centros = null,
                                                string grupoDeCompras = null,
                                                DateTime? fechaDesde = null,
                                                DateTime? fechaHasta = null,
                                                bool sap = false,
                                                bool mantenimiento = false,
                                                bool web = false,
                                                bool repoAutomatica = false,
                                                EstadoListarTratamientoSolp listarPendiente = EstadoListarTratamientoSolp.Todas,
                                                bool contratoMarco = false,
                                                string claseDocumento = null,
                                                string tipoImputacion = null,
                                                string valorTipoImputacion = null,
                                                string tipoPliego = null)
        {

            var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
            var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : "Id"), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : itemsPorPagina.Value);
            var usuario_Id = ObtenerUsuarioActual().Id;

            TipoPliego tipoPliegoEnum = TipoPliego.All;
            if (!string.IsNullOrWhiteSpace(tipoPliego))
            {
                IEnumerable<int> tipoPliegoList = tipoPliego.Split(',').Select(x => int.Parse(x));
                tipoPliegoEnum = (TipoPliego)tipoPliegoList.Aggregate((x, y) => x | y);
            }

            return JsonCustom(new
            {
                data = service.ListarSolpComprador(usuario_Id, paginacion, nroSolp, nombrePedido, fechaDesde, fechaHasta, sap, mantenimiento, web, repoAutomatica, listarPendiente, contratoMarco, !string.IsNullOrEmpty(usuarios) ? usuarios.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                !string.IsNullOrEmpty(estados) ? estados.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(), !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(), !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                tipoPliegoEnum)
            });
        }
        [HttpGet]
        public ActionResult ObtenerSolpDeSap()
        {
            var desde = new DateTime(2021, 01, 01);
            var hasta = new DateTime(2022, 12, 01);
            while (desde < hasta)
            {
                try
                {
                    Log.Info($"ObtenerSolpesDesdeSAPJob desde {desde} hasta {desde.AddMonths(3)}");
                    ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
                    {
                        FechaDesde = desde,
                        FechaHasta = desde.AddMonths(3),
                        CreadoPorUsuarios = new List<string>()
                    };
                    service.ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);
                    desde = desde.AddMonths(3);
                }
                catch (Exception e)
                {
                    Log.Info($"ObtenerSolpesDesdeSAPJob error");
                    Log.Error(e);
                }
            }
            Log.Info($"ObtenerSolpesDesdeSAPJob fin hasta {hasta}");
            return JsonCustom(new { success = true });
        }

        [HttpGet]
        public ActionResult ListarUsuarioCompras()
        {
            return JsonCustom(new { data = usuarioService.ListarUsuarioCompras() });
        }

        [HttpGet]
        public ActionResult TraerSolpId(int idSolp)
        {
            if (idSolp <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

            var solp = service.TraerSolpId(idSolp);

            if (!string.IsNullOrEmpty(solp.EspecificacionesTecnicas) && System.IO.File.Exists(solp.EspecificacionesTecnicas))
                solp.EspecificacionesTecnicas = System.IO.File.ReadAllText(solp.EspecificacionesTecnicas);
            else
                solp.EspecificacionesTecnicas = null;

            return JsonCustom(new { data = solp });
        }

        [HttpGet]
        public ActionResult BorrarSolp(int idSolp)
        {
            if (idSolp <= 0) return Json(new { info = "Id inválido" }, JsonRequestBehavior.AllowGet);

            return JsonCustom(new { data = service.BorrarSolp(idSolp) });
        }

        [HttpGet]
        public JsonResult GenerarSolpPdf(int idSolp)
        {

            return JsonCustom(File(service.GenerarSolpPdf(idSolp), System.Net.Mime.MediaTypeNames.Application.Octet, "PliegoSolp" + idSolp + ".pdf"));

        }

        [HttpGet]
        public JsonResult ObtenerServiciosSap()
        {

            return JsonCustom(new { data = comprasSapService.ObtenerServiciosSap() });

        }

        [HttpGet]
        public JsonResult AutocompleteTablaSap(string tabla, string valor)
        {

            return JsonCustom(tablaSapService.AutocompleteTablaSap(tabla, valor));

        }

        public ActionResult DescargarZipPliego(int solpId)
        {
            var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);

            string rutaZip = service.GenerarZipPliego(solpId, path, out string mimeType);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            return JsonCustom(File(fileBytes, mimeType, fileName));
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

            string rutaZip = service.GenerarZipPliego(solpId, path, out string mimeType);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);
            string fileExt = Path.GetExtension(fileName);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            return File(fileBytes, mimeType, fileName);
        }

        [ValidateInput(false)]
        public JsonResult ObtenerDatosPorCodigosSap(string codigosSap)
        {

            var codigos = JsonConvert.DeserializeObject<List<TablaSapDto>>(codigosSap);

            return JsonCustom(service.ObtenerDatosPorCodigosSap(codigos));

        }

        [HttpGet]
        public JsonResult AutocompleteServicioSolp(string valor)
        {

            return JsonCustom(service.AutocompleteServicioSolp(valor));

        }

        [HttpGet]
        public JsonResult AutocompleteCodigoServicioSolp(string valor)
        {

            return JsonCustom(service.AutocompleteCodigoServicioSolp(valor));

        }

        [HttpGet]
        public JsonResult AutocompleteProveedor(string valor)
        {
            JsonResult result = JsonCustom(service.AutocompleteProveedor(valor));
            return result;

        }

        [ValidateInput(false)]
        public JsonResult ObtenerDatosPorCodigosSapServicioSolp(string codigosSap)
        {

            var codigos = JsonConvert.DeserializeObject<List<string>>(codigosSap);

            return JsonCustom(service.ObtenerDatosPorCodigosSapServicioSolp(codigos));

        }

        [HttpGet]
        public JsonResult AutocompleteMaterialSolp(string valor, int centroId)
        {

            return JsonCustom(service.AutocompleteMaterialSolp(valor, centroId));

        }

        [HttpGet]
        public JsonResult AutocompleteCodigoMaterialSolp(string valor, int centroId)
        {

            return JsonCustom(comprasSolicitanteService.AutocompleteCodigoMaterialSolp(valor, centroId));

        }

        [HttpPost]
        public ActionResult EnviarEmail(string emailCompose)
        {
            var emailComposeDto = JsonConvert.DeserializeObject<EmailComposeDto>(emailCompose);
            service.EnviarEmailSolp(emailComposeDto);
            return JsonCustom(new { success = true });
        }

        [HttpGet]
        public ActionResult ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro)
        {

            if (string.IsNullOrEmpty(fechaEntregaPosicion)) return Json(new { info = "Fecha entrega posición inválido" }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(numeroMaterial)) return Json(new { info = "Número material inválido" }, JsonRequestBehavior.AllowGet);

            return JsonCustom(new { data = comprasSapService.ListarFuenteAprovisionamiento(fechaEntregaPosicion, numeroMaterial, centro) });

        }

        [HttpGet]
        public ActionResult ObtenerContratoMarco(string numeroContrato, string centro)
        {

            if (string.IsNullOrEmpty(numeroContrato)) return Json(new { info = "Número de contrato inválido" }, JsonRequestBehavior.AllowGet);

            return JsonCustom(new { data = comprasSapService.ObtenerContratoMarco(numeroContrato, centro) });

        }

        [HttpGet]
        public ActionResult ListarPOProveedor(int? pagina = null, int? itemsPorPagina = null, string orden = null, string columna = null, string nroSolp = null, string nroPo = null, string nombrePedido = null,
                    int? estadoLicitacion = null, int? estadoCotizacion = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {

            var ordenar = orden == "ASC" ? DirOrden.Asc : DirOrden.Desc;
            var paginacion = new Paginacion((!string.IsNullOrEmpty(columna) ? columna : "Id"), ordenar, (pagina == null) ? 0 : pagina.Value, (itemsPorPagina == 0 || !itemsPorPagina.HasValue) ? 10 : itemsPorPagina.Value);

            return JsonCustom(new
            {
                data = service.ListarPOProveedor(paginacion, nroSolp, nroPo, nombrePedido, SessionPersister.Mail, fechaDesde, fechaHasta, estadoLicitacion, estadoCotizacion)
            });

        }

        [HttpGet]
        public ActionResult ListarOfertasComprador(int peticionOferta_Id)
        {
            var response = new SustitucionMOAApiResponse<PeticionDeOfertaDto>();

            var usuario = ObtenerUsuarioActual();
            response.Data = service.ListarOfertasComprador(peticionOferta_Id, usuario);

            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult ListarAsociarContrato(string solpJson)
        {

            var posiciones = JsonConvert.DeserializeObject<List<SolpPosicionDto>>(solpJson);
            return JsonCustom(new { data = comprasSolicitanteService.DevolverContratosAsociados(posiciones) });

        }

        [HttpGet]
        public ActionResult ObtenerSolpCompras(int id)
        {
            return JsonCustom(new { data = service.ObtenerSolpCompras(id) });

        }

        [HttpPost]
        public ActionResult GrabarPeticionDeOferta(string json)
        {
            var peticion = JsonConvert.DeserializeObject<GuardarPeticionDeOfertaDto>(json);
            peticion.UsuarioActual = ObtenerUsuarioActual();
            var result = service.GrabarPeticionDeOferta(peticion, Request.Files, true, null);
            return JsonCustom(new { data = result });

        }

        [HttpGet]
        public ActionResult ListarProveedores(string filtro)
        {
            var result = usuarioService.ListarProveedores(filtro);
            return JsonCustom(new { data = result });

        }

        [HttpGet]
        public ActionResult ObtenerLegajo(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor, bool esSolicitante)
        {
            var response = new SustitucionMOAApiResponse<ObtenerLegajoResponse>();

            var mailUsuario = SessionPersister.Mail;
            response.Data = service.ObtenerLegajo(peticionDeOfertaId, idPeticionDeOfertaUsuario, esProveedor, mailUsuario, esSolicitante);

            return ContentCustom(response);
        }

        [ValidateInput(false)]
        public ActionResult GuardarAdjuntosPeticionDeOferta(string idPeticion)
        {

            Resultado result = service.GuardarAdjuntosPeticionDeOferta(int.Parse(idPeticion), Request.Files, ObtenerUsuarioActual());

            return JsonCustom(result);

        }

        public ActionResult DescargarLegajo(int idPeticion, int? idPeticionDeOfertaUsuario, bool esProveedor, int? adjudicacionId, bool esSolicitante)
        {
            var mailUsuario = SessionPersister.Mail;
            var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);
            bool zipVacio = false;
            string rutaZip = service.DescargarLegajo(idPeticion, path, idPeticionDeOfertaUsuario, esProveedor, adjudicacionId, mailUsuario, esSolicitante);

            // Verificar si el archivo ZIP contiene entradas
            using (FileStream zipToOpen = new FileStream(rutaZip, FileMode.Open))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    if (!archivo.Entries.Any())
                    {
                        zipVacio = true;
                    }
                }
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);

            // Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            if (zipVacio)
                return JsonCustom(new { FileContents = new byte[0], success = false, message = "El archivo ZIP está vacío o no contiene entradas." });

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        [HttpGet]
        public JsonResult GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario)
        {
            var pdf = service.GenerarPeticionDeOfertaUsuarioPdf(Math.Abs(idPeticionDeOfertaUsuario));
            return JsonCustom(File(pdf.data, System.Net.Mime.MediaTypeNames.Application.Octet, pdf.name));

        }

        [HttpPost]
        public ActionResult GrabarCircular(string json)
        {
            var circular = JsonConvert.DeserializeObject<CircularDto>(json);
            circular.UsuarioId = ObtenerUsuarioActual().Id;
            var result = service.GrabarCircular(circular, Request.Files);
            return JsonCustom(new { data = result });

        }

        [HttpGet]
        public ActionResult ObtenerPeticionDeOferta(int peticionDeOfertaId)
        {
            var result = service.ObtenerPeticionDeOfertaParaCircular(peticionDeOfertaId);
            return JsonCustom(new { data = result });

        }

        [HttpPost]
        public ActionResult GrabarProveedorEnPeticion(string json)
        {
            var peticion = JsonConvert.DeserializeObject<GuardarPeticionDeOfertaDto>(json);
            peticion.UsuarioActual = ObtenerUsuarioActual();
            var result = service.GrabarProveedoresEnPeticionDeOferta(peticion.UsuarioIds, peticion.Id);
            return JsonCustom(new { data = result });

        }

        [HttpPost]
        public ActionResult CrearOrdenDeCompra(string json)
        {
            var adjudicacion = JsonConvert.DeserializeObject<AdjudicacionDto>(json);
            var result = service.GrabarAdjudicacion(adjudicacion, ObtenerUsuarioActual().Id, "");
            return JsonCustom(result);
        }

        [HttpGet]
        public ActionResult DescargarAdjuntosCotizacion(int cotizacionId, bool desdeRevisionTecnica)
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

        [HttpPost]
        public ActionResult GrabarRevisionTecnica(string json, bool finalizar, string jsonRevision)
        {
            var peticionDeOfertaUsuarioDto = JsonConvert.DeserializeObject<List<PeticionDeOfertaUsuarioDto>>(json);
            var revision = JsonConvert.DeserializeObject<PeticionDeOfertaRevisionTecnicaDto>(jsonRevision);

            var result = service.GrabarRevisionTecnica(peticionDeOfertaUsuarioDto, ObtenerUsuarioActual().Id, finalizar, revision);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ObtenerCotizacion(int peticionDeOfertaId)
        {

            var result = service.TraerCotizacion(peticionDeOfertaId);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult GrabarCotizacion(string json)
        {
            var cotizacion = JsonConvert.DeserializeObject<GuardarCotizacion>(json);
            var result = service.GrabarCotizacion(cotizacion, Request.Files, cotizacion.EsFinalizado, true);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult ObtenerPrecioTotalPosicionProveedor(string json)
        {
            var cotizacion = JsonConvert.DeserializeObject<GuardarCotizacion>(json);
            var result = service.ObtenerPrecioTotalPosicionProveedor(cotizacion);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ObtenerAdjudicacion(string nroOC)
        {
            var result = service.ObtenerAdjudicacion(nroOC);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ListarAdjudicaciones(int solpId)
        {
            var result = adjudicacionesService.ListarAdjudicaciones(solpId);
            return JsonCustom(new { data = result });
        }


        [HttpGet]
        public ActionResult ListarPeticionesDeOferta(int solpId)
        {
            var result = service.ListarPeticionesDeOferta(solpId);
            return JsonCustom(new { data = result });
        }

        static readonly object _lockObtenerOrdenDeCompra = new object();

        [HttpGet]
        public ActionResult ObtenerOrdenDeCompra(string nroOC)
        {
            lock (_lockObtenerOrdenDeCompra)
            {
                var result = service.ObtenerOrdenDeCompra(nroOC);
                return JsonCustom(new { data = result });
            }
        }

        [HttpPost]
        public ActionResult GuardarAdjudicacionAutomatica(string json)
        {
            //Falta probar el metodo
            var registrosInfo = JsonConvert.DeserializeObject<List<RegistroInfoDto>>(json);
            var usuarioActual = ObtenerUsuarioActual();
            var result = service.CrearOrdenDeCompraConRegistroInfo(registrosInfo, usuarioActual.Id);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult CerrarCotizacion(int peticionId, string observaciones)
        {
            var result = service.CerrarCotizacion(peticionId, ObtenerUsuarioActual().Id, observaciones);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ObtenerUltimaSolp()
        {
            var usuarioId = ObtenerUsuarioActual().Id;

            return JsonCustom(new
            {
                data = comprasSolicitanteService.ObtenerUltimaSolp(usuarioId)
            });
        }

        [HttpGet]
        public JsonResult AutocompleteMaterialRFC(string material, string centro, string grupoDeCompras)
        {
            return JsonCustom(comprasSolicitanteService.ObtenerUltimoRegistroMaterialConPrecioBase(material, centro, grupoDeCompras));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ObtenerLegajoParaExternos(int adjudicacionId, string token)
        {
            var mailUsuario = SessionPersister.Mail;
            var result = service.ObtenerLegajoParaExternos(adjudicacionId, token, mailUsuario);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult GrabarPeticionDeOfertaVisualizacionPrecio(string json)
        {
            var peticion = JsonConvert.DeserializeObject<PeticionDeOfertaVisualizacionPrecioDto>(json);
            peticion.UsuarioCreador_Id = ObtenerUsuarioActual().Id;
            var result = service.GrabarPeticionDeOfertaVisualizacionPrecio(peticion, Request.Files);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public JsonResult ObtenerReporteOrdenDeCompra(string nroOC, string fechaDesde, string fechaHasta, string codigoProveedor)
        {
            return JsonCustom(comprasSapService.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor));
        }

        [HttpGet]
        public ActionResult ObtenerChat(int solpId)
        {
            var response = new SustitucionMOAApiResponse<ChatsDto>();
            response.Data = service.ObtenerChat(solpId, ObtenerUsuarioActual().Id);
            return ContentCustom(response);
        }

        [HttpGet]
        public JsonResult ObtenerChatProveedor(int peticionDeOfertaUsuarioId)
        {
            return JsonCustom(service.ObtenerChatProveedor(peticionDeOfertaUsuarioId, ObtenerUsuarioActual().Id));
        }

        [HttpPost]
        public ActionResult GrabarMensajeChatInterno(string json)
        {
            var mensaje = JsonConvert.DeserializeObject<ChatInternoComprasDto>(json);
            mensaje.Usuario_Id = ObtenerUsuarioActual().Id;
            var result = service.GrabarMensajeChatInterno(mensaje);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult GrabarMensajeChatExterno(string json)
        {
            var mensaje = JsonConvert.DeserializeObject<ChatExternoComprasDto>(json);
            mensaje.Usuario_Id = ObtenerUsuarioActual().Id;
            var result = service.GrabarMensajeChatExterno(mensaje);
            return JsonCustom(new { data = result });
        }

        public ActionResult ObtenerYExportarChat(int solpId, int? peticionDeOfertaUsuarioId)
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

        [HttpGet]
        public ActionResult ListarRegionesSap()
        {
            var result = service.ListarRegionesSap();
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public JsonResult ValidarSolpTratada(string nroSolp)
        {
            return JsonCustom(service.ValidarSolpTratada(nroSolp));
        }
        [HttpGet]
        public JsonResult DevolverMonedaProveedor(string codigoProveedor)
        {
            return JsonCustom(service.DevolverMonedaProveedor(codigoProveedor));
        }

        [HttpGet]
        public ActionResult ListarLiberadorSap()
        {
            return JsonCustom(new { data = service.ListarLiberadorSap() });
        }

        [HttpGet]
        public ActionResult ListarUsuarioCreadorSolp()
        {
            return JsonCustom(new { data = usuarioService.ListarUsuarioCreadorSolp(ObtenerUsuarioActual()) });
        }

        [HttpGet]
        public ActionResult ListarFiscalesSolp()
        {
            return JsonCustom(new { data = usuarioService.ListarFiscalesSolp() });
        }

        [HttpPost]
        public ActionResult ListarVisitasDeObra(List<VisitaObraDto> visitas)
        {
            var result = comprasSolicitanteService.ListarVisitasDeObra(visitas);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ListarTablaSap(string codigos)
        {
            List<string> tablas = !string.IsNullOrEmpty(codigos) ? codigos.Split(',').ToList() : new List<string>();
            return JsonCustom(new { data = tablaSapService.ListarTablaSap(tablas) });
        }

        [HttpPost]
        public ActionResult ModificarOrdenDeCompra(string json)
        {
            var adjudicacion = JsonConvert.DeserializeObject<AdjudicacionDto>(json);
            var result = comprasSapService.EditarOrdenDeCompra(adjudicacion);
            return JsonCustom(result);
        }

        [HttpGet]
        public JsonResult ListarClaseDocumento(int usuarioId)
        {
            var result = service.ListarClaseDocumento(usuarioId);
            return JsonCustom(result);
        }

        [HttpPost]
        public ActionResult ActualizarProveedorVisibleEnSolicitante(int peticionDeOfertaUsuarioId, bool esVisible)
        {
            var result = service.ActualizarProveedorVisibleEnSolicitante(peticionDeOfertaUsuarioId, esVisible);
            return JsonCustom(result);
        }

        [HttpGet]
        public ActionResult ObtenerHistorial(int id)
        {
            return JsonCustom(new { data = service.ObtenerHistorial(id) });
        }

        [HttpGet]
        public JsonResult ListarUsuarioSolicitante()
        {
            // Obtener el rol del usuario y saber si es solicitante externo
            var usuario = ObtenerUsuarioActual();
            List<RolDropdownDto> roles = usuarioService.GetRolesUsuario(usuario.Id); // o por email
            var result = comprasSolicitanteService.ListarUsuarioSolicitante(roles, usuario.Id);
            return JsonCustom(result);
        }

        [HttpGet]
        public ActionResult ListarPosicionesPOMultiple(bool? tratada,
                                                       string centros = null,
                                                       string grupoDeCompras = null,
                                                       DateTime? fechaDesde = null,
                                                       DateTime? fechaHasta = null,
                                                       bool sap = false,
                                                       bool mantenimiento = false,
                                                       bool web = false,
                                                       bool repoAutomatica = false,
                                                       bool contratoMarco = false,
                                                       string claseDocumento = null,
                                                       string tipoImputacion = null,
                                                       string valorTipoImputacion = null,
                                                       int? numeroPo = null)
        {
            return JsonCustom(new
            {
                data = service.ListarPosicionesPOMultiple(fechaDesde,
                                                          fechaHasta,
                                                          sap,
                                                          mantenimiento,
                                                          web,
                                                          repoAutomatica,
                                                          tratada,
                                                          contratoMarco,
                                                          !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(),
                                                          !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          numeroPo)
            });
        }

        [HttpGet]
        public ActionResult ListarPosicionesPOMultipleServicio(bool? tratada,
                                                       string centros = null,
                                                       string grupoDeCompras = null,
                                                       DateTime? fechaDesde = null,
                                                       DateTime? fechaHasta = null,
                                                       bool sap = false,
                                                       bool mantenimiento = false,
                                                       bool web = false,
                                                       bool repoAutomatica = false,
                                                       bool contratoMarco = false,
                                                       string claseDocumento = null,
                                                       string tipoImputacion = null,
                                                       string valorTipoImputacion = null,
                                                       int? numeroPo = null,
                                                       string nombrePliego = null,
                                                       string tipoPliego = null)
        {

            TipoPliego tipoPliegoEnum = TipoPliego.All;
            if (!string.IsNullOrWhiteSpace(tipoPliego))
            {
                IEnumerable<int> tipoPliegoList = tipoPliego.Split(',').Select(x => int.Parse(x));
                tipoPliegoEnum = (TipoPliego)tipoPliegoList.Aggregate((x, y) => x | y);
            }

            return JsonCustom(new
            {
                data = service.ListarPosicionesPOMultipleServicio(fechaDesde,
                                                          fechaHasta,
                                                          sap,
                                                          mantenimiento,
                                                          web,
                                                          repoAutomatica,
                                                          tratada,
                                                          contratoMarco,
                                                          !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(),
                                                          !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                          numeroPo,
                                                          nombrePliego,
                                                          tipoPliegoEnum)
            });
        }

        [HttpGet]
        public ActionResult DescargarPosicionesPOMultiple(bool? tratada,
                                                       string centros = null,
                                                       string grupoDeCompras = null,
                                                       DateTime? fechaDesde = null,
                                                       DateTime? fechaHasta = null,
                                                       bool sap = false,
                                                       bool mantenimiento = false,
                                                       bool web = false,
                                                       bool repoAutomatica = false,
                                                       bool contratoMarco = false,
                                                       string claseDocumento = null,
                                                       string tipoImputacion = null,
                                                       string valorTipoImputacion = null,
                                                       int? numeroPo = null)
        {
            MemoryStream data = service.DescargarPosicionesPOMultiple(fechaDesde,
                                                      fechaHasta,
                                                      sap,
                                                      mantenimiento,
                                                      web,
                                                      repoAutomatica,
                                                      tratada,
                                                      contratoMarco,
                                                      !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(),
                                                      !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      numeroPo);

            string fileName = $"PoMUltiple-{DateTime.Today:dd-MM-yyyy}";
            const string contentType = CustomMediaTypeNames.Application.xlsx;

            return JsonCustom(new
            {
                file = data.ToArray(),
                contentType,
                fileName,
            });
        }

        [HttpGet]
        public ActionResult DescargarPosicionesPOMultipleServicio(bool? tratada,
                                                       string centros = null,
                                                       string grupoDeCompras = null,
                                                       DateTime? fechaDesde = null,
                                                       DateTime? fechaHasta = null,
                                                       bool sap = false,
                                                       bool mantenimiento = false,
                                                       bool web = false,
                                                       bool repoAutomatica = false,
                                                       bool contratoMarco = false,
                                                       string claseDocumento = null,
                                                       string tipoImputacion = null,
                                                       string valorTipoImputacion = null,
                                                       int? numeroPo = null,
                                                       string nombrePliego = null)
        {
            MemoryStream data = service.DescargarPosicionesPOMultipleServicio(fechaDesde,
                                                      fechaHasta,
                                                      sap,
                                                      mantenimiento,
                                                      web,
                                                      repoAutomatica,
                                                      tratada,
                                                      contratoMarco,
                                                      !string.IsNullOrEmpty(centros) ? centros.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(grupoDeCompras) ? grupoDeCompras.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(claseDocumento) ? claseDocumento.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      !string.IsNullOrEmpty(tipoImputacion) ? tipoImputacion.Split(',').ToList() : new List<string>(),
                                                      !string.IsNullOrEmpty(valorTipoImputacion) ? valorTipoImputacion.Split(',').Select(x => int.Parse(x)).ToList() : new List<int>(),
                                                      numeroPo,
                                                      nombrePliego);

            string fileName = $"PoMUltiple-{DateTime.Today:dd-MM-yyyy}";
            const string contentType = CustomMediaTypeNames.Application.xlsx;

            return JsonCustom(new
            {
                file = data.ToArray(),
                contentType,
                fileName,
            });
        }

        [HttpGet]
        public ActionResult ListarPosicionesPOMultipleSolpId(int idSolp)
        {
            return JsonCustom(new
            {
                data = service.ListarPosicionesPOMultipleSolpId(idSolp)
            });
        }

        [HttpGet]
        public ActionResult ListarSubPosicionesPOMultipleSolpId(int idPosicion)
        {
            return JsonCustom(new
            {
                data = service.ListarSubPosicionesPOMultipleSolpId(idPosicion)
            });
        }

        [HttpGet]
        public ActionResult ObtenerPosicionesMultipleCompras(string ids)
        {
            List<int> listaId = ids.Replace("[", "").Replace("]", "").Split(',').Select(x => int.Parse(x)).ToList();
            SolpCompraDto resultado = service.ObtenerPosicionesMultipleCompras(listaId);

            return JsonCustom(new { data = resultado });
        }

        [HttpGet]
        public ActionResult ListarHistorialDeFechas(int peticionId)
        {
            var resultado = service.ListarHistorialDeFechas(peticionId);
            return JsonCustom(new { data = resultado });
        }

        [HttpPost]
        public ActionResult MarcarChatProveedorComoLeido(string json)
        {
            var proveedorChat = JsonConvert.DeserializeObject<ChatProveedoresDto>(json);
            service.MarcarChatProveedorComoLeido(proveedorChat);
            return JsonCustom(new { success = true });
        }

        [HttpGet]
        public ActionResult ListarSolpCondicionEspecial(string filtroJson)
        {
            var filtro = JsonConvert.DeserializeObject<FiltroDto>(filtroJson);
            var resultado = service.ListarSolpCondicionEspecial(filtro);
            return JsonCustom(new { data = resultado });
        }

        [HttpPost]
        public ActionResult AgruparPeticionesDeOferta(string peticionDeOfertaIds)
        {
            var usuario = ObtenerUsuarioActual();
            var result = service.AgruparPeticionesDeOferta(usuario.Id, peticionDeOfertaIds);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult ValidarPrecioCotizado(string json)
        {
            var adjudicacion = JsonConvert.DeserializeObject<AdjudicacionDto>(json);
            var result = service.ValidarPrecioCotizado(adjudicacion);
            return JsonCustom(new { data = result });
        }

        [HttpGet]
        public ActionResult ObtenerAdjuntosSolpAgrupar(string nroSolp)
        {
            return JsonCustom(new { data = service.ObtenerAdjuntosSolpAgrupar(nroSolp) });
        }

        [HttpPost]
        public ActionResult DesagruparPeticionDeOferta(string nroSolp, string po)
        {
            var result = service.DesagruparPO(nroSolp, po);
            return JsonCustom(new { data = result });
        }

        [HttpPost]
        public ActionResult DesvincularSolpDePOMultipleMaterial(int solpPosicionId, string[] idsPOsADesvincular)
        {
            var idsPOsADesvincularInt = idsPOsADesvincular.Select(x => int.Parse(x)).ToList();

            service.DesvincularSolpDePOMultipleMaterial(solpPosicionId, idsPOsADesvincularInt);
            return JsonCustom(new { data = true });
        }

        [HttpPost]
        public ActionResult DesvincularSolpDePOMultipleServicio(int solpId, string[] idsPOsADesvincular)
        {
            var idsPOsADesvincularInt = idsPOsADesvincular.Select(x => int.Parse(x)).ToList();

            service.DesvincularSolpDePOMultipleServicio(solpId, idsPOsADesvincularInt);
            return JsonCustom(new { data = true });
        }

        [HttpGet]
        public ActionResult ObtenerPeticionesDeOfertaParaDesvincularMaterial(int solpPosicionId)
        {
            var peticiones = service.ObtenerPeticionesDeOfertaParaDesvincularMaterial(solpPosicionId);
            return JsonCustom(new { data = peticiones });
        }

        [HttpGet]
        public ActionResult ObtenerPeticionesDeOfertaParaDesvincularServicio(int solpId)
        {
            var peticiones = service.ObtenerPeticionesDeOfertaParaDesvincularServicio(solpId);
            return JsonCustom(new { data = peticiones });
        }

        [HttpPost]
        public ActionResult GuardarEnvioCircularProveedor(int id, EnviarCircularEnum enviarCircularA, string fechaLimite)
        {
            DateTime? fechaLimiteD;

            if (string.IsNullOrWhiteSpace(fechaLimite))
            {
                if (enviarCircularA != EnviarCircularEnum.NoEnviar)
                {
                    throw new ValidationCustomException("La fecha límite es obligatoria cuando se enviará una circular");
                }
                fechaLimiteD = null;
            }
            else
            {
#pragma warning disable IDE0018 // Inline variable declaration <--> se deba actualizar langVersion.
                DateTime fl;
#pragma warning restore IDE0018 // Inline variable declaration
                if (!DateTime.TryParse(fechaLimite, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces, out fl))
                {
                    throw new ValidationCustomException("La fecha límite no es válida.");
                }
                else
                {
                    fechaLimiteD = fl; // estas asignaciones raras las tengo que hacer porque el DateTime.TryParse no permite hacer DateTime? (language version??).
                }
            }

            var result = service.GuardarEnvioCircularProveedor(id, enviarCircularA, fechaLimiteD);
            return JsonCustom(new { data = result });
        }
        [HttpGet]
        public ActionResult ValidarFechaVigenciaRegistroInfo([HttpHelper.FromUri] ValidarFechaVigenciaRegistroInfoReqDto request)
        {
            return JsonCustom(new
            {
                data = service.ValidarFechaVigenciaRegistroInfo(request)
            });
        }
        [HttpPost]
        public ActionResult ActualizarFechaVigenciaRegistroInfo(string data)
        {
            var dataDeserialized = JsonConvert.DeserializeObject<ActualizarFechaVigenciaRegistroInfoDto>(data);
            service.ActualizarFechaVigenciaRegistroInfo(dataDeserialized);
            return JsonCustom(new
            {
                data = true
            });
        }

        [HttpGet]
        public ActionResult DescargarHistorialCotizaciones(int cotizacionId)
        {
            var response = new SustitucionMOAApiResponse<string>();
            var ms = service.GenerarHistorialCotizaciones(cotizacionId);
            return JsonCustom(File(ms, System.Net.Mime.MediaTypeNames.Application.Octet, "HistorialCotizaciones.xlsx"));
            return ContentCustom(response);
        }

        public ActionResult DescargarArchivoHistorialMovimientos(int idPeticionOferta)
        {
            var ms = service.GenerarExcelHistorialMovimientos(idPeticionOferta);
            return JsonCustom(File(ms, System.Net.Mime.MediaTypeNames.Application.Octet, "HistorialMovimientos.xlsx"));
        }

        [HttpGet]
        public ActionResult DescargarRevisionTecnica(int peticionDeOfertaId)
        {
            var response = new SustitucionMOAApiResponse<string>();
            var ms = service.GenerarArchivoRevisionTecnica(peticionDeOfertaId);
            return JsonCustom(File(ms, System.Net.Mime.MediaTypeNames.Application.Octet, "RevisionTecnica.xlsx"));
            return ContentCustom(response);
        }

        [HttpGet]
        public ActionResult DescargarAdjuntosProveedores(int idPeticion, int? idPeticionDeOfertaUsuario)
        {
            var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
            Directory.CreateDirectory(path);

            string rutaZip = service.DescargarAdjuntosProveedores(idPeticion, path, idPeticionDeOfertaUsuario);
            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaZip);
            string fileName = Path.GetFileName(rutaZip);

            //Para evitar sobrecargar el server con zips, una vez cargado lo borro
            Directory.Delete(path, true);

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

        [HttpPost]
        public ContentResult ProcesarPrecargaSolp(HttpPostedFileBase archivo, int tipoSolpId)
        {
            var response = new SustitucionMOAApiResponse<ProcesarPrecargaSolpResponse>
            {
                Data = service.ProcesarArchivoPrecargaSolp(archivo, tipoSolpId)
            };
            return ContentCustom(response);
        }

        [HttpPost]
        public ActionResult GuardarCertificacionesParciales(List<AdjudicacionDto> adjudicaciones)
        {
            service.GuardarCertificacionesParciales(adjudicaciones);
            return JsonCustom(new SustitucionMOAApiResponse());
        }

        [CustomPermisoAuthorize(Roles = Permiso.CARGAR_FACT_PROV)]
        [HttpGet]
        public ActionResult ObtenerReporteFacturasCertificaciones(
            string fechaInicio,
            string fechaFin,
            int? pagina = null,
            int? itemsPorPagina = null,
            string orden = null,
            string columna = null,
            string ordenDeCompra = null,
            string proveedor = null)
        {
            try
            {
                var mailUsuario = SessionPersister.Mail;

                var certificacionesPaginadas = facturaService.ObtenerReporteFacturasCertificaciones(
                    fechaInicio,
                    fechaFin,
                    mailUsuario,
                    ordenDeCompra,
                    proveedor,
                    itemsPorPagina,
                    pagina,
                    orden,
                    columna
                );

                return JsonCustom(certificacionesPaginadas);
            }
            catch (Exception ex)
            {
                Log.Error($"Error al obtener reporte de facturas: {ex.Message}", ex);
                return JsonCustom(new { error = "Error al obtener certificaciones", mensaje = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ObtenerOrganizacionesDeCompra()
        {
            var organizacionesDeCompra = new SustitucionMOAApiResponse<List<OrganizacionDeCompraDto>>
            {
                Data = service.ObtenerOrganizacionesDeCompra()
            };
            return ContentCustom(organizacionesDeCompra);
        }
    }
}