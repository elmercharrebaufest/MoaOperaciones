using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Extensiones;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SustitucionMOARepositorio.ConsultasEF
{
    public class ListarCertificacionRegistradaConsulta : IConsultaPaginada<CertificacionRegistrada>
    {
        private readonly Paginacion Paginacion;
        private readonly string OrdenDeCompra;
        private readonly string Proveedor;
        private readonly DateTime? FechaInicio;
        private readonly DateTime? FechaFin;

        public ListarCertificacionRegistradaConsulta(
            Paginacion paginacion,
            string ordenDeCompra,
            string proveedor,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            Paginacion = paginacion;
            OrdenDeCompra = ordenDeCompra?.Trim();
            Proveedor = proveedor?.Trim();
            FechaInicio = fechaInicio;
            FechaFin = fechaFin.HasValue ? fechaFin.Value.AddDays(1) : fechaFin; // Incluir el día completo
        }

        public ListaPaginada<CertificacionRegistrada> Ejecutar(DbContext contexto)
        {
            try
            {
                // Configurar el tiempo de espera para la consulta
                ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

                // Construir la consulta base
                var query = contexto.Set<CertificacionRegistrada>().AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(OrdenDeCompra))
                {
                    query = query.Where(c => c.NRO_OC.Contains(OrdenDeCompra));
                }

                if (!string.IsNullOrEmpty(Proveedor))
                {
                    query = query.Where(c => c.Proveedor.RazonSocial.Contains(Proveedor));
                }

                if (FechaInicio.HasValue)
                {
                    query = query.Where(c => c.FechaDeRegistro >= FechaInicio.Value);
                }

                if (FechaFin.HasValue)
                {
                    query = query.Where(c => c.FechaDeRegistro < FechaFin.Value);
                }

                // Obtener el total de elementos antes de la paginación
                int totalItems = query.Count();

                // Aplicar ordenación
                if (!string.IsNullOrEmpty(Paginacion.OrdenarPor))
                {
                    query = Paginacion.DireccionOrden == DirOrden.Asc
                            ? query.OrderBy(x => x.FechaDeRegistro)
                            : query.OrderByDescending(x => x.FechaDeRegistro);
                }

                // Aplicar paginación
                query = query
                    .Skip((Paginacion.Pagina - 1) * Paginacion.ItemsPorPagina)
                    .Take(Paginacion.ItemsPorPagina);

                // Ejecutar la consulta y devolver los resultados
                var items = query.ToList();

                return new ListaPaginada<CertificacionRegistrada>(items, Paginacion.Pagina, Paginacion.ItemsPorPagina, totalItems);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                throw new Exception("Error al ejecutar la consulta de certificaciones registradas.", ex);
            }
        }
    }
}