using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;

namespace SustitucionMOA.Jobs
{
    public interface ICertificarOrdenesCompraContratoMarcoJob : IHangfireJob { }

    public class CertificarOrdenesCompraContratoMarcoJob : IHangfireJob
    {
        private readonly IRepositorio repositorio;
        private readonly IEntradaServicioService entradaServicioService;

        public CertificarOrdenesCompraContratoMarcoJob(IRepositorio repositorio, IEntradaServicioService entradaServicioService)
        {
            this.repositorio = repositorio;
            this.entradaServicioService = entradaServicioService;
        }

        public void Execute()
        {
            try
            {
                Log.Info("Inicia ejecución CertificarOrdenesCompraContratoMarcoJob");
                if (!repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "CertificarOrdenesCompraContratoMarcoJob").Habilitado)
                {
                    return;
                }
                entradaServicioService.CertificarOrdenesDeCompraConContratoMarco();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}