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

        private readonly string rutaArchivosConsulta = ConfigurationManager.AppSettings["RutaArchivosConsulta"];

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
        public ConsultaDto AgregarConsulta(Consulta consulta)
        {
            consulta.Id = -1;
            consulta.Detalle.Id = -1;
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;
            consulta.EstadoConsulta_Id = 1;

            repositorio.Agregar(consulta);
            repositorio.GuardarCambios();

            return ObtenerConsulta(consulta.Id);
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
            includes.Add(x => x.Comentarios.Select(y => y.Archivos));
            includes.Add(x => x.Categoria);
            includes.Add(x => x.SubCategoria);
            includes.Add(x => x.EstadoConsulta);
            includes.Add(x => x.Detalle.CausaConsulta);

            var c = repositorio.Obtener<Consulta>(includes, y=> y.Id == consultaId);

            var ret = new ConsultaDto(c);
            ret.Comentarios = c.Comentarios.Select(x => new ComentarioDto(x)).ToList();

            return ret;
        }

        public string ObtenerRutaArchivo(int archivoId)
        {
            var archivo = repositorio.Obtener<Archivo>(archivoId);

            return archivo?.Ruta;
        }

        public List<ConsultaDto> ListarConsultas(int usuarioId, bool obtenerTodos)
        {
            var ret = new List<ConsultaDto>();

            var includes = new List<Expression<Func<Consulta, object>>>();
            includes.Add(x => x.Detalle);
            includes.Add(x => x.Detalle.CausaConsulta);
            includes.Add(x => x.Categoria);
            includes.Add(x => x.SubCategoria);
            includes.Add(x => x.EstadoConsulta);

            var usuario = repositorio.Obtener<Usuario>(usuarioId);
            var categorias = usuario.Roles.Where(x => x.Categorias.Any()).SelectMany(x => x.Categorias).Select(x => x.Id).ToList();
            
            ret = repositorio.Listar<Consulta>(x=> (obtenerTodos && categorias.Contains(x.Categoria.Id)) || x.Usuario_Id == usuarioId , includes: includes).Select(x => new ConsultaDto(x)).ToList();

            return ret;
        }

        public void RecategorizarConsulta(int consultaId, int categoriaId, int? subCategoriaId)
        {
            var categoria = repositorio.Obtener<Categoria>(c => c.Id == categoriaId);

            if (subCategoriaId.HasValue)
            {
                var subCategoria = repositorio.Obtener<SubCategoria>(c => c.Id == subCategoriaId);
                if (subCategoria == null) throw new InfoCustomException("No existe la subcategoria");
            }

            if (categoria == null) throw new InfoCustomException("No existe la categoria");
            
            var consulta = GetConsulta(consultaId);

            consulta.Categoria_Id = categoriaId;
            consulta.SubCategoria_Id = subCategoriaId;

            repositorio.GuardarCambios();
        }

        public string ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId)
        {
            var categoria = repositorio.Obtener<Categoria>(c => c.Id == categoriaId);
            var estado = repositorio.Obtener<EstadoConsulta>(c => c.Id == estadoConsultaId);

            if (subcategoriaId.HasValue)
            {
                var subCategoria = repositorio.Obtener<SubCategoria>(c => c.Id == subcategoriaId);
                if (subCategoria == null) throw new InfoCustomException("No existe la subcategoria");
            }

            if (estado == null) throw new InfoCustomException("No existe el estado");
            if (categoria == null) throw new InfoCustomException("No existe la categoria");

            var consulta = GetConsulta(consultaId);

            consulta.Categoria_Id = categoriaId;
            consulta.EstadoConsulta_Id = estadoConsultaId;
            consulta.SubCategoria_Id = subcategoriaId;

            repositorio.GuardarCambios();

            return "Se guardo correctamente.";
        }

        public string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files)
        {
            var comentario = repositorio.Obtener<Comentario>(comentarioId);

            if (comentario == null) throw new InfoCustomException("No existe el comentario");
            if (comentario.Consulta_Id != consultaId) throw new InfoCustomException("El comentario no corresponde a la consulta especificada");

            var errores = new List<string>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var fileName = string.Format("{0}_{1}", comentario.Id, Path.GetFileName(file.FileName));

                var ruta = ArmarRutaCarpeta(comentario); // $"{ConfigurationManager.AppSettings["RutaArchivosProveedores"]}/{proveedor.CUIT}/{proveedor.Id}/{FileKeys.Consultas}/{consultaId}";
                var rutaArchivo = string.Concat(ruta, "/", fileName);

                if (File.Exists(rutaArchivo))
                {
                    errores.Add($"{fileName}: {ErrorMsg.ErrorArchivoRepetido}");
                    continue;
                }

                Directory.CreateDirectory(ruta);

                comentario.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.Consultas,
                    Ruta = rutaArchivo
                });

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

        public List<CausaConsultaDto> ObtenerCausas()
        {
            try
            {
                var causas = repositorio.Listar<CausaConsulta>();
                return causas.Select(x => new CausaConsultaDto(x)).ToList();
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

        private string ArmarRutaCarpeta(Comentario comentario)
        {
            return string.Format("{0}/{1}/{2}", rutaArchivosConsulta, comentario.Consulta.Usuario_Id, comentario.Consulta_Id);
        }
    }
}
