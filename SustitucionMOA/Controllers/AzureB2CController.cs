using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;
using Model = SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class AzureB2CController : Controller
    {
        DataAgroService _dataAgroService = new DataAgroService();

        protected readonly IRepositorio repositorio;
        LoginService _loginService = new LoginService();

        public AzureB2CController(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public ActionResult Login()
        {

            ValidarLogin();

            return Redirect("/");

        }

        private void ValidarLogin()
        {
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                string CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");

                CUIT = CUIT.Replace("-", string.Empty);

                string GranosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");

                Entidades.Usuario usuario = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                TestPartido();
                TestProv();
                TestLocalidad();

                switch (GranosFlag.ToLower())
                {
                    case "granos":
                        UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNuevo();

                            usuarioGranos.Roles = new List<Rol>();
                            usuarioGranos.Proveedores = new List<Proveedor>();

                            usuarioGranos.Roles.Add(usuarioNuevo);
                            usuarioGranos.TipoUsuario = ObtenerTipoPorNombreCorto("G");
                            RegistrarUsuarioGranos(usuarioGranos);
                        }
                        else
                        {
                            usuarioGranos = BuscarUsuarioGranos(usuarioGranos);
                        }

                        usuario = usuarioGranos;
                        break;

                    case "no granos":
                        Entidades.UsuarioNoGranos usuarioNoGranos = new Entidades.UsuarioNoGranos { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioNoGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioNoGranos.Roles = new List<Rol>();
                            usuarioNoGranos.Proveedores = new List<Proveedor>();

                            usuarioNoGranos.Roles.Add(usuarioNuevo);
                            usuarioNoGranos.TipoUsuario = ObtenerTipoPorNombreCorto("NG");

                            RegistrarUsuarioGenerico(usuarioNoGranos);
                            usuario = usuarioNoGranos;

                        }
                        else
                        {
                            usuario = BuscarUsuarioPorMail(mail);
                        }
                        break;

                    case "Ambos":
                        Entidades.Usuario usuarioAmbos = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioAmbos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioAmbos.Roles = new List<Rol>();
                            usuarioAmbos.Proveedores = new List<Proveedor>();

                            usuarioAmbos.Roles.Add(usuarioNuevo);
                            usuarioAmbos.TipoUsuario = ObtenerTipoPorNombreCorto("A");

                            RegistrarUsuarioGenerico(usuarioAmbos);
                        }
                        else
                        {
                            usuario = BuscarUsuarioPorMail(mail);
                        }

                        usuario = usuarioAmbos;
                        break;

                    case "Corredor":
                        Entidades.Usuario usuarioCorredor = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioCorredor))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCorredor.Roles = new List<Rol>();
                            usuarioCorredor.Proveedores = new List<Proveedor>();

                            usuarioCorredor.Roles.Add(usuarioNuevo);
                            usuarioCorredor.TipoUsuario = ObtenerTipoPorNombreCorto("C");

                            RegistrarUsuarioGenerico(usuarioCorredor);
                        }
                        else
                        {
                            usuario = BuscarUsuarioPorMail(mail);
                        }

                        usuario = usuarioCorredor;
                        break;

                    case "Cliente":
                        Entidades.Usuario usuarioCliente = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioCliente))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCliente.Roles.Add(usuarioNuevo);
                            usuarioCliente.TipoUsuario = ObtenerTipoPorNombreCorto("CLI");

                            RegistrarUsuarioGenerico(usuarioCliente);
                        }
                        else
                        {
                            usuario = BuscarUsuarioPorMail(mail);
                        }

                        usuario = usuarioCliente;
                        break;
                }

                SessionPersister.User = new Model.Usuario()
                {
                    username = mail,
                    nombre = usuario.ObtenerRazonSocial(),
                    permisos = usuario.ObtenerPermisos()
                };

                SessionPersister.Proveedor = usuario.ObtenerCodigoProveedor();
                SessionPersister.GranosFlag = usuario.TipoUsuario.NombreCorto;
                SessionPersister.Sociedad = "MOA";

                NoticiasDetallesWSMOAResponse noticias = new NoticiasDetallesWSMOAResponse() { };
                try
                {
                    if (usuario.EstaHabilitado())
                    {
                        noticias = _loginService.getNoticias(usuario.ObtenerCodigoProveedor());
                        noticias.cantidad = 0;
                        if (noticias != null && noticias.noticias != null)
                        {
                            SessionPersister.Noticias = noticias.noticias;
                            noticias.cantidad += noticias.noticias.Count;
                        }
                        if (noticias != null && noticias.notificaciones != null)
                        {
                            SessionPersister.Notificaciones = noticias.notificaciones;
                            noticias.cantidad += noticias.notificaciones.Count;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }

        private UsuarioGranos BuscarUsuarioGranos(UsuarioGranos usuarioGranos)
        {
            return repositorio.Obtener<UsuarioGranos>(u => u.Mail == usuarioGranos.Mail);
        }

        private Entidades.Usuario BuscarUsuarioPorMail(string mail)
        {
            return repositorio.Obtener<Entidades.Usuario>(u => u.Mail == mail);
        }


        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto)
        {
            return repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);
        }

        public ActionResult ValidarLoginAzure()
        {


            //if (result.permisos.Count() == 1 && result.permisos[0] == "DATAAGROLOGIN")
            //{
            //    SessionPersister.clear();
            //    DataAgroAuthWSMOAResponse data = _dataAgroService.goToDataAgro(result.proveedor, result.nombre);
            //    return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            //}

            if (SessionPersister.User == null)
            {
                ValidarLogin();
            }

            return Json(new
            {
                success = SuccessMsg.LoginOk,
                username = SessionPersister.User.username,
                nombre = SessionPersister.User.nombre,
                proveedor = SessionPersister.Proveedor,
                granosFlag = SessionPersister.GranosFlag,
                tipoUsuario = "PROV",
                permisos = SessionPersister.User.permisos,
                noticias = SessionPersister.Notificaciones,
                esNuevoUsuario = true
            }, JsonRequestBehavior.AllowGet);
        }


        public bool RegistrarUsuarioGranos(UsuarioGranos usuario)
        {
            Proveedor proveedor = new Proveedor
            {
                CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT"),
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente
            };

            ValidarCUITProveedor(usuario, proveedor);

            usuario.Mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            usuario.Proveedores.Add(proveedor);

            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool RegistrarUsuarioGenerico(Entidades.Usuario usuario)
        {
            Proveedor proveedor = new Proveedor();
            proveedor.CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");
            proveedor.EstadoAprobacion = EstadoAprobacion.AunNoImplementado;

            usuario.Mail = ClaimsPrincipalExtension.GetClaimValue("emails");
            usuario.Proveedores.Add(proveedor);

            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool ExisteUsuario(Entidades.Usuario usuario)
        {
            return (repositorio.Existe<Entidades.Usuario>(u => u.Mail == usuario.Mail));
        }

        public bool ValidarCUITProveedor(UsuarioGranos usuario, Entidades.Proveedor proveedor)
        {
            return _dataAgroService.ValidarCUITProveedorGranos(usuario, proveedor);
        }

        public bool ExisteProveedor(Entidades.Proveedor proveedor)
        {
            return (repositorio.Existe<Entidades.Proveedor>(u => u.CUIT == u.CUIT));
        }

        public bool RegistrarProveedor(Entidades.Proveedor proveedor)
        {
            if (!ExisteProveedor(proveedor))
            {
                repositorio.Agregar(proveedor);
                return repositorio.GuardarCambios() == 1;
            }

            return true;
        }

        public Provincia TestProv()
        {
            return repositorio.Obtener<Provincia>(p => p.ProvinciaId == 1);
        }

        public Localidad TestLocalidad()
        {
            return repositorio.Obtener<Localidad>(p => p.ProvinciaId == 1);
        }

        public Partido TestPartido()
        {
            return repositorio.Obtener<Partido>(p => p.Id == 1);
        }

        public Rol ObtenerRolUsuarioNuevo()
        {
            return repositorio.Obtener<Rol>(u => u.Nombre.Equals("Nuevo Usuario"));
        }

        public Rol ObtenerRolUsuarioNoImplementado()
        {
            return repositorio.Obtener<Rol>(u => u.Nombre.Equals("Usuario No Implementado"));
        }

        public async Task SignOut()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                SessionPersister.clear();
                await MsalAppBuilder.ClearUserTokenCache();
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
            }
        }

        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        public void EditProfile()
        {
            if (Request.IsAuthenticated)
            {
                // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
                HttpContext.GetOwinContext().Set("Policy", Globals.EditProfilePolicyId);

                // Set the page to redirect to after editing the profile
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

                return;
            }

            Response.Redirect("/");
        }
    }
}
