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
        public void ActualizarEstado(ActualizarProgresoReqDto actualizarCursoReq)
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

            switch (actualizarCursoReq.NuevoEstado)
            {
                case EstadoCursoEnum.Iniciado:

                    progreso.FechaInicio = DateTime.Now;
                    break;
                case EstadoCursoEnum.EnProgreso:
                    progreso.DetalleProgreso = actualizarCursoReq.DatosProgreso;
                    progreso.FechaUltimoIntento = DateTime.Now;
                    break;
                case EstadoCursoEnum.Completado:
                    progreso.DetalleProgreso = actualizarCursoReq.DatosProgreso;
                    progreso.FechaCompletado = DateTime.Now;
                    break;
            }
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

        public string ObtenerProgreso(int cursoId, string emailUsuario)
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

            return progreso.DetalleProgreso;
        }
        public Dictionary<string, bool> Asignar(AsignarReqDto asignarReqDto)
        {
            var usuarios = repositorio.Listar<Usuario>(usuario => asignarReqDto.MailsUsuarios.Contains(usuario.Mail));
            var resultados = new Dictionary<string, bool>();
            foreach (var usuario in usuarios)
            {

                var progresoExistente = ObtenerProgreso(asignarReqDto.CursoId, usuario);
                var asignado = progresoExistente != null;
                if (progresoExistente == null)
                {
                    usuario.ProgresoCursosAsignados.Add(
                        new ProgresoCurso
                        {
                            Alumno = usuario,
                            CursoId = asignarReqDto.CursoId,
                        });
                    asignado = true;
                }
                if (!usuario.TienePermiso(PermisoEnum.RealizarCursos))
                {
                    usuario.Roles.Add(RolAlumno());
                }
                resultados.Add(usuario.Mail, asignado);
            }
            repositorio.GuardarCambios();
            return resultados;
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
            return usuario.ProgresoCursosAsignados.First(p => p.CursoId == cursoId);
        }
        private Rol RolAlumno()
        {
            return repositorio.Obtener<Rol>(r => r.Nombre == "ALUMNO CURSOS");
        }
    }
}
