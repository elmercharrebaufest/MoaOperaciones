using SustitucionMOAModel.Dto;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioCompras : IRepositorio
    {
        List<MaterialSolpDto> BuscarMaterialesCatalogadosPorCodigoSap(string codigoSapMatch, int centroId);
        List<ServicioSolpDto> BuscarServiciosCatalogadosPorCodigoSap(string codigoSapMatch);
    }
}
