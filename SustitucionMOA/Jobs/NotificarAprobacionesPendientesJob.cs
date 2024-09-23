using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface INotificarAprobacionesPendientesJob : IHangfireJob { }

    public class NotificarAprobacionesPendientesJob : INotificarAprobacionesPendientesJob
    {
        private readonly IAprobacionesService _aprobacionesService;

        public NotificarAprobacionesPendientesJob(IAprobacionesService aprobacionesService)
        {
            _aprobacionesService = aprobacionesService;
        }

        public void Execute()
        {
            try
            {
                _aprobacionesService.Notificar();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}