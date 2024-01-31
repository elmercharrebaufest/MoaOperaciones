using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IVerificarSituacionCrediticiaJob : IHangfireJob { }

    public class VerificarSituacionCrediticiaJob : IVerificarSituacionCrediticiaJob
    {
        private readonly IOrdenDeCargaService _ordenCargaService;
        
        public VerificarSituacionCrediticiaJob(IOrdenDeCargaService ordenCargaService)
        {
            _ordenCargaService = ordenCargaService;
        }

        public void Execute()
        {
            try
            {

                _ordenCargaService.VerificarSituacionCrediticiaJob();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}