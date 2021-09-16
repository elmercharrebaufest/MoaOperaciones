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
    public class AltaEmpresaGranosController : BaseController
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;
        readonly IAltaEmpresaGranosService altaEmpresaService;

        public AltaEmpresaGranosController(IAltaEmpresaGranosService altaEmpresaService, IDataAgroService dataAgroService, IRepositorio repositorio)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.dataAgroService = dataAgroService;
            this.repositorio = repositorio;
        }

        public ActionResult GenerarInformeComercial(string informeComercialJson, int proveedorId)
        {
            try
            {
                informeComercialJson = informeComercialJson.Replace("nia", "ña");
                var informeComercial = JsonConvert.DeserializeObject<ParamInformeComercial>(informeComercialJson);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");


                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                if (proveedorId == 0)
                {
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                proveedor = repositorio.Obtener<Proveedor>(proveedorId);

                var infoProveedor = altaEmpresaService.ObtenerInfoProveedor(userMail, proveedorId);

                if (infoProveedor.ProveedorClasificacion == "Productor")
                {
                    if (!informeComercial.NuevosCampos.Any())
                    {
                        throw new ValidationCustomException("Para generar el informe comercial debe informar los campos");
                    }
                }


                informeComercial.ContactoComercial.Email1 = proveedor.Mail;

                //Para los proveedores que hicieron el alta con los flujos, tenemos el IDDataAgro y IDComercial. Para los migrados no. Por esto, lo vamos a buscar
                if (proveedor.IdDataAgro == null)
                {
                    informeComercial.ProveedorId = (int)proveedor.IdDataAgro;
                    informeComercial.ComercialID = (int)proveedor.IdComercialDataAgro;
                }
                else
                {
                    var proveedorDAO = dataAgroService.ObtenerValidarCUITProveedorGranos(proveedor.CUIT);
                    informeComercial.ProveedorId = (int)proveedorDAO.ProveedorId;
                    informeComercial.ComercialID = proveedorDAO.ComercialId;
                }

                informeComercial.InformeComercialId = 0;

                if (informeComercial.NuevosCampos != null)
                {
                   /* foreach (var nuevosCampos in informeComercial.NuevosCampos)
                    {
                        informeComercial.Materiales.Add(new ParamInformeComercialMaterial
                        {
                            MaterialId = nuevosCampos.MaterialId,
                            Toneladas = nuevosCampos.Toneladas
                        });
                    }*/

                    foreach (var nuevosCampos in informeComercial.NuevosCampos.GroupBy(x => x.MaterialId))
                    {
                        informeComercial.Materiales.Add(new ParamInformeComercialMaterial
                        {
                            MaterialId = nuevosCampos.First().MaterialId,
                            Toneladas = nuevosCampos.Sum(x => x.Toneladas)
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GrabarNuevoProveedorGranos(string cuit, string mailVendedor)
        {
            try
            {
                var userMail = SessionPersister.getUsername();

                return JsonCustom(new
                {
                    data = altaEmpresaService.GrabarProveedorAltaInternaGranos(cuit, userMail, mailVendedor)
                });
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


        public ActionResult GenerarCartaPresentacion(string cartaPresentacionJson, int proveedorId)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == userMail);
                if (proveedorId == 0)
                {
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                var corredor = usuario.ObtenerCorredor();

                var proveedor = usuario.ObtenerProveedorPorId(proveedorId);

                cartaPresentacionJson = cartaPresentacionJson.Replace("nia", "ña");
                var cartaPresentacion = JsonConvert.DeserializeObject<RptCartaDePresentacionInfo>(cartaPresentacionJson);

                cartaPresentacion.corredorCuit = corredor.CUIT;
                cartaPresentacion.corredorRazonSocial = corredor.RazonSocial;

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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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

        public ActionResult GetLocalidad(int localidadId)
        {
            try
            {
                return JsonCustom(altaEmpresaService.GetLocalidad(localidadId));
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

        public async Task<ActionResult> GetMateriales()
        {
            try
            {
                return JsonCustom(await altaEmpresaService.ObtenerMaterialesDataAgro());
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> GetCampanias()
        {
            try
            {
                string CampanaMin = ConfigurationManager.AppSettings["CampanaMin"].ToString();
                var datosjson = await altaEmpresaService.ObtenerCampañasDataAgroAsync();
                var listCamp = JsonConvert.DeserializeObject<List<CampaniaDto>>(datosjson);

                var idCamp = listCamp.Where(a => a.Descripcion == CampanaMin).Single().CampaniaId;
                listCamp = listCamp.Where(a => a.CampaniaId >= idCamp).ToList();
                return JsonCustom(listCamp);

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
        public ActionResult GuardarArchivo()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);

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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                string rutaArchivoSubido = altaEmpresaService.ObtenerArchivo(mail, archivoID, proveedorId);

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

        public ActionResult DescargarArchivos(string mail, int proveedorId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
                    proveedorId = usuario.ObtenerProveedor().Id;
                }
                
                var path = $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{DateTime.Now.Ticks}";
                Directory.CreateDirectory(path);

                string rutaZip= altaEmpresaService.ObtenerArchivos(mail, proveedorId, path);
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

        public ActionResult EliminarArchivo(int archivoID, int proveedorId)
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
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
        public ActionResult EnviarSolicitudUsuario(int proveedorId, string datosJson)
        {
            try
            {
                var altaEmpresa = JsonConvert.DeserializeObject<AltaEmpresaViewModel>(datosJson);

                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(altaEmpresaService.EnviarSolicitudUsuario(mail, proveedorId, false, altaEmpresa));
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


        public ActionResult CargarSolicitudUsuario(string mail, int proveedorId)
        {
            try
            {
                if (string.IsNullOrEmpty(mail))
                    mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                if (proveedorId == 0)
                {
                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == mail);
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
        public ActionResult SolicitudAltaInterna(int proveedorId, string datosJson)
        {
            try
            {
                string mail = SessionPersister.getUsername();
                var altaEmpresa = JsonConvert.DeserializeObject<AltaEmpresaViewModel>(datosJson);

                return JsonCustom(altaEmpresaService.SolicitudAltaInterna(mail, proveedorId, altaEmpresa));
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
    }
}
