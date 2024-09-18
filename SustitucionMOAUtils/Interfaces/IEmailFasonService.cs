using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasonService
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId);
        void EnviarMailAltaTempranaCuit(OrdenDeCargaFason orden, string ordenId, bool gestionaDestino, bool gestionaDestinatario);
        void EnviarMailCamionAutorizadoEnVariasOrdenes(string patenteChasis, List<string> cuitsClientesOrdenes);
        void EnviarMailIntentoAnulacionActiva(OrdenDeCargaFason orden);
        void EnviarMailIntentoEdicionActiva(OrdenDeCargaFason orden, OrdenDeCargaFasonRequest request);
        void EnviarMailTransporteNoExiste(OrdenDeCargaFason ordenDeCarga);
        void EnviarMailVencieronOrdenesDeCarga(List<OrdenDeCargaFason> ordenes);
    }
}
