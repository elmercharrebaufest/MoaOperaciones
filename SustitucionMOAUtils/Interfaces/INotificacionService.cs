using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface INotificacionService
    {
        string Agregar(Notificacion notificacion);
        string Deshabilitar(int idNotificacion);
        string Editar(Notificacion oNotificacion);
        string Eliminar(int idNotificacion);
        string Habilitar(int idNotificacion);
    }
}
