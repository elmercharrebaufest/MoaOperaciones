using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class ComprasController : BaseController
    {
        private readonly IComprasService service;
        private readonly IUsuarioService usuarioService;

        public ComprasController(IComprasService comprasService, IUsuarioService usuarioService)
        {
            this.service = comprasService;
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
                catch(Exception e)
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
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
                    Moneda = service.ObtenerTablaSap(TablasSap.Moneda),
                    Unidades = service.ObtenerTablaSap(TablasSap.Unidad),
                    EstadosSolpSap = service.ObtenerTablaSap(TablasSap.EstadoSolpSap),
                    CentroBeneficio = service.ObtenerTablaSap(TablasSap.CentroBeneficio),
                    EstadoDocumento = service.ObtenerTablaEstado(TablasEstado.EstadoDocumento),
                    TipoPosicionSolp = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp),
                    TipoPosicion = service.ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp),
                    TipoImputacion = service.ObtenerTablaGeneral(TablasGenerales.TipoImputacionSolp),
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
        public ActionResult ListarSolp()
        {   
            try
            {               
                return JsonCustom(new { data = service.ListarSolp(ObtenerUsuarioActual())});
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

        //[HttpGet]
        //public ActionResult FiltrarMateriales() {
        //    try
        //    {
        //        return JsonCustom(new { data = service.FiltrarMateriales() });
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        [HttpGet]
        public ActionResult ObtenerSolpDeSap()
        {
            try
            {
                //ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
                //{
                //    FechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioObtenerSolpsDesdeSAPJob"].ToString()),
                //    FechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinObtenerSolpsDesdeSAPJob"].ToString()),
                //    CreadoPorUsuarios = new List<string>(),
                //    NumeroSolp = "0212201893"
                //};
                //service.ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);
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
                return JsonCustom(new { data = service.ListarUsuarioCompras(ObtenerUsuarioActual()) });
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
                return JsonCustom( service.AutocompleteTablaSap(tabla, valor) );
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
                                                        ? "Solp no disponible para descarga." 
                                                        : "Token no coincide, no tiene permiso para realizar la descarga";
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
        public ActionResult ObtenerContratoMarco(string numeroContrato)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroContrato)) return Json(new { info = "Numero Contrato inválido" }, JsonRequestBehavior.AllowGet);             

                return JsonCustom(new { data = service.ObtenerContratoMarco(numeroContrato) });
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