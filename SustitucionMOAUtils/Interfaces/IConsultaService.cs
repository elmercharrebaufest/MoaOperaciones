using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IConsultaService
    {
        ConsultaDto AgregarConsulta(Consulta consulta, Comentario comentario, HttpFileCollectionBase files);
        ComentarioDto AgregarComentario(int consultaId, Comentario comentario, HttpFileCollectionBase files);
        string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files);
        ConsultaDto ObtenerConsulta(int consultaId);
        void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId);
        void RecategorizarConsulta(int consultaId, int categoriaId, int? subCategoria);
        List<CategoriaDto> ObtenerCategorias();
        List<EstadoConsultaDto> ObtenerEstados();
        List<SubCategoriaDto> ObtenerSubCategorias();
        List<CausaConsultaDto> ObtenerCausas();
        List<ConsultaDto> ListarConsultas(int usuarioId, bool obtenerTodos);
        string ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId, int? causaConsultaId);
        string ObtenerRutaArchivo(int archivoId);
        string RecordarComentario(int consultaId);
    }
}
