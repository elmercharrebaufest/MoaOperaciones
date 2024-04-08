using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioOrdenResiduos : RepositorioEF, IRepositorioOrdenResiduos
    {
        public RepositorioOrdenResiduos(DbContext context) : base(context) { }

        public MaterialDto[] ObtenerMateriales()
        {
            var materiales =
                from m in Set<Material>()
                where m.TablaSeccionMaterial == TablaSeccionMaterial.OrdenResiduos
                select new MaterialDto
                {
                    MaterialId = m.Id,
                    Descripcion = m.Nombre,
                    CodigoSap = m.CodigoSap,
                    ValidaSisaRuca = m.ValidaSisaRuca
                };

            return materiales.ToArray();
        }

        public List<OrdenResiduosFila> ObtenerListadoOrdenes(DateTime fechaInicio, DateTime fechaFin)
        {
            var ordenes = (
                from o in Set<OrdenResiduos>()
                where
                    o.FechaCreacion >= fechaInicio &&
                    o.FechaCreacion <= fechaFin
                select new
                {
                    o.Id,
                    o.Estado.Semaforo,
                    DescripcionEstado = o.Estado.Nombre,
                    o.FechaCreacion,
                    o.FechaRetiro,
                    LocalidadDescripcion = o.Localidad.Nombre,
                    Material = o.Producto.Nombre,
                    o.PatenteChasis,
                    RazonSocialCorredor = o.CorredorId != null ? o.Corredor.RazonSocial : null,
                    RazonSocialCliente = o.Cliente.RazonSocial
                }).ToList();

            return ordenes
                .Select(x => new OrdenResiduosFila
                {
                    Id = x.Id,
                    ColorSemaforo = x.Semaforo,
                    DescripcionEstado = x.DescripcionEstado,
                    FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    FechaRetiro = x.FechaRetiro.ToString("dd/MM/yyyy"),
                    LocalidadDescripcion = x.LocalidadDescripcion,
                    Material = x.Material,
                    PatenteChasis = x.PatenteChasis,
                    RazonSocialCorredor = x.RazonSocialCorredor,
                    RazonSocialCliente = x.RazonSocialCliente
                })
                .ToList();
        }
    }
}
