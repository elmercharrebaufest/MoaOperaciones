using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface IDerivacionAutomaticaJob : IHangfireJob
    {

    }

    public class DerivacionesAutomaticaJob : IDerivacionAutomaticaJob
    {
        private readonly ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService;

        public DerivacionesAutomaticaJob(ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService)
        {
            this.logicaDerivacionAutomaticaService = logicaDerivacionAutomaticaService;
        }


        public void Execute()
        {
            try
            {
                logicaDerivacionAutomaticaService.CorrerProcesoReasignacion();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}