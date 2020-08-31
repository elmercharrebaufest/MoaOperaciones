using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services
{
    public class AzureB2CService : IAzureB2CService
    {

        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroService dataAgroService;

        public AzureB2CService(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public Usuario LoguearUsuario(string mail, string CUIT, string granosFlag)
        {
            Usuario usuario = new Usuario();

            switch (granosFlag.ToLower())
            {
                case "granos":
                    UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT };

                    if (!ExisteUsuario(usuarioGranos))
                    {
                        RegistrarUsuarioGranos(ref usuarioGranos);
                    }
                    else
                    {
                        usuarioGranos = BuscarUsuarioGranos(usuarioGranos);
                    }

                    usuario = usuarioGranos;
                    break;

                case "no granos":
                    UsuarioNoGranos usuarioNoGranos = new UsuarioNoGranos { Mail = mail, CUITRegistro = CUIT };

                    if (!ExisteUsuario(usuarioNoGranos))
                    {
                        Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

                        usuarioNoGranos.Roles = new List<Rol>();
                        usuarioNoGranos.Proveedores = new List<Proveedor>();

                        usuarioNoGranos.Roles.Add(rolUsuarioNoImplementado);
                        usuarioNoGranos.TipoUsuario = ObtenerTipoPorNombreCorto("NG");

                        RegistrarUsuarioGenerico(usuarioNoGranos);
                        usuario = usuarioNoGranos;

                    }
                    else
                    {
                        usuario = BuscarUsuarioPorMail(mail);
                    }
                    break;

                case "ambos":
                    Usuario usuarioAmbos = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    if (!ExisteUsuario(usuarioAmbos))
                    {
                        Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

                        usuarioAmbos.Roles = new List<Rol>();
                        usuarioAmbos.Proveedores = new List<Proveedor>();

                        usuarioAmbos.Roles.Add(rolUsuarioNoImplementado);
                        usuarioAmbos.TipoUsuario = ObtenerTipoPorNombreCorto("A");

                        RegistrarUsuarioGenerico(usuarioAmbos);
                    }
                    else
                    {
                        usuarioAmbos = BuscarUsuarioPorMail(mail);
                    }

                    usuario = usuarioAmbos;
                    break;

                case "corredor":
                    Usuario usuarioCorredor = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    if (!ExisteUsuario(usuarioCorredor))
                    {
                        Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

                        usuarioCorredor.Roles = new List<Rol>();
                        usuarioCorredor.Proveedores = new List<Proveedor>();

                        usuarioCorredor.Roles.Add(rolUsuarioNoImplementado);
                        usuarioCorredor.TipoUsuario = ObtenerTipoPorNombreCorto("CORR");

                        RegistrarUsuarioGenerico(usuarioCorredor);
                    }
                    else
                    {
                        usuarioCorredor = BuscarUsuarioPorMail(mail);
                    }

                    usuario = usuarioCorredor;
                    break;

                case "cliente":
                    Usuario usuarioCliente = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    if (!ExisteUsuario(usuarioCliente))
                    {
                        Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

                        usuarioCliente.Roles = new List<Rol>();
                        usuarioCliente.Proveedores = new List<Proveedor>();

                        usuarioCliente.Roles.Add(rolUsuarioNoImplementado);
                        usuarioCliente.TipoUsuario = ObtenerTipoPorNombreCorto("CLI");

                        RegistrarUsuarioGenerico(usuarioCliente);
                    }
                    else
                    {
                        usuarioCliente = BuscarUsuarioPorMail(mail);
                    }

                    usuario = usuarioCliente;
                    break;
            }

            return usuario;
        }

        private UsuarioGranos BuscarUsuarioGranos(UsuarioGranos usuarioGranos)
        {
            return repositorio.Obtener<UsuarioGranos>(u => u.Mail == usuarioGranos.Mail);
        }

        private Usuario BuscarUsuarioPorMail(string mail)
        {
            return repositorio.Obtener<Usuario>(u => u.Mail == mail);
        }

        public NoticiasDetallesWSMOAResponse getNoticias(string proveedor)
        {
            try
            {
                return new NoticiasDetalleConsumerMOA().request(proveedor, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto)
        {
            return repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);
        }

        public bool RegistrarUsuarioGranos(ref UsuarioGranos usuario)
        {
            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente
            };

            return ValidarCUITProveedor(ref usuario, proveedor);
       

            //usuario.Proveedores.Add(proveedor);

            //usuario.Habilitado = true;

            //repositorio.Agregar(usuario);
            //return repositorio.GuardarCambios() == 1;
        }

        public bool RegistrarUsuarioGenerico(Usuario usuario)
        {
            Proveedor proveedor = new Proveedor();
            proveedor.CUIT = usuario.CUITRegistro;
            proveedor.EstadoAprobacion = EstadoAprobacion.AunNoImplementado;
            proveedor.Observaciones = "El tipo de usuario seleccionado aún no ha sido implementado. Contactese con su comercial.";

            usuario.Proveedores.Add(proveedor);

            usuario.Habilitado = true;

            repositorio.Agregar(usuario);
            return repositorio.GuardarCambios() == 1;
        }

        public bool ExisteUsuario(Usuario usuario)
        {
            return (repositorio.Existe<Usuario>(u => u.Mail == usuario.Mail));
        }

        public bool ValidarCUITProveedor(ref UsuarioGranos usuario, Proveedor proveedor)
        {
            return dataAgroService.ValidarCUITProveedorGranos(ref usuario, proveedor);
        }

        public bool ExisteProveedor(Proveedor proveedor)
        {
            return (repositorio.Existe<Proveedor>(u => u.CUIT == u.CUIT));
        }

        public bool RegistrarProveedor(Proveedor proveedor)
        {
            if (!ExisteProveedor(proveedor))
            {
                repositorio.Agregar(proveedor);
                return repositorio.GuardarCambios() == 1;
            }

            return true;
        }
        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
        }

        public Usuario ObtenerUsuario(string mail, string granosFlag)
        {
            return BuscarUsuarioPorMail(mail);
        }
    }
}
