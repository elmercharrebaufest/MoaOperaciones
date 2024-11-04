namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class OrdenDeCargaFasonApiDto : OrdenDeCargaNoResiduoApiDto
    {
        public string FechaRetiro { get; set; }
        public string KmARecorrer { get; set; }

        public OrdenDeCargaFasonApiDto(Entities.OrdenDeCargaFason ordenFason)
        {
            Id = ordenFason.Id;           
            Cantidad = ordenFason.Cantidad;
            ChoferApellido = ordenFason.ApellidoChofer;
            ChoferNombre = ordenFason.NombreChofer;
            Cliente = ordenFason.Cliente.RazonSocial;
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
            FleteMOA = ordenFason.FleteMOA;
            KmARecorrer = ordenFason.KmARecorrer;
            LocalidadDescripcion = ordenFason.LocalidadDescripcion;
            LocalidadId = ordenFason.LocalidadId;
            NombreChofer = ordenFason.ApellidoChofer + " " + ordenFason.NombreChofer;
            Observacion = ordenFason.Observacion;
            PatenteAcoplado = ordenFason.PatenteAcoplado;
            PatenteChasis = ordenFason.PatenteChasis;
            PlantaCodigo = ordenFason.PlantaCodigo;
            RazonSocialDestinatario = ordenFason.RazonSocialDestinatario;
            RazonSocialDestino = ordenFason.RazonSocialDestino;
            RazonSocialIntermediarioFlete = ordenFason.RazonSocialIntermediarioFlete;
            RazonSocialTransporte = ordenFason.RazonSocialTransporte;
            Reventa = ordenFason.ClienteComoRemitenteComercial;
            TipoOrden = TipoOrdenes.FASON;
        }
    }
}
