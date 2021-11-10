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
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.html;
using Image = iTextSharp.text.Image;
using SustitucionMOAAssets;
using System.IO.Compression;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Helpers;
using System.Web.UI.WebControls;
using System.Data;

using SustitucionMOAWS.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class ComprasService : IComprasService
    {
        private readonly IRepositorio repositorio;
        private readonly IObtenerCecoSolpConsumerMOA CecoSolpConsumerMOA;
        private readonly IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA;
        private readonly IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA;
        private readonly IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA;
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;
        private readonly ICrearSolpConsumerMOA crearSolpConsumerMOA;
        private readonly IModificarSolpConsumerMOA modificarSolpConsumerMOA;

        private readonly string rutaArchivosCompras = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private static readonly string EMAIL_TEMPLATE_SOLP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Solp.html");


        public ComprasService(IRepositorio repositorio, IObtenerCecoSolpConsumerMOA CecoSolpConsumerMOA,
            IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA, IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA, IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA,
            IObtenerSolpConsumerMOA obtenerSolpConsumerMOA, ICrearSolpConsumerMOA crearSolpConsumerMOA, IModificarSolpConsumerMOA modificarSolpConsumerMOA)
        {
            this.repositorio = repositorio;
            this.CecoSolpConsumerMOA = CecoSolpConsumerMOA;
            this.cuentasSolpConsumerMOA = cuentasSolpConsumerMOA;
            this.ordenesSolpConsumerMOA = ordenesSolpConsumerMOA;
            this.serviciosSolpConsumerMOA = serviciosSolpConsumerMOA;
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
            this.crearSolpConsumerMOA = crearSolpConsumerMOA;
            this.modificarSolpConsumerMOA = modificarSolpConsumerMOA;
        }

        public RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
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
                includes.Add(x => x.Posiciones.Select(y => y.Subposiciones));
                includes.Add(x => x.UsuarioCreacion);
                includes.Add(x => x.UsuarioModificacion);

                solpEntity = repositorio.Obtener<Solp>(solp.Id.Value);

                if (solpEntity != null)
                {
                    //TODO: validar si está en un estado modificable

                    solpEntity.UsuarioModificacion_Id = solp.UsuarioActual.Id;
                    solpEntity.FechaModificacion = DateTime.Now;

                    //solpEntity.NroSolp = solp.NroSolp;

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

                solpEntity.NroSolp = solp.NroSolp;

                repositorio.Agregar(solpEntity);
            }

            //solpEntity.NroSolp = solp.NroSolp;

            pliegoEntity.RevisadoPor = solp.RevisadoPor;

            if (solpEntity != null)
            {
                if (solp.ClaseDocumento != null)
                    solpEntity.ClaseDocumento = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.ClaseDocumento && x.Codigo == solp.ClaseDocumento.Codigo);

                if (solp.TipoSolp != null)
                    solpEntity.TipoSolp = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoSolp && x.Codigo == solp.TipoSolp.Codigo);
                solpEntity.PasoCompletado = solp.PasoCompletado;
                solpEntity.UsuarioCompras_Id = solp.UsuarioCompras.Id; 
                solpEntity.EstadoPasos = solp.EstadoPasos;
                pliegoEntity.NombreObra = solp.NombreDeObra;
                pliegoEntity.FiscalContrato = solp.FiscalContrato;
                pliegoEntity.Telefono = solp.Telefono;
                pliegoEntity.Email = solp.Email;
                pliegoEntity.FechaHoraEntrega = solp.FechaHoraEntrega?.ToLocalTime();
                //pliegoEntity.SupervisorSector = solp.SupervisorSector;
                //pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo;
                pliegoEntity.SupervisorSector = solp.SupervisorSector != null ? string.Join(",", solp.SupervisorSector.Select(x => x)) : string.Empty;
                pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo != null ? string.Join(",", solp.SupervisorTrabajo.Select(x => x)) : string.Empty;

                pliegoEntity.TieneVisitaObra = solp.TieneVisitaObra;
                pliegoEntity.TieneVisitaObraMasiva = solp.TieneVisitaObraMasiva;
                pliegoEntity.TieneObradores = solp.TieneObradores;
                pliegoEntity.TieneMedioElevacion = solp.TieneMedioElevacion;
                pliegoEntity.TieneAndamio = solp.TieneAndamio;
                pliegoEntity.TieneTecnicoSeguridad = solp.TieneTecnicoSeguridad;
                pliegoEntity.TieneDescripcionTecnica = solp.TieneDescripcionTecnica;
                pliegoEntity.TieneDocumentacionTecnica = solp.TieneDocumentacionTecnica;
                pliegoEntity.FechaHoraLimiteConsulta = solp.FechaHoraLimiteConsulta?.ToLocalTime();
                pliegoEntity.ObservacionesGeneracion = solp.ObservacionesGeneracion;

                pliegoEntity.DiasEjecucion = solp.DiasEjecucion;
                pliegoEntity.JornadaLaboralDias = solp.JornadaLaboral != null ? string.Join(",", solp.JornadaLaboral.Select(x => (int)x)) : string.Empty;
                pliegoEntity.JornadaLaboralHorasDesde = solp.JornadaLaboralDesde?.ToLocalTime();
                pliegoEntity.JornadaLaboralHorasHasta = solp.JornadaLaboralHasta?.ToLocalTime();
                pliegoEntity.ObservacionesCotizacion = solp.ObservacionesCotizacion;

                pliegoEntity.TieneCondicionesGenerales = solp.TieneCondicionesGenerales.HasValue ? solp.TieneCondicionesGenerales : true;

                pliegoEntity.RevisadoPor = solp.RevisadoPor;

                if (solp.TieneVisitaObraMasiva && solp.VisitasObraMasiva != null)
                {
                    if (pliegoEntity.VisitasMasivas == null)
                    {
                        pliegoEntity.VisitasMasivas = new List<PliegoVisita>();
                    }
                    var idVisita = -1;

                    foreach (var visita in solp.VisitasObraMasiva)
                    {
                        var visitaExistente = pliegoEntity.VisitasMasivas.FirstOrDefault(x => x.Codigo == visita.Codigo);

                        if (visitaExistente != null)
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
                            });
                        }
                    }
                }

                if (solp.Adjuntos != null && pliegoEntity.Archivos != null)
                {
                    var archivosParaBorrar = pliegoEntity.Archivos
                        .Where(x => !solp.Adjuntos.Select(y => y.Id).Contains(x.Id))
                        .ToList();

                    foreach (var archivo in archivosParaBorrar)
                    {
                        repositorio.Remover<Archivo>(archivo);
                    }
                }

                if (pliegoEntity.Archivos == null)
                {
                    pliegoEntity.Archivos = new List<Archivo>();
                }

                if (solpEntity.Posiciones == null)
                {
                    solpEntity.Posiciones = new List<SolpPosicion>();
                }



                //posiciones eliminadas
                if (solpEntity.Posiciones.Count > 0)
                {
                    var posEliminadas = solpEntity.Posiciones.Where(x => !x.FechaBaja.HasValue).Where(x => solp.Posiciones == null || !solp.Posiciones.Any(y => y.Codigo == x.Codigo));

                    foreach (var pos in posEliminadas)
                    {
                        pos.FechaBaja = DateTime.Now;
                    }
                }

                if (solp.Posiciones != null)
                {
                    //posiciones nuevas y actualizadas
                    foreach (var pos in solp.Posiciones)
                    {
                        SolpPosicion posEntity = null;

                        posEntity = solpEntity.Posiciones.FirstOrDefault(y => y.Codigo == pos.Codigo);

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
                        posEntity.Estado = pos.Estado;
                        posEntity.Indice = pos.Indice;

                        posEntity.CalleEntrega = pos.CalleEntrega;
                        posEntity.NombreEntrega = pos.NombreEntrega;
                        posEntity.CpEntrega = pos.CpEntrega;
                        posEntity.NumeroEntrega = pos.NumeroEntrega;
                        posEntity.PaisEntrega = pos.PaisEntrega;
                        posEntity.PlazoEntrega = pos.PlazoEntrega;
                        posEntity.Solicitante = pos.Solicitante;

                        if (pos.TipoPosicion != null)
                            posEntity.TipoPosicion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoPosicionSolp && x.Codigo == pos.TipoPosicion.Codigo);

                        if (pos.TipoImputacion != null)
                            posEntity.TipoImputacion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoImputacionSolp && x.Codigo == pos.TipoImputacion.Codigo);

                        if (pos.Almacen != null)
                            posEntity.Almacen = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Almacen && x.Codigo == pos.Almacen.Codigo);

                        if (pos.Centro != null)
                            posEntity.Centro = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Centro && x.Codigo == pos.Centro.Codigo);

                        if (pos.GrupoCompras != null)
                            posEntity.GrupoCompras = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoCompras && x.Codigo == pos.GrupoCompras.Codigo);

                        if (pos.GrupoArticulo != null)
                            posEntity.GrupoArticulo = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoArticulo && x.Codigo == pos.GrupoArticulo.Codigo);

                        if (pos.Moneda != null)
                            posEntity.Moneda = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Moneda && x.Codigo == pos.Moneda.Codigo);

                        if (posEntity.Subposiciones == null)
                            posEntity.Subposiciones = new List<SolpSubposicion>();

                        //subposiciones eliminadas 
                        if (posEntity.Subposiciones.Count > 0)
                        {
                            var subposEliminadas = posEntity.Subposiciones.Where(x => pos.Subposiciones == null || !pos.Subposiciones.Any(y => y.Codigo == x.Codigo));

                            foreach (var subpos in subposEliminadas.ToList())
                            {
                                repositorio.Remover(subpos);
                            }
                        }

                        if (pos.Subposiciones != null)
                        {
                            foreach (var subpos in pos.Subposiciones)
                            {
                                SolpSubposicion subposEntity = null;

                                subposEntity = posEntity.Subposiciones.FirstOrDefault(y => y.Codigo == subpos.Codigo);

                                if (subposEntity == null)
                                    subposEntity = new SolpSubposicion();

                                subposEntity.Codigo = subpos.Codigo;
                                subposEntity.Cantidad = subpos.Cantidad;

                                if (subpos.TipoImputacionValor != null)
                                    subposEntity.TipoImputacionSap = repositorio.Obtener<TablaSap>(x => x.Tabla == subpos.TipoImputacionValor.Tabla && x.Codigo == subpos.TipoImputacionValor.Codigo);

                                if (subpos.CuentaMayor != null)
                                    subposEntity.CuentaMayorSap = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.CuentasSolpSap && x.Codigo == subpos.CuentaMayor.Codigo);


                                subposEntity.Numero = subpos.Numero;
                                subposEntity.PrecioBruto = subpos.PrecioBruto;
                                subposEntity.Tarea = subpos.Tarea;


                                if (subpos.Unidad != null)
                                    subposEntity.Unidad = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Unidad && x.Codigo == subpos.Unidad.Codigo);

                                if (subpos.CodigoServicioSap != null)
                                    subposEntity.ServicioSolp = repositorio.Obtener<ServicioSolp>(x => x.CodigoSap == subpos.CodigoServicioSap.Codigo);


                                posEntity.Subposiciones.Add(subposEntity);
                            }
                        }

                        if (posEntity.Proveedores == null)
                            posEntity.Proveedores = new List<SolpProveedor>();

                        //proveedores eliminados 
                        if (posEntity.Proveedores.Count > 0)
                        {
                            var provEliminados = posEntity.Proveedores.Where(x => pos.Proveedores == null || !pos.Proveedores.Any(y => y.RazonSocial == x.RazonSocial));

                            foreach (var prov in provEliminados.ToList())
                            {
                                repositorio.Remover(prov);
                            }
                        }

                        if (pos.Proveedores != null)
                        {
                            foreach (var prov in pos.Proveedores)
                            {
                                SolpProveedor provEntity = null;

                                provEntity = posEntity.Proveedores.FirstOrDefault(x => x.RazonSocial == prov.RazonSocial);

                                if (provEntity == null)
                                    provEntity = new SolpProveedor();

                                provEntity.RazonSocial = prov.RazonSocial;
                                //pendiente otros campos

                                if (prov.TipoFiltroProveedorSolp != null)
                                    provEntity.TipoFiltroProveedorSolp = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoFiltroSolpProveedor && x.Codigo == prov.TipoFiltroProveedorSolp.Codigo);

                                posEntity.Proveedores.Add(provEntity);
                            }
                        }

                        solpEntity.Posiciones.Add(posEntity);
                    }

                }
            }

            repositorio.GuardarCambios();

            solp.Id = solpEntity.Id;

            var rutaArchivo = string.Concat(ObtenerRutaArchivos(solpEntity.Id), "/", FileKeys.EspecificacionesTecnicasPliego, ".txt");

            Directory.CreateDirectory(ObtenerRutaArchivos(solpEntity.Id));
            File.WriteAllText(rutaArchivo, solp.EspecificacionesTecnicas);

            var archivoEspecificacionesTecnicasPliego = pliegoEntity.Archivos.FirstOrDefault(x => x.FileKey == FileKeys.EspecificacionesTecnicasPliego);

            if (archivoEspecificacionesTecnicasPliego == null)
            {
                pliegoEntity.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.EspecificacionesTecnicasPliego,
                    Ruta = rutaArchivo,
                });
            }

            GuardarAdjuntosSolp(solp, adjuntos, pliegoEntity);

            repositorio.GuardarCambios();

            solp.Adjuntos = pliegoEntity.Archivos.Where(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(x => new ArchivoDto()
            {
                Id = x.Id,
                FileKey = x.FileKey,
                Nombre = x.ObtenerNombre(x.Ruta)
            }).ToList();

            var respuestaGuardarSOLP = new RespuestaGuardarSOLP
            {
                Solp = solp
            };

            respuestaGuardarSOLP.Solp.NroSolp = solpEntity.NroSolp?? "";


            if (solp.Finalizar)
            {
                if (string.IsNullOrEmpty(solpEntity.NroSolp))
                {
                    var resultadoCrearSolp = crearSolpConsumerMOA.Request(solpEntity);

                    respuestaGuardarSOLP.Errores = new List<string>();

                    foreach (var error in resultadoCrearSolp.Errores.Where(x => x.Tipo == "E"))
                    {
                        var mensaje = error.Mensaje.Trim();
                        respuestaGuardarSOLP.Errores.Add(mensaje);
                    }

                    respuestaGuardarSOLP.IdEntidad = solp.Id.Value;

                    if (respuestaGuardarSOLP.Errores.Count == 0)
                    {
                        respuestaGuardarSOLP.Mensaje = "OK";
                        solpEntity.NroSolp = resultadoCrearSolp.NumeroSolp;
                        respuestaGuardarSOLP.Solp.NroSolp = resultadoCrearSolp.NumeroSolp;

                        var estadoCreadoCodigo = EstadoDocumentoSolp.Creado.Code();
                        var estadoCreado = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoCreadoCodigo);
                        solpEntity.EstadoDocumento_Id = estadoCreado.Id;

                    }

                    repositorio.GuardarCambios();
                }
                else
                {
                    var resultadoEditarSolp = modificarSolpConsumerMOA.Request(solpEntity);

                    respuestaGuardarSOLP.Errores = new List<string>();

                    foreach (var error in resultadoEditarSolp.Errores.Where(x => x.Tipo == "E"))
                    {
                        var mensaje = error.Mensaje.Trim();
                        respuestaGuardarSOLP.Errores.Add(mensaje);
                    }

                    respuestaGuardarSOLP.IdEntidad = solp.Id.Value;

                    if (respuestaGuardarSOLP.Errores.Count == 0)
                    {
                        respuestaGuardarSOLP.Mensaje = "OK";
                    }

                    repositorio.GuardarCambios();
                }
            }

            return respuestaGuardarSOLP;
        }

        private string ObtenerRutaArchivos(int solpId)
        {
            return string.Format("{0}/Solp_{1}", rutaArchivosCompras, solpId);
        }



        private void GuardarAdjuntosSolp(SolpDto solp, HttpFileCollectionBase files, Pliego pliego)
        {
            var ruta = ObtenerRutaArchivos(solp.Id.Value);

            var filesEspecificaciones = files.GetMultiple("fileEspecificaciones");
            for (int i = 0; i < filesEspecificaciones.Count; i++)
            {
                var file = filesEspecificaciones[i];
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

            var filesCotizaciones = files.GetMultiple("fileCotizaciones");
            for (int i = 0; i < filesCotizaciones.Count; i++)
            {
                var file = filesCotizaciones[i];
                var rutaArchivo = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                if (File.Exists(rutaArchivo))
                {
                    file.SaveAs(rutaArchivo);
                    continue;
                }

                Directory.CreateDirectory(ruta);

                pliego.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.AdjuntoCotizacionesSolp,
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
            return repositorio.Listar<TablaSap>(x => x.Tabla == tabla).Select(x => new TablaSapDto(x)).ToList();
        }

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x => new CentroDireccionDto(x)).ToList();
        }


        public List<SolpDto> ListarSolp(UsuarioDto usuarioActual)
        {
            Expression<Func<Solp, bool>> filtro = x => x.FechaBorrado == null && x.UsuarioCreacion_Id == usuarioActual.Id;

            if (usuarioActual.Permisos.Contains("VER TODAS SOLPS"))
                filtro = (x => x.FechaBorrado == null);

            var todasLasSolp = repositorio.Listar(filtro)
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
                    TipoSolp = x.TipoSolp != null ? new TablaGeneralDto(x.TipoSolp) : new TablaGeneralDto(),
                    VincularPliego = !x.Pliego_Id.HasValue,
                    TieneCondicionesGenerales = x.Pliego?.TieneCondicionesGenerales,
                    RevisadoPor = x.Pliego?.RevisadoPor
                }).OrderByDescending(i => i.FechaCreacion);

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
                TipoSolp = x.TipoSolp != null ? new TablaGeneralDto(x.TipoSolp) : new TablaGeneralDto(),
                VincularPliego = !x.Pliego_Id.HasValue,
                UsuarioCompras = x.UsuarioCompras != null ? new UsuarioComprasDto(x.UsuarioCompras) : new UsuarioComprasDto(),


                NombreDeObra = x.Pliego.NombreObra,
                FiscalContrato = x.Pliego.FiscalContrato,
                Telefono = x.Pliego.Telefono,
                Email = x.Pliego.Email,
                FechaHoraEntrega = x.Pliego.FechaHoraEntrega,
                SupervisorSector = x.Pliego.SupervisorSector.Split(',').ToList(),
                SupervisorTrabajo = x.Pliego.SupervisorTrabajo.Split(',').ToList(),
                VisitasObraMasiva = x.Pliego.VisitasMasivas.Select(a => new VisitaObraDto(a)).ToList(),
                TieneVisitaObra = x.Pliego.TieneVisitaObra ?? false,
                TieneVisitaObraMasiva = x.Pliego.TieneVisitaObraMasiva ?? false,
                TieneObradores = x.Pliego.TieneObradores ?? false,
                TieneMedioElevacion = x.Pliego.TieneMedioElevacion ?? false,
                TieneAndamio = x.Pliego.TieneAndamio ?? false,
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
                
                Adjuntos = x.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),

                EspecificacionesTecnicas = x.Pliego.Archivos.FirstOrDefault(a => a.FileKey == FileKeys.EspecificacionesTecnicasPliego)?.Ruta,

                TieneCondicionesGenerales = x.Pliego.TieneCondicionesGenerales ?? true,

                RevisadoPor = x.Pliego.RevisadoPor,

                EstadoSolpSapId = x.EstadoSolpSap_Id,
                EstadoDocumentoId = x.EstadoDocumento_Id,
                //Posiciones = x.Posiciones.Where(p => !p.FechaBaja.HasValue).Select(p => new SolpPosicionDto(p)).ToList(),
                Posiciones = x.Posiciones.Select(p => new SolpPosicionDto(p)).ToList(),
                PasoCompletado = x.PasoCompletado,
                EstadoPasos = x.EstadoPasos
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
            var solp = TraerSolpId(idSolp);

            var usuarioCompras = ListarUsuarioCompras(solp.UsuarioActual);

            var templateFilePath = solp.TieneCondicionesGenerales ?? true ? Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/PliegoSolpTemplate.html") :
               Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/NewPliegoSolpSinCondicionesTemplate.html");
            var templateString = System.IO.File.ReadAllText(templateFilePath);
            //, "Templates/PliegoSolpSinCondicionesTemplate.html"


            var templateCssFilePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/PliegoSolpTemplate.css");
            var templateCssString = System.IO.File.ReadAllText(templateCssFilePath);

            var solpValores = new Dictionary<string, string>();

            //aca va la asignacion de valores de la solp que se van a reemplazar en el documento
            solpValores.Add(SolpTemplateKeys.FECHA_LIBERACION, ""); //crear campo fecha de liberacion en tabla
            solpValores.Add(SolpTemplateKeys.NOMBRE_OBRA, solp.NombreDeObra);
            solpValores.Add(SolpTemplateKeys.NRO_SOLP, solp.NroSolp);
            solpValores.Add(SolpTemplateKeys.FISCAL_CONTRATO, solp.FiscalContrato);
            solpValores.Add(SolpTemplateKeys.TELEFONO, solp.Telefono);

            solpValores.Add(SolpTemplateKeys.FECHA_PRESENTACION, solp.FechaCreacion.ToString("dd-MM-yyyy"));
            solpValores.Add(SolpTemplateKeys.USUARIO_COMPRAS, solp.UsuarioCompras.Mail == null ? usuarioCompras.Any() ? usuarioCompras[0].UsuarioCompras.Mail : String.Empty : solp.UsuarioCompras.Mail);

            solpValores.Add(SolpTemplateKeys.REVISADO_POR, solp.RevisadoPor);

            //solpValores.Add(SolpTemplateKeys.PAGINAS, PageEventHandler.p);

            //ESPECIFICACION TECNICA DE TAREAS
            if (!string.IsNullOrEmpty(solp.EspecificacionesTecnicas) && System.IO.File.Exists(solp.EspecificacionesTecnicas))
            {
                solp.EspecificacionesTecnicas = System.IO.File.ReadAllText(solp.EspecificacionesTecnicas);
                solpValores.Add(SolpTemplateKeys.ESPECIFICACION_TECNICA, solp.EspecificacionesTecnicas.Replace("<br>", "<br />"));
            }
            else
            {
                solpValores.Add(SolpTemplateKeys.ESPECIFICACION_TECNICA, " ");
            }

            //COTIZACION Y PLAZO DE EJECUCION
            solpValores.Add(SolpTemplateKeys.PLAZO_EJECUCION, solp.DiasEjecucion?.ToString());

            //solpValores.Add(SolpTemplateKeys.DIAS_JORNADA_LABORAL, solp.JornadaLaboral.Select(a => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a)).ToList());
            //solpValores.Add(SolpTemplateKeys.DIAS_JORNADA_LABORAL) = solp.JornadaLaboralDias.Split(",".ToCharArray()).Select(a => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a)).ToList();

            if (solp.JornadaLaboral.Count > 0)
            {
                var diasOrdenado = solp.JornadaLaboral.OrderBy(a => a).ToList();
                var tieneHuecos = false;
                var diaAnterior = diasOrdenado.First();

                foreach (var dia in diasOrdenado)
                {
                    tieneHuecos = dia - diaAnterior > 1;
                    diaAnterior = dia;

                    if (tieneHuecos)
                    {
                        break;
                    }
                }

                var diasJornada = string.Empty;

                if (tieneHuecos || diasOrdenado.Count() == 1)
                {
                    diasJornada = string.Join(",", diasOrdenado.Select(a => getDia(a)).ToList());
                }
                else
                {
                    diasJornada = string.Format("{0} a {1}", getDia(diasOrdenado.First()), getDia(diasOrdenado.Last()));
                }
                solpValores.Add(SolpTemplateKeys.DIAS_JORNADA_LABORAL, diasJornada);

            }
            else
            {

                solpValores.Add(SolpTemplateKeys.DIAS_JORNADA_LABORAL, " ");
            }




            if (solp.JornadaLaboralDesde.HasValue && solp.JornadaLaboralHasta.HasValue)
            {
                var jornadaLaboral = string.Format("{0} a {1}", solp.JornadaLaboralDesde.Value.ToString("HH:mm"), solp.JornadaLaboralHasta.Value.ToString("HH:mm"));
                solpValores.Add(SolpTemplateKeys.INICIO_FINAL_HS_JORNADA_LABORAL, jornadaLaboral);
            }
            else
            {
                solpValores.Add(SolpTemplateKeys.INICIO_FINAL_HS_JORNADA_LABORAL, " ");
            }

            string templateSubposiciones = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td></td></tr>";

            StringBuilder subposiciones = new StringBuilder();

            StringBuilder texto = new StringBuilder();

            string plazoEntrega = "{0}";

            StringBuilder plazo = new StringBuilder();


            solp.Posiciones.ForEach(pos =>
            {
                texto = new StringBuilder();

                plazo = new StringBuilder();

                plazo.AppendLine(string.Format(plazoEntrega,
                    pos.PlazoEntrega));


                pos.Subposiciones.ForEach(subpos =>
                {
                    texto.AppendLine(string.Format(templateSubposiciones,
                        subpos.Numero,
                        subpos.CodigoServicioSap?.Codigo,
                        subpos.Tarea,
                        subpos.Cantidad,
                        subpos.Unidad?.Descripcion
                        ));

                });

                subposiciones.AppendLine($"<tr><th class='posicion' colspan='6'> {pos.Indice} - {pos.TextoGenerico}</th></tr>{texto}<tr><td colspan='6'>&nbsp;</td></tr>");
            });

            solpValores.Add(SolpTemplateKeys.TABLA_POSICIONES_SUBPOSICIONES, subposiciones.ToString());
            solpValores.Add(SolpTemplateKeys.PLAZO_ENTREGA, plazo.ToString());






            //ADJUNTOS
            solpValores.Add(SolpTemplateKeys.LISTADO_ADJUNTOS, "");

            templateString = CombineTemplateValues(templateString, solpValores);

            return ConvertHtmlToPdf(templateString, templateCssString, solp); //agregar solp
        }

        private string getDia(DayOfWeek dia)
        {
            var diaStr = string.Empty;

            switch (dia)
            {
                case DayOfWeek.Sunday:
                    diaStr = "Domingo";
                    break;
                case DayOfWeek.Monday:
                    diaStr = "Lunes";
                    break;
                case DayOfWeek.Tuesday:
                    diaStr = "Martes";
                    break;
                case DayOfWeek.Wednesday:
                    diaStr = "Miércoles";
                    break;
                case DayOfWeek.Thursday:
                    diaStr = "Jueves";
                    break;
                case DayOfWeek.Friday:
                    diaStr = "Viernes";
                    break;
                case DayOfWeek.Saturday:
                    diaStr = "Sábado";
                    break;
                default:
                    break;
            }

            return diaStr;
        }


        private byte[] ConvertHtmlToPdf(string xHtml, string css, SolpDto solp)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 70f, 70f, 150f, 60f))
                {
                    //var solp = repositorio.Obtener<Solp>(idSolp);

                    var PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                    document.Open();

                    Image image = Image.GetInstance(Convert.FromBase64String("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"));
                    image.SetAbsolutePosition(180, 700);
                    PdfWriter.DirectContent.AddImage(image, false);

                    //PdfPTable table = new PdfPTable(2);

                    

                    // Our custom Header and Footer is done using Event Handler
                    TwoColumnHeaderFooter PageEventHandler = new TwoColumnHeaderFooter();
                    PdfWriter.PageEvent = PageEventHandler;

                    //define el header
                    //PageEventHandler.Title = "Revisado por: " + revisadoPor.

                    //string encabezado = "Revisado por: " + revisadoPor;

                    var tablaHeader = new PdfPTable(2);

                    tablaHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

                    PdfPCell obra = new PdfPCell();

                    obra.AddElement(new Paragraph($"Nombre obra: {solp.NombreDeObra}"));
                    tablaHeader.AddCell(obra);

                    var imagen = Image.GetInstance(@"https://b2cmoagro.blob.core.windows.net/moaoperaciones/logo.png");

                    //2284 x 1059
                    imagen.ScaleAbsolute(80f, 40f);
                    PdfPCell cellImagen = new PdfPCell(imagen);

                    cellImagen.FixedHeight = 50f;
                    cellImagen.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellImagen.HorizontalAlignment = Element.ALIGN_CENTER;
                    tablaHeader.AddCell(cellImagen);

                    PdfPCell nombres = new PdfPCell();

                    nombres.AddElement(new Paragraph($"Redactó: {solp.FiscalContrato}"));
                    nombres.AddElement(new Paragraph($"Aprobó: {solp.RevisadoPor}"));

                    tablaHeader.AddCell(nombres);

                    PdfPCell numeros = new PdfPCell();

                    numeros.AddElement(new Paragraph($"Fecha: {solp.FechaCreacion.ToString("dd/MM/yyyy")}"));

                    numeros.FixedHeight = 50f;

                    tablaHeader.AddCell(numeros);

                    PageEventHandler.HeaderWidths = new float[] { 3f, 1f };

                    PageEventHandler.Header = tablaHeader;

                    //PageEventHandler.Header.SetWidthPercentage(new float[] { 10, 10 }, document.PageSize);
                    //PageEventHandler.Header.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 36, writer.DirectContent);

                    //PageEventHandler.Title = document.Add(table).ToString();
                    //PageEventHandler.HeaderFont = FontFactory.GetFont(BaseFont.COURIER_BOLD, 10, Font.BOLD);
                   
                    // instantiate custom tag processor and add to `HtmlPipelineContext`.
                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                  
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                    var pdfWriterPipeline = new PdfWriterPipeline(document, PdfWriter);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);

                    // get an ICssResolver and add the custom CSS
                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    cssResolver.AddCss(css, "utf-8", true);
                    var cssResolverPipeline = new CssResolverPipeline(
                        cssResolver, htmlPipeline
                    );

                    var worker = new XMLWorker(cssResolverPipeline, true);
                    var parser = new XMLParser(worker);
                    using (var stringReader = new StringReader(xHtml))
                    {
                        parser.Parse(stringReader);
                        document.Close();
                        byte[] bytes = stream.ToArray();
                        stream.Close();
                        return bytes;
                    }

                }

            }

        }

        //public void AddOutline(PdfWriter writer, string Title, float Position)
        //{
        //    PdfDestination destination = new PdfDestination(PdfDestination.FITH, Position);
        //    PdfOutline outline = new PdfOutline(writer.DirectContent.RootOutline, destination, Title);
        //    writer.DirectContent.AddOutline(outline, "Name = " + Title);
        //}

        public string GenerarZipPliego(int idSolp, string pathBase)
        {
            var solp = repositorio.Obtener<Solp>(idSolp);

            var middleFileName = solp.NroSolp == null ? (solp.Pliego.NombreObra == null ? "xxxx" : solp.Pliego.NombreObra) : solp.NroSolp;
            var pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now.ToString("yyyyMMdd")}.pdf";
            var pdfFilePath = $"{pathBase}/{pdfFilename}";
            File.WriteAllBytes(pdfFilePath, GenerarSolpPdf(idSolp));

            if (solp.Pliego.Archivos != null && solp.Pliego.Archivos.Any<Archivo>(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp))
            {
                var zipFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now.ToString("yyyyMMdd")}.zip";
                var filePath = $"{pathBase}/{zipFilename}";

                using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        foreach (var archivoSubido in solp.Pliego.Archivos)
                        {
                            if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp))
                            {
                                string fileName = Path.GetFileName(archivoSubido.Ruta);
                                archivo.CreateEntryFromFile(archivoSubido.Ruta, fileName);
                            }
                        }

                        archivo.CreateEntryFromFile(pdfFilePath, pdfFilename);

                    }
                }

                return filePath;
            }

            return pdfFilePath;
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

        public List<TablaSapDto> ObtenerServiciosSap()
        {
            ServicioWSMOAResponse resultSap = (ServicioWSMOAResponse)serviciosSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Servicios.Select(s => new TablaSapDto()
            {
                Tabla = TablasSap.CodigoServicioSap,
                Descripcion = s.Descripcion,
                CodigoSap = int.TryParse(s.Codigo, out codigoNum) ? codigoNum.ToString() : s.Codigo,
                Codigo = s.Codigo
            }).ToList();
        }

        public List<TablaSapDto> ObtenerCuentasSap()
        {
            CuentaWSMOAResponse resultSap = (CuentaWSMOAResponse)cuentasSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cuentas.Select(c => new TablaSapDto()
            {
                Tabla = TablasSap.CuentasSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            }).ToList();
        }

        public List<TablaSapDto> ObtenerOrdenesSap()
        {
            OrdenWSMOAResponse resultSap = (OrdenWSMOAResponse)ordenesSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Ordenes.Select(c => new TablaSapDto()
            {
                Tabla = TablasSap.OrdenSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            }).ToList();
        }

        public List<TablaSapDto> ObtenerCecoSap()
        {
            CecoWSMOAResponse resultSap = (CecoWSMOAResponse)CecoSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cecos.Select(c => new TablaSapDto()
            {
                Tabla = TablasSap.CecoSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.CostCenter, out codigoNum) ? codigoNum.ToString() : c.CostCenter,
                Codigo = c.CostCenter
            }).ToList();
        }

        public List<TablaSapDto> AutocompleteTablaSap(string tabla, string valor)
        {
            var lista = repositorio.Listar<TablaSap>(x => x.Tabla == tabla)
            .FindAll(e => e.Descripcion.ToLower().Contains(valor.ToLower()) || e.CodigoSap.ToLower().Contains(valor.ToLower()))
            .Select(s => new TablaSapDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                CodigoSap = s.CodigoSap,
                Codigo = s.Codigo,
                Tabla = s.Tabla
            }).ToList();

            return lista;
        }

        public List<ServicioSolpDto> AutocompleteServicioSolp(string valor)
        {
            List<ServicioSolpDto> lista = repositorio.Listar<ServicioSolp>()
                .FindAll(e => e.Descripcion.ToLower().Contains(valor.ToLower()) || e.CodigoSap.ToString().ToLower().Contains(valor.ToLower()))
                .Select(s => new ServicioSolpDto(s)).ToList();

            return lista;
        }

        private void EnviarMailSolp(Solp solp, Usuario usuario)
        {
            try
            {
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_SOLP);
                string asunto = "MOA COMPRAS - Solp Liberada";

                var cuerpo = string.Format(cuerpoTemplate, solp.NroSolp, usuario.Mail);
                var Destinatario = usuario.Mail;



                EmailSender.EnviarMail(new List<string> { Destinatario }, asunto, cuerpo, null, null, null, null);
            }
            catch (Exception e)
            {
            }
        }

        public ObtenerSolpSAPResponse ObtenerSolpsSAP(ObtenerSolpRequest obtenerSolpRequest)
        {
            var solps = obtenerSolpConsumerMOA.Request(obtenerSolpRequest);

            return solps;
        }

        public List<TablaSapDto> ObtenerDatosPorCodigosSap(List<TablaSapDto> codigos)
        {
            var ret = new List<TablaSapDto>();
            var codigosPorTabla = new Dictionary<string, List<string>>();

            foreach (var cod in codigos)
            {
                if (!codigosPorTabla.ContainsKey(cod.Tabla))
                {
                    codigosPorTabla.Add(cod.Tabla, new List<string>());
                }

                codigosPorTabla[cod.Tabla].Add(cod.CodigoSap);
            }

            foreach (var tabla in codigosPorTabla)
            {
                var lista = repositorio.Listar<TablaSap>(x => x.Tabla == tabla.Key && tabla.Value.Contains(x.CodigoSap))
                    .Select(x => new TablaSapDto(x)).ToList();
                ret.AddRange(lista);
            }

            return ret;
        }

        public List<ServicioSolpDto> ObtenerDatosPorCodigosSapServicioSolp(List<string> codigos)
        {
            var result = repositorio
                .Listar<ServicioSolp>(x => codigos.Contains(x.CodigoSap.ToString()))
                .Select(x => new ServicioSolpDto(x)).ToList();

            return result;
        }

        public void ActualizarFechaLiberacion(string nrosolp, DateTime fechaLiberacion)
        {
            var solp = repositorio.Obtener<Solp>(x => x.NroSolp == nrosolp);

            if (solp != null)
            {
                solp.FechaLiberacionSap = fechaLiberacion;
                repositorio.GuardarCambios();
            }
        }

        public void ActualizarServiciosSolp()
        {
            ServicioWSMOAResponse resultSap = (ServicioWSMOAResponse)serviciosSolpConsumerMOA.request();
            if (resultSap.Servicios.Any())
            {
                var listaBase = repositorio.Listar<ServicioSolp>();

                foreach (var servicio in resultSap.Servicios)
                {
                    int codigoNum = 0;
                    if (Int32.TryParse(servicio.Codigo, out codigoNum) &&
                        !listaBase.Any(x => x.CodigoSap == codigoNum && x.Descripcion == servicio.Descripcion))
                    {
                        repositorio.Agregar(new ServicioSolp
                        {
                            Codigo = servicio.Codigo,
                            CodigoSap = codigoNum,
                            Descripcion = servicio.Descripcion,
                            GrupoArticulos = Int32.TryParse(servicio.NroGrupo, out int grupoArticulos) ? grupoArticulos : (int?)null,
                            TipoServicio = servicio.Serv,
                            AmbitoServicio = servicio.Ser,
                            Edicion = Int32.TryParse(servicio.Edit, out int edicion) ? edicion : 0,
                            UnidadMedidaBase = servicio.Bas,
                            SSCItem = servicio.SSCItem
                        });
                    }
                }
            }

            repositorio.GuardarCambios();
        }

        public void ActualizarEstadoSolpBulk()
        {
            var lista = repositorio.Listar<TablaSap>(x => x.Tabla == "EstadoSolpSap")
                    .Select(x => new TablaSapDto(x)).ToList();
            foreach (var todasLasSolp in repositorio.Listar<Solp>(o => o.FechaBorrado == null && o.NroSolp != null))
            {
                ConsultaEstadoSolp(todasLasSolp.NroSolp, lista);
            }
        }

        private void ConsultaEstadoSolp(string nroSolp, List<TablaSapDto> listaTablaSap)
        {
            DateTime fechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString());
            DateTime fechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinConsultaSolp"].ToString());
            var filtros = new ObtenerSolpRequest
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                NumeroSolp = nroSolp,
            };
            var solp = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(filtros);
            if (solp.Posiciones.Any())
            {
                var codigoSap = listaTablaSap.Where(x => x.CodigoSap == solp.Posiciones[0].EstadoSolpSap).FirstOrDefault();
                if(codigoSap != null)
                {
                    ActualizarEstadoSolp(nroSolp, codigoSap.Id);
                }
            }
        }

        public void ActualizarEstadoSolp(string nroSolp, int idEstado)
        {
            var solp = repositorio.Obtener<Solp>(x => x.NroSolp == nroSolp);

            if (solp != null)
            {
                solp.EstadoSolpSap_Id = idEstado;
                repositorio.GuardarCambios();
            }
        }

        public List<UsuarioComprasRelacionConUsuariosDto> ListarUsuarioCompras(UsuarioDto usuarioActual)
        {
            var usuarios = repositorio.Listar<Usuario>().ToList();
            var usuariosCompras = repositorio.Listar<UsuarioComprasRelacionConUsuarios>().Where(item => item.Usuario_Id == usuarios[0].Id)
                .Select(x => new UsuarioComprasRelacionConUsuariosDto
                {
                    UsuarioCompras = new UsuarioComprasDto(x.UsuarioCompras)
                });

            return usuariosCompras.ToList();
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

        public const string TEXTO_GENERICO = "TEXTO_GENERICO";

        public const string REVISADO_POR = "REVISADO_POR";

        public const string PLAZO_ENTREGA = "PLAZO_ENTREGA";
        public const string PAGINAS = "PAGINAS";
    }
}
