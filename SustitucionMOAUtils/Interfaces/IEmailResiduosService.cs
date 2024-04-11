using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailResiduosService
    {
        void EnviarMailOrdenesVencidas(IEnumerable<OrdenResiduos> ordenes);
    }
}
