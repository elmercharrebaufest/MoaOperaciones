using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICentroDireccionService
    {
        CentroDireccion GetCentroDireccionByCodigoSap(string CodigoSap);
    }
}
