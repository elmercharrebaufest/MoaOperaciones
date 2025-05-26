using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class UsuarioController : BaseController
    {
        //LoginService _loginService = new LoginService();

        private readonly IUsuarioService _usuarioService;
        private readonly IRepositorio repositorio;
        private readonly IAltaEmpresaNoGranosService altaEmpresaNoGranosService;
        LoginService _loginService = new LoginService();

        public UsuarioController(IUsuarioService usuarioService, IRepositorio repositorio, IAltaEmpresaNoGranosService altaEmpresaNoGranosService)
        {
            this._usuarioService = usuarioService;
            this.repositorio = repositorio;
            this.altaEmpresaNoGranosService = altaEmpresaNoGranosService;
        }

        [CustomPermisoAuthorize(Roles = Permiso.ABM_USUARIOS + "," + Permiso.EDITAR_SUPLENTE)]
        public ActionResult GetUsuarios()
        {
            return JsonCustom(new { data = new { usuarios = _usuarioService.GetUsuarios() } });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_VENDEDORES)]
        public ActionResult GetVendedores()
        {
            return JsonCustom(new { data = new { usuarios = _usuarioService.GetVendedoresUsuario(SessionPersister.User.username) } });
        }


        [CustomPermisoAuthorize(Roles = Permiso.ABM_USUARIOS + "," + Permiso.EDITAR_SUPLENTE)]
        public ActionResult GetRoles()
        {
            return JsonCustom(new { data = new { roles = _usuarioService.GetRoles() } });
        }


        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GuardarRoles(string idRoles, int idUsuario, string usuarioSap)
        {
            List<int> rolesList = idRoles.Split(',').Select(int.Parse).ToList();
            return JsonCustom(new { data = _usuarioService.GuardarRoles(rolesList, idUsuario, usuarioSap) });
        }

        [CustomPermisoAuthorize(Roles = Permiso.EDITAR_SUPLENTE)]
        public ActionResult GuardarSuplente(int idUsuario, string suplente, string fDesde, string fHasta, bool esExterno)
        {
            _usuarioService.GuardarSuplente(idUsuario, suplente, fDesde, fHasta, esExterno);
            return JsonCustom(true);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult ObtenerRolesUsuario(int idUsuario)
        {
            return JsonCustom(new { data = _usuarioService.GetRolesUsuario(idUsuario) });
        }


        [CustomPermisoAuthorize(Roles = Permiso.ABM_USUARIOS + "," + Permiso.EDITAR_SUPLENTE)]
        public ActionResult ObtenerReasignacionUsuario(int idUsuario)
        {
            return JsonCustom(new { data = _usuarioService.GetPeriodoReasignacion(idUsuario) });
        }

        public ActionResult ObtenerRolesUsuarioByEmail(string email)
        {
            var toRet = _usuarioService.GetRolesUsuario(email);
            return JsonCustom(new { data = toRet });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult Deshabilitar(string mailUsuario)
        {
            return JsonCustom(new { data = _usuarioService.DeshabilitarUsuario(mailUsuario) });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult Habilitar(string mailUsuario)
        {
            return JsonCustom(new { data = _usuarioService.HabilitarUsuario(mailUsuario) });
        }

        [System.Web.Http.HttpGet]
        public ActionResult SeccionVisitada(string seccion)
        {
            _usuarioService.SeccionVisitada(SessionPersister.Mail, seccion);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.SELECCIONAR_VENDEDOR)]
        public ActionResult seleccionarVendedor(int? vendedorId)
        {
            if (vendedorId == null)
                return Json(new { error = String.Format(ErrorMsg.ErrorValorNuloVacio, "Vendedor") }, JsonRequestBehavior.AllowGet);

            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == userMail);

            var proveedorAAsignar = repositorio.Obtener<Proveedor>(vendedorId);

            if (proveedorAAsignar.EstadoAprobacion != 0)
            {
                throw new ValidationCustomException("Proveedor deshabilitado");
            }

            if (!usuario.EsAdmin() && !usuario.TienePermiso(PermisoEnum.ElegirTodosVendedores) && !usuario.TieneProveedor(proveedorAAsignar.CodigoProveedor))
            {
                throw new ValidationCustomException("Proveedor incorrecto");
            }


            // get context of the authentication manager
            var authenticationManager = HttpContext.GetOwinContext().Authentication;

            // create a new identity from the old one
            var identity = new ClaimsIdentity(User.Identity);

            // update claim value
            identity.RemoveClaim(identity.FindFirst(Globals.ClaimsProveedorType));
            identity.AddClaim(new Claim(Globals.ClaimsProveedorType, proveedorAAsignar.CodigoProveedor));

            identity.RemoveClaim(identity.FindFirst(Globals.ClaimsNombreType));
            identity.AddClaim(new Claim(Globals.ClaimsNombreType, proveedorAAsignar.RazonSocial));

            if (identity.FindFirst(Globals.ClaimsProveedorId) != null)
            {
                identity.RemoveClaim(identity.FindFirst(Globals.ClaimsProveedorId));
            }

            identity.AddClaim(new Claim(Globals.ClaimsProveedorId, proveedorAAsignar.Id.ToString()));

            if (identity.FindFirst(Globals.ClaimsEsCodigoCorredorType) != null)
            {
                identity.RemoveClaim(identity.FindFirst(Globals.ClaimsEsCodigoCorredorType));
            }
            identity.AddClaim(new Claim(Globals.ClaimsEsCodigoCorredorType, proveedorAAsignar.TipoProveedor.EsCorredor ? "true" : "false"));


            if (!usuario.EsCorredor() && !proveedorAAsignar.TipoProveedor.EsCorredor)
            {
                if (identity.FindFirst(Globals.ClaimsTipoUsuarioType) != null)
                {
                    identity.RemoveClaim(identity.FindFirst(Globals.ClaimsTipoUsuarioType));
                }
                var nuevoTipoUsuario = proveedorAAsignar.TipoProveedor.EsCliente ? "CLI" : "PROV";
                identity.AddClaim(new Claim(Globals.ClaimsTipoUsuarioType, nuevoTipoUsuario));
            }

            identity.RemoveClaim(identity.FindFirst(Globals.ClaimsCuit));
            identity.AddClaim(new Claim(Globals.ClaimsCuit, proveedorAAsignar.CUIT));

            // tell the authentication manager to use this new identity
            authenticationManager.AuthenticationResponseGrant =
                new Microsoft.Owin.Security.AuthenticationResponseGrant(
                    new ClaimsPrincipal(identity),
                    new Microsoft.Owin.Security.AuthenticationProperties { IsPersistent = true }
                );


            NoticiasDetallesWSMOAResponse noticias = new NoticiasDetallesWSMOAResponse() { };

            try
            {
                if (!Globals.EsLocal)
                {
                    noticias = _loginService.ObtenerNoticias(proveedorAAsignar.CodigoProveedor);
                    noticias.cantidad = 0;
                    if (noticias != null && noticias.noticias != null)
                    {
                        noticias.cantidad += noticias.noticias.Count;
                    }
                    if (noticias != null && noticias.notificaciones != null)
                    {
                        noticias.cantidad += noticias.notificaciones.Count;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
            }



            return JsonCustom(new SeleccionarVendedorResponseDto(proveedorAAsignar, noticias));
        }

        public ActionResult getDocumento(string nombre)
        {
            if (nombre == null || nombre == "")
                return Json(new { error = String.Format(ErrorMsg.ErrorValorNuloVacio, "Documento") }, JsonRequestBehavior.AllowGet);
            return JsonCustom(new { documento = _usuarioService.getDocumento(nombre) });
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
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
        //        Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
        //        return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ALTA_EMPRESA_NO_GRANOS)]
        public ActionResult GrabarNuevoProveedorNoGranos(string razonSocial, string cuit, string email, string telefono, bool realizarAnalisisNOSIS, int IdRubro,
            string condicionDePago, string servicioPrestado, string organizacionDeCompra, string razonDeEleccion, int facturacionAnual, string solicitanteInterno,
            int? idProveedor, string observacionesParaElProveedor, bool requiereVerificacionCompras, bool ingresoAPlanta, bool altaInterna, bool siperObligatorio,
            string observacionInterna)
        {

            if (string.IsNullOrWhiteSpace(cuit))
            {
                throw new ValidationCustomException("Debe completar CUIT.");
            }
            else
            {
                cuit = cuit.Replace("-", "");
                if (cuit.Length != 11)
                {
                    throw new ValidationCustomException("El CUIT no tiene el formato correcto.");
                }
                else
                {
                    long l = 0;
                    if (!long.TryParse(cuit, out l))
                    {
                        throw new ValidationCustomException("El CUIT no tiene el formato correcto.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(razonSocial))
            {
                throw new ValidationCustomException("Debe completar la Razon Social.");
            }


            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ValidationCustomException("Debe completar Email.");
            }
            else
            {
                if (!IsValidEmail(email))
                {
                    throw new ValidationCustomException("El Email no tiene un formato valido.");
                }
            }
            if (string.IsNullOrWhiteSpace(telefono))
            {
                throw new ValidationCustomException("Debe completar Telefono.");
            }
            if (IdRubro <= 0)
            {
                throw new ValidationCustomException("Debe completar el Rubro.");
            }
            if (facturacionAnual <= 0)
            {
                throw new ValidationCustomException("Debe completar la Facturacion Anual.");
            }
            if (string.IsNullOrWhiteSpace(servicioPrestado))
            {
                throw new ValidationCustomException("Debe completar Servicio prestado a Molinos.");
            }
            if (string.IsNullOrWhiteSpace(razonDeEleccion))
            {
                throw new ValidationCustomException("Debe completar Razón de elección del proveedor.");
            }

            return JsonCustom(new
            {
                data = altaEmpresaNoGranosService.GrabarNuevoProveedorNoGranos(razonSocial, cuit, email, telefono, realizarAnalisisNOSIS, IdRubro, condicionDePago,
                servicioPrestado, organizacionDeCompra, razonDeEleccion, facturacionAnual, solicitanteInterno, ClaimsPrincipalExtension.GetClaimValue("emails"),
                idProveedor, observacionesParaElProveedor, requiereVerificacionCompras, ingresoAPlanta, altaInterna, siperObligatorio, observacionInterna)
            });
        }

        bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public ActionResult GetRubros()
        {
            return JsonCustom(new { data = altaEmpresaNoGranosService.GetRubros() });
        }

        [HttpGet]
        public ActionResult GetProveedorPorCodigo(string codigo)
        {
            return JsonCustom(_usuarioService.GetProveedorPorCodigo(codigo, SessionPersister.Mail));
        }

        [HttpGet]
        public ActionResult VerificarYObtenerProveedor(string codigoProveedor)
        {
            return JsonCustom(_usuarioService.VerificarYObtenerProveedor(SessionPersister.Mail, SessionPersister.Proveedor, codigoProveedor));
        }

        public ActionResult RechazarProveedorNoGranos(int idProveedor, string observacionesParaElProveedor)
        {
            return JsonCustom(new
            {
                data = altaEmpresaNoGranosService.RechazarProveedorNoGranos(idProveedor, ClaimsPrincipalExtension.GetClaimValue("emails"), observacionesParaElProveedor)
            });
        }

        public ActionResult ObtenerNuevaApiKey()
        {
            return JsonCustom(new { data = _usuarioService.ObtenerNuevoApiKey(SessionPersister.Mail) });
        }

        [System.Web.Http.HttpPost]
        public ActionResult GrabarProveedor(string json)
        {
            var proveedor = JsonConvert.DeserializeObject<ProveedorDto>(json);
            var result = _usuarioService.GrabarProveedor(proveedorDto: proveedor,
                                                         estadoAprobacion: EstadoAprobacion.AltaIncompleta,
                                                         mantenerEstadoAprobacionExistente: true,
                                                         mailUsuarioAdmin: SessionPersister.Mail);
            return JsonCustom(new { data = result });
        }

        #region Metodos de modificacion de alta usuario


        [HttpGet]
        [CustomPermisoAuthorize(Roles = Permiso.ABM_USUARIOS + "," + Permiso.EDITAR_SUPLENTE)]
        public ActionResult GetTipoUsuario()
        {
            try
            {
                return JsonCustom(new { data = new { tipoUsuario = _usuarioService.GetTipoUsuario() } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetUsuarioPorId(int id)
        {
            try
            {
                return JsonCustom(new { data = new { usuario = _usuarioService.GetUsuarioPorId(id) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetProveedorAuditoriaPorUsuario(int id)
        {
            try
            {
                return JsonCustom(new { data = new { usuario = _usuarioService.GetProveedorAuditoriaPorUsuario(id) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetProvedoresEmail(int tipoProveedorId, string email, string cuitUsuario)
        {
            try
            {
                return JsonCustom(new { data = new { proveedores = _usuarioService.GetProvedoresEmail(tipoProveedorId, email, cuitUsuario) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult ValidarMailUsuario(UsuarioModificacionDto usuarioModificacionDto)
        {
            try
            {
                return JsonCustom(new { data = new { validaciones = _usuarioService.ValidarMailUsuario(usuarioModificacionDto) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult ModificarUsuario(UsuarioModificacionDto usuarioModificacionDto)
        {
            try
            {
                return JsonCustom(new { data = new { resultado = _usuarioService.ModificarUsuario(usuarioModificacionDto) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_USUARIOS)]
        public ActionResult GetProvedoresUsuario(int usuarioId)
        {
            try
            {
                return JsonCustom(new { data = new { proveedores = _usuarioService.GetProveedoresUsuario(usuarioId) } });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        public ActionResult EliminarCuitNoHabilitado(int proveedorId)
        {
            string mailUsuarioSesion = SessionPersister.Mail;
            return JsonCustom(new { data = _usuarioService.EliminarCuitNoHabilitado(proveedorId, mailUsuarioSesion) });
        }
        #region AsignarNuevoCUIT
        [System.Web.Http.HttpGet]
        public ActionResult GetProveedorAprobadoPorCuit(string cuit)
        {
            string mailUsuarioSesion = SessionPersister.Mail;
            return JsonCustom(new { data = _usuarioService.GetProveedorAprobadoPorCuit(cuit, mailUsuarioSesion) });
        }
        [System.Web.Http.HttpPost]
        public ActionResult AsignarNuevaCUIT(string datosAAsignar)
        {
            string mailUsuarioSesion = SessionPersister.Mail;
            var datos = JsonConvert.DeserializeObject<AsignarNuevaCuitDto>(datosAAsignar);
            _usuarioService.AsignarNuevaCUIT(datos, mailUsuarioSesion);
            return JsonCustom(new { data = true });
        }

        [System.Web.Http.HttpPost]
        public ActionResult DesasociarVendedor(int usuarioId, int proveedorId)
        {
            string mailUsuarioSesion = SessionPersister.Mail;
            _usuarioService.DesasociarVendedor(usuarioId, proveedorId, mailUsuarioSesion);
            return JsonCustom(new { data = true });
        }

        #endregion
        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ADMINISTRAR_CURSOS)]
        [HttpGet]
        public ContentResult GetMailUsuarios(string mail)
        {
            var response = new SustitucionMOAApiResponse<List<string>>();
            try
            {
                response.Data = _usuarioService.GetMailUsuarios(mail);
            }
            catch (InfoCustomException ice)
            {
                response.Info = ice.Message;
            }
            catch (ValidationCustomException vce)
            {
                response.Error = vce.Message;
            }
            catch (Exception ex)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.Mail, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                response.Error = ErrorMsg.Error;
            }
            return ContentCustom(response);
        }

        [System.Web.Http.HttpPost]
        public ActionResult GuardarConfiguracionUsuario([System.Web.Http.FromBody] string valor, [System.Web.Http.FromBody] int tipo)
        {
            var mailUsuario = SessionPersister.Mail;
            _usuarioService.GuardarConfiguracionUsuario(mailUsuario, valor, (TipoConfiguracionUsuario)tipo);
            return JsonCustom(true);
        }
        [HttpGet]
        public ActionResult ObtenerConfiguracionUsuario(TipoConfiguracionUsuario tipo)
        {
            var mailUsuario = SessionPersister.Mail;
            return JsonCustom(_usuarioService.ObtenerConfiguracion(mailUsuario, tipo));
        }
    }
}