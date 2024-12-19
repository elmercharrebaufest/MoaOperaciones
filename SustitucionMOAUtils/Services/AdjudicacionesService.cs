using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class AdjudicacionesService : IAdjudicacionesService
    {
        private readonly IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA;
        private readonly ITablaSapService tablaSapService;
        private readonly ISolpService solpService;
        private readonly IRepositorio repositorio;

        public AdjudicacionesService(IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA,
                                     ITablaSapService tablaSapService,
                                     ISolpService solpService,
                                     IRepositorio repositorio)
        {
            this.obtenerOrdenesDeCompraParaSOLPConsumerMOA = obtenerOrdenesDeCompraParaSOLPConsumerMOA;
            this.tablaSapService = tablaSapService;
            this.solpService = solpService;
            this.repositorio = repositorio;
        }

        public List<AdjudicacionDto> ListarAdjudicaciones(int solpId)
        {
            string nroSolp = solpService.ObtenerNumeroSolp(solpId);
            var respuestaSAP = obtenerOrdenesDeCompraParaSOLPConsumerMOA.Request(nroSolp, "");
            var estados = tablaSapService.Listar(x => x.Tabla == TablasSap.EstadoOC).ToList();
            var listaResultado = respuestaSAP.Select(adjudicacion => new AdjudicacionDto()
            {
                Id = 0,
                Solp_Id = solpId,
                TipoPosicionCodigo = adjudicacion.Cabecera.Tipo,
                NumeroOrdenDeCompra = adjudicacion.Cabecera.OrdenDeCompra,
                FechaCreacion = adjudicacion.Cabecera.FechaCreacion,
                Proveedor = adjudicacion.Cabecera.RazonSocialProveedor,
                MonedaDescripcion = adjudicacion.Cabecera.Moneda,
                PrecioFinal = adjudicacion.Cabecera.MontoTotal,
                PrecioBruto = adjudicacion.Cabecera.MontoBruto,
                EstadoLiberacionCodigo = adjudicacion.Cabecera.EstadoLiberacionCodigo,
                EstadoLiberacionDetalle = estados.SingleOrDefault(a => a.CodigoSap == adjudicacion.Cabecera.EstadoLiberacionCodigo)?.Descripcion ?? "",

            }).OrderBy(fc => fc.FechaCreacion).ToList();

            return listaResultado;
        }

        public AdjudicacionDto ObtenerAdjudicacion(int adjudicacionId)
        {
            var adjudicar = repositorio.Obtener<Adjudicacion, AdjudicacionDto>(adjudicacion => adjudicacion.Id == adjudicacionId, adjudicacion => new AdjudicacionDto
            {
                Id = adjudicacion.Id,
                TipoPosicionCodigo = adjudicacion.Posiciones.FirstOrDefault().Posicion.TipoPosicion.Codigo,
                NumeroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra,
                PrecioFinal = adjudicacion.MontoTotal,
                TextoDeCabecera = adjudicacion.TextoDeCabecera,
                CondicionesDeEntrega = adjudicacion.CondicionesDeEntrega,
                CondicionesDePago = adjudicacion.CondicionesDePago,
                Garantias = adjudicacion.Garantias,
                AdjudicacionPosiciones = adjudicacion.Posiciones.Select(posicion => new AdjudicacionPosicionDto
                {
                    SolpPosicion_Id = posicion.SolpPosicion_Id,
                    Id = posicion.Id,
                    MaterialComprasCodigo = posicion.Posicion.MaterialSolp.Codigo,
                    Indice = posicion.Posicion.Indice,
                    Tarea = posicion.Posicion.Tarea,
                    TextoSuministro = posicion.Posicion.TextoSuministro,
                    Modelo = posicion.Posicion.Modelo,
                    Cantidad = adjudicacion.Posiciones.FirstOrDefault().Posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.Cantidad : 1,
                    PrecioUnidad = posicion.CotizacionPosicion.Precio,
                    MonedaId = posicion.CotizacionPosicion.Moneda_Id,
                    UnidadDescripcion = posicion.CotizacionPosicion.UnidadDeMedida.Descripcion,
                    MonedaDescripcion = posicion.CotizacionPosicion.Moneda.CodigoSap,
                    PrecioTotal = posicion.Cantidad * posicion.CotizacionPosicion.Precio,
                    FechaEntregaServicio = posicion.Posicion.FechaEntregaServicio,
                    PlazoDeOferta = posicion.Posicion.PlazoEntrega,
                    SubposicionesCompras = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(subpos =>
                                                new SolpSubposicionDto()
                                                {
                                                    Numero = subpos.SolpSubPosicion.Numero,
                                                    Tarea = subpos.SolpSubPosicion.Tarea,
                                                    CodigoSolp = subpos.SolpSubPosicion.ServicioSolp.CodigoSap,
                                                    Cantidad = subpos.Cantidad,
                                                    PrecioBruto = subpos.Precio,
                                                    UnidadComprasDescripcion = subpos.UnidadDeMedida.Descripcion,
                                                    MonedaCotizacionDescripcion = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Count() == 1
                                                    ? subpos.Moneda.Descripcion : "Error",
                                                    PrecioTotalSubPosicion = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Count() == 1 ?
                                                    (subpos.Cantidad.Value * subpos.Precio.Value) : 0,
                                                }).ToList(),
                }).ToList()

            });
            return adjudicar;
        }
    }
}
