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
    public interface IEnviarMailReporteTrabajoYaHecho : IHangfireJob { }

    public class EnviarMailReporteTrabajoYaHecho : IEnviarMailReporteTrabajoYaHecho
    {
        private readonly IRepositorio repositorio;
        private readonly IComprasService comprasService;

        public EnviarMailReporteTrabajoYaHecho(IRepositorio repositorio, IComprasService comprasService)
        {
            this.repositorio = repositorio;
            this.comprasService = comprasService;
        }

        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "EnviarMailReporteTrabajoYaHecho").Habilitado)
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