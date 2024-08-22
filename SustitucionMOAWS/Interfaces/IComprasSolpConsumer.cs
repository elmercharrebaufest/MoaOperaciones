using System.Collections.Generic;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAWS.Interfaces
{
    public interface IObtenerCecoSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerCuentasSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerOrdenSolpConsumerMOA
    {
        object request(string idOrder = "");
    }

    public interface IObtenerServiciosSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerMaterialesSolpConsumerMOA
    {
        MaterialWSMOAResponse request(List<string> CentroCodigo, string NombreDeMaterial);
    }

    public interface IObtenerSolpConsumerMOA
    {
        ObtenerSolpSAPResponse Request(ObtenerSolpRequest req);
        ObtenerSolpSAPResponse RequestSolpWithNroAndDates(ObtenerSolpRequest req);
    }

    public interface IObtenerContratoSolpConsumerMOA
    {
        ContratoSolpWSMOAResponse Request(string numeroContrato, string centro);
    }

    public interface IObtenerTipoCambioConsumerMOA
    {
        ObtenerTipoCambioConsumerMOAResponse Request(string fecha, string monedaDestino, string monedaOrigen);
    }

    public interface IObtenerRegistroInfoConsumerMOA
    {
        List<RegistroInfoDto> ObtenerRegistroInfoConsumer(string material, string centro, string grupoDeCompras, string proveedor);
    }
    public interface IObtenerOrdenDeCompraConsumerMOA
    {
        OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC);
        AdjudicacionDto ObtenerOrdenDeCompraAdjudicacion(string nroOC);
        ResultBAPI_PO_GETDETAIL1 ObtenerOrdenDeCompraRFC(string nroOC);
    }
    public interface IObtenerProveedorConsumerMOA
    {
        ObtenerProveedorWSMOAResponse ObtenerProveedor(string codigoProveedor);
    }
    public interface IObtenerUnidadesDeMedidaAlternativasConsumerMOA
    {
        List<UnidadesDeMedida> Request(List<string> codigosMaterial);
    }
    public interface IObtenerPDFOrdenCompraConsumerMOA
    {
        byte[] Request(string nroOrdenCompra);
    }

    public interface IObtenerEntradaDeServicioPorNumeroConsumerMOA
    {
        EntradaServicioDto ObtenerEntradaServicio(string nroES);
        //BAPIESSR ObtenerEntradaServicio(string nroES);
    }
    public interface IObtenerAdjuntosSOLPEDConsumerMOA
    {
        byte[] ObtenerAdjuntosSolpConsumer(string idArhcivo, string nombreArchivo);
    }


}
