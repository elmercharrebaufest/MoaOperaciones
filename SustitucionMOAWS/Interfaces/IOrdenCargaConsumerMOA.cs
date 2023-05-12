using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAWS.Enum.OrdenCargaConsumer;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using SustitucionMOAWS.WSRequests.OrdenCarga;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOrdenCargaConsumerMOA
    {
        ControlCargaResponseHandler ControlarCarga(ControlCargaRequest datosCarga);
        OrdenCargaCrearOrden CrearOrden(CrearOrdenRequest datosOrden, out string pedidoOutput, out string resultOutput);
        string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista);
        string CrearEntrega(CrearEntregaRequest entregaReq, out string mensaje, bool pedidoAnticipado = false);
        OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request);
        OrdenCargaControlEstado GetOrdenCargaControlEstadoTransportista(string cuitTransportista);
        ResultadoGenerico AnularOrdenCarga(OrdenDeCarga orden);
        ModEntregaResponseHandler AnularEntregaOrdenCarga(string nroEntrega);
        ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaOrdenCargaSAP datosEntrega);
        bool VerificarContratoAbierto(string contrato);
    }
}
