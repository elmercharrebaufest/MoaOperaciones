using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.Curso
{
    public class ActualizarProgresoReqDto
    {
        public int CursoId { get; set; }
        public EstadoCursoEnum NuevoEstado { get; set; }
        public string EmailUsuario { get; set; }
        public string DatosProgreso { get; set; }
    }
}
