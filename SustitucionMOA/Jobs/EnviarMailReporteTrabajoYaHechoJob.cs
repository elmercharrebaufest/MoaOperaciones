using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SustitucionMOA.Jobs
{
    public interface IEnviarMailReporteTrabajoYaHechoJob : IHangfireJob { }

    public class EnviarMailReporteTrabajoYaHechoJob : IEnviarMailReporteTrabajoYaHechoJob
    {
        private readonly IRepositorio repositorio;
        private readonly IComprasService comprasService;

        public EnviarMailReporteTrabajoYaHechoJob(IRepositorio repositorio, IComprasService comprasService)
        {
            this.repositorio = repositorio;
            this.comprasService = comprasService;
        }

        public void Execute()
        {
            try
            {
                Log.Info("Inicia ejecución EnviarMailReporteTrabajoYaHechoJob");
                if (repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "EnviarMailReporteTrabajoYaHechoJob").Habilitado)
                {
                    return;
                }
                comprasService.EnviarReporteTrabajoYaHecho();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}