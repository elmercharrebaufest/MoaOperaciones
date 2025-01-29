using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using Log = SustitucionMOAUtils.Logger.Log;

namespace SustitucionMOA.Jobs
{
    public interface IEliminarFacturasAntiguasJob : IHangfireJob { }

    public class EliminarFacturasAntiguasJob : IEliminarFacturasAntiguasJob
    {
        private readonly IRepositorio repositorio;
        private readonly IFacturaService facturaService;
        public EliminarFacturasAntiguasJob(IRepositorio repositorio, IFacturaService facturaService)
        {
            this.repositorio = repositorio;
            this.facturaService = facturaService;
        }

        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "EliminarFacturasAntiguasJob");
                if (habilitacion == null || !habilitacion.Habilitado)
                {
                    return;
                }

                facturaService.EliminarFacturasAntiguas();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}