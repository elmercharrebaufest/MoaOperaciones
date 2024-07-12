using Entities = SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class OrdenDeCargaFasonApiDto : OrdenDeCargaNoResiduoApiDto
    {
        public bool ClienteComoRemitenteComercial { get; set; }
        public string CUITDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string DestinoMercaderia { get; set; }
        public bool Escalable { get; set; }
        public string FechaRetiro { get; set; }
        public bool FleteMOA { get; set; }
        public string KmARecorrer { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }


        public OrdenDeCargaFasonApiDto(Entities.OrdenDeCargaFason ordenFason)
        {
            Id = ordenFason.Id;
            Cantidad = ordenFason.Cantidad;
            Cliente = ordenFason.Cliente.RazonSocial;
            ClienteComoRemitenteComercial = ordenFason.ClienteComoRemitenteComercial;
            CodigoProducto = ordenFason.Producto.CodigoSap;
            CUILChofer = ordenFason.CUILChofer;
            CUITCliente = ordenFason.Cliente.CUIT;
            CUITCorredor = ordenFason.CorredorId != null ? ordenFason.Corredor.CUIT : null;
            CUITDestinatario = ordenFason.CUITDestinatario;
            CUITDestino = ordenFason.CUITDestino;
            CUITIntermediarioFlete = ordenFason.CUITIntermediarioFlete;
            CUITTransporte = ordenFason.CUITTransporte;
            DescripcionProducto = ParseNombreProducto(ordenFason.Producto.Nombre);
            DestinoMercaderia = ordenFason.DestinoMercaderia;
            DomicilioDescr = ordenFason.DomicilioDescr;
            DomicilioOrden = ordenFason.DomicilioOrden;
            DomicilioTipo = ordenFason.DomicilioTipo;
            Escalable = ordenFason.Escalable;
            FechaCreacion = ordenFason.FechaCreacion.ToString();
            FechaRetiro = ordenFason.FechaRetiro.ToString();
            FleteMOA = ordenFason.FleteMOA;
            KmARecorrer = ordenFason.KmARecorrer;
            LocalidadDescripcion = ordenFason.LocalidadDescripcion;
            LocalidadId = ordenFason.LocalidadId;
            NombreChofer = ordenFason.NombreChofer;
            Observacion = ordenFason.Observacion;
            PatenteAcoplado = ordenFason.PatenteAcoplado;
            PatenteChasis = ordenFason.PatenteChasis;
            PlantaCodigo = ordenFason.PlantaCodigo;
            RazonSocialDestinatario = ordenFason.RazonSocialDestinatario;
            RazonSocialDestino = ordenFason.RazonSocialDestino;
            RazonSocialIntermediarioFlete = ordenFason.RazonSocialIntermediarioFlete;
            RazonSocialTransporte = ordenFason.RazonSocialTransporte;
            TipoOrden = TipoOrdenes.FASON;
        }
    }
}
