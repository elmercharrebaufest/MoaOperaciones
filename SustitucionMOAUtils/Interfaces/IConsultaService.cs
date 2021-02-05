using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IConsultaService
    {
        ComentarioDto AgregarComentario(int consultaId, Comentario comentario);
        string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files);
        ConsultaDto ObtenerConsulta(int consultaId);
        void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId);
        void RecategorizarConsulta(int consultaId, int categoriaId);
    }
}
