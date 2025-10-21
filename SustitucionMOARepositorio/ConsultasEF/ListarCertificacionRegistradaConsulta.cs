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
        private readonly string ProveedorRazonSocial;
        private readonly string ProveedorCuit;
        private readonly DateTime? FechaInicio;
        private readonly DateTime? FechaFin;

        public ListarCertificacionRegistradaConsulta(
            Paginacion paginacion,
            string ordenDeCompra,
            string proveedorRazonSocial,
            string proveedorCuit,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            Paginacion = paginacion;
            OrdenDeCompra = ordenDeCompra?.Trim();
            ProveedorRazonSocial = proveedorRazonSocial?.Trim();
            ProveedorCuit = proveedorCuit?.Trim();
            FechaInicio = fechaInicio;
            FechaFin = fechaFin.HasValue ? fechaFin.Value.AddDays(1) : fechaFin; // Incluir el día completo
        }

        public ListaPaginada<CertificacionRegistrada> Ejecutar(DbContext contexto)
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

            if (!string.IsNullOrEmpty(ProveedorRazonSocial))
            {
                query = query.Where(c => c.Proveedor.RazonSocial.Contains(ProveedorRazonSocial));
            }

            if (!string.IsNullOrEmpty(ProveedorCuit))
            {
                query = query.Where(c => c.Proveedor.CUIT.Equals(ProveedorCuit));
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
    }
}