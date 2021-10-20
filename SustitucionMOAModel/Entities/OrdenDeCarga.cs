using SustitucionMOAModel.Enums;
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

        public string ApellidoChofer { get; set; }

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

        public string Corredor { get; set; }

        public bool TransporteExiste { get; set; }

        public string PatenteAcoplado { get; set; }

        public string ChasisAcoplado { get; set; }

        public bool AprobadoCredito { get; set; }

        public bool InformadaSAP { get; set; }

        public DateTime? FechaEntregaGenerada { get; set; }

        public string NumeroEntrega { get; set; }

        public string NumeroPedido { get; set; }

        public string ContratosRespuesta { get; set; }
        public string NumeroPedidoIngresado { get; set; }
        public string CodigoVerificacionSap { get; set; }
        public string DescripcionCodigoVerificacionSap { get; set; }

        [InverseProperty("OrdenDeCarga")]
        public virtual ICollection<OrdenDeCargaCambiosHistorial> HistorialCambios { get; set; } = new List<OrdenDeCargaCambiosHistorial>();

        public void ActualizarEstado()
        {
            if (Estado != EstadoOrdenDeCarga.Entregada)
            {
                if (string.IsNullOrEmpty(ContratoSAP))
                {
                    Estado = EstadoOrdenDeCarga.Pendiente;
                }
                else
                {
                    if (InformadaSAP)
                    {
                        Estado = EstadoOrdenDeCarga.Confirmado;
                    }
                    if (!AprobadoCredito || !TransporteExiste)
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

        public override bool Equals(object obj)
        {
            return obj is OrdenDeCarga carga &&
                   Id == carga.Id &&
                   Cliente_Id == carga.Cliente_Id &&
                   EqualityComparer<Proveedor>.Default.Equals(Cliente, carga.Cliente) &&
                   FechaCarga == carga.FechaCarga &&
                   CUITCliente == carga.CUITCliente &&
                   NombreChofer == carga.NombreChofer &&
                   ApellidoChofer == carga.ApellidoChofer &&
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
                   Corredor == carga.Corredor &&
                   TransporteExiste == carga.TransporteExiste &&
                   PatenteAcoplado == carga.PatenteAcoplado &&
                   ChasisAcoplado == carga.ChasisAcoplado &&
                   AprobadoCredito == carga.AprobadoCredito &&
                   InformadaSAP == carga.InformadaSAP &&
                   FechaEntregaGenerada == carga.FechaEntregaGenerada &&
                   NumeroEntrega == carga.NumeroEntrega &&
                   NumeroPedido == carga.NumeroPedido &&
                   ContratosRespuesta == carga.ContratosRespuesta &&
                   NumeroPedidoIngresado == carga.NumeroPedidoIngresado &&
                   EqualityComparer<ICollection<OrdenDeCargaCambiosHistorial>>.Default.Equals(HistorialCambios, carga.HistorialCambios);
        }

        public override int GetHashCode()
        {
            int hashCode = 1559664579;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Cliente_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Proveedor>.Default.GetHashCode(Cliente);
            hashCode = hashCode * -1521134295 + FechaCarga.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUITCliente);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreChofer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ApellidoChofer);
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
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Corredor);
            hashCode = hashCode * -1521134295 + TransporteExiste.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PatenteAcoplado);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ChasisAcoplado);
            hashCode = hashCode * -1521134295 + AprobadoCredito.GetHashCode();
            hashCode = hashCode * -1521134295 + InformadaSAP.GetHashCode();
            hashCode = hashCode * -1521134295 + FechaEntregaGenerada.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroEntrega);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroPedido);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ContratosRespuesta);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NumeroPedidoIngresado);
            hashCode = hashCode * -1521134295 + EqualityComparer<ICollection<OrdenDeCargaCambiosHistorial>>.Default.GetHashCode(HistorialCambios);
            return hashCode;
        }
    }


}
