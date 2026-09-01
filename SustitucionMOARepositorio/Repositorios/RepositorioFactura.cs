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

        public void BorrarResultadosOcr(ICollection<ResultadoOcr> resultadosOcr)
        {
            BorrarTodos(resultadosOcr);
        }

        public void BorrarResultadosAnalisisOcr(ICollection<ResultadoAnalisisOcr> resultadosAnalisisOcr)
        {
            BorrarTodos(resultadosAnalisisOcr);
        }

        /// <summary>
        /// Este método es una alternativa al RemoverTodos de RepositorioEF, el cual puede colgarse al intentar remover una gran cantidad de entidades.
        /// Para no solucionar ese problema configurando context.Configuration.AutoDetectChangesEnabled = false en ese método genérico, se agrega este específico para Factura.
        /// Este método establece el estado de cada entidad a Deleted, lo que permite que Entity Framework maneje la eliminación de manera más eficiente.
        /// </summary>
        private void BorrarTodos<TEntidad>(ICollection<TEntidad> entidades) where TEntidad : class
        {
            if (entidades == null || entidades.Count == 0)
            {
                return;
            }

            var autoDetectChangesEnabledOriginal = context.Configuration.AutoDetectChangesEnabled;
            try
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                foreach (var entidad in entidades)
                {
                    context.Entry(entidad).State = EntityState.Deleted;
                }
            }
            finally
            {
                context.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabledOriginal;
            }
        }
    }
}
