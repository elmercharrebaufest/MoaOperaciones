using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;

namespace SustitucionMOAModel.Dto.Curso
{
    public class ProgresoAlumnoEnCursoDto
    {
        public EstadoCursoEnum EstadoCurso { get; set;}
        public string EstadoDescripcion { get { 
                return EstadoCurso.ToString();
        } }
        public string MailAlumno { get; set;}
        public DateTime? FechaUltimoIntento { get; set;}
        public DateTime? FechaCompletado { get; set; }
        public ProgresoAlumnoEnCursoDto(ProgresoCurso progreso)
        {
            EstadoCurso = progreso.EstadoEnum();
            MailAlumno = progreso.Alumno.Mail;
            FechaUltimoIntento = progreso.FechaUltimoIntento;
            FechaCompletado = progreso.FechaCompletado;
        }
    }
}
