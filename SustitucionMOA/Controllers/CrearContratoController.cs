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

        public ActionResult ObteneDatosContrato()
        {
            try
            {
                return JsonCustom(crearContratoService.ObteneDatosContrato());
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

        public ActionResult ObtenerDatosCompraNet()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedor();
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

                var proveedor = usuario.ObtenerProveedor();

                contratoAPrecio.ProveedorId = (int)proveedor.IdDataAgro;
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
                var contratoAPrecio = JsonConvert.DeserializeObject<ContratoAFijar>(contrato);


                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedor();

                contratoAPrecio.ProveedorId = (int)proveedor.IdDataAgro;
                contratoAPrecio.ProveedorCreadorId = (int)proveedor.IdDataAgro;
                contratoAPrecio.ComercialCreadorId = null;
                contratoAPrecio.MonedaSustentable = "USDM ";
                contratoAPrecio.ContratoSAP = "";
                contratoAPrecio.CantidadCamiones = null;


                string result = crearContratoService.CrearContratoAFijar(contratoAPrecio);

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
        public ActionResult GetContratos(string fechaDesde, string fechaHasta, string entregaDesde, string entregaHasta, string fijacionHasta, int? corredorId,
            int? proveedorId, int? boletoId, int? clasificacionId, int? destinoId, int? estadoId, int? materialId, int? campaniaId, int? tipoNegocioId,
            bool? pagoDiferidoTercero, bool? calidadTercero, bool? dolarizadoTercero)
        {
            try
            {

                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                var usuario = repositorio.Obtener<UsuarioGranos>(u => u.Mail == userMail);

                var proveedor = usuario.ObtenerProveedor();

                //contratoAPrecio.ProveedorId = (int)proveedor.IdDataAgro;
                //contratoAPrecio.ProveedorCreadorId = (int)proveedor.IdDataAgro;
                //
                DataSourceRequest request = new DataSourceRequest();
                request.Filter = new Kendo.DynamicLinq.Filter();
                request.Filter.Logic = "and";
                var filtros = new List<Kendo.DynamicLinq.Filter>();
                if (!string.IsNullOrWhiteSpace(fechaDesde))
                {
                    var fecha = DateTime.ParseExact(fechaDesde, "dd/MM/yyyy", null);
                    filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "gte" });
                }
                if (!string.IsNullOrWhiteSpace(fechaHasta))
                {
                    var fecha = DateTime.ParseExact(fechaHasta, "dd/MM/yyyy", null);
                    filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Fecha", Value = fecha, Operator = "lte" });
                }
                if (!string.IsNullOrWhiteSpace(entregaDesde))
                {
                    var fecha = DateTime.ParseExact(entregaDesde, "dd/MM/yyyy", null);
                    filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaDesde", Value = fecha, Operator = "gte" });
                }
                if (!string.IsNullOrWhiteSpace(entregaHasta))
                {
                    var fecha = DateTime.ParseExact(entregaHasta, "dd/MM/yyyy", null);
                    filtros.Add(new Kendo.DynamicLinq.Filter { Field = "FechaHasta", Value = fecha, Operator = "lte" });
                }
                if (!string.IsNullOrWhiteSpace(fijacionHasta))
                {
                    var fecha = DateTime.ParseExact(fijacionHasta, "dd/MM/yyyy", null);
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
                if (estadoId.HasValue && estadoId > 0)
                {
                    filtros.Add(new Kendo.DynamicLinq.Filter { Field = "Estado", Value = estadoId, Operator = "eq" });
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
