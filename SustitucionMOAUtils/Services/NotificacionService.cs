using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
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

        public string GrabarNotificacion(Notificacion notificacion)
        {
            var mensajeError = ValidarNotificacion(notificacion);
            if (mensajeError != "")
            {
                throw new ValidationCustomException(mensajeError);
            }


            string resultado;
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

            if (repositorio.Existe<Notificacion>(n => n.Nombre == notificacion.Nombre && n.Id != notificacion.Id))
            {
                return "Ya existe una notificación con el mismo nombre";
            }

            return "";
        }

        private string Agregar(Notificacion notificacion)
        {
            notificacion.Borrada = false;

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

            notificacion.FiltroRoles.Clear();

            foreach (Rol rol in oNotificacion.FiltroRoles)
            {
                var nuevoRol = repositorio.Obtener<Rol>(rol.Id);
                notificacion.FiltroRoles.Add(nuevoRol);
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
            var listado = repositorio.Listar<Notificacion>(n => !n.Borrada).Select(x => new NotificacionDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaInicio = x.FechaInicio.ToString(),
                HoraInicio = x.FechaInicio.Hour,
                FechaFin = x.FechaFin.ToString(),
                Borrada = x.Borrada,
                Habilitada = x.Habilitada,
                Mensaje = x.Mensaje,
                LinkAdjunto = x.LinkAdjunto
            }).ToList();

            return listado;
        }

        public List<NotificacionDto> ObtenerNotificacionesUsuario(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == mailUsuario);

            var listado = repositorio.Listar<Notificacion>(
                n => !n.Borrada
                && n.Habilitada
                && DateTime.Now >= n.FechaInicio
                && DateTime.Now <= n.FechaFin)
            .AsEnumerable()
            .Where(n => n.FiltroRoles.Any(x => usuario.Roles.Any(y => y.Id == x.Id))

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
                LinkAdjunto = x.LinkAdjunto
            }).OrderByDescending(s => s.Id).ToList();

            if (usuario.Roles.Any(r => r.Codigo == "COMPRADOR" || r.Codigo == "SOLP"))
            {
                var rol = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "SOLP" : "COMPRADOR";
                var peticiones = repositorio.Listar<PeticionDeOfertaSolpPosicion, int>(peti => peti.SolpPosicion.Solp_Id, x => x.PeticionDeOferta.UsuarioCreador_Id == usuario.Id).ToList();
                List<int> solpIds = peticiones.Distinct().ToList();
                var chatSinLeer = repositorio.Listar<ChatInternoCompras, NotificacionDto>(
                    x => new NotificacionDto
                    {
                        Id = x.Solp_Id,
                        Mensaje = "Tiene un mensaje sin leer de la SOLP #" + x.Solp.NroSolp
                    },
                    x =>
                    (solpIds.Contains(x.Solp_Id) || x.Solp.UsuarioCreacion_Id == usuario.Id || x.Solp.UsuarioModificacion_Id == usuario.Id)
                    //&& x.Usuario_Id != usuario.Id
                    && x.Usuario.Roles.Any(r => r.Codigo == rol)
                    && x.Leido == false
                ).Distinct().ToList();

                listado.AddRange(chatSinLeer);
            }


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
                HoraInicio = notificacion.FechaInicio.Hour,
                FechaFin = notificacion.FechaFin?.ToString("dd/MM/yyyy"),
                Borrada = notificacion.Borrada,
                Habilitada = notificacion.Habilitada,
                FiltroRoles = notificacion.FiltroRoles.Select(r => r.Id).ToList(),
                Mensaje = notificacion.Mensaje,
                LinkAdjunto = notificacion.LinkAdjunto
            };

            return notificacionDto;

            //return repositorio.Obtener<Operador, OperadorDto>(x => x.Id == id, x => new OperadorDto { Id = x.Id, Descripcion = x.Descripcion }) ?? new OperadorDto();
        }
    }
}
