using SustitucionMOAModel.Dto.OrdenResiduos;
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
    public class RepositorioOrdenResiduos : RepositorioEF, IRepositorioOrdenResiduos
    {
        public RepositorioOrdenResiduos(DbContext context) : base(context) { }

        public List<OrdenResiduosFila> ObtenerListadoOrdenes()
        {
            var ordenes =
                from o in Set<OrdenResiduos>()
                select new OrdenResiduosFila
                {
                    Id = o.Id,
                    Corredor = o.CorredorId != null ? o.Corredor.RazonSocial : null,
                    Cliente = o.Cliente.RazonSocial,
                    Estado = o.Estado.Nombre
                };

            return ordenes.ToList();
        }
    }
}
