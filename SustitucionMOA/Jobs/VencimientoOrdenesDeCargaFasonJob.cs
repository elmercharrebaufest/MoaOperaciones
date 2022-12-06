using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IVencimientoOrdenesDeCargaFasonJob : IHangfireJob { };

    public class VencimientoOrdenesDeCargaFasonJob : IVencimientoOrdenesDeCargaFasonJob
    {
        private readonly IOrdenDeCargaFasonService _ordenDeCargaFasonService;

        public VencimientoOrdenesDeCargaFasonJob(IOrdenDeCargaFasonService ordenDeCargaService)
        {
            this._ordenDeCargaFasonService = ordenDeCargaService;
        }

        public void Execute()
        {
            try
            {
                var response = _ordenDeCargaFasonService.VerificarVencimientoOrdenDeCargaFason();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

}
