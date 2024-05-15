using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.Curso
{
    public class ProgresoAlumnoEnCursoDto
    {
        public EstadoCursoEnum EstadoCurso { get; set;}
        public string EstadoDescripcion { get { 
                return EstadoCurso.ToDescription();
        } }
        public string MailAlumno { get; set;}
        public string FechaIniciado { get; set; }
        public string FechaUltimoIntento { get; set;}
        public string FechaCompletado { get; set; }
        public ProgresoAlumnoEnCursoDto(ProgresoCurso progreso)
        {
            EstadoCurso = progreso.EstadoEnum();
            MailAlumno = progreso.Alumno.Mail;
            FechaIniciado = progreso.FechaInicio?.ToString("dd-MM-yyyy");
            FechaUltimoIntento = progreso.FechaUltimoIntento?.ToString("dd-MM-yyyy");
            FechaCompletado = progreso.FechaCompletado?.ToString("dd-MM-yyyy");
        }
    }
}
