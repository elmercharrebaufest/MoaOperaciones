using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAWS.Enum.OrdenCargaConsumer;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.Interfaces
{
    public interface IOrdenCargaConsumerMOA
    {
        ControlCargaResponseHandler ControlCargaRequest(string cliente, string contrato, string corredor, string cuit, string material, string pedido, string soloSisa);
        string CrearOrdenRequest(string cliente, string contrato, string corredor, decimal kilos, string material, string pedidoInput, string usuarioSAP, string forzarCreacion, out string pedidoOutput);
        string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista);
        string OrdenCargaEntregadaRequest(string documento, decimal kilos, string nombreConductor, string patenteAcoplado, string patenteChasis, string pedido, string tipoDocumento, string transportista, out string mensaje);
        OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request);
        OrdenCargaControlEstado GetOrdenCargaControlEstadoTransportista(string cuitTransportista);
        ResultadoGenerico AnularOrdenCarga(OrdenDeCarga orden);
        ResultadoGenerico AnularEntregaOrdenCarga(string nroEntrega);
        ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaOrdenCargaSAP datosEntrega);
        bool VerificarContratoAbierto(string contrato);
        string VericarEstadoSISA(string cuit);
    }
}
