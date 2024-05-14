using SustitucionMOAModel.Dto.Curso;
using System.Collections.Generic;


namespace SustitucionMOAUtils.Interfaces
{
    public interface ICursoService
    {
        void ActualizarProgreso(ActualizarProgresoReqDto actualizarCursoReq);
        List<CursoUsuarioDto> AsignadosAUsuario(string emailUsuario);
        List<AsignarAlumnosResDto> Asignar(AsignarReqDto asignarReqDto);
        List<CursoDto> Disponibles();
        string ObtenerProgreso(int cursoId, string emailUsuario);
    }
}
