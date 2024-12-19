// Ignore Spelling: Utils Sustitucion

using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasSapService
    {
        SolpSAPDto ConvertirSOLPSAP(Solp solpActual);

        RespuestaCrearOrdenDeCompra CrearOrdenDeCompra(Adjudicacion AdjudicacionEntity, bool creadoAutomatico = false);
        ResultadoGenerico EditarOrdenDeCompra(AdjudicacionDto adjudicacion);

        CrearSolpConsumerMOAResponse CrearSolpSap(SolpSAPDto solpSap);
        ModificarSolpConsumerMOAResponse ModificarSolpSap(SolpSAPDto solpSap);
        ObtenerSolpSAPResponse ObtenerSolpSap(ObtenerSolpRequest obtenerSolpRequest);

        List<FuenteAprovisionamientoDto> ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro);

        List<ContratoSolp> ObtenerContratoMarco(string numeroContrato, string centro);

        List<TablaSapDto> ObtenerCentrosDeCostoSap();

        List<TablaSapDto> ObtenerCuentasSap();
        List<TablaSapDto> ObtenerOrdenesSap(string idOrder = "");

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<PosicionSolpSAP> posicionesSap);

        bool PosicionPendienteSap(PosicionSolpSAP position);

        OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC);

        AdjudicacionDto ObtenerOrdenDeCompraAdjudicacion(string nroOc);

        AdjudicacionDto ObtenerAdjudicacion(string nroOC);

        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp);

        List<TablaSapDto> ObtenerTablaSap(string tabla);

        List<TablaSapDto> ObtenerServiciosSap();

        List<Servicio> ObtenerServiciosSapRaw();
    }
}