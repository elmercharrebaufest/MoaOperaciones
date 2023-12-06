using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOA.Jobs
{
    public interface IVerificarOrdenesFacturaCompensadaJob : IHangfireJob { }

    public class VerificarOrdenesFacturaCompensadaJob : IVerificarOrdenesFacturaCompensadaJob
    {
        private readonly IOrdenDeCargaService ordenDeCargaService;

        public VerificarOrdenesFacturaCompensadaJob(IOrdenDeCargaService ordenDeCargaService)
        {
            this.ordenDeCargaService = ordenDeCargaService;
        }

        public void Execute()
        {
            try
            {
                ordenDeCargaService.VerificarOrdenesFacturaCompensadaJob();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}