using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Notificacion = SustitucionMOAModel.Entities.Notificacion;

namespace SustitucionMOAUtils.Services
{

    public class NotificacionService : INotificacionService
    {
        protected readonly IRepositorio repositorio;

        public NotificacionService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }


        public string GrabarNotificacion(Notificacion notificacion)
        {
            var mensajeError = ValidarNotificacion(notificacion);
            if (mensajeError != "")
            {
                throw new ValidationCustomException(mensajeError);
            }


            string resultado;

            //formatea el mensaje. si hay una imagen la ajusta al ancho de la pantalla 
            if (!notificacion.Mensaje.Contains("<img width= 100%"))
            {
                notificacion.Mensaje = notificacion.Mensaje.Replace("<img", "<img width= 100%");

            }



            if ((notificacion?.Id ?? 0) > 0)
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
            if (notificacion.Prioridad == 0)
            {
                notificacion.Prioridad = 2;
            }
            if (notificacion.Nombre.Length < 3)
            {
                return "El campo Nombre debe tener al menos 3 caracteres";
            }
            if (notificacion.Mensaje.Length < 3)
            {
                return "El campo Mensaje debe tener al menos 3 caracteres";
            }

            if (notificacion.FechaInicio >= notificacion.FechaFin)
            {
                return "La Fecha Desde debe ser menos a la Fecha Hasta";

            }

            if (repositorio.Existe<Notificacion>(n => n.Nombre == notificacion.Nombre && n.Id != notificacion.Id))
            {
                return "Ya existe una notificación con el mismo nombre";
            }

            return "";
        }


        private string Agregar(Notificacion notificacion)
        {
            notificacion.Borrada = false;
            notificacion.FechaCreacion = DateTime.Now;

            var roles = notificacion.FiltroRoles;

            notificacion.FiltroRoles = new List<Rol>();

            foreach (Rol rol in roles)
            {
                var nuevoRol = repositorio.Obtener<Rol>(rol.Id);
                notificacion.FiltroRoles.Add(nuevoRol);
            }

            repositorio.Agregar(notificacion);

            repositorio.GuardarCambios();

            return SuccessMsg.NotificacionAgregada;
        }


        private string Editar(Notificacion oNotificacion)
        {
            var idNotificacion = oNotificacion.Id;
            var notificacion = repositorio.Obtener<Notificacion>(idNotificacion);

            notificacion.Nombre = oNotificacion.Nombre;
            notificacion.FechaInicio = oNotificacion.FechaInicio;
            notificacion.FechaFin = oNotificacion.FechaFin;
            notificacion.Habilitada = oNotificacion.Habilitada;
            notificacion.LinkAdjunto = oNotificacion.LinkAdjunto;
            notificacion.Mensaje = oNotificacion.Mensaje;
            notificacion.Prioridad = oNotificacion.Prioridad;

            notificacion.FiltroRoles.Clear();
            foreach (Rol rol in oNotificacion.FiltroRoles)
            {
                var nuevoRol = repositorio.Obtener<Rol>(rol.Id);
                notificacion.FiltroRoles.Add(nuevoRol);
            }

            //eliminos los archivos asociados a la notificacion 
            if (notificacion.ArchivosAdjuntos != null && notificacion.ArchivosAdjuntos.Any())
            {
                repositorio.RemoverTodos(notificacion.ArchivosAdjuntos.ToList());
                notificacion.ArchivosAdjuntos.Clear();
            }


            //guardo los nuevos archivos asociados a la notificacion 
            notificacion.ArchivosAdjuntos = oNotificacion.ArchivosAdjuntos;

            //EliminarNotificacionesPorId(idNotificacion);
            List<NotificacionLeida> notificacionesAEliminar = repositorio.Listar<NotificacionLeida>(x => x.Notificacion_Id == idNotificacion);
            if (notificacionesAEliminar != null && notificacionesAEliminar.Any())
            {
                repositorio.RemoverTodos(notificacionesAEliminar);
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

            notificacion.Borrada = true;

            repositorio.GuardarCambios();

            return SuccessMsg.NotificacionBorrada;
        }


        public List<NotificacionDto> Listar()
        {
            var listado = repositorio.Listar<Notificacion>().Select(x => new NotificacionDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaInicio = x.FechaInicio.ToString(),
                HoraInicio = x.FechaInicio.Hour,
                FechaFin = x.FechaFin.ToString(),
                Borrada = x.Borrada,
                Habilitada = x.Habilitada,
                Mensaje = x.Mensaje,
                LinkAdjunto = x.LinkAdjunto,
                Prioridad = x.Prioridad
            }).ToList();

            return listado;
        }


        /// <summary>
        /// Devuelve datos para armar las tarjetas del carrousel
        /// </summary>
        /// <param name="mailUsuario"></param>
        /// <returns></returns>
        public List<NotificacionDto> ObtenerNotificacionesUsuario(string mailUsuario)
        {
            Usuario usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);
            int UsuarioId = usuario.Id;

            List<NotificacionLeida> NoticiasLeidas = repositorio.Listar<NotificacionLeida>(x => x.Usuario_Id == UsuarioId).ToList();

            var listado = repositorio.Listar<Notificacion>
                (
                    n => !n.Borrada
                    && n.Habilitada
                )
            .AsEnumerable()
            .Where(n => n.FiltroRoles.Any(x => usuario.Roles.Any(y => y.Id == x.Id) && DateTime.Now >= n.FechaInicio && DateTime.Now <= n.FechaFin && n.Habilitada == true)

            ).Select(x => new NotificacionDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaInicio = x.FechaInicio.ToString(),
                HoraInicio = x.FechaInicio.Hour,
                FechaFin = x.FechaFin.ToString(),
                Borrada = x.Borrada,
                Habilitada = x.Habilitada,
                Mensaje = x.Mensaje,
                LinkAdjunto = x.LinkAdjunto,
                Prioridad = x.Prioridad,
                ArchivosAdjuntos = x.ArchivosAdjuntos.Where(y => y.AdjuntoTipo == "previsualizacion").ToList(),
                Leida = NoticiasLeidas.Any(nl => nl.Notificacion_Id == x.Id) ? 1 : 0
            })
            .OrderBy(s => s.Leida)
            .ThenBy(s => s.Prioridad)
            .ThenByDescending((x =>
            {
                DateTime dt;
                DateTime.TryParse(x.FechaInicio, out dt);
                return dt;
            }))
            .ToList();
            return listado;
        }


        public NotificacionDto ObtenerNotificacion(int notificacionId)
        {
            var notificacion = repositorio.Obtener<Notificacion>(notificacionId);
            List<NotificacionAdjunto> oArchivosAdjuntos = notificacion.ArchivosAdjuntos.ToList();
            List<NotificacionLeida> NoticiasLeidas = repositorio.Listar<NotificacionLeida>(x => x.Notificacion_Id == notificacionId).ToList();

            var notificacionDto = new NotificacionDto
            {
                Id = notificacion.Id,
                Nombre = notificacion.Nombre,
                FechaInicio = notificacion.FechaInicio.ToString("dd/MM/yyyy"),
                HoraInicio = notificacion.FechaInicio.Hour,
                FechaFin = notificacion.FechaFin?.ToString("dd/MM/yyyy"),
                Borrada = notificacion.Borrada,
                Habilitada = notificacion.Habilitada,
                FiltroRoles = notificacion.FiltroRoles.Select(r => r.Id).ToList(),
                Mensaje = notificacion.Mensaje,
                LinkAdjunto = notificacion.LinkAdjunto,
                Prioridad = notificacion.Prioridad,
                ArchivosAdjuntos = oArchivosAdjuntos,
                Leida = NoticiasLeidas.Any(nl => nl.Notificacion_Id == notificacion.Id) ? 1 : 0
            };
            return notificacionDto;
        }


        public List<NotificacionPrioridadDto> ObtenerTodosNotificacionPrioridad()
        {
            var lst = repositorio.Listar<NotificacionPrioridad>().Select(x => new NotificacionPrioridadDto
            {
                Prioridad_Id = x.Prioridad_Id,
                Prioridad_Descripcion = x.Prioridad_Descripcion,
            }).ToList();

            return lst;
        }


        public string GrabarNotificacionComoLeida(int NotificacionId, string mailUsuario)
        {
            Usuario usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);
            int UsuarioId = usuario.Id;

            List<NotificacionLeida> NoticiaLeida = repositorio.Listar<NotificacionLeida>(x => x.Notificacion_Id == NotificacionId).ToList();
            if (NoticiaLeida.Count == 0)
            {
                var notificacionLeida = new NotificacionLeida
                {
                    Notificacion_Id = NotificacionId,
                    Usuario_Id = UsuarioId,
                    FechaLeida = DateTime.Now
                };

                repositorio.Agregar(notificacionLeida);
                repositorio.GuardarCambios();
                return SuccessMsg.NotificacionActualizada;
            }
            else
            {
                return "No se agrega registro. Noticia leída previamente.";
            }
        }


        /// <summary>
        /// Cuando el admin edita una notificacion, se marca como no leída para todos los usuarios.
        /// </summary>
        /// <param name="NotificacionId"></param>
        /// <param name="mailUsuario"></param>
        /// <returns></returns>
        public string EliminarNotificacionesPorId(int IdParaBorrar)
        {
            List<NotificacionLeida> notificacionesAEliminar = repositorio.Listar<NotificacionLeida>(x => x.Notificacion_Id == IdParaBorrar).ToList();

            if (notificacionesAEliminar.Count > 0)
            {
                //foreach (var notificacion in notificacionesAEliminar)
                //{
                //    repositorio.Remover(notificacion);
                //}

                repositorio.RemoverTodos(notificacionesAEliminar);

                repositorio.GuardarCambios();

                return SuccessMsg.NotificacionActualizada;
            }
            else
            {
                return "No se encontraron notificaciones con el Id proporcionado.";
            }
        }


        public List<NotificacionSinAdjuntosDto> ObtenerListadoCompletoNotificacion(string mailUsuario)
        {
            Usuario usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);
            int UsuarioId = usuario.Id;

            List<NotificacionLeida> NoticiasLeidas = repositorio.Listar<NotificacionLeida>(x => x.Usuario_Id == UsuarioId).ToList();

            var listado = repositorio.Listar<Notificacion>
                (
                    n => !n.Borrada
                    && n.Habilitada
                )
            .AsEnumerable()
            .Where(n => n.FiltroRoles.Any(x => usuario.Roles.Any(y => y.Id == x.Id)) && n.Habilitada == true)
            .Select(x => new NotificacionSinAdjuntosDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaInicio = x.FechaInicio.ToString(),
                HoraInicio = x.FechaInicio.Hour,
                FechaFin = x.FechaFin.ToString(),
                Borrada = x.Borrada,
                Habilitada = x.Habilitada,
                FiltroRoles = x.FiltroRoles.Select(r => r.Id).ToList(),
                Mensaje = x.Mensaje,
                LinkAdjunto = x.LinkAdjunto,
                Prioridad = x.Prioridad,
                Leida = NoticiasLeidas.Any(nl => nl.Notificacion_Id == x.Id) ? 1 : 0,
                FechaCreacion = x.FechaCreacion.ToString(),
            })
            .OrderBy(s => s.Leida)
            .ThenBy(s => s.Prioridad)
            .ThenByDescending((x =>
            {
                DateTime dt;
                DateTime.TryParse(x.FechaCreacion, out dt);
                return dt;
            })
            )
            .ToList();

            return listado;
        }
    }
}
