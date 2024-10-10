using Entities = SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class OrdenResiduosApiDto : OrdenDeCargaApiDtoBase
    {
        public int AlmacenId { get; set; }
        public string KmARecorrer { get; set; }
        public static OrdenResiduosApiDto From(Entities.OrdenResiduos ordenResiduos)
        {
            return new OrdenResiduosApiDto
            {
                Id = ordenResiduos.Id,
                AlmacenId = ordenResiduos.AlmacenId,
                Cliente = ordenResiduos.Cliente.RazonSocial,
                CodigoProducto = ordenResiduos.Producto.CodigoSap,
                CUILChofer = ordenResiduos.ChoferCuil,
                CUITCliente = ordenResiduos.Cliente.CUIT,
                CUITTransporte = ordenResiduos.TransporteCuit,
                DescripcionProducto = ParseNombreProducto(ordenResiduos.Producto.Nombre),
                DomicilioDescr = ordenResiduos.DomicilioDescr,
                DomicilioOrden = ordenResiduos.DomicilioOrden,
                DomicilioTipo = ordenResiduos.DomicilioTipo,
                FechaCreacion = ordenResiduos.FechaCreacion.ToString(),
                LocalidadDescripcion = ordenResiduos.LocalidadScatoDescripcion,
                LocalidadId = ordenResiduos.LocalidadScatoId,
                NombreChofer = ordenResiduos.ChoferNombre,
                ChoferApellido = ordenResiduos.ChoferApellido,
                Observacion = ordenResiduos.Observacion,
                PatenteAcoplado = ordenResiduos.PatenteAcoplado,
                PatenteChasis = ordenResiduos.PatenteChasis,
                PlantaCodigo = ordenResiduos.PlantaCodigo,
                RazonSocialTransporte = ordenResiduos.TransporteRazonSocial,
                KmARecorrer = ordenResiduos.KmsARecorrer,
                TipoOrden = TipoOrdenes.RESIDUOS
            };
        }
    }
}
