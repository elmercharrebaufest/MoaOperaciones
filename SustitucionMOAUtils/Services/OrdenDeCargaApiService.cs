using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaApiService : IOrdenDeCargaApiService
    {

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
            Log.ExternalAPIInfo($"Informado viaje de orden fason: {ingresosEgresosFasones.ToJson()}");
            if (orden == null)
                return new ResultadoGenerico { Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"No se encontro la orden fason numero: {ingresosEgresosFasones.FasonId}" } } };
            
            orden.Estado = EstadoOrdenDeCargaFason.Entregada;

            orden.PesadaTara = ingresosEgresosFasones.PesadaTara;
            orden.PesadaNeto = ingresosEgresosFasones.PesadaNeto;
            orden.FechaEgreso = ingresosEgresosFasones.FechaEgreso;
            orden.FechaIngreso = ingresosEgresosFasones.FechaIngreso;
            orden.NroRemito = ingresosEgresosFasones.NroRemito;
            orden.UniMedCant = ingresosEgresosFasones.UniMedCant;

            Log.ExternalAPIInfo($"Informado viaje de orden fason encontrada y con cambios: " +
                $"OrdenFasonId=${orden.Id} Pesada Tara={orden.PesadaTara} - Pesada Neto={orden.PesadaNeto} - Fecha Egreso={orden.FechaEgreso} - " +
                $"FechaIngreso={orden.FechaIngreso} - NroRemito={orden.NroRemito} - UniMedCant={orden.UniMedCant}");
            _repositorio.GuardarCambios();

            return new ResultadoGenerico();
        }

        public List<OrdenesDeCargaApiDto> ObtenerOrdenesFas(string patenteChasis, bool fason, bool fas)
        {
            var listaOrdenes = new List<OrdenesDeCargaApiDto>();
            
            var ordenesFas = _repositorio.Listar<OrdenDeCarga>(
                x => !_estadosNoPermitidosFas.Contains(x.Estado) &&
                (x.ChasisAcoplado == patenteChasis || patenteChasis == "" || patenteChasis == null)).ToList();

            foreach (var ordenFas in ordenesFas)
            {
                var ordenFasDto = new OrdenesDeCargaApiDto(ordenFas);
                listaOrdenes.Add(ordenFasDto);
            }
            return listaOrdenes;
        }

        public List<OrdenDeCargaFasonApiDto> ObtenerOrdenesFason(string patenteChasis)
        {
            var listaOrdenes = new List<OrdenDeCargaFasonApiDto>();

            var ordenesFason = _repositorio.Listar<OrdenDeCargaFason>(
                    x => x.Estado == EstadoOrdenDeCargaFason.Generada &&
                    (x.PatenteChasis == patenteChasis || patenteChasis == "" || patenteChasis == null)).ToList();
            
            foreach (var ordenFason in ordenesFason)
            {
                var ordenFasonDto = new OrdenDeCargaFasonApiDto(ordenFason);

                listaOrdenes.Add(ordenFasonDto);
            }

            return listaOrdenes;
        }
    }
}