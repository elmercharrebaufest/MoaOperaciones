using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras.POMultiple;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioCompras : IRepositorio
    {
        List<MaterialSolpDto> BuscarMaterialesCatalogadosPorCodigoSap(string codigoSapMatch, int centroId);
        List<ServicioSolpDto> BuscarServiciosCatalogadosPorCodigoSap(string codigoSapMatch);
        List<PeticionDeOfertaDesvincularDto> ListarPOsDesvinculablesDePosicionMaterial(int solpPosicionId);
        List<PeticionDeOfertaDesvincularDto> ListarPOsDesvinculablesDeSolpServicio(int solpId);
    }
}
