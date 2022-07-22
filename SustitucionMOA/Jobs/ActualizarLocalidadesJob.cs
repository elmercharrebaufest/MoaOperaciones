using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOA.Jobs
{

    public interface IActualizarLocalidades : IHangfireJob { };

    public class ActualizarLocalidades : IActualizarLocalidades
    {
        private readonly ILocalidadService localidadService;
        protected readonly IRepositorio repositorio;
        public ActualizarLocalidades(ILocalidadService localidadService, IRepositorio repositorio)
        {
            this.localidadService = localidadService;
            this.repositorio = repositorio;
        }
        public void Execute()
        {
            try
            {
                if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarLocalidades").Habilitado == false)
                    return;

                this.SincronizarLocalidades(localidadService.SincronizarLocalidadesScato(), localidadService.ObtenerProvinciasScato());
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
        }


        private void SincronizarLocalidades(List<LocalidadDto> listaLocalidadesScato, List<ProvinciaDto> listaProvinciaScato)
        {          
            var listaLocalidades = repositorio.Listar<Localidad>().ToList();
            foreach (var local in listaLocalidadesScato)
            {
                var codigoAfip = Convert.ToInt32(local.CodigoAfip);
                var codigo = listaLocalidades.FirstOrDefault(x => x.CodLocalidad == codigoAfip);
                var provinciaId = listaProvinciaScato.Find(x => x.Id == local.ProvinciaId);
                if (codigo == null)
                {
                    Localidad localidad = new Localidad();
                    localidad.Nombre = local.Descripcion;
                    localidad.ProvinciaId = provinciaId.CodigoAfip;
                    localidad.CodLocalidad = Convert.ToInt32(local.CodigoAfip);
                    localidad.PartidoId = 1;
                    repositorio.Agregar(localidad);
                }
            }

            repositorio.GuardarCambios();
        }
    }
}