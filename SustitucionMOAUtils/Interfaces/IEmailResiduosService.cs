using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailResiduosService
    {
        void EnviarMailTransporteNoExiste(string razonSocialTransporte, string cuitTransporte);
        void EnviarMailOrdenesVencidas(IEnumerable<OrdenResiduos> ordenes);
    }
}
