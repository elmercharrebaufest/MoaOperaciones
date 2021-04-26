using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface IReporteConflictosCamposSustentablesJob : IHangfireJob { }

    public class ReporteConflictosCamposSustentablesJob : IReporteConflictosCamposSustentablesJob
    {
        private readonly IReportesService _reportesService;

        public ReporteConflictosCamposSustentablesJob(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public void Execute()
        {
            try
            {
                _reportesService.EnviarReporteConflictosCamposSustentables();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}