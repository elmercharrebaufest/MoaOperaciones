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
                
                Entidades.Usuario usuario = new Entidades.Usuario(mail, CUIT);

                switch (GranosFlag)
                {
                    case "Granos":
                        UsuarioGranos usuarioGranos = new UsuarioGranos(mail, CUIT);

                        if (!ExisteUsuario(usuarioGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNuevo();

                            usuarioGranos.Roles.Add(usuarioNuevo);
                            usuarioGranos.TipoUsuario = TipoUsuario.GetTipoGranos();
                            RegistrarUsuarioGranos(usuarioGranos);
                        }
                        usuario = usuarioGranos;
                        break;

                    case "No Granos":
                        Entidades.Usuario usuarioNoGranos = new Entidades.Usuario(mail, CUIT);

                        if (!ExisteUsuario(usuarioNoGranos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioNoGranos.Roles.Add(usuarioNuevo);
                            usuarioNoGranos.TipoUsuario = TipoUsuario.GetTipoNoGranos();

                            RegistrarUsuarioGenerico(usuarioNoGranos);
                        }
                        usuario = usuarioNoGranos;
                        break;

                    case "Ambos":
                        Entidades.Usuario usuarioAmbos = new Entidades.Usuario(mail, CUIT);

                        if (!ExisteUsuario(usuarioAmbos))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioAmbos.Roles.Add(usuarioNuevo);
                            usuarioAmbos.TipoUsuario = TipoUsuario.GetTipoAmbos();

                            RegistrarUsuarioGenerico(usuarioAmbos);
                        }
                        usuario = usuarioAmbos;
                        break;

                    case "Corredor":
                        Entidades.Usuario usuarioCorredor = new Entidades.Usuario(mail, CUIT);

                        if (!ExisteUsuario(usuarioCorredor))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCorredor.Roles.Add(usuarioNuevo);
                            usuarioCorredor.TipoUsuario = TipoUsuario.GetTipoCorredor();

                            RegistrarUsuarioGenerico(usuarioCorredor);
                        }
                        usuario = usuarioCorredor;
                        break;

                    case "Cliente":
                        Entidades.Usuario usuarioCliente = new Entidades.Usuario(mail, CUIT);

                        if (!ExisteUsuario(usuarioCliente))
                        {
                            Rol usuarioNuevo = ObtenerRolUsuarioNoImplementado();

                            usuarioCliente.Roles.Add(usuarioNuevo);
                            usuarioCliente.TipoUsuario = TipoUsuario.GetTipoCliente();

                            RegistrarUsuarioGenerico(usuarioCliente);
                        }
                        usuario = usuarioCliente;
                        break;
                }

            }
            catch(Exception ex)
            {
            }

            return Redirect("/");
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
