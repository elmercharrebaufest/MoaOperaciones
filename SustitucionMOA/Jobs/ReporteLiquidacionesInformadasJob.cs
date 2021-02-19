using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IReporteLiquidacionesInformadasJob : IHangfireJob { }

    public class ReporteLiquidacionesInformadasJob : IReporteLiquidacionesInformadasJob
    {
        private readonly IReportesService _reportesService;

        public ReporteLiquidacionesInformadasJob(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public void Execute()
        {
            try
            {
                _reportesService.EnviarReporteLiquidacionesInformadas();
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }
    }
}