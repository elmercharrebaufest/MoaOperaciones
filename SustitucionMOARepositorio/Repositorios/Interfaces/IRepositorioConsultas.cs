using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioConsultas
    {
        List<CategoriaDto> ListaCategorias(IEnumerable<string> incluir = null, IEnumerable<string> excluir = null);
        List<EstadoConsultaDto> ListaEstadosConsultas(IEnumerable<int> categoriasPermitidasId = null);
    }
}
