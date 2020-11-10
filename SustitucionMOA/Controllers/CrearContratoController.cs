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
    public class CrearContratoController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly ICrearContratoService crearContratoService;

        public CrearContratoController(ICrearContratoService crearContratoService, IRepositorio repositorio)
        {
            this.crearContratoService = crearContratoService;
            this.repositorio = repositorio;
        }

        public ActionResult GetLocalidadCombo(string localidad)
        {
            localidad = localidad.IsNullOrWhiteSpace() ? "" : localidad;

            if (localidad.Length > 2)
            {
                var listadoLocalidad = repositorio.Listar<Localidad, LocalidadCombo>(x => new LocalidadCombo()
                {
                    LocalidadId = x.LocalidadId,
                    ProvinciaId = x.ProvinciaId,
                    Nombre = x.Nombre + " (" + x.Provincia.Nombre + ")",
                },
                 x => x.Nombre.Contains(localidad)
                 , 500).OrderBy(x => x.Nombre).ToList();

                return JsonCustom(listadoLocalidad);
            }

            return JsonCustom("");
        }
        public ActionResult ObteneDatosContrato(int tiponegocio)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;

                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);
                return JsonCustom(crearContratoService.ObteneDatosContrato(tiponegocio));
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
        public ActionResult ObtenerDatosCompraNet(int? idProveedorDataAgro)
        {
            try
            {
                if (idProveedorDataAgro.HasValue)
                {
                    return JsonCustom(crearContratoService.ObtenerDatosCompraNet(idProveedorDataAgro.Value));
                }
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;
                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);
                return JsonCustom(crearContratoService.ObtenerDatosCompraNet(proveedor.IdDataAgro.Value));
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
        public ActionResult CrearContratoAPrecio(string contrato)
        {
            try
            {
                contrato = contrato.Replace("nia", "ña");
                var contratoAPrecio = JsonConvert.DeserializeObject<ContratoAPrecio>(contrato);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;
                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);

                if (contratoAPrecio.CorredorId == null)
                {
                    contratoAPrecio.ProveedorId = (int)proveedor.IdDataAgro;
                }

                contratoAPrecio.ProveedorCreadorId = (int)proveedor.IdDataAgro;
                contratoAPrecio.ComercialCreadorId = null;
                contratoAPrecio.MonedaSustentable = "USDM ";
                contratoAPrecio.ContratoSAP = "";
                contratoAPrecio.CantidadCamiones = null;


                string result = crearContratoService.CrearContratoAPrecio(contratoAPrecio);

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
        public ActionResult CrearContratoAFijar(string contrato)
        {
            try
            {
                contrato = contrato.Replace("nia", "ña");
                var contratoAFijar = JsonConvert.DeserializeObject<ContratoAFijar>(contrato);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;
                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);

                if (contratoAFijar.CorredorId == null)
                {
                    contratoAFijar.ProveedorId = (int)proveedor.IdDataAgro;
                }
                contratoAFijar.ProveedorCreadorId = (int)proveedor.IdDataAgro;
                contratoAFijar.ComercialCreadorId = null;
                contratoAFijar.MonedaSustentable = "USDM ";
                contratoAFijar.ContratoSAP = "";
                contratoAFijar.CantidadCamiones = null;


                string result = crearContratoService.CrearContratoAFijar(contratoAFijar);

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
        public ActionResult ValidarDirecto()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;

                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);
                var directo = crearContratoService.ValidarDirecto(proveedor.CUIT);
                int result = 0;
                if (directo == "false")
                {
                    result = proveedor.IdDataAgro ?? 0;
                }
                return JsonCustom(result);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult BuscarProveedoresConCorredor(string filtro)
        {
            try
            {
                filtro = filtro.IsNullOrWhiteSpace() ? "" : filtro;

                if (filtro.Length > 2)
                {
                    string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                    var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                    string codigoProveedor = SessionPersister.Proveedor;

                    var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor); 
                    return JsonCustom(crearContratoService.BuscarProveedoresConCorredor(filtro, proveedor.CUIT));
                }
                else
                {
                    return JsonCustom("");

                }

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
        public ActionResult Habilitaciones(int material, int tiponegocio)
        {
            try
            {
                string HabilitarPizarra = crearContratoService.HabilitarPizarra(material, tiponegocio);
                string HabilitarCampana = crearContratoService.HabilitarCampaña(material);
                string TraerPrecioMoa = crearContratoService.TraerPrecioMoa(material, tiponegocio);
                var result = new { HabilitarPizarra, HabilitarCampana, TraerPrecioMoa };
                return JsonCustom(result);
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
        public ActionResult HabilitarCampana(int material)
        {
            try
            {
                return JsonCustom(crearContratoService.HabilitarCampaña(material));
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
        public ActionResult HabilitarPizarra(int material, int tiponegocio)
        {
            try
            {
                return JsonCustom(crearContratoService.HabilitarPizarra(material, tiponegocio));
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
        public ActionResult ObtenerFijacionesAutomaticas(bool esCorredorEnDataAgro, string cuitProveedor, int materialId, string filtro)
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;
                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);

                if (esCorredorEnDataAgro)
                {
                    return JsonCustom(crearContratoService.ObtenerFijacionesAutomaticas(cuitProveedor, proveedor.CUIT, materialId, filtro, 0));
                }
                else
                {
                    return JsonCustom(crearContratoService.ObtenerFijacionesAutomaticas(proveedor.CUIT, "", materialId, filtro, 0));

                }
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
        public ActionResult CrearContratoFijacion(string contrato)
        {
            try
            {
                contrato = contrato.Replace("nia", "ña");
                var contratoFijacion = JsonConvert.DeserializeObject<ContratoFijacion>(contrato);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;
                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);

                if (contratoFijacion.CorredorId == null)
                {
                    contratoFijacion.ProveedorId = (int)proveedor.IdDataAgro;
                }
                contratoFijacion.ProveedorCreadorId = (int)proveedor.IdDataAgro;
                contratoFijacion.ComercialCreadorId = null;
                contratoFijacion.MonedaSustentable = "USDM ";
                contratoFijacion.CantidadCamiones = null;


                string result = crearContratoService.CrearContratoFijacion(contratoFijacion);

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
