using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
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
                    u.OrganizacionDeCompra,
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
                    Tipo = (
                        (u.TipoUsuario.NombreCorto == "G" || u.TipoUsuario.NombreCorto == "NG" || u.TipoUsuario.NombreCorto == "A") ? "Proveedor" :
                        u.TipoUsuario.NombreCorto == "CORR" ? "Corredor" :
                        u.TipoUsuario.NombreCorto == "CLI" ? "Cliente" : ""
                    ),
                    CodigoProveedor = u.Proveedores.FirstOrDefault(a => a.CUIT == u.CUITRegistro && a.NombreCorto == u.TipoUsuario.NombreCorto)?.CodigoProveedor ?? "",
                    OrganizacionDeCompra = u.OrganizacionDeCompra,
                    RazonSocial = u.Proveedores.FirstOrDefault(a => a.CUIT == u.CUITRegistro && a.NombreCorto == u.TipoUsuario.NombreCorto)?.RazonSocial ?? "",
                }).ToList();

            return usuariosDto;
        }

        public bool VerificarActividadUsuario(Usuario usuario)
        {
            return ExecuteQuery<VerificarActividadUsuario>
                ("exec VerificarActividadUsuarioID @IdUsuario", new SqlParameter("@IdUsuario", usuario.Id))
                .Any(verificacion => verificacion.SeEncontraronRegistros && verificacion.Tabla != "ProveedorHistorialAprobacion");
        }
    }
}
