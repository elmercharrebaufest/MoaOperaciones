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
    public interface IReenviarCamposSustentablesACertificadorJob : IHangfireJob { };
    public class ReenviarCamposSustentablesACertificadorJob : IReenviarCamposSustentablesACertificadorJob
    {
        protected readonly ICampoSustentableService campoSustentableService;

        protected readonly IRepositorio repositorio;
        public ReenviarCamposSustentablesACertificadorJob(ICampoSustentableService campoSustentableService, IRepositorio repositorio)
        {
            this.campoSustentableService = campoSustentableService;
            this.repositorio = repositorio;
        }
        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReenviarCamposSustentablesACertificadorJob").Habilitado == false)
                    return;

                campoSustentableService.ReenviarCamposACertificadorDeSustentables();
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

        }
    }
}