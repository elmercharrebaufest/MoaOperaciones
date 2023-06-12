using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;

namespace SustitucionMOAWS.Interfaces
{
    public interface IScatoRepositorioClient
    {
        RespuestaListado<Planta> ObtenerPlantas(string cuitDestino);
        RespuestaListado<Domicilio> ObtenerDomicilios(string cuitDestino);
        ObtenerProveedorPorCuilResponse ObtenerProveedorPorCuil(string cuil);
        Respuesta<Chofer> ObtenerChoferPorCuil(string cuilChofer);
    }
}
