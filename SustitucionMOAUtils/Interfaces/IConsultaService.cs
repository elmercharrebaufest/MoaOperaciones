using iTextSharp.text;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Consulta;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IConsultaService
    {
        AgregarConsultaResponseDto AgregarConsulta(Consulta consulta, Comentario comentario, HttpFileCollectionBase files);
        AgregarConsultaResponseDto AgregarConsultaInterna(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<DestinatarioDto> destinatarios);
        ComentarioDto AgregarComentario(int consultaId, ComentarioDto comentario, HttpFileCollectionBase files);
        string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files);
        ConsultaDto ObtenerConsulta(int consultaId);
        void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId);
        void RecategorizarConsulta(int consultaId, int categoriaId, int? subCategoria);
        List<CategoriaDto> ObtenerCategorias(Boolean? excluir, UsuarioDto usuario, Boolean? mostrarCategoriaInterno);
        List<CategoriaDto> ObtenerCategoriasInterno(Boolean? excluir, UsuarioDto usuario);
        List<EstadoConsultaDto> ObtenerEstados();
        List<SubCategoriaDto> ObtenerSubCategorias(UsuarioDto usuario);
        List<CausaConsultaDto> ObtenerCausas();
        string EnviarMailRecordatorio(int consultaId);
        ListaPaginada<ConsultaDto> ListarConsultas(int usuarioId, bool obtenerTodos, Paginacion paginacion, FiltrosConsultaDto filtro = null);
        string ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId, int? causaConsultaId);
        string ObtenerRutaArchivo(int archivoId);
        string RecordarComentario(int consultaId);
        string GenerarReclamoImpositivoPdf(ReclamoImpositivo reclamoImpositivo);
        List<MaterialDto> ObtenerMaterial(TablaSeccionMaterial tablaSeccionMaterial);
        string AnularConsulta(int consultaId, int usuarioId, string motivoRechazo);
        string ProcesarCM05(HttpFileCollectionBase archivos, string cuitProveedor, int? comentario_Id = null, bool esCargaInterna = false);
        void ReabrirConsulta(int consultaId, UsuarioDto usuarioActual);
        List<ConsultaDto> ListarConsultasSinPaginar(int usuarioId, bool obtenerTodos, FiltrosConsultaDto filtros);
        ConsultaDto ObtenerConsultaDisconformidad(string numeroCCPP, int usuarioId, bool obtenerTodos);
        List<ConsultaDto> ObtenerConsultasPorProveedor(int usuarioId, string vendedor, bool obtenerTodos);
    }
}
