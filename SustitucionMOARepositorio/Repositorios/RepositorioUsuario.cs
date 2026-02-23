using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.UsuarioDtos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioUsuario : RepositorioEF, IRepositorioUsuario
    {
        public RepositorioUsuario(DbContext context) : base(context) { }

        public List<UsuarioDto> ObtenerUsuarios()
        {
            var usuariosBd = (
                from u in Set<Usuario>()
                select new
                {
                    u.Id,
                    u.Mail,
                    u.Habilitado,
                    u.CUITRegistro,
                    u.UsuarioSap,
                    u.Suplente,
                    TipoUsuario = new TipoUsuarioDto
                    {
                        Id = u.TipoUsuario.Id,
                        Nombre = u.TipoUsuario.Nombre,
                        NombreCorto = u.TipoUsuario.NombreCorto
                    },
                    u.Externo,
                    Proveedores = u.Proveedores.Select(x => new { x.CUIT, x.TipoProveedor.NombreCorto, TipoId = x.TipoProveedor.Id, x.RazonSocial, x.CodigoProveedor }
                    ),
                }).ToList();

            var usuariosDto = (
                from u in usuariosBd
                select new UsuarioDto
                {
                    Id = u.Id,
                    Mail = u.Mail,
                    Habilitado = u.Habilitado,
                    CUIT = u.CUITRegistro,
                    UsuarioSap = string.IsNullOrEmpty(u.UsuarioSap) ? "" : u.UsuarioSap,
                    Suplente = string.IsNullOrEmpty(u.Suplente) ? "" : u.Suplente,
                    TipoUsuario = u.TipoUsuario,
                    Externo = u.Externo,
                    Tipo = (
                        (u.TipoUsuario.NombreCorto == "G" || u.TipoUsuario.NombreCorto == "NG" || u.TipoUsuario.NombreCorto == "A") ? "Proveedor" :
                        u.TipoUsuario.NombreCorto == "CORR" ? "Corredor" :
                        u.TipoUsuario.NombreCorto == "CLI" ? "Cliente" : ""
                    ),
                    CodigoProveedor = u.Proveedores.FirstOrDefault(a => a.CUIT == u.CUITRegistro && a.NombreCorto == u.TipoUsuario.NombreCorto)?.CodigoProveedor ?? "",
                    RazonSocial = u.Proveedores.FirstOrDefault(a => a.CUIT == u.CUITRegistro && a.NombreCorto == u.TipoUsuario.NombreCorto)?.RazonSocial ?? "",
                }).ToList();

            return usuariosDto;
        }

        public Usuario ObtenerSuplenteEnPeriodo(string mailUsuario, DateTime fechaDesde, DateTime fechaHasta)
        {
            fechaDesde = fechaDesde.Date;
            fechaHasta = fechaHasta.Date;

            var qrySuplente =
                from usuario in Set<Usuario>()
                join reasignacion in Set<UsuarioReasignacion>() on usuario.Id equals reasignacion.Usuario_Id
                where
                    usuario.Mail == mailUsuario &&
                    fechaDesde <= reasignacion.FechaHasta &&
                    fechaHasta >= reasignacion.FechaDesde
                select usuario;

            return qrySuplente.FirstOrDefault();
        }

        public List<ProveedorARelacionar> GetProveedoresARelacionar(string cuit)
        {
            var proveedoresARelacionar = (
                from prov in Set<Proveedor>()
                where
                    prov.CUIT == cuit &&
                    prov.EstadoAprobacion == EstadoAprobacion.Aprobado
                select new ProveedorARelacionar
                {
                    Id = prov.Id,
                    CodigoProveedor = prov.CodigoProveedor,
                    CUIT = prov.CUIT,
                    IdTipoProveedor = prov.TipoProveedor.Id,
                    RazonSocial = prov.RazonSocial,
                    TipoProveedor = prov.TipoProveedor.Nombre
                })
                .ToList();

            return proveedoresARelacionar;
        }

        public List<string> GetMailsUsuariosConPermisos(ICollection<string> permisos)
        {
            var mailsUsuariosAprobadores = Listar<Usuario, string>(
                u => u.Mail,
                u => u.Roles.Any(r => r.PermisosAsociados.Any(p => permisos.Contains(p.Permiso))));

            return mailsUsuariosAprobadores;
        }

        public bool VerificarActividadUsuario(Usuario usuario)
        {
            return ExecuteQuery<VerificarActividadUsuario>
                ("exec VerificarActividadUsuarioID @IdUsuario", new SqlParameter("@IdUsuario", usuario.Id))
                .Any(verificacion => verificacion.SeEncontraronRegistros && verificacion.Tabla != "ProveedorHistorialAprobacion");
        }
    }
}
