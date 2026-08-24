using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ILogPesificacionService
    {
        LogPesificacionDto GuardarPesificacion(LogPesificacion entidad);
        List<LogPesificacionDto> GuardarPesificaciones(List<LogPesificacion> entidades);
        LogPesificacionDto GuardarPesificacionAutomatica(LogPesificacion entidad, HttpPostedFileBase file);
        List<LogPesificacionDto> ObtenerPorFiltrosPesificacionesManuales(FiltroDeBusquedaDto filtro);
        List<LogPesificacionDto> ObtenerPorFiltrosPesificacionesAutomaticas(FiltroDeBusquedaMasicoDto filtro);
        List<LogPesificacionDto> ObtenerPorUsuario(int idUsuario);
        List<LogPesificacionDto> ObtenerPesificacionesManuales(int idUsuario);
        List<LogPesificacionDto> ObtenerLogPesificaciones(List<int> ids);
        List<LogPesificacionDto> ObtenerPesificacionesAutomaticas(int idUsuario);
        Archivo ObtenerArchivoLogPesificacion(int idArchivo);
        void ActualizarEstadoLogPesificacion(LogPesificacion entidad);
        void ActualizarEstadoLogPesificacion(List<int> logIds);
    }
}
