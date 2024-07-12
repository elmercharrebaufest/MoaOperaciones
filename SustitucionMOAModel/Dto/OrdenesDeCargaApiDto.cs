
namespace SustitucionMOAModel.Dto
{
    public class OrdenesDeCargaApiDto : OrdenDeCargaNoResiduoApiDto
    {
        public string Contrato { get; set; }
        //public string EstadoDescripcion { get; set; }
        public string Pedido { get; set; }
        //public string RazonSocialCorredor { get; set; }
        //public bool Reventa { get; set; }


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
            TipoOrden = TipoOrdenes.FAS;
        }

        //public string RemitenteComercial
        //{
        //    get
        //    {
        //        if(ClienteComoRemitenteComercial && CUITDestino != CUITCliente && TipoOrden==TipoOrdenes.FASON)
        //        {
        //            return CUITCliente;
        //        }
        //        return null;
        //    }
        //}
        //public string PagadorFlete
        //{
        //    get
        //    {
        //        if (FleteMOA && TipoOrden == TipoOrdenes.FASON)
        //        {
        //            string cUIT_MOA = CUIT_MOA;

        //            return cUIT_MOA;
        //        }
        //        return CUITCliente;
        //    }
        //}

        //private readonly string CUIT_MOA = "30715118773";
    }
}
