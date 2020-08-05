using System;
using System.Web.Mvc;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using Model = SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using Entidades = SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System.Text;
using System.Linq;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOARepositorio;
using System.Collections.Generic;
using Microsoft.Owin.Security;
using SustitucionMOA.Utils;
using System.Web;
using System.Security.Claims;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Entities;

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
            try
            {
                string mail = ClaimsPrincipalExtension.GetClaimValue("emails");
                string CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");

                CUIT = CUIT.Replace("-", string.Empty);

                string GranosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");
                
                Entidades.Usuario usuario = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT } ;

                switch (GranosFlag)
                {
                    case "Granos":
                        UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNuevo();

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

                    case "No Granos":
                        Entidades.Usuario usuarioNoGranos = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioNoGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioNoGranos.Roles.Add(usuarioNuevo);
                            usuarioNoGranos.TipoUsuario = ObtenerTipoPorNombreCorto("G");

                            RegistrarUsuarioGenerico(usuarioNoGranos);
                        }
                        usuario = usuarioNoGranos;
                        break;

                    case "Ambos":
                        Entidades.Usuario usuarioAmbos = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioAmbos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioAmbos.Roles.Add(usuarioNuevo);
                            usuarioAmbos.TipoUsuario = ObtenerTipoPorNombreCorto("G");

                            RegistrarUsuarioGenerico(usuarioAmbos);
                        }
                        usuario = usuarioAmbos;
                        break;

                    case "Corredor":
                        Entidades.Usuario usuarioCorredor = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioCorredor))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCorredor.Roles.Add(usuarioNuevo);
                            usuarioCorredor.TipoUsuario = ObtenerTipoPorNombreCorto("G");

                            RegistrarUsuarioGenerico(usuarioCorredor);
                        }
                        usuario = usuarioCorredor;
                        break;

                    case "Cliente":
                        Entidades.Usuario usuarioCliente = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                        if (!ExisteUsuario(usuarioCliente))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCliente.Roles.Add(usuarioNuevo);
                            usuarioCliente.TipoUsuario = ObtenerTipoPorNombreCorto("G");

                            RegistrarUsuarioGenerico(usuarioCliente);
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

                //return Json(new { success = SuccessMsg.LoginOk, username = usuario.Mail, nombre = usuario.ObtenerRazonSocial(), proveedor = usuario.ObtenerCodigoProveedor(), granosFlag = usuario.TipoUsuario.NombreCorto, tipoUsuario = "PROV", permisos = usuario.ObtenerPermisos(), noticias = noticias, esNuevoUsuario = usuario.EsNuevoUsuario() }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

            }

            //return new FilePathResult(Server.MapPath("~/index.html"), "text/html");

            return Redirect("/");

        }
        private UsuarioGranos BuscarUsuarioGranos(UsuarioGranos usuarioGranos)
        {
            return repositorio.Obtener<UsuarioGranos>(u => u.Mail == usuarioGranos.Mail);
        }

        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto)
        {
            return repositorio.Obtener<TipoUsuario>(t => t.Nombre == nombreCorto);
        }

        public ActionResult ValidarLoginAzure()
        {


            //if (result.permisos.Count() == 1 && result.permisos[0] == "DATAAGROLOGIN")
            //{
            //    SessionPersister.clear();
            //    DataAgroAuthWSMOAResponse data = _dataAgroService.goToDataAgro(result.proveedor, result.nombre);
            //    return Json(new { success = SuccessMsg.LoginOk, tipoUsuario = "DATAAGROLOGIN", cuit = data.cuit, error = data.error, username = data.nombreUsuario, url = data.url, vencimiento = data.vencimiento }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new { success = SuccessMsg.LoginOk, 
                            username = SessionPersister.User.username, 
                            nombre = SessionPersister.User.nombre, 
                            proveedor = SessionPersister.Proveedor, 
                            granosFlag = SessionPersister.GranosFlag, 
                            tipoUsuario = "PROV", 
                            permisos = SessionPersister.User.permisos, 
                            noticias = SessionPersister.Notificaciones, 
                            esNuevoUsuario = true }, JsonRequestBehavior.AllowGet);
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

        public Rol ObtenerRolUsuarioNuevo() 
        {
            return repositorio.Obtener<Rol>(u => u.Nombre.Equals("Nuevo Usuario"));
        }

        public Rol ObtenerRolUsuarioNoImplementado()
        {
            return repositorio.Obtener<Rol>(u => u.Nombre.Equals("Usuario No Implementado"));
        }
    }
}
