using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class SolpService : ISolpService
    {
        private readonly IRepositorio repositorio;

        public SolpService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public string ObtenerNumeroSolp(int solpId)
        {
            return repositorio.Obtener<Solp, string>(a => a.Id == solpId, a => a.NroSolp);
        }
    }
}
