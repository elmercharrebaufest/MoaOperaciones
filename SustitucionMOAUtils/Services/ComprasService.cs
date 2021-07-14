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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Net;
using System.Net.Mail;


namespace SustitucionMOAUtils.Services
{
    public class ComprasService : IComprasService
    {
        private readonly IRepositorio repositorio;

        private readonly string rutaArchivosCompras = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public ComprasService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public SolpDto GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
        {
            Solp solpEntity = null;
            Pliego pliegoEntity = null;

            if (solp.Id.HasValue)
            {
                var includes = new List<Expression<Func<Solp, object>>>();
                includes.Add(x => x.Pliego);
                includes.Add(x => x.Pliego.VisitasMasivas);
                includes.Add(x => x.Pliego.Archivos);
                includes.Add(x => x.Posiciones);
                includes.Add(x => x.Posiciones.Select(y=> y.Subposiciones));
                includes.Add(x => x.UsuarioCreacion);
                includes.Add(x => x.UsuarioModificacion);

                solpEntity = repositorio.Obtener<Solp>(solp.Id.Value);

                if(solpEntity != null)
                {
                    //TODO: validar si está en un estado modificable

                    solpEntity.UsuarioModificacion_Id = solp.UsuarioActual.Id;
                    solpEntity.FechaModificacion = DateTime.Now;

                    pliegoEntity = solpEntity.Pliego;
                }
            }
            else
            {
                solpEntity = new Solp()
                {
                    UsuarioCreacion_Id = solp.UsuarioActual.Id,
                    FechaCreacion = DateTime.Now
                };

                var estadoIncompletoCodigo = EstadoDocumentoSolp.Incompleto.Code();
                var estadoIncompleto = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoIncompletoCodigo);
                solpEntity.EstadoDocumento_Id = estadoIncompleto.Id;

                solpEntity.Pliego = new Pliego();
                solpEntity.Posiciones = new List<SolpPosicion>();
                pliegoEntity = solpEntity.Pliego;

                repositorio.Agregar(solpEntity);
            }
             
            if (solpEntity != null)
            {
                if(solp.ClaseDocumento != null)
                    solpEntity.ClaseDocumento = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.ClaseDocumento && x.Codigo == solp.ClaseDocumento.Codigo);

                pliegoEntity.NombreObra = solp.NombreDeObra;
                pliegoEntity.FiscalContrato = solp.FiscalContrato;
                pliegoEntity.Telefono = solp.Telefono;
                pliegoEntity.Email = solp.Email;
                pliegoEntity.FechaHoraEntrega = solp.FechaHoraEntrega?.ToLocalTime();
                pliegoEntity.SupervisorSector = solp.SupervisorSector;
                pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo;
                
                pliegoEntity.TieneVisitaObra = solp.TieneVisitaObra;
                pliegoEntity.TieneVisitaObraMasiva = solp.TieneVisitaObraMasiva;
                pliegoEntity.TieneObradores = solp.TieneObradores;
                pliegoEntity.TieneMedioElevacion = solp.TieneMedioElevacion;
                pliegoEntity.TieneTecnicoSeguridad = solp.TieneTecnicoSeguridad;
                pliegoEntity.TieneDescripcionTecnica = solp.TieneDescripcionTecnica;
                pliegoEntity.TieneDocumentacionTecnica = solp.TieneDocumentacionTecnica;
                pliegoEntity.FechaHoraLimiteConsulta = solp.FechaHoraLimiteConsulta?.ToLocalTime();
                pliegoEntity.ObservacionesGeneracion = solp.ObservacionesGeneracion;
                pliegoEntity.ObservacionesCotizacion = solp.ObservacionesCotizacion;
                pliegoEntity.DiasEjecucion = solp.DiasEjecucion;
                pliegoEntity.JornadaLaboralDias = solp.JornadaLaboral != null ? string.Join(",", solp.JornadaLaboral.Select(x=>(int)x)) : string.Empty;
                pliegoEntity.JornadaLaboralHorasDesde = solp.JornadaLaboralDesde?.ToLocalTime();
                pliegoEntity.JornadaLaboralHorasHasta = solp.JornadaLaboralHasta?.ToLocalTime();

                if(solp.TieneVisitaObraMasiva && solp.VisitasObraMasiva != null)
                {
                    if (pliegoEntity.VisitasMasivas == null)
                    {
                        pliegoEntity.VisitasMasivas = new List<PliegoVisita>();
                    }
                    var idVisita = -1;

                    foreach (var visita in solp.VisitasObraMasiva)
                    {
                        var visitaExistente = pliegoEntity.VisitasMasivas.FirstOrDefault(x => x.Codigo == visita.Codigo);

                        if(visitaExistente != null)
                        {
                            visitaExistente.FechaHora = visita.FechaHora.ToLocalTime();
                        }
                        else
                        {
                            pliegoEntity.VisitasMasivas.Add(new PliegoVisita()
                            {
                                Codigo = visita.Codigo,
                                Id = idVisita--,
                                FechaHora = visita.FechaHora.ToLocalTime()
                            }) ;
                        }
                    }
                }

                if (solp.Adjuntos != null && pliegoEntity.Archivos != null)
                {
                    var archivosParaBorrar = pliegoEntity.Archivos.Where(x => !solp.Adjuntos.Select(y=>y.Id).Contains(x.Id)).ToList();

                    foreach (var archivo in archivosParaBorrar)
                    {
                        repositorio.Remover<Archivo>(archivo);
                    }
                }

                if (pliegoEntity.Archivos == null)
                {
                    pliegoEntity.Archivos = new List<Archivo>();
                }

                if(solpEntity.Posiciones == null)
                {
                    solpEntity.Posiciones = new List<SolpPosicion>();
                }

                if(solp.Posiciones != null)
                {
                    //posiciones nuevas y actualizadas
                    foreach (var pos in solp.Posiciones.Where(x=> !solpEntity.Posiciones.Any(y=> y.TextoGenerico == x.TextoGenerico)))
                    {
                        SolpPosicion posEntity = null;

                        posEntity = solpEntity.Posiciones.FirstOrDefault(y => y.TextoGenerico == pos.TextoGenerico);

                        if (posEntity == null)
                            posEntity = new SolpPosicion();

                        posEntity.Codigo = pos.Codigo;
                        posEntity.TextoGenerico = pos.TextoGenerico;
                        posEntity.CodigosProveedores = pos.CodigosProveedores;
                        posEntity.EsConcluido = pos.EsConcluido;
                        posEntity.EsFijacion = pos.EsFijacion;
                        posEntity.FechaEntregaServicio = pos.FechaEntregaServicio;
                        posEntity.FechaLiberacion = pos.FechaLiberacion;
                        posEntity.NroNecesidad = pos.NroNecesidad;

                        posEntity.CalleEntrega = pos.CalleEntrega;
                        posEntity.NombreEntrega = pos.NombreEntrega;
                        posEntity.CpEntrega = pos.CpEntrega;
                        posEntity.NumeroEntrega = pos.NumeroEntrega;
                        posEntity.PaisEntrega = pos.PaisEntrega;
                        posEntity.PlazoEntrega = pos.PlazoEntrega;
                        posEntity.Solicitante = pos.Solicitante;
                        
                        if(pos.TipoPosicion != null)
                            posEntity.TipoPosicion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoPosicionSolp && x.Codigo == pos.TipoPosicion.Codigo);

                        if (pos.TipoImputacion != null)
                            posEntity.TipoImputacion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoImputacionSolp && x.Codigo == pos.TipoImputacion.Codigo);

                        if (pos.Almacen != null)
                            posEntity.Almacen = repositorio.Obtener<TablaSap>(x=>x.Tabla == TablasSap.Almacen && x.Codigo == pos.Almacen.Codigo);

                        if (pos.Centro != null)
                            posEntity.Centro = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Centro && x.Codigo == pos.Centro.Codigo);

                        if (pos.GrupoCompras != null)
                            posEntity.GrupoCompras = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoCompras && x.Codigo == pos.GrupoCompras.Codigo);

                        if (pos.GrupoArticulo != null)
                            posEntity.GrupoArticulo = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoArticulo && x.Codigo == pos.GrupoArticulo.Codigo);

                        if (pos.Subposiciones != null)
                        {
                            if (posEntity.Subposiciones == null)
                                posEntity.Subposiciones = new List<SolpSubposicion>();

                            foreach (var subpos in pos.Subposiciones)
                            {
                                SolpSubposicion subposEntity = null;

                                subposEntity = posEntity.Subposiciones.FirstOrDefault(y => y.Numero == subpos.Numero);

                                if (subposEntity == null)
                                    subposEntity = new SolpSubposicion();

                                subposEntity.Codigo = subpos.Codigo;
                                subposEntity.Cantidad = subpos.Cantidad;
                                subposEntity.CentroCosto = subpos.TipoImputacionValor; //cambiar campo en base
                                subposEntity.CuentaMayor = subpos.CuentaMayor;
                                subposEntity.Numero = subpos.Numero;
                                subposEntity.PrecioBruto = subpos.PrecioBruto;
                                subposEntity.Tarea = subpos.Tarea;

                                if (subpos.Unidad != null)
                                    subposEntity.Unidad = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Unidad && x.Codigo == subpos.Unidad.Codigo);

                                if(subpos.CodigoServicioSap != null)
                                    subposEntity.CodigoServicioSap = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.CodigoServicioSap && x.Codigo == subpos.Unidad.Codigo);

                                posEntity.Subposiciones.Add(subposEntity);
                            }
                        }

                        if(pos.Proveedores != null)
                        {
                            if(posEntity.Proveedores == null)
                                posEntity.Proveedores = new List<SolpProveedor>();

                            foreach (var prov in pos.Proveedores)
                            {
                                SolpProveedor provEntity = null;

                                provEntity = posEntity.Proveedores.FirstOrDefault(x=>x.RazonSocial == prov.RazonSocial);

                                if (provEntity == null)
                                    provEntity = new SolpProveedor();

                                provEntity.RazonSocial = prov.RazonSocial;
                                //pendiente otros campos

                                if(prov.TipoFiltroProveedorSolp != null)
                                    provEntity.TipoFiltroProveedorSolp = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoFiltroSolpProveedor && x.Codigo == prov.TipoFiltroProveedorSolp.Codigo);

                                posEntity.Proveedores.Add(provEntity);
                            }
                        }

                        solpEntity.Posiciones.Add(posEntity);
                    }

                    //posiciones eliminadas
                }
            }

            repositorio.GuardarCambios();

            solp.Id = solpEntity.Id;

            var rutaArchivo = string.Concat(ObtenerRutaArchivos(solpEntity.Id), "/", FileKeys.EspecificacionesTecnicasPliego, ".txt");
            
            Directory.CreateDirectory(ObtenerRutaArchivos(solpEntity.Id));
            File.WriteAllText(rutaArchivo, solp.EspecificacionesTecnicas);
            
            var archivoEspecificacionesTecnicasPliego = pliegoEntity.Archivos.FirstOrDefault(x => x.FileKey == FileKeys.EspecificacionesTecnicasPliego);

            if(archivoEspecificacionesTecnicasPliego == null)
            {
                pliegoEntity.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.EspecificacionesTecnicasPliego,
                    Ruta = rutaArchivo,
                });
            }

            GuardarAdjuntosSolp(solp, adjuntos, pliegoEntity);

            repositorio.GuardarCambios();

            solp.Adjuntos = pliegoEntity.Archivos.Where(x=>x.FileKey == FileKeys.AdjuntoSolp).Select(x => new ArchivoDto()
            {
                Id = x.Id,
                FileKey = x.FileKey,
                Nombre = x.ObtenerNombre(x.Ruta)
            }).ToList();
            
            return solp;
        }

        private string ObtenerRutaArchivos(int solpId)
        {
            return string.Format("{0}/Solp_{1}", rutaArchivosCompras, solpId);
        }

        private void GuardarAdjuntosSolp(SolpDto solp, HttpFileCollectionBase files, Pliego pliego)
        {
            var ruta = ObtenerRutaArchivos(solp.Id.Value);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var rutaArchivo = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                if (File.Exists(rutaArchivo))
                {
                    file.SaveAs(rutaArchivo);
                    continue;
                }

                Directory.CreateDirectory(ruta);

                pliego.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.AdjuntoSolp,
                    Ruta = rutaArchivo,
                });

                file.SaveAs(rutaArchivo);
            }
        }

        public string ObtenerRutaArchivo(int archivoId)
        {
            var archivo = repositorio.Obtener<Archivo>(archivoId);

            return archivo?.Ruta;
        }

        public List<TablaSapDto> ObtenerTablaSap(string tabla)
        {
            return repositorio.Listar<TablaSap>(x=>x.Tabla == tabla).Select(x=> new TablaSapDto(x)).ToList();
        }

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x=> new CentroDireccionDto(x)).ToList();
        }


        public List<SolpDto> ListarSolp() 
        {
            var todasLasSolp = repositorio.Listar<Solp>(x => x.FechaBorrado == null)
                .Select(x => new SolpDto
                {
                    UsuarioActual = new UsuarioDto(x.UsuarioCreacion),
                    Id = x.Id,
                    NroSolp = x.NroSolp,
                    NombreDeObra = x.Pliego?.NombreObra,
                    FechaCreacion = x.FechaCreacion,
                    EstadoDocumento = new TablaEstadoDto(x.EstadoDocumento),
                    EstadoSolpSapId = x.EstadoSolpSap_Id,
                    EstadoSolpSap = x.EstadoSolpSap != null ? new TablaSapDto(x.EstadoSolpSap) : new TablaSapDto(),
                    //TipoSolp
                    VincularPliego = !x.Pliego_Id.HasValue,
                });

            return todasLasSolp.ToList();


        }

        public SolpDto TraerSolpId(int idSolp)
        {

            var includes = new List<Expression<Func<Solp, object>>>();
            includes.Add(u => u.Pliego);
            includes.Add(u => u.Pliego.VisitasMasivas);
            includes.Add(u => u.Pliego.Archivos);
            includes.Add(u => u.Posiciones);
            includes.Add(u => u.Posiciones.Select(y => y.Subposiciones));
            includes.Add(u => u.UsuarioCreacion);
            includes.Add(u => u.UsuarioModificacion);

            var x = repositorio.Obtener<Solp>(includes, s => s.Id == idSolp);

            if (x == null)
            {
                throw new InfoCustomException("No se encontro la solp");
            }


            var solpDevuelta = new SolpDto()
            {

                UsuarioActual = new UsuarioDto(x.UsuarioCreacion),
                Id = x.Id,
                NroSolp = x.NroSolp,
                FechaCreacion = x.FechaCreacion,
                EstadoDocumento = new TablaEstadoDto(x.EstadoDocumento),
                EstadoSolpSap = x.EstadoSolpSap != null ? new TablaSapDto(x.EstadoSolpSap) : new TablaSapDto(),
                //TipoSolp
                VincularPliego = !x.Pliego_Id.HasValue,


                NombreDeObra = x.Pliego.NombreObra,
                FiscalContrato = x.Pliego.FiscalContrato,
                Telefono = x.Pliego.Telefono,
                Email = x.Pliego.Email,
                FechaHoraEntrega = x.Pliego.FechaHoraEntrega,
                SupervisorSector = x.Pliego.SupervisorSector,
                SupervisorTrabajo = x.Pliego.SupervisorTrabajo,
                VisitasObraMasiva = x.Pliego.VisitasMasivas.Select(a => new VisitaObraDto(a)).ToList(),
                TieneVisitaObra = x.Pliego.TieneVisitaObra ?? false,
                TieneVisitaObraMasiva = x.Pliego.TieneVisitaObraMasiva ?? false,
                TieneObradores = x.Pliego.TieneObradores ?? false,
                TieneMedioElevacion = x.Pliego.TieneMedioElevacion ?? false,
                TieneTecnicoSeguridad = x.Pliego.TieneTecnicoSeguridad ?? false,
                TieneDescripcionTecnica = x.Pliego.TieneDescripcionTecnica ?? false,
                TieneDocumentacionTecnica = x.Pliego.TieneDocumentacionTecnica ?? false,
                FechaHoraLimiteConsulta = x.Pliego.FechaHoraLimiteConsulta,
                ObservacionesGeneracion = x.Pliego.ObservacionesGeneracion,
                //EspecificacionesTecnicas = x.EspecificacionesTecnicas,
                DiasEjecucion = x.Pliego.DiasEjecucion,
                ObservacionesCotizacion = x.Pliego.ObservacionesCotizacion,
                JornadaLaboral = x.Pliego.JornadaLaboralDias.Split(",".ToCharArray()).Select(a => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a)).ToList(), 
                JornadaLaboralDesde = x.Pliego.JornadaLaboralHorasDesde,
                JornadaLaboralHasta = x.Pliego.JornadaLaboralHorasHasta,
                ClaseDocumento = x.ClaseDocumento != null ? new TablaSapDto(x.ClaseDocumento) : new TablaSapDto(),
                Adjuntos = x.Pliego.Archivos.Where(a=>a.FileKey == FileKeys.AdjuntoSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta)
                }).ToList(),

                EspecificacionesTecnicas = x.Pliego.Archivos.FirstOrDefault(a=>a.FileKey == FileKeys.EspecificacionesTecnicasPliego)?.Ruta,
                    
                EstadoSolpSapId = x.EstadoSolpSap_Id,
                EstadoDocumentoId = x.EstadoDocumento_Id,

                Posiciones = x.Posiciones.Select(p=>new SolpPosicionDto(p)).ToList()                   
            };

            return solpDevuelta;


        }



        public string BorrarSolp(int idSolp) 
        {
            var solpABorrar = repositorio.Obtener<Solp>(x => x.Id == idSolp);

            if (solpABorrar == null) 
            {
                throw new InfoCustomException("No se encontro la Solp");
            }

            solpABorrar.FechaBorrado = DateTime.Now;
            repositorio.GuardarCambios();

            return "Se Borro Correctamente";
          
        }

        public List<TablaEstadoDto> ObtenerTablaEstado(string tabla)
        {
            var estados = repositorio.Listar<TablaEstado>(x => x.Tabla == tabla)
                .Select(x => new TablaEstadoDto(x));
                
            return estados.ToList();

        }

        public byte[] GenerarSolpPdf(int idSolp)
        {
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/PliegoSolpTemplate.html");
            var templateString = System.IO.File.ReadAllText(templateFilePath);

            var solpValores = new Dictionary<string, string>();

            //aca va la asignacion de valores de la solp que se van a reemplazar en el documento
            solpValores.Add(SolpTemplateKeys.FECHA_LIBERACION, "20/02/2021");
            solpValores.Add(SolpTemplateKeys.NOMBRE_OBRA, "Ejemplo de nombre de obra");
            solpValores.Add(SolpTemplateKeys.NRO_SOLP, "ID 33333");
            solpValores.Add(SolpTemplateKeys.FISCAL_CONTRATO, "Alberto Hache");
            solpValores.Add(SolpTemplateKeys.TELEFONO, "12345678");

            solpValores.Add(SolpTemplateKeys.FECHA_PRESENTACION, "23/03/2021");
            solpValores.Add(SolpTemplateKeys.USUARIO_COMPRAS, "Miguel sanchez");

            //ESPECIFICACION TECNICA DE TAREAS
            solpValores.Add(SolpTemplateKeys.ESPECIFICACION_TECNICA, "Especificacion tecnica de la obra");

            //COTIZACION Y PLAZO DE EJECUCION
            solpValores.Add(SolpTemplateKeys.PLAZO_EJECUCION, "30 DIAS CORRIDOS");
            solpValores.Add(SolpTemplateKeys.DIAS_JORNADA_LABORAL, "LUNES A VIERNES");
            solpValores.Add(SolpTemplateKeys.INICIO_FINAL_HS_JORNADA_LABORAL, "10 A 16");

            //ANEXO 1
            solpValores.Add(SolpTemplateKeys.TABLA_POSICIONES_SUBPOSICIONES, "Cosas cositas");

            //ADJUNTOS
            solpValores.Add(SolpTemplateKeys.LISTADO_ADJUNTOS, "Adjuntos");


            templateString = CombineTemplateValues(templateString, solpValores);

            #region Generacion del pdf
            StringReader sr = new StringReader(templateString);
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                return bytes;
            }
            #endregion
        }

        private string CombineTemplateValues(string templateStr, Dictionary<string, string> values, string token = "||")
        {
            StringBuilder ret = new StringBuilder();

            foreach (var section in templateStr.Split(token.ToCharArray()))
            {
                var value = values.Keys.Contains(section) ? values[section] : section;
                ret.Append(value);
            }

            return ret.ToString();
        }
    }

    public static class SolpTemplateKeys
    {
        public const string FECHA_LIBERACION = "FECHA_LIBERACION";

        public const string NOMBRE_OBRA = "NOMBRE_OBRA";
        public const string NRO_SOLP = "NRO_SOLP";
        public const string FISCAL_CONTRATO = "FISCAL_CONTRATO";
        public const string TELEFONO = "TELEFONO";

        public const string FECHA_PRESENTACION = "FECHA_PRESENTACION";
        public const string USUARIO_COMPRAS = "USUARIO_COMPRAS";
        public const string ESPECIFICACION_TECNICA = "ESPECIFICACION_TECNICA";

        public const string PLAZO_EJECUCION = "PLAZO_EJECUCION";
        public const string DIAS_JORNADA_LABORAL = "DIAS_JORNADA_LABORAL";
        public const string INICIO_FINAL_HS_JORNADA_LABORAL = "INICIO_FINAL_HS_JORNADA_LABORAL";

        public const string TABLA_POSICIONES_SUBPOSICIONES = "TABLA_POSICIONES_SUBPOSICIONES";
        public const string LISTADO_ADJUNTOS = "LISTADO_ADJUNTOS";
    }
}
