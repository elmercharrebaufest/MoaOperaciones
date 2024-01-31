using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteApiService : IAplicacionCartaPorteApiService
    {
        private readonly IRepositorio _repositorio;

        public AplicacionCartaPorteApiService(IRepositorio repositorio)
        {
            this._repositorio = repositorio;
        }

        public List<AplicacionCartaPorteApiDto> ObtenerAplicacionesAProcesar()
        {
            var aplicacionesResponse = new List<AplicacionCartaPorteApiDto>();

            var aplicacionesBD = _repositorio.Listar<AplicacionCartaPorte>(x => x.Estado == EstadoAplicacionCartaPorte.Pendiente);

            foreach (var entidad in aplicacionesBD)
            {
                aplicacionesResponse.Add(new AplicacionCartaPorteApiDto(entidad));
                entidad.Estado = EstadoAplicacionCartaPorte.EnProceso;
            }
            _repositorio.GuardarCambios();

            return aplicacionesResponse;
        }

        public void ActualizarEstadoAplicacion(int id, int nuevoEstado, string error)
        {
            var aplicacion = _repositorio.Obtener<AplicacionCartaPorte>(id);

            aplicacion.Estado = (EstadoAplicacionCartaPorte)nuevoEstado;
            aplicacion.Error = error;

            _repositorio.GuardarCambios();
        }
    }
}
