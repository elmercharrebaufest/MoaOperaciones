using SustitucionMOAModel.Entities;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IUbicacionGeograficaService
    {
        DistanciaDomicilio ObtenerDistanciaDePlantaMoaADestino(string direccionDestino);
    }
}
