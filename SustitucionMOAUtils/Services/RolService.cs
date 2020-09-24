using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class RolService: IRolService
    {
        protected readonly IRepositorio repositorio;

        public RolService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Rol ObtenerRolPorCodigo(string codigo) => repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
    }
}
