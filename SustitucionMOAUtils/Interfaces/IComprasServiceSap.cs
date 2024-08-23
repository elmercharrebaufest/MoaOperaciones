// Ignore Spelling: Utils Sustitucion

using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasServiceSap
    {
        IEnumerable<PosicionSolpSAP> ObtenerPendientesAdjudicar(string numeroSolpe);
    }
}