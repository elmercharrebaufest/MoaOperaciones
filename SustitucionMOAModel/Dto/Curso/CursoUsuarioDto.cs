using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.Curso
{
    public class CursoUsuarioDto
    {
        public CursoUsuarioDto(Entities.ProgresoCurso progreso)
        {
            CursoId = progreso.CursoId;
            AccesoCurso = progreso.Curso.Acceso;
            NombreCurso = progreso.Curso.Nombre;
            EstadoCurso = progreso.EstadoEnum();
        }

        public int CursoId { get; set; }
        public string NombreCurso { get; set; }
        public string AccesoCurso { get; set; }
        public EstadoCursoEnum EstadoCurso { get; set; }
    }
}
