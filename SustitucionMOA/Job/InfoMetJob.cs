using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using SustitucionMOAUtils.Services;

namespace SustitucionMOA.Job
{
    public class InfoMetJob : IJob
    {
        private AduanaService _service;

        public InfoMetJob() {
            _service = new AduanaService();
        }

        public Task Execute(IJobExecutionContext context)
        {
            _service.obtenerInfoMet();
            return null;
        }
    }
}