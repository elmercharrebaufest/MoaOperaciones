using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using Log = SustitucionMOAUtils.Logger.Log;

namespace SustitucionMOA.Jobs
{
    public interface IEliminarLogsAntiguosJob : IHangfireJob { }

    public class EliminarLogsAntiguosJob : IEliminarLogsAntiguosJob
    {
        private readonly IRepositorio repositorio;
        private readonly ILogService logService;

        public EliminarLogsAntiguosJob(IRepositorio repositorio, ILogService logService)
        {
            this.repositorio = repositorio;
            this.logService = logService;
        }
        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "EliminarLogsAntiguosJob");
                if (habilitacion == null || !habilitacion.Habilitado)
                {
                    return;
                }
                Log.Info("HANGFIRE - EliminarLogsAntiguosJob - Iniciando");

                try
                {
                    logService.EliminarLogsAntiguos();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
                Log.Info($"HANGFIRE - EliminarLogsAntiguosJob - Fin");


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}