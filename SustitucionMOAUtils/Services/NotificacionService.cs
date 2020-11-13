using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{

    public class NotificacionService : INotificacionService
    {
        protected readonly IRepositorio repositorio;

        public NotificacionService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public string Agregar(Notificacion notificacion)
        {

            notificacion.Borrada = false;
            repositorio.Agregar(notificacion);

            repositorio.GuardarCambios();

            return SuccessMsg.NotificacionAgregada;
        }

        public string Editar(Notificacion oNotificacion)
        {
            var idNotificacion = oNotificacion.Id;

            var notificacion = repositorio.Obtener<Notificacion>(idNotificacion);

            notificacion.Nombre = oNotificacion.Nombre;
            notificacion.FechaInicio = oNotificacion.FechaInicio;
            notificacion.FechaFin = oNotificacion.FechaFin;
            notificacion.Habilitada = oNotificacion.Habilitada;
            notificacion.LinkAdjunto = oNotificacion.LinkAdjunto;
            notificacion.Mensaje = oNotificacion.Mensaje;
            notificacion.FiltroRoles = oNotificacion.FiltroRoles;
            notificacion.FiltroTipoUsuario = oNotificacion.FiltroTipoUsuario;

            repositorio.GuardarCambios();

            return SuccessMsg.NotificacionActualizada;
        }

        public string Habilitar(int idNotificacion)
        {
            var notificacion = repositorio.Obtener<Notificacion>(idNotificacion);

            notificacion.Habilitada = true;

            repositorio.GuardarCambios();
            return SuccessMsg.NotificacionActualizada;
        }


        public string Deshabilitar(int idNotificacion)
        {
            var notificacion = repositorio.Obtener<Notificacion>(idNotificacion);

            notificacion.Habilitada = false;

            repositorio.GuardarCambios();
            return SuccessMsg.NotificacionActualizada;
        }

        public string Eliminar(int idNotificacion)
        {
            var notificacion = repositorio.Obtener<Notificacion>(idNotificacion);

            notificacion.Borrada = false;

            repositorio.GuardarCambios();

            return SuccessMsg.NotificacionBorrada;
        }

        public List<Notificacion> Listar()
        {
            var listado = repositorio.Listar<Notificacion>(n => !n.Borrada);

            return listado;
        }

        public List<Notificacion>  ObtenerNotificacionesUsuario (string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);

            var listado = repositorio.Listar<Notificacion>(
                n => !n.Borrada
                && n.Habilitada
                && n.FechaInicio >= DateTime.Now
                && n.FechaFin <= DateTime.Now
                && n.FiltroRoles.Any(x => usuario.Roles.Any(y => y.Id == x.Id))
                && n.FiltroTipoUsuario.Contains(usuario.TipoUsuario)
            ); ;

            return listado;
        }
    }
}
