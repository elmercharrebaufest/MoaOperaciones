using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel.Notificacion;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface INotificacionService
    {
        string GrabarNotificacion(Notificacion notificacion);

        string Deshabilitar(int idNotificacion);
        
        string Eliminar(int idNotificacion);
        
        string Habilitar(int idNotificacion);

        List<NotificacionDto> Listar();

        List<NotificacionDto> ObtenerNotificacionesUsuario(string mailUsuario);

        NotificacionDto ObtenerNotificacion(int notificacionId);

        List<NotificacionPrioridadDto> ObtenerTodosNotificacionPrioridad();

        string GrabarNotificacionComoLeida(int idNotificacion, string userMai);

        List<NotificacionSinAdjuntosDto> ObtenerListadoCompletoNotificacion(string mailUsuario);
    }
}
