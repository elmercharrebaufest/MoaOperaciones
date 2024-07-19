namespace SustitucionMOAModel.Dto
{
    public class OrdenesDeCargaApiDto : OrdenDeCargaNoResiduoApiDto
    {
        public string Contrato { get; set; }
        public string Pedido { get; set; }

        public OrdenesDeCargaApiDto(Entities.OrdenDeCarga ordenFas)
        {

            Id = ordenFas.Id;
            Cantidad = ordenFas.Cantidad;
            CodigoProducto = ordenFas.Producto.CodigoSap;
            Contrato = ordenFas.ContratoIngresado;
            CUILChofer = ordenFas.CUITChofer;
            CUITCliente = ordenFas.CUITCliente;
            CUITCorredor = ordenFas.CUITCorredor;
            CUITTransporte = ordenFas.CUITTransporte;
            DescripcionProducto = ParseNombreProducto(ordenFas.Producto.Nombre);
            FechaCreacion = ordenFas.FechaCarga.ToString();
            NombreChofer = ordenFas.NombreChofer;
            Observacion = ordenFas.Observacion;
            PatenteAcoplado = ordenFas.PatenteAcoplado;
            PatenteChasis = ordenFas.ChasisAcoplado;
            Pedido = ordenFas.NumeroPedido;
            RazonSocialTransporte = ordenFas.RazonSocialTransporte;
            FleteMOA = ordenFas.FleteMOA??false;
            ClienteComoRemitenteComercial = ordenFas.Reventa;
            TipoOrden = TipoOrdenes.FAS;
        }
    }
}