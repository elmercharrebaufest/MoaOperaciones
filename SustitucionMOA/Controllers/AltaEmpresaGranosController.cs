using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
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
        public ActionResult GenerarInformeComercial_OLD(string _EmplRelDep, string EmplRelDepCant, string _Rodados, string RodadosOtros, string _Chacra, string ChacraOtros,
                                                    string AntigActividad, string ActuacionProd, string ClienteAnt, string Comentarios, string Domicilio)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                userMail = userMail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : userMail;

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedorActual();

                ParamInformeComercial informeComercial = new ParamInformeComercial();

                bool EmplRelDep = false;
                int Rodados = 0;
                int Chacra = 0;

                bool.TryParse(_EmplRelDep, out EmplRelDep);
                int.TryParse(_Rodados, out Rodados);
                int.TryParse(_Chacra, out Chacra);

                informeComercial.EmplRelDep = EmplRelDep;
                informeComercial.EmplRelDepCant = EmplRelDepCant;
                informeComercial.Rodados = Rodados;
                informeComercial.RodadosOtros = RodadosOtros;
                informeComercial.Chacra = Chacra;
                informeComercial.ChacraOtros = ChacraOtros;
                informeComercial.AntigActividad = AntigActividad;
                informeComercial.ActuacionProd = ActuacionProd;
                informeComercial.ClienteAnt = ClienteAnt;
                informeComercial.Comentarios = Comentarios;
                informeComercial.Domicilio = Domicilio;

                informeComercial.ProveedorId = proveedor.Id;
                informeComercial.InformeComercialId = 0;

                var nuevosCampos = new List<NuevoProduccion>();

                informeComercial.Materiales.Add(new ParamInformeComercialMaterial()
                {
                    MaterialId = 4,
                    Toneladas = 100
                });

                nuevosCampos.Add(new NuevoProduccion
                {
                    CampañaID = 8,
                    ArrendaPropia = true,
                    Hectareas = 100,
                    LocalidadId = 100,
                    MaterialId = 4,
                    Toneladas = 100
                });

                var nuevosAcopios = new List<NuevoAcopio>();

                informeComercial.ComercialID = (int)proveedor.IdComercialDataAgro;

                informeComercial.NuevosCampos = nuevosCampos;
                informeComercial.NuevosAcopios = nuevosAcopios;

                var FileArray = altaEmpresaService.GenerarInformeComercial(informeComercial);

                return File(FileArray, "application/pdf", "Informe Comercial.pdf");
                /*    PDFResponse result = new PDFResponse
                    {
                        pdf = new Pdf()
                        {
                            data = FileArray
                        }
                    };

                    return JsonCustom(result.pdf);
                    return File(FileArray, "application/pdf", "Informe Comercial.pdf");*/
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


        public ActionResult GenerarInformeComercial(string informeComercialJson)
        {
            try
            {
                var informeComercial = JsonConvert.DeserializeObject<ParamInformeComercial>(informeComercialJson);

                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                userMail = userMail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : userMail;

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedorActual();

                informeComercial.ProveedorId = proveedor.Id;
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

                var FileArray = altaEmpresaService.GenerarInformeComercial(informeComercial);

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
                    Nombre = x.Nombre,
                },
                 x => x.Nombre.Contains(localidad)
                 , 500).OrderBy(x => x.Nombre).ToList();

                //var jsonLocalidad = JsonConvert.SerializeObject(listadoLocalidad);

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
                var fileSubido = Request.Files[0];
                var fileKey = Request.Form.Get("fileKey");
                if (fileSubido.ContentLength > 0)
                {
                    altaEmpresaService.GuardarArchivo(fileSubido, fileKey, mail);
                }

                return Json(new { info = "Todo OK" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult ObtenerArchivosSubidos()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                mail = mail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mail;

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

        public ActionResult DescargarArchivo(string fileKey)
        {

            string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            mail = mail.IsNullOrWhiteSpace() ? "mpfeiffer@baufest.com" : mail;

            string rutaArchivoSubido = altaEmpresaService.ObtenerArchivo(mail, fileKey);

            byte[] fileBytes = System.IO.File.ReadAllBytes(rutaArchivoSubido);
            string fileName = Path.GetFileName(rutaArchivoSubido);
            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
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
