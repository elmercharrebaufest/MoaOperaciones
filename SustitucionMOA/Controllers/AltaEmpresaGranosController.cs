using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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
                informeComercialJson = informeComercialJson.Replace("nia", "ña");
                var informeComercial = JsonConvert.DeserializeObject<ParamInformeComercial>(informeComercialJson);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                userMail = userMail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : userMail;

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedor();

                var proveedorId = proveedor.Id;

                var infoProveedor = altaEmpresaService.ObtenerInfoProveedor(userMail, 0);

                if (infoProveedor.ProveedorClasificacion == "Productor")
                {
                    if (!informeComercial.NuevosCampos.Any())
                    {
                        throw new ValidationCustomException("Para generar el informe comercial debe informar los campos");
                    }
                }

                informeComercial.ContactoComercial.Email1 = userMail;
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

                var FileArray = altaEmpresaService.GenerarInformeComercial(informeComercial, userMail, proveedorId);

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

        public ActionResult GenerarCartaPresentacion(string cartaPresentacionJson, int proveedorId)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                var cartaPresentacion = JsonConvert.DeserializeObject<RptCartaDePresentacionInfo>(cartaPresentacionJson);

                cartaPresentacion.corredorCuit = proveedor.CUIT;
                cartaPresentacion.corredorRazonSocial = proveedor.RazonSocial;

                if (cartaPresentacion.vendedorActividad == "Productor")
                {
                    if (!cartaPresentacion.NuevosCampos.Any())
                    {
                        throw new ValidationCustomException("Para generar la carta de presentacion debe informar los campos");
                    }
                }

                if (cartaPresentacion.vendedorActividad == "Productor")
                {
                    if (!cartaPresentacion.NuevosCampos.Any())
                    {
                        throw new ValidationCustomException("Para generar la carta de presentacion debe informar los almacenamientos");
                    }
                }

                var FileArray = altaEmpresaService.GenerarCartaDePresentacion(cartaPresentacion, userMail, proveedorId);

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

                var proveedorId = int.Parse(Request.Form.Get("proveedorId"));

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
                if (string.IsNullOrEmpty(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(altaEmpresaService.ObtenerArchivosSubidos(mail, proveedorId));
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
                //string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

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
                mail = mail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mail;

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
