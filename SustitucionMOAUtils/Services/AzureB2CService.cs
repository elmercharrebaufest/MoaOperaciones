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
            Usuario usuario = new Usuario { Mail = mail, CUITRegistro = CUIT };

            if (ExisteUsuario(usuario))
            {
                usuario = BuscarUsuarioPorMail(mail);
            }
            else
            {
                RegistrarUsuario(mail, CUIT, granosFlag, ref usuario);
            }

            return usuario;
        }

        private Usuario RegistrarUsuario(string mail, string CUIT, string granosFlag, ref Usuario usuario)
        {
            switch (granosFlag.ToLower())
            {
                case "granos":
                    UsuarioGranos usuarioGranos = new UsuarioGranos { Mail = mail, CUITRegistro = CUIT };

                    RegistrarUsuarioGranos(ref usuarioGranos);
                    usuario = usuarioGranos;
                    break;

                case "no granos":
                    Usuario usuarioNoGranos = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    usuarioNoGranos.TipoUsuario = ObtenerTipoPorNombreCorto("NG");
                    RegistrarUsuarioGenerico(ref usuarioNoGranos);
                    usuario = usuarioNoGranos;
             
                    break;

                case "ambos":
                    Usuario usuarioAmbos = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    usuarioAmbos.TipoUsuario = ObtenerTipoPorNombreCorto("A");

                    RegistrarUsuarioGenerico(ref usuarioAmbos);

                    usuario = usuarioAmbos;
                    break;

                case "corredor":
                    Usuario usuarioCorredor = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    usuarioCorredor.TipoUsuario = ObtenerTipoPorNombreCorto("CORR");

                    RegistrarUsuarioGenerico(ref usuarioCorredor);

                    usuario = usuarioCorredor;
                    break;

                case "cliente":
                    Usuario usuarioCliente = new Usuario { Mail = mail, CUITRegistro = CUIT };

                    usuarioCliente.TipoUsuario = ObtenerTipoPorNombreCorto("CLI");

                    RegistrarUsuarioGenerico(ref usuarioCliente);
 
                    usuario = usuarioCliente;
                    break;
            }

            return usuario;
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
        }

        public bool RegistrarUsuarioGenerico(ref Usuario usuario)
        {
            Rol rolUsuarioNoImplementado = ObtenerRolPorCodigo("NOIMP");

            usuario.Roles = new List<Rol>
            {
                rolUsuarioNoImplementado
            };

            usuario.Proveedores = new List<Proveedor>();

            Proveedor proveedor = new Proveedor
            {
                CUIT = usuario.CUITRegistro,
                EstadoAprobacion = EstadoAprobacion.AunNoImplementado,
                Observaciones = "El tipo de usuario seleccionado aún no ha sido implementado. Contactese con su comercial."
            };

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
