using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.SqlClient;

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
                    TipoUsuario = new TipoUsuarioDto
                    {
                        Id = u.TipoUsuario.Id,
                        Nombre = u.TipoUsuario.Nombre,
                        NombreCorto = u.TipoUsuario.NombreCorto
                    },
                    u.OrganizacionDeCompra,
                    Proveedores = u.Proveedores.Select(x => new { x.CUIT, TipoId = x.TipoProveedor.Id, x.RazonSocial })
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
                    TipoUsuario = u.TipoUsuario,
                    Tipo = (
                        (u.TipoUsuario.NombreCorto == "G" || u.TipoUsuario.NombreCorto == "NG" || u.TipoUsuario.NombreCorto == "A") ? "Proveedor" :
                        u.TipoUsuario.NombreCorto == "CORR" ? "Corredor" :
                        u.TipoUsuario.NombreCorto == "CLI" ? "Cliente" : ""
                    ),
                    CodigoProveedor = (
                        string.IsNullOrEmpty(u.CUITRegistro) ? "" :
                        (u.TipoUsuario.NombreCorto == "CORR" && u.CUITRegistro.Length >= 10) ? string.Concat("C", u.CUITRegistro.Substring(2, 8)) :
                        (u.TipoUsuario.NombreCorto != "CORR" && u.CUITRegistro.Length >= 10) ? string.Concat("00", u.CUITRegistro.Substring(2, 8)) :
                        "CUIT INVALIDO"
                    ),
                    OrganizacionDeCompra = u.OrganizacionDeCompra,
                    RazonSocial =
                        u.Proveedores.Any() ?
                        (
                            u.Proveedores.FirstOrDefault(x => x.CUIT == u.CUITRegistro && x.TipoId == u.TipoUsuario.Id) ??
                            u.Proveedores.FirstOrDefault(x => x.CUIT == u.CUITRegistro) ??
                            u.Proveedores.FirstOrDefault()
                        ).RazonSocial
                        : ""
                }).ToList();

            return usuariosDto;
        }

        public bool VerificarActividadUsuario(Usuario usuario)
        {
            return ExecuteQuery<VerificarActividadUsuario>
                ("exec VerificarActividadUsuarioID @IdUsuario", new SqlParameter("@IdUsuario", usuario.Id))
                .Any(verificacion=>verificacion.SeEncontraronRegistros);
        }
    }
}
