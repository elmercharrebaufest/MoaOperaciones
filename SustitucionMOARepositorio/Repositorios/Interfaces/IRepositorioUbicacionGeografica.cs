using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioUbicacionGeografica : IRepositorio
    {
        DistanciaDomicilio ObtenerDistanciaSegunDescripcionDomicilio(string domicilioDescripcion);
        List<DistanciaDomicilioReemplazos> ObtenerReemplazosParaDomicilios();
    }
}
