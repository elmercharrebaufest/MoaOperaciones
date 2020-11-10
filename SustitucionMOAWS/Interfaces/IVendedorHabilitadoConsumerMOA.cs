using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;

namespace SustitucionMOAWS.WSConsumers
{
    public interface IVendedorHabilitadoConsumerMOA
    {
        VendedorHabilitadoWSMOAResponse Request(string cuit, string sociedad, string usuario);
    }
}