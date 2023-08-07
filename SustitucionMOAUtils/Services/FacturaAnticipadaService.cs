using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Services
{
    public class FacturaAnticipadaService : IFacturaAnticipadaService
    {
        private readonly IOrdenCargaConsumerMOA _consumerOrdenCarga;
        private readonly IOrdenDeCargaEstadoService _ordenDeCargaEstadoService;
        protected readonly IRepositorio _repositorio;
        protected readonly IKgDisponiblesFasService _kgDisponiblesFasService;
        public FacturaAnticipadaService(
            IOrdenCargaConsumerMOA ordenDeCargaService,
            IRepositorio repositorio,
            IOrdenDeCargaEstadoService ordenDeCargaEstadoService,
            IKgDisponiblesFasService kgDisponiblesFasService
            )
        {
            _consumerOrdenCarga = ordenDeCargaService;
            _repositorio = repositorio;
            _ordenDeCargaEstadoService = ordenDeCargaEstadoService;
            _kgDisponiblesFasService = kgDisponiblesFasService;
        }
        public List<FacturaOrdenCarga> ObtenerFacturasDeContrato(OrdenDeCarga orden)
        {
            var contrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
            return ObtenerFacturasDeContrato(contrato);
        }
        public List<FacturaOrdenCarga> ObtenerFacturasDeContrato(string numeroContrato)
        {

            Log.Info($"Obtener facturas de contrato: {numeroContrato}");
            var contratoSAP = _consumerOrdenCarga.ObtenerContratoSAP(numeroContrato, TipoContratoFAS.Anticipado);
            Log.Info($"Obtener facturas de contrato Result: {numeroContrato}, {contratoSAP.ToJson()}");

            return ObtenerFacturasDeContrato(contratoSAP, false);
        }
        public void SeleccionarFactura(int ordenId, string facturaSeleccionada)
        {
            Log.Info($"Seleccionar factura para orden = {ordenId}; facturaSeleccionada = {facturaSeleccionada}");
            if (facturaSeleccionada == null || facturaSeleccionada == "undefined")
                throw new InfoCustomException("No se selecciono una factura.");

            var orden = _repositorio.Obtener<OrdenDeCarga>(ordenId);
            if (orden == null)
                throw new InfoCustomException("No se encuentra la orden.");

            var facturaSeleccionadaSAP = ObtenerFacturasDeContrato(orden)
                .Find(factura => factura.NumeroFactura == facturaSeleccionada);
            if (facturaSeleccionadaSAP == null)
                throw new InfoCustomException("No se selecciono una factura válida.");

            orden.NumeroFacturaSeleccionada = facturaSeleccionada;
            orden.NumeroPedido = facturaSeleccionadaSAP.NumeroPedido;
            orden.DescripcionErrorInterno = null;
            _ordenDeCargaEstadoService.ActualizarEstado(orden);

            _repositorio.GuardarCambios();
        }
        public bool OrdenConMultiplesFacturas(OrdenDeCarga orden)
        {
            var contrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
            return ObtenerFacturasDeContrato(contrato).Count > 1;
        }
        public bool OrdenConMultiplesFacturas(Result contrato)
        {
            return ObtenerFacturasDeContrato(contrato).Count > 1;
        }

        private List<FacturaOrdenCarga> ObtenerFacturasDeContrato(Result contrato, bool logger = true)
        {
            if (logger)
                Log.Info($"Obtener facturas de contrato: {contrato.ToJson()}");
            if (contrato == null)
                throw new InfoCustomException("No se encontró el contrato");
            var listaFacturas = contrato.Detalles
               .GroupBy(det => det.Pedido)
               .Select(grupoPedidos =>
               {
                   var listPedidos = grupoPedidos.ToList();
                   var pedidoPrincipal = _kgDisponiblesFasService.AuxObtenerDetallePedidoPrincipal(listPedidos);
                   if (pedidoPrincipal == null)
                       return null;

                   var kilosDisponiblesPedido = _kgDisponiblesFasService.ObtenerKgDisponiblesPedido(listPedidos, pedidoPrincipal);
                   return new FacturaOrdenCarga(pedidoPrincipal, kilosDisponiblesPedido);
               })
               .Where(factura=>factura!=null)
               .ToList();
            if (logger)
                Log.Info($"Obtener facturas de contrato result: {listaFacturas.ToJson()}");

            return listaFacturas;
        }
    }
}
