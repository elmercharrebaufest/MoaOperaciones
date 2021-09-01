using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;

namespace SustitucionMOAWS.Interfaces
{
    public interface IListarPesificacionesConsumer
    {
        ListarPesificacionesWSMOAResponse Request(string proveedor);
    }
}
