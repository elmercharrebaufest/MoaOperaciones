using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAWS.Enum.OrdenCargaConsumer;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOrdenCargaConsumerMOA
    {
        ControlCargaResponseHandler ControlCargaRequest(string cliente, string contrato, string corredor, string cuit, string material, string pedido, string soloSisa);
        string CrearOrdenRequest(string cliente, string contrato, string corredor, decimal kilos, string material, string pedidoInput, string usuarioSAP, string validaKg, string cuitDestino, string cuitDestinatario, string razonSocialDestino, string razonSocialDestinatario, bool reventa, out string pedidoOutput);
        string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista);
        string OrdenCargaEntregadaRequest(string documento, decimal kilos, string nombreConductor, string patenteAcoplado, string patenteChasis, string pedido, string tipoDocumento, string transportista, string cuitDestinatario, string razonSocialDestinatario, string cuitDestino, string razonSocialDestino, bool reventa, string transportistaReal, out string mensaje);
        OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request);
        OrdenCargaControlEstado GetOrdenCargaControlEstadoTransportista(string cuitTransportista);
        ResultadoGenerico AnularOrdenCarga(OrdenDeCarga orden);
        ResultadoGenerico AnularEntregaOrdenCarga(string nroEntrega);
        ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaOrdenCargaSAP datosEntrega);
        bool VerificarContratoAbierto(string contrato);
    }
}
