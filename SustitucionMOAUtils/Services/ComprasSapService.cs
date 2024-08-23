// Ignore Spelling: Solp Sustitucion Utils Solpe

using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSapService : IComprasSapService
    {
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;

        public ComprasSapService(IObtenerSolpConsumerMOA obtenerSolpConsumerMOA
            )
        {
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPendientesAdjudicar(string numeroSolpe)
        {
            ObtenerSolpRequest request = new ObtenerSolpRequest()
            {
                NumeroSolp = numeroSolpe,
            };

            ObtenerSolpSAPResponse response = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(request);

            return response.Posiciones.Where(posicion => posicion.Ordered < posicion.Cantidad);
        }
    }
}