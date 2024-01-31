using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ClienteSAPResponse
    {
        public string RazonSocial { get; set; }
        public string CuitCliente { get; set; }
        public string CodigoProveedor { get; set; }
    }
    public class CustomComparerClienteSap : IEqualityComparer<ClienteSAPResponse>
    {
        bool IEqualityComparer<ClienteSAPResponse>.Equals(ClienteSAPResponse x, ClienteSAPResponse y)
        {
            return x.RazonSocial == y.RazonSocial && x.CuitCliente == y.CuitCliente && x.CodigoProveedor == y.CodigoProveedor;
        }

        int IEqualityComparer<ClienteSAPResponse>.GetHashCode(ClienteSAPResponse obj)
        {
            int hashNombre = obj.RazonSocial.GetHashCode();
            int hashEdad = obj.CuitCliente.GetHashCode();
            int hashCiudad = obj.CodigoProveedor.GetHashCode();

            return hashNombre ^ hashEdad ^ hashCiudad;
        }
    }
}
