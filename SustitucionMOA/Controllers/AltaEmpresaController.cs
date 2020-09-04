using Microsoft.Ajax.Utilities;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
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
        public ActionResult getEmpresas()
        {
            try
            {
                var empresas = altaEmpresaService.GetEmpresas();
                foreach (var item in empresas)
                {

                    if (item.EstadoAprobacion != EstadoAprobacion.AunNoImplementado &&
                        item.EstadoAprobacion != EstadoAprobacion.DeshabilitadoEnDataAgro)
                    {
                        ResultadoValidarProveedorComercial result = dataAgroService.ObtenerValidarCUITProveedorGranos(item.CUIT);
                        if (result != null)
                        {
                            item.SISAEstadoCuit = result.ProveedorSISAEstadoCuit;
                        }
                    }
                    else
                    {
                        item.SISAEstadoCuit = "";
                    }

                    item.EstadoAprobacionDescripcion = AddSpacesToSentence(item.EstadoAprobacionDescripcion);
                    foreach (var item2 in item.HistorialAprobaciones)
                    {
                        item2.EstadoAprobacionDescripcion = AddSpacesToSentence(item2.EstadoAprobacionDescripcion);
                    }
                }
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult setEstadoAprobacion(int empresaId, EstadoAprobacion estado, string observacion, string observacionParaElProveedor, string estadoSIPER)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.SetEstadoAprobacion(empresaId, estado, observacion, ClaimsPrincipalExtension.GetClaimValue("emails"), observacionParaElProveedor, estadoSIPER, true) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult HabilitarUsuario(int empresaId, string observacion)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.HabilitarUsuario(ClaimsPrincipalExtension.GetClaimValue("emails"), empresaId, observacion) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult DeshabilitarUsuario(int empresaId, string observacion, string observacionParaElProveedor)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.DeshabilitarUsuario(ClaimsPrincipalExtension.GetClaimValue("emails"), empresaId, observacion, observacionParaElProveedor) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult getEstados()
        {
            try
            {
                List<KeyValuePair<int, string>> estados = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>((int)EstadoAprobacion.AprobacionPendiente, EstadoAprobacion.AprobacionPendiente.ToFriendlyString()),
                    new KeyValuePair<int, string>((int)EstadoAprobacion.AnalisisDeNosis, EstadoAprobacion.AnalisisDeNosis.ToFriendlyString()),
                    new KeyValuePair<int, string>((int)EstadoAprobacion.SentenciaFinal, EstadoAprobacion.SentenciaFinal.ToFriendlyString()),
                    new KeyValuePair<int, string>((int)EstadoAprobacion.EdicionRequerida, EstadoAprobacion.EdicionRequerida.ToFriendlyString()),
                    new KeyValuePair<int, string>((int)EstadoAprobacion.DeshabilitadoEnDataAgro, EstadoAprobacion.DeshabilitadoEnDataAgro.ToFriendlyString()),
                    new KeyValuePair<int, string>((int)EstadoAprobacion.AunNoImplementado, EstadoAprobacion.AunNoImplementado.ToFriendlyString())
                };
                return JsonCustom(new { data = estados });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }


        public ActionResult GetEstadoAprobacion()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                return JsonCustom(new { data = altaEmpresaService.GetEstadoAprobacion(userMail) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
