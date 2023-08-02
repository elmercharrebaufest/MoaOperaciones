
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
            TipoOrden = "FAS";
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
            TipoOrden = ordenFason.FleteMOA ? "0" : "10";
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
    }
}
