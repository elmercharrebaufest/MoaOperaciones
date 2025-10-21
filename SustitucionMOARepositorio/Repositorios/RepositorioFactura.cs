using SustitucionMOAModel.Dto.Compras.Factura;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioFactura : RepositorioEF, IRepositorioFactura
    {
        public RepositorioFactura(DbContext context) : base(context) { }

        public IEnumerable<EstadoCertificacion> ObtenerEstadosCertificaciones(ICollection<string> nrosCertificacionesSap)
        {
            var estados =
                Set<Aprobaciones>()
                .Where(a => a.NRO_ES_SAP.HasValue && nrosCertificacionesSap.Contains(a.NRO_ES_SAP.ToString()))
                .Select(a => new EstadoCertificacion
                {
                    NumeroCertificacionSap = a.NRO_ES_SAP.ToString(),
                    Estado = a.Estado_certificacion
                })
                .ToList();

            return estados;
        }
    }
}
