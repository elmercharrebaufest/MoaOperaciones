using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SustitucionMOA.Controllers
{
    public class AltaEmpresaGranosController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly IAltaEmpresaGranosService altaEmpresaService;

        public AltaEmpresaGranosController(IAltaEmpresaGranosService altaEmpresaService, IRepositorio repositorio)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.repositorio = repositorio;
        }
       
        public ActionResult GenerarInformeComercial(string informeComercialJson)
        {
            try
            {
                informeComercialJson = informeComercialJson.Replace("nia","ña");
                var informeComercial = JsonConvert.DeserializeObject<ParamInformeComercial>(informeComercialJson);

                foreach (var item in informeComercial.NuevosCampos)
                {

                }
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                userMail = userMail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : userMail;

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedorActual();

                informeComercial.ProveedorId = (int)proveedor.IdDataAgro;
                informeComercial.InformeComercialId = 0;

                informeComercial.ComercialID = (int)proveedor.IdComercialDataAgro;

                if (informeComercial.NuevosCampos != null)
                {
                    foreach (var nuevosCampos in informeComercial.NuevosCampos)
                    {
                        informeComercial.Materiales.Add(new ParamInformeComercialMaterial
                        {
                            MaterialId = nuevosCampos.MaterialId,
                            Toneladas = nuevosCampos.Toneladas
                        });
                    }
                }

                var FileArray = altaEmpresaService.GenerarInformeComercial(informeComercial, userMail);

                //return File(FileArray, "application/pdf", "Informe Comercial.pdf");
                PDFResponse result = new PDFResponse
                {
                    pdf = new Pdf()
                    {
                        data = FileArray
                    }
                };

                return JsonCustom(result.pdf);
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

        public ActionResult GetLocalidadCombo(string localidad)
        {
            localidad = localidad.IsNullOrWhiteSpace() ? "" : localidad;

            if (localidad.Length > 2)
            {
                var listadoLocalidad = repositorio.Listar<Localidad, LocalidadCombo>(x => new LocalidadCombo()
                {
                    LocalidadId = x.LocalidadId,
                    Nombre = x.Nombre + " (" + x.Provincia.Nombre + ")",
                },
                 x => x.Nombre.Contains(localidad)
                 , 500).OrderBy(x => x.Nombre).ToList();

                return JsonCustom(listadoLocalidad);
            }

            return JsonCustom("");
        }

        public ActionResult GetMateriales()
        {
            try
            {
                return JsonCustom(altaEmpresaService.ObtenerMaterialesDataAgro());
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

        [HttpPost]
        public ActionResult GuardarArchivo()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                
                var fileKey = Request.Form.Get("fileKey");

                List<string> errores = new List<string>();

                for (int i = 0; i < Request.Files.Count; i++)
                { 
                    var fileSubido = Request.Files[i];

                    if (fileSubido.ContentLength > 0)
                    {
                        var result = altaEmpresaService.GuardarArchivo(fileSubido, fileKey, mail);

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

                if ( errores.Count > 0)
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

        public ActionResult ObtenerArchivosSubidos(string mail)
        {
            try
            {
                if (string.IsNullOrEmpty(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(altaEmpresaService.ObtenerArchivosSubidos(mail));
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

        public ActionResult ObtenerCBUSISA()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(altaEmpresaService.ObtenerCBUSISA(mail));
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

        public ActionResult DescargarArchivo(string fileKey, string mail, int archivoID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                mail = mail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mail;
                string rutaArchivoSubido = altaEmpresaService.ObtenerArchivo(mail, fileKey, archivoID);

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

        public ActionResult EliminarArchivo(string fileKey, int archivoID)
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                string result = altaEmpresaService.EliminarArchivo(mail, fileKey, archivoID);

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

        public ActionResult EnviarSolicitudUsuario()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                mail = mail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mail;

                return JsonCustom(altaEmpresaService.EnviarSolicitudUsuario(mail));
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
