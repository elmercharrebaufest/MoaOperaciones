using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class UsuarioController : BaseController
    {
        //LoginService _loginService = new LoginService();

        private readonly IUsuarioService _usuarioService;

        public UsuarioController (IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetUsuarios()
        {
            try
            {
                return JsonCustom(new { data = new { usuarios = _usuarioService.GetUsuarios() } });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDORES)]
        public ActionResult GetVendedores()
        {
            try
            {
                return JsonCustom(new { data = new { usuarios = _usuarioService.GetVendedoresUsuario(SessionPersister.User.username) } });
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetRoles()
        {
            try
            {
                return JsonCustom(new { data = new { roles = _usuarioService.GetRoles() } });
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


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GuardarRoles(string idRoles, int idUsuario)
        {
            try
            {
                List<int> rolesList = idRoles.Split(',').Select(int.Parse).ToList();
                return JsonCustom(new { data = _usuarioService.GuardarRoles(rolesList, idUsuario) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult Deshabilitar(string mailUsuario)
        {
            try
            {
                return JsonCustom(new { data = _usuarioService.DeshabilitarUsuario(mailUsuario) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult Habilitar(string mailUsuario)
        {
            try
            {
                return JsonCustom(new { data = _usuarioService.HabilitarUsuario(mailUsuario) });
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

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.SELECCIONAR_VENDEDOR)]
        public ActionResult seleccionarVendedor(string vendedor, string descripcion)
        {
            try
            {
                if (vendedor == null)
                    return Json(new { error = String.Format(ErrorMsg.ErrorValorNuloVacio, "Vendedor") }, JsonRequestBehavior.AllowGet);

                //TODO: Ver como vamos a pisar esto
                //SessionPersister.Proveedor = vendedor;
                //if (SessionPersister.User != null)
                //{
                //    SessionPersister.User.nombre = descripcion;
                //}
                return JsonCustom(new { vendedor = vendedor, descripcion = descripcion });

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

        public ActionResult getDocumento(string nombre)
        {
            try
            {
                if (nombre == null || nombre == "")
                    return Json(new { error = String.Format(ErrorMsg.ErrorValorNuloVacio, "Documento") }, JsonRequestBehavior.AllowGet);
                return JsonCustom(new { documento = _usuarioService.getDocumento(nombre) });
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


        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        //public ActionResult getPerfiles()
        //{
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.getPerfiles() });
        //    }
        //    catch (InfoCustomException e)
        //    {
        //        return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.CAMBIAR_CONTRASENIA)]     
        //public ActionResult cambiarContrasenia(string contraseniaActual, string contraseniaNueva) {
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.cambiarContrasenia(SessionPersister.getUsername(), contraseniaActual, contraseniaNueva) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        //public ActionResult alta()
        //{
        //    try
        //    {
        //        Stream req = Request.InputStream;
        //        req.Seek(0, System.IO.SeekOrigin.Begin);
        //        string json = new StreamReader(req).ReadToEnd();
        //        UsuarioAlta usuario = JsonConvert.DeserializeObject<UsuarioAlta>(json);

        //        return JsonCustom(new { data = _usuarioService.alta(usuario) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult registrar(string numeroProveedor, string claveActivacion, string username, string contrasenia)
        //{
        //    try
        //    {
        //        _usuarioService.registrar(numeroProveedor, claveActivacion, username, contrasenia);
        //        return JsonCustom(new { data = _loginService.login(username, contrasenia) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult recuperarContrasenia(string username, string contrasenia, string contraseniaNew)
        //{
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.recuperarContrasenia(username) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        //public ActionResult desbloquear(string usuario)
        //{
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.desbloquear(usuario) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        //public ActionResult deshabilitar(string usuario)
        //{
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.deshabilitar(usuario) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}



        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        //public ActionResult habilitar(string usuario)
        //{
        //    try
        //    {
        //        return JsonCustom(new { data = _usuarioService.habilitar(usuario) });
        //    }
        //    catch (ValidationCustomException e)
        //    {
        //        return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}


    }
}