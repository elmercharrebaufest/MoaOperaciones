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
            var estado = repositorio.Obtener<EstadoConsulta>(c => c.Id == estadoConsultaId);

            if (estado == null) throw new InfoCustomException("No existe el estado");

            var consulta = GetConsulta(consultaId);

            consulta.EstadoConsulta_Id = estadoConsultaId;
            repositorio.GuardarCambios();
        }

        public ComentarioDto AgregarComentario(int consultaId, Comentario comentario, Boolean esInterno, HttpFileCollectionBase files)
        {
            var consulta = GetConsulta(consultaId);

            if(esInterno && consulta.EstadoConsulta.Code == "GES")
            {
                EstadoConsulta estado = repositorio.Obtener<EstadoConsulta>(e => e.Code == "DOC");
                ActualizarEstadoConsulta(consultaId, estado.Id);
            }
            if (!esInterno && consulta.EstadoConsulta.Code == "DOC")
            {
                EstadoConsulta estado = repositorio.Obtener<EstadoConsulta>(e => e.Code == "GES");
                ActualizarEstadoConsulta(consultaId, estado.Id);
            }

            if(string.IsNullOrWhiteSpace(comentario.Detalle))
            {
                comentario.Detalle = "";
            }

            consulta.Comentarios.Add(comentario);
            repositorio.GuardarCambios();

            if (files.Count > 0)
            {
                AgregarAdjuntoComentario(consulta.Id, comentario.Id, files);
            }

            return new ComentarioDto(comentario);
        }
        public ConsultaDto AgregarConsulta(Consulta consulta, Comentario comentario, HttpFileCollectionBase files)
        {
            consulta.Id = -1;
            consulta.Detalle.Id = -1;
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;
            consulta.EstadoConsulta_Id = 1;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);
            SubCategoria subcatecategoria = repositorio.Obtener<SubCategoria>(s => s.Id == consulta.SubCategoria_Id);
            Usuario usuario = repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);


            if(usuario.TipoUsuario.NombreCorto != "CORR")
            {
                if(categoria.Code == "ACT" && subcatecategoria.Code == "CAP")
                {
                    throw new InfoCustomException("Tiene que ser corredor para consultar sobre Carta de Presentacion.");
                }
            }
            else
            {
                if (categoria.Code == "ACT" && subcatecategoria.Code == "INF")
                {
                    throw new InfoCustomException("Tiene que ser proveedor directo para consultar sobre Informe Comercial.");
                }
            }

            if (categoria.Code == "FIN")
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINCOR").Id;
                }
                else
                {
                    consulta.Categoria_Id = consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINDIR").Id; ;
                }
            }

            if (categoria.Code == "PAR")
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARCOR").Id;
                }
                else
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARDIR").Id;
                }
            }

            comentario.Usuario_Id = consulta.Usuario_Id;

            if (consulta.Comentarios == null)
            {
                consulta.Comentarios = new List<Comentario>();
            }

            consulta.Comentarios.Add(comentario);

            repositorio.Agregar(consulta);
            repositorio.GuardarCambios();

            if(files.Count > 0) 
            {
                Comentario primerComentario = repositorio.Obtener<Comentario>(c => c.Consulta_Id == consulta.Id);
                AgregarAdjuntoComentario(consulta.Id, primerComentario.Id, files);
            }

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
            
            ret = repositorio.Listar<Consulta>(x=> (obtenerTodos && categorias.Contains(x.Categoria.Id)) || x.Usuario_Id == usuarioId , includes: includes)
                .Select(x => new ConsultaDto 
                {
                    Id = x.Id,
                    Asunto = x.Asunto,
                    CodigoCorredor = x.CodigoCorredor,
                    RazonSocialCorredor = x.RazonSocialCorredor,
                    CodigoProveedor = x.CodigoProveedor,
                    RazonSocialProveedor = x.RazonSocialProveedor,
                    CategoriaId = x.Categoria_Id,
                    Categoria = new CategoriaDto 
                        { 
                            Id = x.Categoria.Id,
                            Code = x.Categoria.Code,
                            Nombre = x.Categoria.Nombre
                        },
                    SubCategoriaId = x.SubCategoria_Id != null? x.SubCategoria_Id : 0,
                    SubCategoria = x.SubCategoria != null? new SubCategoriaDto 
                        {
                            Id = x.SubCategoria.Id,
                            Code = x.SubCategoria.Code,
                            Nombre = x.SubCategoria.Nombre,
                            CategoriaId = x.SubCategoria.Categoria_Id
                        } : new SubCategoriaDto { Nombre = "" },
                    EstadoConsultaId = x.EstadoConsulta_Id,
                    EstadoConsulta = new EstadoConsultaDto 
                        {
                            Id = x.EstadoConsulta.Id,
                            Descripcion = x.EstadoConsulta.Descripcion,
                            Color = x.EstadoConsulta.Color,
                            Code = x.EstadoConsulta.Code
                        },
                    FechaCreacion = x.FechaCreacion,
                    FechaUltimaModificacion = x.FechaUltimaModificacion,
                    UsuarioId = x.Usuario_Id,
                    Fecha = x.Detalle != null? x.Detalle.Fecha : null,
                    ComprobanteNo = x.Detalle != null? x.Detalle.ComprobanteNo : "",
                    OtroComprobanteNo = x.Detalle != null? x.Detalle.OtroComprobanteNo : "",
                    ContratoNo = x.Detalle != null? x.Detalle.ContratoNo : "",
                    Importe = x.Detalle != null? x.Detalle.Importe : null,
                    Impuesto = x.Detalle != null? x.Detalle.Impuesto : null,
                    BolsaEmisoraOblea = x.Detalle != null? x.Detalle.BolsaEmisoraOblea : "",
                    CausaConsultaId = x.Detalle.CausaConsulta != null? x.Detalle.CausaConsulta_Id : null,
                    CausaConsulta = x.Detalle.CausaConsulta != null? new CausaConsultaDto
                        {
                            Id = x.Detalle.CausaConsulta.Id,
                            Nombre = x.Detalle.CausaConsulta.Nombre
                        } : null
                }).ToList();

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

        public string ActualizarCombos(int consultaId, int estadoConsultaId, int categoriaId, int? subcategoriaId, int? causaConsultaId)
        {
            var categoria = repositorio.Obtener<Categoria>(c => c.Id == categoriaId);
            var estado = repositorio.Obtener<EstadoConsulta>(c => c.Id == estadoConsultaId);

            if (subcategoriaId.HasValue && subcategoriaId != 0)
            {
                var subCategoria = repositorio.Obtener<SubCategoria>(c => c.Id == subcategoriaId);
                if (subCategoria == null) throw new InfoCustomException("No existe la subcategoria");
            }

            if (estado == null) throw new InfoCustomException("No existe el estado");
            if (categoria == null) throw new InfoCustomException("No existe la categoria");

            var consulta = GetConsulta(consultaId);

            consulta.Categoria_Id = categoriaId;
            consulta.EstadoConsulta_Id = estadoConsultaId;
            if(subcategoriaId != 0) 
            {
                consulta.SubCategoria_Id = subcategoriaId;
            }

            if (causaConsultaId.HasValue && causaConsultaId != 0) 
            {
                var causaConsulta = repositorio.Obtener<CausaConsulta>(cc => cc.Id == causaConsultaId);
                if(causaConsulta == null) throw new InfoCustomException("No existe la causa de consulta");
                consulta.Detalle.CausaConsulta_Id = causaConsultaId;
            }

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

                if(comentario.Archivos == null)
                {
                    comentario.Archivos = new List<Archivo>();
                }

                comentario.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.Consultas,
                    Ruta = rutaArchivo,
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
                List<string> exclude = new List<string>() { "PARDIR", "PARCOR", "FINDIR", "FINCOR" };
                var categorias = repositorio.Listar<Categoria>(c => !exclude.Contains(c.Code));
                return categorias.Select(x =>new CategoriaDto(x)).ToList();
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
