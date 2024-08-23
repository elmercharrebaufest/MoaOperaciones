// Ignore Spelling: Utils Sustitucion

using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasSapService
    {
        IEnumerable<PosicionSolpSAP> ObtenerPendientesAdjudicar(string numeroSolpe);
    }
}