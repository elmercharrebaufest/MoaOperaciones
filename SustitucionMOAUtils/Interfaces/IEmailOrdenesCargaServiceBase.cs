namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailOrdenesCargaServiceBase
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial);
        void EnviarMailAltaTempranaCuit(string cuit, string razonSocial);
    }
}
