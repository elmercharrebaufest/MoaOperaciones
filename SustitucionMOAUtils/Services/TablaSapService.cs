using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class TablaSapService : ITablaSapService
    {
        private readonly IRepositorio repositorio;

        public TablaSapService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public TablaSap GetById(int id)
        {
            return repositorio.Obtener<TablaSap>(x => x.Id == id);
        }
    }
}
