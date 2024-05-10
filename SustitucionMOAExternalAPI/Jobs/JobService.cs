using Hangfire;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOAExternalAPI.Jobs
{
    public interface IJobService
    {
        void ActualizarSolp(string nrosolp);
    }
    public class JobService: IJobService
    {
        private readonly IComprasService comprasService;

        public JobService(IComprasService comprasService)
        {
            this.comprasService = comprasService;
        }

        public void ActualizarSolp(string nrosolp)
        {
            try
            {
                BackgroundJob.Enqueue(() => comprasService.ObtenerSolpesDesdeSAPJob(new SustitucionMOAWS.WSConsumers.ObtenerSolpRequest
                {
                    NumeroSolp = nrosolp.TrimStart('0').PadLeft(10, '0'),
                    FechaDesde = new DateTime(2010, 01, 01),
                    FechaHasta = DateTime.Now.Date.AddDays(1)
                }));

                // Procesar la solicitud aquí
                
            }
            catch (Exception ex)
            {
                Log.ExternalAPIError(ex);
            }
        }
    }
}