using iTextSharp.text;
using iTextSharp.text.pdf;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.DesignPattern.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using System.ServiceModel.Channels;
using SustitucionMOAUtils.Helpers;

namespace SustitucionMOAUtils.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IRepositorio repositorio;
        private readonly IAzureService azureService;
        private readonly ITimeProvider timeProvider;
        private readonly IConsultaContext consultaContext;
        private readonly IGestionImpuestosService gestionImpuestosService;

        private readonly string rutaArchivosConsulta = ConfigurationManager.AppSettings["RutaArchivosConsulta"];
        private readonly string rutaMisConsultas = ConfigurationManager.AppSettings["UrlMisConsultas"];
        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "RespuestaConsulta.html");
        private readonly string rutaArchivosCM05 = ConfigurationManager.AppSettings["RutaArchivosCM05"];

        public ConsultaService(IRepositorio repositorio, IAzureService azureService, ITimeProvider timeProvider, IConsultaContext consultaContext, IGestionImpuestosService gestionImpuestosService)
        {
            this.repositorio = repositorio;
            this.azureService = azureService;
            this.timeProvider = timeProvider;
            this.consultaContext = consultaContext;
            this.gestionImpuestosService = gestionImpuestosService;
        }

        public virtual void ActualizarEstadoConsulta(int consultaId, int estadoConsultaId)
        {
            var estado = repositorio.Obtener<EstadoConsulta>(c => c.Id == estadoConsultaId);

            if (estado == null) throw new InfoCustomException("No existe el estado");

            var consulta = GetConsulta(consultaId);
            consulta.FechaUltimaModificacion = DateTime.Now;
            consulta.EstadoConsulta_Id = estadoConsultaId;

            repositorio.GuardarCambios();
        }

        public virtual ComentarioDto AgregarComentario(int consultaId, ComentarioDto comentarioDto, HttpFileCollectionBase files)
        {
            var consulta = GetConsulta(consultaId);

            Comentario comentario = new Comentario
            {
                Consulta_Id = consultaId,
                Detalle = comentarioDto.Detalle,
                Fecha = DateTime.Now,
                Recordado = comentarioDto.Recordado,
                FechaRecordado = comentarioDto.FechaRecordado,
                Usuario_Id = comentarioDto.UsuarioId
            };

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

            if (string.IsNullOrWhiteSpace(comentario.Detalle))
            {
                comentario.Detalle = "";
            }
            comentario.Fecha = DateTime.Now;
            consulta.Comentarios.Add(comentario);
            consulta.FechaUltimaModificacion = DateTime.Now;
            repositorio.GuardarCambios();

            if (files != null && files.Count > 0)
            {
                AgregarAdjuntoComentario(consulta.Id, comentario.Id, files);
            }

            return new ComentarioDto(comentario);
        }

        public AgregarConsultaResponseDto AgregarConsulta(Consulta consulta, Comentario comentario, HttpFileCollectionBase files)
        {
            string mensajeResultado = string.Empty;

            consulta.Id = -1;
            consulta.Detalle.Id = -1;
            consulta.FechaCreacion = DateTime.Now;
            consulta.FechaUltimaModificacion = DateTime.Now;
            consulta.EstadoConsulta_Id = 1;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);
            SubCategoria subcatecategoria = repositorio.Obtener<SubCategoria>(s => s.Id == consulta.SubCategoria_Id);
            Usuario usuario = repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);

            if (usuario.TipoUsuario.NombreCorto != "CORR")
            {
                if (categoria.Code == Categorias.Actualizacion && subcatecategoria.Code == SubCategorias.CartaPresentacón)
                {
                    throw new InfoCustomException("Tiene que ser corredor para consultar sobre Carta de Presentacion.");
                }
            }
            else
            {
                if (categoria.Code == Categorias.Actualizacion && subcatecategoria.Code == SubCategorias.InformeComercial)
                {
                    throw new InfoCustomException("Tiene que ser proveedor directo para consultar sobre Informe Comercial.");
                }
            }

            if (categoria.Code == Categorias.Final)
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINCOR").Id;
                    var subcategoriaCode = subcatecategoria.Code + "FC";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
                else
                {
                    consulta.Categoria_Id = consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINDIR").Id;
                    var subcategoriaCode = subcatecategoria.Code + "FD";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
            }

            if (categoria.Code == Categorias.Parcial)
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARCOR").Id;
                    var subcategoriaCode = subcatecategoria.Code + "PC";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
                else
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARDIR").Id;
                    var subcategoriaCode = subcatecategoria.Code + "PD";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
            }

            comentario.Usuario_Id = consulta.Usuario_Id;

            comentario.ComentarioRecordado = new List<ComentarioRecordado>();

            if (consulta.Comentarios == null)
            {
                consulta.Comentarios = new List<Comentario>();
            }

            consulta.Comentarios.Add(comentario);

            repositorio.Agregar(consulta);
            repositorio.GuardarCambios();

            if (files.Count > 0)
            {
                Comentario primerComentario = repositorio.Obtener<Comentario>(c => c.Consulta_Id == consulta.Id);
                AgregarAdjuntoComentario(consulta.Id, primerComentario.Id, files);

                if (categoria.Code == Categorias.Actualizacion && subcatecategoria.Code == SubCategorias.CM05)
                {
                    Proveedor proveedor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == consulta.CodigoProveedor);
                    try
                    {
                        mensajeResultado = this.ProcesarCM05(files, proveedor.CUIT, comentario.Id, false);
                        //mensajeResultado =    this.ProcesarCM05(files, comentario.Id, proveedor.CUIT);
                    }
                    catch (ValidationCustomException vex)
                    {
                        consulta.Comentarios.Add(new Comentario
                        {
                            Consulta_Id = consulta.Id,
                            Detalle = vex.Message,
                            Fecha = DateTime.Now,
                            Usuario_Id = usuario.Id
                        });

                        consulta.FechaUltimaModificacion = DateTime.Now;

                        repositorio.GuardarCambios();
                    }
                }
            }

            return new AgregarConsultaResponseDto
            {
                ConsultaDto = ObtenerConsulta(consulta.Id),
                Mensaje = mensajeResultado,
            };
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
            includes.Add(x => x.Usuario);

            var c = repositorio.Obtener<Consulta>(includes, y => y.Id == consultaId);

            var ret = new ConsultaDto()
            {
                Id = c.Id,
                Asunto = c.Asunto,
                CodigoCorredor = c.CodigoCorredor,
                RazonSocialCorredor = c.RazonSocialCorredor,
                CodigoProveedor = c.CodigoProveedor,
                RazonSocialProveedor = c.RazonSocialProveedor,
                CategoriaId = getIdCategoria(c.Categoria.Code, c.Categoria_Id),
                Categoria = new CategoriaDto
                {
                    Id = c.Categoria.Id,
                    Code = c.Categoria.Code,
                    Nombre = c.Categoria.Nombre
                },
                SubCategoriaId = c.SubCategoria_Id != null ? getIdSubcategoria(c.SubCategoria.Code, c.SubCategoria_Id) : 0,
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
                UsuarioInternoId = c.UsuarioInterno_Id,
                FechaVtoReapertura = c.FechaVtoReapertura,
                Usuario = new UsuarioDto()
                {
                    Id = c.Usuario.Id,
                    CodigoProveedor = c.Usuario.ObtenerCodigoProveedor(),
                    CUIT = c.Usuario.CUITRegistro,
                    Mail = c.Usuario.Mail
                },
                Fecha = c.Detalle != null ? c.Detalle.Fecha : null,
                ComprobanteNo = c.Detalle != null ? FormatearStringNewLine(c.Detalle.ComprobanteNo) : "",
                OtroComprobanteNo = c.Detalle != null ? FormatearStringNewLine(c.Detalle.OtroComprobanteNo) : "",
                ContratoNo = c.Detalle != null ? FormatearStringNewLine(c.Detalle.ContratoNo) : "",
                Importe = c.Detalle != null ? c.Detalle.Importe : null,
                Impuesto = c.Detalle != null ? c.Detalle.Impuesto : null,
                BolsaEmisoraOblea = c.Detalle != null ? c.Detalle.BolsaEmisoraOblea : "",
                OrdenId = c.Detalle != null ? c.Detalle.Orden_Id : null,
                PatenteChasis = c.Detalle != null ? c.Detalle.PatenteChasis : null,
                CausaConsultaId = c.Detalle.CausaConsulta != null ? c.Detalle.CausaConsulta_Id : null,
                CausaConsulta = c.Detalle.CausaConsulta != null ? new CausaConsultaDto
                {
                    Id = c.Detalle.CausaConsulta.Id,
                    Nombre = c.Detalle.CausaConsulta.Nombre
                } : null,
                Rubro = c.Detalle.Rubro
            };

            ret.Comentarios = c.Comentarios.Select(x => new ComentarioDto()
            {
                Id = x.Id,
                Detalle = x.Detalle,
                Fecha = x.Fecha,
                UsuarioId = x.Usuario_Id,
                Recordado = x.Recordado,
                FechaRecordado = x.FechaRecordado,
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
                }).ToList(),
                ComentarioRecordados = x.ComentarioRecordado != null ? x.ComentarioRecordado.Select(cr => new ComentarioRecordadoDto()
                {
                    Id = cr.Id,
                    FechaRecordado = cr.FechaRecordado
                }).ToList() : new List<ComentarioRecordadoDto>()
            }).ToList();

            var i = 0;
            foreach (var item in ret.Comentarios)
            {
                i++;
                item.Detalle = item.Detalle.Replace("<img src=", "<img class=\"galeryimg col-md-12 cursor-pointer\" src=");
            }
            return ret;
        }

        private string FormatearStringNewLine(string dato)
        {
            if (dato == null)
            {
                return dato;
            }

            if (dato.Contains("/") || dato.Contains(",") || dato.Contains(" ") || dato.Contains(";"))
            {
                dato = dato.Replace(",", "<br>");
                dato = dato.Replace("/", "<br>");
                dato = dato.Replace(";", "<br>");
                dato = dato.Replace(" ", "<br>");

                dato = Regex.Replace(dato, @"(<br ?/?>)+", "<br>");
            }

            return dato;
        }

        private int? getIdSubcategoria(string code, int? id)
        {
            List<string> exclude = new List<string> { "NRORPD","NRORPC","NRORFD","PROFFD","SERVFD","BONFD","CDGFD",
                "NRORFC","PROFFC","SERVFC","BONFC","CDGFC" };

            if (exclude.Contains(code))
            {
                if (code.Contains("FD") || code.Contains("FC"))
                {
                    var categoria = repositorio.Obtener<Categoria>(c => c.Code == "FIN");
                    code = code.Replace("FD", "").Replace("FC", "");
                    id = repositorio.Obtener<SubCategoria>(sc => sc.Code == code && sc.Categoria_Id == categoria.Id).Id;
                }
                else
                {
                    code = code.Replace("PD", "").Replace("PC", "");
                    id = repositorio.Obtener<SubCategoria>(sc => sc.Code == code).Id;
                }
            }

            return id;
        }

        private int getIdCategoria(string code, int id = 0)
        {
            if (code.Contains("DIR"))
            {
                code = code.Replace("DIR", "");
                id = repositorio.Obtener<Categoria>(x => x.Code == code).Id;
            }

            if (code.Contains("COR"))
            {
                code = code.Replace("COR", "");
                id = repositorio.Obtener<Categoria>(x => x.Code == code).Id;
            }

            return id;
        }

        public string RecordarComentario(int consultaId)
        {
            try
            {
                var consulta = repositorio.Obtener<Consulta>(c => c.Id == consultaId);
                var copia = new List<string>();
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(consulta.Comentarios.Last().Detalle) ? consulta.Comentarios.Last().Detalle : "-", rutaMisConsultas);
                string asunto = "Molinos Agro - Recordatorio: Respuesta a su consulta N°: " + consulta.Id + " con asunto: " + consulta.Asunto;

                EmailSender.EnviarMail(new List<string> { consulta.Usuario.Mail }, asunto, cuerpo, copia, null, null, null);

                consulta.FechaUltimaModificacion = DateTime.Now;

                var comentarioRecordado = new ComentarioRecordado() { Id = -1, FechaRecordado = DateTime.Now };

                consulta.Comentarios.Last().Recordado = true;
                consulta.Comentarios.Last().ComentarioRecordado.Add(comentarioRecordado);
                repositorio.GuardarCambios();
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
                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(comentario) ? comentario : "-", rutaMisConsultas);
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

                var cuerpo = string.Format(cuerpoTemplate, consulta.Asunto, !string.IsNullOrWhiteSpace(consulta.Comentarios.Last().Detalle) ? consulta.Comentarios.Last().Detalle : "-", rutaMisConsultas);
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

            var includes = new List<Expression<Func<Consulta, object>>>();
            //includes.Add(x => x.Detalle);
            //includes.Add(x => x.Detalle.CausaConsulta);
            //includes.Add(x => x.Categoria);
            //includes.Add(x => x.SubCategoria);
            //includes.Add(x => x.EstadoConsulta);

            var usuario = repositorio.Obtener<Usuario>(usuarioId);
            var esInterno = usuario.TienePermiso(PermisoEnum.ConsultaAbm);
            var categorias = usuario.Roles.Where(x => x.Categorias.Any()).SelectMany(x => x.Categorias).Select(x => x.Id).ToList();

            var ret = repositorio.Listar<Consulta>(x => (obtenerTodos && categorias.Contains(x.Categoria.Id)) || x.Usuario_Id == usuarioId || x.Usuario.CUITRegistro == usuario.CUITRegistro, includes: includes)
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
                    SubCategoriaId = x.SubCategoria_Id != null ? x.SubCategoria_Id : 0,
                    SubCategoria = x.SubCategoria != null ? new SubCategoriaDto
                    {
                        Id = x.SubCategoria.Id,
                        Code = x.SubCategoria.Code,
                        Nombre = x.SubCategoria.Nombre,
                        CategoriaId = x.SubCategoria.Categoria_Id
                    } : new SubCategoriaDto { Nombre = "" },
                    EstadoConsultaId = x.EstadoConsulta_Id,
                    Material_Id = x.Detalle.Material_Id,
                    Material = x.Categoria.Code == "APP" ? x.Detalle.OtroComprobanteNo : "",
                    EstadoConsulta = new EstadoConsultaDto
                    {
                        Id = x.EstadoConsulta.Id,
                        Descripcion = esInterno ? x.EstadoConsulta.Descripcion : x.EstadoConsulta.Code == "GESRTA" ? "En gestión" : x.EstadoConsulta.Descripcion,
                        Color = x.EstadoConsulta.Color,
                        Code = x.EstadoConsulta.Code
                    },
                    FechaCreacion = x.FechaCreacion,
                    FechaUltimaModificacion = x.FechaUltimaModificacion,
                    UsuarioId = x.Usuario_Id,
                    UsuarioInternoId = x.UsuarioInterno_Id,
                    FechaVtoReapertura = x.FechaVtoReapertura,
                    Fecha = x.Detalle != null ? x.Detalle.Fecha : null,
                    ComprobanteNo = x.Detalle != null ? x.Detalle.ComprobanteNo : "",
                    OtroComprobanteNo = x.Detalle != null ? x.Detalle.OtroComprobanteNo : "",
                    ContratoNo = x.Detalle != null ? x.Detalle.ContratoNo : "",
                    Importe = x.Detalle != null ? x.Detalle.Importe : null,
                    Impuesto = x.Detalle != null ? x.Detalle.Impuesto : null,
                    OrdenId = x.Detalle != null ? x.Detalle.Orden_Id : null,
                    PatenteChasis = x.Detalle != null ? x.Detalle.PatenteChasis : null,
                    BolsaEmisoraOblea = x.Detalle != null ? x.Detalle.BolsaEmisoraOblea : "",
                    CausaConsultaId = x.Detalle.CausaConsulta != null ? x.Detalle.CausaConsulta_Id : null,
                    CausaConsulta = x.Detalle.CausaConsulta != null ? new CausaConsultaDto
                    {
                        Id = x.Detalle.CausaConsulta.Id,
                        Nombre = x.Detalle.CausaConsulta.Nombre
                    } : null,
                    RelacionadaPorCodigo = x.Usuario_Id != usuarioId,
                    GeneradaInternamente = x.UsuarioInterno_Id != null && x.UsuarioInterno_Id != usuarioId,
                    GeneradaPorUsuarioSesion = x.UsuarioInterno_Id == usuarioId,
                    GeneradaExternamente = x.UsuarioInterno_Id == null,
                    MailUsuarioIniciaConsulta = x.UsuarioInterno_Id == null ? x.Usuario.Mail : x.UsuarioInterno.Mail
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
            SubCategoria subCategoria = new SubCategoria() { };
            var estado = repositorio.Obtener<EstadoConsulta>(c => c.Id == estadoConsultaId);
            var estadoCerrado = repositorio.Obtener<EstadoConsulta>(c => c.Code == "CER");

            if (subcategoriaId.HasValue && subcategoriaId != 0)
            {
                subCategoria = repositorio.Obtener<SubCategoria>(c => c.Id == subcategoriaId);
                if (subCategoria == null) throw new InfoCustomException("No existe la subcategoria");
            }

            if (estado == null) throw new InfoCustomException("No existe el estado");
            if (categoria == null) throw new InfoCustomException("No existe la categoria");

            var consulta = GetConsulta(consultaId);
            var usuario = repositorio.Obtener<Usuario>(u => u.Id == consulta.Usuario_Id);
            consulta.Categoria_Id = categoriaId;

            if(consulta.EstadoConsulta_Id != estadoCerrado.Id 
                && estado.Id == estadoCerrado.Id)
            {
                consulta.FechaVtoReapertura = DateTime.Now.AddDays(7);
            }
            consulta.EstadoConsulta_Id = estadoConsultaId;

            if (subcategoriaId != 0)
            {
                consulta.SubCategoria_Id = subcategoriaId;
            }

            if (causaConsultaId.HasValue && causaConsultaId != 0)
            {
                var causaConsulta = repositorio.Obtener<CausaConsulta>(cc => cc.Id == causaConsultaId);
                if (causaConsulta == null) throw new InfoCustomException("No existe la causa de consulta");
                consulta.Detalle.CausaConsulta_Id = causaConsultaId;
            }

            if (categoria.Code == "FIN")
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINCOR").Id;
                    var subcategoriaCode = subCategoria.Code;
                    subcategoriaCode = subcategoriaCode = subcategoriaCode.Contains("FC") ? subcategoriaCode : subcategoriaCode.Contains("PC") ? subcategoriaCode.Replace("PC", "FC") : subcategoriaCode + "FC";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Code == subcategoriaCode).Id;
                }
                else
                {
                    consulta.Categoria_Id = consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "FINDIR").Id;
                    var subcategoriaCode = subCategoria.Code;
                    subcategoriaCode = subcategoriaCode.Contains("FD") ? subcategoriaCode : subcategoriaCode.Contains("PD") ? subcategoriaCode.Replace("PD", "FD") : subcategoriaCode + "FD";
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Categoria_Id == consulta.Categoria_Id).Id;
                }
            }

            if (categoria.Code == "PAR")
            {
                if (usuario.TipoUsuario.NombreCorto == "CORR")
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARCOR").Id;
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Categoria_Id == consulta.Categoria_Id).Id;
                }
                else
                {
                    consulta.Categoria_Id = repositorio.Obtener<Categoria>(c => c.Code == "PARDIR").Id;
                    consulta.SubCategoria_Id = repositorio.Obtener<SubCategoria>(sc => sc.Categoria_Id == consulta.Categoria_Id).Id;
                }
            }

            if(categoria.Code == "ACT")
            {
                if(subCategoria.Code == "CM05" && estado.Code == "CER")
                {
                    gestionImpuestosService.ActualizarCM05(consultaId, usuario);
                }
            }

            consulta.FechaUltimaModificacion = DateTime.Now;

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

                if (comentario.Archivos == null)
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

        public List<CategoriaDto> ObtenerCategorias(Boolean? excluir, UsuarioDto usuario, Boolean? mostrarCategoriaInterno)
        {
            try
            {
                List<string> exclude = new List<string>() { };
                List<Categoria> categorias = new List<Categoria>() { };

                if (excluir.HasValue && excluir == true)
                {
                    exclude = new List<string>() { "PARDIR", "PARCOR", "FINDIR", "FINCOR" };
                }
                else
                {
                    exclude = new List<string>() { };
                }

                if (mostrarCategoriaInterno == false)
                    exclude.Add("ORD");

                if (usuario.NuevoUsuario)
                {
                    var categoriasNuevosUsuarios = new List<string>() { "OTRO", "FWEB" };
                    categorias = repositorio.Listar<Categoria>(c => categoriasNuevosUsuarios.Contains(c.Code)).OrderBy(c => c.Nombre).ToList();
                }
                else
                {
                    categorias = repositorio.Listar<Categoria>(c => !exclude.Contains(c.Code)).OrderBy(c => c.Nombre).ToList();
                }
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

        public List<CategoriaDto> ObtenerCategoriasInterno(Boolean? excluir, UsuarioDto usuario)
        {
            try
            {
                List<string> categoriasContacto = new List<string>
                {
                    "BOL", "DATMAE", "REI", "ACT", "PAR", "FIN", "CAL", "COM",
                    "COMP", "APP", "PES", "PAG", "FWEB", "MATBA",
                    "PROVGC", "FLECONSULTA", "OTRO", "PARDIR", "PARCOR",
                    "FINDIR", "FINCOR", "FLE", "CRDECPE", "ORD"
                };
  
                var user = repositorio.Obtener<Usuario>(u => u.Id == usuario.Id);
                var rolesUsuario = user.Roles.Where(r => categoriasContacto.Contains(r.Codigo))
                    .Select(r => r.Codigo).ToList();

                var categorias = repositorio.Listar<Categoria>(c => rolesUsuario.Contains(c.Code));

                return categorias.Select(x => new CategoriaDto(x)).OrderBy(c => c.Nombre).ToList();
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

        public List<SubCategoriaDto> ObtenerSubCategorias(UsuarioDto usuario)
        {
            try
            {
                var subcategorias = repositorio.Listar<SubCategoria>().OrderBy(c => c.Nombre);
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

        public List<MaterialDto> ObtenerMaterial(TablaSeccionMaterial tablaSeccionMaterial)
        {
            try
            {
                var subcategorias = repositorio.Listar<Material>().Where(x => x.TablaSeccionMaterial == tablaSeccionMaterial).OrderBy(c => c.Nombre);
                return subcategorias.Select(x => new MaterialDto
                {
                    MaterialId = x.Id,
                    Descripcion = x.Nombre,
                    CodigoSap = x.CodigoSap,
                    ValidaSisaRuca = x.ValidaSisaRuca

                }).ToList();
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

        public string GenerarReclamoImpositivoPdf(ReclamoImpositivo reclamoImpositivo)
        {
            // Creamos el documento con el tamaño de página tradicional
            Document doc = new Document(PageSize.LETTER);
            // Indicamos donde vamos a guardar el documento
            var ruta = string.Format("{0}/{1}", rutaArchivosConsulta, "ReclamoImpositivo.pdf");
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));

            // Le colocamos el título y el autor
            // **Nota: Esto no será visible en el documento
            doc.AddTitle("Reclamo Impositivo");
            doc.AddCreator(reclamoImpositivo.RazonSocialProveedor);

            // Abrimos el archivo
            doc.Open();

            // Creamos el tipo de Font que vamos utilizar
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _standardFontBold = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


            var parrafo0 = string.Format("Lugar y fecha: {0}, {1} \n\nSeñores: \n\nMolinos Agro S.A \n ____________________________________________________________________________\n", reclamoImpositivo.Lugar, DateTime.Now);
            doc.Add(new Paragraph(parrafo0));

            var parrafo = string.Format("\nEl  que  suscribe, {0} (DNI  N°{1}),  en mi carácter de {2}  (apoderado, representante legal, etc.) de {3},  CUIT N° {4}, por la presente manifiesto en carácter de declaración jurada, que no hemos computado  ni  computaremos  como  pago  a  cuenta  en  las  respectivas  declaraciones  juradas  del  Impuesto sobre los Ingresos Brutos, las retenciones efectuadas por Molinos Agro S.A. de acuerdo con el siguiente detalle:"
                , reclamoImpositivo.RazonSocialProveedor, reclamoImpositivo.Dni, reclamoImpositivo.Vinculo, reclamoImpositivo.RazonSocialEmpresa, reclamoImpositivo.Cuit);
            // Escribimos el encabezamiento en el documento
            doc.Add(new Paragraph(parrafo));
            doc.Add(Chunk.NEWLINE);

            // Creamos una tabla que contendrá el nombre, apellido y país
            // de nuestros visitante.
            PdfPTable tblTabla = new PdfPTable(4);

            // Configuramos el título de las columnas de la tabla
            PdfPCell clNombre = new PdfPCell(new Phrase("", _standardFont));
            clNombre.BorderWidth = 1;

            PdfPCell clFecha = new PdfPCell(new Phrase("Fecha", _standardFont));
            clFecha.BorderWidth = 1;

            PdfPCell clImporte = new PdfPCell(new Phrase("Importe Retención", _standardFont));
            clImporte.BorderWidth = 1;

            PdfPCell clCertificado = new PdfPCell(new Phrase("Certificado n°", _standardFont));
            clCertificado.BorderWidth = 1;

            // Añadimos las celdas a la tabla
            tblTabla.AddCell(clNombre);
            tblTabla.AddCell(clFecha);
            tblTabla.AddCell(clImporte);
            tblTabla.AddCell(clCertificado);

            // Llenamos la tabla con información
            foreach (Reclamo reclamo in reclamoImpositivo.Reclamos)
            {
                clNombre = new PdfPCell(new Phrase(reclamoImpositivo.RazonSocialEmpresa, _standardFont));
                clNombre.BorderWidth = 1;

                reclamo.Fecha = reclamo.Fecha.Substring(0, 10);

                clFecha = new PdfPCell(new Phrase(reclamo.Fecha, _standardFont));
                clFecha.BorderWidth = 1;

                clImporte = new PdfPCell(new Phrase(reclamo.Importe, _standardFont));
                clImporte.BorderWidth = 1;

                clCertificado = new PdfPCell(new Phrase(reclamo.Certificado, _standardFont));
                clCertificado.BorderWidth = 1;

                // Añadimos las celdas a la tabla
                tblTabla.AddCell(clNombre);
                tblTabla.AddCell(clFecha);
                tblTabla.AddCell(clImporte);
                tblTabla.AddCell(clCertificado);
            }

            // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
            doc.Add(tblTabla);

            doc.Add(new Paragraph("\n\n\n__________________\nFirma y aclaración \n"));
            doc.Add(new Paragraph("(Certificada por Banco o Escribano)", _standardFontBold));
            doc.Add(Chunk.NEWLINE);

            doc.Close();
            writer.Close();

            return ruta;
        }

        private string ArmarRutaCarpeta(Comentario comentario)
        {
            return string.Format("{0}/{1}/{2}", rutaArchivosConsulta, comentario.Consulta.Usuario_Id, comentario.Consulta_Id);
        }

        public string AnularConsulta(int consultaId, int usuarioId, string motivoRechazo)
        {
            this.ActualizarEstadoConsulta(consultaId, (int)EstadosConsulta.Rechazado);

            ComentarioDto comentarioDto = new ComentarioDto
            {
                Detalle = motivoRechazo + ", consulta cerrada.",
                Fecha = timeProvider.Now(),
                UsuarioId = usuarioId,
            };

            this.AgregarComentario(consultaId, comentarioDto, null);

            Consulta consulta = repositorio.Obtener<Consulta>(consultaId);
            if (consulta.Categoria.Code == Categorias.Actualizacion && consulta.SubCategoria.Code == SubCategorias.CM05)
            {
                IngresosBrutosCoeficienteUnificado ingresosBrutosCoeficienteUnificado =
                    repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(x => x.Consulta_Id == consultaId);

                ingresosBrutosCoeficienteUnificado.EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.RechazadoPorUsuario;
                repositorio.GuardarCambios();
            }

            return SuccessMsg.ConsultaRechazadaOK;
        }

        public string ProcesarCM05(HttpFileCollectionBase archivos, string cuitProveedor, int? comentario_Id = null, bool esCargaInterna = false)
        {
            try
            {
                var errores = new List<string>();
                string resultado = string.Empty;

                bool existeArchivoConCoeficientes = false;
                for (int i = 0; i < archivos.Count; i++)
                {
                    HttpPostedFileBase archivo = archivos[i];
                    if (archivo.ContentType == "application/pdf")
                    {
                        var operacionOCRId = Task.Run(async () => await azureService.AnalizarImagenAsync(archivo)).Result;

                        Thread.Sleep(2000);

                        var elementosLeidos = Task.Run(async () => await azureService.ObtenerResultadoOCRAsync(operacionOCRId)).Result;

                        if (elementosLeidos.Any(str => str == "Determinación del Coeficiente Unificado"))
                        {
                            Comentario comentario = null;
                            int? consulta_Id = null;
                            int archivo_Id = 1;
                            string nombreArchivo = string.Format("{0}_{1}", comentario_Id, Path.GetFileName(archivo.FileName));

                            if (comentario_Id != null)
                            {
                                comentario = repositorio.Obtener<Comentario>(comentario_Id);
                                consulta_Id = comentario.Consulta_Id;
                                archivo_Id = comentario.Archivos.Single(file => file.ObtenerNombre() == nombreArchivo).Id;
                            }

                            if (esCargaInterna)
                            {
                                var ruta = ArmarRutaCarpetaCM05(cuitProveedor);
                                var rutaArchivo = string.Concat(ruta, "/", nombreArchivo);

                                Directory.CreateDirectory(ruta);

                                var ArchivoAGuardar = new Archivo()
                                {
                                    FileKey = FileKeys.FormularioCM05,
                                    Ruta = rutaArchivo,
                                };

                                archivo.SaveAs(rutaArchivo);
                                repositorio.Agregar(ArchivoAGuardar);
                                repositorio.GuardarCambios();

                                archivo_Id = ArchivoAGuardar.Id;
                            }

                            resultado = ProcesarArchivoCoeficientesImpuestosIngresosBrutos(elementosLeidos.ToList(), archivo_Id, cuitProveedor, consulta_Id, esCargaInterna);
                            existeArchivoConCoeficientes = true;
                            break;
                        }
                    }
                }

                if (!existeArchivoConCoeficientes)
                {
                    throw new ValidationCustomException("No se pudieron obtener los coeficientes. Por favor, asegúrese de adjuntar el documento correspondiente.");
                }

                return resultado;
            }
            catch (ValidationCustomException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidationCustomException(ErrorMsg.ErrorCargaCM05, ex, true);
            }
        }

        private string ProcesarArchivoCoeficientesImpuestosIngresosBrutos(List<string> elementosLeidos, int archivo_Id, string cuitProveedor, int? consulta_Id, bool esCargaInterna)
        {
            //Descarto palabras que ya se que son "basura"
            elementosLeidos
                .RemoveAll(elemento => elemento.StartsWith("..") && elemento.EndsWith("..") ||
                                       elemento.All(caracter => caracter == '.'));

            int indiceDeterminacionDelCoeficienteUnificado = elementosLeidos.IndexOf("Determinación del Coeficiente Unificado");
            string encabezadoFormulario = "OSIRIS";
            int indiceComienzoPaginaCoeficientesBrutos = elementosLeidos.Take(indiceDeterminacionDelCoeficienteUnificado).ToList().LastIndexOf(encabezadoFormulario);
            List<string> info_DeterminacionCoeficienteUnificado = elementosLeidos.Skip(indiceComienzoPaginaCoeficientesBrutos).ToList();

            string cuit = SacarHasta(info_DeterminacionCoeficienteUnificado, "CUIT:")[0].Replace("-", "");

            int anticipoAux;
            int anticipo = Int32.TryParse(SacarHasta(info_DeterminacionCoeficienteUnificado, "Anticipo:")[0], out anticipoAux) ? anticipoAux : 0;

            int sedeAux;
            int sede = Int32.TryParse(SacarHasta(info_DeterminacionCoeficienteUnificado, "Sede:")[0], out sedeAux) ? sedeAux : 0;

            string secuencia = SacarHasta(info_DeterminacionCoeficienteUnificado, "Secuencia:")[0];
            int? idSecuencia =
                secuencia == "Original" ? (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original :
                secuencia.Contains("Rectificativa") ? (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa :
                (int?)null;

            string razonSocial = SacarHasta(info_DeterminacionCoeficienteUnificado, "Contribuyente:")[0];

            var ingresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificado
            {
                EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                CUIT = cuit,
                Anticipo = anticipo,
                Sede = sede,
                FechaCarga = timeProvider.Now(),
                FechaUltimaModificacion = timeProvider.Now(),
                Consulta_Id = consulta_Id,
                Archivo_Id = archivo_Id,
                SecuenciaIngresosBrutosCoeficienteUnificado_Id = idSecuencia,
                RazonSocial = razonSocial,
            };

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetalles = new List<IngresosBrutosCoeficienteUnificadoDetalle>();

            List<string> listadoCoeficientes = SacarHasta(elementosLeidos, "Coeficiente Unificado");

            for (int i = 0; i < listadoCoeficientes.Count; i++)
            {
                try
                {
                    int numeroJurisdiccionAux;
                    int? numeroJurisdiccion = int.TryParse(listadoCoeficientes[i], out numeroJurisdiccionAux) ? numeroJurisdiccionAux : (int?)null;

                    string jurisdiccion = listadoCoeficientes[i + 1];

                    DateTime fechaInicioAux;
                    DateTime? fechaInicio = DateTime.TryParseExact(listadoCoeficientes[i + 2], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioAux) ? fechaInicioAux : (DateTime?)null;

                    DateTime fechaCeseAux;
                    DateTime? fechaCese = DateTime.TryParseExact(listadoCoeficientes[i + 3], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaCeseAux) ? fechaCeseAux : (DateTime?)null;

                    i += (fechaInicio.HasValue ? fechaCese.HasValue ? 4 : 3 : 2);

                    decimal coeficienteIngresosAux;
                    decimal? coeficienteIngresos = decimal.TryParse(listadoCoeficientes[i++], out coeficienteIngresosAux) ? coeficienteIngresosAux : (decimal?)null;

                    decimal coeficienteGastosAux;
                    decimal? coeficienteGastos = decimal.TryParse(listadoCoeficientes[i++], out coeficienteGastosAux) ? coeficienteGastosAux : (decimal?)null;

                    decimal coeficienteUnificadoAux;
                    decimal? coeficienteUnificado = decimal.TryParse(listadoCoeficientes[i], out coeficienteUnificadoAux) ? coeficienteUnificadoAux : (decimal?)null;

                    IngresosBrutosCoeficienteUnificadoDetalle detalleGenerado = new IngresosBrutosCoeficienteUnificadoDetalle
                    {
                        NumeroJurisdiccion = numeroJurisdiccion,
                        Jurisdiccion = jurisdiccion,
                        FechaInicio = fechaInicio,
                        FechaCese = fechaCese,
                        CoeficienteIngresos = coeficienteIngresos,
                        CoeficienteGastos = coeficienteGastos,
                        CoeficienteUnificado = coeficienteUnificado,
                        FechaUltimaModificacion = timeProvider.Now()
                    };

                    //repositorio.Agregar(detalleGenerado);

                    ingresosBrutosCoeficienteUnificadoDetalles.Add(detalleGenerado);

                    if (ingresosBrutosCoeficienteUnificadoDetalles.Count >= 24)
                        break;
                }
                catch (Exception)
                {

                }
            }

            ingresosBrutosCoeficienteUnificado.Detalle = ingresosBrutosCoeficienteUnificadoDetalles;
            ingresosBrutosCoeficienteUnificado.MalCargada = string.IsNullOrWhiteSpace(cuit) || anticipo == 0 || sede == 0 || !idSecuencia.HasValue || string.IsNullOrWhiteSpace(razonSocial) ||
                ingresosBrutosCoeficienteUnificadoDetalles.Any(i => !i.CoeficienteUnificado.HasValue || !i.NumeroJurisdiccion.HasValue || string.IsNullOrWhiteSpace(i.Jurisdiccion));

            repositorio.Agregar(ingresosBrutosCoeficienteUnificado);



            MovimientoIngresosBrutosCoeficienteUnificado movimientoIngresosBrutosCoeficienteUnificado = new MovimientoIngresosBrutosCoeficienteUnificado
            {
                IngresosBrutosCoeficienteUnificado_Id = ingresosBrutosCoeficienteUnificado.Id,
                Observaciones = $"Creado por: {cuitProveedor}",
                Fecha = timeProvider.Now(),
                TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = 1,
                OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = 1,
                EstadoAnterior_Id = 1,
                EstadoPosterior_Id = 1,
            };
            repositorio.Agregar(movimientoIngresosBrutosCoeficienteUnificado);

            repositorio.GuardarCambios();

            return
                esCargaInterna ?
                    ingresosBrutosCoeficienteUnificado.MalCargada ?
                        SuccessMsg.AltaFormularioCM05CargaInternaMalCargadoOK
                      : SuccessMsg.AltaFormularioCM05CargaInternaOK
                : cuit != cuitProveedor ?
                    SuccessMsg.AltaFormularioCM05DistintoCUITOK
                : string.Empty;
        }

        private List<string> SacarHasta(IList<string> listaStrings, string elemento)
        {
            try
            {
                return listaStrings.Skip(1 + listaStrings.IndexOf(elemento)).ToList();
            }
            catch (Exception) { }

            return new List<string>();
        }

        public virtual string ArmarRutaCarpetaCM05(string username)
        {
            var usuario = repositorio.Obtener<Usuario>(x => x.Mail == username);
            return string.Format("{0}/{1}", rutaArchivosCM05, usuario.Id);
        }

        public AgregarConsultaResponseDto AgregarConsultaInterna(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<DestinatarioDto> destinatarios)
        {
            string mensajeResultado = string.Empty;

            Categoria categoria = repositorio.Obtener<Categoria>(c => c.Id == consulta.Categoria_Id);
          
            var estrategia = this.consultaContext.GetStrategy(categoria.Nombre);
            
            consulta = estrategia.AgregarConsulta(consulta, comentario, destinatarios, files);

            return new AgregarConsultaResponseDto
            {
                ConsultaDto = ObtenerConsulta(consulta.Id),
                Mensaje = mensajeResultado,
            };
        }
        public void ReabrirConsulta(int consultaId, UsuarioDto usuarioActual)
        {
            var consulta = GetConsulta(consultaId);

            if (usuarioActual.Id != consulta.Usuario_Id && usuarioActual.CUIT != consulta.Usuario.CUITRegistro)
                throw new ValidationCustomException("No se puede reabrir la consulta ya que ud no inició esta consulta.");

            var estadoIniciado = GetEstadoConsulta("INI");
            var estadoCerrado = GetEstadoConsulta("CER");
            
            if (consulta.EstadoConsulta_Id != estadoCerrado.Id)
                throw new ValidationCustomException("La consulta ya se encuentra en gestión.");
            
            if (consulta.UsuarioInterno_Id != null)
                throw new ValidationCustomException("Ud no tiene permiso para reabrir esta consulta.");

            consulta.EstadoConsulta_Id = estadoIniciado.Id;
            this.repositorio.GuardarCambios();
        }

        private EstadoConsulta GetEstadoConsulta(string codigo)
        {
            var estadoConsulta = repositorio.Obtener<EstadoConsulta>(c => c.Code == codigo);

            if (estadoConsulta == null) 
                throw new InfoCustomException("No existe el estado de la consulta.");

            return estadoConsulta;
        }

    }   
}