using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailResiduosService
    {
        void EnviarMailTransporteNoExiste(string razonSocialTransporte, string cuitTransporte);
        void EnviarMailOrdenesVencidas(IEnumerable<OrdenResiduos> ordenes);
        void EnviarMailIntentoAnulacionOrdenActiva(OrdenResiduos orden);
        void EnviarMailIntentoEdicionOrdenActiva(OrdenResiduos orden);
    }
}
