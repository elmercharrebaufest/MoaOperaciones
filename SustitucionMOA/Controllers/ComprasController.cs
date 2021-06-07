using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
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

                return JsonCustom(service.GuardarSolp(solp, Request.Files));
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
                    CamposObligatoriosCabeceraSolp = service.ObtenerTablaGeneral(TablasGenerales.CamposObligatoriosCabeceraSolp).Where(x => x.IdPadre.HasValue).Select(x => new
                    {
                        ClaseDocumentoCodigo = x.Padre.Codigo,
                        Codigo = x.Codigo
                    })
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
    }
}