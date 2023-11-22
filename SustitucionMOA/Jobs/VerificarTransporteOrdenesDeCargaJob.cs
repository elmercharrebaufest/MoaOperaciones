using System;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{

    public interface IVerificarTransporteOrdenesDeCargaJob : IHangfireJob { }

    public class VerificarTransporteOrdenesDeCargaJob : IVerificarTransporteOrdenesDeCargaJob
    {
        private readonly IOrdenDeCargaService ordebDeCargaService;

        public VerificarTransporteOrdenesDeCargaJob(IOrdenDeCargaService ordebDeCargaService)
        {
            this.ordebDeCargaService = ordebDeCargaService;
        }

        public void Execute()
        {
            try
            {
                ordebDeCargaService.VerificarTransporteBulk();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}