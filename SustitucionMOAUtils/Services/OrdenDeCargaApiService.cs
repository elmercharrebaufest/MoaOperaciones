using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaApiService : IOrdenDeCargaApiService
    {
        private readonly List<EstadoOrdenDeCargaFason> _estadosNoPermitidosFason;
        private readonly List<EstadoOrdenDeCarga> _estadosNoPermitidosFas;
        private readonly IRepositorio _repositorio;
        public OrdenDeCargaApiService(IRepositorio repositorio)
        {
            _repositorio = repositorio;
            _estadosNoPermitidosFas = new List<EstadoOrdenDeCarga> {
                EstadoOrdenDeCarga.SinEnviarASAP ,
                EstadoOrdenDeCarga.Anulada ,
                EstadoOrdenDeCarga.AnuladaPorVencimiento ,
                EstadoOrdenDeCarga.Entregada ,
            };
            _estadosNoPermitidosFason = new List<EstadoOrdenDeCargaFason> { EstadoOrdenDeCargaFason.Entregada, EstadoOrdenDeCargaFason.SinEstado };
        }

        public ResultadoGenerico InformarViajeOrdenesDeCargaFas(IngresosEgresosFas ingresosEgresosFas)
        {
            string entrega = "00" + ingresosEgresosFas.Entrega.TrimStart('0');
            var orden = _repositorio.Obtener<OrdenDeCarga>(x => x.NumeroEntrega == entrega);
            if (orden == null)
                return new ResultadoGenerico { Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"No se encontro la orden fas : {ingresosEgresosFas.Entrega}" } } };

            orden.Estado = EstadoOrdenDeCarga.Entregada;
            _repositorio.GuardarCambios();

            return new ResultadoGenerico();
        }

        public ResultadoGenerico InformarViajeOrdenesDeCargaFason(IngresosEgresosFasones ingresosEgresosFasones)
        {
            var orden = _repositorio.Obtener<OrdenDeCargaFason>(x => x.Id == ingresosEgresosFasones.FasonId);
            if (orden == null)
                return new ResultadoGenerico { Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"No se encontro la orden fason numero: {ingresosEgresosFasones.FasonId}" } } };

            orden.Estado = EstadoOrdenDeCargaFason.Entregada;

            orden.CantidadEntregada = ingresosEgresosFasones.Cantidad;
            orden.FechaEgreso = ingresosEgresosFasones.FechaEgreso;
            orden.FechaIngreso = ingresosEgresosFasones.FechaIngreso;
            orden.NroRemito = ingresosEgresosFasones.NroRemito;
            orden.UniMedCant = ingresosEgresosFasones.UniMedCant;

            _repositorio.GuardarCambios();

            return new ResultadoGenerico();
        }

        public List<OrdenesDeCargaApiDto> ObtenerOrdenes(string patenteChasis, bool fason, bool fas)
        {
            List<OrdenesDeCargaApiDto> listaOrdenes = new List<OrdenesDeCargaApiDto>();
            if (fas)
            {

                var ordenesFas = _repositorio.Listar<OrdenDeCarga>(
                    x => !_estadosNoPermitidosFas.Contains(x.Estado) &&
                    (x.ChasisAcoplado == patenteChasis || patenteChasis == "" || patenteChasis == null)).ToList();

                foreach (var ordenFas in ordenesFas)
                {
                    var ordenFasDto = new OrdenesDeCargaApiDto(ordenFas);
                    listaOrdenes.Add(ordenFasDto);
                }
            }
            if (fason)
            {
                var ordenesFason = _repositorio.Listar<OrdenDeCargaFason>(
                    x => !_estadosNoPermitidosFason.Contains(x.Estado) &&
                    (x.PatenteChasis == patenteChasis || patenteChasis == "" || patenteChasis == null)).ToList();
                foreach (var ordenFason in ordenesFason)
                {
                    var ordenFasonDto = new OrdenesDeCargaApiDto(ordenFason);

                    listaOrdenes.Add(ordenFasonDto);
                }
            }

            return listaOrdenes;
        }
    }
}
