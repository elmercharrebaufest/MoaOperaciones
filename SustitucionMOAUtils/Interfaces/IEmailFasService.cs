using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasService
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial, string ordenId);

        void EnviarMailAltaTempranaCuit(OrdenDeCarga ordenDeCarga, string ordenId, bool gestionaDestino, bool gestionaDestinatario);

        void EnviarMailContratoSinKm(OrdenDeCarga ordenDeCarga);

        void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga);

        void EnviarMailOrdenDeCargaVencida(OrdenDeCarga ordenDeCarga);

        void EnviarMailTransporteNoExiste(OrdenDeCarga ordenDeCarga);

        void EnviarMailValidacionesCrediticias(OrdenDeCarga ordenDeCarga);

        void EnviarMailVariasFacturasPendientes(OrdenDeCarga ordenDeCarga);

        void EnviarMailVariosContratos(OrdenDeCarga ordenDeCarga);

        void EnviarMailVencieronOrdenesDeCarga(List<OrdenDeCarga> ordenesDeCarga);

        void EnviarMailSolicitudEdicion(OrdenDeCarga ordenDeCarga, List<OrdenDeCargaCambiosHistorial> historialCambios);

        void EnviarMailSolicitudAnulacion(OrdenDeCarga ordenDeCarga);

    }
}
