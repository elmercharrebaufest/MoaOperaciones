using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasService
    {
        void EnviarMailAltaIntermediarioFlete(string cuit, string razonSocial);

        void EnviarMailAltaTempranaCuit(string cuit, string razonSocial);

        void EnviarMailContratoSinKm(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga);

        void EnviarMailOrdenDeCargaVencida(OrdenDeCarga ordenDeCarga);

        void EnviarMailTransporteNoExiste(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailValidacionesCrediticias(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailVariasFacturasPendientes(OrdenDeCarga ordenDeCarga);

        void EnviarMailVariosContratos(OrdenDeCarga ordenDeCarga);

        void EnviarMailVencieronOrdenesDeCarga(List<OrdenDeCarga> ordenesDeCarga);

    }
}
