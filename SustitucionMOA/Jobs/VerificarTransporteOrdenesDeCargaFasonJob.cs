using System;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{

    public interface IVerificarTransporteOrdenesDeCargaFasonJob : IHangfireJob { }
    public class VerificarTransporteOrdenesDeCargaFasonJob : IVerificarTransporteOrdenesDeCargaFasonJob
    {
        private readonly IOrdenDeCargaFasonService _ordenDeCargaFasonService;

        public VerificarTransporteOrdenesDeCargaFasonJob(IOrdenDeCargaFasonService service)
        {
            _ordenDeCargaFasonService = service;
        }

        public void Execute()
        {
            try
            {
                _ordenDeCargaFasonService.VerificarTransporteJob();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}