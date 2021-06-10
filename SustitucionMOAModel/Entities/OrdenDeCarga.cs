using SustitucionMOAModel.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class OrdenDeCarga
    {
        [Key]
        public int Id { get; set; }

        public int Cliente_Id { get; set; }


        public virtual Proveedor Cliente { get; set;  }

        public DateTime FechaCarga { get; set; }

        public string CUITCliente { get; set; }

        public string NombreChofer { get; set; }

        public string ApellidoChofer { get; set; }

        public string CUITChofer { get; set; }

        public string CUITTransporte { get; set; }

        public string RazonSocialTransporte { get; set; }

        public string Producto { get; set; }

        public int Cantidad { get; set; }

        public string Observacion { get; set; }

        public EstadoOrdenDeCarga Estado { get; set; }

        public string ContratoSAP { get; set; }

        public bool CorredorSeleccionado { get; set; }

        public string Corredor { get; set; }

        public bool TransporteExiste { get; set; }

        public string PatenteAcoplado { get; set; }

        public string ChasisAcoplado { get; set; }

        public bool AprobadoCredito { get; set; }

        public bool InformadaSAP { get; set; }

        public DateTime? FechaEntregaGenerada { get; set; }

        public string NumeroPedido { get; set; }


        public void ActualizarEstado ()
        {
            if(string.IsNullOrEmpty(ContratoSAP) || !TransporteExiste || !CorredorSeleccionado)
            {
                Estado = EstadoOrdenDeCarga.Pendiente;
            }
            else
            {
                if (InformadaSAP)
                {
                    Estado = EstadoOrdenDeCarga.Confirmado;
                }
                else
                { 
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
    }
}
