using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.UsuarioDtos;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioUsuario : IRepositorio
    {
        Usuario ObtenerSuplenteEnPeriodo(string mailUsuario, DateTime fechaDesde, DateTime fechaHasta);
        List<UsuarioDto> ObtenerUsuarios();
        List<ProveedorARelacionar> GetProveedoresARelacionar(string cuit);
        bool VerificarActividadUsuario(Usuario usuario);
        List<string> GetMailsUsuariosConPermisos(ICollection<string> permisos);
    }
}
