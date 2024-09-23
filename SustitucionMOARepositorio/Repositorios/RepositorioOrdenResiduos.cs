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
                    ValidaSisaRuca = m.ValidaSisaRuca,
                    Almacenes = m.Almacenes.Select(a => new SustitucionMOAModel.Dto.AlmacenDto { Id = a.Id, Nombre = a.Nombre }).ToList()
                };

            return materiales.ToArray();
        }

        public List<OrdenResiduosFila> ObtenerListadoOrdenes(DateTime fechaInicio, DateTime fechaFin)
        {
            var ordenes = (
                from o in Set<OrdenResiduos>()
                where
                    o.FechaCreacion >= fechaInicio &&
                    o.FechaCreacion < DbFunctions.AddDays(fechaFin, 1)
                orderby o.FechaCreacion descending
                select new
                {
                    o.Id,
                    o.Estado.Semaforo,
                    DescripcionEstado = o.Estado.Nombre,
                    o.FechaCreacion,
                    LocalidadDescripcion = o.Localidad.Nombre,
                    Material = o.Producto.Nombre,
                    o.PatenteChasis,
                    RazonSocialCliente = o.Cliente.RazonSocial
                }).ToList();

            return ordenes
                .Select(x => new OrdenResiduosFila
                {
                    Id = x.Id,
                    ColorSemaforo = x.Semaforo,
                    DescripcionEstado = x.DescripcionEstado,
                    FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy"),
                    LocalidadDescripcion = x.LocalidadDescripcion,
                    Material = x.Material,
                    PatenteChasis = x.PatenteChasis,
                    RazonSocialCliente = x.RazonSocialCliente
                })
                .ToList();
        }

        public LocalidadDto[] ObtenerLocalidades()
        {
            var localidades =
                from l in Set<Localidad>()
                orderby l.Nombre
                select new LocalidadDto
                {
                    Id = l.LocalidadId,
                    Nombre = l.Nombre//,
                    //ProvinciaNombre = l.Provincia.Nombre
                };

            return localidades.ToArray();
        }

        public List<SustitucionMOAModel.Dto.ProveedorDto> ObtenerClientesResiduos()
        {
            var lista =
                from u in Set<Usuario>()
                where
                    u.TipoUsuario.Id == (int)TipoUsuarioEnum.Cliente &&
                    u.Habilitado &&
                    u.Roles.Any(r => r.Codigo == "RESIDUOS")
                select u.Proveedores
                    .Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                    .Select(x => new SustitucionMOAModel.Dto.ProveedorDto
                    {
                        Id = x.Id,
                        CodigoProveedor = x.CodigoProveedor,
                        CUIT = x.CUIT,
                        RazonSocial = x.RazonSocial
                    }).ToList();

            var clientes = new List<SustitucionMOAModel.Dto.ProveedorDto>();
            foreach (var item in lista)
            {
                clientes.AddRange(item);
            }
            return clientes;
        }

        public PatentesClienteDto ObtenerPatentesDeOrdenes(int clienteId)
        {
            var patentes =
                from o in Set<OrdenResiduos>()
                where o.ClienteId == clienteId
                select new
                {
                    o.PatenteChasis,
                    o.PatenteAcoplado
                };

            var patentesDto = new PatentesClienteDto
            {
                PatentesChasis = patentes.Select(x => x.PatenteChasis).Distinct().ToArray(),
                PatentesAcoplado = patentes.Select(x => x.PatenteAcoplado).Distinct().ToArray()
            };

            return patentesDto;
        }

        public TransportesIds ObtenerIdsTransportes(int clienteId, string patenteAcoplado)
        {
            var ids = (
                from o in Set<OrdenResiduos>()
                where
                    o.ClienteId == clienteId &&
                    o.PatenteAcoplado == patenteAcoplado &&
                    o.Cliente.EstadoAprobacion == EstadoAprobacion.Aprobado
                select new { o.ChoferCuil, o.TransporteCuit }
                ).ToList();

            return new TransportesIds
            {
                CuilsChoferes = ids.Select(x => x.ChoferCuil).Distinct().ToArray(),
                CuitsTransporte = ids.Select(x => x.TransporteCuit).Distinct().ToArray()
            };
        }

        public OrdenResiduos ObtenerOrdenResiduos(int idOrden)
        {
            return Obtener<OrdenResiduos>(idOrden);
        }

        public Usuario ObtenerUsuarioPorMail(string mailUsuario)
        {
            var usuario = Obtener<Usuario>(u => u.Mail == mailUsuario);
            return usuario;
        }

        public Proveedor ObtenerProveedor(int idProveedor)
        {
            return Obtener<Proveedor>(idProveedor);
        }
    }
}
