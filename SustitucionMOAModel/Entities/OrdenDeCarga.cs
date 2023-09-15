using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCarga
    {
        [Key]
        public int Id { get; set; }

        public int Cliente_Id { get; set; }

        [ForeignKey("Cliente_Id")]
        public virtual Proveedor Cliente { get; set; }

        public DateTime FechaCarga { get; set; }

        public string CUITCliente { get; set; }

        public string NombreChofer { get; set; }

        public string CUITChofer { get; set; }

        public string CUITTransporte { get; set; }

        public string RazonSocialTransporte { get; set; }

        //public string Producto { get; set; }

        public int Producto_Id { get; set; }

        [ForeignKey("Producto_Id")]
        public virtual Material Producto { get; set; }

        public int Cantidad { get; set; }

        public string Observacion { get; set; }

        public EstadoOrdenDeCarga Estado { get; set; }

        public string ContratoIngresado { get; set; }

        public string ContratoSAP { get; set; }

        public bool CorredorSeleccionado { get; set; }

        public string CodigoCorredor { get; set; }

        public int? Corredor_Id { get; set; }

        [ForeignKey("Corredor_Id")]
        public virtual Proveedor Corredor { get; set; }

        public bool TransporteExiste { get; set; }

        public string PatenteAcoplado { get; set; }

        public string ChasisAcoplado { get; set; }

        public bool AprobadoCredito { get; set; }

        public bool InformadaSAP { get; set; }

        public DateTime? FechaEntregaGenerada { get; set; }

        public string NumeroEntrega { get; set; }

        public string NumeroPedido { get; set; }

        public string ContratosRespuesta { get; set; }
        public string PedidosRespuesta { get; set; }
        public string NumeroPedidoIngresado { get; set; }
        public string PedidoSAP { get; set; }
        public string CodigoVerificacionSap { get; set; }
        public string DescripcionCodigoVerificacionSap { get; set; }
        public bool Reventa { get; set; }

        public int UsuarioCreacion_Id { get; set; }
        [ForeignKey("UsuarioCreacion_Id")]
        public virtual Usuario UsuarioCreacion { get; set; }

        [InverseProperty("OrdenDeCarga")]
        public virtual ICollection<OrdenDeCargaCambiosHistorial> HistorialCambios { get; set; } = new List<OrdenDeCargaCambiosHistorial>();
        public bool ContratoSinCantidadPendiente { get; set; }
        public string DescripcionErrorInterno { get; set; }
        public string CUITCorredor { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public bool FechaVencimientoAmpliada { get; set; }

        public bool EdicionRechazada { get; set; }
        public string CUITDestino { get; set; }
        public string CUITDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroFacturaSeleccionada { get; set; }
        public TipoContratoFAS TipoContrato { get; set; }
        public bool Escalable { get; set; }
        public bool? DestinatarioExisteScato { get; set; }
        public bool? DestinoExisteScato { get; set; }


        public bool TieneMultiplesContratos
        {
            get { return !string.IsNullOrEmpty(ContratosRespuesta); }
        }

        public bool TieneCodigoSap(ControlCargaResEnum controlCargaRes)
        {
            return CodigoVerificacionSap == ResponseConverter.GetCodigoControlCarga(controlCargaRes);
        }
        public bool EsFacturaAnticipada
        {
            get
            {
                return TipoContrato == TipoContratoFAS.Anticipado;
            }
        }
        public bool SinSeleccionarFactura
        {
            get
            {
                return EsFacturaAnticipada && (string.IsNullOrEmpty(NumeroFactura) || string.IsNullOrEmpty(NumeroFacturaSeleccionada));
            }
        }

        public bool CuitTerceroExisteScato
        {
            get
            {
                var existeDestino = DestinoExisteScato ?? false;
                var existeDestinatario = DestinatarioExisteScato ?? false;

                return existeDestinatario && existeDestino;
            }
        }

        public string MsgCuitsTerceros
        {
            get {
                var msgDestinatario = DestinatarioExisteScato ?? false ? "" : "Destinatario";
                var msgDestino = DestinoExisteScato ?? false ? "" : "Destino";
                var slash = (!(DestinoExisteScato ?? false)  && !(DestinatarioExisteScato ?? false)) ? "/": "";
                return $"No se pudo generar la entrega. No existe {msgDestinatario}{slash}{msgDestino}.";
            }
        }

        /// <summary>
        /// Actualiza la Orden según su estado interno
        /// </summary>
        /// <returns>Log del cambio de estado</returns>
        public string ActualizarEstado()
        {
            var logCambioEstado = $"Actualizar estado Orden de carga {Id}. Estado inicial:{EstadoOrdenDeCargaExtensions.ToFriendlyString(Estado)}. " +
                $"CodigoVerificacionSap:{CodigoVerificacionSap}, ContratoSAP:{ContratoSAP}, ContratoSinCantidadPendiente:{ContratoSinCantidadPendiente}, " +
                $"NumeroPedido:{NumeroPedido}, InformadaSAP:{InformadaSAP}, TransporteExiste:{TransporteExiste}, AprobadoCredito:{AprobadoCredito}, FechaEntregaGenerada:{FechaEntregaGenerada}." +
                $"EsFacturaAnticipada:{EsFacturaAnticipada}, SinSeleccionarFactura:{SinSeleccionarFactura}";

            if (Estado != EstadoOrdenDeCarga.Entregada && Estado != EstadoOrdenDeCarga.ContratoVencido)
            {
                if (this.TieneCodigoSap(ControlCargaResEnum.MasDeUnContratoVigente) ||
                    this.TieneCodigoSap(ControlCargaResEnum.CC06IdemCC01))
                {
                    Estado = EstadoOrdenDeCarga.ErrorDeCarga;
                }
                else
                {
                    if (
                        ((string.IsNullOrEmpty(ContratoSAP) ||
                        ContratoSinCantidadPendiente ||
                        string.IsNullOrEmpty(NumeroPedido)) && Estado != EstadoOrdenDeCarga.SinEnviarASAP) ||
                        CodigoVerificacionSap == "CC-07"
                        )
                    {
                        Estado = EstadoOrdenDeCarga.Pendiente;
                    }
                    else
                    {
                        if (InformadaSAP)
                        {
                            Estado = EstadoOrdenDeCarga.Confirmado;
                        }

                        if (!TransporteExiste || (EsFacturaAnticipada && SinSeleccionarFactura))
                        {
                            Estado = EstadoOrdenDeCarga.Pendiente;
                        }

                        if (!AprobadoCredito)
                        {
                            Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
                        }
                        else
                        {
                            if (FechaEntregaGenerada != null)
                            {
                                Estado = EstadoOrdenDeCarga.EntregaGenerada;
                            }
                            else
                            {
                                Estado = EstadoOrdenDeCarga.EntregaPendiente;
                            }
                        }
                    }
                }
            }
            logCambioEstado += $" Estado final:{EstadoOrdenDeCargaExtensions.ToFriendlyString(Estado)}";
            return logCambioEstado;
        }

        public override bool Equals(object obj)
        {
            return obj is OrdenDeCarga carga &&
                   Id == carga.Id &&
                   Cliente_Id == carga.Cliente_Id &&
                   EqualityComparer<Proveedor>.Default.Equals(Cliente, carga.Cliente) &&
                   FechaCarga == carga.FechaCarga &&
                   CUITCliente == carga.CUITCliente &&
                   NombreChofer == carga.NombreChofer &&
                   CUITChofer == carga.CUITChofer &&
                   CUITTransporte == carga.CUITTransporte &&
                   RazonSocialTransporte == carga.RazonSocialTransporte &&
                   Producto_Id == carga.Producto_Id &&
                   EqualityComparer<Material>.Default.Equals(Producto, carga.Producto) &&
                   Cantidad == carga.Cantidad &&
                   Observacion == carga.Observacion &&
                   Estado == carga.Estado &&
                   ContratoIngresado == carga.ContratoIngresado &&
                   ContratoSAP == carga.ContratoSAP &&
                   CorredorSeleccionado == carga.CorredorSeleccionado &&
                   CodigoCorredor == carga.CodigoCorredor &&
                   Corredor_Id == carga.Corredor_Id &&
                   EqualityComparer<Proveedor>.Default.Equals(Corredor, carga.Corredor) &&
                   TransporteExiste == carga.TransporteExiste &&
                   PatenteAcoplado == carga.PatenteAcoplado &&
                   ChasisAcoplado == carga.ChasisAcoplado &&
                   AprobadoCredito == carga.AprobadoCredito &&
                   InformadaSAP == carga.InformadaSAP &&
                   FechaEntregaGenerada == carga.FechaEntregaGenerada &&
                   NumeroEntrega == carga.NumeroEntrega &&
                   NumeroPedido == carga.NumeroPedido &&
                   ContratosRespuesta == carga.ContratosRespuesta &&
                   PedidosRespuesta == carga.PedidosRespuesta &&
                   NumeroPedidoIngresado == carga.NumeroPedidoIngresado &&
                   PedidoSAP == carga.PedidoSAP &&
                   CodigoVerificacionSap == carga.CodigoVerificacionSap &&
                   DescripcionCodigoVerificacionSap == carga.DescripcionCodigoVerificacionSap &&
                   EqualityComparer<ICollection<OrdenDeCargaCambiosHistorial>>.Default.Equals(HistorialCambios, carga.HistorialCambios) &&
                   ContratoSinCantidadPendiente == carga.ContratoSinCantidadPendiente &&
                   DescripcionErrorInterno == carga.DescripcionErrorInterno && 
                   DestinatarioExisteScato == carga.DestinatarioExisteScato &&
                   DestinoExisteScato == carga.DestinoExisteScato && 
                   CUITDestinatario == carga.CUITDestinatario &&
                   CUITDestino == carga.CUITDestino &&
                   RazonSocialDestinatario == carga.RazonSocialDestinatario &&
                   RazonSocialDestino == carga.RazonSocialDestino &&
                   DomicilioTipo == carga.DomicilioTipo &&
                   DomicilioDescr == carga.DomicilioDescr &&
                   DomicilioOrden == carga.DomicilioOrden &&
                   PlantaCodigo == carga.PlantaCodigo;
        }

        public override int GetHashCode()
        {
            int hashCode = -1110409952;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Cliente_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Proveedor>.Default.GetHashCode(Cliente);
            hashCode = hashCode * -1521134295 + FechaCarga.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITCliente);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreChofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITChofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITTransporte);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocialTransporte);
            hashCode = hashCode * -1521134295 + Producto_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Material>.Default.GetHashCode(Producto);
            hashCode = hashCode * -1521134295 + Cantidad.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observacion);
            hashCode = hashCode * -1521134295 + Estado.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratoIngresado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratoSAP);
            hashCode = hashCode * -1521134295 + CorredorSeleccionado.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodigoCorredor);
            hashCode = hashCode * -1521134295 + Corredor_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Proveedor>.Default.GetHashCode(Corredor);
            hashCode = hashCode * -1521134295 + TransporteExiste.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PatenteAcoplado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChasisAcoplado);
            hashCode = hashCode * -1521134295 + AprobadoCredito.GetHashCode();
            hashCode = hashCode * -1521134295 + InformadaSAP.GetHashCode();
            hashCode = hashCode * -1521134295 + FechaEntregaGenerada.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroEntrega);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroPedido);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratosRespuesta);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PedidosRespuesta);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroPedidoIngresado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PedidoSAP);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodigoVerificacionSap);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DescripcionCodigoVerificacionSap);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICollection<OrdenDeCargaCambiosHistorial>>.Default.GetHashCode(HistorialCambios);
            hashCode = hashCode * -1521134295 + ContratoSinCantidadPendiente.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DescripcionErrorInterno);
            return hashCode;
        }

        public OrdenDeCargaEditarDto ToDto()
        {
            return new OrdenDeCargaEditarDto(this);
        }
        //public TipoContratoFAS TipoContratoFAS()
        //{
        //    return TipoContratoFASParser.Parse(TipoContrato);
        //}
    }
}
