using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IRepositorio repositorio;

        private readonly string rutaArchivosConsulta = ConfigurationManager.AppSettings["RutaArchivosConsulta"];
        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "RespuestaConsulta.html");
        private static readonly string EMAIL_TEMPLATE_RECORDATORIO = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "RecordatorioComentario.html");

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

        public ComentarioDto AgregarComentario(int consultaId, Comentario comentario, HttpFileCollectionBase files)
        {
            var consulta = GetConsulta(consultaId);
            var usuario = repositorio.Obtener<Usuario>(u => u.Id == comentario.Usuario_Id);
            var esInterno = usuario.TienePermiso("CONSULTA ABM");

            if (esInterno && consulta.EstadoConsulta.Code == "GES")
            {
                EstadoConsulta estado = repositorio.Obtener<EstadoConsulta>(e => e.Code == "DOC");
                ActualizarEstadoConsulta(consultaId, estado.Id);
                var copia = new List<String>();
                EnviarMailRespuesta(consulta, copia, comentario.Detalle);
            }
            if (!esInterno && consulta.EstadoConsulta.Code == "DOC")
            {
                EstadoConsulta estado = repositorio.Obtener<EstadoConsulta>(e => e.Code == "GESRTA");
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

            var ret = new ConsultaDto()
            {
                Id = c.Id,
                Asunto = c.Asunto,
                CodigoCorredor = c.CodigoCorredor,
                RazonSocialCorredor = c.RazonSocialCorredor,
                CodigoProveedor = c.CodigoProveedor,
                RazonSocialProveedor = c.RazonSocialProveedor,
                CategoriaId = c.Categoria_Id,
                Categoria = new CategoriaDto
                {
                    Id = c.Categoria.Id,
                    Code = c.Categoria.Code,
                    Nombre = c.Categoria.Nombre
                },
                SubCategoriaId = c.SubCategoria_Id != null ? c.SubCategoria_Id : 0,
                SubCategoria = c.SubCategoria != null ? new SubCategoriaDto
                {
                    Id = c.SubCategoria.Id,
                    Code = c.SubCategoria.Code,
                    Nombre = c.SubCategoria.Nombre,
                    CategoriaId = c.SubCategoria.Categoria_Id
                } : new SubCategoriaDto { Nombre = "" },
                EstadoConsultaId = c.EstadoConsulta_Id,
                EstadoConsulta = new EstadoConsultaDto
                {
                    Id = c.EstadoConsulta.Id,
                    Descripcion = c.EstadoConsulta.Descripcion,
                    Color = c.EstadoConsulta.Color,
                    Code = c.EstadoConsulta.Code
                },
                FechaCreacion = c.FechaCreacion,
                FechaUltimaModificacion = c.FechaUltimaModificacion,
                UsuarioId = c.Usuario_Id,
                Usuario = new UsuarioDto()
                {
                    Id = c.Usuario.Id,
                    CodigoProveedor = c.Usuario.ObtenerCodigoProveedor(),
                    CUIT = c.Usuario.CUITRegistro,
                    Mail = c.Usuario.Mail
                },
                Fecha = c.Detalle != null ? c.Detalle.Fecha : null,
                ComprobanteNo = c.Detalle != null ? c.Detalle.ComprobanteNo : "",
                OtroComprobanteNo = c.Detalle != null ? c.Detalle.OtroComprobanteNo : "",
                ContratoNo = c.Detalle != null ? FormatearStringNewLine(c.Detalle.ContratoNo) : "",
                Importe = c.Detalle != null ? c.Detalle.Importe : null,
                Impuesto = c.Detalle != null ? c.Detalle.Impuesto : null,
                BolsaEmisoraOblea = c.Detalle != null ? c.Detalle.BolsaEmisoraOblea : "",
                CausaConsultaId = c.Detalle.CausaConsulta != null ? c.Detalle.CausaConsulta_Id : null,
                CausaConsulta = c.Detalle.CausaConsulta != null ? new CausaConsultaDto
                {
                    Id = c.Detalle.CausaConsulta.Id,
                    Nombre = c.Detalle.CausaConsulta.Nombre
                } : null
            };

            ret.Comentarios = c.Comentarios.Select(x => new ComentarioDto()
            {
                Id = x.Id,
                Detalle = x.Detalle,
                Fecha = x.Fecha,
                UsuarioId = x.Usuario_Id,
                Usuario = new UsuarioDto()
                {
                    Id = x.Usuario.Id,
                    CodigoProveedor = x.Usuario.ObtenerCodigoProveedor(),
                    CUIT = x.Usuario.CUITRegistro,
                    Mail = x.Usuario.Mail
                },
                Archivos = x.Archivos.Select(a => new ArchivoDto()
                { 
                    Id = a.Id,
                    Ruta = a.Ruta,
                    FileKey = a.FileKey,
                    Nombre = a.ObtenerNombre(a.Ruta),
                }).ToList() 
            }).ToList();

            return ret;
        }

        private string FormatearStringNewLine(string dato)
        {
            dato = dato.Replace(",", "<br>");
            dato = dato.Replace("/", "<br>");
            dato = dato.Replace(" ", "<br>");

            dato = Regex.Replace(dato, @"(<br ?/?>)+", "<br>");

            return dato;
        }

        public string RecordarComentario(int consultaId)
        {
            var consulta = repositorio.Obtener<Consulta>(c => c.Id == consultaId);

            try
            {
                var copia = new List<string>();
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_RECORDATORIO);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Usuario.Mail, consulta.Id, consulta.Asunto);
                string asunto = "Molinos Agro - Respuesta sin leer en: " + consulta.Asunto;

                EmailSender.EnviarMail(new List<string> { consulta.Usuario.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return "Enviado Correctamente";
        }

        private void EnviarMailRespuesta(Consulta consulta, List<string> copia, string comentario)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(comentario) ? comentario : "-");
                string asunto = "Molinos Agro - Respuesta a su consulta N°: " + consulta.Id + " con asunto: " + consulta.Asunto;

                EmailSender.EnviarMail(new List<string> { consulta.Usuario.Mail }, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public string EnviarMailRecordatorio(int consultaId)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                Consulta consulta = GetConsulta(consultaId);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(consulta.Comentarios.Last().Detalle) ? consulta.Comentarios.Last().Detalle : "-");
                string asunto = "Molinos Agro - Respuesta a su consulta N°: " + consulta.Id + " con asunto: " + consulta.Asunto;

                EmailSender.EnviarMail(new List<string> { consulta.Usuario.Mail }, asunto, cuerpo, null, null, null, null);

                return "Mail enviado correctamente.";
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return "Error al enviar el Mail.";
            }
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
