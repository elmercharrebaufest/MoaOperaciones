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
        LoginService _loginService = new LoginService();
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

                string GranosFlag = ClaimsPrincipalExtension.GetClaimValue("extension_Tipodeproveedor");
                
                Entidades.Usuario usuario = new Entidades.Usuario(mail, CUIT);

                if (GranosFlag.Equals("Granos"))
                {
                    UsuarioGranos usuarioGranos = new UsuarioGranos(mail, CUIT);

                    Proveedor proveedor = new Proveedor();
                    proveedor.CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");
                    proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;

                    if (!ExisteUsuario(usuarioGranos))
                    {
                        Rol usuarioNuevo = ObtenerRolUsuarioNuevo();

                        usuarioGranos.Roles.Add(usuarioNuevo);

                        RegistrarUsuarioGranos(usuarioGranos);
                    }
                    usuario = usuarioGranos;
                }

            }
            catch(Exception ex)
            {
            }

            return Redirect("/");
        }

        public bool RegistrarUsuarioGranos(UsuarioGranos usuario)
        {
            Proveedor proveedor = new Proveedor();
            proveedor.CUIT = ClaimsPrincipalExtension.GetClaimValue("extension_CUIT");
            proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;

            ValidarCUITProveedor(usuario, proveedor);

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
    }
}
