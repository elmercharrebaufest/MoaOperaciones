using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS
{
    public class ResponseConverter
    {
        private static readonly Dictionary<string, ControlCargaResEnum> RespuestasControlCargaDict = new Dictionary<string, ControlCargaResEnum>
        {
            { "CC-00", ControlCargaResEnum.OK },
            { "CC-01", ControlCargaResEnum.MasDeUnContratoVigente }, // Más de un contrato vigente para Cliente/Corredor
            { "CC-02", ControlCargaResEnum.TransportistaNoDadoDeAlta },
            { "CC-03", ControlCargaResEnum.VerificarPedido },
            { "CC-04", ControlCargaResEnum.VerificarCreditoDePedido },
            { "CC-05", ControlCargaResEnum.PedidoEntregadoCompletamente },
            { "CC-06", ControlCargaResEnum.CC06IdemCC01 }, // CC-06: 'Considerar como error CC-01'
            { "CC-07", ControlCargaResEnum.FaltaCargarKmsEnContrato },
            { "CC-08", ControlCargaResEnum.ClienteInhabilitadoEnSisa },
            { "CC-09", ControlCargaResEnum.CorredorInhabilitadoEnSisa },
            { "CC-10", ControlCargaResEnum.DestinoInhabilitadoEnSisa },
            { "CC-11", ControlCargaResEnum.DestinatarioInhabilitadoEnSisa }
        };

        private static readonly Dictionary<string, CrearOrdenResEnum> RespuestasCrearOrdenDict = new Dictionary<string, CrearOrdenResEnum>
        {
            //OV-00   'OK'
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'

            { "OV-00", CrearOrdenResEnum.PedidoCreado },
            { "OV-01", CrearOrdenResEnum.VerificarDatos },
            { "OV-02", CrearOrdenResEnum.VerificarCantidadPendiente },
            { "OV-03", CrearOrdenResEnum.PedidoCreadoVerificarCredito },
            //{ "OV-04", CrearOrdenResEnum.VerificarCreditoDePedido },
            { "OV-05", CrearOrdenResEnum.ContratoSinKg }
        };

        public static ControlEstadoResEnum GetOrdenCargaControlEstadoResponse(string response)
        {
            switch (response)
            {
                case "CE-01": return ControlEstadoResEnum.IngreseUnaSeleccion;
                case "CE-02": return ControlEstadoResEnum.PedidoNoEncontrado;
                case "CE-03": return ControlEstadoResEnum.VerificarCreditoPedido;
                case "CE-04": return ControlEstadoResEnum.PedidoEntregadoCompletamente;
                case "CE-05": return ControlEstadoResEnum.EntregaNoEncontrada;
                case "CE-06": return ControlEstadoResEnum.EntregaCompletada;
                case "CE-07": return ControlEstadoResEnum.TransportistaOK;
                case "CE-08": return ControlEstadoResEnum.TransportistaNoDadoDeAlta;
                default: throw new Exception("Respuesta no esperada en OrdenCargaConsumer.ControlEstado: " + response);
            }
        }

        public static ControlCargaResEnum GetOrdenCargaControlCargaResponse(string response)
        {
            if (RespuestasControlCargaDict.TryGetValue(response, out ControlCargaResEnum valor))
            {
                return valor;
            }
            else
            {
                throw new Exception("Respuesta no esperada en OrdenCargaConsumer.ControlCarga: " + response);
            }
        }

        public static string GetCodigoControlCarga(ControlCargaResEnum valor)
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

        public static CrearOrdenResEnum GetOrdenCargaCrearOrden(string response)
        {
            if (string.IsNullOrEmpty(response))
                return CrearOrdenResEnum.Vacia;

            if (RespuestasCrearOrdenDict.TryGetValue(response, out CrearOrdenResEnum valor))
            {
                return valor;
            }
            else
            {
                // TODO: Agregar log
                //Log.Info(mensaje: "Respuesta no esperada en OrdenCargaConsumer.CrearOrden: " + response);
                return CrearOrdenResEnum.NoEsperado;
            }
        }

        public static string GetCodigoCrearOrden(CrearOrdenResEnum valor)
        {
            var codigo = RespuestasCrearOrdenDict.FirstOrDefault(x => x.Value == valor).Key;
            if (codigo != null)
            {
                return codigo;
            }
            else
            {
                // TODO: Agregar log
                //Log.Info(mensaje: "Código no encontrado para respuesta CrearOrden  " + valor.ToString());
                return "";
            }
        }
    }
}
