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
        private readonly string c_TipoOrden = "RESIDUOS";

        public int AlmacenId { get; set; }
        public string PagadorFlete { get; set; }

        public OrdenResiduosApiDto(Entities.OrdenResiduos ordenResiduos)
        {
            Id = ordenResiduos.Id;
            AlmacenId = ordenResiduos.AlmacenId;
            Cliente = ordenResiduos.Cliente.RazonSocial;
            CodigoProducto = ordenResiduos.Producto.CodigoSap;
            CUILChofer = ordenResiduos.ChoferCuil;
            CUITCliente = ordenResiduos.Cliente.CUIT;
            CUITTransporte = ordenResiduos.TransporteCuit;
            DescripcionProducto = ParseNombreProducto(ordenResiduos.Producto.Nombre);
            DomicilioDescr = ordenResiduos.DomicilioDescr;
            DomicilioOrden = ordenResiduos.DomicilioOrden;
            DomicilioTipo = ordenResiduos.DomicilioTipo;
            FechaCreacion = ordenResiduos.FechaCreacion.ToString();
            KmARecorrer = ordenResiduos.KmsARecorrer;
            LocalidadDescripcion = ordenResiduos.LocalidadDescripcion;
            LocalidadId = ordenResiduos.LocalidadId ?? 0;
            NombreChofer = ordenResiduos.ChoferApellido + " " + ordenResiduos.ChoferNombre;
            ChoferApellido = ordenResiduos.ChoferApellido;
            ChoferNombre = ordenResiduos.ChoferNombre;
            Observacion = ordenResiduos.Observacion;
            PagadorFlete = ordenResiduos.Producto.ValidaSisaRuca ? ordenResiduos.Cliente.CUIT : null;
            PatenteAcoplado = ordenResiduos.PatenteAcoplado;
            PatenteChasis = ordenResiduos.PatenteChasis;
            PlantaCodigo = ordenResiduos.PlantaCodigo;
            RazonSocialTransporte = ordenResiduos.TransporteRazonSocial;
            TipoOrden = c_TipoOrden;
        }
    }
}
