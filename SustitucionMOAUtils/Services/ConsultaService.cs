using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IRepositorio repositorio;

        public ConsultaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId)
        {
            var estado = repositorio.Obtener<Consulta>(c => c.Id == estadoConsultaId);

            if (estado == null) throw new InfoCustomException("No existe el estado");

            var consulta = GetConsulta(consultaId);

            consulta.EstadoConsulta_Id = estadoConsultaId;
            repositorio.GuardarCambios();
        }

        public ComentarioDto AgregarComentario(int consultaId, Comentario comentario)
        {
            var consulta = GetConsulta(consultaId);

            consulta.Comentarios.Add(comentario);
            repositorio.GuardarCambios();

            return new ComentarioDto(comentario);
        }

        private Consulta GetConsulta(int consultaId)
        {
            var consulta = repositorio.Obtener<Consulta>(consultaId);

            if (consulta == null) throw new InfoCustomException("No existe la consulta");

            return consulta;
        }

        public ConsultaDto ObtenerConsulta(int consultaId)
        {
            var includes = new List<Expression<Func<Consulta, object>>>();
            includes.Add(x => x.Comentarios);
            //includes.Add(x => x.Comentarios.Select(y => y.Archivos));
            includes.Add(x => x.Categoria);
            includes.Add(x => x.EstadoConsulta);

            var c = repositorio.Obtener<Consulta>(includes, y=> y.Id == consultaId);

            return new ConsultaDto
            {
                Id = c.Id,
                Asunto = c.Asunto,
                Categoria = new CategoriaDto(c.Categoria),
                Comentarios = c.Comentarios.Select(comentario => new ComentarioDto(comentario)).ToList(),
                EstadoConsulta = new EstadoConsultaDto(c.EstadoConsulta),
                FechaCreacion = c.FechaCreacion,
                FechaUltimaModificacion = c.FechaUltimaModificacion,
            };
        }

        public List<ConsultaDto> ListarConsultas(string email)
        {
            var ret = new List<ConsultaDto>();

            ret = repositorio.Listar<Consulta>().Select(x =>
            new ConsultaDto()
            {
                
            }).ToList();

            return ret;
        }

        public void RecategorizarConsulta(int consultaId, int categoriaId)
        {
            var categoria = repositorio.Obtener<Categoria>(c => c.Id == categoriaId);

            if(categoria == null) throw new InfoCustomException("No existe la categoria");
            
            var consulta = GetConsulta(consultaId);

            consulta.Categoria_Id = categoriaId;
            repositorio.GuardarCambios();
        }

        public string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files)
        {
            var comentario = repositorio.Obtener<Comentario>(comentarioId);

            if (comentario == null) throw new InfoCustomException("No existe el comentario");
            if (comentario.Consulta_Id != consultaId) throw new InfoCustomException("El comentario no corresponde a la consulta especificada");

            var errores = new List<string>();
            var consulta = repositorio.Obtener<Consulta>(c => c.Id == consultaId);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileName(file.FileName);
                var ruta = ""; // $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{proveedor.CUIT}/{proveedor.Id}/{FileKeys.Consultas}/{consultaId}";
                var rutaArchivo = string.Concat(ruta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    errores.Add($"{fileName}: {ErrorMsg.ErrorArchivoRepetido}");
                    continue;
                }

                Directory.CreateDirectory(ruta);

                comentario.Archivos.Add(new Archivo { FileKey = FileKeys.Consultas, Ruta = rutaArchivo });

                file.SaveAs(rutaArchivo);
                repositorio.GuardarCambios();
            }

            return errores.Any() ? string.Join(".", errores) : SuccessMsg.ArchivoSubidoOK;
        }

        public List<CategoriaDto> ObtenerCategorias()
        {
            try
            {
                var categorias = repositorio.Listar<Categoria>();
                return categorias.Select(x => new CategoriaDto(x)).ToList();
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public List<SubCategoriaDto> ObtenerSubCategorias()
        {
            try
            {
                var subcategorias = repositorio.Listar<SubCategoria>();
                return subcategorias.Select(x => new SubCategoriaDto(x)).ToList();
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public List<EstadoConsultaDto> ObtenerEstados()
        {
            try
            {
                var estados = repositorio.Listar<EstadoConsulta>();
                return estados.Select(x => new EstadoConsultaDto(x)).ToList();
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
