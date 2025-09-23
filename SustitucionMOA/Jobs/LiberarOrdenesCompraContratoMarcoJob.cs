using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOA.Jobs
{
    public interface ILiberarOrdenesCompraContratoMarcoJob : IHangfireJob { }

    public class LiberarOrdenesCompraContratoMarcoJob : IHangfireJob
    {
        private readonly IRepositorio repositorio;
        private readonly IEntradaServicioService entradaServicioService;

        public LiberarOrdenesCompraContratoMarcoJob(IRepositorio repositorio, IEntradaServicioService entradaServicioService)
        {
            this.repositorio = repositorio;
            this.entradaServicioService = entradaServicioService;
        }

        public void Execute()
        {
            try
            {
                Log.Info("Inicia ejecución LiberarOrdenesCompraContratoMarcoJob");
                if (!repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "LiberarOrdenesCompraContratoMarcoJob").Habilitado)
                {
                    return;
                }
                entradaServicioService.LiberarOrdenesDeCompraConContratoMarco();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}