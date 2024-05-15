using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.Curso;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class CursoService : ICursoService
    {
        private readonly string ERROR_SIN_ACCESO = "Sin acceso a este curso.";
        private IRepositorio repositorio { get; set; }
        public CursoService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public void ActualizarProgreso(ActualizarProgresoReqDto actualizarCursoReq)
        {
            Usuario usuario;
            Curso curso;
            var tieneAcceso = ValidarAcceso(
                actualizarCursoReq,
                out usuario, out curso);

            if (!tieneAcceso)
            {
                throw new ValidationCustomException(ERROR_SIN_ACCESO);
            }
            var progreso = ObtenerProgreso(usuario, curso);

            if (progreso == null)
            {
                throw new ValidationCustomException(ERROR_SIN_ACCESO);
            }
            var fechaActualizacion = DateTime.Now;
            switch (actualizarCursoReq.NuevoEstado)
            {
                case EstadoCursoEnum.Iniciado:

                    progreso.FechaInicio = fechaActualizacion;
                    progreso.MinutosCursados = 0;
                    break;
                case EstadoCursoEnum.EnProgreso:
                    progreso.MinutosCursados += string.IsNullOrEmpty(actualizarCursoReq.TiempoSesion) ? 0 : DataFormatter.ParseMinutes(actualizarCursoReq.TiempoSesion);
                    progreso.DetalleProgreso = actualizarCursoReq.DatosProgreso;
                    progreso.FechaUltimoIntento = fechaActualizacion;
                    break;
                case EstadoCursoEnum.Completado:
                    progreso.MinutosCursados += string.IsNullOrEmpty(actualizarCursoReq.TiempoSesion) ? 0 : DataFormatter.ParseMinutes(actualizarCursoReq.TiempoSesion);
                    if (progreso.MinutosCursados < curso.MinimosMinutosCursada)
                    {
                        throw new ValidationCustomException("El tiempo de cursada es demasiado corto");
                    }
                    progreso.DetalleProgreso = actualizarCursoReq.DatosProgreso;
                    progreso.FechaCompletado = fechaActualizacion;
                    break;
            }
            repositorio.GuardarCambios();
        }

        public List<CursoUsuarioDto> AsignadosAUsuario(string emailUsuario)
        {
            var usuario = UsuarioPorMail(emailUsuario);
            var asignados = usuario.ProgresoCursosAsignados
                    .Select(c => new CursoUsuarioDto(c))
                    .ToList();
            if (asignados.Count == 0)
            {
                throw new InfoCustomException("No tiene cursos asignados.");
            }
            return asignados;
        }

        public List<CursoDto> Disponibles()
        {
            var disponibles = repositorio.Listar<Curso, CursoDto>(
                    c => new CursoDto { Id = c.Id, Nombre = c.Nombre }
                );
            if (disponibles.Count == 0)
            {
                throw new InfoCustomException("No hay cursos disponibles.");
            }
            return disponibles;
        }

        public ProgresoResDto ObtenerProgreso(int cursoId, string emailUsuario)
        {
            Usuario usuario;
            Curso curso;
            var tieneAcceso = ValidarAcceso(emailUsuario, cursoId, out usuario, out curso);
            if (!tieneAcceso)
            {
                throw new ValidationCustomException(ERROR_SIN_ACCESO);
            }
            var progreso = ObtenerProgreso(cursoId, usuario);

            if (progreso == null)
            {
                throw new ValidationCustomException(ERROR_SIN_ACCESO);
            }
            var detalleProgreso = progreso.DetalleProgreso ?? "";
            return new ProgresoResDto
            {
                DetalleProgreso = detalleProgreso.TrimEnd(),
                TiempoSesion = progreso.MinutosCursados > 0 ? DataFormatter.FormatMinutes(progreso.MinutosCursados) : "0000:00:00.00"
            };
        }
        public List<AsignarAlumnosResDto> Asignar(AsignarReqDto asignarReqDto)
        {
            var usuarios = repositorio.Listar<Usuario>(usuario => asignarReqDto.MailsUsuarios.Contains(usuario.Mail));
            var resultados = new List<AsignarAlumnosResDto>();
            foreach (var usuario in usuarios)
            {

                var progresoExistente = ObtenerProgreso(asignarReqDto.CursoId, usuario);
                var asignado = progresoExistente != null;
                try
                {
                    if (progresoExistente == null)
                    {
                        usuario.ProgresoCursosAsignados.Add(
                            new ProgresoCurso(asignarReqDto.CursoId, usuario));
                        asignado = true;
                    }
                    if (!usuario.TienePermiso(PermisoEnum.RealizarCursos))
                    {
                        usuario.Roles.Add(RolAlumno());
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error(e);
                }

                resultados.Add(new AsignarAlumnosResDto { Mail = usuario.Mail, Resultado = asignado });
            }
            repositorio.GuardarCambios();
            return resultados;
        }

        public List<ProgresoAlumnoEnCursoDto> ObtenerProgresoAlumnos(int cursoId)
        {
            var curso = ObtenerCurso(cursoId);
            //Tomamos primer progreso de cada usuario
            var progresos = curso.ProgresosDelCurso.GroupBy(x => x.UsuarioId);

            return progresos.Select(grouping => new ProgresoAlumnoEnCursoDto(grouping.First())).ToList();
        }

        public ProgresoAlumnoEnCursoDto ObtenerProgresoAlumno(int cursoId, string emailUsuario)
        {
            Curso curso;
            Usuario usuario;
            var cursoValido = ValidarAcceso(emailUsuario, cursoId, out usuario, out curso);

            if (!cursoValido)
            {
                throw new ValidationCustomException(ERROR_SIN_ACCESO);
            }

            return curso.ProgresosDelCurso
                .Where(progreso => progreso.UsuarioId == usuario.Id)
                .Select(progreso => new ProgresoAlumnoEnCursoDto(progreso))
                .First();
        }
        private bool ValidarAcceso(ActualizarProgresoReqDto actualizarProgresoReqDto, out Usuario usuario, out Curso curso)
        {
            return ValidarAcceso(actualizarProgresoReqDto.EmailUsuario, actualizarProgresoReqDto.CursoId, out usuario, out curso);
        }
        private bool ValidarAcceso(string emailUsuario, int cursoId, out Usuario usuario, out Curso curso)
        {
            usuario = UsuarioPorMail(emailUsuario);
            curso = ObtenerCurso(cursoId);
            return ValidarAcceso(usuario, curso);
        }

        private bool ValidarAcceso(Usuario usuario, Curso curso)
        {
            return usuario.ProgresoCursosAsignados
                    .Any(progreso => progreso.CursoId == curso.Id);
        }
        private Curso ObtenerCurso(int cursoId)
        {
            return repositorio.Obtener<Curso>(cursoId);
        }
        private Usuario UsuarioPorMail(string email)
        {
            return repositorio.Obtener<Usuario>(x => x.Mail == email);
        }
        private ProgresoCurso ObtenerProgreso(Usuario usuario, Curso curso)
        {
            var progresoExistente = repositorio.Obtener<ProgresoCurso>(pu => pu.UsuarioId == usuario.Id && pu.CursoId == curso.Id);
            return progresoExistente;
        }
        private ProgresoCurso ObtenerProgreso(int cursoId, Usuario usuario)
        {
            if (usuario.ProgresoCursosAsignados == null || usuario.ProgresoCursosAsignados.Count == 0)
            {
                return null;
            }
            return usuario.ProgresoCursosAsignados.First(p => p.CursoId == cursoId);
        }
        private Rol RolAlumno()
        {
            return repositorio.Obtener<Rol>(r => r.Nombre == "ALUMNO CURSOS");
        }
    }
}
