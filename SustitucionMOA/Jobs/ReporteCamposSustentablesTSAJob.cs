using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface IReporteCamposSustentablesTSAJob : IHangfireJob { }

    public class ReporteCamposSustentablesTSAJob: IReporteCamposSustentablesTSAJob
    {
        private readonly IReportesService _reportesService;

        public ReporteCamposSustentablesTSAJob(IReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        public void Execute()
        {
            try
            {
                _reportesService.EnviarReporteCamposSustentablesTSA();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}