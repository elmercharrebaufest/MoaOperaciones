// Ignore Spelling: Solp Sustitucion Utils Solpe

using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSapService : IComprasSapService
    {
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;

        public ComprasSapService(IObtenerSolpConsumerMOA obtenerSolpConsumerMOA)
        {
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp)
        {
            return ObtenerPosiciones(numeroSolp).Where(posicion => posicion.Ordered < posicion.Cantidad);
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosicionesPendientesAdjudicar(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp)
        {
            ObtenerSolpRequest request = new ObtenerSolpRequest()
            {
                NumeroSolp = numeroSolp,
                FechaDesde = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                FechaHasta = DateTime.Today.AddDays(1),
            };

            try
            {
                ObtenerSolpSAPResponse response = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(request);

                return response.Posiciones;
            }
            catch (Exception ex)
            {
                Logger.Log.ExternalAPIError(ex);
                throw;
            }
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosiciones(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }
    }
}
