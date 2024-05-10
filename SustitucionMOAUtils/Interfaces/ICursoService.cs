using SustitucionMOAModel.Dto.Curso;
using System.Collections.Generic;


namespace SustitucionMOAUtils.Interfaces
{
    public interface ICursoService
    {
        void ActualizarEstado(ActualizarProgresoReqDto actualizarCursoReq);
        List<CursoUsuarioDto> AsignadosAUsuario(string emailUsuario);
        Dictionary<string, bool> Asignar(AsignarReqDto asignarReqDto);
        List<CursoDto> Disponibles();
        string ObtenerProgreso(int cursoId, string emailUsuario);
    }
}
