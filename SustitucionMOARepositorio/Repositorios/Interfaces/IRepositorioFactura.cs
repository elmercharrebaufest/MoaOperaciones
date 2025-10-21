using SustitucionMOAModel.Dto.Compras.Factura;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioFactura : IRepositorio
    {
        IEnumerable<EstadoCertificacion> ObtenerEstadosCertificaciones(ICollection<string> nrosCertificacionesSap);
    }
}
