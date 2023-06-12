using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaApiService : IOrdenDeCargaApiService
    {
        private readonly IRepositorio _repositorio;
        public OrdenDeCargaApiService(IRepositorio repositorio)
        {
            _repositorio = repositorio;
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

        public List<OrdendesDeCargaApiDto> ObtenerOrdenes(string patenteChasis)
        {
            var ordenesFas = _repositorio.Listar<OrdenDeCarga>(x =>
            x.Estado != EstadoOrdenDeCarga.SinEnviarASAP &&
            x.Estado != EstadoOrdenDeCarga.Anulada &&
            x.Estado != EstadoOrdenDeCarga.AnuladaPorVencimiento &&
            x.Estado != EstadoOrdenDeCarga.Entregada && 
            (x.ChasisAcoplado == patenteChasis || patenteChasis == "" || patenteChasis == null)
            ).ToList();
            var ordenesFason = _repositorio.Listar<OrdenDeCargaFason>(x =>
            x.Estado != EstadoOrdenDeCargaFason.Entregada &&
            x.Estado != EstadoOrdenDeCargaFason.SinEstado &&
            (x.PatenteChasis == patenteChasis || patenteChasis == "" || patenteChasis == null)
            ).ToList();
            List<OrdendesDeCargaApiDto> listaOrdenes = new List<OrdendesDeCargaApiDto>();

            foreach (var ordenFas in ordenesFas)
            {
                var ordenFasDto = new OrdendesDeCargaApiDto();
                ordenFasDto.Id = ordenFas.Id;
                ordenFasDto.NombreChofer = ordenFas.NombreChofer;
                ordenFasDto.FechaCreacion = ordenFas.FechaCarga.ToString();
                ordenFasDto.CUITCliente = ordenFas.CUITCliente;
                ordenFasDto.CUILChofer = ordenFas.CUITChofer;
                ordenFasDto.CUITTransporte = ordenFas.CUITTransporte;
                ordenFasDto.RazonSocialTransporte = ordenFas.RazonSocialTransporte;
                ordenFasDto.Cantidad = ordenFas.Cantidad;
                ordenFasDto.Contrato = ordenFas.ContratoIngresado;
                ordenFasDto.Observacion = ordenFas.Observacion;
                ordenFasDto.PatenteAcoplado = ordenFas.PatenteAcoplado;
                ordenFasDto.PatenteChasis = ordenFas.ChasisAcoplado;
                ordenFasDto.Pedido = ordenFas.NumeroPedido;
                ordenFasDto.DescripcionProducto = ordenFas.Producto.Nombre;
                ordenFasDto.TipoOrden = "OrdenCargaFas";
                listaOrdenes.Add(ordenFasDto);

            }

            foreach (var ordenFason in ordenesFason)
            {
                var ordenFasonDto = new OrdendesDeCargaApiDto();
                ordenFasonDto.Id = ordenFason.Id;
                ordenFasonDto.FechaCreacion = ordenFason.FechaCreacion.ToString();
                ordenFasonDto.FechaRetiro = ordenFason.FechaRetiro.ToString();
                ordenFasonDto.Cantidad = ordenFason.Cantidad;
                ordenFasonDto.PatenteAcoplado = ordenFason.PatenteAcoplado;
                ordenFasonDto.PatenteChasis = ordenFason.PatenteChasis;
                ordenFasonDto.NombreChofer = ordenFason.NombreChofer;
                ordenFasonDto.CUILChofer = ordenFason.CUILChofer;
                ordenFasonDto.RazonSocialTransporte = ordenFason.RazonSocialTransporte;
                ordenFasonDto.CUITTransporte = ordenFason.CUITTransporte;
                ordenFasonDto.LocalidadId = ordenFason.LocalidadId;
                ordenFasonDto.LocalidadDescripcion = ordenFason.LocalidadDescripcion;
                ordenFasonDto.Observacion = ordenFason.Observacion;
                ordenFasonDto.Cliente = ordenFason.Cliente.RazonSocial;
                ordenFasonDto.DescripcionProducto = ordenFason.Producto.Nombre;
                ordenFasonDto.TipoOrden = "OrdenCargaFason";
                listaOrdenes.Add(ordenFasonDto);

            }

            return listaOrdenes;
        }
    }
}
