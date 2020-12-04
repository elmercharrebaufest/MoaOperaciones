using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SustitucionMOA.Controllers
{
    public class AltaEmpresaNoGranosController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly IAltaEmpresaNoGranosService altaEmpresaNoGranosService;
        readonly IAltaEmpresaGranosService altaEmpresaService;

        public AltaEmpresaNoGranosController(IAltaEmpresaGranosService altaEmpresaService, IRepositorio repositorio, IAltaEmpresaNoGranosService altaEmpresaNoGranosService)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.altaEmpresaNoGranosService = altaEmpresaNoGranosService;
            this.repositorio = repositorio;
        }

       
        [HttpPost]
        public ActionResult GuardarArchivo()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);

                var fileKey = Request.Form.Get("fileKey");

                var proveedorId = int.Parse(Request.Form.Get("proveedorId"));

                if (proveedorId == 0)//para el caso de los directo que no tienen mas de un proveedor
                {
                    proveedorId = usuario.ObtenerProveedor().Id;
                }

                List<string> errores = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var fileSubido = Request.Files[i];

                    if (fileSubido.ContentLength > 0)
                    {
                        var result = altaEmpresaService.GuardarArchivo(fileSubido, fileKey, mail, proveedorId);

                        if (!result.Equals(SuccessMsg.ArchivoSubidoOK))
                        {
                            errores.Add(string.Concat("Ocurrió un error con el archivo ", fileSubido.FileName, ": ", result));
                        }
                    }
                    else
                    {
                        errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
                    }
                }

                if (errores.Count > 0)
                {
                    return JsonCustom(new { info = errores });
                }

                return JsonCustom(new { data = SuccessMsg.ArchivoSubidoOK });

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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerArchivosSubidos(string mail, int proveedorId)
        {
            try
            {
                var esOperador = false;

                //Si viene con un mail, significa que está siendo consultado por un operador, por lo tanto paso todos los archivos
                if (string.IsNullOrEmpty(mail))
                {
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                }
                else
                {
                    esOperador = true;
                }
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                return JsonCustom(altaEmpresaService.ObtenerArchivosSubidos(mail, proveedorId, esOperador));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ObtenerInfoProveedor(string mail, int proveedorId)
        {
            try
            {
                if (string.IsNullOrEmpty(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                return JsonCustom(altaEmpresaService.ObtenerInfoProveedor(mail, proveedorId));

            }
            catch (InfoCustomException e)
            {
                return Json(new
                {
                    info = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DescargarArchivo(string mail, int archivoID, int proveedorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                string rutaArchivoSubido = altaEmpresaService.ObtenerArchivo(mail, archivoID, proveedorId);

                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
                string fileName = Path.GetFileName(rutaArchivoSubido);
                return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EliminarArchivo(int archivoID, int proveedorId)
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                string result = altaEmpresaService.EliminarArchivo(mail, archivoID, proveedorId);

                return JsonCustom(result);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EnviarSolicitudUsuario(int proveedorId, string datosJson)
        {
            try
            {
                var altaEmpresa = JsonConvert.DeserializeObject<AltaEmpresaViewModel>(datosJson);

                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(altaEmpresaService.EnviarSolicitudUsuario(mail, proveedorId, altaEmpresa));
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CargarSolicitudUsuario(string mail, int proveedorId)
        {
            try
            {
                if (string.IsNullOrEmpty(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioNoGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                AltaEmpresaViewModel result = altaEmpresaService.CargarSolicitudUsuario(mail, proveedorId);
                return JsonCustom(result);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
