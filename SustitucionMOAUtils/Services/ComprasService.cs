using HandlebarsDotNet;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Reporte;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ModificarOCWebServiceMOA;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using static SustitucionMOAWS.WSConsumers.ModificarOrdenDeCompraConsumerMOA;
using Image = iTextSharp.text.Image;


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
        private readonly IObtenerMaterialesSolpConsumerMOA obtenerMaterialesSolpConsumerMOA;
        private readonly ICrearPedidoConsumerMOA crearPedidoConsumerMOA;
        private readonly IObtenerFuenteAprovisionamientoConsumerMOA obtenerFuenteAprovisionamientoConsumerMOA;
        private readonly IObtenerContratoSolpConsumerMOA obtenerContratoSolpConsumerMOA;
        private readonly IVendedorService vendedorService;
        private readonly IObtenerTipoCambioConsumerMOA obtenerTipoCambioConsumerMOA;
        private readonly IHttpContextService httpContextService;
        private readonly IObtenerRegistroInfoConsumerMOA obtenerRegistroInfoConsumerMOA;
        private readonly IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA;
        private readonly IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA;
        private readonly IUsuarioService usuarioService;
        private readonly IObtenerProveedorConsumerMOA obtenerProveedorConsumerMOA;
        private readonly IModificarOrdenDeCompraConsumerMOA modificarOrdenDeCompraConsumerMOA;
        private readonly IVendedoresConsumerMOA vendedoresConsumerMOA;
        private readonly IAgregarRegistroInfoConsumerMOA agregarRegistroInfoConsumerMOA;
        private readonly IReporteOrdenDeCompraConsumerMOA reporteOrdenDeCompraConsumerMOA;
        private readonly IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaConsumerMOA;
        private readonly IListarSolpPendientesConsumerMOA listarSolpPendienteConsumeMOA;
        private readonly IObtenerPDFOrdenCompraConsumerMOA obtenerPDFOrdenCompraConsumerMOA;

        private readonly string rutaArchivosCompras = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly IEmailService emailService;
        //private static readonly string EMAIL_TEMPLATE_SOLP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Solp.html");


        public ComprasService(IRepositorio repositorio,
            IObtenerCecoSolpConsumerMOA CecoSolpConsumerMOA,
            IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA,
            IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA,
            IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA,
            IObtenerSolpConsumerMOA obtenerSolpConsumerMOA,
            ICrearSolpConsumerMOA crearSolpConsumerMOA,
            IModificarSolpConsumerMOA modificarSolpConsumerMOA,
            IObtenerMaterialesSolpConsumerMOA obtenerMaterialesSolpConsumerMOA,
            ICrearPedidoConsumerMOA crearPedidoConsumerMOA,
            IObtenerFuenteAprovisionamientoConsumerMOA obtenerFuenteAprovisionamientoConsumerMOA,
            IObtenerContratoSolpConsumerMOA obtenerContratoSolpConsumerMOA, IVendedorService vendedorService,
            IObtenerTipoCambioConsumerMOA obtenerTipoCambioConsumerMOA, IHttpContextService httpContextService,
            IObtenerRegistroInfoConsumerMOA obtenerRegistroInfoConsumerMOA,
            IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA,
            IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA,
            IUsuarioService usuarioService, IObtenerProveedorConsumerMOA obtenerProveedorConsumerMOA,
            IModificarOrdenDeCompraConsumerMOA modificarOrdenDeCompraConsumerMOA,
            IVendedoresConsumerMOA vendedoresConsumerMOA,
            IAgregarRegistroInfoConsumerMOA agregarRegistroInfoConsumerMOA,
            IEmailService emailService, IReporteOrdenDeCompraConsumerMOA reporteOrdenDeCompraConsumerMOA,
            IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaConsumerMOA,
            IListarSolpPendientesConsumerMOA listarSolpPendienteConsumeMOA,
            IObtenerPDFOrdenCompraConsumerMOA obtenerPDFOrdenCompraConsumerMOA)
        {
            this.repositorio = repositorio;
            this.CecoSolpConsumerMOA = CecoSolpConsumerMOA;
            this.cuentasSolpConsumerMOA = cuentasSolpConsumerMOA;
            this.ordenesSolpConsumerMOA = ordenesSolpConsumerMOA;
            this.serviciosSolpConsumerMOA = serviciosSolpConsumerMOA;
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
            this.crearSolpConsumerMOA = crearSolpConsumerMOA;
            this.modificarSolpConsumerMOA = modificarSolpConsumerMOA;
            this.obtenerMaterialesSolpConsumerMOA = obtenerMaterialesSolpConsumerMOA;
            this.crearPedidoConsumerMOA = crearPedidoConsumerMOA;
            this.obtenerFuenteAprovisionamientoConsumerMOA = obtenerFuenteAprovisionamientoConsumerMOA;
            this.obtenerContratoSolpConsumerMOA = obtenerContratoSolpConsumerMOA;
            this.vendedorService = vendedorService;
            this.obtenerTipoCambioConsumerMOA = obtenerTipoCambioConsumerMOA;
            this.httpContextService = httpContextService;
            this.obtenerRegistroInfoConsumerMOA = obtenerRegistroInfoConsumerMOA;
            this.obtenerOrdenDeCompraConsumerMOA = obtenerOrdenDeCompraConsumerMOA;
            this.obtenerOrdenesDeCompraParaSOLPConsumerMOA = obtenerOrdenesDeCompraParaSOLPConsumerMOA;
            this.usuarioService = usuarioService;
            this.obtenerProveedorConsumerMOA = obtenerProveedorConsumerMOA;
            this.modificarOrdenDeCompraConsumerMOA = modificarOrdenDeCompraConsumerMOA;
            this.vendedoresConsumerMOA = vendedoresConsumerMOA;
            this.agregarRegistroInfoConsumerMOA = agregarRegistroInfoConsumerMOA;
            this.emailService = emailService;
            this.reporteOrdenDeCompraConsumerMOA = reporteOrdenDeCompraConsumerMOA;
            this.obtenerUnidadesDeMedidaConsumerMOA = obtenerUnidadesDeMedidaConsumerMOA;
            this.listarSolpPendienteConsumeMOA = listarSolpPendienteConsumeMOA;
            this.obtenerPDFOrdenCompraConsumerMOA = obtenerPDFOrdenCompraConsumerMOA;
        }

        public RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
        {
            Solp solpEntity = null;
            Pliego pliegoEntity = null;
            SolpPosicion postEntitySubPosicionesEliminadas = null;
            bool enviarMailUrgencia = solp.Urgencia == true && solp.Finalizar && solp.TrabajoYaHecho != true;


            if (solp.Id.HasValue)
            {
                //es la forma de decirle a entity framework que tambien me traiga todas estas cosas
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
                    if (solpEntity.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || solpEntity.TipoSolpSap == (int?)TipoSolpSap.ReposicionAutomatica || solpEntity.TipoSolpSap == (int?)TipoSolpSap.Sap)
                    {
                        solpEntity.UsuarioCreacion_Id = solp.UsuarioActual.Id;
                    }
                    solpEntity.FechaModificacion = DateTime.Now;
                    pliegoEntity = solpEntity.Pliego;

                    if (enviarMailUrgencia && !string.IsNullOrEmpty(solpEntity.NroSolp))
                    {
                        enviarMailUrgencia = false;
                        if (solp.Posiciones.Count > solpEntity.Posiciones.Count) enviarMailUrgencia = true;
                        else
                            if (solpEntity.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "SERVICIO")
                        {
                            for (int i = 0; i < solpEntity.Posiciones.Count; i++)
                            {
                                if (solp.Posiciones[i].Subposiciones.Count > solpEntity.Posiciones.ToList()[i].Subposiciones.Count) enviarMailUrgencia = true;
                                else
                                    for (int j = 0; j < solpEntity.Posiciones.ToList()[i].Subposiciones.Count; j++)
                                    {
                                        if (solp.Posiciones[i].Subposiciones[j].Cantidad > solpEntity.Posiciones.ElementAt(i).Subposiciones.ElementAt(j).Cantidad
                                            || solp.Posiciones[i].Subposiciones[j].PrecioBruto > solpEntity.Posiciones.ElementAt(i).Subposiciones.ElementAt(j).PrecioBruto)
                                            enviarMailUrgencia = true;
                                    }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < solpEntity.Posiciones.Count; i++)
                            {
                                if (solp.Posiciones[i].Cantidad > solpEntity.Posiciones.ElementAt(i).Cantidad || solp.Posiciones[i].PrecioBruto > solpEntity.Posiciones.ElementAt(i).PrecioBruto)
                                {
                                    enviarMailUrgencia = true;
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                solpEntity = new Solp()
                {
                    UsuarioCreacion_Id = solp.UsuarioActual.Id,
                    FechaCreacion = DateTime.Now,
                    EmailLinkToken = Guid.NewGuid()
                };

                solp.TipoSolpSap = (int)TipoSolpSap.Web;
                var estadoIncompletoCodigo = EstadoDocumentoSolp.Incompleto.Code();
                var estadoIncompleto = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoIncompletoCodigo);
                solpEntity.EstadoDocumento_Id = estadoIncompleto.Id;
                solpEntity.Pliego = new Pliego();
                solpEntity.Posiciones = new List<SolpPosicion>();
                solpEntity.TrabajoYaHecho = solp.TrabajoYaHecho;
                solpEntity.CondEspProveedorAsignado = solp.CondEspProveedorAsignado;
                solpEntity.Adicional = solp.Adicional;
                solpEntity.Urgencia = solp.Urgencia;
                solpEntity.NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional;
                solpEntity.ProveedorAsignado_Id = solp.ProveedorAsignado_Id;
                pliegoEntity = solpEntity.Pliego;

                solpEntity.NroSolp = solp.NroSolp;
                solpEntity.THAjustePolinomica = solp.THAjustePolinomica;
                solpEntity.THProveedorDirecto = solp.THProveedorDirecto;
                solpEntity.THServicioPermanente = solp.THServicioPermanente;

                repositorio.Agregar(solpEntity);
            }
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
                solpEntity.TipoSolpSap = solp.TipoSolpSap;
                pliegoEntity.NombreObra = solp.NombreDeObra;
                solpEntity.TrabajoYaHecho = solp.TrabajoYaHecho;
                solpEntity.CondEspProveedorAsignado = solp.CondEspProveedorAsignado;
                solpEntity.Adicional = solp.Adicional;
                solpEntity.Urgencia = solp.Urgencia;
                solpEntity.NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional;
                solpEntity.ProveedorAsignado_Id = solp.ProveedorAsignado_Id;
                solpEntity.THAjustePolinomica = solp.THAjustePolinomica;
                solpEntity.THProveedorDirecto = solp.THProveedorDirecto;
                solpEntity.THServicioPermanente = solp.THServicioPermanente;
                pliegoEntity.FiscalContrato = solp.FiscalContrato;
                pliegoEntity.Telefono = solp.Telefono;
                pliegoEntity.Email = solp.Email;
                pliegoEntity.FechaHoraEntrega = solp.FechaHoraEntrega?.ToLocalTime();
                pliegoEntity.SupervisorSector = solp.SupervisorSector != null ? string.Join(",", solp.SupervisorSector.Select(x => x)) : string.Empty;
                pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo != null ? string.Join(",", solp.SupervisorTrabajo.Select(x => x)) : string.Empty;
                pliegoEntity.TieneVisitaObra = solp.TieneVisitaObra;
                pliegoEntity.TieneVisitaObraMasiva = solp.TieneVisitaObraMasiva;
                pliegoEntity.TieneObradores = solp.TieneObradores;
                pliegoEntity.TieneMedioElevacion = solp.TieneMedioElevacion;
                pliegoEntity.TieneAndamio = solp.TieneAndamio;
                pliegoEntity.TieneGrillaPersonal = solp.TieneGrillaPersonal;
                pliegoEntity.TieneFabricacionTallerExterno = solp.TieneFabricacionTallerExterno;
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

                    var visitasBorrar = pliegoEntity.VisitasMasivas
                        .Where(x => !solp.VisitasObraMasiva.Select(y => y.Codigo).Contains(x.Codigo))
                        .ToList();

                    foreach (var visita in visitasBorrar)
                    {
                        repositorio.Remover<PliegoVisita>(visita);
                    }

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
                // Identifica los que ya no estan en la base de datos y los borra
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

                solpEntity = PosicionesEliminar(solpEntity, solp);
                var codigos = solpEntity.Posiciones.Select(x => x.Codigo).ToList();

                // eliminar posiciones que no se grabaron (Eliminadas en el front). se fija que el estado este en false (osea borrado) y que esas posiciones no existan en la db
                solp.Posiciones = solp.Posiciones.Where(a => a.Estado == true || codigos.Contains(a.Codigo)).ToList();

                if (solp.Posiciones != null)
                {
                    solpEntity = PosicionesNuevasActualizadas(solpEntity, solp);
                }
            }


            if (solpEntity.LiberadoresSapSolp.Any())
                repositorio.RemoverTodos(solpEntity.LiberadoresSapSolp.ToList());
            if (solp.TrabajoYaHecho != true && solp.LiberadoresSapSolp.Any())
            {
                solpEntity.LiberadoresSapSolp = solp.LiberadoresSapSolp.Select(dto => new LiberadorSapSolp
                {
                    LiberadorSap_Id = dto.LiberadorSap_Id
                }).ToList();
            }

            string prefijo = ConfigurarPrefijos(solpEntity);

            if (solpEntity.Posiciones.Any() && solpEntity.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "SERVICIO")
            {
                foreach (var posicion in solpEntity.Posiciones)
                {
                    if (!posicion.Tarea.StartsWith(prefijo))
                    {
                        posicion.Tarea = (prefijo + posicion.Tarea);
                        if (posicion.Tarea.Length > 40)
                        {
                            posicion.Tarea.Substring(0, 40);
                        }
                    }
                }
            }


            repositorio.GuardarCambios();
            solp.Id = solpEntity.Id;

            pliegoEntity = GuardarEspecificacionesTecnicasPliego(solp, solpEntity, pliegoEntity);
            solp = GuardarAdjuntosSolp(solp, adjuntos, pliegoEntity);
            solp.EmailLinkToken = solpEntity.EmailLinkToken;
            var respuestaGuardarSOLP = new RespuestaGuardarSOLP
            {
                Solp = solp
            };
            respuestaGuardarSOLP.Solp.NroSolp = solpEntity.NroSolp ?? "";
            if (solp.Finalizar)
            {
                try
                {
                    var finalizoPrimeraVez = string.IsNullOrEmpty(solpEntity.NroSolp);
                    respuestaGuardarSOLP = FinalizarSolp(solpEntity, postEntitySubPosicionesEliminadas, respuestaGuardarSOLP, enviarMailUrgencia);
                    GuardarUsuarioComprasRelacionado(solp);
                }
                catch (Exception e)
                {
                    return new RespuestaGuardarSOLP { Solp = solp, IdEntidad = solpEntity.Id, Errores = new List<string> { e.Message }, Mensaje = e.Message };
                }
            }

            ActualizarPosiciones(solp, solpEntity);
            return respuestaGuardarSOLP;
        }

        private Solp PosicionesEliminar(Solp solpEntity, SolpDto solp)
        {
            if (solpEntity.Posiciones == null)
            {
                solpEntity.Posiciones = new List<SolpPosicion>();
            }

            //posiciones eliminadas, elimina posiciones que no llegan desde el front al back y no tienen fecha de baja
            if (solpEntity.Posiciones.Count > 0)
            {
                // trae todas que no esten en la web (las que vienen del front)
                var posEliminadas = solpEntity.Posiciones.Where(x => solp.Posiciones == null || !solp.Posiciones.Any(y => y.Codigo == x.Codigo));
                foreach (var pos in posEliminadas.ToList())
                {
                    pos.FechaBaja = DateTime.Now;
                    repositorio.Remover(pos);
                }
            }
            return solpEntity;
        }

        private Solp PosicionesNuevasActualizadas(Solp solpEntity, SolpDto solp)
        {
            SolpPosicion postEntitySubPosicionesEliminadas = null;
            //posiciones nuevas y actualizadas
            foreach (var pos in solp.Posiciones)
            {
                SolpPosicion posEntity = null;
                postEntitySubPosicionesEliminadas = null;
                posEntity = solpEntity.Posiciones.FirstOrDefault(y => y.Codigo == pos.Codigo);
                postEntitySubPosicionesEliminadas = solpEntity.Posiciones.FirstOrDefault(y => y.Codigo == pos.Codigo);
                if (posEntity == null)
                    posEntity = new SolpPosicion();

                List<SolpSubposicion> lstSubPosicion = posEntity.Subposiciones != null ? posEntity.Subposiciones.ToList() : new List<SolpSubposicion>();
                var lstSubPosicionesUnicas = lstSubPosicion.CloneList();
                posEntity.Codigo = pos.Codigo;
                posEntity.FechaEntregaServicio = pos.FechaEntregaServicio;
                posEntity.FechaLiberacion = pos.FechaLiberacion;
                posEntity.NroNecesidad = pos.NroNecesidad;
                posEntity.TextoSuministro = pos.TextoSuministro;
                posEntity.Motivo = pos.Motivo;
                posEntity.Modelo = pos.Modelo;
                posEntity.Estado = pos.Estado;
                posEntity.Indice = pos.Indice;
                posEntity.CalleEntrega = pos.CalleEntrega;
                posEntity.NombreEntrega = pos.NombreEntrega;
                posEntity.CpEntrega = pos.CpEntrega;
                posEntity.NumeroEntrega = pos.NumeroEntrega;
                posEntity.PaisEntrega = pos.PaisEntrega;
                posEntity.PlazoEntrega = pos.PlazoEntrega;
                posEntity.Solicitante = pos.Solicitante;
                posEntity.Tarea = pos.Tarea;
                posEntity.Cantidad = pos.Cantidad;
                if (pos.Unidad != null)
                    posEntity.Unidad = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Unidad && x.Codigo == pos.Unidad.Codigo);
                posEntity.PrecioBruto = pos.PrecioBruto;
                if (pos.CuentaMayor != null)
                    posEntity.CuentaMayorSap = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.CuentasSolpSap && x.Codigo == pos.CuentaMayor.Codigo);
                if (pos.TipoImputacionValor != null)
                    posEntity.TipoImputacionSap = repositorio.Obtener<TablaSap>(x => x.Tabla == pos.TipoImputacionValor.Tabla && x.Codigo == pos.TipoImputacionValor.Codigo);
                if (pos.TipoPosicion != null)
                    posEntity.TipoPosicion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoPosicionSolp && x.Codigo == pos.TipoPosicion.Codigo);
                if (pos.TipoImputacion != null)
                    posEntity.TipoImputacion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoImputacionSolp && x.Codigo == pos.TipoImputacion.Codigo);
                if (pos.Almacen != null)
                {
                    posEntity.Almacen = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Almacen && x.Codigo == pos.Almacen.Codigo);
                }
                else
                {
                    posEntity.Almacen_Id = null;
                }

                if (pos.Centro != null)
                    posEntity.Centro = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Centro && x.Codigo == pos.Centro.Codigo);
                if (pos.GrupoCompras != null)
                {
                    posEntity.GrupoCompras = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoCompras && x.Codigo == pos.GrupoCompras.Codigo);
                }
                else
                {
                    posEntity.GrupoCompras_Id = null;
                }

                if (pos.GrupoArticulo != null)
                {
                    posEntity.GrupoArticulo = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.GrupoArticulo && x.Codigo == pos.GrupoArticulo.Codigo);
                }
                else
                {
                    posEntity.GrupoArticulo_Id = null;
                }

                if (pos.Moneda != null)
                    posEntity.Moneda = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Moneda && x.Codigo == pos.Moneda.Codigo);

                if (pos.CodigoMaterialSap != null)
                {
                    posEntity.MaterialSolp = repositorio.Obtener<MaterialSolp>(x => x.CodigoSap == pos.CodigoMaterialSap.Codigo);
                }
                else
                {
                    posEntity.MaterialSolp = null;
                    posEntity.MaterialSolp_Id = null;
                }

                if (pos.CodigoServicioSap != null)
                    posEntity.ServicioSolp = repositorio.Obtener<ServicioSolp>(x => x.CodigoSap == pos.CodigoServicioSap.Codigo);


                if (pos.Provincia != null)
                {
                    posEntity.ProvinciaId = pos.Provincia.ProvinciaId;
                }
                else
                {
                    posEntity.ProvinciaId = null;
                }

                //Contrato Marco
                posEntity.NumeroContratoSuperior = pos.NumeroContratoSuperior;
                posEntity.NumeroPosicionContratoSuperior = pos.NumeroPosicionContratoSuperior;
                posEntity.NombreProveedor = pos.NombreProveedor;
                posEntity.ProveedorFijo = pos.ProveedorFijo;
                posEntity.OrganizacionCompras = pos.OrganizacionCompras;
                posEntity.NumeroPedido = pos.NumeroPedido;

                if (posEntity.Subposiciones == null)
                    posEntity.Subposiciones = new List<SolpSubposicion>();

                if (pos.Subposiciones != null)
                {
                    foreach (var subpos in pos.Subposiciones)
                    {
                        SolpSubposicion subposEntity = null;

                        subposEntity = posEntity.Subposiciones.FirstOrDefault(y => y.Numero == subpos.Numero);

                        if (subposEntity == null)
                            subposEntity = new SolpSubposicion();

                        subposEntity.Codigo = subpos.Codigo;
                        subposEntity.Cantidad = subpos.Cantidad;
                        subposEntity.Estado = true;

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
                        {
                            subposEntity.ServicioSolp = repositorio.Obtener<ServicioSolp>(x => x.CodigoSap == subpos.CodigoServicioSap.Codigo);
                        }
                        else
                        {
                            subposEntity.ServicioSolp = null;
                            subposEntity.ServicioSolp_Id = null;
                        }

                        posEntity.Subposiciones.Add(subposEntity);
                    }

                    foreach (var subpos2 in lstSubPosicionesUnicas.Where(x => pos.Subposiciones == null || !pos.Subposiciones.Any(y => y.Codigo == x.Codigo)))
                    {
                        SolpSubposicion subposEntity = null;

                        subposEntity = posEntity.Subposiciones.FirstOrDefault(y => y.Codigo == subpos2.Codigo || y.Numero == subpos2.Numero);

                        if (subposEntity == null)
                            subposEntity = new SolpSubposicion();

                        subposEntity.Codigo = subpos2.Codigo;
                        subposEntity.Cantidad = subpos2.Cantidad;
                        subposEntity.Estado = false;
                        subposEntity.Numero = subpos2.Numero;
                        subposEntity.PrecioBruto = subpos2.PrecioBruto;
                        subposEntity.Tarea = subpos2.Tarea;
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
            return solpEntity;
        }

        private string ObtenerRutaArchivos(int id, string path)
        {
            return $"{rutaArchivosCompras}/{path}_{id}";
        }

        private Pliego GuardarEspecificacionesTecnicasPliego(SolpDto solp, Solp solpEntity, Pliego pliegoEntity)
        {
            var rutaArchivo = string.Concat(ObtenerRutaArchivos(solpEntity.Id, "Solp"), "/", FileKeys.EspecificacionesTecnicasPliego, ".txt");

            Directory.CreateDirectory(ObtenerRutaArchivos(solpEntity.Id, "Solp"));
            File.WriteAllText(rutaArchivo, solp.EspecificacionesTecnicas);

            var archivosEspecificacionesTecnicasPliego = pliegoEntity.Archivos.FirstOrDefault(x => x.FileKey == FileKeys.EspecificacionesTecnicasPliego);

            if (archivosEspecificacionesTecnicasPliego == null)
            {
                pliegoEntity.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.EspecificacionesTecnicasPliego,
                    Ruta = rutaArchivo,
                });
            }

            repositorio.GuardarCambios();
            return pliegoEntity;
        }

        private SolpDto GuardarAdjuntosSolp(SolpDto solp, HttpFileCollectionBase files, Pliego pliego)
        {
            var ruta = ObtenerRutaArchivos(solp.Id.Value, "Solp");

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

            repositorio.GuardarCambios();

            solp.Adjuntos = pliego.Archivos.Where(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(x => new ArchivoDto()
            {
                Id = x.Id,
                FileKey = x.FileKey,
                Nombre = x.ObtenerNombre(x.Ruta)
            }).ToList();
            return solp;
        }

        private RespuestaGuardarSOLP FinalizarSolp(Solp solpEntity, SolpPosicion postEntitySubPosicionesEliminadas, RespuestaGuardarSOLP respuestaGuardarSOLP, bool enviarMailUrgencia)
        {
            //variables para ver a que request accedemos
            //var crearPedidoConsumer = crearPedido(solpEntity);
            //var crearSolpComsumer = crearSolp(solpEntity);
            //var modificarSolpConsumer = modificarSolp(solpEntity);
            //var respuestaGuardarSOLP = new RespuestaGuardarSOLP();
            if (string.IsNullOrEmpty(solpEntity.NroSolp))
            {
                var solpSAP = ConvertirSOLPSAP(solpEntity, postEntitySubPosicionesEliminadas);

                var resultadoCrearSolp = crearSolpConsumerMOA.Request(solpSAP);
                respuestaGuardarSOLP.Errores = new List<string>();

                foreach (var error in resultadoCrearSolp.Errores.Where(x => x.Tipo == "E"))
                {
                    var mensaje = error.Mensaje.Trim();
                    respuestaGuardarSOLP.Errores.Add(mensaje);
                }
                if (respuestaGuardarSOLP.Errores.Count == 0)
                {
                    respuestaGuardarSOLP.Mensaje = "OK";
                    solpEntity.NroSolp = resultadoCrearSolp.NumeroSolp;
                    respuestaGuardarSOLP.Solp.NroSolp = resultadoCrearSolp.NumeroSolp;
                    var estadoCreadoCodigo = EstadoDocumentoSolp.Creado.Code();
                    var estadoCreado = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoCreadoCodigo);
                    solpEntity.EstadoDocumento_Id = estadoCreado.Id;
                    SetNombreDePedido(solpEntity);
                    foreach (var posiciones in solpEntity.Posiciones)
                    {
                        posiciones.EsConcluido = true;
                    }

                    if (enviarMailUrgencia)
                    {
                        try
                        {
                            EnviarMailSolpFinalizadaConUrgencia(solpEntity);
                        }
                        catch (Exception e)
                        {
                            Log.Info($"EnviarMailSolpFinalizadaConUrgencia Nro de SOLP: {solpEntity.NroSolp} - Error: " + e);
                        }
                    }
                }
                repositorio.GuardarCambios();
            }
            else
            {
                var solpSAP = ConvertirSOLPSAP(solpEntity, postEntitySubPosicionesEliminadas);

                var resultadoEditarSolp = modificarSolpConsumerMOA.Request(solpSAP);
                respuestaGuardarSOLP.Errores = new List<string>();
                foreach (var error in resultadoEditarSolp.Errores.Where(x => x.Tipo == "E"))
                {
                    var mensaje = error.Mensaje.Trim();
                    respuestaGuardarSOLP.Errores.Add(mensaje);
                }

                if (respuestaGuardarSOLP.Errores.Count == 0)
                {
                    respuestaGuardarSOLP.Mensaje = "OK";
                    if (solpEntity.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || solpEntity.TipoSolpSap == (int?)TipoSolpSap.ReposicionAutomatica || solpEntity.TipoSolpSap == (int?)TipoSolpSap.Sap)
                    {
                        var estadoCreadoCodigo = EstadoDocumentoSolp.Creado.Code();
                        var estadoCreado = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoCreadoCodigo);
                        solpEntity.EstadoDocumento_Id = estadoCreado.Id;
                    }
                    foreach (var pos in solpEntity.Posiciones)
                    {
                        pos.CantidadSubposicionesEnSAP = pos.Subposiciones.Count;
                        pos.EsConcluido = true;
                    }
                    if (solpEntity.TrabajoYaHecho == true && (solpEntity.EstadoSolpSap?.CodigoSap == "05" || solpEntity.EstadoSolpSap?.CodigoSap == "02"))
                    {
                        ActualizarOfertasAlEditarSolpLiberada(solpEntity);
                    }

                    ActualizarPeticionDeOfertaAlEditarSolp(solpEntity);
                    if (enviarMailUrgencia)
                    {
                        try
                        {
                            EnviarMailSolpFinalizadaConUrgencia(solpEntity);
                        }
                        catch (Exception e)
                        {
                            Log.Info($"EnviarMailSolpFinalizadaConUrgencia Nro de SOLP: {solpEntity.NroSolp} - Error: " + e);
                        }
                    }
                }
                else
                {
                    //revertir los cambios si da error
                    ObtenerSolpesDesdeSAPJob(new ObtenerSolpRequest { NumeroSolp = solpEntity.NroSolp, FechaDesde = new DateTime(2010, 01, 01), FechaHasta = DateTime.Now.Date.AddDays(1) });
                    respuestaGuardarSOLP.Solp = TraerSolpId(solpEntity.Id);
                }
                repositorio.GuardarCambios();
            }
            respuestaGuardarSOLP.IdEntidad = solpEntity.Id;
            ValidarSolpAnulada(solpEntity.NroSolp);
            return respuestaGuardarSOLP;

            #region'NO BORRAR EL CODIGO COMENTADO EN ESTA REGION'
            //TODO: Esto de crear pedido queda comentado por que todavia falta las definiciones del requerimiento.

            //Esto estaria temporal ya que despues de haber desarrollado esta parte nos comentaron que el flujo en realidad no es tan directo, sino que 
            //necesitamos que la solp tengo numero de solp y que el estado sea liberado
            //if (crearPedidoConsumer) 
            //{
            //	//Aca obtenemos las posiciones que tienen en mismo proveedor
            //	var proveedorPosiciones = getPosicionesByProveedor(solpEntity);


            //	//al obtener las posiciones agrupadas por proveedor definimos que vamos a tener un numero de pedido para todas las posiciones con el mismo proveedor y numeros de pedido 
            //	//distintos si cambia el proveedor
            //	if (proveedorPosiciones.Any())
            //	{
            //		proveedorPosiciones.AsEnumerable().ToList().ForEach(proveedorConPosiciones =>
            //		{
            //			var resultadoCrearPedido = crearPedidoConsumerMOA.Request(solpEntity, postEntitySubPosicionesEliminadas, proveedorConPosiciones.Value);

            //			respuestaGuardarSOLP.Errores = new List<string>();

            //			foreach (var error in resultadoCrearPedido.Errores.Where(x => x.Tipo == "E"))
            //			{
            //				var mensaje = error.Mensaje.Trim();
            //				respuestaGuardarSOLP.Errores.Add(mensaje);
            //			}

            //			if (respuestaGuardarSOLP.Errores.Count == 0)
            //			{
            //				proveedorConPosiciones.Value.ForEach(posicion => posicion.NumeroPedido = resultadoCrearPedido.NumeroPedido);

            //				respuestaGuardarSOLP.Mensaje = "OK";
            //				repositorio.GuardarCambios();
            //			}
            //		});
            //	}		
            #endregion
        }

        private void ActualizarOfertasAlEditarSolpLiberada(Solp solpEntity)
        {
            var peticionDeOfertaId = repositorio.Obtener<PeticionDeOferta>(x => x.Solp_Id == solpEntity.Id)?.Id;
            if (peticionDeOfertaId != null)
            {
                var peticionUsuarioId = repositorio.Obtener<PeticionDeOfertaUsuario>(x => x.PeticionDeOferta_Id == peticionDeOfertaId)?.Id;
                var cotizacionId = repositorio.Obtener<Cotizacion>(x => x.PeticionDeOfertaUsuario_Id == peticionUsuarioId)?.Id;
                var cotizacionPosicion = repositorio.Listar<CotizacionPosicion>(x => x.Cotizacion_Id == cotizacionId);
                var peticionPosiciones = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => x.PeticionDeOferta_Id == peticionDeOfertaId);
                foreach (var posicion in solpEntity.Posiciones)
                {
                    var cotizacion = cotizacionPosicion.FirstOrDefault(x => x.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id);

                    if (cotizacion == null)
                    {
                        var peticionPosicionNueva = new PeticionDeOfertaSolpPosicion { PeticionDeOferta_Id = peticionDeOfertaId.Value, SolpPosicion_Id = posicion.Id };
                        repositorio.Agregar(peticionPosicionNueva);
                        repositorio.GuardarCambios();
                        var cotizacionPosicionNueva = new CotizacionPosicion
                        {
                            Cantidad = posicion.Cantidad,
                            Precio = posicion.PrecioBruto,
                            Moneda_Id = posicion.Moneda_Id,
                            PeticionDeOfertaSolpPosicion_Id = peticionPosicionNueva.Id,
                            Cotizacion_Id = cotizacionId.Value,
                            UnidadDeMedida_Id = posicion.Unidad_Id,
                            FechaDeEntrega = DateTime.Today.AddDays(-1)
                        };
                        repositorio.Agregar(cotizacionPosicionNueva);
                        if (solpEntity.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "SERVICIO")
                        {
                            repositorio.GuardarCambios();
                            var solpSubPosiciones = repositorio.Listar<SolpSubposicion>(x => x.SolpPosicion_Id == posicion.Id);
                            foreach (var subPos in solpSubPosiciones)
                            {
                                var cotizacionSubPosicionNueva = new CotizacionSubPosicion
                                {
                                    CotizacionPosicion_Id = cotizacionPosicionNueva.Id,
                                    SolpSubPosicion_Id = subPos.Id,
                                    Cantidad = subPos.Cantidad,
                                    Precio = subPos.PrecioBruto,
                                    UnidadDeMedida_Id = subPos.Unidad_Id,
                                    Moneda_Id = posicion.Moneda_Id
                                };
                                repositorio.Agregar(cotizacionSubPosicionNueva);
                            }
                        }
                    }
                    else
                    {
                        cotizacion.Cantidad = posicion.Cantidad;
                        cotizacion.Precio = posicion.PrecioBruto;
                        cotizacion.Moneda_Id = posicion.Moneda_Id;
                        cotizacion.UnidadDeMedida_Id = posicion.Unidad_Id;

                        if (solpEntity.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "SERVICIO")
                        {
                            var solpSubPosiciones = repositorio.Listar<SolpSubposicion>(x => x.SolpPosicion_Id == posicion.Id);
                            foreach (var subPos in solpSubPosiciones)
                            {
                                var cotizacionSubPosicion = repositorio.Obtener<CotizacionSubPosicion>(x => x.SolpSubPosicion_Id == subPos.Id);
                                cotizacionSubPosicion.Cantidad = subPos.Cantidad;
                                cotizacionSubPosicion.Precio = subPos.PrecioBruto;
                                cotizacionSubPosicion.Moneda_Id = posicion.Moneda_Id;
                                cotizacionSubPosicion.UnidadDeMedida_Id = subPos.Unidad_Id;
                            }
                        }
                    }
                    repositorio.GuardarCambios();
                }
            }
        }

        private void ActualizarPeticionDeOfertaAlEditarSolp(Solp solpEntity)
        {
            if (solpEntity.Posiciones.First().TipoPosicion.Codigo == "MATERIALES")
            {
                return;
            }

            var peticionesDeOferta = repositorio.Listar<PeticionDeOferta>(x => x.Solp_Id == solpEntity.Id);

            foreach (var po in peticionesDeOferta)
            {
                var posNueva = false;

                foreach (var solpPos in solpEntity.Posiciones)
                {
                    if (!po.Posiciones.Any(x => x.SolpPosicion_Id == solpPos.Id))
                    {
                        //insertar posicion de oferta solp posicion en po (PeticionDeOfertaSolpPosicion)
                        var peticionPosicionNueva = new PeticionDeOfertaSolpPosicion { PeticionDeOferta_Id = po.Id, SolpPosicion_Id = solpPos.Id };
                        posNueva = true;
                        po.Posiciones.Add(peticionPosicionNueva);
                    }

                }

                if (posNueva)
                {
                    foreach (var poUsusario in po.Usuarios)
                    {
                        if (poUsusario.Cotizaciones != null && poUsusario.Cotizaciones.Count > 0)
                        {
                            foreach (var poCotizacion in poUsusario.Cotizaciones)
                            {
                                poCotizacion.CotizacionEstado_Id = (int)CotizacionEstadoEnum.Incompleta;
                            }
                        }
                    }
                }

                repositorio.GuardarCambios();
            }
        }
        private void GuardarUsuarioComprasRelacionado(SolpDto solp)
        {
            var usuarioComprasRelacionado = repositorio.Obtener<UsuarioComprasRelacionConUsuarios>(x => x.Usuario_Id == solp.UsuarioActual.Id && x.UsuarioCompras_Id == solp.UsuarioCompras.Id);
            if (usuarioComprasRelacionado == null)
            {
                if (solp.UsuarioActual != null && solp.UsuarioCompras != null && solp.UsuarioCompras.Id != null)
                {
                    usuarioComprasRelacionado = new UsuarioComprasRelacionConUsuarios()
                    {
                        Usuario_Id = solp.UsuarioActual.Id,
                        UsuarioCompras_Id = (int)solp.UsuarioCompras.Id
                    };
                    repositorio.Agregar(usuarioComprasRelacionado);
                    repositorio.GuardarCambios();
                }
            }
        }

        private void ActualizarPosiciones(SolpDto solp, Solp solpEntity)
        {
            if (solp.Posiciones != null)
            {
                //posiciones nuevas y actualizadas
                foreach (var pos in solp.Posiciones)
                {
                    SolpPosicion posEntity = null;

                    posEntity = solpEntity.Posiciones.FirstOrDefault(y => y.Codigo == pos.Codigo);
                    //subposiciones eliminadas 
                    if (posEntity.Subposiciones.Count > 0)
                    {
                        var subposEliminadas = posEntity.Subposiciones.Where(x => pos.Subposiciones == null || !pos.Subposiciones.Any(y => y.Codigo == x.Codigo));

                        foreach (var subpos in subposEliminadas.ToList())
                        {
                            if (subpos.Cotizaciones.Any())
                            {
                                foreach (var cotizacion in subpos.Cotizaciones.ToList())
                                {
                                    repositorio.Remover(cotizacion);
                                }
                            }
                            repositorio.Remover(subpos);
                        }
                    }
                }
                repositorio.GuardarCambios();
            }
        }

        public string ObtenerRutaArchivo(int archivoId)
        {
            var archivo = repositorio.Obtener<Archivo>(x => x.Id == archivoId);

            return archivo?.Ruta;
        }

        public List<TablaSapDto> ObtenerTablaSap(string tabla)
        {
            var tablaSap = repositorio.Listar<TablaSap>(x => x.Tabla == tabla).Select(x => new TablaSapDto(x)).ToList();
            if (tabla == TablasSap.EstadoSolpSap)
            {
                tablaSap.Add(new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" });
            }
            return tablaSap;
        }

        public List<TablaSapDto> ListarTablaSap(List<string> tablas)
        {
            List<TablaSapDto> tablaSap = new List<TablaSapDto>();
            if (tablas.Contains("OrdenSolpSap") || tablas.Contains("CecoSolpSap") || tablas.Contains("CentroBeneficio"))
            {
                var tipoImputacionEnPosYSubpos = repositorio.Listar<SolpPosicion>().Select(x => x.ValorTipoImputacion_Id).Where(id => id != null).Distinct().ToList();
                tipoImputacionEnPosYSubpos.AddRange(repositorio.Listar<SolpSubposicion>().Select(x => x.TipoImputacion_Id).Where(id => id != null).Distinct().ToList());
                tablaSap = repositorio.Listar<TablaSap>(x => tablas.Contains(x.Tabla) && tipoImputacionEnPosYSubpos.Contains(x.Id)).Select(x => new TablaSapDto(x)).ToList();
            }
            else
                tablaSap = repositorio.Listar<TablaSap>(x => tablas.Contains(x.Tabla)).Select(x => new TablaSapDto(x)).ToList();

            return tablaSap;
        }

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<TablaGeneralDto> ObtenerImputaciones(string tabla)
        {
            var imputaciones = repositorio.Listar<TablaGeneral, TablaGeneralDto>(x => new TablaGeneralDto
            {
                Codigo = x.Codigo,
                Descripcion = x.Descripcion,
                Tabla = x.Tabla,
                Id = x.Id,
                CodigoVisualizacion = x.Codigo == "ordenDeOt" ? "nroDeOt" : x.Codigo
            }, x => x.Tabla == tabla).ToList();

            return imputaciones;

        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x => new CentroDireccionDto(x)).ToList();
        }

        public ListaPaginada<SolpDto> ListarSolp(UsuarioDto usuarioActual, Paginacion paginacion, string nroSolp, DateTime? desde, DateTime? hasta, bool? sap, bool? mantenimiento, bool? web, bool? repoAutomatica, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null)
        {
            try
            {
                var fechaHasta = hasta != null ? hasta.Value.AddDays(1) : (DateTime?)null;
                var hoy = DateTime.Now.Date;
                var usuariosCompras = repositorio.Listar<UsuarioCompras>();
                var usuariosComprasRelacion = repositorio.Listar<UsuarioComprasRelacionConUsuarios>(x => x.Usuario_Id == usuarioActual.Id);
                Usuario usuario = repositorio.Obtener<Usuario>(u => u.Id == usuarioActual.Id);
                var rol = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "SOLP" : "COMPRADOR";
                nroSolp = nroSolp.Trim();
                //var peticionCierre = repositorio.Listar<PeticionDeOfertaCierre, PeticionDeOfertaCierreDto>(pc => new PeticionDeOfertaCierreDto());
                var peticionesDeOferta = repositorio.Listar<PeticionDeOferta, PeticionDeOfertaDto>(po => new PeticionDeOfertaDto
                {
                    Id = po.Id,
                    RegistroInfo = po.RegistroInfo,
                    Solp_Id = po.Solp_Id,
                    FechaCreacion = po.FechaCreacion,
                    UsuarioCreador_Id = po.UsuarioCreador_Id,
                    Observaciones = po.Observaciones,
                    PlazoDeOfertaOriginal = po.PlazoDeOferta,
                    PlazoDeOfertaCircular = po.Usuarios.GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta,
                    FechaCircular = po.Usuarios.GroupBy(x => x).SelectMany(x => x.Key.Circulares)
                                    .Where(x => x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                                    .OrderByDescending(x => x.Circular.Id).FirstOrDefault().Circular.FechaCreacion,
                    PlazoDeOfertaCierre = po.Cierres.Any() ? po.Cierres.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                    RevisionFinalizada = po.RevisionTecnica != null && po.RevisionTecnica.Finalizada,
                });

                var ordenCompra = repositorio.Listar<Adjudicacion, AdjudicacionDto>(adjudicacion => new AdjudicacionDto
                {
                    Id = adjudicacion.Id,
                    Solp_Id = adjudicacion.Solp_Id,
                    NumeroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra,
                    FechaCreacion = adjudicacion.FechaCreacion,
                    Proveedor = adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Proveedores.FirstOrDefault().RazonSocial,
                    MonedaDescripcion = adjudicacion.Moneda.CodigoSap,
                    PrecioFinal = adjudicacion.MontoTotal,
                });

                UsuarioComprasRelacionConUsuarios usuarioComprasRelacionEntity = null;
                if (!usuariosComprasRelacion.ToList().Any())
                {
                    usuariosCompras.ToList().ForEach(x =>
                    {
                        usuarioComprasRelacionEntity = new UsuarioComprasRelacionConUsuarios
                        {
                            Usuario_Id = usuarioActual.Id,
                            UsuarioCompras_Id = x.Id
                        };
                        repositorio.Agregar(usuarioComprasRelacionEntity);
                    });
                    repositorio.GuardarCambios();
                }

                Expression<Func<Solp, bool>> filtro = x => x.FechaBorrado == null; //&& x.UsuarioCreacion_Id == usuarioActual.Id;

                if (usuarioActual.Permisos.Contains("VER TODAS SOLPS"))
                {
                    filtro = (x => x.FechaBorrado == null);
                }

                var todasLasSolp = repositorio.Listar<Solp, SolpDto>(x => new SolpDto
                {
                    UsuarioActual = new UsuarioDto { Mail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "" },
                    Id = x.Id,
                    NroSolp = x.NroSolp,
                    VerCircular = x.TrabajoYaHecho == null || x.TrabajoYaHecho == false,
                    NombreDeObra = x.Pliego == null ? "" : x.Pliego.NombreObra,
                    FechaCreacion = x.FechaCreacion,
                    EstadoDocumento = new TablaEstadoDto { Descripcion = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Descripcion, Color = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Color, Codigo = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Codigo },
                    EstadoSolpSap_Id = x.NroSolp != null && x.Posiciones.All(p => p.Estado == false) ? -1 : (x.EstadoSolpSap != null ? x.EstadoSolpSap_Id : 0),
                    EstadoSolpSap = new TablaSapDto { Descripcion = x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : "", Id = x.EstadoSolpSap != null ? x.EstadoSolpSap.Id : 0 },
                    EstadoSolpDescripcion = x.NroSolp != null && x.Posiciones.All(p => p.Estado == false) ? "Borrado en SAP" : (x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : ""),
                    TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "", Codigo = x.TipoSolp != null ? x.TipoSolp.Codigo : "" },
                    VincularPliego = !x.Pliego_Id.HasValue,
                    TieneCondicionesGenerales = x.Pliego == null ? null : x.Pliego.TieneCondicionesGenerales,
                    RevisadoPor = x.Pliego == null ? "" : x.Pliego.RevisadoPor,
                    TipoSolpSap = x.TipoSolpSap,
                    EstadoPasos = x.EstadoPasos,
                    PosicionesEstado = x.Posiciones.All(p => p.Estado == false),
                    ItemPorPagina = paginacion.ItemsPorPagina,
                    Pagina = paginacion.Pagina,
                    TipoPosicionCodigo = x.Posiciones.Select(posiciones => posiciones.TipoPosicion.Codigo).FirstOrDefault(),
                    SolpConAdjuntos = x.Pliego.Archivos.Where(r => r.FileKey == FileKeys.AdjuntoCotizacionesSolp).Any(),
                    ChatSinLeer = x.ChatInternoCompras.Any(a => a.Leido == false && a.Usuario.Roles.Any(r => r.Codigo == rol)),
                },
                paginacion,
                x => x.FechaBorrado == null && (string.IsNullOrEmpty(nroSolp) || x.NroSolp.ToUpper().StartsWith(nroSolp.ToUpper())) &&
                (!estados.Any() || (x.EstadoSolpSap_Id != null && estados.Contains((int)x.EstadoSolpSap_Id)) || (estados.Any(y => y == -1) && x.NroSolp != null && x.Posiciones.All(p => p.Estado == false))) &&
                (!usuarios.Any() || (x.UsuarioCreacion_Id != null && usuarios.Contains((int)x.UsuarioCreacion_Id))) &&
                (sap == true && x.TipoSolpSap == 3 || mantenimiento == true && x.TipoSolpSap == 2 || repoAutomatica == true && x.TipoSolpSap == 4 ||
                (web == true && (x.TipoSolpSap == null || x.TipoSolpSap == 1)) || (sap == false && mantenimiento == false && web == false && repoAutomatica == false)) &&
                (desde == null || x.FechaCreacion >= desde.Value) && (fechaHasta == null || x.FechaCreacion <= fechaHasta.Value) &&
                (!centros.Any() || x.Posiciones.Any(c => centros.Contains(c.Centro_Id))) && (!grupoDeCompras.Any() || x.Posiciones.Any(gc => grupoDeCompras.Contains((int)gc.GrupoCompras_Id))) &&
                (!claseDocumento.Any() || x.EstadoSolpSap_Id != null && claseDocumento.Contains((int)x.ClaseDocumento_Id)) && (!tipoImputacion.Any() || x.Posiciones.Any(c => tipoImputacion.Contains(c.TipoImputacion.Codigo))) &&
                (!valorTipoImputacion.Any() || x.Posiciones.Any(p => valorTipoImputacion.Contains((int)p.ValorTipoImputacion_Id)) || x.Posiciones.Any(p => p.Subposiciones.Any(sp => valorTipoImputacion.Contains((int)sp.TipoImputacion_Id)))));

                if (todasLasSolp.Items != null && todasLasSolp.Items.Count() > 0)
                {
                    todasLasSolp.Items.FirstOrDefault().ItemsTotales = todasLasSolp.ItemsTotales;
                    foreach (var item in todasLasSolp)
                    {
                        item.PeticionesDeOferta = peticionesDeOferta.Where(peticionDeOferta => peticionDeOferta.RegistroInfo != true && peticionDeOferta.Solp_Id == item.Id).ToList();
                        item.TienePeticionDeOferta = peticionesDeOferta.Any(peticionDeOferta => peticionDeOferta.RegistroInfo != true && peticionDeOferta.Solp_Id == item.Id);
                        item.OrdenesDeCompraSolicitante = ordenCompra.Where(oc => oc.Solp_Id == item.Id).OrderBy(x => x.FechaCreacion).ToList();
                    }
                }

                return todasLasSolp;
            }
            catch (Exception e)
            {
                throw;
            }
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

            var solp = repositorio.Obtener<Solp>(includes, s => s.Id == idSolp);
            var po = repositorio.Obtener<PeticionDeOferta>(x => x.Solp_Id == idSolp);

            if (solp == null)
            {
                throw new InfoCustomException("No se encontró la SOLP.");
            }

            //if (!String.IsNullOrEmpty(x.NroSolp))
            //{
            //    ObtenerSolpRequest obtenerSolpRequest = new ObtenerSolpRequest
            //    {
            //        FechaDesde = Convert.ToDateTime(new DateTime(2010, 01, 01)),
            //        FechaHasta = Convert.ToDateTime(DateTime.Now.Date.AddDays(1)),
            //        CreadoPorUsuarios = new List<string>(),
            //        NumeroSolp = x.NroSolp
            //    };

            //    //ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);

            //    x = repositorio.Obtener<Solp>(s => s.Id == idSolp);
            //}

            var solpDevuelta = new SolpDto()
            {
                UsuarioActual = solp.UsuarioCreacion != null ? new UsuarioDto(solp.UsuarioCreacion) : new UsuarioDto(),
                Id = solp.Id,
                NroSolp = solp.NroSolp,
                FechaCreacion = solp.FechaCreacion,
                EstadoDocumento = new TablaEstadoDto(solp.EstadoDocumento),
                EstadoSolpSap = solp.EstadoSolpSap != null ? new TablaSapDto(solp.EstadoSolpSap) : new TablaSapDto(),
                TipoSolp = solp.TipoSolp != null ? new TablaGeneralDto(solp.TipoSolp) : new TablaGeneralDto(),
                VincularPliego = !solp.Pliego_Id.HasValue,
                UsuarioCompras = solp.UsuarioCompras != null ? new UsuarioComprasDto(solp.UsuarioCompras) : new UsuarioComprasDto(),
                TipoSolpSap = solp.TipoSolpSap,
                NombreDeObra = solp.Pliego.NombreObra,
                FiscalContrato = solp.Pliego.FiscalContrato,
                Telefono = solp.Pliego.Telefono,
                Email = solp.Pliego.Email,
                FechaHoraEntrega = solp.Pliego.FechaHoraEntrega,
                SupervisorSector = solp.Pliego.SupervisorSector.Split(',').ToList(),
                SupervisorTrabajo = solp.Pliego.SupervisorTrabajo.Split(',').ToList(),
                VisitasObraMasiva = solp.Pliego.VisitasMasivas.Select(a => new VisitaObraDto(a)).ToList(),
                TieneVisitaObra = solp.Pliego.TieneVisitaObra ?? false,
                TieneVisitaObraMasiva = solp.Pliego.TieneVisitaObraMasiva ?? false,
                TieneObradores = solp.Pliego.TieneObradores ?? false,
                TieneMedioElevacion = solp.Pliego.TieneMedioElevacion ?? false,
                TieneAndamio = solp.Pliego.TieneAndamio ?? false,
                TieneTecnicoSeguridad = solp.Pliego.TieneTecnicoSeguridad ?? false,
                TieneGrillaPersonal = solp.Pliego.TieneGrillaPersonal ?? false,
                TieneFabricacionTallerExterno = solp.Pliego.TieneFabricacionTallerExterno ?? false,
                TieneDescripcionTecnica = solp.Pliego.TieneDescripcionTecnica ?? false,
                TieneDocumentacionTecnica = solp.Pliego.TieneDocumentacionTecnica ?? false,
                FechaHoraLimiteConsulta = solp.Pliego.FechaHoraLimiteConsulta,
                ObservacionesGeneracion = solp.Pliego.ObservacionesGeneracion,
                //EspecificacionesTecnicas = x.EspecificacionesTecnicas,
                DiasEjecucion = solp.Pliego.DiasEjecucion,
                ObservacionesCotizacion = solp.Pliego.ObservacionesCotizacion,
                JornadaLaboral = string.IsNullOrEmpty(solp.Pliego.JornadaLaboralDias) ? new List<DayOfWeek>() :
                                solp.Pliego.JornadaLaboralDias.Split(",".ToCharArray()).Select(a => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a)).ToList(),
                JornadaLaboralDesde = solp.Pliego.JornadaLaboralHorasDesde,
                JornadaLaboralHasta = solp.Pliego.JornadaLaboralHorasHasta,
                ClaseDocumento = solp.ClaseDocumento != null ? new TablaSapDto(solp.ClaseDocumento) : new TablaSapDto(),
                ProveedorAsignado_Id = solp.ProveedorAsignado_Id,
                TrabajoYaHecho = solp.TrabajoYaHecho,
                CondEspProveedorAsignado = solp.CondEspProveedorAsignado,
                Adicional = solp.Adicional,
                Urgencia = solp.Urgencia,
                NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                DeshabilitarAdicional = solp.Adjudicaciones.Any(),
                EditarCondicionesEspeciales = (solp.EstadoSolpSap_Id == null || solp.EstadoSolpSap.CodigoSap != "05" || solp.EstadoSolpSap.CodigoSap != "02") && po == null,

                Adjuntos = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),

                EspecificacionesTecnicas = solp.Pliego.Archivos.FirstOrDefault(a => a.FileKey == FileKeys.EspecificacionesTecnicasPliego)?.Ruta,
                TieneCondicionesGenerales = solp.Pliego.TieneCondicionesGenerales ?? true,
                RevisadoPor = solp.Pliego.RevisadoPor,
                EstadoSolpSap_Id = solp.EstadoSolpSap_Id,
                EstadoDocumento_Id = solp.EstadoDocumento_Id,

                //Posiciones = (x.TipoSolpSap == (int)TipoSolpSap.Sap || x.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || x.TipoSolpSap == (int)TipoSolpSap.ReposicionAutomatica) ? 
                //                x.Posiciones.Select(p => new SolpPosicionDto(p)).ToList() : 
                //                x.Posiciones.Where(p => !p.FechaBaja.HasValue).Select(p => new SolpPosicionDto(p)).ToList(),

                Posiciones = solp.Posiciones.Select(p => new SolpPosicionDto(p)).ToList(),
                PasoCompletado = solp.PasoCompletado,
                EstadoPasos = solp.EstadoPasos,
                EmailLinkToken = solp.EmailLinkToken,
                LiberadoresSapSolp = solp.LiberadoresSapSolp.Select(l => new LiberadorSapSolpDto(l)).ToList(),
                THAjustePolinomica = solp.THAjustePolinomica,
                THProveedorDirecto = solp.THProveedorDirecto,
                THServicioPermanente = solp.THServicioPermanente
            };
            if (solpDevuelta.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || solpDevuelta.TipoSolpSap == (int)TipoSolpSap.ReposicionAutomatica || solpDevuelta.TipoSolpSap == (int)TipoSolpSap.Sap)
            {
                if (solpDevuelta.JornadaLaboral == null || solpDevuelta.JornadaLaboral.Count() == 0)
                    solpDevuelta.JornadaLaboral = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

                if (solpDevuelta.JornadaLaboralDesde == null)
                    solpDevuelta.JornadaLaboralDesde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0).ToLocalTime();

                if (solpDevuelta.JornadaLaboralHasta == null)
                    solpDevuelta.JornadaLaboralHasta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0).ToLocalTime();
            }
            if (solpDevuelta.Adicional == true)
            {
                var ordenDeCompraSAPDto = ObtenerOrdenDeCompra(solpDevuelta.NroOrdenDeCompraAdicional);

                if (ordenDeCompraSAPDto.Error == null)
                {
                    solpDevuelta.ProveedorIdAdicional = ordenDeCompraSAPDto.Cabecera.Usuario_Id;
                    solpDevuelta.ProveedorRazonSocialAdicional = ordenDeCompraSAPDto.Cabecera.RazonSocialProveedor;
                    solpDevuelta.MonedaOC = ordenDeCompraSAPDto.Cabecera.Moneda;
                    solpDevuelta.MontoTotalOC = ordenDeCompraSAPDto.Cabecera.MontoTotal;
                    solpDevuelta.FechaCreacionOC = ordenDeCompraSAPDto.Cabecera.FechaCreacionString;
                }
            }

            if (solpDevuelta.ProveedorAsignado_Id != null)
            {
                var usuario = repositorio.Obtener<Usuario>(solpDevuelta.ProveedorAsignado_Id);
                solpDevuelta.ProveedorAsignado = usuario.ObtenerRazonSocial();
            }

            return solpDevuelta;
        }

        public string BorrarSolp(int idSolp)
        {
            var solpABorrar = repositorio.Obtener<Solp>(x => x.Id == idSolp);

            if (solpABorrar == null)
            {
                throw new InfoCustomException("No se encontró la SOLP.");
            }

            solpABorrar.FechaBorrado = DateTime.Now;
            repositorio.GuardarCambios();

            return "Se borró correctamente.";
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
            var templateFilePath = httpContextService.GetDirectory("Templates/NewPliegoSolpSinCondicionesTemplate.html");
            var templateString = System.IO.File.ReadAllText(templateFilePath);
            //, "Templates/PliegoSolpSinCondicionesTemplate.html"

            var templateCssFilePath = httpContextService.GetDirectory("Templates/PliegoSolpTemplate.css");
            var templateCssString = System.IO.File.ReadAllText(templateCssFilePath);
            var solpValores = new Dictionary<string, string>();

            //aca va la asignacion de valores de la solp que se van a reemplazar en el documento
            solpValores.Add(SolpTemplateKeys.FECHA_LIBERACION, ""); //crear campo fecha de liberacion en tabla
            solpValores.Add(SolpTemplateKeys.NOMBRE_OBRA, solp.NombreDeObra);
            solpValores.Add(SolpTemplateKeys.NRO_SOLP, solp.NroSolp);
            solpValores.Add(SolpTemplateKeys.NRO_PEDIDO, solp.NroPedido);
            solpValores.Add(SolpTemplateKeys.FISCAL_CONTRATO, solp.FiscalContrato);
            solpValores.Add(SolpTemplateKeys.TELEFONO, solp.Telefono);

            solpValores.Add(SolpTemplateKeys.FECHA_PRESENTACION, Convert.ToDateTime(solp.FechaHoraEntrega).ToString("dd-MM-yyyy"));
            solpValores.Add(SolpTemplateKeys.FECHA_CREACION, solp.FechaCreacion.ToString("dd-MM-yyyy"));

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
                    diasJornada = string.Join(",", diasOrdenado.Select(a => GetDia(a)).ToList());
                }
                else
                {
                    diasJornada = string.Format("{0} a {1}", GetDia(diasOrdenado.First()), GetDia(diasOrdenado.Last()));
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

                subposiciones.AppendLine($"<tr><th class='posicion' colspan='6'> {pos.Indice} - {pos.Tarea}</th></tr>{texto}<tr><td colspan='6'>&nbsp;</td></tr>");
            });

            solpValores.Add(SolpTemplateKeys.TABLA_POSICIONES_SUBPOSICIONES, subposiciones.ToString());
            solpValores.Add(SolpTemplateKeys.PLAZO_ENTREGA, plazo.ToString());

            //ADJUNTOS
            solpValores.Add(SolpTemplateKeys.LISTADO_ADJUNTOS, "");
            templateString = CombineTemplateValues(templateString, solpValores);
            return ConvertHtmlToPdf(templateString, templateCssString, solp); //agregar solp
        }

        private string GetDia(DayOfWeek dia)
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

                    Image image = Image.GetInstance(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAPcAAABqCAYAAABgdMfOAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAC2uSURBVHhe7V0FnFVF+16sz/isT8VGBVspAWksFERQQVBEwEDpXpbu7m5YekkJ6e5curs7lmaBJd7/+7xn5t5z7z03dsEF9j8Pv/lx9545c+acO89b886cMDIwMEiSMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2uKtw/HwMXb0Wp/4yuBUYchvcFbgSd5U6zhtGz9T6hObsiFbfGtwKDLkN7ij2xRymDkzq99r+TGGV0lFYxTQ0ft1cddTgVmDIbeCJmzfp5o3rUujGDfXlbQC3dfFqLO1nMi/avZY6LRhBX/cPp8fr5WZSp6ew6lnovtqfUFjVjDR23Rx1ksGtwJD7/wnOX75Ie08dovWHd9CULUuoz5Kx1GBaXyozpjUVHlSHcverRlm7laF0HX+l1O2LU+p2xSlth5KUpVtp+qxPFcofGUGFuF6xoQ0pf79wKjiwFhWPakwlueD/okMbUJEh9ajYsEZUcnhTKs7//8D18zKBc/QoT2nal6BXWhSix0Dm8KwUVpkJzUQOq5mT7q/9qauEVf6IolZNV702uBUYcidxnDofQ+PXzqIm0/rQt0zQsIgcFqmYRGIG4/8qGazvqn0sGjQsXBV8xndVM1nHoWHLvkffMbF7Lh1PQ5mEo9bMpGGrp8vfdaf0ojx9q7Fpnda53RrZhMzJ2K+2E1oXrbmNWX57YMidhLD5yE7qsWg0nbxwWn1DdJ3N4Ws3rqm/iLVrfSGeE7kClWS1cvF5GajD/BGqJWe0mD1ISO3URrCiyT1xw3zVmsGtwJD7Hse+mCPUaV4U5WST+v6InBRW7gNaume9OuqLfzYuEPLd50AufwV1oY1rTe6hWnHGifOn6b/1v2QNnd2xnWDlPtboYdUz09ydq1SLBrcCQ+57FEv2rBN/1wpIsXkN05dNXpi/i3atUbV8sfvEAXqozmdcN5cjwXwL1+X2Cw6srVrwj7Hr54pVEB/BYS9hTO5k7DasObRdtWhwKzDkvsewkIlboF+45cMiKMVkcJGDyQ3i7j55UNX2RezVy5SiRSHxf+3Eci5MbDaT32vzswTkgqHS+I4iCJzbCl7Q/0fr5qZ9pw6rFg1uBYbc9wj2nDpExYc1VKTOwFrOV/NCe6fv9DvduBl4CgsRcNT1Pt+7QAA8ytp0w+Gd6szAyNz1r5Da9VdwvVebF6RLV2NViwa3AkPuewBdF46ip7X57UBqXeAXt507TJ3lHzCxw6pkdGxDF/F/2TIYFD1FnRUYR8+dpMfr5/GwJOJbIBggIAxuDwy572IcZsIU6FfdIrWY0Z/5EEIXHP9fo3wekXJ/KPt3WyGuUztWYXOcBcVfo1urM4JjxrblbMJnkoi3c5vBC/x1zJkb3B4Yct+lWLR7HaVo+i0TO31IASqQEdNQoaDh9H7SrlM72s9GIsvluCvqjOBoNmsA9yHh/jYK+tRuXpRq0eBWYch9F2LMutn0HySbwH+t419b64KMr1eafR9S0AvoumCkX80dFpGdHq6Zi9aH6GdrfC+mfsLmt1GQ2BJWLTPN37VatWhwqzDkvssweOVU1pwfC2EDmeG6yBw0m+29lo5XLQTHsDUzxAT2bYsJhraWjFM1Q8PVa1cpVeufVJ892wy1YG78uSb56WzsedWqwa3CkPsuwsi1s1h7ZZKBHgqxUaDdP2AT+tp1dxYacDNAxHzK5sW+/jHmvpnYhQfXU7U8EXfd/xrr7cf30QPQvCHPnfsWaP0CA2qqFg1uBwy57xLM37WGHmRSBwuc2YtoWtbAEzZ6pmvO3r6CVh7Yov7yBZJcoGXdOd5M7OqZ6VU27WMunlW13BgWPYWOnTul/vLF30heiWfWm71o66PvsgmqRYPbAUPuuwCHzhyn5xvlE4KFSmwUEOqzXpVUK27k6FGOJrF29oeNR3dbueJK0yZDZhtbALN3rFQ13Ji6dRnl7x9ON/mfP9SZ0kvI6d2/UEtYRE5ZLXb47AnVosHtgCH3HcbNmzfp896VmBzpQwqe6QJyJmNhsHzfRtWSBeSVh5X/kCZuXKC+8cWB00fpv/W+lIwwtCV541N6qqNunLt8gZLX/4qazIxU3zjjm/41JMLu3cdQC6yPHwbXVa0Z3C4Yct9hdJg/nMmVJp4mLfzj9FTMYU74m0gm2l9vUdSameobX8RcPEPJmxSwln+y9s/UpZSPzw78hIy4Mu/SMi8BYgeCaSlb/ZjgYJpeCTZz2wrVosHtgiH3HQRSSh9DICsifquosLjiEf7fO4d82d4NMp0EE7nnUv8R7zgmcqo2RYVUSC/dfHSPOuJGzyVjZYXZ802/pQsBpth2nthPD0LYKCsgvgWBvYwsXLADjMHthSH3HUTRqEbKVw3dHBetzWZ07cm+ZnSeftXYxM0gWr3VrIHqW2dk7PQ7hZV+m7ouGq2+cSN6/2Z6CME97ttXfauqb50xcdNCERIJCabpgOD49fNUawa3E4bcdwirmEDJqmcR39lp4PsrMH9fYJP6zCXP+WAkf0ALSjIIk7t2kLXXGVhbZnHI40a7qVoWlgAbEl1qO/jidjTH5gwJCqaxkGKXIGeP8qolg9sNQ+47hMLYEQVBNMeB71ysKaO0YjJ7I2fPCkwWK6iFdiuM76iOOKPS2HaOWWiSacZklWsx+Uavna2OOKPoUPbLuZ53X4MVROiTVc9KK1nIGfw7MOS+A9gBPxXR7ngmfUCbpulQ0mPbJGAyklKYYNo0xucSw5uqo84475AJ1mL2YDH50Y5MlbFvvzbQxgnsJ6fr+JuawvPtr//CWrtCGqoxqbtqyODfgCH3HUC9qb2FRM4D37nIEkwm7dQtS1UrFm7cuCFruJHZpuvCBy44oJaqERpkVRcLD2hUaYOJba0yi1E1fIEVaKgTr2WeCCDyfXzEQuHKtauqJYN/A4bciYy4a3H0JiLV7G87Dn7HwoSo/BHli6yhWnFjQPQklxmt68P3/rJPZVUjOPafPkrPNfxa9ckK7uFz6vYlZB7eHxB4Q0Zd6HEDvo/wLPRE3S9o67G9qhWDfwuG3ImMhbvXWhrSlfoZvECbPsjaceORXaoVCxeuXKIUzQsJYez10X7WHuVUrcBAznhW7KCChSS2JBpo/3z9w1UtZ2BLYznPdu1ABTnzSLyZttXT+jD4d2DIncioOzW+qZqs7diEdwqQIXPMMu89p9Iw152u0x8hzR2XGdPG0UUAacuNbadqOQMvNQjtXvgeMLVWJSNFhrizi8Gtw5A7kYG3ekCzOpPAt8DsfYZNZu8dVg6fPU5PIIXUIQEmrHpWeq9tMbrukHVmR59lEyyT3sGKQMQ92JZNEimvHCRSDh87PKu4Cn2XT1RnGiQGDLkTEVhZFZ99xmQ6irVq54WjVAtulMZWSbLziW8CTFh4NkrZsghdvnpZ1fbF4j3r6UEWHE7CwT0NNkvVdkbQjRZBbCb1IzVz0eh1gafUDG4/DLkTEbO3RwsZPNZRByioi7XaSBe1YxP73vDBdWTb5zw2ga2dWS6oMzxx+MxxeqlJfjHfnYSDJMKwtoUA8IfYK7EBt0hOhuwztgrebPEDLQ2Qm27w78GQOxGBN1uG6m9bU18ZaZrX1BeQf0BNFcjyJSaK7GrSOD/FXDqnznADC0RydC/rE0DzOB8BPG4b8/H+sD/mCD2CHVl9BIwyw9msLzq4XkgbNhr8OzDkTkSU/btNyNFlmMXfD6qjznRj+tZlQvpA0XaY/U82ykfHL5xRZ7lRBn0IMscOUx3z1ycCEBOZZRL9tvUDREfbL7FgGRQ9SdU0uFMw5E5E5MU2xVWDr3tG5tpDXLYc81ytdfVaHL3f7pegATmQ7FHWqnixvR1dFo32G0CzF5jar7HJfTHAarAZ21e4XAwhNbf7CLdbeVwHOn7ef+KLQeLBkDuRgD3N8O7rUFI1Qb77uB60tB3NZg1UWtfZnNYFZHu4zufyXjANpKhijjmUYB7M6nfa/uy4xltD3gtWPrX0B8tW/xrZnLZ6CSODOwtD7kRCbNxlStHyB78BKO+CKHPO7u5ElDUHttJDCKJFBF83je2THqiViw6yXwxMYyHxGL5n0gYTDCiol7JVETnXHxbsXktvNv2W6kzqIWu6De4+GHInEk5ePEPPNP7GcerJqch0VPUs1Gr2YBqwYiKlwm4nfqLb3gWm8n1M7kIDa1ORIfXksxDbTwDNuyCd9CH+P3L5RNpx8gDtZfN+z8mDsjkEXiQIXGdL5HKcyQ2/m2HInUg4wFr0cdu+ZaEUawuiTBKEE40fIjl1kXOrZBBNHkgoyNQX/GZkkWEnF5zD5nZYuQ8lgSZP70rUZuYAWrJrDV28Yl7Sd6/AkDuRAM33WN0vHMktJGaTW6aQNCFB6ErpJVAl/+N1vfjOb0FdXfQ56hja0wV/6zo4B9+xhYBX/77EZjb2U0PmWQv27ydvWkR7Th1k3/u6uguDewmG3ImEvScPeZBbzG5kiIFsTOgn6n9F77YtJjuh/jikPpUf04bqTO1Nzdks7zAvinosGk39lo6nvkvG+RR833PRGKnXdt4waszExG6mFce2o99HNGOyNqCfhtanX4c3obKjW1ONSd2o2exBcs6YdXNowc7V4jef9drdxeDehiF3IsGuueWtIqwxUzQvSGXHtBYNeejMMbp+w2hIg9sHQ+5EwsGYo7JrKXzZ7F3/ouGrp4f84j4Dg4TAkDuRgA3+s3Uvy+b1GPWNgcG/C0NuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4Mkilsi9/yF26nvwIU0KGqplAFDl9CIMdF05Urgd1T5Q2xsHA3mdgZyO2gPn3tHLqCpM8wbKwwM4otbIneLdlPpvqfL0LMpq0t5+rWqlDxVOO3YdUzViB/qNBpLDz9fwdXeM29Uo8derEiTp/l/rY2BgYEzboncXXrOFhK+9VF9KW+mr0/PvxVO02dtUjVCxyQm8HOpqlOqdPVc7b3wdgT9+GsfVSM0XLx4hY4dP0eHDp+hs+di6eaN4K+xTSwEepH97UR8LnPlShydOnWBDh48TTGnL1Lc1YRZXXZcZgvs4oXLdOnSFfXNv4OLVy7JK5POxJ4P+NLD+CD2aiyd5vZQblebAN64ir6GtkHHTamH+pf4HhOKWyJ3xRrDKfmb4S4yokDbdu01R9UIDQcOxtAHmRvRqx/U8mlr4LAlqpZ/bN56mNp1mUFFSvamrLlbUZpsTeiDLI0p46ctKE/BzlS9zihauHiHqh0YQ0cso+J/9qcyVYZKKcul2B/9qO+ABSGRc9PmQ/Rb2YFUpvIQ1/mlKgyiSuFRQiI7zpy5RFUiRtCfFQdLXVx31tzN6mhw9Ow7l0r+FenqK65ZgttYv979MgInHD12liKHLJZ+5srbjtLlaCbPP33OZvRpvvb0V6UhNGb8Krp2LfRtn/AbtOowlQqX6E0587SlrF+0ouxftqZvf+pOzdtOpuMscG8VK/dvolazB1GhgbXkBQ/YpuqZJvkpedNvKWXLwpSxSykqMqgONZsZSbO2r6DYuODC5fi5UzQ0eorsNfdx1z/p9RY/0LPc5nNNCtAb3KZsGDmkPnWYN0z2jvdGzMWzVH9qb6ozuQfVndKLak3oTOsPucfa6oNb6U9u+93WP9HT6o0sTjh96SwNXjFJ9rtL26Ekvdzse3qmcX6+x0KUo1tpqju5J209tlfVDg0JJjf84xx52lCKD2p7EBJkr1A9StUKDhDmp9/6iNZ+O4O7HWjwlGnq0LbtR1VNXxw5coYqMzleT12H/vd6NXqRNX2KD2vT63zeG2nq0mv8/cvv1aRnUlZji6KGDP5zrM0DoW2n6fTAs+WkPsr/WMC8/G5N2rLN2uA/GHbsPEbP8zOARaPbeJDba9HO96XzV69co0wsgJ54tbLUe/yVypT9qzZ0+XKcqhEYjVr849FXuDSwdK760b7Qyh26zqAPWfDhecHKgkDF88Pzwv+vvl+LnuP+43iegp1oy9bA9x0be5XqNRlPr/Nz/9/rVenFdyK4zdryO6C8xH//96VK9PUPXRKsyQdHT6bsXf6ydofFrq1qx1bZYDIiu7UlMz7rbZnLp5aXMhw5e0K14Is9pw7JJpTPNsjj3im22sf+28R1+e/s3crIq5g1IETCyrxLeJ2ybAfNnxftWSfHms6IpAfQBo6h7+U/lL3g7cA+8M1n9Jf3q7n6IffG15d+oA/cL2774Zq5qAnXDRUJJveatftlIKRK7zajUUAm/JChmqCde8wSX93eBsor3PYXBTrQtThn7bFg0XZKn6OpDMKUaeuKYIBAwPUxaGHSg+S6PfTzSSYRtHsg8jRsPsHD1cDn2g3HqqPBsXrtPhF4b6rngj7AkoiJ8TXHIGgyf9aSXmMSyPX4HkCQ3pHzVY3AiKg/RoQizsX1Xn43glZE71ZHPQHrCFr0qRRV5HpakILcEAwvvF1DyCj9UAVto++79ziT5Pz5y1SwWE9+rlXUb9BA/kdbOFcLj5T8u2CsbNvhX1A7YQVr6hysTYVY2PIZr1nCrrHYiw6DHsVhq2jU/aRnBdWKLzovGElPYrNKkM72kghr73Zuk7/Dixnsbcrx6pnpedbol9h01+jEbYGUcpz79XSjfGLOR0zsRmEVUst3cozbxiue9qm3wACr2BJIi3e/VUgj13VdR9+fOtfjexYQ1f7poloIjASTGwMQZrN9MKBgMKflAXHiRPBtcpfzQAQZ3+AB4d0OSFWjnvN+Y9NmbpTB8hJrVAwoFHwGqb4p0pU1NJvSpfpR6qyNRZPguG73CdaO3Xo7uw0QSN8V7S5t6fqwRGbODj2GMCRqqYdwwAAvVWGwOuqJ9RsPCtHscQYQDM/v5Ennd2tr3Lh+Q1yOV96zXBmQCOawk9bes/ckZf68patfeB64Lkj9ab520j+Y6NlytxZi6r6gHoRnidKRqiVP1Kg7Wp6nrou+Q8igfkSDv8VV+PSb9nIdPIdtbNWEir5Lx8vrk6A1QWghNTQY//14/a/YDC9Cr7HJ+kSDvHLMgwQsDPBeNW/gRYq/DGvoJpN6yYPWzo8w4fFe82fVm2GE+LYXQUCrlhzeVLVm4feRzeV7Oc7kz92nCnWcHyV9wMseXMKI/87U5U91Fl7xtJQex7Eq6sWQfB0hNPfjCbYmUvH9PQmrgv+2Cxp8hjCZtGmRask/Ekzu38sNlB9MDwRd8OO+ygMuelXgl8KdPRtL2dgne4m1jd0c1+U5Hoh/T1ilaruxdt1+l/mI8zCoQMDc33akxUt3ekST9u0/JZoFpqFuFxYBBvGlS76vwtnP9d/h9qB9UBemKkzY+PiLFcM94xAQgP0HO/8Qkfz9s14CEvcEQjVp5Wm+eWPX7uPyrKEVcR40JVwUb8A6AMGeSxUuzwoFAvX9jxvRyL+jPawY1K3bZJwHwSF4IEA2sCCyY+eu4/I76GeFz+nYkvL+3RGwmzJjA33HVgN+j1DQas4QIQNMUxn0IAcTCCbx0FXT6MDpo6wdY+UtpL2XjBOCaILL21PCs9Byrxf+46WG3+Atq6xN8RIIeY84iFI5PWXs9Duby//QrpMH6XzsBTp54Qxr1S3yHnO8PFETC3UHsovgAo81+PnWa5742iwMPmxfnJ5qmNfqM168WDUjkzQvvVD3c2oxa5CctnTvBjaxreMiPHCPbG08z0Kl2+IxdFDu7zL/f4waT+8nJj1eCeXqB7eZuWvpoNZxgsgNcwy+osucVINAf4aGGD56hartjEo1hos/i8GWMp2n5rYGTD3ROHYgEv4JaxoMPovYiKjXoHzsBpw94xxVhEn5FkigBiEI8QoP7pWrfIMT4yeucZm5KND6MONDBaLEOdhn1nEIuAJ4Rt7E0EAgzTsgiQJLJhUXENgfQEy7hYDPI8b4PvOqtUaK24PnjHrQru9nakhrAwTdvvq+kzwj3fYzLGx69fd0FUbh+m/Yrs/PzZ+lBcC9ivPjYtnRn31SITbMbaXN7uPSeYGv4AIqj+8omsw18JkIKVoWpstewbQ/R7cSYut6yWCCs1BoMLUP3fCzX3ymruznK+LilUwPslDYYXvp4dFzJ+lxaFcmstUu95friXnPwih9h5I0YvUMOsy+/7nY83Tj5g0WHKfp1abfiRUir3gSYmekD9oWoz2nPF+5rFFrcg+XdaD7cj9fY1uQAFuCyL10+S4JMoEo+sd9L5Pb9MVAQ7DHH0aOiXaZ9JD48Oveyeg+X/vtN7ymsRAMekoGqlUPAbP3eKBC4wZC/iLdpE3dPgg8jonsjZpsStoJgz4iCh8q1rBVkYItAx2HEFM5b1vHpB4IKpjK2s/FPcFSQAER0Y/y1fwHJqvVHuUSRCnT1qM3Wbh6C4NFS3ZIcE8LXkxVQphMnGIFfPyhYfN/PJ4DrlOLn40dQ0ew+2GzOvB8IWS9f7P4YM3BbfSQBLKUxub/H2RtOGHTAlXDE9dv3KD34LNWz+Ie+EyCX6IaqxoWhjPBIDD0e8nxP8zdLuwv+8OWo3uEQNDu0i5r2TQdf2VteUPVIJq9PVpIalkC1vXFDGdh8+uIpnTF4UWJv8KMr+h+DTOEUfJGX9M+1tb+cPjMcd+31fB9jl43W9VwRoLI3anHLB9/G1M67zLRMIAQzILP6wSYc6gPYkLDYfDXqDNazGXdFgZW/abj1RkWTp66QGmyNvGwFtCH7n3mqhr+8WPJvh6m+TPc/sjxK9VRC9fZh4Vpr/uB+8A5C5kgoaI3azf7cwEpqtR01jg63qAFJLT1x5+1FNNWNDcTEi5LtIOFAS34GZvauq/QsvC/vYmFWYjkNtcJn+FbB0PrDtO8yB1O1dgCsAOCDNe1B1Rxv8h9SAjwthUxcZl0GPiagINXTlU1fLHpyC66PyKnyyfVg35wtHtm4syl8/RCk29tAoCFBpOv9Jg2qoYzei9lc99uEfDncmPbqaMWmsyIFKGh64hA4usX6F9D1fDEqv2bRUi4+4t7/IhGrZ2lajgDAuV9CDF2N1z94et0X+wpcL2RIHKDuNovw0DM8ElziV6nZg0M8xekxZQOpknswFRMgR+7uQYcBs2U6etlvhxBFz1IkOU2aapnVlr/QYskkqzrgOQZcjWXRJXAuEnfFO7qqbl54I7x8ucx5Ya+ax8WwifjJy2CTp3ZAeLY7wMEcTKVAZDALghg1jZvN4WGDF8qPje0N9pycgswPQUNrzUyrlO38Th11ML6DQc9ZjMgRCCsIFSCoX6z8T7kRvDMDgiSH4r39LhfPD9cY/qs+KcLD4ieJBrN8p0tAv4S1UQddUa3RWM8yAXN9h+JSLvN2+bs54ZV8NSUmEO+ECSZpMiQ+kwgCBolNFjQ4L1qdiB4BpPadX12H5I3ykcnLsSoGp6AReEhMNjPztbD/Q52v2DfOl2HX30slN7LPBWgN+JN7piYC5Q2e1PXNBO0CxIXrlyOE0LDzBQfmgcTtLQdSGbQ/h8GNsxgEB7JE7o9CAuY2ocPn1FnWfiheC960RbocdLuTgA5JT7AZMV50MhoZ868LaqGBSSv2H1IDNpQtJzGufOxIgz0dWAqg3zez0Djl1L9PQJX0HpIs0WQBH679nkhiKZM36DOsjCE+/qMra8wtb1TdJFQAiGh64jZXDi0KcpfywzwIC3yBNCeNxDAfIEFtRYycC0wxw0XYd2GwIk0diB764P2JZQfygOXfdgn6+VmX9V/zAH4flBtpenVgOfzP2Y/WQN+92vsf3toPCZXBz/+u8alK7H0cvOCQlY5h4XGo3Vzy/vcNE5dPENPN/za5m+j7XTU0Y+pf+J8jETBdX1tZYxYM1PV8A9k4tn7YwmbjPTPRmd3RSPe5J63cJsMSpBE//DN2lgRxMIlmIDK/MXgmGGbQpo7fysPhBoyEDDQsnzRSiK18N/x/ZtqIEFYfP9zD3WWBWRUIYPqDSUANEFhLQQDouuwJLQGw/Wd/NOK4VE+Ue5e/eepo8GBgQ6tpZ+LmMrfdxJz3xvIVLMLSGg8ROl1NBnxAE1eTMthvt8+xYWIPBJNcBzC8P2PG9KRo57CsNAvPV2/BQrup13n6eqof8Dayv6lJaT1uRCko8d5ujEaiK4//VoVITbqQnDjulk+bymuVCiYu2OlaDHtuzqZwN44F3uBnm2cn8lim6dmctWe0lPVIJqxdZmQwNVuzRzyNtXjTLRAWLJnvZjP2keH0MjStbQ6amHK5sWqbXVtJt6LTb9ji8A5sAv/GGR21ed+P8f9P8v3EQwbDu8U/1+b84gD3Md/wy0JhHiTGxLcrjUQsNEktidVYDB17WX5XydOnhcTGqQGuV5iYiLYAyBzy26e4rMWFhpIHbUTB0E4aHunxBBv9Ow3z6N9+Kmf5/dMjoGQEatDRblhwsKkhV8ZKjp0nelxHRACmVtOEEEHAanqgsD52V1hD0IA7QpyQkCCLNDAA4culmOwkBCnQLAO54JIqGvH6TMX6SNYQzZrBc8vFGG4cvVeEUw6FoDfSzIF/SSgYIYAgTQIRvQV5+g+l6s6TNUKjIrjOrjMVZnKYvNzscry8gf4qXayJIP/yiREkEsjYlI3ITyOCylYgHzVt6o66h91p/b2PI8/15zcXR21ED6xq1ed9BK594fqUt9ukmekfP3D1dHA6LpwlOe57Fqkav2jzNsHQrzJXegXt3aG3weNCs0K2ImEH7tKTctE+aP8IBns+NFhlrdo6w54QEtDW+McFGjxWXM886ujRq8Q81TXARm8tbs/eLeP/jVoNkEdtQASg8x6QOuYQahpoACCV3ZNacUNnAdoy/beAq06NWk1SR21sGLlHiXQ6nF/aolwRH+2M8nEEmHSWedWo9Ydp6mzLGzddkSm0nQdS7s3okOHTqsa/gHBau8bnjVmLpwsEI39B07JjAeeoT4PAgWCf8HiIAKFBdlHnf9gYqopJ9aAMEFhGgfC570re/m72Sl5kwKi0TUsnziTuw4TJGKSJ0m9AVM+RcsfhEA4R4JefJ2pW+xrHG5Shs6lRLujTrJa3Ha1TB6CxRsFIiM8+8t9qTu1lzoaGFm6lxHB5T43HZUJEhAE4kVukPhDm3mMwQxfWAOrwaBtoJGgIQuX7E19IudLQAZmG45hoGgT8/Dh0+JfY/ChPZipyCo7yZreDpjH9gAPkmdKVx6ijvoHIs1IyXRpIf4fhFm1Zp+qYQFzuPYBDcFUgc30UAHLJHXWJq7nouMGh/j+nIDEGrsgAAlmegk0oHz1KJdQRDCxT+QCGjdhtauv/tyTVWv20mssELQrAhM7a+7WslIrEC7wcXv+An4zCGMI7WDAghctjPR9wYrDIpRAgImskz5k4PIgztmjvDrqjDkw47meNpvlPCaOXSsj+v6ud4SZCdV5of/pLwA+s4dG5n49xb415qc19p46RA8jMq6mpiBYsNAkkImdvUc5EQCudiunp64BpuI0FuxcJfcKiwbniWXDf8N1CIZ4kRtpnxiI+sfDILMHWrbvOGaZcVww8DDAU6WzNAiIC02P1UMaCAJBw+n2sPDDaYknBped3Ii2VwxhcUqxUv1FoOCct3Ee9/3n3/uqo278VtYzgIRrDRu5TB0NDqSn2s+XuEExZ8sCS1HtAg3PBILBKV0XSTyID6AOCla5fV2oi4t8CN7B/D7DZrgdIHeK993TVND0WP0Fkz4QOnX3nOKEUEibrWnIvjPm5e3PwRIqrXxmTexYd2i7lVCi/UnWtAUinaeSgLjrcZS6fXEZ4Jooch4Tsh6b0xrINHul2XeuIJSu02K2cyowgAw1BPKSYZ5dn8P9gZVgB7LkPPxn9r3z+Zn+0sjqTW7uS6sAfRGwVZMZiTR266PKR/RZ70qqQmDEi9xITHGRjMkLE3q2LeqMzLWPP0PE2O27atMQmgfTWXbAJ7WTFgOrvUPSyLBRyz3qQesVdSCpHZhS0v4/CiL40Cze88ZIg8VUno5yi3Bi4kFQhYqmrSdx/+xmdjXHVWAApvg8BBr3KdCa9ZbcDp4dtDcIDmJr3xZEcsr7xgo23IN+9hCsCOCdPu0/RoGlqjDlX+dr6L49lSI0ra0xd8FWEf46lgByI1EnkMWwbPd61q7Z3MEiNs/tEW9v/Dm6pRADPrYe8Fb0OAON2+Du6xU2r1O2KuIyr6VtJkmeftVUDU8cZ82cBkIDRGKt7DqHr4X5bDt+k3xy+xx4Omo/L7CygX8NIeA6hy2Nb9hUDwQIK7uvDSGYrHoWWrk/tGXBIZMbQR4sytCLKvSA8V6jXJA1lt3HhWkHYnoPQix8yFuos2v+WQd9dKDNjujVMK/dQR4MXGhif6uVMM+KJYg6Go3VVlgN1bilb742rgeC4fqoi/5gLj4YXD7oTZLVVvqeMbARLEPQzAmILnsLNATj/OHM2UuUkYUPppj0OfZzncgH8xoJMTrijT7BlRk91jnijZkDzF7gt8XvBeEBV+pbfg46ZRQZdRDOgfLssZjIPuOA54BgX6DpN2humL5acyermYv+w2XtwW2qhoW4a3FUcWw70ZiIND+EeWtFQjmXy5ajnnP4n/aq6KH1YMbfx2S3J7kA89n0fbfNTx7aWPoiwb3MtGCXO5sROepvtfnZJTSsXPZstPJAYMIJUSUzTbedix5kC2EuX9sJbeYMtqLx6rmIAKuQmupPC6zU7AiZ3Fgy+A7/8CAWfjh/GseeFomCqG7qLI3p8BHPqRoQE2TVeeWI7H6Uq7kMZm/oCLEWBCgYiBAOiO5iEchlNv0wSKExMagtDacit0wCDDIsYvAGLAW7KYprII6AQNDa9ftpw6aDtH7DAVq+cg/9M3mtZMSV+CtSstGAg/JcWKuq5+JP6AEY5HkL2gWaFQNYhAUvAaATW3Qf9blYoOMdP9DAIhLcF54B6sMyQTwDqaeY+8czxfTZYG4bS2fxe+JZoeAzFszYhSfyG97ke0SWIBa1QCgiBoP5/f0HYmQzC/uiGxQsBR02crlqwRlnL523VmHZTWH2k7H5wvA1M2XZ58AVkylDp98tYjPZoJELDqjlIhjOfZFNcO/ElJZs9mIFmE5gQREXgE31L9jUhhUgAoCFhZ4vf6TelxJEs9rNRi/I9Ja73fWHd9B9LIxclgb34XXuDyyFQICwwiyAPk+3/3SDPNRv2QTaf/oonWDrAevDvxtQU/oDQSfZeqjLxP5+YO2AgtIbIZMbmVZ2jYPPTvOm2IVFkwU+H4julMs8YjTacw9YSHl/SwuBwVFLZLBoDYtBCJJAKCDpIxeTH4ML1xPNzp9RFwGhr77r5HcJKhJwMJh1P3AezF+QDgLiDW4fVgCmh2Byor1HX6hIK1ZaWmLshNUezwVxA38uAzZygOmrs+DQd0TBvX1mb2DaDuml9n7Cj8aqOn/+LBarvKqeD+rjvuAuwcKAqYylnohwQ9NCGOI4BAHiGXiO3plsBw7FSKwAzxz3i+eB89EWIvFoB88N7eB6T7KlVPS3vn7X49vxI7LB2LR1kRDBKiYNtC4IAA0GYshnNtvn71hFxaIaucxcED5j51KqNTeOnTvJ5Mkrxz0IDm2LtiEs2HfH8QeY0NUndJJ5cB28AsEKMNHs6LpotOqrIii34Z3L7g/FkKEmi1ds98mCCffxWP0vZS243Cc0tj4OYSTErkVXHXLVAyFkckuGmBpcIA0G8dwFnqYTgGwqCbrxjwxtE+6VtqgBrY8cdD1YMWC857ftgMQqX22Y7FbiMre5wK/EQIfZqrWGmPg8iCFksPLKyRoA9u6zAlbeq9KsNqx4gS5iZXD72FXEvnkDtlCyL33FfTRt4zmtpQGtb7cS8DxDXXWG2AYIhH7gXHzG8wiEfgMXijCy5+3jfAgtWFT254hniEU5uQt0oI2bDqkW3Dh4+DSTlwWdOgft4G8RgDZtjWvBBYKAC+Tj2wFteD8GOQ9sOwmhQUE00Vzsoz7AhB+6xorJvNW6qFtzMwmRreaECRsXUDIWCKijN3uQtkFwaGw2lZ+q+wVN37ac+rAG9ZhPZhJjYwc7vmOSaS1v1Unvs7uKP2ArpbTIxGOyajcERe4PS0DFIlCChY9DcCTje8T2TQlBSORGauZ/nisvAxODF2YudiVdtMTXnNy05ZAEgLDlDzQqgmzeGMW+33+Su9tDwS6qtRsF3vHk2rUbksIK7YdrQMvAPMeAgkaB9kebCPRhEciYcc7+DIC2kGRx///KuvoQqKBdaCsIKz2VN4f9amxW8DT3RdfDfdRzSIvFyrW3Weg8/nIlV92H+BnkL9JV/PZQAHfgIf4dcC76ja2fgwFLQxFRh6CFHw2hh+eFe4FwQVvIIUBQEb5/rMM6dwDCFSvDYD3JM+ZnjfNRENjDdwgUwnfvE+J+c3ZEMWkfgZZiskE7i+aGFoN2ZZKla1/c5ftiPbfUAUmYXGF/vOGziYIds3dEU2aY9dD8lZnkSmM/y1r9jxFNaedJa0mufbpKzGfuz5Zj7vXpSDl9AEKh/IfWdbHpA2vZHcedXSMnnLp4looOqWdZDNgJhq8HN8S6XyWE+H5h+n/bP5yW7k34zr9ByY3AEfymzj1my3xwz35WwbTJ5i2++2tdYjOx36BF7MvO5OO+61Ox4AB7m3fyag/tT5nmmUPtD/CtYf6XKj9IMruQcZb3hy6Sr92ctf/8hdvoOpM3ELA5YTduA/uuD2eXI8pWhvL96n6hdO8zjwaxWwD/245/Jq+jDt1m+t6HVy44EL1yL7sxMzzqdus9l/3pZSFvRAhLo1P32XzuPDn32LHQNpFAxhoENBboIKnnCxZ8X7KrAquhFlsh4yetkecRCjAdCiH8S6l+VKREL2kDvwOsLuQ5IJiXUGw8upsqjGlDH3X8jTXzT/Qha7kfB9el4aumSSBL4+T5GGozZwi15tJm7lBqNaM/zdvhHCy0Y+3BrTSa/fihK6fS3O3RdOK8e6nwZr72A3ZfmsmH9dx2HD13glrOGui+Ln/GZhH2ZaChYtHutVR9fCfK2b0svdv2Z3q7TVFZUvpdZIRsAmnfZDGhCNksv5sRdzU0chhYgNVy43r8NGtiIw7+ZTy1/62g1KhWoo21qQwLoutCZ5fydgMLZ+Kuxc+fDgVJgtwGBreChaxF72ez2KW12f/Hii+Y4fcyDLkN/l9j45Fd9FKT/LZg3mfiSzeY7rzZyL0EQ26DJIl284ZR/UndaePhnZIA4w3kindbOIqerv+V+NdWQgwTu2omStWqiKyhvtdhyG2Q5HDp6mVZIRZW7n3JXEOwKk/falRiRDMqObKZrBZLDm2NqDmSZzSx2TR/sEY2WhzCoox7AYbcBkkOUaunC3Gt+eNc1jSTTlpBweeI7O45b9SrkoEejshB4zaE9kKIewGG3AZJCphfz9qtNIWVftua12bNrUmsC/6WLDXJDsO8d3rK2vkPig5xQca9AkNug6QFJvfMbcvo5yH1rSWfyEKrmlEILIknKCA0a/PnG+en7yMjaLTsPnp3Tw0mBIbcBkkWFy5fkAUb/7CpHbliEvVZ/g/1XTGR/l47m1bs20inL1o7CCVVGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRJEP0fYv2GdMyEcoMAAAAASUVORK5CYII="));
                    image.SetAbsolutePosition(180, 700);
                    PdfWriter.DirectContent.AddImage(image, false);

                    //PdfPTable table = new PdfPTable(2);

                    // Our custom Header and Footer is done using Event Handler
                    TwoColumnHeaderFooter PageEventHandler = new TwoColumnHeaderFooter();
                    PdfWriter.PageEvent = PageEventHandler;

                    var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

                    //define el header
                    //PageEventHandler.Title = "Revisado por: " + revisadoPor.

                    //string encabezado = "Revisado por: " + revisadoPor;

                    var tablaHeader = new PdfPTable(2);

                    tablaHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

                    PdfPCell obra = new PdfPCell();

                    obra.AddElement(new Paragraph($"Nombre de obra: {solp.NombreDeObra}"));
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
                    //var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);

                    // get an ICssResolver and add the custom CSS
                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    cssResolver.AddCss(css, "utf-8", true);
                    //var cssResolverPipeline = new CssResolverPipeline(
                    //    cssResolver, htmlPipeline
                    //);

                    var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                    var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(document, PdfWriter));
                    var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    var worker = new XMLWorker(pipeline, true);
                    var charset = Encoding.UTF8;
                    var xmlParser = new XMLParser(true, worker, charset);
                    xmlParser.Parse(new StringReader(xHtml));
                    document.Close();
                    byte[] bytes = stream.ToArray();
                    stream.Close();

                    return bytes;
                }
            }
        }

        public string GenerarZipPliego(int idSolp, string pathBase)
        {
            var solp = repositorio.Obtener<Solp>(idSolp);
            var middleFileName = solp.NroSolp ?? (solp.Pliego.NombreObra ?? "xxxx");
            var pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.pdf";
            var pdfFilePath = $"{pathBase}/{pdfFilename}";

            if (solp.TipoSolp != null && solp.TipoSolp.Codigo == "CON_PLIEGO" || (solp.Urgencia == true && solp.TrabajoYaHecho != true))
            {
                File.WriteAllBytes(pdfFilePath, GenerarSolpPdf(idSolp));
            }

            if (solp.Pliego.Archivos != null && solp.Pliego.Archivos.Any<Archivo>(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp))
            {
                var zipFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.zip";
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

                        if (solp.TipoSolp != null && solp.TipoSolp.Codigo == "CON_PLIEGO" || (solp.Urgencia == true && solp.TrabajoYaHecho != true))
                        {
                            archivo.CreateEntryFromFile(pdfFilePath, pdfFilename);
                        }
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

        public List<TablaSapDto> ObtenerOrdenesSap(string idOrder = "")
        {
            OrdenWSMOAResponse resultSap = (OrdenWSMOAResponse)ordenesSolpConsumerMOA.request(idOrder);
            var codigoNum = 0;

            return resultSap.Ordenes.Select(c => new TablaSapDto()
            {
                Tabla = TablasSap.OrdenSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            }).ToList();
        }

        private List<TablaSap> ActualizarTablaSap(List<TablaSapDto> listaSap, string tablaSap)
        {
            List<TablaSap> nuevosItems = new List<TablaSap>();
            if (listaSap.Count > 0)
            {
                var listaBaseCodigoSAP = repositorio.Listar<TablaSap>(c => c.Tabla == tablaSap).Select(a => a.CodigoSap).ToList();

                nuevosItems = listaSap.Where(x => !listaBaseCodigoSAP.Contains(x.CodigoSap)).Select(item => new TablaSap()
                {
                    Codigo = item.Codigo,
                    CodigoSap = item.CodigoSap,
                    Descripcion = item.Descripcion,
                    Tabla = item.Tabla,
                    Padre_id = null,
                }).ToList();

                foreach (var item in nuevosItems)
                {
                    // uso un Agregar en lugar de AgregarTodos para que me devuelva el id de la entidad generar ya que necesito usarlo mas adelante.
                    //el AgregarTodos no devuelve el id de las entidades agregadas.
                    repositorio.Agregar(item);
                }
                repositorio.GuardarCambios();
            }
            return nuevosItems;
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
            var lista = repositorio.Listar<TablaSap, TablaSapDto>(s => new TablaSapDto
            {
                Id = s.Id,
                Descripcion = s.Descripcion,
                CodigoSap = s.CodigoSap,
                Codigo = s.Codigo,
                Tabla = s.Tabla
            }, x => x.Tabla == tabla && (
            x.Descripcion.Contains(valor) || x.CodigoSap.Contains(valor)
            ), 10000);
            return lista;
        }

        public List<ServicioSolpDto> AutocompleteServicioSolp(string valor)
        {
            string[] palabras = valor.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<ServicioSolpDto> lista = repositorio.Listar<ServicioSolp, ServicioSolpDto>(x =>
            new ServicioSolpDto
            {
                Id = x.Id,
                Codigo = x.CodigoSap,
                Descripcion = x.Descripcion,
                GrupoArticulos = x.GrupoArticulos,
                TipoServicio = x.TipoServicio,
                AmbitoServicio = x.AmbitoServicio,
                Edicion = x.Edicion,
                UnidadMedidaBase = x.UnidadMedidaBase,
                SSCItem = x.SSCItem,
            }
            , e => palabras.All(p => e.Descripcion.Contains(p)));

            return lista;
        }

        public List<ServicioSolpDto> AutocompleteCodigoServicioSolp(string valor)
        {
            List<ServicioSolpDto> lista = repositorio.Listar<ServicioSolp, ServicioSolpDto>(x =>
             new ServicioSolpDto
             {
                 Id = x.Id,
                 Codigo = x.CodigoSap,
                 Descripcion = x.Descripcion,
                 GrupoArticulos = x.GrupoArticulos,
                 TipoServicio = x.TipoServicio,
                 AmbitoServicio = x.AmbitoServicio,
                 Edicion = x.Edicion,
                 UnidadMedidaBase = x.UnidadMedidaBase,
                 SSCItem = x.SSCItem,
             }
             , e => e.CodigoSap.ToString().Contains(valor));

            return lista;
        }

        public ObtenerSolpSAPResponse ObtenerSolpsSAP(DateTime fechaDesde, DateTime fechaHasta, string numeroSolp,
                                    string centroLogistico, string filtroTipoPosicion, string indicadorDeLiberacion, string origenCreacion, List<string> creadoPorUsuarios,
                                    string tipoDeImputacion, bool ObtenerDireccionDeEntrega, bool ObtenerImputacion, bool ObtenerServicios, bool MostrarItemsBorrados
                                    )
        {
            var filtros = new ObtenerSolpRequest
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                NumeroSolp = numeroSolp,
                CentroLogistico = centroLogistico,
                FiltroTipoPosicion = filtroTipoPosicion,
                IndicadorDeLiberacion = indicadorDeLiberacion,
                OrigenCreacion = origenCreacion,
                CreadoPorUsuarios = creadoPorUsuarios,
                TipoDeImputacion = tipoDeImputacion,
                ObtenerDireccionDeEntrega = ObtenerDireccionDeEntrega,
                ObtenerImputacion = ObtenerImputacion,
                ObtenerServicios = ObtenerServicios,
                MostrarItemsBorrados = MostrarItemsBorrados,

            };
            var solps = obtenerSolpConsumerMOA.Request(filtros);

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
            var estadoSolpSapLiberada = repositorio.Obtener<TablaSap>(x => x.Tabla == "EstadoSolpSap" && x.CodigoSap == "05").Id;

            if (solp != null)
            {
                var enviarMail = solp.SeEnvioMailLiberacion != true;
                solp.FechaLiberacionSap = fechaLiberacion;
                solp.EstadoSolpSap_Id = estadoSolpSapLiberada;
                solp.SeEnvioMailLiberacion = true;
                repositorio.GuardarCambios();

                if (!solp.PeticionesDeOferta.Any())
                {
                    if (solp.TrabajoYaHecho == true)
                    {
                        CrearCotizacionConTrabajoYaHecho(solp);
                    }

                    if ((solp.TrabajoYaHecho != true && solp.Adicional == true) || solp.CondEspProveedorAsignado == true)
                    {
                        CrearPeticionAutomatica(solp, new List<int> { solp.ProveedorAsignado_Id.Value }, null, false);
                    }
                }
                if (solp.UsuarioCompras != null && solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() == "SERVICIO" && enviarMail && (solp.Urgencia != true || solp.Urgencia == true && solp.TrabajoYaHecho == true))
                {
                    try
                    {
                        EnviarMailSolpLiberada(solp, "");
                    }
                    catch (Exception)
                    {
                        Logger.Log.Info($"EnviarMailSolpLiberada Nro de SOLP: {solp.NroSolp}");
                    }
                }
            }
        }

        private void EnviarMailSolpFinalizadaConUrgencia(Solp solp)
        {
            try
            {
                Log.Info($"EnviarMailSolpFinalizadaConUrgencia Nro de SOLP: {solp.NroSolp}");
                var usuarioCreacion = repositorio.Obtener<Usuario>(solp.UsuarioCreacion_Id);
                var copia = new List<string> { usuarioCreacion.Mail };

                if (!string.IsNullOrEmpty(usuarioCreacion.Mail))
                {
                    copia.Add(usuarioCreacion.Mail);
                }

                if (!string.IsNullOrEmpty(solp.Pliego?.Email))
                {
                    //Mail del solicitante
                    copia.Add(solp.Pliego.Email);
                }

                var asunto = $"Nueva SOLP de urgencia Finalizada - {solp.NroSolp} - {usuarioCreacion.ObtenerRazonSocial()}";

                var usuariosComprasHabilitados = repositorio.Listar<UsuarioCompras>(x => x.Habilitado == true);

                var liberadoresSapId = solp.LiberadoresSapSolp.Select(x => x.LiberadorSap_Id);
                var liberadoresSap = repositorio.Listar<LiberadorSap>(x => liberadoresSapId.Contains(x.Id)).Select(x => x.Mail).ToList();

                var enviarA = usuariosComprasHabilitados.Select(x => x.Mail).ToList();
                enviarA.AddRange(liberadoresSap);
                //var enviarA = repositorio.Listar<UsuarioCompras, string>(x => x.Mail, null, x => x.Habilitado == true).ToList();

                emailService.EnviarMail(enviarA, asunto, "", copia, CuerpoMailSolpFinalizada(solp), null, "");
            }
            catch (Exception e)
            {
                Log.Info($"Error al enviar mail urgencia - Nro de SOLP: {solp.NroSolp}");
                Log.Error(e);
            }
        }

        private AlternateView CuerpoMailSolpFinalizada(Solp solp)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la finalización de la SOLP {solp.NroSolp} generada con Molinos Agro S.A. <br />";
            htmlBody += "<br/>";
            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private void EnviarMailSolpLiberada(Solp solp, string mensaje = "")
        {
            try
            {
                Log.Info($"EnviarMailSolpLiberada Nro de SOLP {solp.NroSolp}");
                Log.Info($"Copia mail comprador {solp.UsuarioCompras.Mail}");
                Log.Info($"Copia mail creador {solp.UsuarioCreacion.Mail}");
                Log.Info($"Fecha {DateTime.Now}");

                var copia = new List<string> { };
                if (!string.IsNullOrEmpty(solp?.UsuarioCreacion?.Mail))
                {
                    copia.Add(solp.UsuarioCreacion.Mail);
                    Log.Info($"Copia mail solicitante {solp.UsuarioCreacion.Mail}");
                }

                if (!string.IsNullOrEmpty(solp?.Pliego.Email))
                {
                    //Mail del solicitante
                    copia.Add(solp.Pliego.Email);
                    Log.Info($"Copia mail solicitante paso 1 {solp.Pliego.Email}");
                }

                var asunto = solp.TrabajoYaHecho == true ? "Nueva SOLP de trabajo ya hecho liberada" : "Nueva SOLP liberada";
                asunto += $": {solp.NroSolp}";
                if (solp.Adicional == true) asunto += $" - con Adicional OC: {solp.NroOrdenDeCompraAdicional}";
                var enviarA = new List<string> { solp.UsuarioCompras.Mail };

                emailService.EnviarMail(enviarA, asunto, "", copia, CuerpoMailSolpLiberada(solp, mensaje), null, "");
            }
            catch (Exception e)
            {
                Log.Info($"Error al enviar mail {solp.UsuarioCompras.Mail} - Nro de SOLP {solp.NroSolp}");
                Log.Error(e);
            }
        }

        private AlternateView CuerpoMailSolpLiberada(Solp solp, string mensaje)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la liberación de la SOLP {solp.NroSolp} generada con Molinos Agro S.A. <br />";
            htmlBody += mensaje + "<br/>";
            var esMaterial = solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";

            // Agregar la tabla de posiciones y subposiciones
            if (solp.Posiciones != null && solp.Posiciones.Any())
            {
                htmlBody += "<b>Detalle:</b><br/>";
                htmlBody += "<br/>";

                foreach (var posicion in solp.Posiciones)
                {
                    htmlBody += "<table style=\"border-collapse: collapse; border: 2px solid #ddd; text-align: center; font-size: 13px; width: 100%;\">";
                    htmlBody += "<tr>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Posición</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Centro</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Descripción</th>";
                    if (esMaterial)
                    {
                        htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Precio Bruto</th>";
                    }

                    htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Moneda</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Grupo de compras</th>" +
                                "</tr>";

                    // Agregar la fila para la posición
                    htmlBody += "<tr>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Indice}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Centro.Codigo}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{(!string.IsNullOrEmpty(posicion.MaterialSolp?.Descripcion) ? posicion.MaterialSolp.Descripcion : posicion.Tarea)}</td>";

                    if (esMaterial)
                    {
                        htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Unidad?.CodigoSap}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Cantidad.Value.ToString("n2")}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.PrecioBruto.Value.ToString("n2")}</td>";
                    }

                    htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Moneda?.CodigoSap}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.GrupoCompras?.CodigoSap}</td>" +
                                "</tr>";

                    if (!esMaterial)
                    {
                        htmlBody += "<tr>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Subposición</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Tarea a subcontratar</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Precio bruto</th>" +
                                    "</tr>";

                        foreach (var subpos in posicion.Subposiciones)
                        {
                            htmlBody += "<tr>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Numero}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Tarea}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Cantidad.Value.ToString("n2")}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Unidad.CodigoSap}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.PrecioBruto.Value.ToString("n2")}</td>" +
                                        "</tr>";
                        }
                    }

                    htmlBody += "</table>";
                    htmlBody += "<br/>";
                }
            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width='15%' src='cid:" + res.ContentId + @"'/>";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void ActualizarFechaLiberacionOC(string nroOc, DateTime fechaLiberacion)
        {
            try
            {
                var adjudicaciones = repositorio.Listar<Adjudicacion>(a => a.NumeroOrdenDeCompra == nroOc);

                foreach (var adjudicacionOC in adjudicaciones)
                {
                    adjudicacionOC.FechaLiberacionSap = fechaLiberacion;
                }

                if (adjudicaciones.Count > 0)
                {
                    repositorio.GuardarCambios();

                    try
                    {
                        EnviarMailOrdenCompra(adjudicaciones.Last(), "");
                    }
                    catch (Exception e)
                    {
                        Log.Error(new Exception($"Error al enviar mail ActualizarFechaLiberacionOC. Adjudicacion_Id: " + adjudicaciones.Last().Id));
                        Log.Error(e);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ActualizarServiciosSolp()
        {
            ServicioWSMOAResponse resultSap = (ServicioWSMOAResponse)serviciosSolpConsumerMOA.request();
            if (resultSap.Servicios.Any())
            {
                var listaBase = repositorio.Listar<ServicioSolp>();
                int agregados = 0;
                int actualizados = 0;
                foreach (var servicio in resultSap.Servicios)
                {
                    int codigoNum = 0;
                    if (Int32.TryParse(servicio.Codigo, out codigoNum))
                    {
                        var serv = listaBase.Where(x => x.CodigoSap == codigoNum).FirstOrDefault();
                        if (serv == null)
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
                            agregados += 1;
                        }
                        else
                        {
                            serv.Descripcion = servicio.Descripcion;
                            serv.GrupoArticulos = Int32.TryParse(servicio.NroGrupo, out int grupoArticulos) ? grupoArticulos : (int?)null;
                            serv.TipoServicio = servicio.Serv;
                            serv.AmbitoServicio = servicio.Ser;
                            serv.Edicion = Int32.TryParse(servicio.Edit, out int edicion) ? edicion : 0;
                            serv.UnidadMedidaBase = servicio.Bas;
                            serv.SSCItem = servicio.SSCItem;
                        }
                    }
                }
                Log.Info($"items agregados: {agregados}");
                Log.Info($"items actualizados: {actualizados}");
            }

            repositorio.GuardarCambios();
        }

        public void ActualizarMaterialesSolp()
        {
            var centros = repositorio.Listar<TablaSap>(x => x.Tabla == "Centro");
            var Materiales = new List<SustitucionMOAModel.Models.WSMapMOA.Compras.Material>();
            var centrosLista = new List<string>();
            var materialFiltro = "";

            foreach (var centr in centros)
            {
                //Materiales.AddRange(resultSap.Materiales);
                centrosLista.Add(centr.CodigoSap);
            }
            var resultSap = obtenerMaterialesSolpConsumerMOA.request(centrosLista, materialFiltro);

            Materiales = resultSap.Materiales;

            if (Materiales.Any())
            {
                var listaBase = repositorio.Listar<MaterialSolp>();

                List<string> tablasSapAConsultar = new List<string>
                {
                    TablasSap.Centro,
                    TablasSap.GrupoArticulo,
                    TablasSap.Unidad,
                    TablasSap.GrupoCompras,
                    TablasSap.CuentasSolpSap
                };

                var tablaSap = repositorio.Listar<TablaSap>(x => tablasSapAConsultar.Contains(x.Tabla));
                List<int> idsActualizados = new List<int>();
                List<TablaSap> centro = tablaSap.Where(x => x.Tabla == TablasSap.Centro).ToList();
                List<TablaSap> grupoArticulo = tablaSap.Where(x => x.Tabla == TablasSap.GrupoArticulo).ToList();
                List<TablaSap> unidad = tablaSap.Where(x => x.Tabla == TablasSap.Unidad).ToList();
                List<TablaSap> grupoCompras = tablaSap.Where(x => x.Tabla == TablasSap.GrupoCompras).ToList();
                List<TablaSap> cuentas = tablaSap.Where(x => x.Tabla == TablasSap.CuentasSolpSap).ToList();

                var contador = 0;
                int agregados = 0;
                foreach (var material in Materiales)
                {
                    try
                    {
                        var centroId = centro.FirstOrDefault(x => x.CodigoSap == material.CentroLogistico)?.Id;
                        var item = listaBase.FirstOrDefault(x => x.CodigoSap == material.NroMaterial && x.Centro_Id == centroId);

                        contador += 1;
                        if (item == null)
                        {
                            agregados += 1;
                            repositorio.Agregar(new MaterialSolp
                            {
                                Centro_Id = centroId,
                                Codigo = material.NroMaterial,
                                CodigoSap = material.NroMaterial,
                                Descripcion = material.NombreDeMaterial,
                                GrupoArticulo_Id = grupoArticulo.FirstOrDefault(x => x.CodigoSap == material.GrupoArticulo)?.Id,
                                TipoMaterial = material.TipoMaterial,
                                UnidadMedidaBase_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaBase)?.Id,
                                UnidadMedidaCompras_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaCompras)?.Id,
                                UnidadMedidaSalida_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaSalida)?.Id,
                                TipoValoracion = material.TipoValoracion,
                                PrecioMaterial = material.PrecioDelMaterial,
                                GrupoCompras_Id = grupoCompras.FirstOrDefault(x => x.CodigoSap == material.GrupoCompras)?.Id,
                                CuentaMayor_Id = cuentas.FirstOrDefault(x => x.Codigo == material.CuentaDeMayor)?.Id,
                                Estado = true,
                                TextoAmpliado = material.TextoAmpliado,
                            });
                        }
                        else
                        {
                            item.Centro_Id = centroId;
                            item.Codigo = material.NroMaterial;
                            item.CodigoSap = material.NroMaterial;
                            item.Descripcion = material.NombreDeMaterial;
                            item.GrupoArticulo_Id = grupoArticulo.FirstOrDefault(x => x.CodigoSap == material.GrupoArticulo)?.Id;
                            item.TipoMaterial = material.TipoMaterial;
                            item.UnidadMedidaBase_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaBase)?.Id;
                            item.UnidadMedidaCompras_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaCompras)?.Id;
                            item.UnidadMedidaSalida_Id = unidad.FirstOrDefault(x => x.CodigoSap == material.UnidadDeMedidaSalida)?.Id;
                            item.TipoValoracion = material.TipoValoracion;
                            item.PrecioMaterial = material.PrecioDelMaterial;
                            item.GrupoCompras_Id = grupoCompras.FirstOrDefault(x => x.CodigoSap == material.GrupoCompras)?.Id;
                            item.CuentaMayor_Id = cuentas.FirstOrDefault(x => x.Codigo == material.CuentaDeMayor)?.Id;
                            item.Estado = true;
                            item.TextoAmpliado = material.TextoAmpliado;
                            idsActualizados.Add(item.Id);
                        }

                        //Al ser alrededor de 150.000 valores guardamos cada 1.000 por si hay una excepcion en el medio.
                        if (contador % 1000 == 1)
                        {
                            repositorio.GuardarCambios();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log.Error(new Exception($"Error al grabar el material {material.ToJson()}"));
                        Logger.Log.Error(e);
                    }
                }
                Log.Info($"items agregados: {agregados}");
                Log.Info($"items actualizados: {idsActualizados.Count}");
                listaBase.Where(a => !idsActualizados.Contains(a.Id)).ToList().ForEach(a => a.Estado = false);
            }
            repositorio.GuardarCambios();
        }

        public void ObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest)
        {
            try
            {
                //TODO: ver como actualizar Solp.EstadoDocumento_Id segun la RFC
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob inicio");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob desde {obtenerSolpRequest.FechaDesde.ToString()} hasta {obtenerSolpRequest.FechaHasta.ToString()}");
                Logger.Log.Info($"ObtenerSolpesDesdeSAPJob INICIO - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");
                if (string.IsNullOrEmpty(obtenerSolpRequest.NumeroSolp)) throw new Exception("NumeroSolp no puede ser vacio");
                ObtenerSolpSAPResponse result = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(obtenerSolpRequest);
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob fin obtener solps");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob Posiciones {result.Posiciones.Count()}");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob Direcciones {result.Direcciones.Count()}");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob ImputacionesSuposiciones {result.ImputacionesSuposiciones.Count()}");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob ServiciosSuposiciones {result.ServiciosSuposiciones.Count()}");
                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob TipoImputaciones {result.TipoImputaciones.Count()}");

                List<TablaSap> ordenes = new List<TablaSap>();

                List<int> subPosicionesBorradas = new List<int>();

                List<string> tablasSapAConsultar = new List<string>
            {
                TablasSap.Moneda,
                TablasSap.Almacen,
                TablasSap.Centro,
                TablasSap.GrupoCompras,
                TablasSap.GrupoArticulo,
                TablasSap.Unidad,
                TablasSap.ClaseDocumento,
                TablasSap.CecoSolpSap,
                TablasSap.OrdenSolpSap,
                TablasSap.CentroBeneficio,
                TablasSap.CuentasSolpSap,

            };

                var tablaSap = repositorio.Listar<TablaSap>(x => tablasSapAConsultar.Contains(x.Tabla));
                var tablaGeneral = repositorio.Listar<TablaGeneral>(x => x.Tabla == "TipoSolp");

                if (result.TipoImputaciones.Count > 0)
                {
                    var listaSap = new List<TablaSapDto>();
                    var imputacionesTemp = result.TipoImputaciones.ToList();
                    foreach (var impTemp in imputacionesTemp)
                    {
                        ordenes = tablaSap.Where(x => x.Tabla == TablasSap.OrdenSolpSap).ToList();
                        if (!String.IsNullOrEmpty(impTemp.IdOrden))
                        {
                            var existeOrden = ordenes.Where(x => x.Codigo == impTemp.IdOrden).ToList();
                            if (existeOrden.Count == 0)
                            {
                                //this.ActualizarTablaSap(ObtenerOrdenesSap(impTemp.IdOrden), TablasSap.OrdenSolpSap);
                                listaSap.AddRange(ObtenerOrdenesSap(impTemp.IdOrden));
                            }
                        }
                    }
                    // agrego el resultado a la lista de ordenes de ot para usar
                    ordenes.AddRange(this.ActualizarTablaSap(listaSap, TablasSap.OrdenSolpSap));
                }

                IList<Solp> solpsFinales = new List<Solp>();

                List<TipoSolpPosicionSAP> tiposSolpPosicionSAP = repositorio.Listar<TipoSolpPosicionSAP>();
                List<SustitucionMOAModel.Entities.TipoImputacionSAP> tiposImputacionSAP = repositorio.Listar<SustitucionMOAModel.Entities.TipoImputacionSAP>();

                List<MaterialSolp> materialesSap = repositorio.Listar<MaterialSolp>();
                List<TablaSap> monedas = tablaSap.Where(x => x.Tabla == TablasSap.Moneda).ToList();
                List<TablaSap> almacenes = tablaSap.Where(x => x.Tabla == TablasSap.Almacen).ToList();
                List<TablaSap> centros = tablaSap.Where(x => x.Tabla == TablasSap.Centro).ToList();
                List<TablaSap> gruposCompras = tablaSap.Where(x => x.Tabla == TablasSap.GrupoCompras).ToList();
                List<TablaSap> gruposArticulos = tablaSap.Where(x => x.Tabla == TablasSap.GrupoArticulo).ToList();
                List<TablaSap> unidadesDeMedida = tablaSap.Where(x => x.Tabla == TablasSap.Unidad).ToList();
                List<TablaSap> clasesDeDocumento = tablaSap.Where(x => x.Tabla == TablasSap.ClaseDocumento).ToList();
                List<TablaSap> centrosDeCosto = tablaSap.Where(x => x.Tabla == TablasSap.CecoSolpSap).ToList();
                List<UsuarioDto> usuarios = repositorio.Listar<Usuario, UsuarioDto>(a => new UsuarioDto { Id = a.Id, UsuarioSap = a.UsuarioSap }, a => a.UsuarioSap != null && a.UsuarioSap != "").ToList();
                List<TablaSap> centrosDeBeneficio = tablaSap.Where(x => x.Tabla == TablasSap.CentroBeneficio).ToList();
                List<TablaSap> cuentasSolpesSap = tablaSap.Where(x => x.Tabla == TablasSap.CuentasSolpSap).ToList();
                var listaEstadosSolpSap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.EstadoSolpSap).Select(x => new TablaSapDto(x)).ToList();
                List<ServicioSolp> listaServicioSolp = repositorio.Listar<ServicioSolp>();
                int? estadoIncompletoId = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == "INCOMPLETO")?.Id;
                //int? estadoIncompletoId = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == EstadoDocumentoSolp.Incompleto.Code())?.Id;
                List<UnidadMedidaSap> unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();

                List<string> numeroSolicitudes = result.Posiciones.Select(a => a.NumeroSolicitud).Distinct().ToList();
                var solpdsDB = repositorio.Listar<Solp>(s => numeroSolicitudes.Contains(s.NroSolp));
                Solp solp = null;
                foreach (var posicion in result.Posiciones)
                {
                    try
                    {
                        SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacion = result.TipoImputaciones
                        .FirstOrDefault(dir => dir.NumeroSolicitud == posicion.NumeroSolicitud && dir.NumeroPosicion == posicion.NumeroPosicion);

                        solp = solpsFinales.SingleOrDefault(x => x.NroSolp == posicion.NumeroSolicitud);

                        bool nuevaSolp = solp == null;
                        if (nuevaSolp)
                        {
                            solp = solpdsDB.Where(s => s.NroSolp == posicion.NumeroSolicitud).SingleOrDefault() ??
                                    new Solp
                                    {
                                        FechaCreacion = DateTime.Now,
                                        EstadoDocumento_Id = estadoIncompletoId,
                                        NroSolp = posicion.NumeroSolicitud,
                                        ClaseDocumento_Id = clasesDeDocumento.SingleOrDefault(cd => cd.Codigo == posicion.TipoDocumento)?.Id,
                                        EstadoPasos = "0,0,0,0,1",
                                        TipoSolpSap = tipoImputacion != null && !string.IsNullOrEmpty(tipoImputacion.IdOrden) && posicion.OrigenCreacion == "F"
                                        ? (int?)TipoSolpSap.Mantenimiento :
                                        posicion.OrigenCreacion == "B" || posicion.OrigenCreacion == "U" ?
                                        (int?)TipoSolpSap.ReposicionAutomatica : (int?)TipoSolpSap.Sap,
                                        Pliego = new Pliego
                                        {
                                            SupervisorSector = string.Empty,
                                            SupervisorTrabajo = string.Empty,
                                            JornadaLaboralDias = string.Empty,
                                        },
                                        Posiciones = new List<SolpPosicion>(),
                                        UsuarioCreacion_Id = 0,
                                        EmailLinkToken = Guid.NewGuid()
                                    };

                            solp.ClaseDocumento_Id = clasesDeDocumento.SingleOrDefault(cd => cd.Codigo == posicion.TipoDocumento)?.Id;
                            solpsFinales.Add(solp);
                        }
                        if (solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || solp.TipoSolpSap == (int)TipoSolpSap.ReposicionAutomatica || solp.TipoSolpSap == (int)TipoSolpSap.Sap)
                        {
                            if (solp.UsuarioCreacion_Id == 0 || solp.UsuarioCreacion_Id == null)
                            {
                                var usuario = usuarios.Where(a => a.UsuarioSap == posicion.UsuarioCreado).FirstOrDefault();
                                if (usuario != null)
                                {
                                    solp.UsuarioCreacion_Id = usuario.Id;
                                }
                            }
                        }
                        var codigoSap = listaEstadosSolpSap.Where(x => x.CodigoSap == posicion.EstadoSolpSap).FirstOrDefault();
                        if (codigoSap != null)
                        {
                            solp.EstadoSolpSap_Id = codigoSap.Id;
                        }
                        DireccionSolpSAP direccion = result.Direcciones
                            .SingleOrDefault(dir => dir.NumeroSolicitud == posicion.NumeroSolicitud &&
                                                    dir.NumeroPosicion == posicion.NumeroPosicion);

                        int? tipoSolpPosicion_Id = tiposSolpPosicionSAP.FirstOrDefault(t => t.Codigo == posicion.Tipo)?.TablaGeneral_Id;
                        SustitucionMOAModel.Entities.TipoImputacionSAP tipoImputacionPosicion = tiposImputacionSAP.FirstOrDefault(t => t.Codigo == posicion.TipoImputacion);
                        TablaSap moneda = monedas.FirstOrDefault(m => m.Codigo == posicion.Moneda);
                        TablaSap almacen = almacenes.FirstOrDefault(a => a.Codigo == posicion.Almacen);
                        TablaSap centro = centros.First(c => c.Codigo == posicion.CentroLogistico);
                        TablaSap grupoCompras = gruposCompras.FirstOrDefault(g => g.Codigo == posicion.GrupoCompras);
                        TablaSap grupoArticulo = gruposArticulos.FirstOrDefault(g => g.Codigo == posicion.GrupoArticulo);

                        var posicionEntity = solp.Posiciones.SingleOrDefault(pos => pos.Indice == Int32.Parse(posicion.NumeroPosicion));

                        if (posicionEntity == null)
                        {
                            posicionEntity = new SolpPosicion
                            {
                                Indice = Int32.Parse(posicion.NumeroPosicion),
                                Subposiciones = new List<SolpSubposicion>(),
                                PaisEntrega = "AR",
                                //CodigosProveedores = string.Empty,
                                Codigo = Guid.NewGuid().ToString(),
                                Estado = posicion.EstadoPosicion != "X"
                            };
                        }

                        posicionEntity.TipoPosicion_Id = tipoSolpPosicion_Id;
                        posicionEntity.TipoImputacion_Id = tipoImputacionPosicion?.TablaGeneral_Id;
                        posicionEntity.PlazoEntrega = (int)posicion.CantidadDiasEntrega;
                        posicionEntity.FechaEntregaServicio = posicion.FechaEntregaDate;
                        posicionEntity.Centro_Id = centro.Id;
                        posicionEntity.Almacen_Id = almacen?.Id;
                        posicionEntity.NombreEntrega = direccion?.NombreUbicacion ?? string.Empty;
                        posicionEntity.CalleEntrega = (direccion?.Calle ?? string.Empty) + " " + (direccion?.Numero ?? string.Empty);
                        posicionEntity.NumeroEntrega = direccion?.Telefono ?? string.Empty;
                        posicionEntity.CpEntrega = direccion?.CodigoPostal ?? string.Empty;
                        posicionEntity.GrupoCompras_Id = grupoCompras?.Id;
                        posicionEntity.Solicitante = posicion.NombreSolicitante;
                        posicionEntity.GrupoArticulo_Id = grupoArticulo?.Id;
                        posicionEntity.Moneda_Id = moneda?.Id;
                        posicionEntity.Estado = posicion.EstadoPosicion != "X";
                        Log.Info($"ObtenerSolpesDesdeSAPJob NumeroSolp: {obtenerSolpRequest.NumeroSolp}, pos: {posicion.NumeroPosicion}, estado: {posicion.EstadoPosicion}.");
                        posicionEntity.Tarea = posicion.TextoPosicion;
                        posicionEntity.NroNecesidad = posicion.NumeroRequerimientoInterno;
                        posicionEntity.EsConcluido = true;

                        if (!string.IsNullOrEmpty(posicion.NumeroContratoMarco)) //Contrato Marco
                        {
                            var datosContratoMarco = ObtenerContratoMarco(posicion.NumeroContratoMarco, posicion.CentroLogistico);
                            posicionEntity.NumeroContratoSuperior = posicion.NumeroContratoMarco;
                            posicionEntity.NumeroPosicionContratoSuperior = posicion.PosicionContratoMarco;
                            posicionEntity.ProveedorFijo = posicion.ProveedorFijo;
                            posicionEntity.NombreProveedor = datosContratoMarco.Any() ? datosContratoMarco.First().NombreProveedor : "";
                            posicionEntity.OrganizacionCompras = posicion.OrganizacionCompras;
                        }

                        if (posicion.Tipo == "0")
                        {
                            posicionEntity.Cantidad = posicion.Cantidad;
                            var codigoUnidad = unidadMedidaSap.Where(a => a.UM == posicion.UnidadMedida).Single().Comercial;
                            posicionEntity.Unidad_Id = tablaSap.Where(x => x.CodigoSap == codigoUnidad).FirstOrDefault()?.Id;
                            posicionEntity.PrecioBruto = posicion.PrecioSolp;
                            if (tipoImputacion != null)
                            {
                                var cuentamayor = cuentasSolpesSap.Where(a => a.Codigo == tipoImputacion.CuentaContableImputada).FirstOrDefault();
                                posicionEntity.CuentaMayor_Id = cuentamayor?.Id;

                                var centrodecosto = centrosDeCosto.Where(a => a.Codigo == tipoImputacion.CentroDeCosto).FirstOrDefault();
                                posicionEntity.ValorTipoImputacion_Id = centrodecosto?.Id;
                            }
                            var material = materialesSap.Where(a => a.CodigoSap == posicion.Material && a.Centro_Id == posicionEntity.Centro_Id).FirstOrDefault();
                            posicionEntity.MaterialSolp_Id = material?.Id;

                            //posicionEntity.FechaLiberacion = posicion.FechaEstimadaLiberacionDate; es lo mismo estimada que no estimada??

                        }
                        //posicionEntity.TextoSuministro no se completa desde sap, no lo envian.

                        IList<SuposicionServicioSAP> subPosicionesDeLaPosicion =
                            result.ServiciosSuposiciones.Where(x => x.NumeroPosicion == posicion.NumeroPosicion &&
                                                                    x.NumeroSolicitud == posicion.NumeroSolicitud).ToList();
                        var numerosExistentes = subPosicionesDeLaPosicion.Select(a => Int32.Parse(a.SumeroSubPosicion) / 10).ToList();

                        var eliminadas = posicionEntity.Subposiciones.Where(a => !numerosExistentes.Contains(a.Numero)).Select(a => a.Numero);
                        //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob numerosExistentes " + numerosExistentes.ToJson());
                        //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob posicionEntity.Subposiciones " + posicionEntity.Subposiciones.Select(a => a.Numero).ToJson());
                        //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob eliminadas " + eliminadas.Count());
                        foreach (var nro in eliminadas)
                        {
                            var item = posicionEntity.Subposiciones.Where(a => nro == a.Numero).Single();
                            //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob subPosicionesBorradas.Add " + item.Id);
                            subPosicionesBorradas.Add(item.Id);
                        }
                        int subposicionIndice = 0;

                        foreach (var subPosicion in subPosicionesDeLaPosicion)
                        {
                            int indiceSubPosicion = Int32.Parse(subPosicion.SumeroSubPosicion) / 10;
                            SolpSubposicion subPosicionEntity = null;

                            if (posicionEntity.Subposiciones != null)
                            {
                                //if (subposicionIndice + 1 > posicionEntity.Subposiciones.Count())
                                //{

                                //}
                                //else
                                //{
                                //    subPosicionEntity = posicionEntity.Subposiciones.ToList()[subposicionIndice];
                                //}
                                subPosicionEntity = posicionEntity.Subposiciones.Where(a => a.Numero == indiceSubPosicion).SingleOrDefault();
                            }

                            if (subPosicionEntity == null)
                            {
                                subPosicionEntity = new SolpSubposicion();
                                posicionEntity.Subposiciones.Add(subPosicionEntity);
                            }

                            subposicionIndice += 1;

                            ImputacionSuposicionSAP imputacionSubposicion = result.ImputacionesSuposiciones
                                .FirstOrDefault(ims => ims.NumeroSolicitud == subPosicion.NumeroSolicitud &&
                                                       ims.NumeroPosicion == subPosicion.NumeroPosicion &&
                                                       ims.NumeroSubPosicion == subPosicion.SumeroSubPosicion);

                            SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacionSAP = result.TipoImputaciones
                                .FirstOrDefault(ti => ti.NumeroSolicitud == subPosicion.NumeroSolicitud &&
                                                      ti.NumeroPosicion == subPosicion.NumeroPosicion &&
                                                      ti.NumeroDeSerie == imputacionSubposicion.NumeroActualImputacion);

                            var codigoUnidad = unidadMedidaSap.Where(a => a.UM == subPosicion.UnidadDeMedida).Single().Comercial;

                            TablaSap tipoImputacionSubposicion =
                                tipoImputacionPosicion == null || imputacionSubposicion == null || tipoImputacionSAP == null ? null :
                                tipoImputacionPosicion.Codigo == "K" ? centrosDeCosto.FirstOrDefault(ceco => Int32.Parse(ceco.CodigoSap) == Int32.Parse(tipoImputacionSAP.CentroDeCosto)) :
                                tipoImputacionPosicion.Codigo == "F" ? ordenes.FirstOrDefault(o => o.Codigo == tipoImputacionSAP.IdOrden) :
                                tipoImputacionPosicion.Codigo == "Y" ? centrosDeBeneficio.FirstOrDefault(cebe => cebe.CodigoSap == tipoImputacionSAP.CentroDeBeneficio) :
                                null;

                            TablaSap cuentaSolpSap =
                                tipoImputacionPosicion == null || imputacionSubposicion == null || tipoImputacionSAP == null ? null :
                                cuentasSolpesSap.FirstOrDefault(c => Int32.Parse(c.Codigo) == Int32.Parse(tipoImputacionSAP.CuentaContableImputada));

                            int codigoServicio = 0;
                            ServicioSolp servicioSolp = Int32.TryParse(subPosicion.CodigoServicio, out codigoServicio) ? listaServicioSolp.Where(x => x.CodigoSap == codigoServicio).FirstOrDefault() : null;

                            subPosicionEntity.Numero = indiceSubPosicion;
                            //subPosicionEntity.Codigo = subPosicion.CodigoServicio;
                            subPosicionEntity.ServicioSolp_Id = servicioSolp?.Id;
                            subPosicionEntity.Tarea = subPosicion.DescripcionServicio;
                            subPosicionEntity.CuentaMayor_Id = cuentaSolpSap?.Id;
                            subPosicionEntity.TipoImputacion_Id = tipoImputacionSubposicion?.Id;
                            subPosicionEntity.Cantidad = subPosicion.Cantidad;
                            subPosicionEntity.Unidad_Id = tablaSap.Where(x => x.CodigoSap == codigoUnidad).FirstOrDefault()?.Id;
                            subPosicionEntity.PrecioBruto = subPosicion.PrecioUnitario;
                            subPosicionEntity.Estado = true;

                            if (string.IsNullOrEmpty(subPosicionEntity.Codigo))
                            {
                                subPosicionEntity.Codigo = Guid.NewGuid().ToString();
                            }

                            //TablaSap unidadMedidapos = unidadesDeMedida.FirstOrDefault(um => um.Codigo == subPosicion.UnidadDeMedida);
                            //subPosicionEntity.Unidad_Id = unidadMedidapos?.Id;
                            //subPosicionEntity.PrecioBruto = subPosicion.PrecioUnitario;
                            //var cuentamayor = cuentasSolpesSap.Where(a => a.Codigo == tipoImputacionSubposicion.Codigo).FirstOrDefault();
                            //subPosicionEntity.CuentaMayor_Id = cuentamayor?.Id;
                            //var centrodecosto = centrosDeCosto.Where(a => a.Codigo == tipoImputacionSubposicion.Codigo).FirstOrDefault();
                            //subPosicionEntity.TipoImputacion_Id = centrodecosto?.Id;
                            //var centroDeBeneficio = centrosDeBeneficio.Where(a => a.Codigo == tipoImputacionSubposicion.Codigo).FirstOrDefault();
                            //subPosicionEntity.TipoImputacion_Id = centroDeBeneficio?.Id;
                        }

                        solp.Posiciones.Add(posicionEntity);


                        if (solp.Id == 0)
                            repositorio.Agregar(solp);
                    }
                    catch (Exception e)
                    {
                        Logger.Log.Info($"Error al agregar la SOLP {posicion.NumeroSolicitud} - NumeroPosicion {posicion.NumeroPosicion}");
                        Logger.Log.Error(e);
                        continue;
                    }
                }
                SetNombreDePedido(solp);

                //Logger.Log.Info($"ObtenerSolpesDesdeSAPJob subPosicionesBorradas " + subPosicionesBorradas.Count());

                if (subPosicionesBorradas.Count() > 0)
                {
                    var subposborradas = repositorio.Listar<SolpSubposicion>(x => subPosicionesBorradas.Contains(x.Id));

                    foreach (var subpos in subposborradas.ToList())
                    {
                        if (subpos.Cotizaciones.Any())
                        {
                            foreach (var cotizacion in subpos.Cotizaciones.ToList())
                            {
                                repositorio.Remover(cotizacion);
                            }
                        }
                        repositorio.Remover(subpos);
                    }
                }
                repositorio.GuardarCambios();
                ValidarSolpAnulada(obtenerSolpRequest.NumeroSolp);
                Logger.Log.Info($"ObtenerSolpesDesdeSAPJob FIN - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");

                //actualizo el estado en la creacion/actualizacion del la solp
                //foreach (var resultPosicion in result.Posiciones)
                //{
                //    var codigoSap = listaEstadosSolpSap.Where(x => x.CodigoSap == resultPosicion.EstadoSolpSap).FirstOrDefault();
                //    if (codigoSap != null)
                //    {
                //        ActualizarEstadoSolp(resultPosicion.NumeroSolicitud, codigoSap.Id);
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
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

        public void ActualizarEstadoSolpPorId(string nroSolp)
        {
            var lista = repositorio.Listar<TablaSap>(x => x.Tabla == "EstadoSolpSap")
                    .Select(x => new TablaSapDto(x)).ToList();
            this.ConsultaEstadoSolp(nroSolp, lista);
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
                if (codigoSap != null)
                {
                    ActualizarEstadoSolp(nroSolp, codigoSap.Id);
                }
            }
        }

        private void ActualizarEstadoSolp(string nroSolp, int idEstado)
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
            var usuariosCompras = repositorio.Listar<UsuarioComprasRelacionConUsuarios>(x => x.Usuario_Id == usuarioActual.Id && x.UsuarioCompras.Habilitado)
                .Select(x => new UsuarioComprasRelacionConUsuariosDto
                {
                    Usuario = new UsuarioDto(x.Usuario),
                    Id = x.Id,
                    UsuarioCompras = new UsuarioComprasDto(x.UsuarioCompras)
                });

            return usuariosCompras.ToList();
        }

        public List<MaterialSolpDto> AutocompleteMaterialSolp(string valor, int centroId)
        {
            string[] palabras = valor.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<MaterialSolpDto> lista = repositorio.Listar<MaterialSolp>(e =>
                palabras.All(p => e.Descripcion.Contains(p)) && e.Centro_Id == centroId && e.Estado, 0, null, DirOrden.Asc)
                .Select(s => new MaterialSolpDto(s)).ToList();

            return lista;
        }

        public List<MaterialSolpDto> AutocompleteCodigoMaterialSolp(string valor, int centroId)
        {
            List<MaterialSolpDto> lista = repositorio.Listar<MaterialSolp>(e =>
                (e.Descripcion.Contains(valor) || e.CodigoSap.ToString().Contains(valor)) && e.Centro_Id == centroId && e.Estado, 0, null, DirOrden.Asc)
                .Select(s => new MaterialSolpDto(s)).ToList();

            return lista;
        }

        public void EnviarEmailSolp(EmailComposeDto emailCompose)
        {
            Handlebars.RegisterHelper("isTrue", (ctx, args) =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                string str1 = args[0].ToString();


                return str1 == "true" || str1 == "1" || str1 == "True";
            });


            var templateContent = GetSolpEmailTemplate();
            var template = Handlebars.Compile(templateContent);
            var bodyHtml = template(emailCompose);

            emailService.EnviarMail(
                emailCompose.To,
                emailCompose.Subject,
                bodyHtml,
                emailCompose.Cc,
                null,
                null,
                null,
                emailCompose.From,
                emailCompose.Bcc);
        }

        public SolpDescargaZipPorLink PuedeDescargarPliegoDesdeLink(int solpId, Guid? token)
        {
            var solp = repositorio.Obtener<Solp>(x => x.Id == solpId);

            if (solp == null)
            {
                return SolpDescargaZipPorLink.SolpIdNoExiste;
            }
            else
            {
                if (solp.EmailLinkToken != token)
                {
                    return SolpDescargaZipPorLink.EmailTokenInvalido;
                }
            }

            if (solp.TipoSolp.Descripcion == "SIN_PLIEGO" && solp.Pliego.Archivos.Count > 0)
            {
                return SolpDescargaZipPorLink.SinArchivos;
            }

            return SolpDescargaZipPorLink.PuedeDescargar;
        }

        private string GetSolpEmailTemplate()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "solp-email", "solp-email.hbs");
            using (var fileStream = File.OpenRead(templatePath))
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }

        public List<ProvinciaDto> ListarProvincia()
        {
            List<ProvinciaDto> lista = repositorio.Listar<Provincia>()
                  .Select(s => new ProvinciaDto(s)).ToList();
            return lista;
        }

        //Fuente de aprovisionamiento es donde consultamos cuando ponemos un numero de material y asociamos un contrato
        public List<FuenteAprovisionamientoDto> ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro)
        {
            var result = obtenerFuenteAprovisionamientoConsumerMOA.request(fechaEntregaPosicion, numeroMaterial, centro);
            return result.ContratosAprovisionamiento.Select(item => new FuenteAprovisionamientoDto
            {
                ProveedorFijo = item.ProveedorFijo,
                NombreProveedor = item.NombreProveedor,

                CentroAprovisionamiento = item.CentroAprovisionamiento,
                NumeroContratoSuperior = item.NumeroContratoSuperior,
                NumeroPosicionContratoSuperior = item.NumeroPosicionContratoSuperior,
                NumeroRegistroInfoCompras = item.NumeroRegistroInfoCompras,
                TipoDocumentoCompras = item.TipoDocumentoCompras,
                OrganizacionCompras = item.OrganizacionCompras,
                UnidadMedida = item.UnidadMedida,
                TipoPosicionDocumento = item.TipoPosicionDocumento,
                NumeroMaterial = item.NumeroMaterial,
                TipoPosicionDocumentoCompras = item.TipoPosicionDocumentoCompras
            }).ToList();
        }

        //Obtener contrato es lo que consultamos cuando vamos a crear una posicion desde contrato marco
        public List<ContratoSolp> ObtenerContratoMarco(string numeroContrato, string centro)
        {
            var result = obtenerContratoSolpConsumerMOA.Request(numeroContrato, centro);
            return result.ContratosSolp;
        }

        public ListaPaginada<SolpDto> ListarSolpComprador(int usuario_Id, Paginacion paginacion, string nroSolp, DateTime? desde, DateTime? hasta, bool? sap, bool? mantenimiento, bool? web, bool? repoAutomatica, bool? listarPendiente, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null)
        {

            var solpsSAP = new List<string>();
            var solps = new List<string>();
            if (listarPendiente == true)
            {
                solps.AddRange(listarSolpPendienteConsumeMOA.ListarSolpPendientes());
            }
            else
            {
                if (!string.IsNullOrEmpty(nroSolp))
                {
                    nroSolp = nroSolp.Trim();
                    solps.Add(nroSolp);
                }
            }

            var todasLasSolp = repositorio.ListarConsultaPaginada(new ListarSolpConsulta(paginacion, solps, desde, hasta, sap, mantenimiento, web, repoAutomatica, listarPendiente, usuarios, estados, centros, grupoDeCompras, usuario_Id, claseDocumento, tipoImputacion, valorTipoImputacion));

            if (todasLasSolp != null && todasLasSolp.Count() > 0)
            {
                var listId = todasLasSolp.Select(y => y.Id.Value).ToList();
                var solpsDB = repositorio.Listar<Solp>(x => listId.Contains(x.Id));
                todasLasSolp.FirstOrDefault().ItemsTotales = todasLasSolp.ItemsTotales;
                if (listarPendiente == true)
                {
                    todasLasSolp.FirstOrDefault().ItemPorPagina = todasLasSolp.Count();
                }
                else
                {
                    todasLasSolp.FirstOrDefault().ItemPorPagina = 10;
                }

                foreach (var item in todasLasSolp.Items)
                {
                    item.CentroFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.CentroCodigo).GroupBy(x => x.CentroCodigo).Select(x => x.Key)) : "";
                    item.GrupoCompraFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.GrupoComprasCodigo).GroupBy(x => x.GrupoComprasCodigo).Select(x => x.Key)) : "";

                    if (item.VerPublicar == true && item.PosicionCompras.Count() > 0)
                    {
                        var solpDB = solpsDB.First(i => i.Id == item.Id);
                        if (item.PosicionCompras.First().TipoPosicion.Codigo == "SERVICIO")
                        {
                            if (solpDB.Adjudicaciones.Count > 0)
                            {
                                item.VerPublicar = false;
                            }
                        }
                        else
                        {
                            var verPublicarDeshabilitado = solpDB.Posiciones.All(d => d.Cantidad <= d.AdjudicacionPosiciones.Sum(ap => ap.Cantidad));
                            if (verPublicarDeshabilitado)
                            {
                                item.VerPublicar = false;
                            }
                        }

                        if (solpDB.CondEspProveedorAsignado == true)
                        {
                            item.VerPublicar = false;
                        }

                        if (solpDB.TrabajoYaHecho == true && solpDB.Urgencia == true)
                        {
                            item.VerPublicar = false;
                        }
                    }
                }
            }
            return todasLasSolp;
        }

        public ListaPaginada<PeticionDeOfertaDto> ListarPOProveedor(Paginacion paginacion, string nroSolp, string nroPo, string nombrePedido, string username, DateTime? desde, DateTime? hasta, int? estadoLicitacion, int? estadoCotizacion)
        {
            try
            {
                var cuitUsuario = repositorio.Obtener<Usuario>(a => a.Mail == username).CUITRegistro;
                string[] palabras = nombrePedido.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var todasLasPO = repositorio.ListarConsultaPaginada(new ListarSolpPOConsulta(paginacion, nroSolp, nroPo, palabras, cuitUsuario, estadoCotizacion, estadoLicitacion, desde, hasta));
                var listId = todasLasPO.ToList().Select(y => y.Id);
                if (todasLasPO != null && todasLasPO.Count() > 0)
                {
                    var peticionesDeOferta = repositorio.Listar<PeticionDeOferta>(x => listId.Contains(x.Id));
                    todasLasPO.FirstOrDefault().ItemsTotales = todasLasPO.ItemsTotales;
                    foreach (var item in todasLasPO)
                    {
                        if (peticionesDeOferta.Where(x => x.Id == item.Id).FirstOrDefault().Solp.Pliego != null)
                        {
                            item.VisitasMasivas = peticionesDeOferta.Where(x => x.Id == item.Id).FirstOrDefault().Solp.Pliego.VisitasMasivas.Select(x => x.FechaHora.HasValue ? x.FechaHora : (DateTime?)null);
                            item.TieneVisitaObra = peticionesDeOferta.Where(x => x.Id == item.Id).FirstOrDefault().Solp.Pliego.TieneVisitaObra == null ? "No requiere visita" : "Requiere visita a coordinar";
                        }
                    }
                }
                return todasLasPO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PeticionDeOfertaDto ListarOfertasComprador(int PeticionOferta_Id, UsuarioDto usuario)
        {
            try
            {
                var hoy = DateTime.Now;
                var todasLasOfertas = repositorio.ObtenerConsultaEscalar(new ComparadorOfertasConsulta(PeticionOferta_Id));
                Dictionary<int, decimal> tipodecambio = new Dictionary<int, decimal>();
                var destino = repositorio.Obtener<TablaSap>(x => x.Codigo == "ARP" && x.Tabla == TablasSap.Moneda);
                var adjudicaciones = repositorio.Listar<Adjudicacion>(x => x.Solp_Id == todasLasOfertas.Solp_Id);
                DateTime fechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString());
                DateTime fechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinConsultaSolp"].ToString());
                var filtros = new ObtenerSolpRequest
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    NumeroSolp = todasLasOfertas.NroSolp,
                };
                var esAdmin = usuario.Permisos.Any(p => p == "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS");
                var solp = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(filtros);
                var noSolicitoVerPrecios = ValidarVisualizarPrecio(usuario.Id, PeticionOferta_Id);
                var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
                if (todasLasOfertas.TipoPosicionCodigo == "MATERIALES")
                {
                    unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(todasLasOfertas.PeticionDeOfertaPosicion.Select(x => x.Posicion.CodigoMaterialSap.Codigo).ToList());
                }
                foreach (var item in todasLasOfertas.Usuarios)
                {
                    var respetaMateriales = true;
                    if (item.Cotizacion != null && item.Cotizacion.CotizacionPosiciones != null)
                    {
                        if (item.Cotizacion.RespetaMateriales == false)
                            respetaMateriales = false;

                        foreach (var cotizacionPosicion in item.Cotizacion.CotizacionPosiciones.Where(x => !x.EstaEliminado && x.NoDisponible != true))
                        {
                            var solpPosicion = todasLasOfertas.PeticionDeOfertaPosicion.Where(x => x.Id == cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id).First()?.Posicion;
                            if (solpPosicion != null && cotizacionPosicion.UnidadMedida != null && !string.IsNullOrEmpty(cotizacionPosicion.UnidadMedida.Descripcion) && cotizacionPosicion.UnidadMedida.Descripcion != solpPosicion.Unidad.Descripcion
                                && !string.IsNullOrEmpty(solpPosicion.CodigoMaterialSap.Codigo))
                            {
                                var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.CodigoMaterialSap.Codigo).ToList();
                                var unidadSolicitada = unidadesDelMaterial.First(x => x.UnidadDeMedida == solpPosicion.Unidad.Descripcion);
                                if (!string.IsNullOrEmpty(cotizacionPosicion.UnidadMedida.Descripcion))
                                {
                                    var unidadCotizada = unidadesDelMaterial.First(x => x.UnidadDeMedida == cotizacionPosicion.UnidadMedida.Descripcion);
                                    cotizacionPosicion.UnidadMedida.Descripcion = unidadSolicitada.UnidadDeMedida;
                                    cotizacionPosicion.Cantidad = Math.Round(cotizacionPosicion.Cantidad * (unidadCotizada.Numerador / unidadCotizada.Denominador) / (unidadSolicitada.Numerador / unidadSolicitada.Denominador), 2);
                                    cotizacionPosicion.Precio = Math.Round((cotizacionPosicion.Precio / (unidadCotizada.Numerador / unidadCotizada.Denominador)) * (unidadSolicitada.Numerador / unidadSolicitada.Denominador), 2);
                                }
                            };

                            if (!tipodecambio.TryGetValue(cotizacionPosicion.Moneda_Id, out decimal cambio) && cotizacionPosicion.Moneda_Id > 0)
                            {
                                var tipoCambio = ObtenerTipoCambio(cotizacionPosicion.Moneda_Id, destino.Id, DateTime.Now);
                                tipodecambio.Add(cotizacionPosicion.Moneda_Id, tipoCambio.TipoCambio);
                                cambio = tipoCambio.TipoCambio;
                            }

                            if (cotizacionPosicion.CotizacionSubPosiciones != null)
                            {
                                foreach (var subpos in cotizacionPosicion.CotizacionSubPosiciones)
                                {
                                    if (subpos.Moneda_Id != null && !tipodecambio.TryGetValue(subpos.Moneda_Id.Value, out cambio) && subpos.Moneda_Id > 0)
                                    {
                                        var tipoCambio = ObtenerTipoCambio(subpos.Moneda_Id.Value, destino.Id, DateTime.Now);
                                        tipodecambio.Add(subpos.Moneda_Id.Value, tipoCambio.TipoCambio);
                                        cambio = tipoCambio.TipoCambio;
                                    }

                                    subpos.TotalARPSubPosCotizacion = cambio * subpos.PrecioTotalSubPosCotizacion;
                                }
                                cotizacionPosicion.TotalPosicionCotizacion = cotizacionPosicion.CotizacionSubPosiciones.Sum(x => x.PrecioTotalSubPosCotizacion);
                            }
                            cotizacionPosicion.TotalPesos = cambio * cotizacionPosicion.PrecioTotal;
                            cotizacionPosicion.TotalARPCotizacionPosicion = cotizacionPosicion.CotizacionSubPosiciones.Sum(x => x.TotalARPSubPosCotizacion);
                        }
                        item.Cotizacion.TotalGlobal = item.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalPesos);
                        item.Cotizacion.TotalGlobalSubPos = item.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalARPCotizacionPosicion);
                    }
                    //  item.VerAdjudicar = item.Cotizacion == null ? false : item.Cotizacion != null && item.PlazoDeOferta.Date <= hoy && item.Cotizacion.CotizacionEstadoDescripcion == "Cotizado" ? false : item.EstaHabilitado ? false : todasLasOfertas.EstaLiberado ? false: true;


                    var mensaje = "Adjudicar";
                    var verAdjudicar = true;
                    item.VerImportes = true;
                    if (item.Cotizacion == null)
                    {
                        mensaje = "Sin Cotizar";
                        verAdjudicar = false;
                    }
                    if (item.Cotizacion != null && item.Cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Incompleta)
                    {
                        mensaje = "Oferta sin finalizar";
                        verAdjudicar = false;
                    }
                    if (item.Cotizacion != null && item.PropuestaTecnicaAprobada == false)
                    {
                        mensaje = "Propuesta técnica Rechazada";
                        verAdjudicar = false;
                    }
                    if (!todasLasOfertas.EstaLiberado)
                    {
                        mensaje = "SOLP Sin liberar";
                        verAdjudicar = false;
                    }
                    var fechaFinPlazo = (item.PlazoDeOfertaCierre == null && item.FechaCircular == null) ? item.PlazoDeOfertaOriginal :
                    item.PlazoDeOfertaCierre == null ? item.PlazoDeOfertaCircular.Value :
                    item.FechaCircular == null ? item.PlazoDeOfertaCierre.Value :
                    item.PlazoDeOfertaCierre.Value > item.FechaCircular.Value ? item.PlazoDeOfertaCierre.Value : item.PlazoDeOfertaCircular.Value;


                    if (item.Cotizacion != null && fechaFinPlazo >= hoy && todasLasOfertas.Urgencia != true)
                    {
                        mensaje = "Plazo de oferta sin finalizar";
                        verAdjudicar = false;
                        item.VerImportes = false;
                    }

                    if (esAdmin && !noSolicitoVerPrecios)
                    {
                        item.VerImportes = true;
                    }

                    if (!item.EstaHabilitado)
                    {
                        mensaje = "Proveedor desahabilitado";
                        verAdjudicar = false;
                    }
                    if (todasLasOfertas.TipoPosicionCodigo == "MATERIALES")
                    {
                        if (!respetaMateriales && !todasLasOfertas.RevisionFinalizada)
                        {
                            mensaje = "Revisión técnica sin finalizar.";
                            verAdjudicar = false;
                        }
                    }
                    else
                    {
                        if (!todasLasOfertas.RevisionFinalizada)
                        {
                            mensaje = "Revisión técnica sin finalizar.";
                            verAdjudicar = false;
                        }
                    }
                    item.MensajeAdjudicar = mensaje;
                    item.VerAdjudicar = verAdjudicar;
                }

                todasLasOfertas.VerBotonVerPrecio = noSolicitoVerPrecios && esAdmin && todasLasOfertas.Usuarios.Any(a => a.VerImportes == false);


                foreach (var posicion in todasLasOfertas.PeticionDeOfertaPosicion)
                {

                    posicion.Posicion.CantidadAdjudicada = solp != null && solp.Posiciones.Count > 0 &&
                        solp.Posiciones.Any(x => Int32.Parse(x.NumeroPosicion) == posicion.Posicion.Indice) ?
                       (solp.Posiciones.Where(x => Int32.Parse(x.NumeroPosicion) == posicion.Posicion.Indice).FirstOrDefault().Ordered) : 0; //Cantidad que ya se adjudico

                    posicion.Posicion.CantidadPendiente = solp != null && solp.Posiciones.Count > 0 && solp.Posiciones.Any(x => Int32.Parse(x.NumeroPosicion) == posicion.Posicion.Indice) ?
                        (posicion.Posicion.Cantidad - posicion.Posicion.CantidadAdjudicada) : posicion.Posicion.Cantidad; //Cantidad Pendiente

                    posicion.Posicion.CantidadAdjudicacion = posicion.Posicion.CantidadPendiente; //Cantidad A Adjudicar 

                    if (todasLasOfertas.TipoPosicionCodigo == "MATERIALES")
                    {
                        if (posicion.Posicion.CantidadPendiente <= 0)
                        {
                            posicion.Posicion.AdjudicacionCompleta = true;
                        }
                        else
                        {
                            posicion.Posicion.AdjudicacionCompleta = false;
                        }
                    }
                    else
                    {
                        if (adjudicaciones.Any(x => x.Solp_Id == posicion.Posicion.Solp_Id) &&
                            adjudicaciones.Any(x => x.Posiciones.Any(y => y.SolpPosicion_Id == posicion.Posicion.Id)))
                        {
                            posicion.Posicion.AdjudicacionCompleta = true;
                        }
                        else
                        {
                            posicion.Posicion.AdjudicacionCompleta = false;
                        }
                    }
                }

                return todasLasOfertas;
            }
            catch (Exception e)
            {
                Logger.Log.Info($"ListarOfertasComprador {e.Message}");
                Log.Error(e);
                throw;
            }
        }

        private SolpSAPDto ConvertirSOLPSAP(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        {
            SolpSAPDto solpSAP = new SolpSAPDto();
            solpSAP.Id = solpActual.Id;
            if (!string.IsNullOrEmpty(solpActual.NroSolp))
            {
                solpSAP.NroSolp = solpActual.NroSolp;
            }
            #region posiciones y servicios
            int numeroPosicion = 0;

            //•	el problema está en que siempre debes poner en el campo OUT_LINE= "000000001", sino debieras llenar otra tabla de SAP que no la estamos cargando. Para quitarle complejidad se saco dicha tabla.
            string outlineNumber = "000000001";
            string numeroPaquete = "";
            string preqItem = "";
            string serialNumber = "";
            string serviceAccountSerialNumber = "01";

            string docItem = "";

            string textId = "B03";
            string formatText = "*";

            /* Algunas cuestiones con los números que se mandan:
             * DOC_ITEM, PREQ_ITEM, OUTLINE, SERIAL_NO, PCKG_NO, corresponden al número de la posicion pero formateados de distintas formas
             */

            solpSAP.IM_PR_TYPE = solpActual.ClaseDocumento.CodigoSap;

            var unidadesCodigoSap = solpActual.Posiciones.SelectMany(p => new[] { p.Unidad?.CodigoSap }.Concat(p.Subposiciones.Select(sp => sp.Unidad.CodigoSap))).Distinct();

            var unidadesMedidaSap = repositorio.Listar<UnidadMedidaSap, dynamic>(x => new { x.Comercial, x.UM },
                x => unidadesCodigoSap.Contains(x.Comercial)).Select(x => System.Tuple.Create(x.Comercial, x.UM)).ToList();

            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
                bool eliminarPosicion = false;
                bool eliminarSubPosicion = false;
                if (posicion.TipoPosicion.Codigo.ToLower() == "servicios")
                {
                    eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                    eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                }

                eliminarPosicion = eliminarPosicion || !posicion.Estado;
                numeroPosicion++;

                preqItem = $"{numeroPosicion:00000}";
                docItem = preqItem;
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";

                var IM_PRITEM = new ZMPES5700();

                //Nombre: ZBAPIMEREQITEMIMP Denominación: Posición de SOLPED
                IM_PRITEM.PREQ_ITEM = preqItem; //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
                IM_PRITEM.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP EKGRP Grupo de compras
                IM_PRITEM.CREATED_BY = solpActual.UsuarioCreacion != null ? solpActual.UsuarioCreacion.UsuarioSap : repositorio.Obtener<Usuario>(solpActual.UsuarioCreacion_Id).UsuarioSap; //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
                IM_PRITEM.PREQ_NAME = posicion.Solicitante; //PREQ_NAME AFNAM Nombre del solicitante
                IM_PRITEM.SHORT_TEXT = posicion.Tarea; //SHORT_TEXT TXZ01 Texto breve
                IM_PRITEM.PLANT = posicion.Centro.CodigoSap.ToString(); //PLANT EWERK   Centro
                IM_PRITEM.STORE_LOC = solpActual.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento ? "" : posicion.Almacen.CodigoSap.ToString(); //STORE_LOC   LGORT_D Almacén
                IM_PRITEM.TRACKINGNO = posicion.NroNecesidad; //TRACKINGNO BEDNR   Número de necesidad
                IM_PRITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString(); //MATL_GROUP  MATKL Grupo de artículos
                IM_PRITEM.PREQ_DATE = SAPFormatter.PrepararFecha(solpActual.FechaCreacion); //PREQ_DATE   BADAT Fecha de solicitud
                IM_PRITEM.DELIV_DATE = SAPFormatter.PrepararFecha(posicion.FechaEntregaServicio ?? DateTime.Now); //DELIV_DATE EINDT   Fecha de entrega de posición
                IM_PRITEM.REL_DATE = null;

                //Estos datos se envian si la posición es de materiales
                if (posicion.TipoPosicion.Codigo.ToLower() == "materiales")
                {
                    IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
                    IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad; //QUANTITY BAMNG   Cantidad solicitud de pedido
                    IM_PRITEM.QUANTITYSpecified = true;
                    IM_PRITEM.UNIT = unidadesMedidaSap.Find(u => u.Item1 == posicion.Unidad.CodigoSap).Item2; //UNIT BAMEI - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
                    IM_PRITEM.PREQ_PRICE = (Decimal)posicion.PrecioBruto; //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
                    IM_PRITEM.PREQ_PRICESpecified = true;
                    //IM_PRITEM.PRICE_UNIT = null; //PRICE_UNIT EPEIN Cantidad base  
                    //IM_PRITEM.PRICE_UNITSpecified = true;

                    //Estos datos de imputacion se envian solo para materiales por que en servicio van a nivel de subposicion
                    if (!solpSAP.IM_PRACCOUNTList.Any(x =>
                            x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.SERIAL_NO == "01" && //SERIAL_NO	DZEKKN	Número actual de la imputación
                            x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio

                    ))
                    {
                        solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
                        {
                            PREQ_ITEM = preqItem, //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = "01", //SERIAL_NO	DZEKKN	Número actual de la imputación
                            GL_ACCOUNT = getCodigoTablaSap(posicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
                            BUS_AREA = "GENE",
                            CO_AREA = "MOA"
                        });

                        solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = "01",
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            GL_ACCOUNT = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : "",
                            BUS_AREA = "X",
                            CO_AREA = "X"
                        });
                    }
                    //Este metodo lo usamos para enviar el texto de suministro. Solo se pueden enviar 132 caracteres por linea
                    var linesTextoSuministro = getLinesFromTextoSuministro(posicion.TextoSuministro);

                    linesTextoSuministro.ForEach(texto =>
                    {
                        solpSAP.IM_PRITEMTEXTList.Add(new BAPIMEREQITEMTEXT
                        {
                            PREQ_ITEM = preqItem,
                            TEXT_ID = textId,
                            TEXT_FORM = formatText,
                            TEXT_LINE = texto
                        });
                    });

                    solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber,
                        SERIAL_NO = "01",
                        SERIAL_NO_ITEM = serialNumber,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber,
                        SERIAL_NO = "01",
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //IM_PRITEM.UNIT = null; //UNIT BAMEI   Unidad de medida de solicitud pedido
                //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido

                //Indica el tipo de posicion de la solp
                switch (posicion.TipoPosicion.Codigo.ToLower())
                //ITEM_CAT PSTYP   Tipo de posición del documento de compras
                {
                    case "servicio":
                        IM_PRITEM.ITEM_CAT = "9";
                        break;

                    case "materiales":
                    default:
                        IM_PRITEM.ITEM_CAT = "0";
                        break;
                }

                //Indica el tipo de imputacion de la solp
                if (posicion.TipoImputacion != null)
                {
                    switch (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower())
                    //ACCTASSCAT  KNTTP Tipo de imputación
                    {
                        case "centrodecosto":
                            IM_PRITEM.ACCTASSCAT = "K";
                            break;
                        case "ordendeot":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "ordendeinversion":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "siniestrobeneficio":
                            IM_PRITEM.ACCTASSCAT = "Y";
                            break;
                    }
                }
                else
                {
                    IM_PRITEM.ACCTASSCAT = "";
                }

                //IM_PRITEM.DES_VENDOR = null; //DES_VENDOR WLIEF   Proveedor deseado
                //Contrato marco          
                IM_PRITEM.FIXED_VEND = posicion.ProveedorAdjudicado_Id != null ? posicion.ProveedorAdjudicado.ObtenerCodigoProveedor() : ""; //FIXED_VEND FLIEF   Proveedor fijo
                IM_PRITEM.PURCH_ORG = posicion.OrganizacionDeComprasCodigo; //PURCH_ORG EKORG   Organización de compras
                IM_PRITEM.AGREEMENT = posicion.NumeroContratoSuperior; //AGREEMENT   KONNR Número del contrato superior
                IM_PRITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; //AGMT_ITEM   KTPNR Número de posición del contrato superior
                IM_PRITEM.INFO_REC = posicion.RegistroInfoNro; //INFO_REC    INFNR Número del registro info de compras
                IM_PRITEM.CLOSED = null; //Contrato marco? No está en este MVP //CLOSED  EBAKZ Solicitud de pedido concluida
                IM_PRITEM.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY    WAERS Clave de moneda
                IM_PRITEM.CURRENCY_ISO = null; //CURRENCY_ISO BAPIISOCD   Código ISO para moneda
                                               //IM_PRITEM.INFO_REC = null; //INFO_REC    INFNR Número del registro info de compras                            
                IM_PRITEM.PLND_DELRY = (decimal)posicion.PlazoEntrega; //PLND_DELRY PLIFZ   Plazo de entrega previsto en días
                IM_PRITEM.PLND_DELRYSpecified = true;
                IM_PRITEM.PCKG_NO = numeroPaquete; //PCKG_NO PACKNO  Nº paquete

                //Indica si esta borrada la posicion 
                if (!string.IsNullOrEmpty(solpActual.NroSolp))
                {
                    IM_PRITEM.DELETE_IND = SAPFormatter.FormatearBooleano(eliminarPosicion);
                }

                //Agregamos todos los items a la estructura de posicion
                solpSAP.IM_PRITEMList.Add(IM_PRITEM);

                //Esta es una lista de campos que SAP nos pide que enviemos una "X" con los datos.
                solpSAP.IM_PRITEMXList.Add(new ZMPES5660
                {
                    PREQ_ITEM = preqItem,
                    PREQ_ITEMX = "X",
                    PUR_GROUP = "X",
                    CREATED_BY = "X",
                    PREQ_NAME = "X",
                    SHORT_TEXT = "X",
                    MATERIAL = (IM_PRITEM.MATL_GROUP != null) ? "X" : "",
                    PLANT = "X",
                    STORE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    QUANTITY = ((decimal)IM_PRITEM.QUANTITY == 0) ? "" : "X",
                    UNIT = "X",
                    //PREQ_UNIT_ISO = "X",
                    PREQ_DATE = "X",
                    DELIV_DATE = "X",
                    //REL_DATE = "X",
                    //GR_PR_TIME = "X",
                    PREQ_PRICE = ((decimal)IM_PRITEM.PREQ_PRICE == 0) ? "" : "X",
                    //PRICE_UNIT = "X"
                    ITEM_CAT = "X",
                    ACCTASSCAT = "X",
                    //DES_VENDOR = "X",
                    FIXED_VEND = posicion.ProveedorAdjudicado_Id != null ? "X" : "",
                    PURCH_ORG = !string.IsNullOrEmpty(posicion.OrganizacionDeComprasCodigo) ? "X" : "",
                    //AGREEMENT = "X",
                    //AGMT_ITEM = "X",
                    INFO_REC = !string.IsNullOrEmpty(posicion.RegistroInfoNro) ? "X" : "",
                    //CLOSED = "X",
                    CURRENCY = "X",
                    //CURRENCY_ISO = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X",
                    DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : "",
                });


                //---Desde aca empiezan las subposiciones---
                var numeroSubPosicion = 0;
                var numeroSerialNumberItem = 0;
                string serviceLineNumber = "";
                string serialNumberItem = "";

                foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
                {
                    numeroSubPosicion++;
                    serviceLineNumber = $"{subPosicion.Numero:000000000}0";

                    //serialNumberItem = serialNumber;

                    //SUBPOSICION
                    var IM_SERVICELINE = new ZMPES5780();

                    IM_SERVICELINE.DOC_ITEM = docItem; //DOC_ITEM EBELP   Número de posición de la solicitud de pedido = PREQ_ITEM
                    IM_SERVICELINE.OUTLINE = outlineNumber; //OUTLINE OUTLINE_NO  Número de estructuración
                    IM_SERVICELINE.SRV_LINE = serviceLineNumber; //SRV_LINE    EXTROW Número de línea
                    IM_SERVICELINE.DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)); //DEL_IND DEL Indicador de borrado

                    if (subPosicion.ServicioSolp != null)
                        IM_SERVICELINE.SERVICE = subPosicion.ServicioSolp.Codigo.ToString(); //SERVICE ASNUM Número de servicio
                    else
                        IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea; //SHORT_TEXT SH_TEXT1 Texto breve

                    IM_SERVICELINE.QUANTITY = (decimal)subPosicion.Cantidad.Value; //QUANTITY MENGEV  Cantidad con signo +/ -
                    IM_SERVICELINE.QUANTITYSpecified = true;
                    IM_SERVICELINE.UOM = unidadesMedidaSap.Find(u => u.Item1 == subPosicion.Unidad.CodigoSap).Item2; //UOM MEINS - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_SERVICELINE.UOM_ISO = null; //UOM_ISO MEINS_ISO   Unidad medida base en código ISO
                    IM_SERVICELINE.GROSS_PRICE = (decimal)subPosicion.PrecioBruto.Value; //GROSS_PRICE SBRTWR Precio bruto Unitario
                    IM_SERVICELINE.GROSS_PRICESpecified = true;
                    IM_SERVICELINE.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY WAERS   Clave de moneda

                    solpSAP.IM_SERVICELINESList.Add(IM_SERVICELINE);

                    solpSAP.IM_SERVICELINESXList.Add(new ZMPES5720
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber,
                        SRV_LINE = serviceLineNumber,
                        DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)),
                        SERVICE = "X",
                        SHORT_TEXT = (subPosicion.ServicioSolp == null) ? "X" : "",
                        QUANTITY = "X",
                        UOM = "X",
                        //UOM_ISO = "X",
                        GROSS_PRICE = "X",
                        CURRENCY = "X"
                    });

                    if (!solpSAP.IM_PRACCOUNTList.Any(x =>
                            x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.SERIAL_NO == serialNumber && //SERIAL_NO    DZEKKN  Número actual de la imputación
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                        ))
                    {
                        numeroSerialNumberItem++;

                        serialNumberItem = $"{numeroSerialNumberItem:00}";

                        solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
                        {
                            PREQ_ITEM = preqItem, //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = serialNumberItem, //SERIAL_NO    DZEKKN  Número actual de la imputación
                            QUANTITY = subPosicion.Cantidad.Value, //QUANTITY	MENGE_D	Cantidad
                            GL_ACCOUNT = getCodigoTablaSap(subPosicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(subPosicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(subPosicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                        });

                        solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumberItem,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            QUANTITY = "X",
                            GL_ACCOUNT = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : ""
                        });
                    }
                    else
                    {
                        serialNumberItem = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
                            x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.SERIAL_NO == serialNumber && //SERIAL_NO    DZEKKN  Número actual de la imputación
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                        ).SERIAL_NO;
                    }

                    //IMPUTACION SUBPOSICION
                    solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber,
                        SRV_LINE = serviceLineNumber,
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = serialNumberItem,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber, //Preguntar a Ulises
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //Estos son los metodos con los que nos fijamos si se editó algun campo de la direccion de entrega. Si no editó ninguno no 
                //hace falta enviar a SAP, pero si se editó por lo menos uno tenemos que enviar todos los campos 
                CentroDireccion centroPorDefecto = repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == posicion.Centro.CodigoSap);
                TablaSap centroPorDefecto2 = repositorio.Obtener<TablaSap>(x => x.Id == posicion.Centro_Id);

                if (centroPorDefecto2.Descripcion != posicion.NombreEntrega ||
                    centroPorDefecto.Cp != posicion.CpEntrega ||
                    centroPorDefecto2.Descripcion != posicion.Centro.Descripcion ||
                    centroPorDefecto.Direccion != posicion.CalleEntrega ||
                    centroPorDefecto.Numero != posicion.NumeroEntrega)
                {
                    solpSAP.IM_PRADDRDELIVERYList.Add(
                    new ZMPES5750
                    {
                        PREQ_NO = preqItem, //PREQ_NO BANFN   Numero de SOLPED
                        PREQ_ITEM = preqItem, //PREQ_ITEM   BNFPO Número de posición de la solicitud de pedido
                        NAME = posicion.NombreEntrega, //NAME    AD_NAME1 Nombre 1
                        POSTL_COD1 = posicion.CpEntrega, //POSTL_COD1 AD_PSTCD1   Código postal de la población
                        CITY = posicion.Centro.Descripcion, //CITY    AD_CITY1 Población
                        STREET = posicion.CalleEntrega, //STREET AD_STREET   Calle
                        TEL1_NUMBR = posicion.NumeroEntrega, //TEL1_NUMBR  AD_TLNMBR1 Primer número teléfono: Prefijo + número
                    });
                }
            }
            #endregion

            return solpSAP;
        }

        private string getCodigoTablaSap(TablaSap imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private string getCodigoTablaGeneral(TablaGeneral imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private List<string> getLinesFromTextoSuministro(string texto)
        {
            const int maxLengthPerLine = 132;
            var result = new List<string>();

            if (!string.IsNullOrEmpty(texto))
            {
                var line = string.Empty;

                while (texto.Length > maxLengthPerLine)
                {
                    line = texto.Substring(0, maxLengthPerLine - 1);
                    result.Add(line);
                    texto = texto.Substring(maxLengthPerLine - 1, texto.Length - (maxLengthPerLine - 1));
                }

                if (!string.IsNullOrEmpty(texto))
                {
                    result.Add(texto);
                }
            }
            return result;
        }

        public List<AsociarContratoDto> DevolverContratosAsociados(List<SolpPosicionDto> posiciones)
        {
            var contratosParaAsociar = new List<AsociarContratoDto>();
            foreach (var p in posiciones)
            {
                if (p.FechaEntregaServicio.HasValue && p.CodigoMaterialSap != null && !string.IsNullOrEmpty(p.CodigoMaterialSap.Codigo))
                {
                    var datosPosicion = AutocompleteCodigoMaterialSolp(p.CodigoMaterialSap.Codigo, p.Centro.Id);
                    var contratos = ListarFuenteAprovisionamiento(p.FechaEntregaServicio.Value.ToString("yyyy-MM-dd"), p.CodigoMaterialSap.Codigo.Remove(0, 10), p.Centro.Codigo);
                    var asociado = new AsociarContratoDto
                    {
                        Indice = p.Indice,
                        Tarea = datosPosicion != null && datosPosicion.Count > 0 ?
                        datosPosicion[0].Descripcion : p.Tarea,
                        Codigo = p.CodigoMaterialSap.Codigo,
                        Centro = p.Centro.Codigo,
                        ContratoMarco = p.NumeroContratoSuperior,
                        Proveedor = p.ProveedorFijo,
                        ContratosAsociados = contratos,
                    };
                    contratosParaAsociar.Add(asociado);
                }
            }
            return contratosParaAsociar.Where(x => x.ContratosAsociados != null && x.ContratosAsociados.Count > 0).ToList();
        }

        public SolpCompraDto ObtenerSolpCompras(int id)
        {
            try
            {
                var registrosInfo = new List<RegistroInfoDto>();
                var solp = repositorio.ObtenerConsultaEscalar(new ObtenerSolpCompras(id));
                var hoy = DateTime.Now.Date;
                var tablaSap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                DateTime fechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString());
                DateTime fechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinConsultaSolp"].ToString());
                var filtros = new ObtenerSolpRequest
                {
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    NumeroSolp = solp.NroSolp,
                };
                var solpSAPResponse = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(filtros);
                var posiciones = solp.PosicionCompras.ToList();
                var consultaRegistro = posiciones.Where(a => !string.IsNullOrEmpty(a.MaterialComprasCodigo))
                    .GroupBy(x => new { Centro = x.Centro.CodigoSap, Material = x.MaterialComprasCodigo, GrupoDeCompras = x.GrupoCompras.CodigoSap });

                foreach (var posicionAgrupada in consultaRegistro)
                {
                    var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(posicionAgrupada.Key.Material, posicionAgrupada.Key.Centro, posicionAgrupada.Key.GrupoDeCompras, "");
                    if (registros != null)
                    {
                        CrearProveedor(registros.Select(x => x.Vendedor).ToList());
                        foreach (var posicion in posicionAgrupada)
                        {
                            foreach (var registroInfo in registros)
                            {
                                var i = 0;
                                var proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == registroInfo.Vendedor && x.TipoProveedor.Id == (int)TipoUsuarioEnum.NoGranos);
                                var usuario = proveedor?.UsuariosAsociados.Where(a => a.Mail == proveedor.Mail && a.CUITRegistro == proveedor.CUIT && a.TipoUsuario.Id == proveedor.TipoProveedor.Id).FirstOrDefault();
                                if (proveedor != null && usuario != null)
                                {
                                    decimal pendienteAdjudicar = 0;

                                    var solpSAPPosicion = solpSAPResponse?.Posiciones.FirstOrDefault(x => Int32.Parse(x.NumeroPosicion) == posicion.Indice);
                                    if (solpSAPPosicion != null)
                                    {
                                        pendienteAdjudicar = solpSAPPosicion.Cantidad - solpSAPPosicion.Ordered;
                                    }
                                    try
                                    {
                                        registrosInfo.Add(new RegistroInfoDto
                                        {
                                            Id = registroInfo.Id,
                                            Numero = i + 1,
                                            PosicionId = posicion.Id,
                                            Indice = posicion.Indice,
                                            DescripcionPosicion = posicion.Tarea,
                                            Cantidad = pendienteAdjudicar,
                                            Centro = posicion.Centro.Descripcion,
                                            FechaVigencia = registroInfo.FechaVigencia,
                                            FechaUltimaCompra = registroInfo.FechaUltimaCompra,
                                            Moneda = registroInfo.Moneda,
                                            NombreProveedor = proveedor?.RazonSocial,
                                            Codigo = registroInfo.Vendedor,
                                            Precio = registroInfo.Moneda == "USDM" ? registroInfo.Precio / 10 : registroInfo.Moneda == "CLP" ? registroInfo.Precio * 100 : registroInfo.Precio,
                                            Unidad = registroInfo.Unidad,
                                            ProveedorId = usuario.Id,
                                            Cuit = proveedor?.CUIT,
                                            Deshabilitado = registroInfo.FechaFormateada != null ? registroInfo.FechaFormateada < hoy : false,
                                            CantidadAdjudicacion = 0,
                                            MonedaId = tablaSap.Where(x => x.CodigoSap == registroInfo.Moneda).FirstOrDefault().Id,
                                            UnidadId = tablaSap.Where(x => x.CodigoSap == registroInfo.Unidad).FirstOrDefault().Id,
                                            MaterialCodigo = registroInfo.MaterialCodigo,
                                            NumeroOrdenDeCompra = registroInfo.NumeroOrdenDeCompra
                                        });

                                    }
                                    catch (Exception e)
                                    {
                                        Log.Info("Posible error al obtener el codigo de material" + registroInfo.Unidad);
                                        Log.Error(e);
                                    }
                                }
                                else
                                {
                                    //nose
                                }
                            }
                        }
                    }
                }
                solp.RegistrosInfo = registrosInfo;

                return solp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CrearProveedor(List<string> codigos)
        {
            foreach (var codigo in codigos)
            {
                if (!repositorio.Existe<Proveedor>(x => x.CodigoProveedor == codigo))
                {
                    try
                    {
                        var newProveedor = ObtenerProveedorCompras(codigo);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public RespuestaGuardarSOLP GrabarPeticionDeOferta(GuardarPeticionDeOfertaDto peticionDeOferta, HttpFileCollectionBase adjuntos, bool enviarMail, List<RegistroInfoDto> registroInfo = null)
        {
            try
            {
                var solp = new SolpDto
                {
                    Id = peticionDeOferta.SolpId
                };

                var respuestaGuardarSOLP = new RespuestaGuardarSOLP
                {
                    Solp = solp,
                    Errores = new List<string>()
                };

                if (peticionDeOferta.UsuarioIds == null || peticionDeOferta.UsuarioIds.Count == 0)
                {
                    throw new ValidationCustomException("El campo Proveedor es obligatorio");
                }
                if (peticionDeOferta.PosIds == null || peticionDeOferta.PosIds.Count == 0)
                {
                    throw new ValidationCustomException("Debe seleccionar al menos una posición");
                }

                var posiciones = repositorio.Listar<SolpPosicion>(x => peticionDeOferta.PosIds.Contains(x.Id));
                var posicionesPeticion = posiciones.Select(x => new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = x.Id }).ToList();
                if (registroInfo != null && registroInfo.Count > 0)
                {
                    foreach (var pos in posicionesPeticion)
                    {
                        pos.NumeroRegistroInfo = registroInfo.First(a => a.PosicionId == pos.SolpPosicion_Id).Id;
                    }
                }
                var fechaOferta = posiciones.First().Solp.TrabajoYaHecho == true ? DateTime.Today.AddDays(-1) : posiciones.First().Solp.Pliego?.FechaHoraEntrega;
                var usuarios = repositorio.Listar<Usuario>();

                List<PeticionDeOfertaUsuario> poUsuarios = new List<PeticionDeOfertaUsuario>();
                List<PeticionDeOfertaUsuarioAdicional> poUsuariosAdicionales = new List<PeticionDeOfertaUsuarioAdicional>();

                foreach (var proveedor in usuarios.Where(x => peticionDeOferta.UsuarioIds.Contains(x.Id)).GroupBy(a => a.CUITRegistro))
                {
                    poUsuarios.Add(new PeticionDeOfertaUsuario { Usuario_Id = proveedor.First().Id });

                    foreach (var adicional in proveedor)
                    {
                        if (adicional.Id != proveedor.First().Id)
                        {
                            poUsuariosAdicionales.Add(new PeticionDeOfertaUsuarioAdicional { Usuario_Id = adicional.Id });
                        }
                    }

                }
                var usuario = repositorio.Obtener<Usuario>(peticionDeOferta.UsuarioActual.Id);

                var peticion = new PeticionDeOferta()
                {
                    UsuarioCreador_Id = peticionDeOferta.UsuarioActual.Id,
                    Usuario = usuarios.Where(x => x.Id == peticionDeOferta.UsuarioActual.Id).FirstOrDefault(),
                    FechaCreacion = DateTime.Now,
                    Solp_Id = peticionDeOferta.SolpId,
                    Observaciones = peticionDeOferta.Observacion ?? "",
                    Posiciones = posicionesPeticion,
                    PlazoDeOferta = fechaOferta ?? posiciones.OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value,
                    Usuarios = poUsuarios,
                    UsuariosAdicionales = poUsuariosAdicionales,
                    RegistroInfo = peticionDeOferta.RegistroInfo,
                    AdjuntoPliego = peticionDeOferta.AdjuntoPliego
                };

                peticion = repositorio.Agregar(peticion);
                repositorio.GuardarCambios();

                if (adjuntos != null && adjuntos.Count > 0)
                {
                    GuardarArchivosPeticionDeOferta(peticion, adjuntos);
                }
                repositorio.GuardarCambios();

                respuestaGuardarSOLP.IdEntidad = peticion.Id;

                if (enviarMail)
                {
                    try
                    {
                        EnviarMailPeticionDeOferta(peticion, peticion.Usuarios.ToList(), peticion.UsuariosAdicionales.ToList());
                    }
                    catch (Exception e)
                    {
                        Logger.Log.Error(new Exception($"Error al enviar mail GrabarPeticionDeOferta en peticion: " + peticion.Id));
                        Logger.Log.Error(e);
                    }
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GuardarArchivosPeticionDeOferta(PeticionDeOferta peticion, HttpFileCollectionBase files)
        {
            var ruta = ObtenerRutaArchivos(peticion.Id, FileKeys.PeticionDeOferta);
            var filesEspecificaciones = files.GetMultiple("filePeticionDeOferta");
            for (int i = 0; i < filesEspecificaciones.Count; i++)
            {
                var file = filesEspecificaciones[i];
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                peticion.Archivos.Add(new PeticionDeOfertaArchivo
                {
                    Archivo = new Archivo
                    {
                        FileKey = FileKeys.PeticionDeOferta,
                        Ruta = rutaArchivoRename
                    },
                    Fecha = DateTime.Now,
                });

                file.SaveAs(rutaArchivoRename);
            }
        }

        public List<LegajoDto> ObtenerLegajo(int peticionDeOfertaId, int? peticiondeOfertaUsuarioId)
        {
            List<LegajoDto> legajo = new List<LegajoDto>();
            var peticion = repositorio.Obtener<PeticionDeOferta>(peticionDeOfertaId);
            var peticionPrecio = repositorio.Obtener<PeticionDeOfertaVisualizacionPrecio>(x => x.PeticionDeOferta_Id == peticionDeOfertaId);
            var middleFileName = peticion.Solp.NroSolp ?? peticion.Solp.Pliego.NombreObra ?? "xxxx";
            var pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.pdf";

            var tienePliego = (peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.Sap || peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.ReposicionAutomatica) && peticion.Solp.EstadoDocumento.Codigo == "CREADO";

            if (tienePliego || peticion.Solp.TipoSolp?.Codigo == "CON_PLIEGO")
            {
                //invento registro con id de archivo 0 para bajar el pliego
                legajo.Add(new LegajoDto
                {
                    ArchivoId = 0,
                    Observacion = pdfFilename,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = peticion.Solp.FechaCreacion,
                    FechaFormateado = peticion.Solp.FechaCreacion.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Solp.UsuarioCreacion.CUITRegistro, Mail = peticion.Solp.UsuarioCreacion.Mail, Id = peticion.Solp.UsuarioCreacion_Id.Value },
                    Tipo = TipoLegajo.Pliego
                });
            }

            // buscar archivos de la solp
            if (peticion.Solp.Pliego != null && peticion.Solp.Pliego.Archivos != null && peticion.Solp.Pliego.Archivos.Any<Archivo>(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp))
            {
                foreach (var archivoSubido in peticion.Solp.Pliego.Archivos)
                {
                    if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp))
                    {
                        string fileName = Path.GetFileName(archivoSubido.Ruta);
                        legajo.Add(new LegajoDto
                        {
                            ArchivoId = archivoSubido.Id,
                            Observacion = fileName,
                            PeticionDeOfertaId = peticionDeOfertaId,
                            SolpId = peticion.Solp_Id,
                            Fecha = peticion.Solp.FechaCreacion,
                            FechaFormateado = peticion.Solp.FechaCreacion.ToString("dd/MM/yyyy"),
                            Usuario = new UsuarioDto { CUIT = peticion.Solp.UsuarioCreacion.CUITRegistro, Mail = peticion.Solp.UsuarioCreacion.Mail, Id = peticion.Solp.UsuarioCreacion_Id.Value },
                            Tipo = TipoLegajo.Solp
                        });
                    }
                }
            }

            //mostrar observación ingresada en el paso 4 si es SOLP con condiciones especiales
            if (peticion.Solp.Pliego != null && (peticion.Solp.TrabajoYaHecho == true || peticion.Solp.Urgencia == true || peticion.Solp.Adicional == true || peticion.Solp.CondEspProveedorAsignado == true))
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = "Justificación de condición especial: " + peticion.Solp.Pliego.ObservacionesCotizacion,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = peticion.Solp.FechaCreacion,
                    FechaFormateado = peticion.Solp.FechaCreacion.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Solp.UsuarioCreacion.CUITRegistro, Mail = peticion.Solp.UsuarioCreacion.Mail, Id = peticion.Solp.UsuarioCreacion_Id.Value },
                    Tipo = TipoLegajo.Solp
                });
            }

            //buscar archivos de la peticion ( menos lo de legajo cuando es un usuario proveedor)
            foreach (var item in peticion.Archivos.Where(a => peticiondeOfertaUsuarioId == null || (peticiondeOfertaUsuarioId != null && a.Archivo.FileKey != FileKeys.PeticionDeOfertaLegajo)))
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = item.Archivo.Id,
                    Observacion = item.Archivo.ObtenerNombre(item.Archivo.Ruta),
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = item.Fecha,
                    FechaFormateado = item.Fecha.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Usuario.CUITRegistro, Mail = peticion.Usuario.Mail, Id = peticion.UsuarioCreador_Id },
                    Tipo = peticiondeOfertaUsuarioId == null ? TipoLegajo.Legajo : TipoLegajo.PeticionDeOferta
                });
            }

            // pdf peticion de oferta materiales
            if (peticion.Solp.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "MATERIALES")
            {
                foreach (var peticionUsuario in peticion.Usuarios.Where(u => peticiondeOfertaUsuarioId == null || u.Id == peticiondeOfertaUsuarioId))
                {
                    var pdfPOUsuario = $"PO-{peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf";
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = peticionUsuario.Id * -1,//lo ponemos en negtivo para difernciarlo de los ids de archivos
                        Observacion = pdfPOUsuario,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Solp_Id,
                        Fecha = peticion.FechaCreacion,
                        FechaFormateado = peticion.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = peticion.Usuario.CUITRegistro, Mail = peticion.Usuario.Mail, Id = peticion.UsuarioCreador_Id },
                        Tipo = TipoLegajo.PeticionDeOferta
                    });
                }
            }

            //circular
            var peticionDeOfertaUsuarios_Id = peticion.Usuarios.Where(u => peticiondeOfertaUsuarioId == null || u.Id == peticiondeOfertaUsuarioId).Select(u => u.Id).ToList();
            var circulares = repositorio.Listar<Circular>(x => x.PeticionDeOfertaUsuarios.Any(a => peticionDeOfertaUsuarios_Id.Contains(a.PeticionDeOfertaUsuario_Id)));
            var peticionVisualizacionPrecio = repositorio.Listar<PeticionDeOfertaVisualizacionPrecio>(x => x.PeticionDeOferta_Id == peticion.Id);

            foreach (var circular in circulares)
            {
                bool noLeido = false;
                if (peticiondeOfertaUsuarioId.HasValue)
                {
                    noLeido = circular.PeticionDeOfertaUsuarios.Any(a => a.PeticionDeOfertaUsuario_Id == peticiondeOfertaUsuarioId && a.Leida != true);
                }

                //buscar archivos de la circular
                foreach (var item in circular.Archivos)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = item.Id,
                        Observacion = item.ObtenerNombre(item.Ruta),
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Solp_Id,
                        Fecha = circular.FechaCreacion,
                        FechaFormateado = circular.FechaCreacion.ToString("dd/MM/yyyy"),
                        Leido = !noLeido,
                        Usuario = new UsuarioDto { CUIT = circular.Usuario.CUITRegistro, Mail = circular.Usuario.Mail, Id = circular.UsuarioCreador_Id },
                        Tipo = TipoLegajo.Circular
                    });
                }
                //buscar comentarios de la circular
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = circular.Observaciones,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = circular.FechaCreacion,
                    FechaFormateado = circular.FechaCreacion.ToString("dd/MM/yyyy"),
                    Leido = !noLeido,
                    Usuario = new UsuarioDto { CUIT = circular.Usuario.CUITRegistro, Mail = circular.Usuario.Mail, Id = circular.UsuarioCreador_Id },
                    Tipo = TipoLegajo.Circular
                });
                //buscar cambios de fechas de la circular
                if (circular.RequiereCambioDeFechas == true)
                {
                    if (circular.PlazoDeOferta.HasValue)
                    {
                        legajo.Add(new LegajoDto
                        {
                            ArchivoId = null,
                            Observacion = $"Nuevo plazo de oferta: {circular.PlazoDeOferta.Value.ToString("dd/MM/yyyy")}",
                            PeticionDeOfertaId = peticionDeOfertaId,
                            SolpId = peticion.Solp_Id,
                            Fecha = circular.FechaCreacion,
                            FechaFormateado = circular.FechaCreacion.ToString("dd/MM/yyyy"),
                            Leido = !noLeido,
                            Usuario = new UsuarioDto { CUIT = circular.Usuario.CUITRegistro, Mail = circular.Usuario.Mail, Id = circular.UsuarioCreador_Id },
                            Tipo = TipoLegajo.Circular
                        });
                    }
                    if (circular.FechaDeEntrega.HasValue)
                    {
                        legajo.Add(new LegajoDto
                        {
                            ArchivoId = null,
                            Observacion = $"Nueva fecha de entrega: {circular.FechaDeEntrega.Value.ToString("dd/MM/yyyy")}",
                            PeticionDeOfertaId = peticionDeOfertaId,
                            SolpId = peticion.Solp_Id,
                            Fecha = circular.FechaCreacion,
                            FechaFormateado = circular.FechaCreacion.ToString("dd/MM/yyyy"),
                            Leido = !noLeido,
                            Usuario = new UsuarioDto { CUIT = circular.Usuario.CUITRegistro, Mail = circular.Usuario.Mail, Id = circular.UsuarioCreador_Id },
                            Tipo = TipoLegajo.Circular
                        });
                    }
                }

                //buscar archivos de la peticion visualizacion de precio
                if (peticionVisualizacionPrecio.Count > 0)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = peticionPrecio.Archivo.Id,
                        Observacion = "PeticionDeOfertaVisualizacionPrecio" + peticionPrecio.Observaciones,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Solp_Id,
                        Fecha = peticionPrecio.FechaCreacion,
                        FechaFormateado = peticionPrecio.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = peticionPrecio.Usuario.CUITRegistro, Mail = peticionPrecio.Usuario.Mail, Id = peticionPrecio.UsuarioCreador_Id },
                        Tipo = TipoLegajo.PeticionDeOfertaVisualizacionPrecio

                    });
                }

                if (noLeido)
                {
                    foreach (var circularNoLeida in circular.PeticionDeOfertaUsuarios.Where(a => a.PeticionDeOfertaUsuario_Id == peticiondeOfertaUsuarioId && a.Leida != true))
                    {
                        circularNoLeida.Leida = true;
                        circularNoLeida.FechaLeida = DateTime.Now;
                    }
                    repositorio.GuardarCambios();
                }
            }

            //Cierres plazo de oferta
            var cierres = repositorio.Listar<PeticionDeOfertaCierre>(a => a.PeticionDeOferta_Id == peticionDeOfertaId);
            foreach (var cierre in cierres)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = cierre.Observacion,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = cierre.Fecha,
                    FechaFormateado = cierre.Fecha.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = cierre.Usuario.CUITRegistro, Mail = cierre.Usuario.Mail, Id = cierre.Usuario_Id },
                    Tipo = TipoLegajo.CierreOferta
                });
            }

            //Chat interno
            if (peticion.Solp.ChatInternoCompras != null && peticion.Solp.ChatInternoCompras.Count > 0)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = 0,
                    Observacion = "Chat interno",
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = peticion.Solp.ChatInternoCompras.First().FechaEnvio,
                    FechaFormateado = peticion.Solp.ChatInternoCompras.First().FechaEnvio.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Solp.ChatInternoCompras.First().Usuario.CUITRegistro, Mail = peticion.Solp.ChatInternoCompras.First().Usuario.Mail, Id = peticion.Solp.ChatInternoCompras.First().Usuario_Id },
                    Tipo = TipoLegajo.ChatInterno
                });
            }

            // revision tecnica anticipada
            if (peticion.RevisionTecnica != null)
            {
                if (peticion.RevisionTecnica.RecotizacionEconomica)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = 0,
                        Observacion = "Solicitud de re cotización - " + peticion.RevisionTecnica.ObservacionRecotizacion,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Solp_Id,
                        Fecha = peticion.RevisionTecnica.Fecha,
                        FechaFormateado = peticion.RevisionTecnica.Fecha.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = peticion.RevisionTecnica.Usuario.CUITRegistro, Mail = peticion.RevisionTecnica.Usuario.Mail, Id = peticion.RevisionTecnica.Usuario.Id },
                        Tipo = TipoLegajo.RevisionTecnica
                    });
                }

                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = "Finalización revisión tecnica",
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = peticion.RevisionTecnica.Fecha,
                    FechaFormateado = peticion.RevisionTecnica.Fecha.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.RevisionTecnica.Usuario.CUITRegistro, Mail = peticion.RevisionTecnica.Usuario.Mail, Id = peticion.RevisionTecnica.Usuario.Id },
                    Tipo = TipoLegajo.RevisionTecnica
                });
            }

            return legajo.OrderByDescending(x => x.Fecha).ToList();
        }

        public Resultado GuardarAdjuntosPeticionDeOferta(int idPeticion, HttpFileCollectionBase files, UsuarioDto usuarioDto)
        {
            var ruta = "C:\\adjuntospliego";//ObtenerRutaArchivosPeticionDeOferta(idPeticion);
            var peticion = repositorio.Obtener<PeticionDeOferta>(idPeticion);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));
                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                peticion.Archivos.Add(new PeticionDeOfertaArchivo
                {
                    Archivo = new Archivo
                    {
                        FileKey = FileKeys.PeticionDeOfertaLegajo,
                        Ruta = rutaArchivoRename
                    },
                    Fecha = DateTime.Now,
                });

                file.SaveAs(rutaArchivoRename);
            }

            repositorio.GuardarCambios();

            return new Resultado();
        }

        public string DescargarLegajo(int idPeticion, string pathBase, int? peticiondeOfertaUsuarioId)
        {
            var peticion = repositorio.Obtener<PeticionDeOferta>(idPeticion);

            var middleFileName = peticion.Solp.NroSolp == null ? (peticion.Solp.Pliego.NombreObra == null ? "xxxx" : peticion.Solp.Pliego.NombreObra) : peticion.Solp.NroSolp;
            var pliegoFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now.ToString("yyyyMMdd")}.pdf";
            var pdfFilePath = $"{pathBase}/{pliegoFilename}";
            File.WriteAllBytes(pdfFilePath, GenerarSolpPdf(peticion.Solp_Id));

            var zipFilename = $"PO-{peticion.Id}-{peticion.FechaCreacion.ToString("yyyyMMdd")}.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    //solp
                    if (peticion.Solp.Pliego.Archivos != null)
                    {
                        foreach (var archivoSubido in peticion.Solp.Pliego.Archivos)
                        {
                            if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp))
                            {
                                string fileName = Path.GetFileName(archivoSubido.Ruta);
                                archivo.CreateEntryFromFile(archivoSubido.Ruta, fileName);
                            }
                        }
                    }
                    //peticion de oferta
                    if (peticion.Archivos != null)
                    {
                        foreach (var archivoSubido in peticion.Archivos.Where(a => peticiondeOfertaUsuarioId == null || (peticiondeOfertaUsuarioId != null && a.Archivo.FileKey != FileKeys.PeticionDeOfertaLegajo)))
                        {
                            if (File.Exists(archivoSubido.Archivo.Ruta))
                            {
                                string fileName = Path.GetFileName(archivoSubido.Archivo.Ruta);
                                archivo.CreateEntryFromFile(archivoSubido.Archivo.Ruta, fileName);
                            }
                        }
                    }

                    // pdf peticion de oferta materiales
                    if (peticion.Solp.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "MATERIALES")
                    {
                        foreach (var peticionUsuario in peticion.Usuarios)
                        {
                            string codigoProveedor = peticionUsuario.Usuario.ObtenerProveedor().CodigoProveedor;
                            var pdf = GenerarPDFPeticionDeOferta(peticion, codigoProveedor);
                            var pdfFilePathUsuario = $"{pathBase}/PO-{peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf";
                            File.WriteAllBytes(pdfFilePathUsuario, pdf);
                            archivo.CreateEntryFromFile(pdfFilePathUsuario, $"PO-{peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf");
                        }
                    }

                    //circular
                    var peticionDeOfertaUsuarios_Id = peticion.Usuarios.Where(u => peticiondeOfertaUsuarioId == null || u.Id == peticiondeOfertaUsuarioId).Select(u => u.Id).ToList();
                    var circulares = repositorio.Listar<Circular>(x => x.PeticionDeOfertaUsuarios.Any(a => peticionDeOfertaUsuarios_Id.Contains(a.PeticionDeOfertaUsuario_Id)));

                    foreach (var circular in circulares)
                    {
                        //buscar archivos de la circular
                        foreach (var item in circular.Archivos)
                        {
                            if ((peticionDeOfertaUsuarios_Id != null && item.FileKey != FileKeys.PeticionDeOfertaLegajo) || peticionDeOfertaUsuarios_Id == null)
                            {
                                string fileName = Path.GetFileName(item.Ruta);
                                archivo.CreateEntryFromFile(item.Ruta, fileName);
                            }
                        }
                    }

                    //adjuntos del proveedor (preguntar?)
                    //agrega pliego
                    archivo.CreateEntryFromFile(pdfFilePath, pliegoFilename);

                    //Chat interno
                    if (peticion.Solp.ChatInternoCompras != null && peticion.Solp.ChatInternoCompras.Count > 0)
                    {
                        var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                        Directory.CreateDirectory(path);

                        string rutaTxt = ExportarChatInternoAtexto(peticion.Solp.Id, path);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(rutaTxt);
                        string fileName = Path.GetFileName(rutaTxt);

                        archivo.CreateEntryFromFile(rutaTxt, fileName);

                        //Para evitar sobrecargar el server con zips, una vez cargado lo borro
                        Directory.Delete(path, true);
                    }
                }
            }

            return filePath;
        }

        private byte[] GenerarPDFPeticionDeOferta(PeticionDeOferta peticion, string codigoProveedor)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4, 10f, 10f, 10f, 100f))
                    {
                        string templateFilePath = httpContextService.GetDirectory("Templates/PeticionDeOfertaTemplate.html");
                        var templateString = System.IO.File.ReadAllText(templateFilePath);
                        var xHtml = templateString;
                        xHtml = CompletarHtml(xHtml, peticion, codigoProveedor);

                        var PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                        document.Open();

                        PdfFooter PageEventHandler = new PdfFooter();
                        PdfWriter.PageEvent = PageEventHandler;

                        var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                        tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                        tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

                        var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                        var htmlPipelineContext = new HtmlPipelineContext(null);
                        htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                        var pdfWriterPipeline = new PdfWriterPipeline(document, PdfWriter);

                        // get an ICssResolver and add the custom CSS
                        var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                        var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                        hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                        var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(document, PdfWriter));
                        var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);

                        var worker = new XMLWorker(pipeline, true);
                        var charset = Encoding.UTF8;
                        var xmlParser = new XMLParser(true, worker, charset);
                        xmlParser.Parse(new StringReader(xHtml));
                        document.Close();
                        byte[] bytes = stream.ToArray();
                        stream.Close();
                        return bytes;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CompletarHtml(string xHtml, PeticionDeOferta peticion, string codigoProveedor)
        {
            var stylesHtml = @"<style>h1{color:#000;font-family:'Times New Roman',serif;font-style:italic;font-weight:700;text-decoration:none;font-size:12px}.s1{color:#000;font-family:'Times New Roman',serif;font-style:italic;font-weight:400;text-decoration:none;font-size:10px}.s2{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:700;text-decoration:none;font-size:8px}.s3{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:9px}h2{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:700;text-decoration:none;font-size:8px}p{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:7px;margin:0}table,tbody{vertical-align:top;overflow:visible}.peticion{font-family:'Times New Roman',serif;font-style:italic;font-weight:700;text-decoration:none;font-size:10px;border:.1px solid #000;border-collapse:collapse}.s4{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:700;text-decoration:none;font-size:9px}.s5{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:8px}.s6{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:700;text-decoration:none;font-size:18px}.s7{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:9px}.s8{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:9px}.s9{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:9px}table,tbody{vertical-align:top;overflow:visible}.border{border:.1px solid #000;border-collapse:collapse}.s6{color:#000;font-family:Arial,sans-serif;font-style:italic;text-decoration:none;font-size:7px}.cls_003{font-family:Arial,serif;font-size:12.1px;color:#fff;font-weight:700;font-style:normal;text-decoration:none;background-color:#000;text-align:center;top:-59px;position:relative;left:-1px;width:102%}.cls_002{font-family:Arial,serif;font-size:14.1px;color:#000;font-weight:700;font-style:italic;text-decoration:none}.noborder{border-collapse:collapse;border:1px solid #fff}.cls_005{font-family:Arial,serif;font-size:8.1px;color:#000;font-weight:700;font-style:normal;text-decoration:none}.cls_006{font-family:Arial,serif;font-size:8px;color:#000;font-weight:400;font-style:normal;text-decoration:none}.cls_008{font-family:Arial,serif;font-size:10px;color:#000;font-weight:400;font-style:normal;text-decoration:none}.cls_009{font-family:Arial,serif;font-size:11.1px;color:#000;font-weight:700;font-style:normal;text-decoration:none;text-align:center}.cls_011{font-family:Courier New,serif;font-size:10.1px;color:#000;font-weight:400;font-style:normal;text-decoration:none}.espacio{height:10px;display:block}.w33{width:30%;display:inline-block}.cls_012{font-family:Arial,serif;font-size:6px;text-align:justify}</style>";
            var datosProveedor = new VendedorDetalleWSMOAResponse() { cabeceras = null };
            try
            {
                datosProveedor = vendedorService.GetDatosFiscales(codigoProveedor, codigoProveedor);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }

            var posiciones = "";
            try
            {
                var listaPosiciones = peticion.Posiciones.Where(x => x.SolpPosicion.Estado == true && x.SolpPosicion.EsConcluido == true);
                foreach (var peti in listaPosiciones)
                {
                    var item = peti.SolpPosicion;
                    posiciones +=
                    $"<tr class='border'> <td style='font-size: 8px;'>{item.Indice} </td> " +
                    $"<td style='font-size: 8px;'> {(item.MaterialSolp != null ? item.MaterialSolp.Codigo : "")} </td>" +
                    $"<td style='font-size: 8px;'> {(item.MaterialSolp != null ? item.MaterialSolp.Descripcion : "")} </td>" +
                    $"<td style='font-size: 8px;'>{item.Cantidad}</td>" +
                    $"<td style='font-size: 8px;'>{item.Unidad.Descripcion}</td>" +
                    $"<td style='font-size: 8px;'>{peticion.PlazoDeOferta.ToString("dd.MM.yyyy")}</td>" +
                    $"<td style='font-size: 8px;'>{listaPosiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value.ToString("dd.MM.yyyy")}</td> </tr>";
                    posiciones += $"<tr><td colspan='7' style='font-size: 8px; text-align: justify'>{(item.MaterialSolp != null ? item.MaterialSolp.TextoAmpliado : "")}</td></tr>";
                }

                var posicion = listaPosiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.Id).FirstOrDefault();
                var localidad = repositorio.Obtener<Localidad>(x => x.ProvinciaId == posicion.ProvinciaId);
                var centro = repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == posicion.Centro.CodigoSap);
                //var centroPlanta = repositorio.Obtener<TablaSap>(x => x.CodigoSap == posicion.Centro.CodigoSap);
                var lugarEntrega = $"{posicion.NombreEntrega}, {posicion.CalleEntrega} - ({posicion.CpEntrega}) {localidad?.Nombre ?? ""} - {posicion.Provincia?.Nombre ?? ""}";

                xHtml = string.Format(xHtml, stylesHtml,
                    peticion.Id,
                    datosProveedor.cabeceras?.FirstOrDefault().cuit.Substring(2, 8),
                    datosProveedor.cabeceras?.FirstOrDefault().descripcion,
                    datosProveedor.cabeceras?.FirstOrDefault().calleFiscal,
                    $"({datosProveedor.cabeceras?.FirstOrDefault().cpFiscal}) {datosProveedor.cabeceras?.FirstOrDefault().locaFiscal}",
                    datosProveedor.cabeceras?.FirstOrDefault().provFiscal,
                    "Argentina",
                    peticion.PlazoDeOferta.ToString("dd.MM.yyyy"),
                    listaPosiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value.ToString("dd.MM.yyyy"),
                    lugarEntrega,
                    peticion.FechaCreacion.ToString("dd.MM.yyyy"),
                    "San Lorenzo",
                    centro.CodigoSap,
                    peticion.Usuario.UsuarioSap,
                    posiciones
                    );

                return xHtml;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnviarMailPeticionDeOferta(PeticionDeOferta peticion, List<PeticionDeOfertaUsuario> usuarios, List<PeticionDeOfertaUsuarioAdicional> usuariosAdicionales)
        {
            var archs = ObtenerArchivosPeticionDeOferta(peticion);
            var solicitanteYComprador = new List<string> { peticion.Usuario.Mail };
            if (!string.IsNullOrEmpty(peticion.Solp?.UsuarioCreacion?.Mail))
            {
                solicitanteYComprador.Add(peticion.Solp.UsuarioCreacion.Mail);
            }

            if (!string.IsNullOrEmpty(peticion.Solp.Pliego.Email))
            {
                //Mail al solicitante
                solicitanteYComprador.Add(peticion.Solp.Pliego.Email);
            }

            var proveedores = new List<string>();
            var asunto = $"MOA - Pedido de Oferta {peticion.Id}: {peticion.Solp.Pliego.NombreObra}";
            if (peticion.Solp.Adicional == true) asunto += $" - con Adicional OC: {peticion.Solp.NroOrdenDeCompraAdicional}";

            List<UsuarioDto> usuariosDto = new List<UsuarioDto>();
            foreach (var item in usuarios)
            {
                usuariosDto.Add(new UsuarioDto
                {
                    Mail = item.Usuario.Mail,
                    RazonSocial = item.Usuario.ObtenerRazonSocial(),
                    CUIT = item.Usuario.CUITRegistro,
                    CodigoProveedor = item.Usuario.ObtenerCodigoProveedor()
                });
            }
            foreach (var item in usuariosAdicionales)
            {
                usuariosDto.Add(new UsuarioDto
                {
                    Mail = item.Usuario.Mail,
                    RazonSocial = item.Usuario.ObtenerRazonSocial(),
                    CUIT = item.Usuario.CUITRegistro,
                    CodigoProveedor = item.Usuario.ObtenerCodigoProveedor()
                });
            }

            foreach (var prov in usuariosDto.GroupBy(a => a.CUIT))
            {

                proveedores.AddRange(prov.Select(a => a.RazonSocial + " - " + a.Mail).ToList());

                var enviarA = prov.Select(a => a.Mail).ToList();
                if (peticion.Posiciones.Select(x => x.SolpPosicion).Where(x => x.TipoPosicion_Id != null).FirstOrDefault().TipoPosicion.Codigo == "MATERIALES")
                {
                    var pdf = GenerarPDFPeticionDeOferta(peticion, prov.First().CodigoProveedor);
                    if (archs.ContainsKey("Peticion de Oferta.pdf"))
                        archs.Remove("Peticion de Oferta.pdf");
                    archs.Add("Peticion de Oferta.pdf", pdf);
                }
                var cuerpoProv = CuerpoMailPeticionDeOferta(peticion, true);
                emailService.EnviarMail(enviarA, asunto, "", null, cuerpoProv, null, null, null, null, archs);
            }

            var cuerpo = CuerpoMailPeticionDeOferta(peticion, false, proveedores);
            emailService.EnviarMail(solicitanteYComprador, asunto, "", null, cuerpo, null, null, null, null, null);
        }

        private Dictionary<string, byte[]> ObtenerArchivosPeticionDeOferta(PeticionDeOferta peticion)
        {
            var archs = new Dictionary<string, byte[]>();
            foreach (var p in peticion.Archivos)
            {
                WebClient wc = new WebClient();
                byte[] b = wc.DownloadData(p.Archivo.Ruta);
                archs.Add(p.Archivo.ObtenerNombre(), b);
            }
            return archs;
        }

        private AlternateView CuerpoMailPeticionDeOferta(PeticionDeOferta peticion, bool esProveedor, List<string> proveedores = null)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            var configuracion = repositorio.Obtener<Configuracion>(con => con.Code == "PliegoDeGeneralidades");
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = $"En el presente mail se informa la nueva PO {peticion.Id} generada con Molinos Agro S.A <br />";

            if (!esProveedor)
            {
                htmlBody = $"En el presente mail se informa la nueva PO {peticion.Id} que se envió a los siguientes proveedores: <br />";
                foreach (var proveedor in proveedores)
                {
                    htmlBody += proveedor + "<br />";
                }
            }
            if (!string.IsNullOrEmpty(peticion.Observaciones))
            {
                string observacionesFormatted = peticion.Observaciones.Replace("\n", "<br />");

                htmlBody += $"<br />Observaciones: {observacionesFormatted} <br /><br />";
            }

            if (esProveedor)
            {
                var esDeServicioSapMantConPliego = (peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.ReposicionAutomatica || peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.Sap) && peticion.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() == "SERVICIO" && peticion.Solp.EstadoDocumento.Codigo == "CREADO";
                var esDeServicioWebConPliego = peticion.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() == "SERVICIO" && (peticion.Solp.TipoSolpSap == (int?)TipoSolpSap.Web) && peticion.Solp.TipoSolp.Codigo != "SIN_PLIEGO";

                if (esDeServicioWebConPliego || esDeServicioSapMantConPliego)
                {
                    var downloadLinkUrl = ConfigurationManager.AppSettings["ida:RedirectUri"] + "/api/compras/DescargarPliegoDesdeLink?solpId=" + peticion.Solp.Id + "&token=" + peticion.Solp.EmailLinkToken;

                    htmlBody += "<p" +
                               "style = 'line-height: 24px; font-size: 16px; margin: 0;'" +
                               "align = 'center' >" +
                               " Para descargar el legajo, haga  " +
                               $"<a href = '{downloadLinkUrl}' download rel='noopener noreferrer'>" +
                               "click aquí" +
                               "</a></p> <br />";
                }
                if (peticion.AdjuntoPliego == true)
                {
                    htmlBody += "<p" +
                               "style = 'line-height: 24px; font-size: 16px; margin: 0;'" +
                               "align = 'center' >" +
                               "Para descargar el pliego de generalidades, haga " +
                               $"<a href = '{configuracion.Value}' download rel='noopener noreferrer'>" +
                               "click aquí" +
                               "</a></p> <br />";
                }
            }

            htmlBody += "<br />En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public Pdf GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario)
        {
            var po = repositorio.Obtener<PeticionDeOfertaUsuario>(idPeticionDeOfertaUsuario);
            var pdf = GenerarPDFPeticionDeOferta(po.PeticionDeOferta, po.Usuario.ObtenerProveedor().CodigoProveedor);
            return new Pdf { Data = pdf, Name = "PO" + po.Usuario.ObtenerProveedor().CUIT + ".pdf" };
        }

        private byte[] GenerarPDFOrdenCompra(Adjudicacion adjudicacion, string codigoProveedor)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4, 10f, 10f, 10f, 100f))
                    {
                        string templateFilePath = httpContextService.GetDirectory("Templates/OrdenCompraTemplate.html");

                        var templateString = System.IO.File.ReadAllText(templateFilePath);

                        var xHtml = templateString;
                        var adjudicacionDto = ObtenerAdjudicacion(adjudicacion.NumeroOrdenDeCompra);
                        xHtml = CompletarHtmlOC(xHtml, adjudicacionDto, codigoProveedor, adjudicacion);

                        var PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                        document.Open();

                        PdfFooter PageEventHandler = new PdfFooter();
                        PdfWriter.PageEvent = PageEventHandler;

                        var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                        tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                        tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

                        var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();

                        var htmlPipelineContext = new HtmlPipelineContext(null);
                        htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                        var pdfWriterPipeline = new PdfWriterPipeline(document, PdfWriter);

                        // get an ICssResolver and add the custom CSS
                        var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                        var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                        hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors

                        var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(document, PdfWriter));
                        var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                        var worker = new XMLWorker(pipeline, true);
                        var charset = Encoding.UTF8;
                        var xmlParser = new XMLParser(true, worker, charset);
                        xmlParser.Parse(new StringReader(xHtml));
                        document.Close();
                        byte[] bytes = stream.ToArray();
                        stream.Close();
                        return bytes;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string CompletarHtmlOC(string xHtml, AdjudicacionDto adjudicacion, string codigoProveedor, Adjudicacion adjudicacionEntidad)
        {
            var stylesHtml = @"<style>h1{color:#000;font-family:'Times New Roman',serif;font-style:italic;font-weight:700;text-decoration:none;font-size:12px}
                            .s1{color:#000;font-family:'Times New Roman',serif;font-style:italic;font-weight:400;text-decoration:none;font-size:10px}
                            .s2{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:700;text-decoration:none;font-size:8px}
                            .s3{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:9px}
                            h2{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:700;text-decoration:none;font-size:8px}
                            p{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:7px;margin:0}
                            table,tbody{vertical-align:top;overflow:visible}
                            .peticion{font-family:'Times New Roman',serif;font-style:italic;font-weight:700;text-decoration:none;font-size:10px;border:.1px solid #000;border-collapse:collapse}
                            .s4{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:700;text-decoration:none;font-size:9px}
                            .s5{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:8px}
                            .s6{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:700;text-decoration:none;font-size:18px}
                            .s7{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:9px}
                            .s8{color:#000;font-family:Arial,sans-serif;font-style:italic;font-weight:400;text-decoration:none;font-size:9px}
                            .s9{color:#000;font-family:Arial,sans-serif;font-style:normal;font-weight:400;text-decoration:none;font-size:9px}
                            table,tbody{vertical-align:top;overflow:visible}.border{border:.1px solid #000;border-collapse:collapse}
                            .s6{color:#000;font-family:Arial,sans-serif;font-style:italic;text-decoration:none;font-size:7px}
                            .cls_003{font-family:Arial,serif;font-size:12.1px;color:#fff;font-weight:700;font-style:normal;text-decoration:none;background-color:#000;text-align:center;top:-59px;position:relative;left:-1px;width:102%}
                            .cls_002{font-family:Arial,serif;font-size:14.1px;color:#000;font-weight:700;font-style:italic;text-decoration:none}
                            .noborder{border-collapse:collapse;border:1px solid #fff}
                            .cls_005{font-family:Arial,serif;font-size:8.1px;color:#000;font-weight:700;font-style:normal;text-decoration:none}
                            .cls_006{font-family:Arial,serif;font-size:8px;color:#000;font-weight:400;font-style:normal;text-decoration:none}
                            .cls_008{font-family:Arial,serif;font-size:10px;color:#000;font-weight:400;font-style:normal;text-decoration:none}
                            .cls_009{font-family:Arial,serif;font-size:11.1px;color:#000;font-weight:700;font-style:normal;text-decoration:none;text-align:center}
                            .cls_011{font-family:Courier New,serif;font-size:10.1px;color:#000;font-weight:400;font-style:normal;text-decoration:none}
                            .espacio{height:10px;display:block}.w33{width:30%;display:inline-block}.cls_012{font-family:Arial,serif;font-size:6px;text-align:justify}
                            .ft116{position:absolute;top:699px;white-space:nowrap}
                            .ft11{position:absolute;top:712px;white-space:nowrap}
                            .ft15{position:absolute;top:709px;white-space:nowrap}</style>";

            var datosProveedor = new VendedorDetalleWSMOAResponse() { cabeceras = null };
            try
            {
                datosProveedor = vendedorService.GetDatosFiscales(codigoProveedor, codigoProveedor);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            var head = "";
            var posiciones = "";
            try
            {
                var tipoPosicion = adjudicacion.TipoPosicionCodigo;

                head = "<tr style='text-align: center'>   " +
                   "<th class='s4'>POS</th>" +
                   "<th class='s4'>MATERIAL</th>" +
                   "<th class='s4'>DENOMINACIÓN</th> " +
                   "<th class='s4'>CANTIDAD<br /> PEDIDO</th> " +
                   "<th class='s4'>UNIDAD</th>" +
                   "<th class='s4'>FECHA ENTREGA</th>" +
                   "<th class='s4'>PRECIO POR UNIDAD</th>" +
                   "<th class='s4'>VALOR NETO</th></tr>";

                if (tipoPosicion == "MATERIALES")
                {
                    foreach (var posi in adjudicacion.AdjudicacionPosiciones)
                    {
                        //var item = peti.Posicion;
                        //var cotizacionPosicion = adjudicacion.Cotizacion.CotizacionPosiciones.Where(p => p.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == peti.SolpPosicion_Id).First();

                        posiciones +=
                        $"<tr class='border'> <td style='font-size: 8px;'>{posi.Indice} </td> " +
                        $"<td style='font-size: 8px;'> {(!string.IsNullOrEmpty(posi.MaterialComprasCodigo) ? posi.MaterialComprasCodigo : "")} </td>" +
                        $"<td style='font-size: 8px;'> {(!string.IsNullOrEmpty(posi.MaterialComprasDescripcion) ? posi.MaterialComprasDescripcion : "")} </td>" +
                        $"<td style='font-size: 8px;'>{posi.Cantidad}</td>" +
                        $"<td style='font-size: 8px;'>{posi.UnidadDescripcion}</td>" +
                        //$"<td style='font-size: 8px;'>{adjudicacion.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value.ToString("dd.MM.yyyy")}</td>" +
                        $"<td style='font-size: 8px;'>{posi.FechaEntregaServicio}</td>" +
                        $"<td style='font-size: 8px;'>{posi.PrecioUnidad.Value.ToString("N2")} {posi.MonedaCodigo} / {posi.UnidadDescripcion}</td>" +
                        $"<td style='font-size: 8px;'>{(posi.Cantidad * posi.PrecioUnidad.Value).ToString("N2")} {posi.MonedaCodigo}</td></tr>";
                        posiciones += $"<tr><td colspan='7' style='font-size: 8px; text-align: justify'>{(!string.IsNullOrEmpty(posi.MaterialTextoAmpliado) ? posi.MaterialTextoAmpliado : "")}</td></tr>";
                    }
                }
                else
                {
                    head = "<tr style='text-align: center'>   " +
                    "<th class='s4'>POS</th>" +
                    "<th class='s4'>DENOMINACIÓN</th> " +
                    "<th class='s4'>CANTIDAD<br /> PEDIDO</th> " +
                    "<th class='s4'>UNIDAD</th>" +
                    "<th class='s4'>FECHA ENTREGA</th>" +
                    "<th class='s4'>PRECIO POR UNIDAD</th>" +
                    "<th class='s4'>VALOR NETO</th></tr>";

                    foreach (var solpPosicion in adjudicacion.AdjudicacionPosiciones)
                    {
                        //var solpPosicion = adjudicacion.Solp.Posiciones.Where(c => c.Id == item.SolpPosicion_Id).First();
                        //var cotizacionPosicion = adjudicacion.Cotizacion.CotizacionPosiciones.Where(p => p.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == solpPosicion.Id).First();
                        //var montoTotalPosicion = cotizacionPosicion.CotizacionSubPosiciones.Sum(s => s.Precio * s.Cantidad);

                        posiciones +=
                        $"<tr class='border'> <td style='font-size: 8px;'>{solpPosicion.Indice} </td> " +
                        $"<td style='font-size: 8px;'> {(solpPosicion.Tarea != null ? solpPosicion.Tarea : "")} </td>" +
                        $"<td style='font-size: 8px;'> 1 </td>" +
                        $"<td style='font-size: 8px;'> </td>" +
                        $"<td style='font-size: 8px;'>{solpPosicion.FechaEntregaServicio}</td>" +
                        $"<td style='font-size: 8px;'>{solpPosicion.PrecioTotal.Value.ToString("N2")} {solpPosicion.MonedaCodigo} / 001</td>" +
                        $"<td style='font-size: 8px;'>{solpPosicion.PrecioTotal.Value.ToString("N2")} {solpPosicion.MonedaCodigo}</td></tr>" +
                        $"<tr><td colspan='4' style='font-size: 10px; text-align: end'><strong>La posición contiene los siguientes servicios:</strong></td></tr>";

                        foreach (var subPos in solpPosicion.SubposicionesCompras)
                        {
                            posiciones +=
                            $"<tr class='border'>" +
                            $"<td style='font-size: 8px;'>{subPos.Numero * 10} </td>" +
                            $"<td style='font-size: 8px;'>&nbsp;</td>" +
                            $"<td style='font-size: 8px;'>&nbsp;</td>" +
                            $"<td colspan='4' style='font-size: 8px;'>{subPos.Tarea}</td>" +
                            $"</tr>" +
                            $"<tr>" +
                            $"<td style='font-size: 8px;'>&nbsp;</td>" +
                            $"<td style='font-size: 8px;'>{subPos.Cantidad} {subPos.UnidadComprasDescripcion}</td>" +
                            $"<td style='font-size: 8px;'>{subPos.PrecioBruto?.ToString("N2")}</td>" +
                            $"<td style='font-size: 8px;'>{(subPos.PrecioBruto * subPos.Cantidad).Value.ToString("N2")}</td>" +
                            $"</tr>";
                        }
                    }
                }

                string textos = "";

                if (!string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ||
                    !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega) ||
                    !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ||
                    !string.IsNullOrEmpty(adjudicacion.Garantias))
                {
                    textos += "<tr class='border'>";

                    if (!string.IsNullOrEmpty(adjudicacion.TextoDeCabecera))
                    {
                        textos += $"<td style='font-size: 8px; text-align: justify'><strong>Texto de cabecera</strong><br/><br/> {adjudicacion.TextoDeCabecera}</td></tr>";
                    }

                    if (!string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega))
                    {
                        textos += $"<tr class='border'><td style='font-size: 8px; text-align: justify'><strong>Condiciones de entrega</strong><br/><br/> {adjudicacion.CondicionesDeEntrega}</td></tr>";
                    }

                    if (!string.IsNullOrEmpty(adjudicacion.CondicionesDePago))
                    {
                        textos += $"<tr class='border'><td style='font-size: 8px; text-align: justify'><strong>Condiciones de pago</strong><br/><br/> {adjudicacion.CondicionesDePago}</td></tr>";
                    }

                    if (!string.IsNullOrEmpty(adjudicacion.Garantias))
                    {
                        textos += $"<tr class='border'><td style='font-size: 8px; text-align: justify'><strong>Garantias</strong><br/><br/> {adjudicacion.Garantias}</td></tr>";
                    }
                }
                else
                {
                    textos += $"<tr class='border'><td style='font-size: 8px; text-align: justify'>&nbsp;<br/><br/></td></tr>";
                }

                var posicion = adjudicacion.AdjudicacionPosiciones.OrderByDescending(x => x.Id).FirstOrDefault();
                var posicionEntidad = adjudicacionEntidad.Solp.Posiciones.OrderByDescending(x => x.Id).FirstOrDefault();

                //Antes de la provincia deberia ir la localidad pero no la tenemos
                var lugarEntrega = $"{adjudicacion.Centro}, {adjudicacion.CalleEntrega} - ({adjudicacion.CodigoPostal}) - {posicionEntidad.Provincia?.Nombre ?? ""}";

                xHtml = string.Format(xHtml, stylesHtml,
                    adjudicacion.NumeroOrdenDeCompra,
                    datosProveedor.cabeceras?.FirstOrDefault().cuit.Substring(2, 8),
                    datosProveedor.cabeceras?.FirstOrDefault().descripcion,
                    datosProveedor.cabeceras?.FirstOrDefault().calleFiscal,
                    $"({datosProveedor.cabeceras?.FirstOrDefault().cpFiscal}) {datosProveedor.cabeceras?.FirstOrDefault().locaFiscal}",
                    datosProveedor.cabeceras?.FirstOrDefault().provFiscal,
                    "Argentina",
                    posicion.PlazoDeOferta?.ToString("dd.MM.yyyy"),
                    posicion.FechaEntregaServicio?.ToString("dd.MM.yyyy"),
                    lugarEntrega,
                    adjudicacion.FechaCreacion.ToString("dd.MM.yyyy"),
                    "San Lorenzo",
                    posicion.CentroComprasCodigo,
                    adjudicacionEntidad.Usuario.UsuarioSap,
                    posiciones,
                    posicion.MonedaCodigo,
                    posicion.MonedaDescripcion,
                    adjudicacion.PrecioFinal.ToString("N2"),
                    textos,
                    head
                    );

                return xHtml;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void EnviarMailOrdenCompra(Adjudicacion adjudicacion, string mensaje = "")
        {
            try
            {
                Log.Info($"EnviarMailOrdenCompra Adjudicacion_Id: {adjudicacion.Id}");
                Log.Info($"Nueva OC creada con número {adjudicacion.NumeroOrdenDeCompra} y fecha {DateTime.Now}");
                Log.Info($"Copia mail comprador: {adjudicacion.Usuario.Mail}");
                Log.Info($"Mail al proveedor adjudicado: {adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail}");

                var copia = new List<string> { adjudicacion.Usuario.Mail };

                if (!string.IsNullOrEmpty(adjudicacion.Solp.Pliego.Email))
                {
                    copia.Add(adjudicacion.Solp.Pliego.Email);
                    Log.Info($"Copia mail solicitante paso 1: {adjudicacion.Solp.Pliego.Email}");
                }

                if (!string.IsNullOrEmpty(adjudicacion.Solp?.UsuarioCreacion?.Mail))
                {
                    copia.Add(adjudicacion.Solp.UsuarioCreacion.Mail);
                    Log.Info($"Copia mail solicitante: {adjudicacion.Solp.UsuarioCreacion.Mail}");
                }

                var asunto = $"Nueva OC creada - {adjudicacion.NumeroOrdenDeCompra} - {adjudicacion.Usuario.ObtenerRazonSocial()}";

                var enviarA = new List<string> { adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail };
                var adicionales = adjudicacion.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.UsuariosAdicionales;
                enviarA.AddRange(adicionales.Where(a => a.Usuario.CUITRegistro == adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.CUITRegistro).Select(a => a.Usuario.Mail).ToList());

                var pdf = obtenerPDFOrdenCompraConsumerMOA.Request(adjudicacion.NumeroOrdenDeCompra);

                emailService.EnviarMail(enviarA, asunto, "", copia, CuerpoMailOrdenCompra(adjudicacion, mensaje), pdf, $"Orden de Compra {adjudicacion.NumeroOrdenDeCompra}.pdf");
            }
            catch (Exception e)
            {
                Log.Info($"Error en EnviarMailOrdenCompra. Adjudicacion_Id {adjudicacion.Id} - NumeroOrdenDeCompra: {adjudicacion.NumeroOrdenDeCompra}");
                Log.Error(e);
            }
        }

        private AlternateView CuerpoMailOrdenCompra(Adjudicacion adjudicacion, string mensaje)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la nueva OC {adjudicacion.NumeroOrdenDeCompra} generada con Molinos Agro S.A. <br />";
            htmlBody += mensaje + "<br/>";

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public PeticionDeOfertaDto ObtenerPeticionDeOfertaParaCircular(int peticionId)
        {

            var usuarios = new List<PeticionDeOfertaUsarioDto>();
            var peticion = new PeticionDeOfertaDto();
            var peticionEntidad = repositorio.Obtener<PeticionDeOferta>(peticionId);
            var peticionDeOfertaUsuarios_Id = peticionEntidad.Usuarios.Select(u => u.Id).ToList();
            var cotizaciones = repositorio.Listar<Cotizacion>(x => peticionDeOfertaUsuarios_Id.Contains(x.PeticionDeOfertaUsuario_Id));
            var legajos = new List<LegajoDto>();

            foreach (var u in peticionEntidad.Usuarios)
            {
                var cotizacion = cotizaciones.Where(c => c.PeticionDeOfertaUsuario_Id == u.Id).Select(c => new CotizacionDto
                {
                    TieneObservacionTecnica = !string.IsNullOrEmpty(c.ObservacionTecnica),
                    TieneObservacionEconomica = !string.IsNullOrEmpty(c.ObservacionEconomica),
                    ObservacionTecnica = c.ObservacionTecnica,
                    ObservacionEconomica = c.ObservacionEconomica,
                    Id = c.Id,
                    CotizacionEstado_Id = c.CotizacionEstado_Id,
                    RespetaMateriales = c.RespetaMateriales,
                    CotizacionEstadoDescripcion = c.CotizacionEstado.Descripcion,
                    PorcentajeDeHoras = c.PorcentajeDeHoras,

                    Archivos = c.Archivos.Select(archivo => new ArchivoDto
                    {
                        Id = archivo.Id,
                        FileKey = archivo.FileKey,
                        Ruta = archivo.ObtenerNombre(archivo.Ruta),
                    }).ToList(),
                    CotizacionesHoras = c.CotizacionesHoras.Select(x => new CotizacionHorasDto
                    {
                        CantidadPersonas = x.CantidadPersonas,
                        Categoria = x.Categoria,
                        Cotizacion_Id = x.Cotizacion_Id,
                        Gremio = x.Gremio,
                        HorasExtras = x.HorasExtras,
                        HorasNocturnas = x.HorasNocturnas,
                        HorasNormales = x.HorasNormales,
                        Id = x.Id
                    }).ToList()
                }).FirstOrDefault();

                var usuario = new PeticionDeOfertaUsarioDto()
                {
                    RazonSocial = u.Usuario.ObtenerRazonSocial(),
                    UsuarioId = u.Usuario_Id,
                    Id = u.Id,
                    CUIT = u.Usuario.ObtenerProveedor().CUIT,
                    Mail = u.Usuario.Mail,
                    Cotizacion = cotizacion,
                    PropuestaTecnicaAprobada = u.PropuestaTecnicaAprobada,
                    RealizoVisita = u.RealizoVisita,
                    EstaHabilitado = u.Usuario.Habilitado,
                    ValidacionCircularSolicitante = ValidacionCircularSolicitante(u, cotizacion),
                    ObservacionNoCumple = u.ObservacionNoCumple,
                };
                usuarios.Add(usuario);
            }

            peticion = ObtenerPeticionDeOfertaDto(peticionId);

            peticion.Usuarios = usuarios;
            peticion.UsuariosAdicionales = peticionEntidad.UsuariosAdicionales.Select(a => new PeticionDeOfertaUsuarioAdicionalDto(a)).ToList();
            peticion.TipoPosicionCodigo = peticionEntidad.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault();
            peticion.Id = peticionEntidad.Id;
            peticion.PlazoDeOfertaEstado = peticionEntidad.PlazoDeOferta > DateTime.Now.Date ? "Abierto" : "Cerrado";
            peticion.TieneVisitaObraBool = peticionEntidad.Solp.Pliego.TieneVisitaObra ?? false;
            peticion.TieneVisitaObraMasiva = peticionEntidad.Solp.Pliego.TieneVisitaObraMasiva ?? false;


            var fechaEntrega = peticionEntidad.Posiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).FirstOrDefault()?.FechaEntregaServicio;
            peticion.FechaEntregaFormateado = fechaEntrega != null ? fechaEntrega.Value.ToString("yyyy-MM-dd") : "";
            return peticion;
        }

        private PeticionDeOfertaDto ObtenerPeticionDeOfertaDto(int peticionId)
        {

            var peticionDeOfertaDto = repositorio.Obtener<PeticionDeOferta, PeticionDeOfertaDto>(po => po.Id == peticionId, po =>
                        new PeticionDeOfertaDto()
                        {
                            Id = po.Id,
                            Solp_Id = po.Solp_Id,
                            FechaCreacion = po.FechaCreacion,
                            UsuarioCreador_Id = po.UsuarioCreador_Id,
                            PlazoDeOfertaOriginal = po.PlazoDeOferta,
                            PlazoDeOfertaCircular = po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                    .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                    .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.PlazoDeOferta,
                            FechaCircular = po.Usuarios.GroupBy(p => p).SelectMany(p => p.Key.Circulares)
                                                    .Where(p => p.Circular.RequiereCambioDeFechas == true && p.Circular.PlazoDeOferta.HasValue)
                                                    .OrderByDescending(p => p.Circular.Id).FirstOrDefault().Circular.FechaCreacion,
                            PlazoDeOfertaCierre = po.Cierres.Any() ? po.Cierres.OrderByDescending(p => p.Fecha).FirstOrDefault().Fecha : (DateTime?)null,
                            Observaciones = po.Observaciones,
                            TipoPosicionCodigo = po.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault(),
                            TieneVisitaObraBool = po.Solp.Pliego.TieneVisitaObra ?? false,
                            TieneVisitaObraMasiva = po.Solp.Pliego.TieneVisitaObraMasiva ?? false,
                            RevisionTecnicaId = po.RevisionTecnica_Id
                        });

            var revisionFinalizada = repositorio.Obtener<PeticionDeOfertaRevisionTecnica>(x => x.Id == peticionDeOfertaDto.RevisionTecnicaId);

            peticionDeOfertaDto.RevisionFinalizada = revisionFinalizada == null ? false : revisionFinalizada.Finalizada;

            return peticionDeOfertaDto;
        }

        private static bool ValidacionCircularSolicitante(PeticionDeOfertaUsuario u, CotizacionDto cotizacion)
        {
            if (cotizacion == null || cotizacion.CotizacionEstado_Id != (int)CotizacionEstadoEnum.Cotizado)
            {
                return false;
            }
            if (u.PeticionDeOferta.Solp.Posiciones.First().TipoPosicion.Codigo == "MATERIALES")
            {
                return cotizacion.RespetaMateriales == true || u.PropuestaTecnicaAprobada == true;
            }
            else
            {
                return u.PropuestaTecnicaAprobada == true &&
                    (u.RealizoVisita == true || (u.PeticionDeOferta.Solp.Pliego.TieneVisitaObra != true && u.PeticionDeOferta.Solp.Pliego.TieneVisitaObraMasiva != true));
            }
        }

        public RespuestaGuardarSOLP GrabarCircular(CircularDto circularDto, HttpFileCollectionBase adjuntos)
        {
            try
            {
                ValidarCircular(circularDto);

                var peticion = repositorio.Obtener<PeticionDeOferta>(circularDto.PeticionDeOferta_Id);
                var usuario = repositorio.Obtener<Usuario>(circularDto.UsuarioId);
                var rolUsuario = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "COMPRADOR" : "SOLICITANTE";
                var circular = new Circular()
                {
                    UsuarioCreador_Id = circularDto.UsuarioId,
                    Usuario = usuario,
                    FechaCreacion = DateTime.Now,
                    Observaciones = circularDto.Observacion,
                    PlazoDeOferta = circularDto.PlazoDeOferta?.ToLocalTime(),
                    FechaDeEntrega = circularDto.FechaEntrega,
                    RequiereCambioDeFechas = circularDto.RequiereCambioDeFecha,
                    PeticionDeOfertaUsuarios = peticion.Usuarios.Where(x => circularDto.UsuarioIds.Contains(x.Usuario_Id))
                    .Select(a => new CircularPeticionDeOfertaUsuario
                    {
                        PeticionDeOfertaUsuario_Id = a.Id
                    }).ToList()
                };

                foreach (var proveedor in peticion.Usuarios.Where(x => circularDto.UsuarioIds.Contains(x.Usuario_Id)))
                {
                    if (rolUsuario == "SOLICITANTE")
                    {
                        proveedor.PropuestaTecnicaAprobada = null;
                        proveedor.PropuestaTecnicaFecha = null;
                        proveedor.PropuestaTecnicaUsuario_Id = null;
                        proveedor.ObservacionNoCumple = "";
                    }
                    if (proveedor.Cotizaciones != null && proveedor.Cotizaciones.Count > 0 && proveedor.Cotizaciones.First().CotizacionEstado_Id == 1)
                    {
                        proveedor.Cotizaciones.First().CotizacionEstado_Id = 2;
                    }
                }

                circular = repositorio.Agregar(circular);
                repositorio.GuardarCambios();

                if (adjuntos != null && adjuntos.Count > 0)
                {
                    GuardarArchivosCircular(circular, adjuntos);
                }
                repositorio.GuardarCambios();

                var solp = new SolpDto
                {
                    Id = circular.Id
                };

                var respuestaGuardarSOLP = new RespuestaGuardarSOLP
                {
                    Solp = solp
                };
                respuestaGuardarSOLP.IdEntidad = circular.PeticionDeOfertaUsuarios.FirstOrDefault().PeticionDeOfertaUsuario.PeticionDeOferta_Id;
                try
                {

                    EnviarMailCircular(circular);

                }
                catch (Exception e)
                {
                    Logger.Log.Error(new Exception($"Error al enviar mail GrabarCircular en circular: " + circular.Id));
                    Logger.Log.Error(e);
                }

                return respuestaGuardarSOLP;

            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        private void ValidarCircular(CircularDto circularDto)
        {
            if (string.IsNullOrEmpty(circularDto.Observacion))
            {
                throw new ValidationCustomException("El campo Observación es obligatorio");
            }
            if (circularDto.RequiereCambioDeFecha == true && circularDto.FechaEntrega == null)
            {
                throw new ValidationCustomException("Debe completar la Fecha de entrega");
            }
            if (circularDto.RequiereCambioDeFecha == true && circularDto.PlazoDeOferta == null)
            {
                throw new ValidationCustomException("Debe completar el Plazo de oferta");
            }
            if (circularDto.UsuarioIds == null || circularDto.UsuarioIds.Count == 0)
            {
                throw new ValidationCustomException("Debe seleccionar al menos un proveedor");
            }
        }

        private void GuardarArchivosCircular(Circular circular, HttpFileCollectionBase files)
        {
            var ruta = ObtenerRutaArchivos(circular.Id, FileKeys.Circular);


            var filesEspecificaciones = files.GetMultiple("fileCircular");
            for (int i = 0; i < filesEspecificaciones.Count; i++)
            {
                var file = filesEspecificaciones[i];
                var rutaArchivo = string.Concat(ruta, "/", Path.GetFileName(file.FileName));
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                circular.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.PeticionDeOferta,
                    Ruta = rutaArchivoRename

                });

                file.SaveAs(rutaArchivoRename);
            }
        }

        private void EnviarMailCircular(Circular circular)
        {
            var archs = ObtenerArchivosCircular(circular);
            var asunto = "";
            var copia = new List<string> { circular.Usuario.Mail };
            if (!string.IsNullOrEmpty(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Solp?.UsuarioCreacion?.Mail))
            {
                copia.Add(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Solp?.UsuarioCreacion?.Mail);
            }

            if (!string.IsNullOrEmpty(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Solp.Pliego.Email))
            {
                //Mail al solicitante
                copia.Add(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Solp.Pliego.Email);
            }

            if (!string.IsNullOrEmpty(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Usuario?.Mail))
            {
                copia.Add(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Usuario?.Mail);
            }
            var peticionDeOferta_Id = circular.PeticionDeOfertaUsuarios.First().PeticionDeOfertaUsuario.PeticionDeOferta_Id;
            var adicionales = repositorio.Listar<PeticionDeOfertaUsuarioAdicional>(p => p.PeticionDeOferta_Id == peticionDeOferta_Id);
            foreach (var prov in circular.PeticionDeOfertaUsuarios)
            {
                var enviarA = new List<string> { prov.PeticionDeOfertaUsuario.Usuario.Mail };
                enviarA.AddRange(adicionales.Where(a => a.Usuario.CUITRegistro == prov.PeticionDeOfertaUsuario.Usuario.CUITRegistro).Select(a => a.Usuario.Mail).ToList());
                asunto = $"Nueva circular con PO {prov.PeticionDeOfertaUsuario.PeticionDeOferta_Id} - {prov.PeticionDeOfertaUsuario.Usuario.ObtenerRazonSocial()}";

                emailService.EnviarMail(enviarA, asunto, "", copia, CuerpoMailCircular(prov), null, null, null, null, archs);
            }
        }

        private Dictionary<string, byte[]> ObtenerArchivosCircular(Circular circular)
        {
            var archs = new Dictionary<string, byte[]>();
            foreach (var p in circular.Archivos)
            {
                WebClient wc = new WebClient();
                byte[] b = wc.DownloadData(p.Ruta);
                archs.Add(p.ObtenerNombre(), b);
            }
            return archs;
        }

        private AlternateView CuerpoMailCircular(CircularPeticionDeOfertaUsuario circular)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";

            htmlBody += $"En el presente mail, se informa la nueva circular con PO {circular.PeticionDeOfertaUsuario.PeticionDeOferta_Id}" +
                $" para el proveedor {circular.PeticionDeOfertaUsuario.Usuario.ObtenerRazonSocial()}" +
                $" ({circular.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor().CUIT}) generada con Molinos Agro S.A <br />";
            if (!string.IsNullOrEmpty(circular.Circular.Observaciones))
            {
                htmlBody += $"Observaciones: {circular.Circular.Observaciones} <br />";
            }

            if (circular.Circular.RequiereCambioDeFechas == true)
            {
                htmlBody += $"Plazo de oferta actualizado: {circular.Circular.PlazoDeOferta} <br />";
                htmlBody += $"Fecha de entrega actualizada: {circular.Circular.FechaDeEntrega.Value.ToString("dd/MM/yyyy")} <br />";
            }

            htmlBody += "En caso de tener alguna consulta ingresar www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public RespuestaGuardarSOLP GrabarProveedoresEnPeticionDeOferta(List<int> usuariosId, int peticionId)
        {
            try
            {
                var solp = new SolpDto
                {
                    Id = peticionId
                };
                var respuestaGuardarSOLP = new RespuestaGuardarSOLP
                {
                    Solp = solp
                };
                var usuarios = repositorio.Listar<Usuario>();

                var peticion = repositorio.Obtener<PeticionDeOferta>(peticionId);

                List<PeticionDeOfertaUsuario> poUsuarios = new List<PeticionDeOfertaUsuario>();
                List<PeticionDeOfertaUsuarioAdicional> poUsuariosAdicionales = new List<PeticionDeOfertaUsuarioAdicional>();

                foreach (var proveedor in usuarios.Where(x => usuariosId.Contains(x.Id)).GroupBy(a => a.CUITRegistro))
                {
                    int ii = 0;
                    if (peticion.Usuarios.All(a => a.Usuario.CUITRegistro != proveedor.First().CUITRegistro))
                    {
                        var usuario = new PeticionDeOfertaUsuario
                        {
                            Usuario_Id = proveedor.First().Id,
                            Usuario = usuarios.Where(x => x.Id == proveedor.First().Id).FirstOrDefault(),
                            PeticionDeOferta_Id = peticion.Id
                        };
                        poUsuarios.Add(usuario);
                        peticion.Usuarios.Add(usuario);
                        ii = 1;
                    }

                    for (int i = ii; i < proveedor.Count(); i++)
                    {
                        var poAdicional = new PeticionDeOfertaUsuarioAdicional { Usuario_Id = proveedor.ToList()[i].Id, PeticionDeOferta_Id = peticion.Id };
                        poUsuariosAdicionales.Add(poAdicional);
                        peticion.UsuariosAdicionales.Add(poAdicional);
                    }
                }

                repositorio.GuardarCambios();
                respuestaGuardarSOLP.IdEntidad = peticion.Id;
                try
                {
                    EnviarMailPeticionDeOferta(peticion, poUsuarios, poUsuariosAdicionales);
                }
                catch (Exception e)
                {
                    Logger.Log.Error(new Exception($"Error al enviar mail GrabarProveedoresEnPeticionDeOferta en peticion: " + peticion.Id));
                    Logger.Log.Error(e);
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private RespuestaCrearOrdenDeCompra CrearOrdenDeCompra(Adjudicacion AdjudicacionEntity, bool creadoAutomatico = false)
        {
            var respuesta = new RespuestaCrearOrdenDeCompra();
            respuesta.Errores = new List<string>();
            CrearPedidoConsumerMOAResponse resultadoCrearPedido = new CrearPedidoConsumerMOAResponse();
            if (string.IsNullOrEmpty(AdjudicacionEntity.Usuario.OrganizacionDeCompra))
            {
                respuesta.Errores.Add("El usuario creador no tiene una organización de compra registrada en su perfil. Comunicarse con sistemas para agregarla.");
                return respuesta;
            }

            if (AdjudicacionEntity.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Adicional == true)
            {
                resultadoCrearPedido = modificarOrdenDeCompraConsumerMOA.Request(AdjudicacionEntity);
            }
            else
            {
                resultadoCrearPedido = crearPedidoConsumerMOA.Request(AdjudicacionEntity, creadoAutomatico);
            }

            respuesta.NumeroPedido = resultadoCrearPedido.NumeroPedido;
            respuesta.NumeroSolp = AdjudicacionEntity.Solp.NroSolp;
            foreach (var error in resultadoCrearPedido.Errores.Where(x => x.Tipo == "E"))
            {
                var mensaje = error.Mensaje.Trim();
                respuesta.Errores.Add(mensaje);
            }

            if (respuesta.Errores.Count == 0)
            {
                respuesta.Mensaje = "OK";
            }

            return respuesta;
        }

        public string DescargarAdjuntosCotizacion(int idCotizacion, string pathBase, bool desdeRevisionTecnica)
        {
            var cotizacion = repositorio.Obtener<Cotizacion>(idCotizacion);

            var zipFilename = $"Cotizacion-{cotizacion.Id}-{cotizacion.FechaCreacion:yyyyMMdd}.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    if (cotizacion.Archivos != null)
                    {
                        List<string> fileKey = new List<string>();
                        if (desdeRevisionTecnica)
                        {
                            if (cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones.First().TipoPosicion.Codigo == "MATERIALES")
                            {
                                if (cotizacion.RespetaMateriales == false)
                                {
                                    fileKey.Add("CotizacionRevisionEconomica");
                                }
                            }
                            else
                            {
                                fileKey.Add("CotizacionRevisionTecnica");
                            }
                        }
                        else
                        {
                            fileKey.Add("CotizacionRevisionEconomica");
                            fileKey.Add("CotizacionRevisionTecnica");
                        }

                        foreach (var archivoSubido in cotizacion.Archivos.Where(a => fileKey.Contains(a.FileKey)))
                        {
                            if (File.Exists(archivoSubido.Ruta))
                            {
                                string fileName = Path.GetFileName(archivoSubido.Ruta);
                                archivo.CreateEntryFromFile(archivoSubido.Ruta, fileName);
                            }
                        }
                    }
                }
            }

            return filePath;
        }

        public RespuestaGuardarSOLP GrabarRevisionTecnica(List<PeticionDeOfertaUsarioDto> peticionDeOfertaUsuarioDto, int usuarioId, bool finalizar, PeticionDeOfertaRevisionTecnicaDto revision)
        {
            RespuestaGuardarSOLP respuesta = new RespuestaGuardarSOLP();
            var ids = peticionDeOfertaUsuarioDto.Select(a => a.Id);
            var peticiones = repositorio.Listar<PeticionDeOfertaUsuario>(a => ids.Contains(a.Id));
            foreach (var peticion in peticiones)
            {
                var data = peticionDeOfertaUsuarioDto.Where(a => a.Id == peticion.Id).Single();

                if (peticion.RealizoVisita != data.RealizoVisita)
                {
                    peticion.RealizoVisita = data.RealizoVisita;
                    peticion.RealizoVisitaFecha = DateTime.Now;
                    peticion.RealizoVisitaUsuario_Id = usuarioId;

                }
                if (peticion.PropuestaTecnicaAprobada != data.PropuestaTecnicaAprobada)
                {
                    peticion.PropuestaTecnicaAprobada = data.PropuestaTecnicaAprobada;
                    peticion.PropuestaTecnicaFecha = DateTime.Now;
                    peticion.PropuestaTecnicaUsuario_Id = usuarioId;
                    peticion.ObservacionNoCumple = data.ObservacionNoCumple;
                }
                if (peticion.PropuestaTecnicaAprobada == false)
                {
                    peticion.ObservacionNoCumple = data.ObservacionNoCumple;
                }
            }

            if (peticiones.First().PeticionDeOferta.RevisionTecnica == null)
            {
                peticiones.First().PeticionDeOferta.RevisionTecnica = new PeticionDeOfertaRevisionTecnica
                {
                    Usuario_Id = usuarioId,
                    Fecha = DateTime.Now,
                    RecotizacionEconomica = revision.RecotizacionEconomica,
                    ModificacionSolp = revision.ModificacionSolp,
                    ObservacionRecotizacion = revision.ObservacionRecotizacion
                };
            }
            else
            {
                peticiones.First().PeticionDeOferta.RevisionTecnica.Usuario_Id = usuarioId;
                peticiones.First().PeticionDeOferta.RevisionTecnica.Fecha = DateTime.Now;
                peticiones.First().PeticionDeOferta.RevisionTecnica.RecotizacionEconomica = revision.RecotizacionEconomica;
                peticiones.First().PeticionDeOferta.RevisionTecnica.ModificacionSolp = revision.ModificacionSolp;
                peticiones.First().PeticionDeOferta.RevisionTecnica.ObservacionRecotizacion = revision.ObservacionRecotizacion;
            }

            if (finalizar)
            {
                peticiones.First().PeticionDeOferta.RevisionTecnica.Finalizada = true;

                var fechaActual = DateTime.Now;
                var plazoNoFinalizado = false;
                var todosCotizaron = peticiones.SelectMany(a => a.Cotizaciones).All(a => a.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado);

                var peticionesDeOferta = ObtenerPeticionDeOfertaDto(peticiones.First().PeticionDeOferta_Id);

                if (peticionesDeOferta.PlazoDeOferta > fechaActual)
                {
                    plazoNoFinalizado = true;
                }

                if (plazoNoFinalizado && todosCotizaron)
                {
                    var po = new PeticionDeOfertaCierre()
                    {
                        PeticionDeOferta_Id = peticiones.First().PeticionDeOferta_Id,
                        Usuario_Id = usuarioId,
                        Fecha = DateTime.Now,
                        Observacion = "Cierre de cotización automática por revisión técnica anticipada",
                    };
                    repositorio.Agregar(po);
                }


                peticiones.First().PeticionDeOferta.PlazoDeOferta = fechaActual;
            }
            repositorio.GuardarCambios();

            return respuesta;
        }

        public PeticionDeOfertaDto TraerCotizacion(int peticionId)
        {
            try
            {
                var peticionCotizacion = repositorio.ObtenerConsultaEscalar(new TraerCotizacionConsulta(peticionId));
                var listaHoras = new List<CotizacionHorasDto> {
                    new CotizacionHorasDto
                     {
                       Gremio = "UOCRA",
                       Categoria = "Oficial especializado",
                       Fila = true,
                       ConfigurarHora = false
                    },
                    new CotizacionHorasDto
                     {
                       Gremio = "UOCRA",
                       Categoria = "Oficial",
                       Fila = true,
                       ConfigurarHora = false

                    },
                     new CotizacionHorasDto
                     {
                       Gremio = "UOCRA",
                       Categoria = "Medio oficial",
                       Fila = true,
                       ConfigurarHora = false
                    },
                      new CotizacionHorasDto
                     {
                       Gremio = "UOCRA",
                       Categoria = "Ayudante",
                       Fila = true,
                       ConfigurarHora = false

                    },
                    //    new CotizacionHorasDto
                    //  {
                    //    Gremio = "UOCRA",
                    //    Categoria = "Horas taller (referenciales)",
                    //    Fila = true,
                    //    ConfigurarHora = false

                    // },
                          new CotizacionHorasDto
                     {
                       Gremio = "UOCRA",
                       Categoria = "SHyMA",
                       Fila = true,
                       ConfigurarHora = false

                    },
                };
                if (peticionCotizacion.CotizacionId != 0)
                {
                    var cotizacion = repositorio.Obtener<Cotizacion>(peticionCotizacion.CotizacionId);
                    peticionCotizacion.Cotizacion.ArchivosCotizacion = cotizacion.Archivos != null ? cotizacion.Archivos.Select(archivo => new ArchivoDto
                    {
                        Id = archivo.Id,
                        Nombre = Path.GetFileName(archivo.Ruta),
                        FileKey = archivo.FileKey,
                        Ruta = archivo.Ruta
                    }).ToList() : new List<ArchivoDto>();

                    foreach (var cot in listaHoras)
                    {
                        var hora = cotizacion.CotizacionesHoras.FirstOrDefault(x => x.Gremio == cot.Gremio && x.Categoria == cot.Categoria);
                        if (hora != null)
                        {
                            cot.HorasExtras = hora.HorasExtras;
                            cot.HorasNocturnas = hora.HorasNocturnas;
                            cot.HorasNormales = hora.HorasNormales;
                            cot.CantidadPersonas = hora.CantidadPersonas;
                            cot.ConfigurarHora = hora.ConfigurarHora ?? false;
                        }
                    }
                    listaHoras.AddRange(cotizacion.CotizacionesHoras.Where(x => x.Gremio != "UOCRA").Select(x => new CotizacionHorasDto
                    {
                        CantidadPersonas = x.CantidadPersonas,
                        Categoria = x.Categoria,
                        Cotizacion_Id = x.Cotizacion_Id,
                        Gremio = x.Gremio,
                        HorasExtras = x.HorasExtras,
                        HorasNocturnas = x.HorasNocturnas,
                        HorasNormales = x.HorasNormales,
                        ConfigurarHora = x.ConfigurarHora
                    }));

                }
                peticionCotizacion.Cotizacion.CotizacionesHoras = listaHoras;


                if (peticionCotizacion.TipoPosicionCodigo == "MATERIALES")
                {
                    var todasLasUM = ObtenerTablaSap(TablasSap.Unidad);
                    var unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(peticionCotizacion.PeticionDeOfertaPosicion.Where(x => x.Posiciones.Codigo != null).Select(x => x.Posiciones.Codigo).ToList());
                    foreach (var posicion in peticionCotizacion.PeticionDeOfertaPosicion)
                    {
                        if (!string.IsNullOrEmpty(posicion.Posiciones.Codigo))
                        {
                            var unidadesPorMaterialSAP = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == posicion.Posiciones.Codigo).Select(x => x.UnidadDeMedida).ToList();
                            posicion.Posiciones.UnidadesDeMedida = todasLasUM.Where(x => unidadesPorMaterialSAP.Contains(x.Codigo)).ToList();
                        }
                        else
                        {
                            posicion.Posiciones.UnidadesDeMedida = todasLasUM.Where(x => x.Id == posicion.Posiciones.UnidadId).ToList();
                        }
                    }
                }

                return peticionCotizacion;
            }
            catch (Exception e)
            {
                Logger.Log.Info($"TraerCotizacion {e.Message}");
                Log.Error(e);
                throw;
            }
        }

        public RespuestaGuardarSOLP GrabarCotizacion(GuardarCotizacion cotizacionDto, HttpFileCollectionBase adjuntos, bool esFinalizado, int usuarioActualId, bool enviarMail = true)
        {
            try
            {
                var respuestaGuardarSOLP = new RespuestaGuardarSOLP() { Errores = new List<string>() };
                var usuario = repositorio.Obtener<Usuario>(usuarioActualId);
                var peticionUsuario = repositorio.Obtener<PeticionDeOfertaUsuario>(cotizacionDto.PeticionOfertaUsuarioId);
                var peticionDeOfertaSolpPosiciones = repositorio.Listar<PeticionDeOfertaSolpPosicion>();
                var cotizacion = cotizacionDto.CotizacionId == 0 ? null :
                    repositorio.Obtener<Cotizacion>(cotizacionDto.CotizacionId);
                var info = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);

                if (cotizacion == null)
                {
                    if (peticionUsuario != null)
                    {
                        cotizacion = new Cotizacion()
                        {
                            PeticionDeOfertaUsuario_Id = peticionUsuario.Id,
                            ObservacionEconomica = cotizacionDto.ObservacionEconomica,
                            ObservacionTecnica = cotizacionDto.ObservacionTecnica,
                            RespetaMateriales = cotizacionDto.RespetaMateriales,
                            RespetaServicios = cotizacionDto.RespetaServicios,
                            CotizacionEstado_Id = (int)(esFinalizado ? CotizacionEstadoEnum.Cotizado : CotizacionEstadoEnum.Incompleta),
                            PeticionDeOfertaUsuario = peticionUsuario,
                            PorcentajeDeHoras = cotizacionDto.PorcentajeDeHoras,
                            CotizacionPosiciones = cotizacionDto.CotizacionPosiciones.Count > 0 ? cotizacionDto.CotizacionPosiciones.Select(x => new CotizacionPosicion
                            {
                                Cantidad = x.Cantidad,
                                FechaDeEntrega = x.FechaDeEntrega != null ? x.FechaDeEntrega.Value : (DateTime?)null,
                                Moneda_Id = x.MonedaId > 0 ? x.MonedaId : (int?)null,
                                Moneda = x.MonedaId > 0 ? info.Where(moneda => moneda.Id == x.MonedaId).FirstOrDefault() : null,
                                Precio = x.Precio,
                                UnidadDeMedida_Id = x.UnidadDeMedidaId > 0 ? x.UnidadDeMedidaId : (int?)null,
                                UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == x.UnidadDeMedidaId).FirstOrDefault() : null,
                                PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                                PeticionDeOfertaSolpPosicion = peticionDeOfertaSolpPosiciones.Where(peticion => peticion.Id == x.PeticionDeOfertaSolpPosicionId).FirstOrDefault(),
                                NoDisponible = x.NoDisponible,
                                FechaDeVigencia = x.FechaDeVigencia != null ? x.FechaDeVigencia.Value : (DateTime?)null,
                                CotizacionSubPosiciones = cotizacionDto.CotizacionSubposiciones.Count > 0 ? cotizacionDto.CotizacionSubposiciones
                                .Where(y => y.CotizacionPosicionId == x.PeticionDeOfertaSolpPosicionId).Select(sub => new CotizacionSubPosicion
                                {
                                    Cantidad = sub.Cantidad,
                                    Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : (int?)null,
                                    Moneda = sub.MonedaId > 0 ? info.Where(moneda => moneda.Id == sub.MonedaId).FirstOrDefault() : null,
                                    Precio = sub.Precio,
                                    UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : (int?)null,
                                    UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == sub.UnidadDeMedidaId).FirstOrDefault() : null,
                                    CotizacionPosicion_Id = sub.CotizacionPosicionId,
                                    SolpSubPosicion_Id = sub.SolpSubPosicionId,
                                }).ToList() : null,
                                PrimerPlazoDeOferta = x.PrimerPlazoDeOferta,
                                PrimeraCantidad = x.PrimeraCantidad,
                                SegundoPlazoDeOferta = x.SegundoPlazoDeOferta,
                                SegundaCantidad = x.SegundaCantidad,
                                TercerPlazoDeOferta = x.TercerPlazoDeOferta,
                                TerceraCantidad = x.TerceraCantidad,


                            }).ToList() : null,
                            UsuarioCreador = usuario,
                            FechaCreacion = DateTime.Now
                        };

                        //if (cotizacionDto.RespetaServicios == true && cotizacionDto.RespetaMateriales == true)
                        //{
                        //    cotizacion.PeticionDeOfertaUsuario.PropuestaTecnicaAprobada = true;
                        //}
                    }
                    repositorio.Agregar(cotizacion);
                }
                else
                {
                    cotizacion.ObservacionEconomica = cotizacionDto.ObservacionEconomica;
                    cotizacion.CotizacionEstado_Id = (int)(esFinalizado ? CotizacionEstadoEnum.Cotizado : CotizacionEstadoEnum.Incompleta);
                    cotizacion.Revision = cotizacion.Revision + 1;
                    GuardarCotizacionPosicion(cotizacionDto, cotizacion, info);
                    cotizacion.ObservacionTecnica = cotizacionDto.ObservacionTecnica;
                    cotizacion.RespetaMateriales = cotizacionDto.RespetaMateriales;
                    cotizacion.RespetaServicios = cotizacionDto.RespetaServicios;
                    cotizacion.PorcentajeDeHoras = cotizacionDto.PorcentajeDeHoras;

                    var archivos = cotizacion.Archivos;
                    if (archivos != null && archivos.Count > 0 && cotizacionDto.ArchivosGuardados.Count != archivos.Count)
                    {
                        var archivosParaBorrar = cotizacion.Archivos
                            .Where(x => !cotizacionDto.ArchivosGuardados.Select(y => y.Id).Contains(x.Id))
                            .ToList();

                        foreach (var archivo in archivosParaBorrar)
                        {
                            repositorio.Remover<Archivo>(archivo);
                        }
                    }
                    if (cotizacion.Archivos == null)
                    {
                        cotizacion.Archivos = new List<Archivo>();
                    }

                    //if (cotizacionDto.RespetaServicios == true && cotizacionDto.RespetaMateriales == true)
                    //{
                    //    cotizacion.PeticionDeOfertaUsuario.PropuestaTecnicaAprobada = true;
                    //}
                }
                bool tieneUnidadDeMedidaNula = false;
                if (peticionUsuario.PeticionDeOferta.Solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES")
                {
                    tieneUnidadDeMedidaNula = tieneUnidadDeMedidaNula = cotizacionDto.CotizacionPosiciones.Where(x => !(x.NoDisponible == true))?.Any(pos =>
                    pos.UnidadDeMedidaId == null || !info.Any(unidad => unidad.Id == pos.UnidadDeMedidaId)) ?? false;
                }
                else
                {
                    tieneUnidadDeMedidaNula = cotizacionDto.CotizacionSubposiciones != null && cotizacionDto.CotizacionSubposiciones.Any(subPosicion =>
                    subPosicion.UnidadDeMedidaId == null || !info.Any(unidad => unidad.Id == subPosicion.UnidadDeMedidaId));
                }

                if (esFinalizado && tieneUnidadDeMedidaNula)
                {
                    respuestaGuardarSOLP.Errores.Add("Debe ingresar la unidad de medida");
                    return respuestaGuardarSOLP;
                }

                repositorio.GuardarCambios();
                GuardarCotizacionHora(cotizacionDto, cotizacion);
                if (adjuntos != null && adjuntos.Count > 0)
                {
                    GuardarArchivosCotizacion(cotizacion, adjuntos);
                }

                try
                {
                    if (cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado && enviarMail)
                    {
                        EnviarMailCotizacion(cotizacion);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error(new Exception($"Error al enviar mail GrabarCotizacion en cotizacion: " + cotizacion.Id));
                    Logger.Log.Error(e);
                }


                if (cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.RegistroInfo != true && cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado
                    && cotizacion.CotizacionPosiciones.FirstOrDefault().PeticionDeOfertaSolpPosicion.SolpPosicion.TipoPosicion.Codigo == "MATERIALES")
                {
                    if (!cotizacion.CotizacionPosiciones.All(x => x.NoDisponible == true))
                    {
                        var registros = CrearRegistroInfoDto(cotizacion);
                        if (registros.Any(x => !x.EsModificar))
                        {
                            CrearRegistroInfo(cotizacion, registros.Where(x => !x.EsModificar).ToList());
                        }
                        registros.ForEach(x => x.EsModificar = true);
                        CrearRegistroInfo(cotizacion, registros);
                    }
                }

                respuestaGuardarSOLP.IdEntidad = cotizacion.Id;
                repositorio.GuardarCambios();
                return respuestaGuardarSOLP;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CrearRegistroInfo(Cotizacion cotizacion, List<RegistroInfoDto> registros)
        {
            var respuesta = agregarRegistroInfoConsumerMOA.AgregarRegistroInfo(registros);
            if (respuesta.Errores != null && respuesta.Errores.Any(x => x.Tipo == "E"))
            {
                try
                {
                    EnviarMailAvisoDeErrorRegistroInfo(cotizacion);
                }
                catch (Exception e)
                {

                    Logger.Log.Error(new Exception($"Error al enviar mail AgregarRegistroInfo en cotizacion: " + cotizacion.Id));
                    Logger.Log.Error(e);
                }
            }
        }

        public RespuestaGuardarSOLP CerrarCotizacion(int peticionId, int usuarioActualId, string observaciones)
        {
            try
            {
                var respuestaGuardarSOLP = new RespuestaGuardarSOLP() { Errores = new List<string>() };


                var po = new PeticionDeOfertaCierre()
                {
                    PeticionDeOferta_Id = peticionId,
                    Usuario_Id = usuarioActualId,
                    Fecha = DateTime.Now,
                    Observacion = observaciones,
                };

                repositorio.Agregar(po);
                repositorio.GuardarCambios();
                respuestaGuardarSOLP.IdEntidad = peticionId;
                return respuestaGuardarSOLP;


            }
            catch (Exception e)
            {
                return new RespuestaGuardarSOLP { Errores = new List<string> { e.Message }, Mensaje = e.Message };
            }
        }

        private void GuardarCotizacionPosicion(GuardarCotizacion cotizacionDto, Cotizacion cotizacion, List<TablaSap> info)
        {
            if (cotizacion.CotizacionPosiciones != null && cotizacion.CotizacionPosiciones.Count > 0)
            {
                foreach (var cotizacionPosicion in cotizacion.CotizacionPosiciones.ToList())
                {
                    var cotizacionPos = cotizacionDto.CotizacionPosiciones.Where(x => x.PeticionDeOfertaSolpPosicionId == cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id).FirstOrDefault();
                    cotizacionPosicion.Cantidad = cotizacionPos.Cantidad;
                    cotizacionPosicion.Precio = cotizacionPos.Precio;
                    cotizacionPosicion.FechaDeEntrega = (DateTime?)cotizacionPos.FechaDeEntrega;
                    cotizacionPosicion.Moneda_Id = cotizacionPos.MonedaId > 0 ? cotizacionPos.MonedaId : (int?)null;
                    cotizacionPosicion.UnidadDeMedida_Id = cotizacionPos.UnidadDeMedidaId > 0 ? cotizacionPos.UnidadDeMedidaId : (int?)null;
                    cotizacionPosicion.Moneda = cotizacionPos.MonedaId > 0 && cotizacionPos.MonedaId != null ? info.Where(moneda => moneda.Id == cotizacionPos.MonedaId).FirstOrDefault() : null;
                    cotizacionPosicion.UnidadDeMedida = cotizacionPos.UnidadDeMedidaId > 0 && cotizacionPos.UnidadDeMedidaId != null ? info.Where(unidad => unidad.Id == cotizacionPos.UnidadDeMedidaId).FirstOrDefault() : null;
                    cotizacionPosicion.NoDisponible = cotizacionPos.NoDisponible;
                    cotizacionPosicion.FechaDeVigencia = (DateTime?)cotizacionPos.FechaDeVigencia;
                    cotizacionPosicion.PrimerPlazoDeOferta = cotizacionPos.PrimerPlazoDeOferta;
                    cotizacionPosicion.PrimeraCantidad = cotizacionPos.PrimeraCantidad;
                    cotizacionPosicion.SegundoPlazoDeOferta = cotizacionPos.SegundoPlazoDeOferta;
                    cotizacionPosicion.SegundaCantidad = cotizacionPos.SegundaCantidad;
                    cotizacionPosicion.TercerPlazoDeOferta = cotizacionPos.TercerPlazoDeOferta;
                    cotizacionPosicion.TerceraCantidad = cotizacionPos.TerceraCantidad;

                    if (cotizacionPosicion.CotizacionSubPosiciones != null && cotizacionPosicion.CotizacionSubPosiciones.Count > 0)
                    {

                        foreach (var item in cotizacionPosicion.CotizacionSubPosiciones)
                        {
                            var sub = cotizacionDto.CotizacionSubposiciones.Where(x => x.CotizacionSubPosicionId == item.Id).FirstOrDefault();

                            item.Cantidad = sub.Cantidad;
                            item.Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : (int?)null;
                            item.Moneda = sub.MonedaId > 0 ? info.Where(moneda => moneda.Id == sub.MonedaId).FirstOrDefault() : null;
                            item.Precio = sub.Precio;
                            item.UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : (int?)null;
                            item.UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == sub.UnidadDeMedidaId).FirstOrDefault() : null;
                            item.SolpSubPosicion_Id = sub.SolpSubPosicionId;
                        }
                    }

                    if (cotizacionDto.CotizacionSubposiciones.Where(x => x.CotizacionSubPosicionId == 0 || x.CotizacionSubPosicionId == null).Count() > 0)
                    {
                        foreach (var sub in cotizacionDto.CotizacionSubposiciones.Where(x => (x.CotizacionSubPosicionId == 0 || x.CotizacionSubPosicionId == null) &&
                        x.CotizacionPosicionId == cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id))
                        {
                            cotizacionPosicion.CotizacionSubPosiciones.Add(new CotizacionSubPosicion
                            {
                                Cantidad = sub.Cantidad,
                                Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : (int?)null,
                                Moneda = sub.MonedaId > 0 ? info.Where(moneda => moneda.Id == sub.MonedaId).FirstOrDefault() : null,
                                Precio = sub.Precio,
                                UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : (int?)null,
                                UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == sub.UnidadDeMedidaId).FirstOrDefault() : null,
                                CotizacionPosicion_Id = sub.CotizacionPosicionId,
                                SolpSubPosicion_Id = sub.SolpSubPosicionId
                            });
                        }
                    }
                }
            }
            else
            {
                cotizacion.CotizacionPosiciones = cotizacionDto.CotizacionPosiciones.Select(x => new CotizacionPosicion
                {
                    Cantidad = x.Cantidad,
                    FechaDeEntrega = x.FechaDeEntrega != null ? x.FechaDeEntrega.Value : (DateTime?)null,
                    Moneda_Id = x.MonedaId > 0 ? x.MonedaId : (int?)null,
                    Precio = x.Precio,
                    UnidadDeMedida_Id = x.UnidadDeMedidaId > 0 ? x.UnidadDeMedidaId : (int?)null,
                    UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == x.UnidadDeMedidaId).FirstOrDefault() : null,
                    PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                    Moneda = x.MonedaId > 0 ? info.Where(moneda => moneda.Id == x.MonedaId).FirstOrDefault() : null,
                    NoDisponible = x.NoDisponible,
                    PrimerPlazoDeOferta = x.PrimerPlazoDeOferta,
                    PrimeraCantidad = x.PrimeraCantidad,
                    SegundoPlazoDeOferta = x.SegundoPlazoDeOferta,
                    SegundaCantidad = x.SegundaCantidad,
                    TercerPlazoDeOferta = x.TercerPlazoDeOferta,
                    TerceraCantidad = x.TerceraCantidad,
                    CotizacionSubPosiciones = cotizacionDto.CotizacionSubposiciones.Count > 0 ? cotizacionDto.CotizacionSubposiciones.Where(y => y.CotizacionPosicionId == x.PeticionDeOfertaSolpPosicionId).Select(sub => new CotizacionSubPosicion
                    {
                        Cantidad = sub.Cantidad,
                        Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : (int?)null,
                        Moneda = sub.MonedaId > 0 ? info.Where(moneda => moneda.Id == sub.MonedaId).FirstOrDefault() : null,
                        Precio = sub.Precio,
                        UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : (int?)null,
                        UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == sub.UnidadDeMedidaId).FirstOrDefault() : null,
                        CotizacionPosicion_Id = sub.CotizacionPosicionId,
                        SolpSubPosicion_Id = sub.SolpSubPosicionId
                    }).ToList() : null,
                }).ToList();
            }
        }

        private void GuardarArchivosCotizacion(Cotizacion cotizacion, HttpFileCollectionBase files)
        {
            var ruta = ObtenerRutaArchivos(cotizacion.Id, FileKeys.AdjuntoCotizacionRevisionEconomica);
            var filesEspecificaciones = files.GetMultiple("fileCotizacionRevisionEconomica");
            for (int i = 0; i < filesEspecificaciones.Count; i++)
            {
                var file = filesEspecificaciones[i];
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                cotizacion.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.AdjuntoCotizacionRevisionEconomica,
                    Ruta = rutaArchivoRename,
                });

                file.SaveAs(rutaArchivoRename);
            }

            var filesEspecificacionesTecnico = files.GetMultiple("fileCotizacionRevisionTecnica");
            for (int i = 0; i < filesEspecificacionesTecnico.Count; i++)
            {
                var file = filesEspecificacionesTecnico[i];
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                cotizacion.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.AdjuntoCotizacionRevisionTecnica,
                    Ruta = rutaArchivoRename,
                });

                file.SaveAs(rutaArchivoRename);
            }
        }

        private void GuardarCotizacionHora(GuardarCotizacion cotizacionDto, Cotizacion cotizacion)
        {
            var cotizacionesHorasEntidad = repositorio.Listar<CotizacionHora>(x => x.Cotizacion_Id == cotizacion.Id);
            //var cotizacionesHoraNuevo = new List<CotizacionHora>();

            if (cotizacionesHorasEntidad != null && cotizacionesHorasEntidad.Count > 0)
            {
                var listaCotizacionBorrar = cotizacionesHorasEntidad.Where(coti => !cotizacionDto.CotizacionesHoras.Any(x => x.Id == coti.Id)).ToList();

                if (listaCotizacionBorrar != null && listaCotizacionBorrar.Count > 0)
                {
                    repositorio.RemoverTodos(listaCotizacionBorrar);
                }
            }
            if (cotizacionDto.CotizacionesHoras != null && cotizacionDto.CotizacionesHoras.Count > 0)
            {
                foreach (var cotiHora in cotizacionDto.CotizacionesHoras)
                {
                    if (cotiHora.Id != 0)
                    {
                        var cot = cotizacionesHorasEntidad.FirstOrDefault(x => x.Id == cotiHora.Id);
                        cot.Gremio = cotiHora.Gremio;
                        cot.Categoria = cotiHora.Categoria;
                        cot.HorasExtras = cotiHora.HorasExtras;
                        cot.HorasNormales = cotiHora.HorasNormales;
                        cot.HorasNocturnas = cotiHora.HorasNocturnas;
                        cot.CantidadPersonas = cotiHora.CantidadPersonas;
                        cot.ConfigurarHora = cotiHora.ConfigurarHora;
                    }
                    else
                    {
                        var cotiH = new CotizacionHora()
                        {
                            Cotizacion_Id = cotizacion.Id,
                            Gremio = cotiHora.Gremio,
                            Categoria = cotiHora.Categoria,
                            HorasExtras = cotiHora.HorasExtras,
                            HorasNocturnas = cotiHora.HorasNocturnas,
                            HorasNormales = cotiHora.HorasNormales,
                            CantidadPersonas = cotiHora.CantidadPersonas,
                            ConfigurarHora = cotiHora.ConfigurarHora
                        };
                        repositorio.Agregar(cotiH);
                        //cotizacionesHoraNuevo.Add(cotiH);
                    }
                }
                //repositorio.AgregarTodos(cotizacionesHoraNuevo);
            }
        }

        private ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(int MonedaOrigen_Id, int MonedaDestino_Id, DateTime Fecha)
        {
            var origen = repositorio.Obtener<TablaSap>(MonedaOrigen_Id);
            var destino = repositorio.Obtener<TablaSap>(MonedaDestino_Id);
            ObtenerTipoCambioConsumerMOAResponse result = obtenerTipoCambioConsumerMOA.Request(Fecha.ToString("yyyy-MM-dd"), destino.Codigo, origen.Codigo);

            return result;
        }

        private void EnviarMailCotizacion(Cotizacion cotizacion)
        {
            var peticion = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta;
            var asunto = "";
            var enviarA = new List<string> { peticion.Usuario.Mail };
            if (!string.IsNullOrEmpty(peticion.Solp?.UsuarioCreacion?.Mail))
            {
                enviarA.Add(peticion.Solp.UsuarioCreacion.Mail);
            }

            if (!string.IsNullOrEmpty(peticion.Solp?.Pliego?.Email))
            {
                //Mail del solicitante
                enviarA.Add(peticion.Solp.Pliego.Email);
            }

            asunto += "NUEVA cotización creada - SOLP " + peticion.Solp.NroSolp;
            emailService.EnviarMail(enviarA, asunto, "", null, CuerpoMailCotizacion(cotizacion), null, null, null, null);
        }

        private void EnviarMailAvisoDeErrorRegistroInfo(Cotizacion cotizacion)
        {
            var asunto = "";
            var enviarA = new List<string> { ConfigurationManager.AppSettings["EmailToReporteLogins"] };
            asunto += "Error al agregar registro info en cotizacion: " + cotizacion.Id;

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString("Se informa que al momento de finalizar una cotizacion, el registro info no se pudo generar, revisar los logs", null, "text/html");
            emailService.EnviarMail(enviarA, asunto, "", null, alternateView, null, null, null, null);
        }

        private AlternateView CuerpoMailCotizacion(Cotizacion cotizacion)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            var proveedor = cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la cotización realizada para la SOLP " +
                $"{cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.NroSolp} y la PO {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id}, generada por el proveedor {proveedor.RazonSocial} ({proveedor.CUIT}). <br /> <br/>";

            var todasLasPosicionesNoDisponibles = cotizacion.CotizacionPosiciones.All(x => x.NoDisponible != null && x.NoDisponible.Value);
            var algunaPosicionNoDisponible = cotizacion.CotizacionPosiciones.Any(x => x.NoDisponible != null && x.NoDisponible.Value);
            var noRespetaMateriales = cotizacion.RespetaMateriales == false;
            if (todasLasPosicionesNoDisponibles || algunaPosicionNoDisponible || noRespetaMateriales)
            {
                htmlBody += $"<strong>Nota:</strong><br/>";
            }
            if (todasLasPosicionesNoDisponibles)
            {
                htmlBody += $"El proveedor no cuenta con el material disponible.<br/>";

            }
            else if (algunaPosicionNoDisponible)
            {
                htmlBody += $"El proveedor no cuenta con algún material disponible.<br/>";

            }
            if (noRespetaMateriales)
            {
                htmlBody += $"La propuesta no cumple con las especificaciones técnicas solicitadas. Revisar con prioridad. <br/>";
            }

            htmlBody += " <br/>Puede visualizar la cotización en www.moaoperaciones.com.ar " +
                 "<br/><br/>Saludos Cordiales<br/>" +
                 "Molinos Agro S.A. <br/><br/> " +
                  @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public GuardarCotizacion ObtenerPrecioTotalPosicionProveedor(GuardarCotizacion cotizacionDto)
        {
            try
            {
                var precioTotalPosicion = new List<CotizacionPosicionDto>();
                var subposiciones = cotizacionDto.CotizacionSubposiciones;
                var posiciones = cotizacionDto.CotizacionPosiciones;
                Dictionary<int, decimal> tipodecambio = new Dictionary<int, decimal>();
                decimal cambio = 0;
                var destino = repositorio.Obtener<TablaSap>(x => x.Codigo == "ARP" && x.Tabla == TablasSap.Moneda);

                if (posiciones != null)
                {
                    foreach (var posicion in posiciones)
                    {
                        if (subposiciones != null)
                        {
                            foreach (var subpos in subposiciones.Where(x => x.CotizacionPosicionId == posicion.PeticionDeOfertaSolpPosicionId))
                            {
                                if (subpos.Precio > 0 && subpos.Cantidad > 0 && subpos.MonedaId > 0)
                                {

                                    if ((subpos.MonedaId != null && !tipodecambio.TryGetValue(subpos.MonedaId.Value, out cambio)))
                                    {
                                        var tipoCambio = ObtenerTipoCambio(subpos.MonedaId.Value, destino.Id, DateTime.Now);
                                        tipodecambio.Add(subpos.MonedaId.Value, tipoCambio.TipoCambio);
                                        cambio = tipoCambio.TipoCambio;
                                    }
                                    subpos.PrecioTotal = subpos.Cantidad * subpos.Precio;
                                    //si la moneda está en pesos no hacer conversion
                                    subpos.PrecioTotal = subpos.MonedaId != 224 ? cambio * subpos.PrecioTotal : subpos.PrecioTotal;
                                }
                            }
                            posicion.PrecioTotal = subposiciones.Where(x => x.CotizacionPosicionId == posicion.PeticionDeOfertaSolpPosicionId).Sum(x => x.PrecioTotal);

                        }
                    }
                }
                cotizacionDto.CotizacionSubposiciones = subposiciones;
                cotizacionDto.CotizacionPosiciones = posiciones;

                return cotizacionDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RespuestaCrearOrdenDeCompra GrabarAdjudicacion(AdjudicacionDto adjudicacionDto, int usuarioActualId, string mensaje = "")
        {
            var adjudicacion = new Adjudicacion();
            try
            {
                var respuestaGuardarSOLP = new RespuestaCrearOrdenDeCompra();
                var usuario = repositorio.Obtener<Usuario>(usuarioActualId);
                var cotizacion = repositorio.Obtener<Cotizacion>(adjudicacionDto.Cotizacion_Id);
                var tablasap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                var numerosDePedido = new List<string>();
                var esMateriales = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";
                if (adjudicacionDto.EsMonedaProveedor)
                {
                    var monedaProv = DevolverMonedaProveedor(adjudicacionDto.Proveedor).Moneda;
                    if (!string.IsNullOrEmpty(monedaProv))
                    {
                        adjudicacionDto.Moneda_Id = tablasap.Where(moneda => moneda.CodigoSap == monedaProv).FirstOrDefault().Id;
                        adjudicacionDto.AdjudicacionPosiciones.ForEach(x => x.MonedaId = adjudicacionDto.Moneda_Id);
                    }
                    else
                    {
                        respuestaGuardarSOLP.Errores = new List<string> { "El proveedor seleccionado no tiene una moneda configurada." };
                        return respuestaGuardarSOLP;
                    }
                }
                else
                {
                    if (!esMateriales)
                    {
                        var cotizacionPosicionIds = adjudicacionDto.AdjudicacionPosiciones.Select(x => x.CotizacionPosicion_Id).ToList();
                        var cotizacionPosiciones = cotizacion.CotizacionPosiciones.Where(posicion => cotizacionPosicionIds.Contains(posicion.Id)).ToList();
                        ValidarAdjudicarSubposicionMoneda(cotizacionPosicionIds, cotizacionPosiciones, respuestaGuardarSOLP);
                        if (respuestaGuardarSOLP.Errores != null && respuestaGuardarSOLP.Errores.Any())
                        {
                            return respuestaGuardarSOLP;
                        }
                        adjudicacionDto.AdjudicacionPosiciones.ForEach(x => x.MonedaId = cotizacionPosiciones.FirstOrDefault().CotizacionSubPosiciones.FirstOrDefault().Moneda_Id);
                    }
                }

                //adjudicacionDto.Moneda_Id = tablasap.Where(moneda => moneda.CodigoSap == "ARP").FirstOrDefault().Id;
                var todasLasCotizacionPosiciones = cotizacion.CotizacionPosiciones.ToDictionary(x => x.Id);
                var todasLasSolpPosiciones = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones.ToDictionary(x => x.Id);

                var posicionesPorMoneda = adjudicacionDto.AdjudicacionPosiciones.GroupBy(posicion => posicion.MonedaId);
                var regiones = repositorio.Listar<RegionSap>();
                foreach (var grupo in posicionesPorMoneda)
                {
                    var monedaKey = grupo.Key;
                    if (monedaKey != null)
                    {
                        var posiciones = grupo.Select(x =>
                        {
                            var cotizacionPosicion = todasLasCotizacionPosiciones.TryGetValue(x.CotizacionPosicion_Id, out var cotPos) ? cotPos : null;
                            var solpPosicion = todasLasSolpPosiciones.TryGetValue(x.SolpPosicion_Id, out var solpPos) ? solpPos : null;
                            decimal monto = 0;

                            var ap = new AdjudicacionPosicion
                            {
                                Cantidad = x.Cantidad,
                                CotizacionPosicion_Id = x.CotizacionPosicion_Id,
                                CotizacionPosicion = cotizacionPosicion,
                                Posicion = solpPosicion,
                                SolpPosicion_Id = x.SolpPosicion_Id,
                                PlazoDeEntrega = esMateriales ? x.PlazoDeEntrega.Value
                                : x.PlazoDeEntrega.Value.AddDays(cotizacionPosicion.PrimerPlazoDeOferta ?? 0)
                            };

                            if (!esMateriales)
                            {
                                monto = DevolverMontoServicio(ap, tablasap.Where(moneda => moneda.Id == monedaKey.Value).FirstOrDefault().Codigo);
                            }
                            else
                            {
                                monto = cotizacionPosicion?.Precio.Value *
                                ObtenerTipoCambio(cotizacionPosicion?.Moneda_Id ?? 0, monedaKey.Value, DateTime.Now).TipoCambio ?? 0;
                            }
                            ap.Monto = monto;
                            return ap;

                        }).ToList();

                        adjudicacion = new Adjudicacion
                        {
                            Cotizacion_Id = adjudicacionDto.Cotizacion_Id,
                            Moneda_Id = monedaKey.Value,
                            Moneda = tablasap.FirstOrDefault(moneda => moneda.Id == monedaKey.Value),
                            Cotizacion = cotizacion,
                            Solp_Id = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp_Id,
                            Solp = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp,
                            FechaCreacion = DateTime.Now,
                            Usuario = usuario,
                            UsuarioCreador_Id = usuario.Id,
                            MontoTotal = CalcularMontoTotal(adjudicacionDto, cotizacion, tablasap),
                            CondicionesDeEntrega = adjudicacionDto.CondicionesDeEntrega,
                            CondicionesDePago = adjudicacionDto.CondicionesDePago,
                            Garantias = adjudicacionDto.Garantias,
                            TextoDeCabecera = adjudicacionDto.TextoDeCabecera,
                            Posiciones = posiciones,
                            Token = Guid.NewGuid().ToString(),
                            RegionSap = regiones.Where(c => c.Id == adjudicacionDto.RegionSap).FirstOrDefault(),
                            RegionSap_Id = adjudicacionDto.RegionSap,
                            NumeroOrdenDeCompra = ""
                        };

                        //if (adjudicacion.Posiciones.FirstOrDefault().Posicion.TipoPosicion.Codigo == "SERVICIO")
                        //{
                        //    var precioSolp = adjudicacion.Posiciones.SelectMany(p => p.Posicion.Subposiciones).Sum(subpos => subpos.PrecioBruto * subpos.Cantidad);
                        //    var precioCotizacion = adjudicacion.Posiciones.SelectMany(c => c.CotizacionPosicion.CotizacionSubPosiciones).Sum(subpos => subpos.Precio * subpos.Cantidad);

                        //    if (adjudicacion.Posiciones.Any(x => x.Posicion.Moneda_Id != adjudicacion.Moneda_Id))
                        //    {
                        //        respuestaGuardarSOLP.Errores = new List<string> { $"La moneda de lo solicitado en la SOLP no coincide con la moneda de la cotización.\n" +
                        //            $"Por favor, edite la SOLP para que la moneda y monto sean iguales a lo que cotizó el proveedor e intente adjudicar nuevamente.\n\n" };
                        //        return respuestaGuardarSOLP;
                        //    }
                        //    if (precioSolp != precioCotizacion)
                        //    {
                        //        respuestaGuardarSOLP.Errores = new List<string> { $"El monto cotizado no coincide con el monto solicitado en la SOLP.\n \n" +
                        //            $"- A adjudicar: {adjudicacion.Moneda.CodigoSap} {precioCotizacion:N2}\n " +
                        //            $"- Solicitado: {adjudicacion.Moneda.CodigoSap} {precioSolp:N2}\n\n" };
                        //        return respuestaGuardarSOLP;
                        //    }
                        //}
                        repositorio.Agregar(adjudicacion);
                        repositorio.GuardarCambios();
                        respuestaGuardarSOLP = CrearOrdenDeCompra(adjudicacion, adjudicacionDto.CreadoAutomatico);

                        if (respuestaGuardarSOLP.Errores == null || respuestaGuardarSOLP.Errores.Count == 0)
                        {
                            adjudicacion.NumeroOrdenDeCompra = respuestaGuardarSOLP.NumeroPedido;
                            numerosDePedido.Add(adjudicacion.NumeroOrdenDeCompra);
                        }
                        else
                        {
                            repositorio.Remover(adjudicacion);
                        }

                        repositorio.GuardarCambios();
                        try
                        {
                            var mails = DevolverMailResultadoLicitacion(cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta);
                            if (mails.Count > 0 && !esMateriales)
                            {
                                EnviarMailResultadoAdjudicacion(mails, cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta);
                            }
                        }
                        catch (Exception)
                        {
                            Logger.Log.Info($"Error al enviar mail {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id} para el cierre de la cotizacion");
                        }
                    }
                }
                respuestaGuardarSOLP.NumerosDePedido = numerosDePedido;
                ActualizarDatosSolp(numerosDePedido, cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp);
                return respuestaGuardarSOLP;
            }
            catch (Exception e)
            {
                if (adjudicacion != null && adjudicacion.Id > 0)
                {
                    repositorio.Remover(adjudicacion);
                    repositorio.GuardarCambios();
                }
                throw;
            }
        }

        private decimal DevolverMontoServicio(AdjudicacionPosicion adjudicacionPosicion, string monedaCodigo)
        {
            decimal total = 0;
            var fecha = DateTime.Now;
            decimal tipoDeCambio = 1;
            var moneda = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.FirstOrDefault().Moneda.Codigo;

            if (moneda != monedaCodigo)
            {
                tipoDeCambio = obtenerTipoCambioConsumerMOA.Request(fecha.ToString("yyyy-MM-dd"), moneda, monedaCodigo).TipoCambio;
            }

            foreach (var item in adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones)
            {
                total += item.Cantidad.Value * item.Precio.Value * tipoDeCambio;
            }


            return total;
        }

        private RespuestaCrearOrdenDeCompra ValidarAdjudicarSubposicionMoneda(List<int> cotizacionPosicionIds, List<CotizacionPosicion> cotizacionPosiciones, RespuestaCrearOrdenDeCompra respuestaGuardarSOLP)
        {
            var posicionesAValidar = cotizacionPosiciones.Where(posicion => cotizacionPosicionIds.Contains(posicion.Id));
            var todasLasSubposiciones = new List<CotizacionSubPosicion>();
            foreach (var posicion in posicionesAValidar)
            {
                todasLasSubposiciones.AddRange(posicion.CotizacionSubPosiciones);
            }

            foreach (var item in todasLasSubposiciones)
            {
                bool monedasIguales = todasLasSubposiciones.Select(s => s.Moneda_Id).Distinct().Count() == 1;
                if (!monedasIguales)
                {
                    if (respuestaGuardarSOLP.Errores == null)
                    {
                        respuestaGuardarSOLP.Errores = new List<string>();
                    }
                    respuestaGuardarSOLP.Errores.Add("Todas las subposiciones seleccionadas deben tener la misma moneda");
                    respuestaGuardarSOLP.MostrarModalMoneda = true;
                    return respuestaGuardarSOLP;
                }
            }

            return respuestaGuardarSOLP;
        }


        private List<string> DevolverMailResultadoLicitacion(PeticionDeOferta peticion)
        {
            var mails = new List<string>();
            var adjudicacionPosiciones = repositorio.Listar<AdjudicacionPosicion>();
            var adicionales = repositorio.Listar<PeticionDeOfertaUsuarioAdicional>(a => a.PeticionDeOferta_Id == peticion.Id);
            bool seAdjudicaronTodasLasPosiciones = peticion.Posiciones.All(p => adjudicacionPosiciones.Any(ap => ap.SolpPosicion_Id == p.SolpPosicion_Id));
            if (seAdjudicaronTodasLasPosiciones)
            {
                // Obtener todos los usuarios que realizaron una cotización
                var cotizaciones = repositorio.Listar<Cotizacion>()
                .Where(cotizacion => peticion.Usuarios.Contains(cotizacion.PeticionDeOfertaUsuario))
                .ToList();

                foreach (var cotizacion in cotizaciones)
                {
                    var cotizacionIds = cotizacion.CotizacionPosiciones.Select(cp => cp.Id).ToList();

                    // Verificar si al menos una de las adjudicaciones de posiciones tiene una cotización
                    bool algunaAdjudicacionConCotizacion = cotizacionIds.Any(id =>
                        adjudicacionPosiciones.Any(ap => ap.CotizacionPosicion_Id == id)
                    );

                    if (!algunaAdjudicacionConCotizacion)
                    {
                        mails.Add(cotizacion.PeticionDeOfertaUsuario.Usuario.Mail);
                        mails.AddRange(adicionales.Where(a => a.Usuario.CUITRegistro == cotizacion.PeticionDeOfertaUsuario.Usuario.CUITRegistro).Select(a => a.Usuario.Mail).ToList());
                    }
                }
            }
            return mails.Distinct().ToList();
        }

        private decimal CalcularMontoTotal(AdjudicacionDto adjudicacionDto, Cotizacion cotizacion, List<TablaSap> info)
        {
            Dictionary<int, decimal> tipodecambio = new Dictionary<int, decimal>();
            decimal cambio = 1;
            decimal montoTotal = 0;
            var cotizacionPosiciones = cotizacion.CotizacionPosiciones.Where(x => adjudicacionDto.AdjudicacionPosiciones.Select(y => y.CotizacionPosicion_Id).Contains(x.Id));
            if (cotizacionPosiciones != null)
            {
                bool esServicios = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() != "MATERIALES";
                foreach (var cotizacionPosicion in cotizacionPosiciones)
                {
                    if (esServicios)
                    {
                        if (cotizacionPosicion.CotizacionSubPosiciones != null)
                        {
                            foreach (var subpos in cotizacionPosicion.CotizacionSubPosiciones)
                            {

                                if (subpos.Precio > 0 && subpos.Cantidad > 0 && subpos.Moneda_Id.Value > 0)
                                {
                                    decimal totalSub;
                                    cambio = 1;

                                    if ((subpos.Moneda_Id != null && !tipodecambio.TryGetValue(subpos.Moneda_Id.Value, out cambio)))
                                    {
                                        var tipoCambio = ObtenerTipoCambio(subpos.Moneda_Id.Value, info.First(moneda => moneda.CodigoSap == "ARP").Id, DateTime.Now);
                                        tipodecambio.Add(subpos.Moneda_Id.Value, tipoCambio.TipoCambio);
                                        cambio = tipoCambio.TipoCambio;
                                    }

                                    totalSub = (decimal)(subpos.Cantidad * subpos.Precio * cambio);
                                    montoTotal += totalSub;
                                }

                            }
                            cotizacionPosicion.Cantidad = 1;
                            cotizacionPosicion.Moneda_Id = cotizacionPosicion.CotizacionSubPosiciones.First().Moneda_Id;
                        }
                    }
                    else
                    {
                        decimal totalPos;
                        cambio = 1;
                        if ((cotizacionPosicion.Moneda_Id != null && !tipodecambio.TryGetValue(cotizacionPosicion.Moneda_Id.Value, out cambio)))
                        {
                            var tipoCambio = ObtenerTipoCambio(cotizacionPosicion.Moneda_Id.Value, info.First(moneda => moneda.CodigoSap == "ARP").Id, DateTime.Now);
                            tipodecambio.Add(cotizacionPosicion.Moneda_Id.Value, tipoCambio.TipoCambio);
                            cambio = tipoCambio.TipoCambio;
                        }
                        totalPos = (decimal)(cotizacionPosicion.Cantidad * cotizacionPosicion.Precio * cambio);

                        montoTotal += totalPos;
                    }
                }
            }
            return montoTotal;
        }

        public List<AdjudicacionDto> ListarAdjudicaciones(int solpId)
        {
            var nroSolp = repositorio.Obtener<Solp, string>(a => a.Id == solpId, a => a.NroSolp);
            var respuestaSAP = obtenerOrdenesDeCompraParaSOLPConsumerMOA.Request(nroSolp, "");
            var estados = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.EstadoOC).ToList();
            var listaResultado = respuestaSAP.Select(adjudicacion => new AdjudicacionDto()
            {
                Id = 0,
                Solp_Id = solpId,
                TipoPosicionCodigo = adjudicacion.Cabecera.Tipo,
                NumeroOrdenDeCompra = adjudicacion.Cabecera.OrdenDeCompra,
                FechaCreacion = adjudicacion.Cabecera.FechaCreacion,
                Proveedor = adjudicacion.Cabecera.RazonSocialProveedor,
                MonedaDescripcion = adjudicacion.Cabecera.Moneda,
                PrecioFinal = adjudicacion.Cabecera.MontoTotal,
                PrecioBruto = adjudicacion.Cabecera.MontoBruto,
                EstadoLiberacionCodigo = adjudicacion.Cabecera.EstadoLiberacionCodigo,
                EstadoLiberacionDetalle = estados.SingleOrDefault(a => a.CodigoSap == adjudicacion.Cabecera.EstadoLiberacionCodigo)?.Descripcion ?? "",

            }).OrderBy(fc => fc.FechaCreacion).ToList();

            return listaResultado;
        }

        public AdjudicacionDto ObtenerAdjudicacion(int adjudicacionId) // No se está usando pero no borrar
        {
            var adjudicar = repositorio.Obtener<Adjudicacion, AdjudicacionDto>(adjudicacion => adjudicacion.Id == adjudicacionId, adjudicacion => new AdjudicacionDto
            {
                Id = adjudicacion.Id,
                TipoPosicionCodigo = adjudicacion.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault(),
                NumeroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra,
                PrecioFinal = adjudicacion.MontoTotal,
                TextoDeCabecera = adjudicacion.TextoDeCabecera,
                CondicionesDeEntrega = adjudicacion.CondicionesDeEntrega,
                CondicionesDePago = adjudicacion.CondicionesDePago,
                Garantias = adjudicacion.Garantias,
                AdjudicacionPosiciones = adjudicacion.Posiciones.Select(posicion => new AdjudicacionPosicionDto
                {
                    SolpPosicion_Id = posicion.SolpPosicion_Id,
                    Id = posicion.Id,
                    MaterialComprasCodigo = posicion.Posicion.MaterialSolp.Codigo,
                    Indice = posicion.Posicion.Indice,
                    Tarea = posicion.Posicion.Tarea,
                    TextoSuministro = posicion.Posicion.TextoSuministro,
                    Modelo = posicion.Posicion.Modelo,
                    Cantidad = posicion.Adjudicacion.Solp.Posiciones.Select(posicionSolp => posicionSolp.TipoPosicion.Codigo).FirstOrDefault() == "MATERIALES" ? posicion.Cantidad : 1,
                    PrecioUnidad = posicion.CotizacionPosicion.Precio,
                    MonedaId = posicion.CotizacionPosicion.Moneda_Id,
                    UnidadDescripcion = posicion.CotizacionPosicion.UnidadDeMedida.Descripcion,
                    MonedaDescripcion = posicion.CotizacionPosicion.Moneda.CodigoSap,
                    PrecioTotal = posicion.Cantidad * posicion.CotizacionPosicion.Precio,
                    FechaEntregaServicio = posicion.Posicion.FechaEntregaServicio,
                    PlazoDeOferta = posicion.Posicion.PlazoEntrega,
                    SubposicionesCompras = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(subpos =>
                                                new SolpSubposicionDto()
                                                {
                                                    Numero = subpos.SolpSubPosicion.Numero,
                                                    Tarea = subpos.SolpSubPosicion.Tarea,
                                                    CodigoSolp = subpos.SolpSubPosicion.ServicioSolp.CodigoSap,
                                                    Cantidad = subpos.Cantidad,
                                                    PrecioBruto = subpos.Precio,
                                                    UnidadComprasDescripcion = subpos.UnidadDeMedida.Descripcion,
                                                    MonedaCotizacionDescripcion = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Count() == 1
                                                    ? subpos.Moneda.Descripcion : "Error",
                                                    PrecioTotalSubPosicion = posicion.CotizacionPosicion.CotizacionSubPosiciones.Select(moneda => moneda.Moneda_Id).GroupBy(m => m).Count() == 1 ?
                                                    (subpos.Cantidad.Value * subpos.Precio.Value) : 0,
                                                }).ToList(),
                }).ToList()

            });
            return adjudicar;
        }

        public AdjudicacionDto ObtenerAdjudicacion(string nroOC)
        {
            var ordenDeCompraSAP = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraAdjudicacion(nroOC);

            var monedaPesos = repositorio.Obtener<TablaSap>(a => a.Tabla == "Moneda" && a.CodigoSap == "ARP");
            if (ordenDeCompraSAP.Moneda_Id != monedaPesos.Id)
            {
                var monedaAdjudicacion = repositorio.Obtener<TablaSap>(a => a.Tabla == "Moneda" && a.Id == ordenDeCompraSAP.Moneda_Id);
                var tipoCambio = ObtenerTipoCambio(monedaAdjudicacion.Id, monedaPesos.Id, ordenDeCompraSAP.FechaCreacion);
                ordenDeCompraSAP.PrecioFinal = ordenDeCompraSAP.PrecioFinal * tipoCambio.TipoCambio;
            }


            if (ordenDeCompraSAP.TipoPosicionCodigo == "MATERIALES")
            {
                var todasLasUM = ObtenerTablaSap(TablasSap.Unidad);
                var unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(ordenDeCompraSAP.AdjudicacionPosiciones.Where(x => x.MaterialComprasCodigo != null).Select(x => x.MaterialComprasCodigo).ToList());
                foreach (var posicion in ordenDeCompraSAP.AdjudicacionPosiciones)
                {
                    posicion.UnidadMedida = new TablaSapDto
                    {
                        Codigo = posicion.UnidadCodigo,
                        Id = posicion.UnidadId
                    };
                    if (!string.IsNullOrEmpty(posicion.MaterialComprasCodigo))
                    {
                        var unidadesPorMaterialSAP = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == posicion.MaterialComprasCodigo).Select(x => x.UnidadDeMedida).ToList();
                        posicion.UnidadesDeMedida = todasLasUM.Where(x => unidadesPorMaterialSAP.Contains(x.Codigo)).ToList();
                    }
                    else
                    {
                        posicion.UnidadesDeMedida = todasLasUM.Where(x => x.Id == posicion.UnidadId).ToList();
                    }
                }
            }

            return ordenDeCompraSAP;
        }

        public void CrearCotizacionConTrabajoYaHecho(Solp solp)
        {
            try
            {
                //Crear Peticion 
                var usuariosIds = new List<int> { solp.ProveedorAsignado_Id.Value };
                PeticionDeOferta peticionEntidad = CrearPeticionAutomatica(solp, usuariosIds);
                //Crear Cotizacion
                CrearCotizacionAutomatica(solp, false, peticionEntidad, out RespuestaGuardarSOLP respuestaCotizacion, out Cotizacion cotizacionNueva, null);
                //Completar revisión técnica
                peticionEntidad.RevisionTecnica = new PeticionDeOfertaRevisionTecnica
                {
                    Usuario_Id = peticionEntidad.UsuarioCreador_Id,
                    Fecha = DateTime.Now,
                    RecotizacionEconomica = false,
                    ObservacionRecotizacion = "Trabajo ya hecho",
                    Finalizada = true
                };
                peticionEntidad.PlazoDeOferta = DateTime.Now;
                repositorio.GuardarCambios();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private RespuestaCrearOrdenDeCompra CrearOrdenDeCompraAutomatica(Solp solp, bool enviarMail, List<RegistroInfoDto> registroInfo, bool crearAdjudicacion, int usuarioActual)
        {
            try
            {
                var respuestaGuardarSOLP = new RespuestaCrearOrdenDeCompra();
                RespuestaGuardarSOLP respuestaCotizacion;
                Cotizacion cotizacionNueva;
                var solpPosicionIds = registroInfo.Select(registro => registro.PosicionId).ToList();
                var centroSolp = solp.Posiciones.FirstOrDefault().Centro.CodigoSap;
                var centroRegion = repositorio.Obtener<CentroDireccion>(cr => cr.CodigoSap == centroSolp);

                List<SolpPosicion> posiciones = solp.Posiciones.Where(a => solpPosicionIds.Contains(a.Id)).ToList();
                //Crear Peticion 
                var usuariosIds = new List<int>();
                int proveedorId = registroInfo.Select(x => x.ProveedorId).First();
                usuariosIds.Add(proveedorId);
                PeticionDeOferta peticionEntidad = CrearPeticionAutomatica(solp, usuariosIds, posiciones, true, registroInfo);
                //CrearCotizacion
                CrearCotizacionAutomatica(solp, enviarMail, peticionEntidad, out respuestaCotizacion, out cotizacionNueva, posiciones, registroInfo);
                //Crear Adjudicacion
                if (crearAdjudicacion)
                {
                    List<AdjudicacionPosicionDto> adjudicacionPosiciones =
                        cotizacionNueva.CotizacionPosiciones.Select(x => new AdjudicacionPosicionDto
                        {
                            CotizacionPosicion_Id = x.Id,
                            Cantidad = registroInfo.FirstOrDefault(a => a.PosicionId == x.PeticionDeOfertaSolpPosicion.SolpPosicion_Id)?.CantidadAdjudicacion ?? 1,
                            SolpPosicion_Id = x.PeticionDeOfertaSolpPosicion.SolpPosicion_Id,
                            MonedaId = x.Moneda_Id,
                            PlazoDeEntrega = x.PeticionDeOfertaSolpPosicion.SolpPosicion.FechaEntregaServicio
                        }).ToList();

                    var adjudicacion = new AdjudicacionDto()
                    {
                        Cotizacion_Id = respuestaCotizacion.IdEntidad,
                        AdjudicacionPosiciones = adjudicacionPosiciones,//solp.Posiciones.Where(a=>a.Id == ).Select(x => new AdjudicacionPosicionDto
                                                                        //{
                                                                        //    Cantidad = registroInfo.Where(registro => registro.PosicionId == x.Id).FirstOrDefault().CantidadAdjudicacion,
                                                                        //    CotizacionPosicion_Id = cotizacionNueva.CotizacionPosiciones.Where(cotPos => cotPos.Id == x.Id).Select(pos => pos.Id).FirstOrDefault()
                                                                        //}).ToList(),
                        TextoDeCabecera = "Orden de compra generada a partir de los registros info: " + string.Join(", ", registroInfo.Select(a => a.Id)),
                        CondicionesDePago = "",
                        CondicionesDeEntrega = "",
                        Garantias = "",
                        RegionSap = centroRegion.RegionSap.Id
                    };
                    string mensaje = "Orden de compra generada a partir de las órdenes: " + string.Join(", ", registroInfo.Select(a => a.NumeroOrdenDeCompra));
                    adjudicacion.CreadoAutomatico = true;
                    var resultado = GrabarAdjudicacion(adjudicacion, usuarioActual, mensaje);
                    return resultado;
                }
                return respuestaGuardarSOLP;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CrearCotizacionAutomatica(Solp solp, bool enviarMail, PeticionDeOferta peticionEntidad, out RespuestaGuardarSOLP respuestaCotizacion, out Cotizacion cotizacionNueva, List<SolpPosicion> solpPosiciones = null, List<RegistroInfoDto> registroInfo = null)
        {
            var posiciones = solpPosiciones != null ? solpPosiciones : solp.Posiciones;
            var cotizacion = new GuardarCotizacion
            {
                RespetaMateriales = true,
                RespetaServicios = true,
                FechaDeEntrega = DateTime.Today.AddDays(-1),
                PeticionOfertaUsuarioId = peticionEntidad.Usuarios.Select(x => x.Id).FirstOrDefault(),
                CotizacionPosiciones = posiciones.Select(x => new GuardarCotizacionPosicionDto
                {
                    PeticionDeOfertaSolpPosicionId = peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).FirstOrDefault().Id,
                    Cantidad = peticionEntidad.RegistroInfo == true ? registroInfo.FirstOrDefault(a => a.PosicionId ==
                               peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).FirstOrDefault().SolpPosicion_Id)?.CantidadAdjudicacion
                              : x.Cantidad ?? 1,
                    MonedaId = peticionEntidad.RegistroInfo == true ? registroInfo.FirstOrDefault(a => a.PosicionId ==
                               peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).FirstOrDefault().SolpPosicion_Id)?.MonedaId
                               : x.Moneda_Id,
                    UnidadDeMedidaId = peticionEntidad.RegistroInfo == true ? registroInfo.FirstOrDefault(a => a.PosicionId ==
                               peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).FirstOrDefault().SolpPosicion_Id)?.UnidadId
                               : x.Unidad_Id,
                    FechaDeEntrega = DateTime.Today.AddDays(-1),
                    Precio = peticionEntidad.RegistroInfo == true ? registroInfo.FirstOrDefault(a => a.PosicionId ==
                               peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).FirstOrDefault().SolpPosicion_Id).Precio
                               : x.PrecioBruto != null ? (decimal)x.PrecioBruto : 0,
                }).ToList(),

            };

            foreach (var posicion in posiciones)
            {
                if (posicion.Subposiciones != null && posicion.Subposiciones.Count > 0)
                {
                    foreach (var subposicion in posicion.Subposiciones)
                    {
                        var subpos = new CotizacionSubposicionesDto
                        {
                            Precio = (decimal)subposicion.PrecioBruto,
                            Cantidad = (int)subposicion.Cantidad,
                            UnidadDeMedidaId = subposicion.Unidad_Id,
                            SolpSubPosicionId = subposicion.Id,
                            CotizacionPosicionId = peticionEntidad.Posiciones.Where(pos => pos.SolpPosicion_Id == posicion.Id).FirstOrDefault().Id,
                            MonedaId = posicion.Moneda_Id
                        };
                        cotizacion.CotizacionSubposiciones.Add(subpos);
                    }
                }
            }
            respuestaCotizacion = GrabarCotizacion(cotizacion, null, true, solp.UsuarioCreacion_Id.Value, enviarMail);
            cotizacionNueva = repositorio.Obtener<Cotizacion>(respuestaCotizacion.IdEntidad);
        }

        private PeticionDeOferta CrearPeticionAutomatica(Solp solp, List<int> usuariosIds, List<SolpPosicion> solpPosicions = null, bool esRegistroInfo = false, List<RegistroInfoDto> registroInfoLista = null)
        {
            int usuarioCreacionPOId = ObtenerCompradorCondicionesEspeciales(solp);
            var peticion = new GuardarPeticionDeOfertaDto()
            {
                Observacion = "",
                PosIds = solpPosicions != null ? solpPosicions.Select(x => x.Id).ToList() : solp.Posiciones.Select(x => x.Id).ToList(),
                SolpId = solp.Id,
                UsuarioIds = usuariosIds,
                UsuarioActual = new UsuarioDto
                {
                    Id = usuarioCreacionPOId
                },
                Adjuntos = null,
                RegistroInfo = esRegistroInfo
            };

            var resultado = GrabarPeticionDeOferta(peticion, null, solp.TrabajoYaHecho != true && solp.Adicional == true, registroInfoLista);
            var peticionEntidad = repositorio.Obtener<PeticionDeOferta>(resultado.IdEntidad);

            return peticionEntidad;
        }

        private int ObtenerCompradorCondicionesEspeciales(Solp solp)
        {
            int usuarioCreadorPOId = solp.UsuarioCreacion.Id;
            if (solp.UsuarioCompras_Id.HasValue)
            {
                var usuarioId = repositorio.Obtener<Usuario, int?>(a => a.Mail == solp.UsuarioCompras.Mail, a => a.Id);
                if (usuarioId.HasValue) usuarioCreadorPOId = usuarioId.Value;
            }
            else
            {
                if (solp.Adicional == true)
                {
                    var creadorAdj = repositorio.Obtener<Adjudicacion, int?>(a => a.NumeroOrdenDeCompra == solp.NroOrdenDeCompraAdicional, a => a.UsuarioCreador_Id);
                    if (creadorAdj.HasValue) usuarioCreadorPOId = creadorAdj.Value;
                    else
                    {
                        var adjudicacionDto = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraAdjudicacion(solp.NroOrdenDeCompraAdicional);
                        if (adjudicacionDto != null && adjudicacionDto.UsuarioCreador_Id != 0)
                            usuarioCreadorPOId = adjudicacionDto.UsuarioCreador_Id;
                    }
                }
            }

            return usuarioCreadorPOId;
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            var result = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra(nroOC);
            Log.Info("ObtenerOrdenDeCompra obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra" + result.ToJson());
            if (result.Error == null || string.IsNullOrEmpty(result.Error.Mensaje))
            {
                try
                {
                    ProveedorComprasDto proveedor = ObtenerProveedorCompras(result.Cabecera.CodigoProveedor);
                    Log.Info("ObtenerOrdenDeCompra ObtenerProveedorCompras" + proveedor.ToJson());

                    result.Cabecera.RazonSocialProveedor = proveedor.RazonSocial;
                    result.Cabecera.CUITProveedor = proveedor.CUIT;
                    result.Cabecera.CodigoProveedor = proveedor.CodigoProveedor;
                    result.Cabecera.Usuario_Id = proveedor.Usuario_Id;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    result.Error = new ErrorOC
                    {
                        Mensaje = e.Message,
                        Tipo = "E"
                    };
                    result.Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = "",
                        CodigoProveedor = ""
                    };
                }
            }
            if (result.Error == null || string.IsNullOrEmpty(result.Error.Mensaje))
            {
                try
                {
                    int? UsuarioCompras_Id = null;
                    var usuariosCompras = repositorio.Listar<UsuarioCompras>();

                    var UsuarioCompras_Mail = repositorio.Obtener<Usuario>(a => a.UsuarioSap == result.Cabecera.UsuarioComprasSAP)?.Mail;
                    Log.Info("ObtenerOrdenDeCompra UsuarioCompras_Mail " + UsuarioCompras_Mail);
                    if (UsuarioCompras_Mail != null)
                    {
                        UsuarioCompras_Id = usuariosCompras.Where(a => a.Mail.ToLower() == UsuarioCompras_Mail.ToLower()).FirstOrDefault()?.Id;
                        Log.Info("ObtenerOrdenDeCompra UsuarioCompras_Mail UsuarioCompras_Id" + UsuarioCompras_Id);

                        result.Cabecera.UsuarioCompras_Id = UsuarioCompras_Id;
                    }

                    if (result.Cabecera.UsuarioCompras_Id == null)
                    {
                        var adjudicacion = repositorio.Listar<Adjudicacion>(a => a.NumeroOrdenDeCompra == nroOC, 1, "Id", DirOrden.Desc).FirstOrDefault();
                        Log.Info("ObtenerOrdenDeCompra Adjudicacion " + (adjudicacion == null ? "" : (adjudicacion.Usuario.Mail + "," + adjudicacion.NumeroOrdenDeCompra)));
                        if (adjudicacion != null)
                        {
                            UsuarioCompras_Id = usuariosCompras.FirstOrDefault(a => a.Mail.ToLower() == adjudicacion.Usuario.Mail.ToLower())?.Id;
                            result.Cabecera.UsuarioCompras_Id = UsuarioCompras_Id;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    result.Error = new ErrorOC
                    {
                        Mensaje = e.Message,
                        Tipo = "E"
                    };
                    result.Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = "",
                        CodigoProveedor = ""
                    };
                }
            }
            if ((result.Error == null || string.IsNullOrEmpty(result.Error.Mensaje)) && result.Cabecera.UsuarioCompras_Id == null)
            {
                result.Error = new ErrorOC
                {
                    Mensaje = "No se encontro el usuario Comprador.",
                    Tipo = "E"
                };
                result.Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = "",
                    CodigoProveedor = ""
                };
            }
            Log.Info("ObtenerOrdenDeCompra result" + result.ToJson());
            return result;
        }

        private ProveedorComprasDto ObtenerProveedorCompras(string codigoProveedor)
        {
            var proveedorMoa = obtenerProveedorConsumerMOA.ObtenerProveedor(codigoProveedor);
            if (proveedorMoa == null)
            {
                throw new WSCustomException("No existe un proveedor con ese codigo");
            }
            List<SustitucionMOAModel.Models.FechaWS> fechas = CommonUtil.toDateList(DateTime.Now.AddYears(-5).ToShortDateString(), DateTime.Now.ToShortDateString());
            var vendedoresMoa = vendedoresConsumerMOA.Request(codigoProveedor, fechas);
            if (vendedoresMoa == null || vendedoresMoa.vendedores == null || vendedoresMoa.vendedores.Count == 0)
                throw new WSCustomException("No existe un proveedor con ese codigo.");
            var cuit = vendedoresMoa.vendedores.First().cuit;

            var usuarioDb = repositorio.Obtener<Usuario>(a => a.Mail == proveedorMoa.MAIL);
            if (usuarioDb == null)
            {
                var proveedor = new ProveedorDto
                {
                    Mail = proveedorMoa.MAIL,
                    CUIT = cuit,
                    RazonSocial = proveedorMoa.NAME
                };

                var resultado = usuarioService.GrabarProveedor(proveedor, EstadoAprobacion.Aprobado);
                return new ProveedorComprasDto
                {
                    RazonSocial = proveedorMoa.NAME,
                    CodigoProveedor = codigoProveedor,
                    CUIT = cuit,
                    Usuario_Id = resultado.ProveedorDto.Id
                };
            }
            else
            {
                if (usuarioDb.CUITRegistro != cuit)
                    throw new WSCustomException("El mail está registrado con otro CUIT.");
                if (usuarioDb.TipoUsuario.Id != (int)TipoUsuarioEnum.NoGranos)
                    throw new WSCustomException("El mail no está registrado con el tipo de usuario ''No Granos''.");

                var proveedor = usuarioDb.ObtenerProveedorAsignado();

                return new ProveedorComprasDto
                {
                    RazonSocial = proveedor.RazonSocial,
                    CodigoProveedor = codigoProveedor,
                    CUIT = cuit,
                    Usuario_Id = usuarioDb.Id,
                    Proveedor_Id = proveedor.Id
                };
            }
        }

        public List<RespuestaCrearOrdenDeCompra> CrearOrdenDeCompraConRegistroInfo(List<RegistroInfoDto> registros, int usuarioActualId)
        {
            if (registros.Count == 0)
                throw new ValidationCustomException("Tiene que seleccionar al menos un registro");
            if (registros.Any(x => x.CantidadAdjudicacion == 0))
                throw new ValidationCustomException("Todos los registros seleccionados tienen que tener la cantidad ingresada.");

            try
            {
                var resultado = new List<RespuestaCrearOrdenDeCompra>();
                var solpPosicionIds = registros.Select(registro => registro.PosicionId).ToList();
                var posicionesSolp = repositorio.Listar<SolpPosicion>(x => solpPosicionIds.Contains(x.Id));
                foreach (var item in registros.GroupBy(a => new { a.ProveedorId, a.Moneda }))
                {
                    RespuestaCrearOrdenDeCompra r = CrearOrdenDeCompraAutomatica(posicionesSolp.First().Solp, false, item.ToList(), true, usuarioActualId);
                    r.Proveedor = item.First().NombreProveedor;
                    resultado.Add(r);
                }
                //foreach (var item in posiciones)
                //{
                //    var r = CrearOrdenDeCompraAutomatica(item.Solp, false, registros, true, usuarioActualId, item);
                //    resultado.Add(r.NumeroPedido);
                //}
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<RegistroInfoDto> CrearRegistroInfoDto(Cotizacion cotizacion)
        {
            var registros = new List<RegistroInfoDto>();
            var solpPosiciones = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones.Where(x => x.MaterialSolp != null);
            var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
            if (solpPosiciones.Count() > 0 && solpPosiciones.First().TipoPosicion.Codigo == "MATERIALES")
            {
                unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(solpPosiciones.Select(x => x.MaterialSolp.Codigo).ToList());
            }
            foreach (var cotizacionPosicion in cotizacion.CotizacionPosiciones.Where(x => x.NoDisponible != true && x.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp != null)) //excluye no catalogados
            {
                var registro = new RegistroInfoDto
                {
                    Cantidad = cotizacionPosicion.Cantidad.Value,
                    MaterialCodigo = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo,
                    Cuit = cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor(),
                    Unidad = cotizacionPosicion.UnidadDeMedida.Codigo,
                    OrganizacionDeCompra = "2029",
                    Centro = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.Centro.Codigo,
                    Moneda = cotizacionPosicion.Moneda.Codigo,
                    Precio = cotizacionPosicion.Precio.Value,
                    FechaVigencia = cotizacionPosicion.FechaDeVigencia.HasValue ? cotizacionPosicion.FechaDeVigencia.Value.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(15).Date.ToString("yyyy-MM-dd"),
                    FechaVigenciaFormateada = cotizacionPosicion.FechaDeVigencia ?? DateTime.Now.AddDays(15).Date,
                    GrupoDeCompras = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.GrupoCompras.Codigo,
                    EsModificar = ObtenerUltimoRegistroPorMaterialYProveedor(cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo,
                    cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.Centro.Codigo, "2029", cotizacionPosicion.Cotizacion.UsuarioCreador.ObtenerCodigoProveedor()).EsModificar
                };
                var solpPosicion = solpPosiciones.FirstOrDefault(p => p.MaterialSolp.Codigo == cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo);
                if (solpPosicion != null && cotizacionPosicion.UnidadDeMedida.Id != solpPosicion.Unidad_Id)
                {
                    var unidadesDelMaterial = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
                    var unidadBase = unidadesDelMaterial.First(x => x.Numerador == 1 && x.Denominador == 1);
                    var unidadCotizada = unidadesDelMaterial.First(x => x.UnidadDeMedida == cotizacionPosicion.UnidadDeMedida.Codigo);
                    registro.Unidad = unidadBase.UnidadDeMedida;
                    registro.Cantidad = Math.Round(cotizacionPosicion.Cantidad.Value * (unidadCotizada.Numerador / unidadCotizada.Denominador), 2);
                    registro.Precio = Math.Round(cotizacionPosicion.Precio.Value / (unidadCotizada.Numerador / unidadCotizada.Denominador), 2);
                };
                registros.Add(registro);
            }

            return registros;
        }

        public DatosUltimaSolpDto ObtenerUltimaSolp(int usuarioId)
        {
            var ultimaSolp = repositorio.Listar<Solp>(a => a.UsuarioCreacion_Id == usuarioId && !string.IsNullOrEmpty(a.NroSolp)).OrderByDescending(a => a.Id).FirstOrDefault();


            if (ultimaSolp == null)
                return null;

            DatosUltimaSolpDto result = new DatosUltimaSolpDto();

            result.FiscalContrato = ultimaSolp.Pliego?.FiscalContrato;
            result.Telefono = ultimaSolp.Pliego?.Telefono;

            result.ClaseDocumento = ultimaSolp.ClaseDocumento != null ? new TablaSapDto
            {
                Id = ultimaSolp.ClaseDocumento.Id,
                Tabla = ultimaSolp.ClaseDocumento.Tabla,
                Codigo = ultimaSolp.ClaseDocumento.Codigo,
                CodigoSap = ultimaSolp.ClaseDocumento.CodigoSap,
                Descripcion = ultimaSolp.ClaseDocumento.Descripcion,
                IdPadre = ultimaSolp.ClaseDocumento.Padre_id
            }
                                    : null;

            result.GrupoCompras = ultimaSolp.Posiciones.FirstOrDefault()?.GrupoCompras != null
                                ? new TablaSapDto
                                {
                                    Id = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Id,
                                    Tabla = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Tabla,
                                    Codigo = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Codigo,
                                    CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.CodigoSap,
                                    Descripcion = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Descripcion,
                                    IdPadre = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Padre_id
                                }
                                : null;

            result.CuentaMayor = ultimaSolp.Posiciones.FirstOrDefault()?.CuentaMayorSap != null
                                ? new TablaSapDto
                                {
                                    Id = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Id,
                                    Tabla = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Tabla,
                                    Codigo = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Codigo,
                                    CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.CodigoSap,
                                    Descripcion = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                                    IdPadre = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Padre_id
                                }
                                : null;

            result.Almacen = ultimaSolp.Posiciones.FirstOrDefault()?.Almacen != null
                            ? new TablaSapDto
                            {
                                Id = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Id,
                                Tabla = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Tabla,
                                Codigo = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Codigo,
                                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().Almacen.CodigoSap,
                                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Descripcion,
                                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Padre_id
                            }
                            : null;

            result.TipoPosicion = ultimaSolp.Posiciones.FirstOrDefault()?.TipoPosicion != null
                                ? new TablaGeneralDto
                                {
                                    Id = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Id,
                                    Tabla = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Tabla,
                                    Codigo = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Codigo,
                                    Descripcion = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Descripcion,
                                    IdPadre = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Padre_Id
                                }
                                : null;

            result.Centro = ultimaSolp.Posiciones.FirstOrDefault()?.Centro != null
                               ? new TablaSapDto
                               {
                                   Id = ultimaSolp.Posiciones.FirstOrDefault().Centro.Id,
                                   Tabla = ultimaSolp.Posiciones.FirstOrDefault().Centro.Tabla,
                                   Codigo = ultimaSolp.Posiciones.FirstOrDefault().Centro.Codigo,
                                   CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().Centro.CodigoSap,
                                   Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Centro.Descripcion,
                                   IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Centro.Padre_id
                               }
                               : null;

            result.CuentaMayorSP = ultimaSolp.Posiciones.FirstOrDefault()?.Subposiciones.FirstOrDefault()?.CuentaMayorSap != null
                               ? new TablaSapDto
                               {
                                   Id = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Id,
                                   Tabla = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Tabla,
                                   Codigo = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Codigo,
                                   Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.CodigoSap + " - " + ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                                   IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Padre_id
                               }
                               : null;


            return result;
        }

        /// <summary>Busca en la RFC de registros info el último cargado para un determinado material.</summary>
        /// <param name="material">El código del material</param>
        /// <param name="centro">El código del centro</param>
        /// <param name="grupoDeCompras">El código del grupo de compras</param>
        /// <returns>El último registro info disponible para el material elegido.</returns>
        public RegistroInfoDto ObtenerUltimoRegistroMaterial(string material, string centro, string grupoDeCompras)
        {
            RegistroInfoDto ultimoRegistro = new RegistroInfoDto();
            ultimoRegistro.EsModificar = false;
            var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(material, centro, grupoDeCompras, "")
                               .Where(x => x.NumeroOrdenDeCompra != null).OrderByDescending(x => x.FechaUltimaCompra);
            if (registros.Any())
            {
                ultimoRegistro = registros.First();
                ultimoRegistro.EsModificar = true;
            }
            return ultimoRegistro;
        }

        private RegistroInfoDto ObtenerUltimoRegistroPorMaterialYProveedor(string material, string centro, string grupoDeCompras, string proveedor)
        {
            RegistroInfoDto ultimoRegistro = new RegistroInfoDto();
            ultimoRegistro.EsModificar = false;
            var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(material, centro, grupoDeCompras, proveedor)
                               .Where(x => x.NumeroOrdenDeCompra != null).OrderByDescending(x => x.FechaUltimaCompra);
            if (registros.Any())
            {
                ultimoRegistro = registros.First();
                ultimoRegistro.EsModificar = true;
            }
            return ultimoRegistro;
        }

        private void EnviarMailResultadoAdjudicacion(List<string> mails, PeticionDeOferta peticion)
        {
            try
            {
                var asunto = "";
                var enviarA = new List<string>();
                asunto += $"Cierre de Licitación PO - {peticion.Id} ";
                foreach (var mail in mails)
                {
                    var mailProveedor = new List<string> { mail };
                    emailService.EnviarMail(mailProveedor, asunto, "", null, CuerpoMailResultadoAdjudicacion(peticion));
                }
            }
            catch (Exception e)
            {
                Logger.Log.Info($"Error al enviar mail {peticion.Id}  para el cierre de la cotizacion");
                Logger.Log.Error(e);
            }
        }

        private AlternateView CuerpoMailResultadoAdjudicacion(PeticionDeOferta peticion)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"Estimado proveedor, notificamos que Molinos Agro Sa ha generado el cierre de la licitación bajo número de Pet. De Oferta {peticion.Id}. <br />";
            htmlBody += $"Bajo esta formalidad de aviso, informamos que su empresa <strong>no ha resultado adjudicada</strong>. Esperamos que sea un aporte útil a su seguimiento y sistema de gestión.. <br />";
            htmlBody += $"Agradecemos su participación e interés. <br/><br/> ";

            htmlBody += "<strong>No dar respuesta a este mail.</strong>" +
                "<br/><br/>Atte…<br/>" +
                "Molinos Agro S.A. <br/> Oficina Compras <br/><br/>" +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public LegajoExternoDto ObtenerLegajoParaExternos(int adjudicacionId, string token)
        {
            LegajoExternoDto resultado = new LegajoExternoDto();
            var adjudicacion = repositorio.Obtener<Adjudicacion>(x => x.Id == adjudicacionId && x.Token == token);
            if (adjudicacion != null)
            {
                var cotizacion = adjudicacion.Cotizacion;
                resultado.NroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra;
                resultado.NroSolp = adjudicacion.Solp.NroSolp;
                resultado.Proveedor = new UsuarioDto(adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario);
                resultado.FechaAdjudicacionFormateado = adjudicacion.FechaCreacion.ToString("dd/MM/yyyy");

                resultado.ListaLegajos = ObtenerLegajo(cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id, null);
            }
            return resultado;
        }

        private void ValidarSolpAnulada(string nroSolp)
        {
            var solp = repositorio.Obtener<Solp>(x => x.NroSolp == nroSolp && x.TrabajoYaHecho != true
            && x.PeticionesDeOferta.Count() > 0 && x.Adjudicaciones.Count() == 0 && x.SeEnvioMailAnulacion != true);
            if (solp != null)
            {
                if (solp.Posiciones.All(x => !x.Estado))
                {
                    try
                    {
                        solp.SeEnvioMailAnulacion = true;
                        repositorio.GuardarCambios();
                        EnviarMailSolpAnulada(solp);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        private void EnviarMailSolpAnulada(Solp solp)
        {
            try
            {
                var asunto = "";
                var enviarA = new List<string>();
                var peticionesId = solp.PeticionesDeOferta.Select(y => y.Id);
                var peticionesDeOfertaUsuario = repositorio.Listar<PeticionDeOfertaUsuario>(x => peticionesId.Contains(x.PeticionDeOferta_Id));
                foreach (var peticion in solp.PeticionesDeOferta)
                {
                    var adicionales = repositorio.Listar<PeticionDeOfertaUsuarioAdicional>(x => x.PeticionDeOferta_Id == peticion.Id);
                    asunto += $"Cierre por Baja de Requerimiento PO - {peticion.Id} ";
                    foreach (var peticionUsuario in peticionesDeOfertaUsuario.Where(x => x.PeticionDeOferta_Id == peticion.Id))
                    {
                        var mailProveedor = new List<string> { peticionUsuario.Usuario.Mail };
                        mailProveedor.AddRange(adicionales.Where(a => a.Usuario.CUITRegistro == peticionUsuario.Usuario.CUITRegistro).Select(a => a.Usuario.Mail).ToList());
                        emailService.EnviarMail(mailProveedor, asunto, "", null, CuerpoMailSolpAnulada(peticion));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Info($"Error al enviar EnviarMailSolpAnulada");
                Logger.Log.Error(e);
            }
        }

        private AlternateView CuerpoMailSolpAnulada(PeticionDeOferta peticion)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"Estimado proveedor, notificamos que Molinos Agro Sa ha generado el cierre de la compulsa bajo número de Pet. De Oferta {peticion.Id}. <br/><br/>";
            htmlBody += $"Bajo esta formalidad de aviso, informamos que la compañía <strong>ha desestimado avanzar en la compra del material o servicio solicitado.</strong> <br/><br/>";
            htmlBody += $"Sepa disculpar las molestias ocasionadas. <br/><br/> ";

            htmlBody += "<strong>No dar respuesta a este mail.</strong>" +
                "<br/><br/>Atte…<br/>" +
                "Molinos Agro S.A. <br/> Oficina Compras <br/><br/>" +
                 @"<img width:'5%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<OrdenDeCompraSAPDto> ObtenerReporteOrdenDeCompra(string nroOC, string fechaDesde, string fechasHasta, string codigoProveedor)
        {
            var result = reporteOrdenDeCompraConsumerMOA.Request(nroOC, fechaDesde, codigoProveedor);

            var fechaHastaDate = string.IsNullOrEmpty(fechasHasta) ? DateTime.Now : DateTime.Parse(fechasHasta);

            result = result.OrderByDescending(x => x.Cabecera.FechaCreacion).ToList();

            result = result
              .Where(x => x.Cabecera == null || (x.Cabecera.FechaCreacion <= fechaHastaDate))
              .ToList();

            return result;
        }

        public Resultado GrabarPeticionDeOfertaVisualizacionPrecio(PeticionDeOfertaVisualizacionPrecioDto peticionDeOfertaVisualizacionPrecioDto, HttpFileCollectionBase adjuntos)
        {
            try
            {
                var resultado = new Resultado();
                var peticionDeOfertaVisualizacionPrecio = new PeticionDeOfertaVisualizacionPrecio()
                {
                    Observaciones = peticionDeOfertaVisualizacionPrecioDto.Observacion,
                    PeticionDeOferta_Id = peticionDeOfertaVisualizacionPrecioDto.PeticionDeOferta_Id,
                    UsuarioCreador_Id = peticionDeOfertaVisualizacionPrecioDto.UsuarioCreador_Id,
                    FechaCreacion = DateTime.Now,
                };
                repositorio.Agregar(peticionDeOfertaVisualizacionPrecio);
                repositorio.GuardarCambios();
                GrabarArchivosEnPeticionDeOfertaVisualizacionPrecio(peticionDeOfertaVisualizacionPrecio, adjuntos);
                repositorio.GuardarCambios();
                resultado.IdEntidad = peticionDeOfertaVisualizacionPrecio.Id;
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GrabarArchivosEnPeticionDeOfertaVisualizacionPrecio(PeticionDeOfertaVisualizacionPrecio peticionDeOfertaVisualizacionPrecio, HttpFileCollectionBase files)
        {

            var ruta = ObtenerRutaArchivos(peticionDeOfertaVisualizacionPrecio.Id, FileKeys.PeticionDeOfertaVisualizacionPrecio);
            var filesEspecificaciones = files.GetMultiple("filePeticionDeOfertaVisualizacionPrecio");

            for (int i = 0; i < filesEspecificaciones.Count; i++)
            {
                var file = filesEspecificaciones[i];
                var rutaArchivo = string.Concat(ruta, "/", Path.GetFileName(file.FileName));
                var rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                Directory.CreateDirectory(ruta);

                int copyNro = 1;
                while (File.Exists(rutaArchivoRename))
                {
                    rutaArchivoRename = string.Concat(ruta, "/", Path.GetFileName($"({copyNro}) " + file.FileName));
                    copyNro += 1;
                }

                peticionDeOfertaVisualizacionPrecio.Archivo = new Archivo
                {
                    FileKey = FileKeys.PeticionDeOfertaVisualizacionPrecio,
                    Ruta = rutaArchivoRename

                };

                file.SaveAs(rutaArchivoRename);

            }
        }

        private bool ValidarVisualizarPrecio(int usuarioId, int peticionDeOfertaId)
        {
            return !repositorio.Listar<PeticionDeOfertaVisualizacionPrecio>(x => x.UsuarioCreador_Id == usuarioId && x.PeticionDeOferta_Id == peticionDeOfertaId).Any();
        }

        public ChatComprasDto ObtenerChat(int solpId, int usuarioActualId)
        {
            ChatComprasDto chat = new ChatComprasDto();
            var solp = repositorio.Obtener<Solp>(solpId);

            chat.Solp_Id = solp.Id;
            chat.FechaCreacion = solp.FechaCreacion.ToString("dd-MM-yyyy HH-mm-ss");
            chat.FechaCreacionDate = solp.FechaCreacion;
            chat.UsuarioActualId = usuarioActualId;

            //chat.Proveedores = peticion.Usuarios.Select(u => new ProveedorDto
            //{
            //    CUIT = u.Usuario.CUITRegistro,
            //    Mail = u.Usuario.Mail,
            //    RazonSocial = u.Usuario.ObtenerRazonSocial()
            //}).ToList();

            foreach (var item in solp.ChatInternoCompras.Where(l => l.Usuario_Id != usuarioActualId))
            {
                item.Leido = true;
            }
            repositorio.GuardarCambios();

            chat.Mensajes = solp.ChatInternoCompras.Select(m => new ChatInternoComprasDto
            {
                Id = m.Id,
                FechaEnvio = m.FechaEnvio.ToString("dd-MM-yyyy HH-mm-ss"),
                FechaEnvioDate = m.FechaEnvio,
                FechaDiaEnvio = m.FechaEnvio.ToString("ddd, d MMM"),
                Leido = m.Leido,
                Mail = m.Usuario.Mail,
                Mensaje = m.Mensaje,
                Solp_Id = m.Solp_Id,
                RolUsuario = m.Usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "COMPRADOR" : "SOLICITANTE",
                Usuario_Id = m.Usuario_Id
            }).ToList();

            return chat;
        }

        public Resultado GrabarMensajeChatInterno(ChatInternoComprasDto mensaje)
        {
            try
            {
                var resultado = new Resultado();
                var chatInternoCompras = new ChatInternoCompras()
                {
                    FechaEnvio = DateTime.Now,
                    Leido = false,
                    Mensaje = mensaje.Mensaje,
                    Solp_Id = mensaje.Solp_Id,
                    Usuario_Id = mensaje.Usuario_Id
                };

                repositorio.Agregar(chatInternoCompras);
                repositorio.GuardarCambios();

                resultado.IdEntidad = chatInternoCompras.Id;
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ExportarChatInternoAtexto(int solpId, string rutaArchivo)
        {
            try
            {
                var solp = repositorio.Obtener<Solp>(solpId);
                //var chatInterno = repositorio.Obtener<ChatComprasDto>(chat.PeticionDeOferta_Id);

                var txtFilename = $"ChatInterno-SOLP-{solp.NroSolp}-{DateTime.Now.ToString("yyyyMMdd")}.txt";
                var txtFilePath = $"{rutaArchivo}/{txtFilename}";

                using (StreamWriter sw = new StreamWriter(txtFilePath))
                {
                    // Escribir información general del chat
                    sw.WriteLine($"SOLP : {solp.Id}");
                    sw.WriteLine($"Fecha creacion: {solp.FechaCreacion}");
                    //sw.WriteLine($"Comprador: {solp.Usuario.Mail}");

                    // Escribir información de proveedores
                    //sw.WriteLine();
                    //sw.WriteLine("Proveedores:");
                    //foreach (var usuario in solp.Usuarios)
                    //{
                    //    sw.WriteLine($"CUIT: {usuario.Usuario.CUITRegistro}, Mail: {usuario.Usuario.Mail}, RazonSocial: {usuario.Usuario.ObtenerRazonSocial()}");
                    //}

                    // Escribir mensajes del chat
                    sw.WriteLine();
                    sw.WriteLine("Mensajes:");
                    foreach (var mensaje in solp.ChatInternoCompras)
                    {
                        sw.WriteLine($"{mensaje.FechaEnvio.ToString("dd-MM-yyyy HH:mm")} - {mensaje.Usuario.Mail} - {mensaje.Mensaje}");
                    }
                }
                return txtFilePath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProveedorComprasDto DevolverMonedaProveedor(string codigoProveedor)
        {
            var proveedorMoa = obtenerProveedorConsumerMOA.ObtenerProveedor(codigoProveedor);
            var proveedorDto = new ProveedorComprasDto();
            if (proveedorMoa == null)
            {
                return null;
            }
            else
            {
                proveedorDto.Moneda = proveedorMoa.CURRENCY;
            }

            return proveedorDto;
        }

        public bool ValidarSolpTratada(string nroSolp)
        {
            DateTime fechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString());
            DateTime fechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinConsultaSolp"].ToString());
            var filtros = new ObtenerSolpRequest
            {
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                NumeroSolp = nroSolp,
            };
            var tratada = false;
            try
            {
                if (!string.IsNullOrEmpty(nroSolp))
                {
                    var solpEntidad = repositorio.Obtener<Solp>(a => a.NroSolp == nroSolp);
                    var solp = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(filtros);
                    var cantidadPendienteSap = (decimal)0;
                    foreach (var item in solpEntidad.Posiciones)
                    {
                        cantidadPendienteSap += solp != null && solp.Posiciones.Count > 0 &&
                          solp.Posiciones.Any(x => Int32.Parse(x.NumeroPosicion) == item.Indice) ?
                          (solp.Posiciones.Where(x => Int32.Parse(x.NumeroPosicion) == item.Indice).FirstOrDefault().Ordered) : 0;
                    }

                    if (cantidadPendienteSap > 0)
                    {
                        tratada = (solpEntidad.Posiciones.Sum(x => x.Cantidad) - cantidadPendienteSap) <= 0;
                    }
                }
                return tratada;
            }
            catch (Exception e)
            {
                Logger.Log.Info($"ValidarSolpTratada" + nroSolp);
                Logger.Log.Error(e);
                throw;
            }
        }

        public List<RegionSap> ListarRegionesSap()
        {
            List<RegionSap> lista = repositorio.Listar<RegionSap>(x => x.CodigoPais == "AR").ToList();
            return lista;
        }

        public List<LiberadorSapDto> ListarLiberadorSap()
        {
            List<LiberadorSapDto> lista = repositorio.Listar<LiberadorSap, LiberadorSapDto>(x => new LiberadorSapDto
            {
                Id = x.Id,
                NombreCompleto = x.NombreCompleto,
                Cargo = x.Cargo,
                Obligatorio = x.Obligatorio
            }, x => x.Habilitado);
            return lista;
        }

        public List<TablaSapDto> ListarUnidadesDeMedida(string material)
        {
            List<TablaSapDto> resultado = new List<TablaSapDto>();
            List<TablaSapDto> todasLasUM = ObtenerTablaSap(TablasSap.Unidad);
            List<UnidadesDeMedida> unidadesDeMedidaSAP = obtenerUnidadesDeMedidaConsumerMOA.Request(new List<string> { material });
            var unidadesPorMaterialSAP = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == material).Select(x => x.UnidadDeMedida).ToList();
            resultado = todasLasUM.Where(x => unidadesPorMaterialSAP.Contains(x.Codigo)).ToList();

            return resultado;
        }

        private AdjudicacionEditarDto ConvertirAjudicacionDtoEnAdjudicacionSAP(AdjudicacionDto adjudicacionDto)
        {
            return new AdjudicacionEditarDto
            {
                NumeroOrdenDeCompra = adjudicacionDto.NumeroOrdenDeCompra,
                TextoDeCabecera = adjudicacionDto.TextoDeCabecera,
                CondicionesDeEntrega = adjudicacionDto.CondicionesDeEntrega,
                CondicionesDePago = adjudicacionDto.CondicionesDePago,
                Garantias = adjudicacionDto.Garantias,
                CondicionDePagoCodigo = adjudicacionDto.CondicionDePago.Codigo,
                PagoEn1 = adjudicacionDto.PagoEn1,
                PagoEn2 = adjudicacionDto.PagoEn2,
                PagoEn3 = adjudicacionDto.PagoEn3,
                PagoEn1Porcentaje = adjudicacionDto.PagoEn1Porcentaje,
                PagoEn2Porcentaje = adjudicacionDto.PagoEn2Porcentaje,
                CondicionDeImportacionCodigo = adjudicacionDto.CondicionDeImportacion.Codigo,
                CondicionDeImportacionComplemento = adjudicacionDto.CondicionDeImportacionDescripcion,
                MonedaCodigo = !string.IsNullOrEmpty(adjudicacionDto.MonedaCodigo) ? adjudicacionDto.MonedaCodigo : adjudicacionDto.AdjudicacionPosiciones.FirstOrDefault().MonedaCodigo,
                Posiciones = adjudicacionDto.AdjudicacionPosiciones.Select(posicionDto => new AdjudicacionPosicionEditarDto
                {
                    SubPosiciones = posicionDto.SubposicionesCompras?.Select(subPosicionDto => new AdjudicacionSubPosicionEditarDto
                    {
                        Indice = subPosicionDto.Numero,
                        Cantidad = subPosicionDto.Cantidad ?? 0,
                        Eliminado = subPosicionDto.Eliminado,
                        PrecioUnitario = subPosicionDto.PrecioBruto ?? 0
                    }).ToList(),
                    Indice = posicionDto.Indice ?? 0,
                    PrecioUnidadCodigo = posicionDto.PrecioUnidad ?? 0,
                    Eliminado = posicionDto.Eliminado,
                    RegionCodigo = posicionDto.RegionCodigo,
                    PaisCodigo = posicionDto.PaisSap,
                    FechaEntrega = posicionDto.FechaEntregaServicio.Value,
                    EntregaFinal = posicionDto.EntregaFinal,
                    Cantidad = posicionDto.Cantidad
                }).ToList()
            };
        }

        private bool HayModificacionCondicionesDePago(SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER original, AdjudicacionEditarDto adjudicacion)
        {
            return
                original.PMNTTRMS != adjudicacion.CondicionDePagoCodigo ||
                original.DSCNT1_TO != adjudicacion.PagoEn1 ||
                original.DSCNT2_TO != adjudicacion.PagoEn2 ||
                original.DSCNT3_TO != adjudicacion.PagoEn3 ||
                original.DSCT_PCT1 != adjudicacion.PagoEn1Porcentaje ||
                original.DSCT_PCT2 != adjudicacion.PagoEn2Porcentaje;
        }

        public ResultadoGenerico EditarOrdenDeCompra(AdjudicacionDto adjudicacionDto)
        {
            var adjudicacion = ConvertirAjudicacionDtoEnAdjudicacionSAP(adjudicacionDto);
            ResultadoGenerico resultadoEditarOC = new ResultadoGenerico();
            //adjudicacion = TestCompletarAdjudicacion(adjudicacion);
            //Consulta de OC en SAP
            var ocSap = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraRFC(adjudicacion.NumeroOrdenDeCompra);
            //Convertir OC de SAP a ModificarPedidoSAP
            ModificarPedidoSAP modificarPedidoSAP = new ModificarPedidoSAP
            {
                POACCOUNT = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNT
                {
                    PO_ITEM = a.PO_ITEM
                }).ToList(),
                POACCOUNTX = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNTX
                {
                    PO_ITEM = a.PO_ITEM
                }).ToList(),
                POADDRDELIVERY = ocSap.POADDRDELIVERY.Select(a => new BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = a.PO_ITEM,
                    POSTL_COD1 = a.POSTL_COD1,
                    CITY = a.CITY,
                    ADDR_NO = "",
                    NAME = a.NAME,
                    TEL1_NUMBR = "",
                    STREET = a.STREET,
                    STREET_NO = "",
                    REGION = a.REGION,
                    COUNTRY = a.COUNTRY
                }).ToList(),
                POCOND = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCOND
                {
                    ITM_NUMBER = x.ITM_NUMBER,  //el número de ítem al que corresponda la condición
                    COND_ST_NO = x.COND_ST_NO,
                    COND_TYPE = x.COND_TYPE,
                    COND_VALUE = x.COND_VALUE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = x.CURRENCY,
                    CHANGE_ID = "U",
                    //COND_COUNT = x.COND_COUNT,

                }).ToList(),
                POCONDX = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCONDX
                {
                    ITM_NUMBER = x.ITM_NUMBER,
                    ITM_NUMBERX = "X",
                    COND_ST_NO = "001",
                    COND_ST_NOX = "X",
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                    CONDITION_NOX = "X",
                }).ToList(),

                POHEADER = new BAPIMEPOHEADER(),
                POHEADERX = new BAPIMEPOHEADERX(),
                POITEM = ocSap.POITEM.Select(x => new BAPIMEPOITEM
                {
                    PO_ITEM = x.PO_ITEM,
                    PCKG_NO = x.PCKG_NO
                }).ToList(),
                POITEMX = ocSap.POITEM.Select(x => new BAPIMEPOITEMX
                {
                    PO_ITEM = x.PO_ITEM,
                }).ToList(),
                POSCHEDULE = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULE { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = x.DELIVERY_DATE }).ToList(),
                POSCHEDULEX = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULX { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = "X" }).ToList(),
                POSERVICES = ConvertirLista<BAPIESLLC>(ocSap.POSERVICES.ToList()),
                //POSRVACCESSVALUES = ocSap.POSRVACCESSVALUES.Select(x => new BAPIESKLC
                //{
                //    PCKG_NO = x.PCKG_NO,
                //    LINE_NO = x.LINE_NO,
                //    PERCENTAGE = x.PERCENTAGE,
                //    SERNO_LINE = x.SERNO_LINE,
                //    SERIAL_NO = x.SERIAL_NO,
                //    QUANTITY = x.QUANTITY,
                //    NET_VALUE = x.NET_VALUE,
                //    NET_VALUESpecified = true,
                //    PERCENTAGESpecified = true,
                //    QUANTITYSpecified = true,
                //}).ToList(),
                POTEXTHEADER = new List<BAPIMEPOTEXTHEADER>(),
                PURCHASEORDER = adjudicacion.NumeroOrdenDeCompra,

            };
            foreach (var bAPIESLLC in modificarPedidoSAP.POSERVICES)
            {
                bAPIESLLC.GR_PRICESpecified = true;
                bAPIESLLC.QUANTITYSpecified = true;
                bAPIESLLC.NET_VALUESpecified = true;
                bAPIESLLC.PRICE_UNITSpecified = true;
            }

            //Racional de compras
            var listaVaciaTexto = new string[] { "" };
            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };
            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAP.POTEXTHEADER.Add(new BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }

            }

            //Condición de Pago  - no funcionan para las  ZPE1 y ZDIR por que sap no lo permite
            List<string> condicionesNoEditables = new List<string>() { "ZPE1", "ZDIR" };
            if (!condicionesNoEditables.Contains(ocSap.POHEADER.DOC_TYPE))
            {
                modificarPedidoSAP.POHEADER.PMNTTRMS = adjudicacion.CondicionDePagoCodigo;
                modificarPedidoSAP.POHEADER.DSCNT1_TO = adjudicacion.PagoEn1;
                modificarPedidoSAP.POHEADER.DSCNT1_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT2_TO = adjudicacion.PagoEn2;
                modificarPedidoSAP.POHEADER.DSCNT2_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT3_TO = adjudicacion.PagoEn3;
                modificarPedidoSAP.POHEADER.DSCNT3_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT1 = adjudicacion.PagoEn1Porcentaje;
                modificarPedidoSAP.POHEADER.DSCT_PCT1Specified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT2 = adjudicacion.PagoEn2Porcentaje;
                modificarPedidoSAP.POHEADER.DSCT_PCT2Specified = true;
                modificarPedidoSAP.POHEADERX.PMNTTRMS = "X";
                modificarPedidoSAP.POHEADERX.DSCNT1_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT2_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT3_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT1 = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT2 = "X";
            }
            else
            {
                if (HayModificacionCondicionesDePago(ocSap.POHEADER, adjudicacion))// TODO: validar si edito alguna condicion de pago.
                {
                    resultadoEditarOC.Errores.Add(new ErrorMessage("Condición de Pago no se puede editar para las clase de documento  ZPE1 y ZDIR."));
                    return resultadoEditarOC;
                }
            }


            //Condición de Importacion
            if (!string.IsNullOrEmpty(adjudicacion.CondicionDeImportacionComplemento) && adjudicacion.CondicionDeImportacionComplemento.Length > 28)
            {
                resultadoEditarOC.Errores.Add(new ErrorMessage("Condicion de importacion muy largo, 28 caracteres maximo."));
                return resultadoEditarOC;
            }
            modificarPedidoSAP.POHEADER.INCOTERMS1 = adjudicacion.CondicionDeImportacionCodigo;
            modificarPedidoSAP.POHEADER.INCOTERMS2 = adjudicacion.CondicionDeImportacionComplemento;
            modificarPedidoSAP.POHEADERX.INCOTERMS1 = "X";
            modificarPedidoSAP.POHEADERX.INCOTERMS2 = "X";

            //Datos utiles
            bool esMateriales = ocSap.POITEM[0].ITEM_CAT == "0";
            bool modificoMoneda = ocSap.POHEADER.CURRENCY != adjudicacion.MonedaCodigo;

            //Modificar moneda
            if (modificoMoneda)
            {
                modificarPedidoSAP.POHEADER.CURRENCY = adjudicacion.MonedaCodigo;
                modificarPedidoSAP.POHEADERX.CURRENCY = "X";
            }

            foreach (var posAdj in adjudicacion.Posiciones)
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');

                //Región
                var direccioSap = modificarPedidoSAP.POADDRDELIVERY.Single(a => a.PO_ITEM == PO_ITEM);
                direccioSap.REGION = posAdj.RegionCodigo;
                direccioSap.COUNTRY = posAdj.PaisCodigo;

                //Fechas de entrega(por posición)
                var fechaEntregaSap = modificarPedidoSAP.POSCHEDULE.Single(a => a.PO_ITEM == PO_ITEM);
                var fechaEntregaSapX = modificarPedidoSAP.POSCHEDULEX.Single(a => a.PO_ITEM == PO_ITEM);
                fechaEntregaSap.DELIVERY_DATE = posAdj.FechaEntrega.ToString("dd.MM.yyyy");
                fechaEntregaSapX.DELIVERY_DATE = "X";

                //Posición
                var posicionSap = modificarPedidoSAP.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var posicionSapX = modificarPedidoSAP.POITEMX.Single(a => a.PO_ITEM == PO_ITEM);
                //Impitación
                var imputacionSap = new BAPIMEPOACCOUNT();
                var imputacionSapX = new BAPIMEPOACCOUNTX();
                if (modificarPedidoSAP.POACCOUNT.Count > 0)
                {
                    imputacionSap = modificarPedidoSAP.POACCOUNT.Single(a => a.PO_ITEM == PO_ITEM);
                    imputacionSapX = modificarPedidoSAP.POACCOUNTX.Single(a => a.PO_ITEM == PO_ITEM);
                }
                //Condición
                var condicionSap = modificarPedidoSAP.POCOND.Single(a => a.ITM_NUMBER == "0" + PO_ITEM);

                //Eliminar posición
                posicionSap.DELETE_IND = posAdj.Eliminado ? "X" : "";
                posicionSapX.DELETE_IND = "X";

                //Tilde entrega final
                posicionSap.NO_MORE_GR = posAdj.EntregaFinal ? "X" : "";
                posicionSapX.NO_MORE_GR = "X";

                if (esMateriales)
                {
                    bool modificoImporte = ModificoImporte(ocSap, adjudicacion);

                    //Modificar cantidad
                    posicionSap.QUANTITY = posAdj.Cantidad;
                    posicionSap.QUANTITYSpecified = true;
                    posicionSapX.QUANTITY = "X";
                    if (modificarPedidoSAP.POACCOUNT.Count > 0)
                    {
                        imputacionSap.QUANTITY = posAdj.Cantidad;
                        imputacionSapX.QUANTITY = "X";
                    }

                    //Modificar importe
                    if (modificoImporte)
                    {
                        posicionSap.NET_PRICE = posAdj.PrecioUnidadCodigo;
                        posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        condicionSap.COND_VALUE = posAdj.PrecioUnidadCodigo;
                        condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        //"ZP01" no deja cambiar importes por eso se cambia a "ZP00"
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP00" : condicionSap.COND_TYPE;
                    }
                }
                else
                {

                    if (!posAdj.Eliminado)
                    {
                        foreach (var subPosAdj in posAdj.SubPosiciones)
                        {
                            //Subposición
                            string LINE_NO = subPosAdj.Indice.ToString().PadLeft(10, '0');
                            var subPosicionSap = modificarPedidoSAP.POSERVICES.First(a => a.LINE_NO == LINE_NO);
                            //var imputacionSubPos = modificarPedidoSAP.POSRVACCESSVALUES.First(a => a.LINE_NO == LINE_NO);//

                            //Eliminar subposición 
                            subPosicionSap.DELETE_IND = subPosAdj.Eliminado ? "X" : "";
                            if (subPosAdj.Eliminado) continue;
                            //Cantidad
                            subPosicionSap.QUANTITY = subPosAdj.Cantidad;
                            //imputacionSubPos.QUANTITY = subPosAdj.Cantidad;
                            //Importe 1/2                            
                            subPosicionSap.GR_PRICE = subPosAdj.PrecioUnitario;
                            subPosicionSap.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;
                            //imputacionSubPos.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;

                        }

                        //Importe 2/2                        
                        var totalPosicion = posAdj.SubPosiciones.Where(a => a.Eliminado != true).Sum(a => a.Cantidad * a.PrecioUnitario);
                        condicionSap.COND_VALUE = totalPosicion;
                        condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP01" : condicionSap.COND_TYPE;

                        posicionSap.NET_PRICE = totalPosicion;
                        posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        //imputacionSap.NET_VALUE = totalPosicion;
                        //imputacionSap.NET_VALUESpecified = true;
                        //imputacionSapX.NET_VALUE = "X";

                    }

                }
            }

            if (!esMateriales) // para servicios guille nos dijo que no lo enviemos pero para materiales si lo necesitamos enviar.
            {
                modificarPedidoSAP.POACCOUNT = new List<BAPIMEPOACCOUNT>();
                modificarPedidoSAP.POACCOUNTX = new List<BAPIMEPOACCOUNTX>();
            }

            // Llama al WebService
            var resultadoSAP = modificarOrdenDeCompraConsumerMOA.EditarPedidoRequest(modificarPedidoSAP);
            resultadoSAP.Where(a => a.MESSAGE == "No se han modificado datos").ToList().ForEach(a => a.TYPE = "E");


            foreach (var item in resultadoSAP.Where(x => x.TYPE == "E"))
            {
                resultadoEditarOC.Error(item.TYPE, item.MESSAGE);
            }

            return resultadoEditarOC;
        }

        private AdjudicacionEditarDto TestCompletarAdjudicacion(AdjudicacionEditarDto adjudicacion)
        {
            //var nroOC = "4123002022";//materiales
            var nroOC = "4123002087";//servicios

            adjudicacion = new AdjudicacionEditarDto();
            adjudicacion.NumeroOrdenDeCompra = nroOC;

            if (nroOC == "4123002022")
            {
                adjudicacion.MonedaCodigo = "ARP";
                adjudicacion.TextoDeCabecera = "Prueba1";
                adjudicacion.CondicionesDeEntrega = "condiciones de entrega";
                adjudicacion.CondicionesDePago = "condiciones de pago";
                adjudicacion.Garantias = "garantias";
                adjudicacion.CondicionDePagoCodigo = "0060";
                adjudicacion.PagoEn1 = 60;
                adjudicacion.PagoEn2 = 0;
                adjudicacion.PagoEn3 = 0;
                adjudicacion.PagoEn1Porcentaje = 0;
                adjudicacion.PagoEn2Porcentaje = 0;
                adjudicacion.CondicionDeImportacionCodigo = "";
                adjudicacion.CondicionDeImportacionComplemento = "";
                //adjudicacion.CondicionDeImportacion = "CIW";
                //adjudicacion.CondicionDeImportacionComplemento = "Opcional";
                adjudicacion.Posiciones.Add(new AdjudicacionPosicionEditarDto
                {
                    Indice = 1,
                    PrecioUnidadCodigo = 5000,
                    Cantidad = 5,
                    PaisCodigo = "AR",
                    RegionCodigo = "22",
                    Eliminado = false,
                    EntregaFinal = false,
                    FechaEntrega = new DateTime(2024, 02, 01)
                });

            }


            if (nroOC == "4123002087")
            {
                adjudicacion.MonedaCodigo = "USD";
                adjudicacion.TextoDeCabecera = "Prueba1";
                adjudicacion.CondicionesDeEntrega = "condiciones de entrega";
                adjudicacion.CondicionesDePago = "condiciones de pago";
                adjudicacion.Garantias = "garantias";
                //adjudicacion.CondicionDePagoCodigo = "0060";
                //adjudicacion.PagoEn1 = 60;
                //adjudicacion.PagoEn2 = 0;
                //adjudicacion.PagoEn3 = 0;
                //adjudicacion.PagoEn1Porcentaje = 0;
                //adjudicacion.PagoEn2Porcentaje = 0;
                //adjudicacion.CondicionDeImportacionCodigo = "";
                //adjudicacion.CondicionDeImportacionComplemento = "";
                //adjudicacion.CondicionDeImportacionCodigo = "CIW";
                //adjudicacion.CondicionDeImportacionComplemento = "Opcional";

                adjudicacion.Posiciones.Add(new AdjudicacionPosicionEditarDto
                {
                    Indice = 1,
                    PrecioUnidadCodigo = 0,
                    Cantidad = 1,
                    PaisCodigo = "AR",
                    RegionCodigo = "22",
                    Eliminado = false,
                    EntregaFinal = false,
                    FechaEntrega = new DateTime(2024, 02, 01),
                    SubPosiciones = new List<AdjudicacionSubPosicionEditarDto> {
                        new AdjudicacionSubPosicionEditarDto{
                            Indice = 3,
                            Cantidad = 2,
                            PrecioUnitario = 2000,
                            Eliminado = false,
                        },
                    }
                });
            }
            return adjudicacion;
        }

        private bool ModificoImporte(ResultBAPI_PO_GETDETAIL1 ocSap, AdjudicacionEditarDto adjudicacion)
        {
            foreach (var posAdj in adjudicacion.Posiciones.Where(a => !a.Eliminado))
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');
                var posicionSap = ocSap.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var modifico = posAdj.PrecioUnidadCodigo != posicionSap.NET_PRICE;
                if (modifico) return true;

            }
            return false;
        }

        private bool ModificoCantidad(ResultBAPI_PO_GETDETAIL1 ocSap, AdjudicacionEditarDto adjudicacion)
        {
            foreach (var posAdj in adjudicacion.Posiciones.Where(a => !a.Eliminado))
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');
                var posicionSap = ocSap.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var SUBPCKG_NO = ocSap.POSERVICES.First(a => a.PCKG_NO == posicionSap.PCKG_NO).SUBPCKG_NO;
                var servicios = ocSap.POSERVICES.Where(a => a.PCKG_NO == SUBPCKG_NO);
                var modifico = posAdj.SubPosiciones.Where(a => !a.Eliminado).Sum(a => a.PrecioUnitario * a.Cantidad) != servicios.Sum(a => a.GR_PRICE * a.QUANTITY);
                if (modifico) return true;
            }
            return false;
        }

        public List<TDestino> ConvertirLista<TDestino>(IEnumerable<object> listaOrigen)
           where TDestino : class, new()
        {
            // Utiliza reflexión para copiar propiedades automáticamente
            return listaOrigen.Select(item => ConvertirObjeto<TDestino>(item)).ToList();
        }

        private TDestino ConvertirObjeto<TDestino>(object objetoOrigen)
            where TDestino : class, new()
        {
            // Crea una instancia del tipo de destino
            var objetoDestino = Activator.CreateInstance<TDestino>();

            // Obtiene las propiedades de ambos tipos
            PropertyInfo[] propiedadesOrigen = objetoOrigen.GetType().GetProperties();
            PropertyInfo[] propiedadesDestino = typeof(TDestino).GetProperties();

            // Copia los valores de las propiedades automáticamente
            foreach (var propiedadOrigen in propiedadesOrigen)
            {
                PropertyInfo propiedadDestino = propiedadesDestino.FirstOrDefault(p => p.Name == propiedadOrigen.Name);

                if (propiedadDestino != null && propiedadDestino.PropertyType == propiedadOrigen.PropertyType)
                {
                    // Copia el valor de la propiedad del objeto de origen al objeto de destino
                    propiedadDestino.SetValue(objetoDestino, propiedadOrigen.GetValue(objetoOrigen));
                }
            }

            return objetoDestino;
        }

        private void ActualizarDatosSolp(List<string> nroOrdenDeCompra, Solp solp)
        {
            try
            {
                var respuestaGuardarSOLP = new RespuestaGuardarSOLP { Solp = new SolpDto { NroSolp = solp.NroSolp } };
                var proveedores = repositorio.Listar<Usuario>();
                if (nroOrdenDeCompra.Count > 0)
                {
                    foreach (var nro in nroOrdenDeCompra)
                    {
                        var ordenDeCompra = ObtenerOrdenDeCompra(nro);
                        Log.Info("ActualizarDatosSolp ObtenerOrdenDeCompra" + ordenDeCompra.ToJson());
                        var proveedor = ObtenerProveedorCompras(ordenDeCompra.Cabecera.CodigoProveedor);
                        foreach (var posicionOCSap in ordenDeCompra.Posiciones.Where(x => x.NroSolp == solp.NroSolp))
                        {
                            var posicionSolp = solp.Posiciones.Where(x => x.Indice == Int32.Parse(posicionOCSap.IndiceSolp)).FirstOrDefault();
                            posicionSolp.ProveedorAdjudicado_Id = proveedor.Usuario_Id;
                            posicionSolp.ProveedorAdjudicado = proveedores.Where(x => x.Id == proveedor.Usuario_Id).FirstOrDefault();
                            posicionSolp.RegistroInfoNro = posicionOCSap.RegistroInfo;
                            posicionSolp.OrganizacionDeComprasCodigo = ordenDeCompra.Cabecera.OrganizacionDeComprasCodigo;
                        }
                    }

                    FinalizarSolp(solp, null, respuestaGuardarSOLP, false);
                }
            }
            catch (Exception e)
            {
                Log.Info($"ActualizarDatosSolp " + solp.NroSolp);
                Log.Error(e);
                throw;
            }
        }

        public InfoVisitasDeObraDto ListarVisitasDeObra(List<VisitaObraDto> visitas)
        {
            var fechas = visitas.Select(x => x.FechaHora.Date).ToList();

            var detalleVisitas = repositorio.Listar<PliegoVisita, DetalleVisitaDto>(pliegoVisita => new DetalleVisitaDto
            {
                FechaHora = pliegoVisita.FechaHora,
                PliegoId = pliegoVisita.Pliego_Id
            })
              .AsEnumerable()
              .Where(vis => vis.FechaHora.HasValue && fechas.Any(f => vis.FechaHora.Value.Date == f))
              .ToList();

            var listaIdPliego = detalleVisitas.Select(x => x.PliegoId).ToList();

            var solpDB = repositorio.Listar<Solp, SolpDto>(x => new SolpDto
            {
                NroSolp = x.NroSolp,
                Pliego_Id = x.Pliego_Id,
                Id = x.Id
            }, so => so.Pliego_Id.HasValue && listaIdPliego.Contains(so.Pliego_Id.Value)).ToList();

            var solpIds = solpDB.Select(x => x.Id).ToList();

            var po = repositorio.Listar<PeticionDeOferta>(peticion => solpIds.Contains(peticion.Solp_Id)).ToList();

            foreach (var detalle in detalleVisitas)
            {
                detalle.NroSolp = solpDB.Where(solp => solp.Pliego_Id == detalle.PliegoId).FirstOrDefault() != null ? solpDB.Where(solp => solp.Pliego_Id == detalle.PliegoId).FirstOrDefault().NroSolp : "";
                detalle.Proveedores = po.Where(pou => pou.Solp_Id == solpDB.Where(solp => solp.Pliego_Id == detalle.PliegoId).FirstOrDefault().Id) != null ?
                 po.Where(pou => pou.Solp_Id == solpDB.Where(solp => solp.Pliego_Id == detalle.PliegoId).FirstOrDefault().Id)
                    .SelectMany(pro => pro.Usuarios).Select(peticionUsuario => new ProveedorDto
                    {
                        RazonSocial = peticionUsuario.Usuario.ObtenerRazonSocial(),
                        Mail = peticionUsuario.Usuario.Mail
                    }).Distinct().ToList() : null;
            }

            var info = new InfoVisitasDeObraDto()
            {
                CantidadVisitas = detalleVisitas.Count(),
                DetalleVisitas = detalleVisitas
            };


            return info;
        }

        private void SetNombreDePedido(Solp solp)
        {
            if (string.IsNullOrEmpty(solp.Pliego.NombreObra) || solp.TipoSolpSap != (int)TipoSolpSap.Web || solp.TipoSolp == null || solp.TipoSolp.Codigo == "SIN_PLIEGO")
            {
                string nombre = solp.Posiciones.Count > 2 ? string.Join(" + ", solp.Posiciones.Take(2).Select(x => x.Tarea)) + " + Otros" :
                                string.Join(" + ", solp.Posiciones.Select(x => x.Tarea));
                solp.Pliego.NombreObra = nombre;
            }
        }

        private string ConfigurarPrefijos(Solp solp)
        {
            var prefijo = "";

            if (solp.Adicional == true)
            {
                prefijo = "AD: ";
            }

            if (solp.TrabajoYaHecho == true && solp.Adicional == true)
            {
                prefijo = "AD-OR: ";
            }

            if (solp.TrabajoYaHecho == true && solp.Urgencia == true || solp.Urgencia == true)
            {
                prefijo = "URG: ";
            }

            if (solp.TrabajoYaHecho == true)
            {
                prefijo = "TR: ";
            }

            if (solp.TrabajoYaHecho == true && solp.THServicioPermanente == true)
            {
                prefijo = "SP: ";
            }

            if (solp.TrabajoYaHecho == true && solp.THAjustePolinomica == true)
            {
                prefijo = "AJ: ";
            }

            if (solp.TrabajoYaHecho == true && solp.THProveedorDirecto == true)
            {
                prefijo = "PD: ";
            }

            if (solp.CondEspProveedorAsignado == true)
            {
                prefijo = "PA: ";
            }

            return prefijo;

        }

        public void ObtenerDatosReporteSolp()
        {

            DateTime startDate = new DateTime(2023, 9, 1);
            DateTime endDate = DateTime.Now.Date;

            var resultadoFinal = new List<SolpMailDto>();

            while (startDate < endDate)
            {
                DateTime startOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1);

                var periodo = startOfMonth.ToString("yyyy-MM");
                var resultadoTemporal = repositorio.Listar<Solp, SolpDto>(
                    x => new SolpDto
                    {
                        TipoDeSolp = ((TipoSolpSap)x.TipoSolpSap).ToString(),
                        UsuarioCreadorMail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "Sin usuario creador",
                        Periodo = periodo,
                        NroSolp = x.NroSolp,
                        FechaCreacion = x.FechaCreacion
                    },
                    s => s.TipoSolpSap.HasValue && s.FechaCreacion >= startOfMonth && s.FechaCreacion < endOfMonth && !string.IsNullOrEmpty(s.NroSolp),
                    maxResultados: 0,
                    orden: null,
                    direccionOrden: DirOrden.Asc
                );

                resultadoFinal.AddRange(resultadoTemporal
                    .GroupBy(x => new { x.TipoDeSolp, x.UsuarioCreadorMail, x.Periodo })
                    .Select(g => new SolpMailDto
                    {
                        TipoSolp = g.Key.TipoDeSolp,
                        UsuarioCreadorMail = g.Key.UsuarioCreadorMail,
                        Cantidad = g.Count().ToString(),
                        Periodo = g.Key.Periodo
                    })
                    .ToList());

                startDate = startDate.AddMonths(1);
            }
            var outputMemStream = new MemoryStream();
            MemoryStream streamExcel = ExcelExport.CreateExcelFileMs(resultadoFinal, new string[] { "Origen", "Usuario", "Cantidad", "Periodo" });

            var nombreArchivoXls = $"Reporte SOLPs {DateTime.Today:dd-MM-yyyy}.xlsx";
            Attachment archivoExcel;
            archivoExcel = new Attachment(streamExcel, nombreArchivoXls);

            EnviarMailReporteSolp(streamExcel.ToArray(), nombreArchivoXls);

        }


        private void EnviarMailReporteSolp(byte[] archivoExcel, string archivo)
        {
            try
            {
                var asunto = $"Reporte SOLPs";

                var enviarA = new List<string>();

                var mail = ConfigurationManager.AppSettings["EmailRerporteSolpTo"];

                if (mail.Contains(","))
                {
                    foreach (var destinatario in mail.Split(','))
                    {
                        enviarA.Add(destinatario);
                    }
                }
                else
                {
                    enviarA.Add(mail);
                }

                emailService.EnviarMail(enviarA, asunto, "", null, CuerpoMailReporteSolp(), archivoExcel, archivo);
            }
            catch (Exception e)
            {
                Log.Info($"Error al enviar mail reporte sap");
                Log.Error(e);
            }
        }

        private AlternateView CuerpoMailReporteSolp()
        {

            string htmlBody = "";
            htmlBody += $"En el presente mail se informa las solps generadas en SAP y en la WEB. <br />";
            htmlBody += "<br/>" +
                "Equipo Compras";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            return alternateView;
        }


        public static class SolpTemplateKeys
        {
            public const string FECHA_LIBERACION = "FECHA_LIBERACION";
            public const string FECHA_CREACION = "FECHA_CREACION";
            public const string NOMBRE_OBRA = "NOMBRE_OBRA";
            public const string NRO_SOLP = "NRO_SOLP";
            public const string NRO_PEDIDO = "NRO_PEDIDO";
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
}