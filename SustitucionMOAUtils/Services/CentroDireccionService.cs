using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class CentroDireccionService : ICentroDireccionService
    {
        private readonly IRepositorio repositorio;

        public CentroDireccionService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public CentroDireccion GetCentroDireccionByCodigoSap(string codigoSap)
        {
            return repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == codigoSap);
        }
    }
}
