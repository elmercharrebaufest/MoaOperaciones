using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasonService
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId);
        void EnviarMailAltaTempranaCuit(OrdenDeCargaFason orden, string ordenId, bool gestionaDestino, bool gestionaDestinatario);
        void EnviarMailTransporteNoExiste(OrdenDeCargaFason ordenDeCarga);
    }
}
