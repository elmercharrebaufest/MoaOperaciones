using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAWS.Interfaces
{
    public interface IObtenerCecoSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerCuentasSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerOrdenSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerServiciosSolpConsumerMOA
    {
        object request();
    }

    public interface IObtenerSolpConsumerMOA
    {
        object request(ObtenerSolpRequest req);
    }
}
