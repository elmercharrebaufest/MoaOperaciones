using SustitucionMOAModel.Models.WSMapMOA.Compras;
using System;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ITipoCambioService
    {
        ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(int MonedaOrigen_Id, int MonedaDestino_Id, DateTime Fecha);

        ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(string origen, string destino, string fecha);
    }
}
