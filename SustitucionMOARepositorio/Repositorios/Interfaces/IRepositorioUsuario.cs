using SustitucionMOAModel.Dto;
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
    }
}
