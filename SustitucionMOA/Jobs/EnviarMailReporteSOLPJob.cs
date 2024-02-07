using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface IEnviarMailReporteSOLPJob : IHangfireJob { }

    public class EnviarMailReporteSOLPJob : IEnviarMailReporteSOLPJob
    {
        private readonly IComprasService _comprasService;
        private readonly IRepositorio repositorio;

        public EnviarMailReporteSOLPJob(IComprasService comprasService, IRepositorio repositorio)
        {
            _comprasService = comprasService;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnviarMailReporteSOLPJob").Habilitado == false)
                    return;

                _comprasService.ObtenerDatosReporteSolp();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}