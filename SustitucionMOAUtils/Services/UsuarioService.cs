using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entidades = SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Services
{
    public class UsuarioService : IUsuarioService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IVendedorService vendedorService;

        public UsuarioService(IRepositorio repositorio, IVendedorService vendedorService)
        {
            this.repositorio = repositorio;
            this.vendedorService = vendedorService;
        }

        public void SeccionVisitada (string mailUsuario, string seccion)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == mailUsuario);
            if (usuario.SeccionesVisitadas.Contains(seccion)) return;

            usuario.SeccionesVisitadas += $"-{seccion}";
            repositorio.GuardarCambios();
        }

        public List<UsuarioDto> GetUsuarios()
        {
            try
            {
                List<Entidades.Usuario> usuarios = repositorio.Listar<Entidades.Usuario>();

                List<UsuarioDto> usuariosDto = usuarios.Select(x => new UsuarioDto(x)).ToList();

                if (usuariosDto.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Usuarios"));
                }
                return usuariosDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UsuarioDto GetUsuario(string email)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(x=> x.Mail == email);

                if (usuario == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Usuario", email));
                }
                var ret = new UsuarioDto(usuario);
                ret.Permisos = usuario.ObtenerPermisos();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string HabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            var proveedor = usuario.ObtenerProveedor();

            proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;

            usuario.Habilitado = true;

            usuario.RemoverRol("DES");

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioHabilitadoOK, usuario.Mail);
        }

        public string DeshabilitarUsuario(string usuarioMail)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == usuarioMail);

            usuario.Habilitado = false;

            var proveedor = usuario.ObtenerProveedor();

            proveedor.EstadoAprobacion = EstadoAprobacion.Deshabilitado;

            proveedor.Observaciones = "Su usuario ha sido deshabilitado.";

            var rolUsuario = ObtenerRolPorCodigo("DES");

            usuario.AgregarRol(rolUsuario);

            repositorio.GuardarCambios();

            return string.Format(SuccessMsg.UsuarioDeshabilitadoOK, usuario.Mail);
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo == codigo);
        }

        public byte[] getDocumento(string nombre)
        {
            string sourcePath = @"\\vicinf01\Legajo_Impositivo\Documentacion_MoaOperaciones";
            try
            {
                string[] filepaths = Directory.GetFiles(sourcePath);
                foreach (string filepath in filepaths)
                {
                    string result = Path.GetFileName("@\\" + filepath);
                    if (nombre == result)
                    {
                        byte[] file = System.IO.File.ReadAllBytes(filepath);
                        return file;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        public List<RolDropdownDto> GetRoles()
        {
            List<string> interno = new List<string>
            {
                "ADM", "OPE", "APRO", "COMPRAS", "ADMINCCSS", "TODOS", "COMERCIAL"
            };

            List<string> contacto = new List<string>
            {
                "BOL", "DATMAE", "REI", "ACT", "PAR", "FIN", "CAL", "COM",
                "COMP", "APP", "PES", "PAG", "FWEB", "MATBA",
                "PROVGC", "FLECONSULTA", "OTRO", "PARDIR", "PARCOR",
                "FINDIR", "FINCOR", "FLE"
            };

            var roles = repositorio.Listar<Rol>().Where(r => r.EsEditable)
                .Select(x => new RolDropdownDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Code = interno.Contains(x.Codigo)? "Interno" : contacto.Contains(x.Codigo)? "Contacto" : "Externo"
                }).ToList();

            return roles;
        }

        public string GuardarRoles(List<int> idRoles, int idUsuario)
        {
            Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Id == idUsuario);

            usuario.RemoverRolesEditables();

            foreach (int idRol in idRoles)
            {
                Rol rolAAgregar = repositorio.Obtener<Rol>(r => r.Id == idRol);
                usuario.AgregarRol(rolAAgregar);
            }

            var esAdministradorMolinos = usuario
                                            .Roles
                                            .Where(
                                                r => r.Codigo == "ADM"
                                                || r.Codigo == "TODOS"
                                                || r.Codigo == "OPE"
                                                || r.Codigo == "APRO"
                                                || r.Codigo == "COMPRAS"
                                                || r.Codigo == "COMERCIAL")
                                            .Any();

            //Si es usuario de molinos, buscamos todos los proveedores que tiene, lo aprobamos y le sacamos el historial de aprobación. 
            //Esto es para que estos proveedores no aparezcan en el listado de altas pendientes

            if (esAdministradorMolinos)
            {
                foreach (var prov in usuario.Proveedores)
                {
                    prov.EstadoAprobacion = EstadoAprobacion.Aprobado;

                    prov.Observaciones = "";

                    var historial = repositorio.Listar<ProveedorHistorialAprobacion>(h => h.Proveedor_Id == prov.Id);

                    repositorio.RemoverTodos(historial);

                    prov.HistorialAprobaciones = new List<ProveedorHistorialAprobacion>();

                }
            }

            repositorio.GuardarCambios();
            var proveedor = usuario.ObtenerProveedor();
            if (proveedor != null)
            {
                return string.Format(SuccessMsg.RolesActualizadosOk, usuario.Mail, " ( CUIT: " + proveedor.CUIT + ")");
            }
            else
            {
                return string.Format(SuccessMsg.RolesActualizadosOk, usuario.Mail, "");
            }

        }

        public List<RolDropdownDto> GetRolesUsuario(int idUsuario)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Id == idUsuario);

                var rolesDto = usuario.Roles.Select(x => new RolDropdownDto(x)).ToList();

                return rolesDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Rol> GetRolesUsuario(string email)
        {
            try
            {
                Entidades.Usuario usuario = repositorio.Obtener<Entidades.Usuario>(u => u.Mail == email);
                return usuario.Roles.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProveedorDto> GetVendedoresUsuario(string usuarioMail)
        {
            return vendedorService.GetVendedores(usuarioMail);
        }

        public ProveedorDto GetProveedorPorCodigo(string codigo)
        {

            Entidades.Proveedor proveedor = repositorio.Obtener<Entidades.Proveedor>(x => x.CodigoProveedor == codigo);

            if (proveedor == null)
            {
                throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Proveedor", codigo));
            }
            var ret = new ProveedorDto(proveedor);

            return ret;
        }
    }
}
