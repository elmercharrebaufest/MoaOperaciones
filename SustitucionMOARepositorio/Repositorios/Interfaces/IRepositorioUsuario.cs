using SustitucionMOAModel.Dto;
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
        bool VerificarActividadUsuario(Usuario usuario);
    }
}
