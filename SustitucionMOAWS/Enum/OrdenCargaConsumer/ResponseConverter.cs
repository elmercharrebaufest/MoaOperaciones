using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.Enum.OrdenCargaConsumer
{
    internal class ResponseConverter
    {
        private static readonly Dictionary<string, OrdenCargaControlCarga> RespuestasControlCargaDict = new Dictionary<string, OrdenCargaControlCarga>
        {
            { "CC-00", OrdenCargaControlCarga.OK },
            { "CC-01", OrdenCargaControlCarga.MasDeUnContratoVigente },
            { "CC-02", OrdenCargaControlCarga.TransportistaNoDadoDeAlta },
            { "CC-03", OrdenCargaControlCarga.VerificarPedido },
            { "CC-04", OrdenCargaControlCarga.VerificarCreditoDePedido },
            { "CC-05", OrdenCargaControlCarga.PedidoEntregadoCompletamente },
            { "CC-06", OrdenCargaControlCarga.CC06IdemCC01 }, // CC-06: 'Considerar como error CC-01'
            { "CC-07", OrdenCargaControlCarga.FaltaCargarKmsEnContrato },
            { "CC-08", OrdenCargaControlCarga.ClienteInhabilitadoEnSisa },
            { "CC-09", OrdenCargaControlCarga.CorredorInhabilitadoEnSisa },
            { "CC-10", OrdenCargaControlCarga.DestinoInhabilitadoEnSisa },
            { "CC-11", OrdenCargaControlCarga.DestinatarioInhabilitadoEnSisa }
        };

        private static readonly Dictionary<string, OrdenCargaCrearOrden> RespuestasCrearOrdenDict = new Dictionary<string, OrdenCargaCrearOrden>
        {
            { "OV-00", OrdenCargaCrearOrden.PedidoCreado },
            { "OV-01", OrdenCargaCrearOrden.VerificarDatos },
            { "OV-02", OrdenCargaCrearOrden.VerificarCantidadPendiente },
            { "OV-03", OrdenCargaCrearOrden.PedidoCreadoVerificarCredito },
            //{ "OV-04", OrdenCargaCrearOrden.VerificarCreditoDePedido },
            { "OV-05", OrdenCargaCrearOrden.ContratoSinKg }
        };

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
                default: throw new Exception("Respuesta no esperada en OrdenCargaConsumer.ControlEstado: " + response);
            }
        }

        internal static OrdenCargaControlCarga GetOrdenCargaControlCargaResponse(string response)
        {
            if (RespuestasControlCargaDict.TryGetValue(response, out OrdenCargaControlCarga valor))
            {
                return valor;
            }
            else
            {
                throw new Exception("Respuesta no esperada en OrdenCargaConsumer.ControlCarga: " + response);
            }
        }

        internal static string GetCodigoControlCarga(OrdenCargaControlCarga valor)
        {
            var codigo = RespuestasControlCargaDict.FirstOrDefault(x => x.Value == valor).Key;
            if (codigo != null)
            {
                return codigo;
            }
            else
            {
                throw new Exception("Código no encontrado para respuesta ControlCarga " + valor.ToString());
            }
        }

        internal static OrdenCargaCrearOrden GetOrdenCargaCrearOrden(string response)
        {
            if (RespuestasCrearOrdenDict.TryGetValue(response, out OrdenCargaCrearOrden valor))
            {
                return valor;
            }
            else
            {
                throw new Exception("Respuesta no esperada en OrdenCargaConsumer.CrearOrden: " + response);
            }
        }

        internal static string GetCodigoCrearOrden(OrdenCargaCrearOrden valor)
        {
            var codigo = RespuestasCrearOrdenDict.FirstOrDefault(x => x.Value == valor).Key;
            if (codigo != null)
            {
                return codigo;
            }
            else
            {
                throw new Exception("Código no encontrado para respuesta CrearOrden " + valor.ToString());
            }
        }

    }
}
