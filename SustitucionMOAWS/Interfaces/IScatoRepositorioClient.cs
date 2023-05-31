using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAWS.ScatoComandosWebService;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoRepositorioClient
    {
        RespuestaListado<Planta> ObtenerPlantas(string cuitDestino);
        RespuestaListado<Domicilio> ObtenerDomicilios(string cuitDestino);
        Respuesta<ChoferDto> ObtenerChoferPorCuil(string cuilChofer);
    }
}
