using SustitucionMOAModel.Dto.Compras.Factura;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioFactura : IRepositorio
    {
        void BorrarResultadosAnalisisOcr(ICollection<ResultadoAnalisisOcr> resultadosAnalisisOcr);
        void BorrarResultadosOcr(ICollection<ResultadoOcr> resultadosOcr);
        IEnumerable<EstadoCertificacion> ObtenerEstadosCertificaciones(ICollection<string> nrosCertificacionesSap);
    }
}
