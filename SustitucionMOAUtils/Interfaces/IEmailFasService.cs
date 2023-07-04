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
        void EnviarMailContratoSinKm(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga);

        void EnviarMailTransporteNoExiste(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailValidacionesCrediticias(OrdenDeCarga ordenDeCarga);
        
        void EnviarMailVariasFacturasPendientes(OrdenDeCarga ordenDeCarga);
    }
}
