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
        List<UsuarioDto> ObtenerUsuarios();
        bool VerificarActividadUsuario(Usuario usuario);
    }
}
