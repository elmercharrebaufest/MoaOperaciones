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

        public override bool Equals(object obj)
        {
            return obj is EstadoVendedorDto dto &&
                   CUIT == dto.CUIT &&
                   CodigoProveedor == dto.CodigoProveedor &&
                   RazonSocial == dto.RazonSocial &&
                   Estado == dto.Estado;
        }

        public override int GetHashCode()
        {
            int hashCode = -1246237457;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUIT);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodigoProveedor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocial);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Estado);
            return hashCode;
        }
    }
}
