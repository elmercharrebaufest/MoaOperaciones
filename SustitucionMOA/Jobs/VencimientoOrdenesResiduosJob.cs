using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOA.Jobs
{
    public interface IVencimientoOrdenesResiduosJob : IHangfireJob { };
    public class VencimientoOrdenesResiduosJob : IVencimientoOrdenesResiduosJob
    {
        private readonly IOrdenResiduosService ordenResiduosService;

        public VencimientoOrdenesResiduosJob(IOrdenResiduosService service)
        {
            this.ordenResiduosService = service;
        }

        public void Execute()
        {
            try
            {
                ordenResiduosService.VerificarVencimientoOrdenesResiduos();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}