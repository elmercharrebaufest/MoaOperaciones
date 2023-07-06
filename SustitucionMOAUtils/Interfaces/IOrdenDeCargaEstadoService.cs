using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaEstadoService
    {
        void AccionesARealizar(OrdenDeCarga orden);
        void ActualizarEstado(OrdenDeCarga orden);
    }
}
