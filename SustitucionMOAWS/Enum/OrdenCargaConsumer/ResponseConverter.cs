using System;

namespace SustitucionMOAWS.Enum.OrdenCargaConsumer
{
    internal class ResponseConverter
    {
        internal static OrdenCargaControlEstado GetOrdenCargaControlEstadoResponse(string response)
        {
            switch (response)
            {
                case "CE-01": return OrdenCargaControlEstado.IngreseUnaSeleccion;
                case "CE-02": return OrdenCargaControlEstado.PedidoNoEncontrado;
                case "CE-03": return OrdenCargaControlEstado.VerificarCreditoPedido;
                case "CE-04": return OrdenCargaControlEstado.PedidoEntregadoCompletamente;
                case "CE-05": return OrdenCargaControlEstado.EntregaNoEncontrada;
                case "CE-06": return OrdenCargaControlEstado.EntregaCompletada;
                case "CE-07": return OrdenCargaControlEstado.TransportistaOK;
                case "CE-08": return OrdenCargaControlEstado.TransportistaNoDadoDeAlta;
                default: throw new Exception("Respuesta no esperada en OrdenCargaConsumer: " + response);
            }
        }
    }
}
