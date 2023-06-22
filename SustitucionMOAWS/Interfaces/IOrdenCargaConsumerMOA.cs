using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using SustitucionMOAWS.WSRequests.OrdenCarga;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOrdenCargaConsumerMOA
    {
        ControlCargaResponseHandler ControlarCarga(ControlCargaRequest datosCarga);
        CrearOrdenResEnum CrearOrden(CrearOrdenRequest datosOrden, out string pedidoOutput, out string resultOutput);
        string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista);
        OrdenCargaEntreResponseHandler CrearEntrega(CrearEntregaRequest entregaReq, bool pedidoAnticipado = false);
        OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request);
        ControlEstadoResEnum GetOrdenCargaControlEstadoTransportista(string cuitTransportista);
        ModOrdenCargaResponseHandler AnularOrdenCarga(OrdenDeCarga orden);
        ModEntregaResponseHandler AnularEntregaOrdenCarga(string nroEntrega);
        ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaOrdenCargaSAP datosEntrega);
        bool VerificarContratoAbierto(string contrato);
        Result ObtenerContratoSAP(string numeroContrato, TipoContratoFAS tipoContrato = TipoContratoFAS.NORMAL);
        Result ObtenerContratoSAP(OrdenDeCarga orden, TipoContratoFAS tipoContrato = TipoContratoFAS.NORMAL);
    }
}
