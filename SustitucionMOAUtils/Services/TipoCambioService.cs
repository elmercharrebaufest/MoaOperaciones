using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;

namespace SustitucionMOAUtils.Services
{
    public class TipoCambioService : ITipoCambioService
    {
        private readonly IRepositorio repositorio;
        private readonly IObtenerTipoCambioConsumerMOA obtenerTipoCambioConsumerMOA;

        public TipoCambioService(IRepositorio repositorio,
                                 IObtenerTipoCambioConsumerMOA obtenerTipoCambioConsumerMOA)
        {
            this.repositorio = repositorio;
            this.obtenerTipoCambioConsumerMOA = obtenerTipoCambioConsumerMOA;
        }

        public ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(int MonedaOrigen_Id, int MonedaDestino_Id, DateTime Fecha)
        {
            var origen = repositorio.Obtener<TablaSap>(MonedaOrigen_Id);
            var destino = repositorio.Obtener<TablaSap>(MonedaDestino_Id);
            return ObtenerTipoCambio(origen.Codigo, destino.Codigo, Fecha.ToString("yyyy-MM-dd"));
        }

        public ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(string origen, string destino, string fecha)
        {
            return obtenerTipoCambioConsumerMOA.Request(fecha, destino, origen);
        }
    }
}
