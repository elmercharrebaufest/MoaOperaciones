using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoVendedorDto
    {
        public string CUIT { get; set; }
        public string CodigoProveedor { get; set; }
        public string RazonSocial { get; set; }
        public string Estado { get; set; }

        public EstadoVendedorDto() { }

        public EstadoVendedorDto(VendedorHabilitadoWSMOAResponse response)
        {
            CodigoProveedor = response.cabeceras.FirstOrDefault().proveedor;
            RazonSocial = response.cabeceras.FirstOrDefault().descripcion;
            Estado = response.status;
        }

    }
}
