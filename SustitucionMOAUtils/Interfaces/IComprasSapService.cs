// Ignore Spelling: Utils Sustitucion

using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasSapService
    {
        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp);
        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp);
    }
}