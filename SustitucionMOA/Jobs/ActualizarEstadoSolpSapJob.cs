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
    public interface IActualizarEstadoSolpSapJob : IHangfireJob { }

    public class ActualizarEstadoSolpSapJob : IActualizarEstadoSolpSapJob
    {
        private readonly IComprasService _comprasService;
        private readonly IRepositorio repositorio;

        public ActualizarEstadoSolpSapJob(IComprasService comprasService, IRepositorio repositorio)
        {
            _comprasService = comprasService;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarEstadoSolpSapJob").Habilitado == false)
                    return;

                _comprasService.ActualizarEstadoSolpBulk();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}