using System;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{
    public interface ICcSsObtenerArchivosUcropJob : IHangfireJob { };

    public class CcSsObtenerArchivosUcropJob : ICcSsObtenerArchivosUcropJob
    {
        protected readonly ICampoSustentableService campoSustentableService;
        protected readonly IRepositorio repositorio;
        public CcSsObtenerArchivosUcropJob(ICampoSustentableService campoSustentableService, IRepositorio repositorio)
        {
            this.campoSustentableService = campoSustentableService;
            this.repositorio = repositorio;
        }
        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "CcSsObtenerArchivosUcropJob");
                if (habilitacion == null || !habilitacion.Habilitado)
                {
                    return;
                }

                var archivosADescargar = repositorio.Listar<ArchivoCampoSustentable>(archivo => !archivo.ProcesadoUcropit);

                foreach (var archivoAdescargar in archivosADescargar)
                {
                    try
                    {
                        campoSustentableService.DescargarArchivosDeGoogleDrive(archivoAdescargar);

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }

                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}