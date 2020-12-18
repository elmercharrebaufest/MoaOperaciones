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
using Kendo.DynamicLinq;
using System.Web.Script.Serialization;
using SustitucionMOAUtils.Export;

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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                contratoAPrecio.ComercialId = (int)proveedor.IdComercialDataAgro;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                contratoAFijar.ComercialId = (int)proveedor.IdComercialDataAgro;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
                contratoFijacion.ComercialId = (int)proveedor.IdComercialDataAgro;
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId,
            int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId,
            bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, bool? sustentableTercero)
        {
            try
            {

                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;

                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);


                string result = obteberContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, proveedor, sustentableTercero);

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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ExportContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId,
           int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId,
           bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, bool? sustentableTercero)
        {
            try
            {

                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);
                string codigoProveedor = SessionPersister.Proveedor;

                var proveedor = usuario.ObtenerProveedorPorCodigo(codigoProveedor);

                string result = obteberContratos(fechaDesde, fechaHasta, entregaDesde, entregaHasta, fijacionHasta, corredorId, proveedorId, boletoId, clasificacionId, destinoId, estadoId, materialId, campaniaId, tipoNegocioId, pagoDiferidoTercero, calidadTercero, dolarizadoTercero, proveedor, sustentableTercero);
                System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result2 = (Dictionary<string, object>)ser.DeserializeObject(result);
                var list = ser.Deserialize<List<BasicoContrato>>(ser.Serialize(result2["Data"]));
                foreach (var item in list)
                {
                    if (item.FechaDesde.HasValue)
                        item.FechaDesde = item.FechaDesde.Value.AddHours(-3);
                    if (item.FechaHasta.HasValue)
                        item.FechaHasta = item.FechaHasta.Value.AddHours(-3);
                }
                var excel = ExcelExport.ToExcel(list, new string[] { "Cuit", "Proveedor", "Corredor", "ContratoCorredor", "TipoNegocio", "Cantidad", "Precio", "Moneda",
                    "Destino", "FechaDesde", "FechaHasta", "Material", "Campaña", "Clasificacion", "Localidad", "Consignatario", "Estado", "Pago Diferido", "Dolarizado", "Calidad", "Sustentable" }, "Reporte Contratos");

                return JsonCustom(excel);
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        private string obteberContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId, int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, string estadoId, int? materialId, int? campaniaId, int? tipoNegocioId, bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero, Proveedor proveedor, bool? sustentableTercero)
        {
            DataSourceRequest request = new DataSourceRequest();
            request.Filter = new Kendo.DynamicLinq.Filter();
            request.Filter.Logic = "and";
            var filtros = new List<Kendo.DynamicLinq.Filter>();
            if (!string.IsNullOrWhiteSpace(fechaDesde))
            {
                var fecha = DateTime.ParseExact(fechaDesde, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "gte" });
            }
            if (!string.IsNullOrWhiteSpace(fechaHasta))
            {
                var fecha = DateTime.ParseExact(fechaHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "lte" });
            }
            if (!string.IsNullOrWhiteSpace(entregaDesde))
            {
                var fecha = DateTime.ParseExact(entregaDesde, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaDesde", Value = fecha, Operator = "gte" });
            }
            if (!string.IsNullOrWhiteSpace(entregaHasta))
            {
                var fecha = DateTime.ParseExact(entregaHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaHasta", Value = fecha, Operator = "lte" });
            }
            if (!string.IsNullOrWhiteSpace(fijacionHasta))
            {
                var fecha = DateTime.ParseExact(fijacionHasta, "dd/MM/yyyy", null).ToUniversalTime();
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "HastaFijacion", Value = fecha, Operator = "lte" });
            }
            if (corredorId.HasValue && corredorId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CorredorId", Value = corredorId, Operator = "eq" });
            }
            if (proveedorId.HasValue && proveedorId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ProveedorId", Value = proveedorId, Operator = "eq" });
            }
            if (boletoId.HasValue && boletoId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "BoletoId", Value = boletoId, Operator = "eq" });
            }
            if (clasificacionId.HasValue && clasificacionId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ClasificacionId", Value = clasificacionId, Operator = "eq" });
            }
            if (destinoId.HasValue && destinoId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "DestinoId", Value = destinoId, Operator = "eq" });
            }
            if (!string.IsNullOrWhiteSpace(estadoId))
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Estado_Contrato", Value = estadoId, Operator = "eq" });
            }
            if (materialId.HasValue && materialId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "MaterialId", Value = materialId, Operator = "eq" });
            }
            if (campaniaId.HasValue && campaniaId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CampanaId", Value = campaniaId, Operator = "eq" });
            }
            if (tipoNegocioId.HasValue && tipoNegocioId > 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "TipoNegocio", Value = tipoNegocioId, Operator = "eq" });
            }
            if (pagoDiferidoTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "PagoDiferidoTercero", Value = pagoDiferidoTercero, Operator = "eq" });
            }
            if (dolarizadoTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "DolarizadoTercero", Value = dolarizadoTercero, Operator = "eq" });
            }
            if (sustentableTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "SustentableTercero", Value = sustentableTercero, Operator = "eq" });
            }
            if (calidadTercero.HasValue)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "CalidadTercero", Value = calidadTercero, Operator = "eq" });
            }
            if (corredorId == null || corredorId == 0)
            {
                filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ProveedorId", Value = (int)proveedor.IdDataAgro, Operator = "eq" });
            }
            filtros.Add(new Kendo.DynamicLinq.Filter { Field = "ComercialCreadorId", Value = (int)proveedor.IdDataAgro, Operator = "eq" });//es el ProveedorCreadorId en el BasicoContrato
            request.Filter.Filters = filtros;
            string result = crearContratoService.GetContratos(request);
            return result;
        }

        public ActionResult ValidarProveedor(string proveedorId)
        {
            try
            {
                return JsonCustom(crearContratoService.ValidarProveedor(proveedorId));
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

        public ActionResult TraerPrecioMoaMateriales(int tipoNegocioId = 0)
        {
            try
            {
                return JsonCustom(crearContratoService.TraerPrecioMoaMateriales(tipoNegocioId));
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
    }
}
