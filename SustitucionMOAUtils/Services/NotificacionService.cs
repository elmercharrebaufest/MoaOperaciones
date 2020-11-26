using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
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

        public string GrabarNotificacion (Notificacion notificacion)
        {
            var resultado = "";

            var mensajeError = ValidarNotificacion(notificacion);
            if (mensajeError != "")
            {
                throw new ValidationCustomException(mensajeError);
            }
           
            if (repositorio.Existe<Notificacion>(n => n.Id == notificacion.Id))
            {
                resultado = Editar(notificacion);
            }
            else
            {
                resultado = Agregar(notificacion);
            }

            return resultado;
        }

        private string ValidarNotificacion(Notificacion notificacion)
        {
            if (repositorio.Existe<Notificacion>(n => n.Nombre == notificacion.Nombre))
            {
                return "Ya existe una notificación con el mismo nombre";
            }
            
            return "";
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

            notificacion.FiltroRoles = new List<Rol>();

            foreach (Rol rol in oNotificacion.FiltroRoles)
            {
                var nuevoRol = repositorio.Obtener<Rol>(rol.Id);
                notificacion.FiltroRoles.Add(nuevoRol);
            }

            foreach (TipoUsuario tipoUsuario in oNotificacion.FiltroTipoUsuario)
            {
                var nuevoTipo = repositorio.Obtener<TipoUsuario>(tipoUsuario.Id);
                notificacion.FiltroTipoUsuario.Add(nuevoTipo);
            }

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

        public List<NotificacionDto> Listar()
        {
            var listado = repositorio.Listar<Notificacion>(n => !n.Borrada).Select(x => new NotificacionDto{
                    Id = x.Id,
                    Nombre = x.Nombre,
                    FechaInicio = x.FechaInicio.ToString(),
                    FechaFin= x.FechaFin.ToString(),
                    Borrada = x.Borrada,
                    Habilitada = x.Habilitada,
                    Mensaje = x.Mensaje,
                    LinkAdjunto = x.LinkAdjunto
            }).ToList();

            return listado;
        }

        public List<NotificacionDto>  ObtenerNotificacionesUsuario (string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);

            var listado = repositorio.Listar<Notificacion>(
                n => !n.Borrada
                && n.Habilitada
                && DateTime.Now >= n.FechaInicio
                && DateTime.Now <= n.FechaFin
                //&& n.FiltroRoles.Any(x => usuario.Roles.Any(y => y.Id == x))
                //&& n.FiltroTipoUsuario.Contains(usuario.TipoUsuario.Id)
            ).Select(x => new NotificacionDto
            {
                Nombre = x.Nombre,
                FechaInicio = x.FechaInicio.ToString(),
                FechaFin = x.FechaFin.ToString(),
                Borrada = x.Borrada,
                Habilitada = x.Habilitada,
                Mensaje = x.Mensaje,
                LinkAdjunto = x.LinkAdjunto
            }).ToList();


            return listado;
        }

        public NotificacionDto ObtenerNotificacion(int notificacionId)
        {
            var notificacion = repositorio.Obtener<Notificacion>(notificacionId);

            var notificacionDto = new NotificacionDto
            {
                Id = notificacion.Id,
                Nombre = notificacion.Nombre,
                FechaInicio = notificacion.FechaInicio.ToString("dd/MM/yyyy"),
                FechaFin = notificacion.FechaFin?.ToString("dd/MM/yyyy"),
                Borrada = notificacion.Borrada,
                Habilitada = notificacion.Habilitada,
                FiltroRoles = notificacion.FiltroRoles.Select(r => r.Id).ToList(),
                FiltroTipoUsuario = notificacion.FiltroTipoUsuario.Select(r => r.Id).ToList(),
                Mensaje = notificacion.Mensaje,
                LinkAdjunto = notificacion.LinkAdjunto
            };

            return notificacionDto;

            //return repositorio.Obtener<Operador, OperadorDto>(x => x.Id == id, x => new OperadorDto { Id = x.Id, Descripcion = x.Descripcion }) ?? new OperadorDto();
        }
    }
}
