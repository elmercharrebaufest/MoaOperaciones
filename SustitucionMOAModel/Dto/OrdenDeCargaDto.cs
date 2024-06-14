using SustitucionMOAModel.Entities;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Ent = SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCargaDto
    {
        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string DescripcionEstado { get; set; }
        public string DescripcionEstadoListado { get; set; }
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
        public bool NoEstaEnSAP { get; set; }
        public bool EstaSeleccionado { get; set; }
        public List<AutoCompleteDropdownElement> ordenes { get; set; }
        public bool EdicionRechazada { get; set; }
        public string CUITDestino { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Reventa { get; set; }
        public bool Escalable { get; set; }
        public TipoContratoFAS TipoContrato { get; set; }
        public string DestinoMercaderia { get; set; }
        public bool TienePatentesRepetidas { get; set; }
        public bool TieneConsultasRealizadas { get; set; }
        public bool FleteMOA { get; set; }
        public bool TienePatenteMultiplesAutorizaciones { get; set; }
        public OrdenDeCargaDto()
        {
            this.ordenes = new List<AutoCompleteDropdownElement> { };
        }
    }

    public class OrdenDeCargaEditarDto
    {
        public OrdenDeCargaEditarDto(Ent.OrdenDeCarga orden)
        {
            Id = orden.Id;
            CUITCliente = orden.CUITCliente;
            NombreChofer = orden.NombreChofer;
            CUITChofer = orden.CUITChofer;
            PatenteAcoplado = orden.PatenteAcoplado;
            ChasisAcoplado = orden.ChasisAcoplado;
            RazonSocialTransporte = orden.RazonSocialTransporte;
            CUITTransporte = orden.CUITTransporte;
            Producto_Id = orden.Producto_Id;
            Observacion = orden.Observacion;
            ContratoIngresado = orden.ContratoIngresado;
            ContratoSeleccionado = new ContratoOrdenFas(orden);
            Cantidad = orden.Cantidad;
            NumeroEntrega = orden.NumeroEntrega;
            NumeroPedidoIngresado = string.IsNullOrEmpty(orden.NumeroPedidoIngresado) ? orden.NumeroPedido : orden.NumeroPedidoIngresado;
            ColorSemaforo = orden.Estado.ObtenerSemaforo();
            Cliente_Id = orden.Cliente_Id;
            Corredor_Id = orden.Corredor_Id;
            CUITCorredor = orden.CUITCorredor;
            CodigoCliente = orden.Cliente != null ? orden.Cliente.CodigoProveedor : "";
            CodigoCorredor = orden.Corredor != null ? orden.Corredor.CodigoProveedor : "";
            Estado = (int)orden.Estado;
            CUITDestinatario = orden.CUITDestinatario;
            RazonSocialDestinatario = orden.RazonSocialDestinatario;
            CUITDestino = orden.CUITDestino;
            RazonSocialDestino = orden.RazonSocialDestino;
            PlantaCodigo = orden.PlantaCodigo;
            DomicilioTipo = orden.DomicilioTipo;
            DomicilioOrden = orden.DomicilioOrden;
            DomicilioDescr = orden.DomicilioDescr;
            CUITIntermediarioFlete = orden.CUITIntermediarioFlete;
            Reventa = orden.Reventa;
            Escalable = orden.Escalable;
            NumeroPedido = orden.NumeroPedido;
            DestinoMercaderia = orden.DestinoMercaderia;
        }

        public int Id { get; set; }
        public string CUITCliente { get; set; }
        public string NombreChofer { get; set; }
        public string CUITChofer { get; set; }
        public string PatenteAcoplado { get; set; }
        public string ChasisAcoplado { get; set; }
        public string RazonSocialTransporte { get; set; }
        public string CUITTransporte { get; set; }
        public int Producto_Id { get; set; }
        public string Observacion { get; set; }
        public string ContratoIngresado { get; set; }
        public ContratoOrdenFas ContratoSeleccionado { get; set; }
        public int Cantidad { get; set; }
        public string NumeroEntrega { get; set; }
        public string NumeroPedidoIngresado { get; set; }
        public string ColorSemaforo { get; set; }
        public int Cliente_Id { get; set; }
        public int? Corredor_Id { get; set; }
        public string CUITCorredor { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoCorredor { get; set; }
        public int Estado { get; set; }
        public string CUITDestino { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Reventa { get; set; }
        public bool Escalable { get; set; }
        public string NumeroPedido { get; set; }
        public string DestinoMercaderia { get; set; }
    }

    public class OrdenDeCargaHistorialDto
    {
        public OrdenDeCargaHistorialDto(Ent.OrdenDeCargaCambiosHistorial orden)
        {
            if (orden != null)
            {
                Id = orden.Id;
                OrdenDeCarga_Id = orden.OrdenDeCarga_Id;
                Antes = orden.Antes;
                Despues = orden.Despues;
                FechaCambio = orden.FechaCambio;
                Usuario_Id = orden.Usuario_Id;
                NombreColumnaCambio = orden.NombreColumnaCambio;
            }
        }
        public int Id { get; set; }
        public int OrdenDeCarga_Id { get; set; }
        public string Antes { get; set; }
        public string Despues { get; set; }
        public DateTime? FechaCambio { get; set; }
        public int Usuario_Id { get; set; }
        public string NombreColumnaCambio { get; set; }
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
        public string CUITChofer { get; set; }
        public string FechaCarga { get; set; }
        public string Transporte { get; set; }
        public int Cantidad { get; set; }
        public string Observacion { get; set; }
        public string ContratoSAP { get; set; }
        public ContratoOrdenFas ContratoSeleccionado { get; set; }
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
        public string ContratosRespuesta { get; set; }
        public bool ContratoSinCantidadPendiente { get; set; }
        public string DescripcionErrorInterno { get; set; }
        public string NumeroPedidoIngresado { get; set; }
        public bool EsOrdenVencida { get; set; }

        public bool ValidaSisaRuca { get; set; }
        public string CUITDestino { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string PlantaCodigo { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroFacturaSeleccionada { get; set; }
        public TipoContratoFAS TipoContrato { get; set; }
        //public string DomicilioTipo { get; set; }
        //public short DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public bool Reventa { get; set; }

        public IEnumerable<OrdenDeCargaCambiosHistorialDto> OrdenDeCargaCambiosHistorial { get; set; }
        public bool FechaVencimientoAmpliada { get; set; }
        public bool EdicionRechazada { get; set; }
        public bool Escalable { get; set; }
        public bool NecesitaVerificarCuitsTerceros { get; set; }
        public string DestinoMercaderia { get; set; }
        public List<int> OrdenesConPatentesRepetidas { get; set; }
        public bool FleteMOA { get; set; }
        public OrdenDeCargaDetalleDto() { }

        public OrdenDeCargaDetalleDto(Ent.OrdenDeCarga orden, List<OrdenDeCargaCambiosHistorialDto> ordenDeCargaCambiosHistorial, Proveedor cliente)
        {
            Id = orden.Id;
            CUITCliente = orden.CUITCliente;
            DescripcionEstado = orden.EdicionRechazada ? orden.Estado.ToFriendlyString() + "(Edición Rechazada)" : orden.Estado.ToFriendlyString();
            DescripcionEstadoUsuarioFinal = orden.EdicionRechazada ? orden.Estado.ToUserFriendlyString() + "(Edición Rechazada)" : orden.Estado.ToUserFriendlyString();
            ColorSemaforo = orden.Estado.ObtenerSemaforo();
            ContratoIngresado = orden.ContratoIngresado;
            NumeroPedidoIngresado = orden.NumeroPedidoIngresado;
            Cliente = orden.Cliente.CodigoProveedor;
            AprobadoCredito = orden.AprobadoCredito;
            Cantidad = orden.Cantidad;
            ChasisAcoplado = orden.ChasisAcoplado;
            Chofer = $"{orden.NombreChofer} ({orden.CUITChofer})";
            ContratoSAP = orden.ContratoSAP;
            PedidoSAP = string.IsNullOrEmpty(orden.PedidoSAP) ? "-" : orden.PedidoSAP;
            Corredor = orden.CodigoCorredor;
            RazonSocialCorredor = string.IsNullOrWhiteSpace(orden.Corredor?.RazonSocial) ? "-" : orden.Corredor?.RazonSocial;
            CorredorSeleccionado = orden.CorredorSeleccionado;
            Estado = (int)orden.Estado;
            FechaCarga = orden.FechaCarga.ToString("dd/MM/yyyy hh:mm");
            FechaEntregaGenerada = orden.FechaEntregaGenerada?.ToString("dd/MM/yyyy hh:mm");
            InformadaSAP = orden.InformadaSAP;
            Observacion = orden.Observacion;
            PatenteAcoplado = orden.PatenteAcoplado;
            RazonSocialCliente = cliente.RazonSocial;
            Transporte = $"{orden.RazonSocialTransporte} ({orden.CUITTransporte})";
            TransporteExiste = orden.TransporteExiste;
            Producto = orden.Producto.Nombre;
            PedidosRespuesta = string.IsNullOrEmpty(orden.PedidosRespuesta) ? "-" : orden.PedidosRespuesta;
            ContratosRespuesta = string.IsNullOrEmpty(orden.ContratosRespuesta) ? "-" : orden.ContratosRespuesta;
            NumeroEntrega = string.IsNullOrEmpty(orden.NumeroEntrega) ? "-" : orden.NumeroEntrega;
            NumeroPedido = string.IsNullOrEmpty(orden.NumeroPedido) ? "-" : orden.NumeroPedido;
            MensajeValidacionSAP = string.IsNullOrEmpty(orden.DescripcionCodigoVerificacionSap) ? "" : orden.DescripcionCodigoVerificacionSap;
            ContratoSinCantidadPendiente = orden.ContratoSinCantidadPendiente;
            DescripcionErrorInterno = string.IsNullOrEmpty(orden.DescripcionErrorInterno) ? "" : orden.DescripcionErrorInterno;
            OrdenDeCargaCambiosHistorial = ordenDeCargaCambiosHistorial;
            EsOrdenVencida = orden.FechaVencimiento < DateTime.Now.Date;
            FechaVencimientoAmpliada = orden.FechaVencimientoAmpliada;
            EdicionRechazada = orden.EdicionRechazada;
            ValidaSisaRuca = orden.Producto.ValidaSisaRuca;
            Reventa = orden.Reventa;
            CUITDestino = orden.CUITDestino;
            CUITDestinatario = orden.CUITDestinatario;
            RazonSocialDestino = orden.RazonSocialDestino;
            RazonSocialDestinatario = orden.RazonSocialDestinatario;
            CUITIntermediarioFlete = orden.CUITIntermediarioFlete;
            PlantaCodigo = orden.PlantaCodigo;
            DomicilioDescr = orden.DomicilioDescr;
            Escalable = orden.Escalable;
            NumeroFactura = orden.NumeroFactura;
            NumeroFacturaSeleccionada = orden.NumeroFacturaSeleccionada;
            TipoContrato = orden.TipoContrato;
            CUITChofer = orden.CUITChofer;
            NecesitaVerificarCuitsTerceros = orden.Producto.ValidaSisaRuca && !orden.CuitTerceroExisteScato;
            DestinoMercaderia = orden.DestinoMercaderia;
            FleteMOA = orden.FleteMOA ?? false;
        }

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
                   MensajeValidacionSAP == dto.MensajeValidacionSAP &&
                   NumeroFacturaSeleccionada == dto.NumeroFacturaSeleccionada &&
                   NumeroFactura == dto.NumeroFactura &&
                   NecesitaVerificarCuitsTerceros == dto.NecesitaVerificarCuitsTerceros &&
                   DestinoMercaderia == dto.DestinoMercaderia;
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

    public class OrdenDeCargaCambiosHistorialDto
    {
        public int Id { get; set; }
        public int OrdenDeCarga_Id { get; set; }
        public string NombreColumnaCambio { get; set; }
        public string FechaCambio { get; set; }
        public string Usuario { get; set; }
        public string Antes { get; set; }
        public string Despues { get; set; }
    }
}
