using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCargaDto
    {
        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string DescripcionEstado { get; set; }
        public string ColorSemaforo { get; set; }
        public string Material { get; set; }
        public string Cliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string Corredor { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string Entrega { get; set; }
        public bool EsFacturaAnticipada { get; set; }
        public string Fecha { get; set; }
        public string PatenteChasis { get; set; }
    }

    public class OrdenDeCargaEditarDto
    {
        public OrdenDeCargaEditarDto(OrdenDeCarga orden)
        {
            Id = orden.Id;
            CUITCliente = orden.CUITCliente;
            NombreChofer = orden.NombreChofer;
            ApellidoChofer = orden.ApellidoChofer;
            CUITChofer = orden.CUITChofer;
            PatenteAcoplado = orden.PatenteAcoplado;
            ChasisAcoplado = orden.ChasisAcoplado;
            RazonSocialTransporte = orden.RazonSocialTransporte;
            CUITTransporte = orden.CUITTransporte;
            Producto_Id = orden.Producto_Id;
            Observacion = orden.Observacion;
            ContratoIngresado = orden.ContratoIngresado;
            Cantidad = orden.Cantidad;
            NumeroPedidoIngresado = string.IsNullOrEmpty(orden.NumeroPedidoIngresado) ? orden.NumeroPedido : orden.NumeroPedidoIngresado;
            ColorSemaforo = orden.Estado.ObtenerSemaforo();
            Cliente_Id = orden.Cliente_Id;
            Corredor_Id = orden.Corredor_Id;
            CUITCorredor = orden.CUITCorredor;
            CodigoCliente = orden.Cliente.CodigoProveedor;
            CodigoCorredor = orden.Corredor != null ? orden.Corredor.CodigoProveedor : "";

        }

        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string NombreChofer { get; set; }
        public string ApellidoChofer { get; set; }
        public string CUITChofer { get; set; }
        public string PatenteAcoplado { get; set; }
        public string ChasisAcoplado { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public int Producto_Id { get; set; }
        public string Observacion { get; set; }
        public string ContratoIngresado { get; set; }
        public int Cantidad { get; set; }
        public string NumeroPedidoIngresado { get; set; }
        public string ColorSemaforo { get; set; }
        public int Cliente_Id { get; set; }
        public int? Corredor_Id { get; set; }
        public string CUITCorredor { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoCorredor { get; set; }
    }

    public class OrdenDeCargaDetalleDto
    {
        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string RazonSocialCliente { get; set; }
        public string RazonSocialCorredor { get; set; }
        public string DescripcionEstado { get; set; }
        public string ColorSemaforo { get; set; }
        public string Chofer { get; set; }
        public string FechaCarga { get; set; }
        public string Transporte { get; set; }
        public int Cantidad { get; set; }
        public string Observacion { get; set; }
        public string ContratoSAP { get; set; }
        public string PedidoSAP { get; set; }
        public bool CorredorSeleccionado { get; set; }
        public string Corredor { get; set; }
        public bool TransporteExiste { get; set; }
        public string PatenteAcoplado { get; set; }
        public string ChasisAcoplado { get; set; }
        public bool AprobadoCredito { get; set; }
        public bool InformadaSAP { get; set; }
        public string FechaEntregaGenerada { get; set; }
        public string Producto { get; set; }
        public int Estado { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroEntrega { get; set; }
        public string ContratoIngresado { get; set; }
        public string Cliente { get; set; }
        public string DescripcionEstadoUsuarioFinal { get; set; }
        public string MensajeValidacionSAP { get; set; }
        public string PedidosRespuesta { get; set; }
        public bool ContratoSinCantidadPendiente { get; set; }
        public string DescripcionErrorInterno { get; set; }
        public string NumeroPedidoIngresado { get; set; }

        public override bool Equals(object obj)
        {
            return obj is OrdenDeCargaDetalleDto dto &&
                   Id == dto.Id &&
                   CUITCliente == dto.CUITCliente &&
                   RazonSocialCliente == dto.RazonSocialCliente &&
                   DescripcionEstado == dto.DescripcionEstado &&
                   ColorSemaforo == dto.ColorSemaforo &&
                   Chofer == dto.Chofer &&
                   FechaCarga == dto.FechaCarga &&
                   Transporte == dto.Transporte &&
                   Cantidad == dto.Cantidad &&
                   Observacion == dto.Observacion &&
                   ContratoSAP == dto.ContratoSAP &&
                   CorredorSeleccionado == dto.CorredorSeleccionado &&
                   Corredor == dto.Corredor &&
                   TransporteExiste == dto.TransporteExiste &&
                   PatenteAcoplado == dto.PatenteAcoplado &&
                   ChasisAcoplado == dto.ChasisAcoplado &&
                   AprobadoCredito == dto.AprobadoCredito &&
                   InformadaSAP == dto.InformadaSAP &&
                   FechaEntregaGenerada == dto.FechaEntregaGenerada &&
                   Producto == dto.Producto &&
                   Estado == dto.Estado &&
                   NumeroPedido == dto.NumeroPedido &&
                   NumeroEntrega == dto.NumeroEntrega &&
                   ContratoIngresado == dto.ContratoIngresado &&
                   Cliente == dto.Cliente &&
                   DescripcionEstadoUsuarioFinal == dto.DescripcionEstadoUsuarioFinal &&
                   MensajeValidacionSAP == dto.MensajeValidacionSAP;
        }

        public override int GetHashCode()
        {
            int hashCode = -527641486;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITCliente);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocialCliente);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DescripcionEstado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ColorSemaforo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Chofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FechaCarga);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Transporte);
            hashCode = hashCode * -1521134295 + Cantidad.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observacion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratoSAP);
            hashCode = hashCode * -1521134295 + CorredorSeleccionado.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Corredor);
            hashCode = hashCode * -1521134295 + TransporteExiste.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PatenteAcoplado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChasisAcoplado);
            hashCode = hashCode * -1521134295 + AprobadoCredito.GetHashCode();
            hashCode = hashCode * -1521134295 + InformadaSAP.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FechaEntregaGenerada);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Producto);
            hashCode = hashCode * -1521134295 + Estado.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroPedido);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroEntrega);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratoIngresado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Cliente);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DescripcionEstadoUsuarioFinal);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MensajeValidacionSAP);
            return hashCode;
        }
    }
}
