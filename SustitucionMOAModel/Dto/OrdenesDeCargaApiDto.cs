using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class OrdenesDeCargaApiDto : OrdenDeCargaApiDtoBase
    {
        public int Cantidad { get; set; }
        public bool ClienteComoRemitenteComercial { get; set; }
        public string Contrato { get; set; }
        public string CUITCorredor { get; set; }
        public string CUITDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string DestinoMercaderia { get; set; }
        public bool Escalable { get; set; }
        public string EstadoDescripcion { get; set; }
        public string FechaRetiro { get; set; }
        public bool FleteMOA { get; set; }
        public string KmARecorrer { get; set; }
        public string Pedido { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public bool Reventa { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }


        public OrdenesDeCargaApiDto() { }

        public OrdenesDeCargaApiDto(Entities.OrdenDeCarga ordenFas)
        {

            Id = ordenFas.Id;
            NombreChofer = ordenFas.NombreChofer;
            FechaCreacion = ordenFas.FechaCarga.ToString();
            CUITCliente = ordenFas.CUITCliente;
            CUITCorredor = ordenFas.CUITCorredor;
            CUILChofer = ordenFas.CUITChofer;
            CUITTransporte = ordenFas.CUITTransporte;
            RazonSocialTransporte = ordenFas.RazonSocialTransporte;
            Cantidad = ordenFas.Cantidad;
            Contrato = ordenFas.ContratoIngresado;
            Observacion = ordenFas.Observacion;
            PatenteAcoplado = ordenFas.PatenteAcoplado;
            PatenteChasis = ordenFas.ChasisAcoplado;
            Pedido = ordenFas.NumeroPedido;
            DescripcionProducto = ParseNombreProducto(ordenFas.Producto.Nombre);
            CodigoProducto = ordenFas.Producto.CodigoSap;
            TipoOrden = TipoOrdenes.FAS;
        }
        public OrdenesDeCargaApiDto(Entities.OrdenDeCargaFason ordenFason)
        {
            Id = ordenFason.Id;
            FechaCreacion = ordenFason.FechaCreacion.ToString();
            FechaRetiro = ordenFason.FechaRetiro.ToString();
            Cantidad = ordenFason.Cantidad;
            PatenteAcoplado = ordenFason.PatenteAcoplado;
            PatenteChasis = ordenFason.PatenteChasis;
            NombreChofer = ordenFason.NombreChofer;
            CUILChofer = ordenFason.CUILChofer;
            RazonSocialTransporte = ordenFason.RazonSocialTransporte;
            CUITTransporte = ordenFason.CUITTransporte;
            LocalidadId = ordenFason.LocalidadId;
            LocalidadDescripcion = ordenFason.LocalidadDescripcion;
            Observacion = ordenFason.Observacion;
            Cliente = ordenFason.Cliente.RazonSocial;
            DescripcionProducto = ParseNombreProducto(ordenFason.Producto.Nombre);
            CUITCliente = ordenFason.Cliente.CUIT;
            CUITCorredor = ordenFason.CorredorId != null ? ordenFason.Corredor.CUIT : null;
            CodigoProducto = ordenFason.Producto.CodigoSap;
            KmARecorrer = ordenFason.KmARecorrer;
            TipoOrden = TipoOrdenes.FASON;
            FleteMOA = ordenFason.FleteMOA;
            ClienteComoRemitenteComercial = ordenFason.ClienteComoRemitenteComercial;
            PlantaCodigo = ordenFason.PlantaCodigo;
            DomicilioTipo = ordenFason.DomicilioTipo;
            DomicilioOrden = ordenFason.DomicilioOrden;
            DomicilioDescr = ordenFason.DomicilioDescr;
            Escalable = ordenFason.Escalable;
            CUITDestinatario = ordenFason.CUITDestinatario;
            RazonSocialDestinatario = ordenFason.RazonSocialDestinatario;
            CUITDestino = ordenFason.CUITDestino;
            RazonSocialDestino = ordenFason.RazonSocialDestino;
            CUITIntermediarioFlete = ordenFason.CUITIntermediarioFlete;
            RazonSocialIntermediarioFlete = ordenFason.RazonSocialIntermediarioFlete;
            DestinoMercaderia = ordenFason.DestinoMercaderia;
        }

        public string RemitenteComercial
        {
            get
            {
                if(ClienteComoRemitenteComercial && CUITDestino != CUITCliente && TipoOrden==TipoOrdenes.FASON)
                {
                    return CUITCliente;
                }
                return null;
            }
        }
        public string PagadorFlete
        {
            get
            {
                if (FleteMOA && TipoOrden == TipoOrdenes.FASON)
                {
                    string cUIT_MOA = CUIT_MOA;

                    return cUIT_MOA;
                }
                return CUITCliente;
            }
        }

        private readonly string CUIT_MOA = "30715118773";
    }
}
