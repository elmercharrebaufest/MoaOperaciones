using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IReporteLoginsJob : IHangfireJob { }

    public class ReporteLoginsJob : IReporteLoginsJob
    {
        private readonly IReportesService _reportesService;

        public ReporteLoginsJob(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public void Execute()
        {
            try
            {
                _reportesService.EnviarReporteLogin();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}