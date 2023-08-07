using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasService : IEmailOrdenesCargaServiceBase
    {
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
