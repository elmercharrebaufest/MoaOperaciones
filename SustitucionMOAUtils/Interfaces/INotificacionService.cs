using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface INotificacionService
    {
        string GrabarNotificacion(Notificacion notificacion);

        string Deshabilitar(int idNotificacion);
        
        string Eliminar(int idNotificacion);
        
        string Habilitar(int idNotificacion);

        List<Notificacion> Listar();

        List<Notificacion> ObtenerNotificacionesUsuario(string mailUsuario);
    }
}
