using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class OrdenesDeCargaApiDto
    {
        public long Id { get; set; }
        public string EstadoDescripcion { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaRetiro { get; set; }
        public int Cantidad { get; set; }
        public string PatenteChasis { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NombreChofer { get; set; }
        public string CUILChofer { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public string Observacion { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string CUITCorredor { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string CUITCliente { get; set; }
        public string TipoOrden { get; set; }
        public string DescripcionProducto { get; set; }
        public string Cliente { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadDescripcion { get; set; }
        public string CodigoProducto { get; set; }
        public string KmARecorrer { get; set; }
        public bool FleteMOA { get; set; }
        public bool Reventa { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Escalable { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public string RazonSocialDestino { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }
        public string DestinoMercaderia { get; set; }
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
            Reventa = ordenFason.Reventa;
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

        public static OrdenesDeCargaApiDto From(Entities.OrdenResiduos ordenResiduo)
        {
            
            return new OrdenesDeCargaApiDto
            {
                Id = ordenResiduo.Id,
                FechaCreacion = ordenResiduo.FechaCreacion.ToString(),
                //FechaRetiro = ordenResiduo.FechaRetiro.ToString(),
                //Cantidad = ordenResiduo.Cantidad,
                PatenteAcoplado = ordenResiduo.PatenteAcoplado,
                PatenteChasis = ordenResiduo.PatenteChasis,
                NombreChofer = ordenResiduo.ChoferNombre,
                CUILChofer = ordenResiduo.ChoferCuil,
                RazonSocialTransporte = ordenResiduo.TransporteRazonSocial,
                CUITTransporte = ordenResiduo.TransporteCuit,
                LocalidadId = ordenResiduo.LocalidadId,
                LocalidadDescripcion = ordenResiduo.Localidad.Nombre,
                Observacion = ordenResiduo.Observacion,
                Cliente = ordenResiduo.Cliente.RazonSocial,
                DescripcionProducto = new OrdenesDeCargaApiDto().ParseNombreProducto(ordenResiduo.Producto.Nombre),
                CUITCliente = ordenResiduo.Cliente.CUIT,
                //CUITCorredor = ordenResiduo.CorredorId != null ? ordenResiduo.Corredor.CUIT : null,
                CodigoProducto = ordenResiduo.Producto.CodigoSap,
                //KmARecorrer = ordenResiduo.DistanciaKm.ToString(),
                TipoOrden = TipoOrdenes.RESIDUOS,
                //FleteMOA = ordenResiduo.FleteMOA,
                //Reventa = ordenResiduo.Reventa,
                PlantaCodigo = ordenResiduo.PlantaCodigo,
                DomicilioTipo = ordenResiduo.DomicilioTipo,
                DomicilioOrden = ordenResiduo.DomicilioOrden,
                DomicilioDescr = ordenResiduo.DomicilioDescr,
                //Escalable = ordenResiduo.Escalable,
                //CUITDestinatario = ordenResiduo.CUITDestinatario,
                //RazonSocialDestinatario = ordenResiduo.RazonSocialDestinatario,
                //CUITDestino = ordenResiduo.CUITDestino,
                //RazonSocialDestino = ordenResiduo.RazonSocialDestino,
                //CUITIntermediarioFlete = ordenResiduo.IntermediarioFleteCuit,
                //RazonSocialIntermediarioFlete = ordenResiduo.IntermediarioFleteRazonSocial,
                //DestinoMercaderia = ordenResiduo.DestinoMercaderia,
            };
        }

        private string ParseNombreProducto(string nombreMaterial)
        {
            try
            {
                var split = nombreMaterial.Split('-');

                return split.LastOrDefault()?.Trim();
            }
            catch
            {
                return "";
            }
        }

        public string RemitenteComercial
        {
            get
            {
                if(Reventa && CUITDestino != CUITCliente && TipoOrden==TipoOrdenes.FASON)
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
    public static class TipoOrdenes
    {
        public static string FAS = "FAS";
        public static string FASON = "FASON";
        public static string RESIDUOS = "RESIDUOS";
    }
}
