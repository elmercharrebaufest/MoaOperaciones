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
        string CrearOrden(CrearOrdenRequest datosOrden, out string pedidoOutput);
        string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista);
        string CrearEntrega(CrearEntregaRequest entregaReq, out string mensaje);
        OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request);
        OrdenCargaControlEstado GetOrdenCargaControlEstadoTransportista(string cuitTransportista);
        ResultadoGenerico AnularOrdenCarga(OrdenDeCarga orden);
        ResultadoGenerico AnularEntregaOrdenCarga(string nroEntrega);
        ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaOrdenCargaSAP datosEntrega);
        bool VerificarContratoAbierto(string contrato);
    }
}
