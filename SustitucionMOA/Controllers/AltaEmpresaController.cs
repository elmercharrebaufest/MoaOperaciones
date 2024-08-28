using Microsoft.Ajax.Utilities;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class AltaEmpresaController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly IAltaEmpresaService altaEmpresaService;
        readonly IDataAgroService dataAgroService;

        public AltaEmpresaController(IAltaEmpresaService altaEmpresaService, IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult GetEmpresas(int IdTipoProveedor, string fechaInicio, string fechaFin)
        {
            try
            {
                List<int> idTiposProveedor = new List<int>();

                if (IdTipoProveedor > 0)
                {
                    idTiposProveedor.Add(IdTipoProveedor);
                }
                else
                {
                    if (SessionPersister.User.permisos.Contains("VER ALTAS GRANOS"))
                    {
                        idTiposProveedor.Add((int)TipoUsuarioEnum.Ambos);
                        idTiposProveedor.Add((int)TipoUsuarioEnum.Granos);
                        idTiposProveedor.Add((int)TipoUsuarioEnum.Corredor);
                        idTiposProveedor.Add((int)TipoUsuarioEnum.Cliente);
                    }

                    if (SessionPersister.User.permisos.Contains("VER ALTAS NO GRANOS"))
                    {
                        idTiposProveedor.Add((int)TipoUsuarioEnum.NoGranos);
                    }
                }

                var empresas = altaEmpresaService.GetEmpresas(idTiposProveedor, fechaInicio, fechaFin);
                //MP: Comento esta parte, ya que esto ahora lo formateamos en el service. Ademas, esto generaba que se rompan algunos filtros
                //foreach (var item in empresas)
                //{
                //    item.EstadoAprobacionDescripcion = AddSpacesToSentence(item.EstadoAprobacionDescripcion);
                //    foreach (var item2 in item.HistorialAprobaciones)
                //    {
                //        item2.EstadoAprobacionDescripcion = AddSpacesToSentence(item2.EstadoAprobacionDescripcion);
                //    }
                //}
                return JsonCustom(new { data = empresas });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        [HttpPost]
        public ActionResult SetEstadoAprobacion(int empresaId, EstadoAprobacion estado, string observacion, string observacionParaElProveedor, string estadoSIPER, string razonSocial, string codigoCliente)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.SetEstadoAprobacion(empresaId, estado, observacion, ClaimsPrincipalExtension.GetClaimValue("emails"), observacionParaElProveedor, estadoSIPER, true, razonSocial, codigoCliente) });
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

        public ActionResult GetEstados()
        {
            try
            {
                List<KeyValuePair<string, string>> estadosIntermedios = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(EstadoAprobacion.AprobacionPendiente.ToFriendlyString(), EstadoAprobacion.AprobacionPendiente.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.DocumentacionPendiente.ToFriendlyString(), EstadoAprobacion.DocumentacionPendiente.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.AnalisisDeNosis.ToFriendlyString(), EstadoAprobacion.AnalisisDeNosis.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.DeshabilitadoEnDataAgro.ToFriendlyString(), EstadoAprobacion.DeshabilitadoEnDataAgro.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.EtapaFinal.ToFriendlyString(), EstadoAprobacion.EtapaFinal.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.EdicionRequerida.ToFriendlyString(), EstadoAprobacion.EdicionRequerida.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.PendienteAprobacionCompras.ToFriendlyString(), EstadoAprobacion.PendienteAprobacionCompras.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.RechazadoPorCompras.ToFriendlyString(), EstadoAprobacion.RechazadoPorCompras.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.AltaIncompleta.ToFriendlyString(), EstadoAprobacion.AltaIncompleta.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.SinAlta.ToFriendlyString(), EstadoAprobacion.SinAlta.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.AnalisisInterno.ToFriendlyString(), EstadoAprobacion.AnalisisInterno.ToFriendlyString())
                };
                List<KeyValuePair<string, string>> estadosFinales = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(EstadoAprobacion.Aprobado.ToFriendlyString(), EstadoAprobacion.Aprobado.ToFriendlyString()),
                    new KeyValuePair<string, string>(EstadoAprobacion.Rechazado.ToFriendlyString(), EstadoAprobacion.Rechazado.ToFriendlyString())
                };

                return JsonCustom(new { intermedios = estadosIntermedios, finales = estadosFinales });
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
       
        [System.Web.Http.HttpGet]
        public ActionResult GuardarSIPER(int proveedorId, string estadoSIPER)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.GuardarSIPER(proveedorId, estadoSIPER) });
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetEstadoAprobacion()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                var data = altaEmpresaService.GetEstadoAprobacion(userMail);

                if (data.Estado == EstadoAprobacion.AnalisisDeNosis
                    || data.Estado == EstadoAprobacion.EtapaFinal
                    || data.Estado == EstadoAprobacion.AprobacionPendiente
                    || data.Estado == EstadoAprobacion.PendienteAprobacionCompras
                    )
                {
                    data.EstadoDescripcion = "Alta en Gestión";
                }
                if (data.Estado == EstadoAprobacion.EdicionRequerida)
                {
                    data.EstadoDescripcion = "Solicitud de información";
                }
                if (!data.DocumentacionFisica)
                {
                    data.EstadoDescripcion = data.EstadoDescripcion + " - pendiente de envío documentación original";
                }

                return JsonCustom(new { data });
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

        public ActionResult VerificarEstadoDataAgro(int proveedorID)
        {
            try
            {
                return JsonCustom(new { data = dataAgroService.VerificarEstadoProveedor(proveedorID, ClaimsPrincipalExtension.GetClaimValue("emails")) });
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

        public ActionResult SolicitarInformacion(int empresaId)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.SolicitarInformacion(empresaId, ClaimsPrincipalExtension.GetClaimValue("emails")) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        [HttpPost]
        public JsonResult AgregarObservacion(int empresaId, string observacion)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.AgregarObservacion(empresaId, observacion, ClaimsPrincipalExtension.GetClaimValue("emails")) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        [HttpGet]
        public JsonResult VerificarExistenciaEmpresa(string cuit)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.VerificarExistenciaEmpresa(cuit) }) ;
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

        [HttpGet]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.MODIFICAR_ESTADO_PROVEEDOR)]
        public JsonResult ModificarEstadoProveedor(int proveedorId, string nuevoEstado) {
            try
            {
                var emailUsuario = SessionPersister.getUsername();
                var nuevoEstadoInt = altaEmpresaService.ModificarEstadoProveedor(proveedorId, nuevoEstado, emailUsuario);
                return JsonCustom(new { data = new { nuevoEstado = nuevoEstadoInt } });
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

    }
}
