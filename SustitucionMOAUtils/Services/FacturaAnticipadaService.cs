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

namespace SustitucionMOAUtils.Services
{
    public class FacturaAnticipadaService : IFacturaAnticipadaService
    {
        private readonly IOrdenCargaConsumerMOA _consumerOrdenCarga;
        private readonly IOrdenDeCargaEstadoService _ordenDeCargaEstadoService;
        protected readonly IRepositorio _repositorio;
        public FacturaAnticipadaService(IOrdenCargaConsumerMOA ordenDeCargaService, IRepositorio repositorio, IOrdenDeCargaEstadoService ordenDeCargaEstadoService)
        {
            _consumerOrdenCarga = ordenDeCargaService;
            _repositorio = repositorio;
            _ordenDeCargaEstadoService = ordenDeCargaEstadoService;
        }
        public List<string> ObtenerFacturasDeContrato(OrdenDeCarga orden)
        {
            return ObtenerFacturasDeContrato(string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP);
        }
        public List<string> ObtenerFacturasDeContrato(string numeroContrato)
        {

            Log.Info($"Obtener facturas de contrato: {numeroContrato}");
            var contratoSAP = _consumerOrdenCarga.ObtenerContratoSAP(numeroContrato, TipoContratoFAS.ANTICIPADO);
            Log.Info($"Obtener facturas de contrato Result: {numeroContrato}, {contratoSAP.ToJson()}");

            return ObtenerFacturasDeContrato(contratoSAP, false);
        }
        public List<string> ObtenerFacturasDeContrato(Result contrato, bool logger = true)
        {
            if (logger)
                Log.Info($"Obtener facturas de contrato: {contrato.ToJson()}");
            if (contrato == null)
                throw new InfoCustomException("No se encontró el contrato");
            var listaFacturas = contrato.Detalles
               .Where(det => !string.IsNullOrEmpty(det.FacturaLegal))
               .Select(det => det.FacturaLegal)
               .ToList();
            if (logger)
                Log.Info($"Obtener facturas de contrato result: {listaFacturas.ToJson()}");

            return listaFacturas;
        }
        public void SeleccionarFactura(int ordenId, string facturaSeleccionada)
        {
            Log.Info($"Seleccionar factura para orden = {ordenId}; facturaSeleccionada = {facturaSeleccionada}");
            var orden = _repositorio.Obtener<OrdenDeCarga>(ordenId);
            if (orden == null)
                throw new InfoCustomException("No se encuentra la orden.");
            var facturasDisponibles = ObtenerFacturasDeContrato(orden);

            if (!facturasDisponibles.Contains(facturaSeleccionada))
                throw new InfoCustomException("No se selecciono una factura válida.");

            orden.NumeroFacturaSeleccionada = facturaSeleccionada;
            _ordenDeCargaEstadoService.ActualizarEstado(orden);
            _repositorio.GuardarCambios();

        }
        public bool OrdenConMultiplesFacturas(OrdenDeCarga orden)
        {
            var contrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
            return ObtenerFacturasDeContrato(contrato).Count != 1;
        }
        public bool OrdenConMultiplesFacturas(Result contrato)
        {
            return ObtenerFacturasDeContrato(contrato).Count != 1;
        }
    }
}
