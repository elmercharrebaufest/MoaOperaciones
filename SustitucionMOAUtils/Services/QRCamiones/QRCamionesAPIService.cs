using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities.QRCamiones;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces.QRCamiones;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOAUtils.Services.QRCamiones
{
    public class QRCamionesAPIService : IQRCamionesAPIService
    {
        private readonly IRepositorio _repositorio;

        public QRCamionesAPIService(IRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public List<QRCamionesConfiguracion> ObtenerConfiguracionesPorTipoWorkflow(string tipoWorkflow)
        {
            try
            {
                var lista = _repositorio.Listar<QRCamionesConfiguracion>(x => x.TipoWorkflow == tipoWorkflow)
                                        .OrderBy(y => y.Id)
                                        .ToList();
                return lista;
            }
            catch (Exception ex)
            {
                Log.Error($"Error obteniendo configuraciones QRCamiones: {ex.Message}", ex);
                return new List<QRCamionesConfiguracion>();
            }
        }

        public QRCamionesConfiguracion ObtenerConfiguracionPorNombre(string nombreEtapa)
        {
            try
            {
                var etapa = _repositorio.Obtener<QRCamionesConfiguracion>(x => x.NombreEtapa == nombreEtapa);
				return etapa;
            }
            catch (Exception ex)
            {
                Log.Error($"Error obteniendo configuracion QRCamiones nombreEtapa:{nombreEtapa} {ex.Message}", ex);
                return null;
            }
        }

        public ResultadoGenerico GuardarConfiguracion(QRCamionesConfiguracion model)
        {
            var resultado = new ResultadoGenerico();

            if (model == null)
            {
                resultado.Errores = new List<ErrorMessage> { new ErrorMessage { Message = "El modelo de configuración es nulo" } };
                return resultado;
            }

            try
            {
                if (model.Id == 0)
                {
                    _repositorio.Agregar(model);
                    Log.ExternalAPIInfo($"Creada nueva configuración QRCamiones: {model.NombreEtapa}");
                }
                else
                {
                    var existente = _repositorio.Obtener<QRCamionesConfiguracion>(x => x.Id == model.Id);
                    if (existente == null)
                    {
                        resultado.Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"No se encontró la configuración con Id {model.Id}" } };
                        return resultado;
                    }

                    existente.NombreEtapa = model.NombreEtapa;
                    existente.TiempoEstimado = model.TiempoEstimado;
                    existente.TipoWorkflow = model.TipoWorkflow;
                    existente.FinEtapa = model.FinEtapa;
                    existente.FinEtapaEsControlRecorrido = model.FinEtapaEsControlRecorrido;

                    Log.ExternalAPIInfo($"Actualizada configuración QRCamiones Id={existente.Id} NombreEtapa={existente.NombreEtapa}");
                }

                _repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error($"Error guardando configuración QRCamiones: {ex.Message}", ex);
                resultado.Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"Error interno: {ex.Message}" } };
                return resultado;
            }
        }

        public ResultadoGenerico EliminarConfiguracion(int id)
        {
            var resultado = new ResultadoGenerico();

            try
            {
                var existente = _repositorio.Obtener<QRCamionesConfiguracion>(x => x.Id == id);
                if (existente == null)
                {
                    resultado.Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"No se encontró la configuración con Id {id}" } };
                    return resultado;
                }

                _repositorio.Remover(existente);
                _repositorio.GuardarCambios();

                Log.ExternalAPIInfo($"Eliminada configuración QRCamiones Id={id} NombreEtapa={existente.NombreEtapa}");
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error($"Error eliminando configuración QRCamiones id={id}: {ex.Message}", ex);
                resultado.Errores = new List<ErrorMessage> { new ErrorMessage { Message = $"Error interno: {ex.Message}" } };
                return resultado;
            }
        }
    }
}