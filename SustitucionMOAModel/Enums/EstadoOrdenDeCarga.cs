using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoOrdenDeCarga
    {
        Pendiente,
        Confirmado,
        PendienteAprobacionCredito,
        EntregaGenerada,
        Anulada,
        Entregada,
        Vencida
    }

    public static class EstadoOrdenDeCargaExtensions
    {
        public static string ObtenerSemaforo(this EstadoOrdenDeCarga me)
        {
            switch (me)
            {
                case EstadoOrdenDeCarga.Pendiente:
                case EstadoOrdenDeCarga.Vencida:
                    return "red";
                case EstadoOrdenDeCarga.Confirmado:
                case EstadoOrdenDeCarga.PendienteAprobacionCredito:
                    return "yellow";
                case EstadoOrdenDeCarga.EntregaGenerada:
                case EstadoOrdenDeCarga.Entregada:
                    return "green";
                default:
                    return "white";
            }
        }
    }
}
