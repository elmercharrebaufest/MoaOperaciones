using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IConsultaService
    {
        void AgregarComentario(int consultaId, Comentario comentario);
        ConsultaDto ObtenerConsulta(int consultaId);
        void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId);
        void RecategorizarConsulta(int consultaId, int categoriaId);
    }
}
