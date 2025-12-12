using System;
using System.Collections.Generic;
using System.Linq;
using ModelDto = SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Helpers;

namespace SustitucionMOA.Jobs
{
    public interface IActualizarLocalidadesPartidosJob : IHangfireJob
    {
        bool Habilitado();
    }

    public class ActualizarLocalidadesPartidosJob : IActualizarLocalidadesPartidosJob
    {
        protected readonly IRepositorio repositorio;
        private readonly IDataAgroService dataAgroService;

        private bool _Habilitado = false;
        public bool Habilitado()
        {
            return _Habilitado;
        }

        public ActualizarLocalidadesPartidosJob(IRepositorio repositorio, IDataAgroService dataAgroService)
        {
            this.repositorio = repositorio;
            this.dataAgroService = dataAgroService;
        }

        public void Execute()
        {
            try
            {
                var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarLocalidades");
                if (habilitacion == null || !habilitacion.Habilitado)
                    return;

                Log.Debug("Inicia job ActualizarLocalidades");

                _Habilitado = true;

                var partidos = dataAgroService.ListarPartidos();
                SincronizarPartidos(partidos);

                var localidades = dataAgroService.ListarLocalidades();
                SincronizarLocalidades(localidades);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }


        private void SincronizarLocalidades(List<ModelDto.LocalidadDto> localidadesDataAgro)
        {
            ModelDto.LocalidadDto localidadTemp;
            var listaLocalidades = repositorio.ListarTodos<Localidad>().ToList();
            foreach (var localidad in localidadesDataAgro)
            {
                localidadTemp = localidad;
                var localidadGuardada = listaLocalidades.FirstOrDefault(x => x.CodLocalidad == localidad.LocalidadId);
                try
                {
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
                }
                catch (Exception ex)
                {
                    Log.Error($"Error sincronizando la Localidad {localidadTemp.ToJson()}", ex);
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

                if (partidoGuardado == null)
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