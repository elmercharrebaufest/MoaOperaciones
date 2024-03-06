using System;
using System.Collections.Generic;
using System.Linq;
using ModelDto =  SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Jobs
{

    public interface IActualizarLocalidadesPartidosJob : IHangfireJob
    {
        bool Habilitado();
    }
    public class ActualizarLocalidadesPartidosJob : IActualizarLocalidadesPartidosJob
    {
        protected readonly IRepositorio repositorio;
        protected readonly IDataAgroApiService dataAgroApiService;

        private bool _Habilitado = false;
        public bool Habilitado()
        {
            return _Habilitado;
        }
        public ActualizarLocalidadesPartidosJob(IRepositorio repositorio, IDataAgroApiService dataAgroApiService)
        {
            this.repositorio = repositorio;
            this.dataAgroApiService = dataAgroApiService;
        }
        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarLocalidades");
                if (habilitacion == null  || !habilitacion.Habilitado)
                    return;

                _Habilitado = true;
                var localidades = dataAgroApiService.ListarLocalidades();

                SincronizarLocalidades(localidades);
                var partidos = dataAgroApiService.ListarPartidos();
                SincronizarPartidos(partidos);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }


        private void SincronizarLocalidades(
            List<ModelDto.LocalidadDto> localidadesDataAgro)
        {          
            var listaLocalidades = repositorio.ListarTodos<Localidad>().ToList();
            foreach (var localidad in localidadesDataAgro)
            {
                var localidadGuardada = listaLocalidades.FirstOrDefault(x => x.CodLocalidad == localidad.LocalidadId);
                try {
                    if (localidadGuardada == null)
                    {
                        Localidad nuevaLocalidad = new Localidad();
                        nuevaLocalidad.Nombre = localidad.Nombre;
                        nuevaLocalidad.ProvinciaId = localidad.ProvinciaId;
                        nuevaLocalidad.CodLocalidad = localidad.LocalidadId;
                        nuevaLocalidad.PartidoId = Convert.ToInt32(localidad.PartidoId);
                        repositorio.Agregar(nuevaLocalidad);
                    }
                    else
                    {
                        localidadGuardada.Nombre = localidad.Nombre;
                        localidadGuardada.ProvinciaId = localidad.ProvinciaId;
                        localidadGuardada.PartidoId = Convert.ToInt32(localidad.PartidoId);
                    }
                }catch(Exception ex)
                {
                    Log.Error(ex);
                }
                
            }

            repositorio.GuardarCambios();
        }
        private void SincronizarPartidos(List<ModelDto.PartidoDto> partidosDA)
        {
            var partidosGuardados = repositorio.ListarTodos<Partido>();
            foreach (var partidoDA in partidosDA)
            {
                var partidoGuardado = partidosGuardados.FirstOrDefault(p => p.Id == partidoDA.Id);

                if(partidoGuardado == null)
                {
                    var partidoNuevo = new Partido
                    {
                        Id = partidoDA.Id,
                        ProvinciaID = partidoDA.ProvinciaId,
                        Descripcion = partidoDA.Descripcion,
                    };
                    repositorio.Agregar(partidoNuevo);
                }
                else
                {
                    partidoGuardado.ProvinciaID = partidoDA.ProvinciaId;
                    partidoGuardado.Descripcion = partidoDA.Descripcion;
                }

            }
            repositorio.GuardarCambios();
        }
    }
}