using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class OrganizacionDeCompraDto
    {
        public string Id { get; set; }

        public string Descripcion { get; set; }

        public OrganizacionDeCompraDto() { }

        public OrganizacionDeCompraDto(OrganizacionDeCompra entity)
        {
            if (entity != null)
            {
                Id = entity.Id;
                Descripcion = entity.Descripcion;
            }
        }
    }
}
