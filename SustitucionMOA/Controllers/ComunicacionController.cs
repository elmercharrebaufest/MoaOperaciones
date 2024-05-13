using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class ComunicacionController : BaseController
    {
        readonly IComunicacionService comunicacionService;
        //private readonly ILiquidacionService liquidacionService;
        private readonly IUsuarioService usuarioService;

        public ComunicacionController(IComunicacionService comunicacionService, IUsuarioService usuarioService)
        {
            this.comunicacionService = comunicacionService;
            this.usuarioService = usuarioService;
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        public ActionResult GetAllByProveedor(string vendedor, string fechaInicio, string fechaFin)
        {
            try
            {
                if (vendedor == "" || vendedor == null)
                {
                    vendedor = SessionPersister.Proveedor;
                }

                UsuarioDto usuarioActual = ObtenerUsuarioActual();
                bool obtenerTodos = usuarioActual.Permisos.Contains(Permiso.CONSULTA_AMB);

                comunicacionService.ActualizarComunicacionesPorProveedor(vendedor, SessionPersister.Proveedor, fechaInicio, fechaFin, usuarioActual.Id, obtenerTodos);

                return JsonCustom(new { data = comunicacionService.ObtenerComunicacionesPorProveedor(vendedor, SessionPersister.Proveedor, fechaInicio, fechaFin, usuarioActual.Id, obtenerTodos) });
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


        //[ValidateInput(false)]
        public ActionResult PostComunicacionLeida(ComunicacionListaIdDto comunicacionIds)
        {
            try
            {
                return JsonCustom(new { data = comunicacionService.GrabarComunicacionComoLeida(comunicacionIds) });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PostComunicacionNoLeida(ComunicacionListaIdDto comunicacionIds)
        {
            try
            {
                return JsonCustom(new { data = comunicacionService.GrabarComunicacionComoNoLeida(comunicacionIds) });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_DATOS_FISCALES)]
        public ActionResult GetCM05(string vendedor)
        {
            try
            {
                if (vendedor == "" || vendedor == null)
                {
                    vendedor = SessionPersister.Proveedor;
                }
                return JsonCustom(new { data = comunicacionService.ProcesarCM05(vendedor, SessionPersister.Proveedor) });
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_DATOS_FISCALES)]
        public ActionResult GetCuentasHabilitadas(string vendedor)
        {
            try
            {
                if (vendedor == "" || vendedor == null)
                {
                    vendedor = SessionPersister.Proveedor;
                }
                return JsonCustom(new { data = comunicacionService.ProcesarCuentasHabilitadas(vendedor, SessionPersister.Proveedor) });
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_LIQUIDACIONES)]
        public ActionResult GetLiquidacionesObservadas(string vendedor, string fechaInicio, string fechaFin)
        {
            try
            {
                return JsonCustom(comunicacionService.ProcesarLiquidacionesObservadas(SessionPersister.Proveedor, fechaInicio, fechaFin));
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