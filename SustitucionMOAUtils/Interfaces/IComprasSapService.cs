// Ignore Spelling: Utils Sustitucion

using SustitucionMOAModel.Dto;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasSapService
    {
        List<TablaSapDto> ObtenerCecoSap();

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<PosicionSolpSAP> posicionesSap);

        bool PosicionPendienteSap(PosicionSolpSAP position);

        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp);

        IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp);
    }
}