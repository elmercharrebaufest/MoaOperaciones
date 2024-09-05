using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasonService
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId);
        void EnviarMailAltaTempranaCuit(OrdenDeCargaFason orden, string ordenId, bool gestionaDestino, bool gestionaDestinatario);
        void EnviarMailIntentoAnulacionActiva(OrdenDeCargaFason orden);
        void EnviarMailIntentoEdicionActiva(OrdenDeCargaFason orden, OrdenDeCargaFasonRequest request);
        void EnviarMailTransporteNoExiste(OrdenDeCargaFason ordenDeCarga);
    }
}
