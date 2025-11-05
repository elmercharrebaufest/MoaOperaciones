// Ignore Spelling: Solp href noopener noreferrer pdf img ciberseguridad Posicion Imputacion Descripcion Almacen paginacion nro username Licitacion Cotizacion Condicion

using DocumentFormat.OpenXml;
using HandlebarsDotNet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Newtonsoft.Json;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.POMultiple;
using SustitucionMOAModel.Dto.Compras.PrecargaSolp;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Extensions;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services.Email.Dto;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Web;


namespace SustitucionMOAUtils.Services
{
    public class ComprasService : IComprasService
    {
        private readonly IRepositorioCompras repositorio;
        private readonly IVendedorService vendedorService;
        private readonly IHttpContextService httpContextService;
        private readonly IUsuarioService usuarioService;

        private readonly string rutaArchivosCompras = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly string EmailEnvioErrores = ConfigurationManager.AppSettings["EmailEnvioErrores"];

        private readonly IEmailService emailService;
        private readonly IEmailComprasService emailComprasService;
        private readonly IComprasArchivosService comprasArchivosService;
        private readonly IComprasArchivosImportService comprasArchivosImportService;

        private readonly IComprasSapService comprasServiceSap;
        private readonly ITipoCambioService tipoCambioService;
        private readonly IUnidadMedidaService unidadMedidaService;
        private readonly IRegistroInfoService registroInfoService;
        private readonly ITablaSapService tablaSapService;

        public ComprasService(IRepositorioCompras repositorioCompras,
            IVendedorService vendedorService,
            IHttpContextService httpContextService,
            IUsuarioService usuarioService,
            IEmailService emailService,
            IEmailComprasService emailComprasService,
            IComprasArchivosService comprasArchivosService,
            IComprasArchivosImportService comprasArchivosImportService,
            IComprasSapService comprasServiceSap,
            ITipoCambioService tipoCambioService,
            IUnidadMedidaService unidadMedidaService,
            IRegistroInfoService registroInfoService,
            ITablaSapService tablaSapService)
        {
            this.repositorio = repositorioCompras;
            this.vendedorService = vendedorService;
            this.httpContextService = httpContextService;
            this.usuarioService = usuarioService;
            this.emailService = emailService;
            this.emailComprasService = emailComprasService;
            this.comprasArchivosService = comprasArchivosService;
            this.comprasArchivosImportService = comprasArchivosImportService;
            this.comprasServiceSap = comprasServiceSap;
            this.tipoCambioService = tipoCambioService;
            this.unidadMedidaService = unidadMedidaService;
            this.registroInfoService = registroInfoService;
            this.tablaSapService = tablaSapService;
        }

        public Pliego GuardarPliego(SolpDto solp,
                                     HttpFileCollectionBase adjuntos,
                                     bool condEsp,
                                     string rutaArchivos = null,
                                     Pliego pliegoEntity = null,
                                     bool esPliegoMultiple = false)
        {
            pliegoEntity = pliegoEntity ?? new Pliego();

            pliegoEntity.RevisadoPor = solp.RevisadoPor;
            pliegoEntity.Usuario_Id = solp.UsuarioActual.Id;
            pliegoEntity.FechaModificacion = DateTime.Now; //FechaAlta es valor predeterminado en clase Pliego

            pliegoEntity.FiscalContrato = solp.FiscalContrato;
            pliegoEntity.Telefono = solp.Telefono;
            pliegoEntity.Email = solp.Email;
            pliegoEntity.FechaHoraEntrega = solp.FechaHoraEntrega?.ToLocalTime();
            pliegoEntity.SupervisorSector = solp.SupervisorSector != null ? string.Join(",", solp.SupervisorSector.Select(x => x)) : string.Empty;
            pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo;
            pliegoEntity.TieneVisitaObraMasiva = solp.TieneVisitaObraMasiva;
            pliegoEntity.TieneObradores = solp.TieneObradores;
            pliegoEntity.TieneMedioElevacion = solp.TieneMedioElevacion;
            pliegoEntity.TieneAndamio = solp.TieneAndamio;
            pliegoEntity.TieneGrillaPersonal = solp.TieneGrillaPersonal;
            pliegoEntity.TieneFabricacionTallerExterno = solp.TieneFabricacionTallerExterno;
            pliegoEntity.TieneTecnicoSeguridad = solp.TieneTecnicoSeguridad;
            pliegoEntity.TieneDescripcionTecnica = solp.TieneDescripcionTecnica;
            pliegoEntity.TieneDocumentacionTecnica = solp.TieneDocumentacionTecnica;
            pliegoEntity.RequisitoCiberseguridad = solp.RequisitoCiberseguridad;
            pliegoEntity.FechaHoraLimiteConsulta = solp.FechaHoraLimiteConsulta?.ToLocalTime();
            pliegoEntity.ObservacionesGeneracion = solp.ObservacionesGeneracion;
            pliegoEntity.DiasEjecucion = solp.DiasEjecucion;
            pliegoEntity.JornadaLaboralDias = solp.JornadaLaboral != null ? string.Join(",", solp.JornadaLaboral.Select(x => (int)x)) : string.Empty;
            pliegoEntity.JornadaLaboralHorasDesde = solp.JornadaLaboralDesde?.ToLocalTime();
            pliegoEntity.JornadaLaboralHorasHasta = solp.JornadaLaboralHasta?.ToLocalTime();
            pliegoEntity.ObservacionesCotizacion = solp.ObservacionesCotizacion;
            pliegoEntity.ObservacionesCotizacionCondEsp = solp.ObservacionesCotizacionCondEsp;
            pliegoEntity.TieneCondicionesGenerales = solp.TieneCondicionesGenerales ?? true;
            pliegoEntity.RevisadoPor = solp.RevisadoPor;
            pliegoEntity.MultipleFinalizado = solp.MultipleFinalizado;

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

                foreach (PliegoVisita visita in visitasBorrar)
                {
                    repositorio.Remover(visita);
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

            // Identifica los que ya no están en la base de datos y los borra
            if (solp.Adjuntos != null && pliegoEntity.Archivos != null)
            {
                var archivosParaBorrar = pliegoEntity.Archivos
                    .Where(x => !solp.Adjuntos.Select(y => y.Id).Contains(x.Id))
                    .ToList();

                foreach (Archivo archivo in archivosParaBorrar)
                {
                    repositorio.Remover(archivo);
                }
            }
            if (pliegoEntity.Archivos == null)
            {
                pliegoEntity.Archivos = new List<Archivo>();
            }

            if (condEsp)
            {
                string justificacionTexto = "Justificación de condición especial: " + pliegoEntity.ObservacionesCotizacionCondEsp;

                if (!string.IsNullOrEmpty(pliegoEntity.ObservacionesGeneracion))
                {
                    int index = pliegoEntity.ObservacionesGeneracion.IndexOf("Justificación de condición especial:");
                    if (index != -1)
                    {
                        pliegoEntity.ObservacionesGeneracion = pliegoEntity.ObservacionesGeneracion.Substring(0, index).TrimEnd();
                    }

                    pliegoEntity.ObservacionesGeneracion += "\n\n" + justificacionTexto;
                }
                else
                {
                    pliegoEntity.ObservacionesGeneracion = justificacionTexto;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pliegoEntity.ObservacionesGeneracion))
                {
                    const string fraseABuscar = "Justificación de condición especial:";

                    int indice = pliegoEntity.ObservacionesGeneracion.IndexOf(fraseABuscar);
                    if (indice != -1)
                    {
                        pliegoEntity.ObservacionesGeneracion = pliegoEntity.ObservacionesGeneracion.Substring(0, indice);
                    }
                    pliegoEntity.ObservacionesCotizacionCondEsp = null;
                }
                if (pliegoEntity.Archivos.Any(a => a.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp))
                {
                    repositorio.RemoverTodos(pliegoEntity.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp).ToList());
                }
            }

            repositorio.GuardarCambios();

            if (esPliegoMultiple)
            {
                if (string.IsNullOrWhiteSpace(rutaArchivos))
                {
                    //override filename
                    rutaArchivos = ObtenerRutaArchivos(pliegoEntity.Id, "PliegoMultiple");
                }

                pliegoEntity.Multiple = true;
            }

            pliegoEntity = GuardarEspecificacionesTecnicasPliego(solp, rutaArchivos, pliegoEntity);
            if (!esPliegoMultiple)
            {
                solp = GuardarAdjuntosSolp(solp, adjuntos, pliegoEntity);
            }
            else
            {
                GuardarAdjuntosPliegoMultiple(solp, adjuntos, pliegoEntity);
            }

            repositorio.GuardarCambios();
            return pliegoEntity;
        }

        public RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
        {
            Solp solpEntity = null;

            bool enviarMailUrgencia = solp.Urgencia == true && solp.Finalizar && solp.TrabajoYaHecho != true;

            if (solp.Id.HasValue)
            {
                //es la forma de decirle a entity framework que tambien me traiga todas estas cosas
                var includes = new List<Expression<Func<Solp, object>>>
                {
                    x => x.Pliego,
                    x => x.Pliego.VisitasMasivas,
                    x => x.Pliego.Archivos,
                    x => x.Posiciones,
                    x => x.Posiciones.Select(y => y.Subposiciones),
                    x => x.UsuarioCreacion,
                    x => x.UsuarioModificacion
                };

                solpEntity = repositorio.Obtener<Solp>(solp.Id.Value);
                ExistenPosicionesNuevas(solp, solpEntity);
                if (solpEntity != null)
                {
                    if (enviarMailUrgencia && !string.IsNullOrEmpty(solpEntity.NroSolp))
                    {
                        enviarMailUrgencia = false;
                        if (solp.Posiciones.Count > solpEntity.Posiciones.Count)
                        {
                            enviarMailUrgencia = true;
                        }
                        else
                        {
                            if (solpEntity.Posiciones.Where(a => a.TipoPosicion_Id != null).FirstOrDefault()?.TipoPosicion.Codigo == "SERVICIO")
                            {
                                for (int i = 0; i < solpEntity.Posiciones.Count; i++)
                                {
                                    if (solp.Posiciones[i].Subposiciones.Count > solpEntity.Posiciones.ToList()[i].Subposiciones.Count)
                                    {
                                        enviarMailUrgencia = true;
                                    }
                                    else
                                    {
                                        for (int j = 0; j < solpEntity.Posiciones.ToList()[i].Subposiciones.Count; j++)
                                        {
                                            if (solp.Posiciones[i].Subposiciones[j].Cantidad > solpEntity.Posiciones.ElementAt(i).Subposiciones.ElementAt(j).Cantidad
                                                || solp.Posiciones[i].Subposiciones[j].PrecioBruto > solpEntity.Posiciones.ElementAt(i).Subposiciones.ElementAt(j).PrecioBruto)
                                            {
                                                enviarMailUrgencia = true;
                                            }
                                        }
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
            }
            else
            {
                var estadoIncompletoCodigo = EstadoDocumentoSolp.Incompleto.Code();
                var estadoIncompleto = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoIncompletoCodigo);

                solpEntity = new Solp()
                {
                    UsuarioCreacion_Id = solp.UsuarioActual.Id,
                    FechaCreacion = DateTime.Now,
                    EmailLinkToken = Guid.NewGuid(),
                    TieneModificaciones = false,
                    EstadoDocumento_Id = estadoIncompleto.Id,
                    Pliego = new Pliego(),
                    Posiciones = new List<SolpPosicion>(),
                    TrabajoYaHecho = solp.TrabajoYaHecho,
                    ConPresupuesto = solp.ConPresupuesto,
                    SeraUsadoEnPliegoMultiple = solp.SeraUsadoEnPliegoMultiple,
                    CertificacionAutomatica = solp.CertificacionAutomatica,
                    CondEspProveedorAsignado = solp.CondEspProveedorAsignado,
                    Adicional = solp.Adicional,
                    Urgencia = solp.Urgencia,
                    NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                    ProveedorAsignado_Id = solp.ProveedorAsignado_Id,
                    NroSolp = solp.NroSolp,
                    THAjustePolinomica = solp.THAjustePolinomica,
                    THProveedorDirecto = solp.THProveedorDirecto,
                    THServicioPermanente = solp.THServicioPermanente,
                    AdmiteCertificacionesParciales = solp.AdmiteCertificacionesParciales,
                };

                solp.TipoSolpSap = (int)TipoSolpSap.Web;
                repositorio.Agregar(solpEntity);
            }

            if (solpEntity != null)
            {
                if (solp.ClaseDocumento != null)
                {
                    solpEntity.ClaseDocumento = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.ClaseDocumento && x.Codigo == solp.ClaseDocumento.Codigo);
                }

                if (solp.TipoSolp != null)
                {
                    solpEntity.TipoSolp = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoSolp && x.Codigo == solp.TipoSolp.Codigo);
                }
                solpEntity.PasoCompletado = solp.PasoCompletado;
                solpEntity.UsuarioCompras_Id = solp.UsuarioCompras.Id;
                solpEntity.EstadoPasos = solp.EstadoPasos;
                solpEntity.TipoSolpSap = solp.TipoSolpSap;
                solpEntity.TrabajoYaHecho = solp.TrabajoYaHecho;
                solpEntity.CertificacionAutomatica = solp.CertificacionAutomatica;
                solpEntity.AdmiteCertificacionesParciales = solp.AdmiteCertificacionesParciales;
                solpEntity.CondEspProveedorAsignado = solp.CondEspProveedorAsignado;
                solpEntity.ConPresupuesto = solp.ConPresupuesto;
                solpEntity.SeraUsadoEnPliegoMultiple = solp.SeraUsadoEnPliegoMultiple;
                solpEntity.Adicional = solp.Adicional;
                solpEntity.Urgencia = solp.Urgencia;
                solpEntity.NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional;
                solpEntity.ProveedorAsignado_Id = solp.ProveedorAsignado_Id;
                solpEntity.THAjustePolinomica = solp.THAjustePolinomica;
                solpEntity.THProveedorDirecto = solp.THProveedorDirecto;
                solpEntity.THServicioPermanente = solp.THServicioPermanente;
                solpEntity.Pliego.NombreObra = solp.NombreDeObra;

                solpEntity.EnvioCircularA = EnviarCircularEnum.NoEnviar; /* el se marca con el valor definitivo en GuardarEnvioCircularProveedor
                                                                               * (llamar desde el front)
                                                                               */

                solpEntity = PosicionesEliminar(solpEntity, solp);
                var codigos = solpEntity.Posiciones.Select(x => x.Codigo).ToList();

                // eliminar posiciones que no se grabaron (Eliminadas en el front). se fija que el estado este en false (osea borrado) y que esas posiciones no existan en la db
                solp.Posiciones = solp.Posiciones.Where(a => a.Estado || codigos.Contains(a.Codigo)).ToList();

                if (solp.Posiciones != null)
                {
                    solpEntity = PosicionesNuevasActualizadas(solpEntity, solp);
                }
            }

            if (solpEntity.LiberadoresSapSolp.Any())
                repositorio.RemoverTodos(solpEntity.LiberadoresSapSolp.ToList());
            if (solp.TrabajoYaHecho != true && !solp.ConPresupuesto && solp.LiberadoresSapSolp.Any())
            {
                solpEntity.LiberadoresSapSolp = solp.LiberadoresSapSolp.ConvertAll(dto => new LiberadorSapSolp
                {
                    LiberadorSap_Id = dto.LiberadorSap_Id
                });
            }

            bool condEsp = TieneCondicionEspecial(solpEntity);

            if (condEsp)
            {
                bool esServicio = solpEntity.Posiciones.Any() && solpEntity.Posiciones.FirstOrDefault()?.TipoPosicion != null && solpEntity.Posiciones.FirstOrDefault()?.TipoPosicion.Codigo == "SERVICIO";
                if (esServicio)
                {
                    CompletarPrefijoCondicionEspecial(solpEntity);
                }
            }
            else
            {
                solpEntity.ProveedorAsignado_Id = null;
            }
            SetNombreDePedido(solpEntity);
            ExistenPosicionesNuevas(solp, solpEntity);
            repositorio.GuardarCambios();
            solp.Id = solpEntity.Id;

            solpEntity.Pliego = GuardarPliego(solp, adjuntos, condEsp, ObtenerRutaArchivos(solpEntity.Id, "Solp"), solpEntity.Pliego);

            solp.EmailLinkToken = solpEntity.EmailLinkToken;
            var respuestaGuardarSOLP = new RespuestaGuardarSOLP
            {
                Solp = solp
            };
            respuestaGuardarSOLP.Solp.NroSolp = solpEntity.NroSolp ?? "";
            respuestaGuardarSOLP.Solp.TieneModificaciones = solpEntity.TieneModificaciones;
            respuestaGuardarSOLP.Solp.TienePeticionDeOferta = solpEntity.Posiciones != null && solpEntity.Posiciones.Any(p => p.Peticiones != null && p.Peticiones.Any());

            if (solp.Finalizar)
            {
                try
                {
                    respuestaGuardarSOLP = FinalizarSolp(solpEntity, respuestaGuardarSOLP, enviarMailUrgencia);
                    solpEntity.UsuarioModificacion_Id = solp.UsuarioActual.Id;
                    solpEntity.FechaModificacion = DateTime.Now;
                }
                catch (Exception e)
                {
                    return new RespuestaGuardarSOLP { Solp = solp, IdEntidad = solpEntity.Id, Errores = new List<string> { e.Message }, Mensaje = e.Message };
                }
            }

            ActualizarPosiciones(solp, solpEntity);
            return respuestaGuardarSOLP;
        }

        private void ExistenPosicionesNuevas(SolpDto solp, Solp solpEntity)
        {
            var cotizaciones = repositorio.Listar<Cotizacion>(coti => coti.CotizacionPosiciones.Any(posicion => posicion.PeticionDeOfertaSolpPosicion
                .SolpPosicion.Solp.NroSolp == solpEntity.NroSolp) && coti.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado);

            var peticiones = repositorio.Listar<PeticionDeOferta>(po => po.Posiciones.FirstOrDefault().SolpPosicion.Solp.NroSolp == solpEntity.NroSolp);
            var revisionFinalizada = peticiones.Any(po => po.RevisionTecnica != null && po.RevisionTecnica.Finalizada);
            var solpLiberada = solpEntity.NroSolp != null && solpEntity.EstadoSolpSap != null && solpEntity.EstadoSolpSap.CodigoSap == "05";

            // Comparar las posiciones
            foreach (var nuevaPosicion in solp.Posiciones)
            {
                if (cotizaciones != null && cotizaciones.Count > 0 && revisionFinalizada)
                {

                    if (!solpEntity.Posiciones.Any(p => p.Codigo == nuevaPosicion.Codigo) && solpLiberada)
                    {
                        solpEntity.TieneModificaciones = true;
                        break;
                    }

                    // Comparar las subposiciones si existen
                    if (nuevaPosicion.Subposiciones != null)
                    {
                        foreach (var nuevaSubposicion in nuevaPosicion.Subposiciones)
                        {
                            var posicionExistente = solpEntity.Posiciones.FirstOrDefault(p => p.Codigo == nuevaPosicion.Codigo);
                            if ((posicionExistente != null && !posicionExistente.Subposiciones.Any(sp => sp.Codigo == nuevaSubposicion.Codigo)) && solpLiberada)
                            {
                                solpEntity.TieneModificaciones = true;
                                break;
                            }
                        }
                    }

                    if (solpEntity.TieneModificaciones == true)
                    {
                        break;
                    }
                }
            }
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
            //posiciones nuevas y actualizadas
            foreach (var pos in solp.Posiciones)
            {
                SolpPosicion posEntity = solpEntity.Posiciones.FirstOrDefault(y => y.Codigo == pos.Codigo)
                    ?? new SolpPosicion();

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
                {
                    posEntity.Unidad = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Unidad && x.Codigo == pos.Unidad.Codigo);
                }
                posEntity.PrecioBruto = pos.PrecioBruto;
                if (pos.CuentaMayor != null)
                {
                    posEntity.CuentaMayorSap = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.CuentasSolpSap && x.Codigo == pos.CuentaMayor.Codigo);
                }
                if (pos.TipoImputacionValor != null)
                {
                    posEntity.TipoImputacionSap = repositorio.Obtener<TablaSap>(x => x.Tabla == pos.TipoImputacionValor.Tabla && x.Codigo == pos.TipoImputacionValor.Codigo);
                }
                if (pos.TipoPosicion != null)
                {
                    posEntity.TipoPosicion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoPosicionSolp && x.Codigo == pos.TipoPosicion.Codigo);
                }
                if (pos.TipoImputacion != null)
                {
                    posEntity.TipoImputacion = repositorio.Obtener<TablaGeneral>(x => x.Tabla == TablasGenerales.TipoImputacionSolp && x.Codigo == pos.TipoImputacion.Codigo);
                }
                if (pos.Almacen != null)
                {
                    posEntity.Almacen = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Almacen && x.Codigo == pos.Almacen.Codigo);
                }
                else
                {
                    posEntity.Almacen_Id = null;
                }
                int? centroId = null;
                if (pos.Centro != null)
                {
                    posEntity.Centro = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Centro && x.Codigo == pos.Centro.Codigo);
                    centroId = posEntity.Centro.Id;
                }

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
                {
                    posEntity.Moneda = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Moneda && x.Codigo == pos.Moneda.Codigo);
                }

                if (pos.CodigoMaterialSap != null)
                {
                    posEntity.MaterialSolp = repositorio.Obtener<MaterialSolp>(x => x.Estado && x.CodigoSap == pos.CodigoMaterialSap.Codigo
                        && centroId == x.Centro_Id
                    );
                    if (posEntity.MaterialSolp == null)
                    {
                        throw new ValidationCustomException($"Pos: {pos.Indice} .El material {pos.CodigoMaterialSap.Codigo} no esta habilitado para el centro {posEntity.Centro.Descripcion}.");
                    }
                }
                else
                {
                    posEntity.MaterialSolp = null;
                    posEntity.MaterialSolp_Id = null;
                }

                if (pos.CodigoServicioSap != null)
                {
                    posEntity.ServicioSolp = repositorio.Obtener<ServicioSolp>(x => x.CodigoSap == pos.CodigoServicioSap.Codigo);
                }

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
                {
                    posEntity.Subposiciones = new List<SolpSubposicion>();
                }

                if (pos.Subposiciones != null)
                {
                    int numeroPosicionConsecutivo = (posEntity.Subposiciones.OrderByDescending(a => a.Numero).FirstOrDefault()?.Numero ?? 0) + 1;
                    /* para que las subposiciones tengan un numero consecutivo
                    * mantener el orden de las subposiciones con OrderBy en el ciclo.
                    */

                    foreach (var subpos in pos.Subposiciones.OrderBy(sp => sp.Numero))
                    {
                        SolpSubposicion subposEntity =
                            posEntity.Subposiciones.FirstOrDefault(y => y.Codigo == subpos.Codigo)
                            ?? new SolpSubposicion();

                        subposEntity.Codigo = subpos.Codigo;
                        subposEntity.Cantidad = subpos.Cantidad;
                        subposEntity.Estado = true;

                        if (subpos.TipoImputacionValor != null)
                        {
                            subposEntity.TipoImputacionSap = repositorio.Obtener<TablaSap>(x => x.Tabla == subpos.TipoImputacionValor.Tabla && x.Codigo == subpos.TipoImputacionValor.Codigo);
                        }

                        if (subpos.CuentaMayor != null)
                        {
                            subposEntity.CuentaMayorSap = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.CuentasSolpSap && x.Codigo == subpos.CuentaMayor.Codigo);
                        }

                        subposEntity.Numero = subposEntity.Numero == 0 ? numeroPosicionConsecutivo++ : subposEntity.Numero;
                        subposEntity.PrecioBruto = subpos.PrecioBruto;
                        subposEntity.Tarea = subpos.Tarea;

                        if (subpos.Unidad != null)
                        {
                            subposEntity.Unidad = repositorio.Obtener<TablaSap>(x => x.Tabla == TablasSap.Unidad && x.Codigo == subpos.Unidad.Codigo);
                        }

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
                        SolpSubposicion subposEntity =
                            posEntity.Subposiciones.FirstOrDefault(y => y.Codigo == subpos2.Codigo || y.Numero == subpos2.Numero)
                            ?? new SolpSubposicion();

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
                {
                    posEntity.Proveedores = new List<SolpProveedor>();
                }

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
                        SolpProveedor provEntity =
                            posEntity.Proveedores.FirstOrDefault(x => x.RazonSocial == prov.RazonSocial)
                            ?? new SolpProveedor();

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

        public string ObtenerRutaArchivos(int id, string path)
        {
            return $"{rutaArchivosCompras}/{path}_{id}";
        }


        private Pliego GuardarEspecificacionesTecnicasPliego(SolpDto solp, Solp solpEntity, Pliego pliegoEntity)
        {
            return GuardarEspecificacionesTecnicasPliego(solp, ObtenerRutaArchivos(solpEntity.Id, "Solp"), pliegoEntity);
        }

        private Pliego GuardarEspecificacionesTecnicasPliego(SolpDto solp, string rutaArchivos, Pliego pliegoEntity)
        {
            var rutaArchivo = string.Concat(rutaArchivos, "/", FileKeys.EspecificacionesTecnicasPliego, ".txt");
            solp.EspecificacionesTecnicas = ImageResizer.AjustarImagenesEnHtml(solp.EspecificacionesTecnicas);

            Directory.CreateDirectory(rutaArchivos);
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
            Directory.CreateDirectory(ruta);

            GuardarArchivosSolp(files.GetMultiple("fileEspecificaciones"), ruta, pliego, FileKeys.AdjuntoSolp);
            GuardarArchivosSolp(files.GetMultiple("fileCotizaciones"), ruta, pliego, FileKeys.AdjuntoCotizacionesSolp);
            GuardarArchivosSolp(files.GetMultiple("fileCotizacionesCondEsp"), ruta, pliego, FileKeys.AdjuntoCotizacionesSolpCondEsp);

            repositorio.GuardarCambios();

            solp.Adjuntos = pliego.Archivos
                .Where(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp)
                .Select(x => new ArchivoDto()
                {
                    Id = x.Id,
                    FileKey = x.FileKey,
                    Nombre = x.ObtenerNombre(x.Ruta)
                }).ToList();

            return solp;
        }

        private SolpDto GuardarAdjuntosPliegoMultiple(SolpDto solp, HttpFileCollectionBase files, Pliego pliego)
        {
            var ruta = ObtenerRutaArchivos(pliego.Id, "PliegoMultiple");
            Directory.CreateDirectory(ruta);

            GuardarArchivosSolp(files.GetMultiple("fileEspecificaciones"), ruta, pliego, FileKeys.AdjuntoSolp);
            GuardarArchivosSolp(files.GetMultiple("fileCotizaciones"), ruta, pliego, FileKeys.AdjuntoCotizacionesSolp);

            repositorio.GuardarCambios();

            solp.Adjuntos = pliego.Archivos
                .Where(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp)
                .Select(x => new ArchivoDto()
                {
                    Id = x.Id,
                    FileKey = x.FileKey,
                    Nombre = x.ObtenerNombre(x.Ruta)
                }).ToList();

            return solp;
        }

        private void GuardarArchivosSolp(IEnumerable<HttpPostedFileBase> archivos, string ruta, Pliego pliego, string fileKey)
        {
            foreach (var file in archivos)
            {
                var rutaArchivo = CrearRutaArchivo(Path.Combine(ruta, Path.GetFileName(file.FileName)), pliego.Archivos);
                file.SaveAs(rutaArchivo);
                pliego.Archivos.Add(new Archivo
                {
                    FileKey = fileKey,
                    Ruta = rutaArchivo,
                });
            }
        }

        private string CrearRutaArchivo(string ruta, ICollection<Archivo> archivos, int numeroIncremental = 0)
        {
            string nuevaRuta = ruta;

            if (numeroIncremental != 0)
            {
                // Separar la ruta y la extensión del archivo
                string nombreArchivo = System.IO.Path.GetFileNameWithoutExtension(ruta);
                string extensionArchivo = System.IO.Path.GetExtension(ruta);
                string directorioArchivo = System.IO.Path.GetDirectoryName(ruta);

                // Generar la nueva ruta con el número incremental
                nuevaRuta = $"{directorioArchivo}/{nombreArchivo}({numeroIncremental}){extensionArchivo}";
            }

            bool rutaExiste = archivos.Any(archivo => archivo.Ruta == nuevaRuta);

            // Si la ruta ya existe, generar una nueva ruta con el número incremental
            if (rutaExiste)
            {
                // Llamar recursivamente a la función con la nueva ruta y el siguiente número incremental
                return CrearRutaArchivo(ruta, archivos, numeroIncremental + 1);
            }

            // Si la ruta no existe, devolver la ruta original
            return nuevaRuta;
        }

        private RespuestaGuardarSOLP FinalizarSolp(Solp solpEntity, RespuestaGuardarSOLP respuestaGuardarSOLP, bool enviarMailUrgencia)
        {
            CrearSolpConsumerMOAResponse resultadoCrearSolp = new CrearSolpConsumerMOAResponse();
            resultadoCrearSolp.Errores = new List<CrearSolpConsumerMOAError>();
            respuestaGuardarSOLP.Errores = new List<string>();
            if (string.IsNullOrEmpty(solpEntity.NroSolp))
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        SolpSAPSinPIDto solpSAPSinPI = comprasServiceSap.ConvertirSOLPSAPSinPI(solpEntity);
                        resultadoCrearSolp = comprasServiceSap.CrearSolpSapSinPI(solpSAPSinPI);
                    }
                    else
                    {
                        SolpSAPDto solpSAP = comprasServiceSap.ConvertirSOLPSAP(solpEntity);
                        resultadoCrearSolp = comprasServiceSap.CrearSolpSap(solpSAP);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    resultadoCrearSolp.Errores.Add(new CrearSolpConsumerMOAError { Mensaje = e.Message, Tipo = "E" });
                }

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
                else
                {
                    Log.Error("Error sap al guardar la solp id: " + solpEntity.Id, new Exception(respuestaGuardarSOLP.Errores.ToJson()));
                }
                repositorio.GuardarCambios();
            }
            else
            {
                ModificarSolpConsumerMOAResponse resultadoEditarSolp = new ModificarSolpConsumerMOAResponse();
                resultadoEditarSolp.Errores = new List<ModificarSolpConsumerMOAError>();
                if (solpEntity.TipoSolpSap != 2)
                {
                    try
                    {


                        if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                        {
                            SolpSAPSinPIDto solpSAPSinPI = comprasServiceSap.ConvertirSOLPSAPSinPI(solpEntity);
                            resultadoEditarSolp = comprasServiceSap.ModificarSolpSapSinPI(solpSAPSinPI);

                        }
                        else
                        {
                            SolpSAPDto solpSAP = comprasServiceSap.ConvertirSOLPSAP(solpEntity);
                            resultadoEditarSolp = comprasServiceSap.ModificarSolpSap(solpSAP);
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        ObtenerSolpesDesdeSAPJob(new ObtenerSolpRequest { NumeroSolp = solpEntity.NroSolp, FechaDesde = new DateTime(2010, 01, 01), FechaHasta = DateTime.Now.Date.AddDays(1) });
                        respuestaGuardarSOLP.Solp = TraerSolpId(solpEntity.Id);
                        resultadoEditarSolp.Errores.Add(new ModificarSolpConsumerMOAError { Mensaje = e.Message, Tipo = "E" });
                    }

                }

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
                    if ((solpEntity.TrabajoYaHecho == true || solpEntity.ConPresupuesto) &&
                        (solpEntity.EstadoSolpSap?.CodigoSap == "05" || solpEntity.EstadoSolpSap?.CodigoSap == "02"))
                    {
                        ActualizarOfertasAlEditarSolpLiberada(solpEntity);
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
                else
                {
                    Log.Error("Error sap al guardar la solp nro: " + solpEntity.NroSolp, new Exception(respuestaGuardarSOLP.Errores.ToJson()));
                    //revertir los cambios si da error
                    ObtenerSolpesDesdeSAPJob(new ObtenerSolpRequest { NumeroSolp = solpEntity.NroSolp, FechaDesde = new DateTime(2010, 01, 01), FechaHasta = DateTime.Now.Date.AddDays(1) });
                    respuestaGuardarSOLP.Solp = TraerSolpId(solpEntity.Id);
                }
                repositorio.GuardarCambios();
            }

            respuestaGuardarSOLP.IdEntidad = solpEntity.Id;
            ValidarSolpAnulada(solpEntity.NroSolp);
            return respuestaGuardarSOLP;
        }

        private void ActualizarOfertasAlEditarSolpLiberada(Solp solpEntity)
        {
            var posicionId = solpEntity.Posiciones.FirstOrDefault()?.Id;
            var peticionDeOfertaId = repositorio.Obtener<PeticionDeOferta>(x => x.Posiciones.Any(y => y.SolpPosicion_Id == posicionId)).Id;
            var posicionesId = solpEntity.Posiciones.Select(x => x.Id).ToList();
            var peticionDeOfertaSolpPosicion = repositorio.Listar<PeticionDeOfertaSolpPosicion>(peticionPos => posicionesId.Contains(peticionPos.SolpPosicion_Id));

            var peticionUsuarioId = repositorio.Obtener<PeticionDeOfertaUsuario>(x => x.PeticionDeOferta_Id == peticionDeOfertaId).Id;
            var cotizacion = repositorio.Obtener<Cotizacion>(x => x.PeticionDeOfertaUsuario_Id == peticionUsuarioId);
            var cotizaciones = new List<Cotizacion> { cotizacion };
            ActualizarPosicionesDePeticionDeOferta(solpEntity, peticionDeOfertaSolpPosicion);
            AgregarPosicionACotizacionTrabajoYaHechoOPresupuestado(cotizaciones, solpEntity);
        }

        private void ActualizarPosicionesDePeticionDeOferta(Solp solpEntity, List<PeticionDeOfertaSolpPosicion> peticionPosiciones)
        {
            foreach (var posicion in solpEntity.Posiciones)
            {
                if (!peticionPosiciones.Any(pos => pos.SolpPosicion_Id == posicion.Id))
                {
                    var peticionPosicionNueva = new PeticionDeOfertaSolpPosicion
                    {
                        PeticionDeOferta_Id = peticionPosiciones.FirstOrDefault()?.PeticionDeOferta_Id ?? 0, // Asume que siempre hay al menos una PeticionDeOferta en peticionPosiciones
                        SolpPosicion_Id = posicion.Id
                    };
                    repositorio.Agregar(peticionPosicionNueva);
                    repositorio.GuardarCambios();
                }
            }
        }

        private void ActualizarPeticionDeOfertaAlEditarSolp(Solp solpEntity, bool actualizarEstadoCotizacion = false)
        {
            if (solpEntity.Posiciones.First().TipoPosicion.Codigo == "MATERIALES" && solpEntity.TrabajoYaHecho == false && !solpEntity.ConPresupuesto)
            {
                return;
            }
            var posicionesId = solpEntity.Posiciones.Select(x => x.Id).ToList();
            var peticionesDeOferta = repositorio.Listar<PeticionDeOferta>(x => x.Posiciones.Any(y => posicionesId.Contains(y.SolpPosicion_Id)));

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

                if (posNueva && actualizarEstadoCotizacion)
                {
                    foreach (var poUsusario in po.Usuarios)
                    {
                        if (poUsusario.Cotizaciones != null && poUsusario.Cotizaciones.Count > 0)
                        {
                            foreach (var poCotizacion in poUsusario.Cotizaciones)
                            {
                                if (poCotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado)
                                {
                                    poCotizacion.CotizarNuevaPosicion = true;
                                }
                                poCotizacion.CotizacionEstado_Id = (int)CotizacionEstadoEnum.Incompleta;
                            }
                        }
                    }
                }

                repositorio.GuardarCambios();
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

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<TablaGeneralDto> ObtenerTiposPosicionSolp()
        {
            return ObtenerTablaGeneral(TablasGenerales.TipoPosicionSolp);
        }

        public List<TablaGeneralDto> ObtenerTiposImputaciones()
        {
            var tabla = TablasGenerales.TipoImputacionSolp;
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

        public List<TablaSapDto> ObtenerMonedas()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.Moneda).Where(a => a.Codigo != "USDM" && a.Codigo != "CLP").ToList();
        }

        public List<TablaSapDto> ObtenerGrupoCompras()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.GrupoCompras);
        }

        public List<TablaSapDto> ObtenerGrupoArticulos()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.GrupoArticulo);
        }

        public List<TablaSapDto> ObtenerCentros()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.Centro);
        }

        public List<TablaSapDto> ObtenerAlmacenes()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.Almacen);
        }

        public List<TablaSapDto> ObtenerUnidades()
        {
            return comprasServiceSap.ObtenerTablaSap(TablasSap.Unidad);
        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x => new CentroDireccionDto(x)).ToList();
        }

        public List<PeticionDeOfertaDto> ListarPeticionesDeOferta(int solpId)
        {
            var solps = repositorio.Obtener<Solp>(solpId);
            var posicionesId = solps.Posiciones.Select(x => x.Id);
            var peticiones = repositorio.Listar<PeticionDeOferta, PeticionDeOfertaDto>(po => new PeticionDeOfertaDto
            {
                Id = po.Id,
                Solp_Id = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Id,
                RegistroInfo = po.RegistroInfo,
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
                TrabajoHecho = solps.TrabajoYaHecho == true,
                ConPresupuesto = solps.ConPresupuesto,
            }, peti => peti.RegistroInfo != true && peti.Posiciones.Any(y => posicionesId.Contains(y.SolpPosicion_Id)));
            return peticiones;
        }

        public SolpDto TraerSolpPliego(int idPliego, out int solpCount)
        {
            var includes = new List<Expression<Func<Pliego, object>>>
            {
                u => u.Solps,
                u => u.Solps.Select(y => y.Pliego.VisitasMasivas),
                u => u.Solps.Select(y => y.Pliego.Archivos),
                u => u.Solps.Select(y => y.Posiciones),
                u => u.Solps.Select(y => y.Posiciones.Select(z => z.Subposiciones)),
                u => u.Solps.Select(y => y.UsuarioCreacion),
                u => u.Solps.Select(y => y.UsuarioModificacion),
            };

            Pliego pliego = repositorio.Obtener<Pliego>(includes, x => x.Id == idPliego);

            if (pliego is null) { throw new InvalidOperationException("Pliego no encontrado"); }
            if (!pliego.Multiple) { throw new InvalidOperationException("Esta funcionalidad sólo está disponible para pliegos múltiples"); }
            if (pliego.Solps.Count == 0) { throw new InvalidOperationException("No se encontraron SOLPs para el pliego"); }

            solpCount = pliego.Solps.Count;

            IEnumerator<Solp> enumerator = pliego.Solps.OrderBy(solp => solp.NroSolp).GetEnumerator();
            StringBuilder numeroSolpBuilder = new StringBuilder();

            enumerator.MoveNext(); // primer elemento

            SolpDto data = TraerSolp(enumerator.Current);
            numeroSolpBuilder.Append(enumerator.Current.NroSolp);

            while (enumerator.MoveNext())
            {
                data.Posiciones.AddRange(TraerSolp(enumerator.Current).Posiciones);
                numeroSolpBuilder
                    .Append(", ")
                    .Append(enumerator.Current.NroSolp);
            }

            data.NroSolp = numeroSolpBuilder.ToString();

            return data;
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

            return TraerSolp(solp);
        }

        private SolpDto TraerSolp(Solp solp)
        {
            if (solp == null)
            {
                throw new InfoCustomException("No se encontró la SOLP.");
            }

            var hayAdjudicacionPosicion = repositorio.Existe<AdjudicacionPosicion>(posi => posi.Posicion.Solp.NroSolp.Contains(solp.NroSolp));
            var solpDevuelta = new SolpDto()
            {
                UsuarioActual = solp.UsuarioCreacion != null ? new UsuarioDto(solp.UsuarioCreacion) : new UsuarioDto(),
                Id = solp.Id,
                Pliego_Id = solp.Pliego_Id,
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
                SupervisorTrabajo = solp.Pliego.SupervisorTrabajo,
                VisitasObraMasiva = solp.Pliego.VisitasMasivas.Select(a => new VisitaObraDto(a)).ToList(),
                TieneVisitaObraMasiva = solp.Pliego.TieneVisitaObraMasiva,
                TieneObradores = solp.Pliego.TieneObradores ?? false,
                TieneMedioElevacion = solp.Pliego.TieneMedioElevacion ?? false,
                TieneAndamio = solp.Pliego.TieneAndamio ?? false,
                TieneTecnicoSeguridad = solp.Pliego.TieneTecnicoSeguridad ?? false,
                TieneGrillaPersonal = solp.Pliego.TieneGrillaPersonal ?? false,
                TieneFabricacionTallerExterno = solp.Pliego.TieneFabricacionTallerExterno ?? false,
                TieneDescripcionTecnica = solp.Pliego.TieneDescripcionTecnica ?? false,
                TieneDocumentacionTecnica = solp.Pliego.TieneDocumentacionTecnica ?? false,
                RequisitoCiberseguridad = solp.Pliego.RequisitoCiberseguridad ?? false,
                FechaHoraLimiteConsulta = solp.Pliego.FechaHoraLimiteConsulta,
                ObservacionesGeneracion = solp.Pliego.ObservacionesGeneracion,
                //EspecificacionesTecnicas = x.EspecificacionesTecnicas,
                DiasEjecucion = solp.Pliego.DiasEjecucion,
                ObservacionesCotizacion = solp.Pliego.ObservacionesCotizacion,
                ObservacionesCotizacionCondEsp = solp.Pliego.ObservacionesCotizacionCondEsp,
                JornadaLaboral = string.IsNullOrEmpty(solp.Pliego.JornadaLaboralDias) ? new List<DayOfWeek>() :
                                solp.Pliego.JornadaLaboralDias.Split(",".ToCharArray()).Select(a => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), a)).ToList(),
                JornadaLaboralDesde = solp.Pliego.JornadaLaboralHorasDesde,
                JornadaLaboralHasta = solp.Pliego.JornadaLaboralHorasHasta,
                ClaseDocumento = solp.ClaseDocumento != null ? new TablaSapDto(solp.ClaseDocumento) : new TablaSapDto(),
                ProveedorAsignado_Id = solp.ProveedorAsignado_Id,
                SeraUsadoEnPliegoMultiple = solp.SeraUsadoEnPliegoMultiple,
                TrabajoYaHecho = solp.TrabajoYaHecho,
                ConPresupuesto = solp.ConPresupuesto,
                CertificacionAutomatica = solp.CertificacionAutomatica,
                AdmiteCertificacionesParciales = solp.AdmiteCertificacionesParciales,
                CondEspProveedorAsignado = solp.CondEspProveedorAsignado,
                Adicional = solp.Adicional,
                Urgencia = solp.Urgencia,
                NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                DeshabilitarAdicional = hayAdjudicacionPosicion,
                EditarCondicionesEspeciales = (solp.EstadoSolpSap_Id == null || (solp.EstadoSolpSap.CodigoSap != "05" && solp.EstadoSolpSap.CodigoSap != "02")),

                Adjuntos = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp).Select(s => new ArchivoDto
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
                Posiciones = solp.Posiciones.Select(p => new SolpPosicionDto(p)).ToList(),
                PasoCompletado = solp.PasoCompletado,
                EstadoPasos = solp.EstadoPasos,
                EmailLinkToken = solp.EmailLinkToken,
                LiberadoresSapSolp = solp.LiberadoresSapSolp.Select(l => new LiberadorSapSolpDto(l)).ToList(),
                THAjustePolinomica = solp.THAjustePolinomica,
                THProveedorDirecto = solp.THProveedorDirecto,
                THServicioPermanente = solp.THServicioPermanente,
                TienePeticionDeOferta = solp.Posiciones.Any(p => p.Peticiones.Any()),
                TieneModificaciones = solp.TieneModificaciones,
                TieneRevisionTecnicaFinalizada = solp.Posiciones.Any(p => p.Peticiones != null && p.Peticiones.Any(po => po.PeticionDeOferta.RevisionTecnica != null && po.PeticionDeOferta.RevisionTecnica.Finalizada)),
                EnvioCircularA = solp.EnvioCircularA,
                EsPliegoMultiple = solp.Pliego.Multiple,
                MultipleFinalizado = solp.Pliego.MultipleFinalizado,
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
                    solpDevuelta.ProveedorAsignado_Id = ordenDeCompraSAPDto.Cabecera.Usuario_Id;
                    solpDevuelta.ProveedorRazonSocialAdicional = ordenDeCompraSAPDto.Cabecera.RazonSocialProveedor;
                    solpDevuelta.MonedaOC = ordenDeCompraSAPDto.Cabecera.Moneda;
                    solpDevuelta.MontoTotalOC = ordenDeCompraSAPDto.Cabecera.MontoTotal;
                    solpDevuelta.FechaCreacionOC = ordenDeCompraSAPDto.Cabecera.FechaCreacionString;
                    solpDevuelta.CodigoProveedorSap = ordenDeCompraSAPDto.Cabecera.CodigoProveedor;
                }
            }

            if (solpDevuelta.ProveedorAsignado_Id != null)
            {
                var usuario = repositorio.Obtener<Usuario>(solpDevuelta.ProveedorAsignado_Id);
                solpDevuelta.ProveedorAsignado = usuario.ObtenerRazonSocial();
                solpDevuelta.CodigoProveedorSap = usuario.ObtenerCodigoProveedor();
            }

            return solpDevuelta;
        }

        public SolpESDto TraerSolpPorNumero(string nroSolp)
        {
            var includes = new List<Expression<Func<Solp, object>>>
            {
                u => u.Pliego,
                u => u.Pliego.VisitasMasivas,
                u => u.Pliego.Archivos,
                u => u.Posiciones,
                u => u.Posiciones.Select(y => y.Subposiciones),
                u => u.UsuarioCreacion,
                u => u.UsuarioModificacion
            };

            var solp = repositorio.Listar<Solp>(s => s.NroSolp == nroSolp, 0, null, DirOrden.Asc, includes).ToList().FirstOrDefault()
                ?? throw new InfoCustomException("No se encontró la SOLP.");

            var solpDevuelta = new SolpESDto()
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
                VisitasObraMasiva = solp.Pliego.VisitasMasivas.Select(a => new VisitaObraESDto(a)).ToList(),
                TieneVisitaObraMasiva = solp.Pliego.TieneVisitaObraMasiva,
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
                //ProveedorAsignadoId = solp.ProveedorAsignado_Id,
                TrabajoYaHecho = solp.TrabajoYaHecho,
                Adicional = solp.Adicional,
                Urgencia = solp.Urgencia,
                NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                // DeshabilitarAdicional = solp.Adjudicacions.Any(),

                Adjuntos = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),

                EspecificacionesTecnicas = solp.Pliego.Archivos.FirstOrDefault(a => a.FileKey == FileKeys.EspecificacionesTecnicasPliego)?.Ruta,

                TieneCondicionesGenerales = solp.Pliego.TieneCondicionesGenerales ?? true,

                RevisadoPor = solp.Pliego.RevisadoPor,

                //EstadoSolpSapId = solp.EstadoSolpSap_Id,
                //EstadoDocumentoId = solp.EstadoDocumento_Id,

                //Posiciones = (x.TipoSolpSap == (int)TipoSolpSap.Sap || x.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || x.TipoSolpSap == (int)TipoSolpSap.ReposicionAutomatica) ? 
                //                x.Posiciones.Select(p => new SolpPosicionDto(p)).ToList() : 
                //                x.Posiciones.Where(p => !p.FechaBaja.HasValue).Select(p => new SolpPosicionDto(p)).ToList(),

                Posiciones = solp.Posiciones.Select(p => new SolpPosicionESDto(p)).ToList(),
                PasoCompletado = solp.PasoCompletado,
                EstadoPasos = solp.EstadoPasos,
                EmailLinkToken = solp.EmailLinkToken
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
                solpDevuelta.ProveedorIdAdicional = ordenDeCompraSAPDto.Cabecera.Usuario_Id;
                solpDevuelta.ProveedorRazonSocialAdicional = ordenDeCompraSAPDto.Cabecera.RazonSocialProveedor;
                solpDevuelta.MonedaOC = ordenDeCompraSAPDto.Cabecera.Moneda;
                solpDevuelta.MontoTotalOC = ordenDeCompraSAPDto.Cabecera.MontoTotal;
                solpDevuelta.FechaCreacionOC = ordenDeCompraSAPDto.Cabecera.FechaCreacionString;
            }

            //if (solpDevuelta.ProveedorAsignadoId != null)
            //{
            //    var usuario = repositorio.Obtener<Usuario>(solpDevuelta.ProveedorAsignadoId);
            //    solpDevuelta.ProveedorAsignado = usuario.ObtenerRazonSocial();
            //}

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

        public byte[] GenerarSolpPdf(int id, bool esPliego = false)
        {
            SolpDto solp;
            if (esPliego)
            {
                solp = TraerSolpPliego(id, out _);
            }
            else
            {
                solp = TraerSolpId(id);
            }
            var usuarioCompras = usuarioService.ListarUsuarioCompras();
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
            solpValores.Add(SolpTemplateKeys.FECHA_OBRA, solp.Posiciones.FirstOrDefault()?.FechaEntregaServicio?.ToString("dd-MM-yyyy"));


            solpValores.Add(SolpTemplateKeys.FECHA_PRESENTACION, Convert.ToDateTime(solp.FechaHoraEntrega).ToString("dd-MM-yyyy"));
            solpValores.Add(SolpTemplateKeys.FECHA_CREACION, solp.FechaCreacion.ToString("dd-MM-yyyy"));

            solpValores.Add(SolpTemplateKeys.USUARIO_COMPRAS, solp.UsuarioCompras.Mail == null ? usuarioCompras.Any() ? usuarioCompras[0].Mail : String.Empty : solp.UsuarioCompras.Mail);

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

                if (tieneHuecos || diasOrdenado.Count == 1)
                {
                    diasJornada = string.Join(",", diasOrdenado.Select(a => a.GetDia().ToList()));
                }
                else
                {
                    diasJornada = string.Format("{0} a {1}", diasOrdenado.First().GetDia(), diasOrdenado.Last().GetDia());
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

        private byte[] ConvertHtmlToPdf(string xHtml, string css, SolpDto solp)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4, 70f, 70f, 150f, 60f))
                {
                    //var solp = repositorio.Obtener<Solp>(idSolp);

                    var PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                    document.Open();

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAPcAAABqCAYAAABgdMfOAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAC2uSURBVHhe7V0FnFVF+16sz/isT8VGBVspAWksFERQQVBEwEDpXpbu7m5YekkJ6e5curs7lmaBJd7/+7xn5t5z7z03dsEF9j8Pv/lx9545c+acO89b886cMDIwMEiSMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2MEiiMOQ2uKtw/HwMXb0Wp/4yuBUYchvcFbgSd5U6zhtGz9T6hObsiFbfGtwKDLkN7ij2xRymDkzq99r+TGGV0lFYxTQ0ft1cddTgVmDIbeCJmzfp5o3rUujGDfXlbQC3dfFqLO1nMi/avZY6LRhBX/cPp8fr5WZSp6ew6lnovtqfUFjVjDR23Rx1ksGtwJD7/wnOX75Ie08dovWHd9CULUuoz5Kx1GBaXyozpjUVHlSHcverRlm7laF0HX+l1O2LU+p2xSlth5KUpVtp+qxPFcofGUGFuF6xoQ0pf79wKjiwFhWPakwlueD/okMbUJEh9ajYsEZUcnhTKs7//8D18zKBc/QoT2nal6BXWhSix0Dm8KwUVpkJzUQOq5mT7q/9qauEVf6IolZNV702uBUYcidxnDofQ+PXzqIm0/rQt0zQsIgcFqmYRGIG4/8qGazvqn0sGjQsXBV8xndVM1nHoWHLvkffMbF7Lh1PQ5mEo9bMpGGrp8vfdaf0ojx9q7Fpnda53RrZhMzJ2K+2E1oXrbmNWX57YMidhLD5yE7qsWg0nbxwWn1DdJ3N4Ws3rqm/iLVrfSGeE7kClWS1cvF5GajD/BGqJWe0mD1ISO3URrCiyT1xw3zVmsGtwJD7Hse+mCPUaV4U5WST+v6InBRW7gNaume9OuqLfzYuEPLd50AufwV1oY1rTe6hWnHGifOn6b/1v2QNnd2xnWDlPtboYdUz09ydq1SLBrcCQ+57FEv2rBN/1wpIsXkN05dNXpi/i3atUbV8sfvEAXqozmdcN5cjwXwL1+X2Cw6srVrwj7Hr54pVEB/BYS9hTO5k7DasObRdtWhwKzDkvsewkIlboF+45cMiKMVkcJGDyQ3i7j55UNX2RezVy5SiRSHxf+3Eci5MbDaT32vzswTkgqHS+I4iCJzbCl7Q/0fr5qZ9pw6rFg1uBYbc9wj2nDpExYc1VKTOwFrOV/NCe6fv9DvduBl4CgsRcNT1Pt+7QAA8ytp0w+Gd6szAyNz1r5Da9VdwvVebF6RLV2NViwa3AkPuewBdF46ip7X57UBqXeAXt507TJ3lHzCxw6pkdGxDF/F/2TIYFD1FnRUYR8+dpMfr5/GwJOJbIBggIAxuDwy572IcZsIU6FfdIrWY0Z/5EEIXHP9fo3wekXJ/KPt3WyGuUztWYXOcBcVfo1urM4JjxrblbMJnkoi3c5vBC/x1zJkb3B4Yct+lWLR7HaVo+i0TO31IASqQEdNQoaDh9H7SrlM72s9GIsvluCvqjOBoNmsA9yHh/jYK+tRuXpRq0eBWYch9F2LMutn0HySbwH+t419b64KMr1eafR9S0AvoumCkX80dFpGdHq6Zi9aH6GdrfC+mfsLmt1GQ2BJWLTPN37VatWhwqzDkvssweOVU1pwfC2EDmeG6yBw0m+29lo5XLQTHsDUzxAT2bYsJhraWjFM1Q8PVa1cpVeufVJ892wy1YG78uSb56WzsedWqwa3CkPsuwsi1s1h7ZZKBHgqxUaDdP2AT+tp1dxYacDNAxHzK5sW+/jHmvpnYhQfXU7U8EXfd/xrr7cf30QPQvCHPnfsWaP0CA2qqFg1uBwy57xLM37WGHmRSBwuc2YtoWtbAEzZ6pmvO3r6CVh7Yov7yBZJcoGXdOd5M7OqZ6VU27WMunlW13BgWPYWOnTul/vLF30heiWfWm71o66PvsgmqRYPbAUPuuwCHzhyn5xvlE4KFSmwUEOqzXpVUK27k6FGOJrF29oeNR3dbueJK0yZDZhtbALN3rFQ13Ji6dRnl7x9ON/mfP9SZ0kvI6d2/UEtYRE5ZLXb47AnVosHtgCH3HcbNmzfp896VmBzpQwqe6QJyJmNhsHzfRtWSBeSVh5X/kCZuXKC+8cWB00fpv/W+lIwwtCV541N6qqNunLt8gZLX/4qazIxU3zjjm/41JMLu3cdQC6yPHwbXVa0Z3C4Yct9hdJg/nMmVJp4mLfzj9FTMYU74m0gm2l9vUdSameobX8RcPEPJmxSwln+y9s/UpZSPzw78hIy4Mu/SMi8BYgeCaSlb/ZjgYJpeCTZz2wrVosHtgiH3HQRSSh9DICsifquosLjiEf7fO4d82d4NMp0EE7nnUv8R7zgmcqo2RYVUSC/dfHSPOuJGzyVjZYXZ802/pQsBpth2nthPD0LYKCsgvgWBvYwsXLADjMHthSH3HUTRqEbKVw3dHBetzWZ07cm+ZnSeftXYxM0gWr3VrIHqW2dk7PQ7hZV+m7ouGq2+cSN6/2Z6CME97ttXfauqb50xcdNCERIJCabpgOD49fNUawa3E4bcdwirmEDJqmcR39lp4PsrMH9fYJP6zCXP+WAkf0ALSjIIk7t2kLXXGVhbZnHI40a7qVoWlgAbEl1qO/jidjTH5gwJCqaxkGKXIGeP8qolg9sNQ+47hMLYEQVBNMeB71ysKaO0YjJ7I2fPCkwWK6iFdiuM76iOOKPS2HaOWWiSacZklWsx+Uavna2OOKPoUPbLuZ53X4MVROiTVc9KK1nIGfw7MOS+A9gBPxXR7ngmfUCbpulQ0mPbJGAyklKYYNo0xucSw5uqo84475AJ1mL2YDH50Y5MlbFvvzbQxgnsJ6fr+JuawvPtr//CWrtCGqoxqbtqyODfgCH3HUC9qb2FRM4D37nIEkwm7dQtS1UrFm7cuCFruJHZpuvCBy44oJaqERpkVRcLD2hUaYOJba0yi1E1fIEVaKgTr2WeCCDyfXzEQuHKtauqJYN/A4bciYy4a3H0JiLV7G87Dn7HwoSo/BHli6yhWnFjQPQklxmt68P3/rJPZVUjOPafPkrPNfxa9ckK7uFz6vYlZB7eHxB4Q0Zd6HEDvo/wLPRE3S9o67G9qhWDfwuG3ImMhbvXWhrSlfoZvECbPsjaceORXaoVCxeuXKIUzQsJYez10X7WHuVUrcBAznhW7KCChSS2JBpo/3z9w1UtZ2BLYznPdu1ABTnzSLyZttXT+jD4d2DIncioOzW+qZqs7diEdwqQIXPMMu89p9Iw152u0x8hzR2XGdPG0UUAacuNbadqOQMvNQjtXvgeMLVWJSNFhrizi8Gtw5A7kYG3ekCzOpPAt8DsfYZNZu8dVg6fPU5PIIXUIQEmrHpWeq9tMbrukHVmR59lEyyT3sGKQMQ92JZNEimvHCRSDh87PKu4Cn2XT1RnGiQGDLkTEVhZFZ99xmQ6irVq54WjVAtulMZWSbLziW8CTFh4NkrZsghdvnpZ1fbF4j3r6UEWHE7CwT0NNkvVdkbQjRZBbCb1IzVz0eh1gafUDG4/DLkTEbO3RwsZPNZRByioi7XaSBe1YxP73vDBdWTb5zw2ga2dWS6oMzxx+MxxeqlJfjHfnYSDJMKwtoUA8IfYK7EBt0hOhuwztgrebPEDLQ2Qm27w78GQOxGBN1uG6m9bU18ZaZrX1BeQf0BNFcjyJSaK7GrSOD/FXDqnznADC0RydC/rE0DzOB8BPG4b8/H+sD/mCD2CHVl9BIwyw9msLzq4XkgbNhr8OzDkTkSU/btNyNFlmMXfD6qjznRj+tZlQvpA0XaY/U82ykfHL5xRZ7lRBn0IMscOUx3z1ycCEBOZZRL9tvUDREfbL7FgGRQ9SdU0uFMw5E5E5MU2xVWDr3tG5tpDXLYc81ytdfVaHL3f7pegATmQ7FHWqnixvR1dFo32G0CzF5jar7HJfTHAarAZ21e4XAwhNbf7CLdbeVwHOn7ef+KLQeLBkDuRgD3N8O7rUFI1Qb77uB60tB3NZg1UWtfZnNYFZHu4zufyXjANpKhijjmUYB7M6nfa/uy4xltD3gtWPrX0B8tW/xrZnLZ6CSODOwtD7kRCbNxlStHyB78BKO+CKHPO7u5ElDUHttJDCKJFBF83je2THqiViw6yXwxMYyHxGL5n0gYTDCiol7JVETnXHxbsXktvNv2W6kzqIWu6De4+GHInEk5ePEPPNP7GcerJqch0VPUs1Gr2YBqwYiKlwm4nfqLb3gWm8n1M7kIDa1ORIfXksxDbTwDNuyCd9CH+P3L5RNpx8gDtZfN+z8mDsjkEXiQIXGdL5HKcyQ2/m2HInUg4wFr0cdu+ZaEUawuiTBKEE40fIjl1kXOrZBBNHkgoyNQX/GZkkWEnF5zD5nZYuQ8lgSZP70rUZuYAWrJrDV28Yl7Sd6/AkDuRAM33WN0vHMktJGaTW6aQNCFB6ErpJVAl/+N1vfjOb0FdXfQ56hja0wV/6zo4B9+xhYBX/77EZjb2U0PmWQv27ydvWkR7Th1k3/u6uguDewmG3ImEvScPeZBbzG5kiIFsTOgn6n9F77YtJjuh/jikPpUf04bqTO1Nzdks7zAvinosGk39lo6nvkvG+RR833PRGKnXdt4waszExG6mFce2o99HNGOyNqCfhtanX4c3obKjW1ONSd2o2exBcs6YdXNowc7V4jef9drdxeDehiF3IsGuueWtIqwxUzQvSGXHtBYNeejMMbp+w2hIg9sHQ+5EwsGYo7JrKXzZ7F3/ouGrp4f84j4Dg4TAkDuRgA3+s3Uvy+b1GPWNgcG/C0NuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4MkCkNuA4Mkilsi9/yF26nvwIU0KGqplAFDl9CIMdF05Urgd1T5Q2xsHA3mdgZyO2gPn3tHLqCpM8wbKwwM4otbIneLdlPpvqfL0LMpq0t5+rWqlDxVOO3YdUzViB/qNBpLDz9fwdXeM29Uo8derEiTp/l/rY2BgYEzboncXXrOFhK+9VF9KW+mr0/PvxVO02dtUjVCxyQm8HOpqlOqdPVc7b3wdgT9+GsfVSM0XLx4hY4dP0eHDp+hs+di6eaN4K+xTSwEepH97UR8LnPlShydOnWBDh48TTGnL1Lc1YRZXXZcZgvs4oXLdOnSFfXNv4OLVy7JK5POxJ4P+NLD+CD2aiyd5vZQblebAN64ir6GtkHHTamH+pf4HhOKWyJ3xRrDKfmb4S4yokDbdu01R9UIDQcOxtAHmRvRqx/U8mlr4LAlqpZ/bN56mNp1mUFFSvamrLlbUZpsTeiDLI0p46ctKE/BzlS9zihauHiHqh0YQ0cso+J/9qcyVYZKKcul2B/9qO+ABSGRc9PmQ/Rb2YFUpvIQ1/mlKgyiSuFRQiI7zpy5RFUiRtCfFQdLXVx31tzN6mhw9Ow7l0r+FenqK65ZgttYv979MgInHD12liKHLJZ+5srbjtLlaCbPP33OZvRpvvb0V6UhNGb8Krp2LfRtn/AbtOowlQqX6E0587SlrF+0ouxftqZvf+pOzdtOpuMscG8VK/dvolazB1GhgbXkBQ/YpuqZJvkpedNvKWXLwpSxSykqMqgONZsZSbO2r6DYuODC5fi5UzQ0eorsNfdx1z/p9RY/0LPc5nNNCtAb3KZsGDmkPnWYN0z2jvdGzMWzVH9qb6ozuQfVndKLak3oTOsPucfa6oNb6U9u+93WP9HT6o0sTjh96SwNXjFJ9rtL26Ekvdzse3qmcX6+x0KUo1tpqju5J209tlfVDg0JJjf84xx52lCKD2p7EBJkr1A9StUKDhDmp9/6iNZ+O4O7HWjwlGnq0LbtR1VNXxw5coYqMzleT12H/vd6NXqRNX2KD2vT63zeG2nq0mv8/cvv1aRnUlZji6KGDP5zrM0DoW2n6fTAs+WkPsr/WMC8/G5N2rLN2uA/GHbsPEbP8zOARaPbeJDba9HO96XzV69co0wsgJ54tbLUe/yVypT9qzZ0+XKcqhEYjVr849FXuDSwdK760b7Qyh26zqAPWfDhecHKgkDF88Pzwv+vvl+LnuP+43iegp1oy9bA9x0be5XqNRlPr/Nz/9/rVenFdyK4zdryO6C8xH//96VK9PUPXRKsyQdHT6bsXf6ydofFrq1qx1bZYDIiu7UlMz7rbZnLp5aXMhw5e0K14Is9pw7JJpTPNsjj3im22sf+28R1+e/s3crIq5g1IETCyrxLeJ2ybAfNnxftWSfHms6IpAfQBo6h7+U/lL3g7cA+8M1n9Jf3q7n6IffG15d+oA/cL2774Zq5qAnXDRUJJveatftlIKRK7zajUUAm/JChmqCde8wSX93eBsor3PYXBTrQtThn7bFg0XZKn6OpDMKUaeuKYIBAwPUxaGHSg+S6PfTzSSYRtHsg8jRsPsHD1cDn2g3HqqPBsXrtPhF4b6rngj7AkoiJ8TXHIGgyf9aSXmMSyPX4HkCQ3pHzVY3AiKg/RoQizsX1Xn43glZE71ZHPQHrCFr0qRRV5HpakILcEAwvvF1DyCj9UAVto++79ziT5Pz5y1SwWE9+rlXUb9BA/kdbOFcLj5T8u2CsbNvhX1A7YQVr6hysTYVY2PIZr1nCrrHYiw6DHsVhq2jU/aRnBdWKLzovGElPYrNKkM72kghr73Zuk7/Dixnsbcrx6pnpedbol9h01+jEbYGUcpz79XSjfGLOR0zsRmEVUst3cozbxiue9qm3wACr2BJIi3e/VUgj13VdR9+fOtfjexYQ1f7poloIjASTGwMQZrN9MKBgMKflAXHiRPBtcpfzQAQZ3+AB4d0OSFWjnvN+Y9NmbpTB8hJrVAwoFHwGqb4p0pU1NJvSpfpR6qyNRZPguG73CdaO3Xo7uw0QSN8V7S5t6fqwRGbODj2GMCRqqYdwwAAvVWGwOuqJ9RsPCtHscQYQDM/v5Ennd2tr3Lh+Q1yOV96zXBmQCOawk9bes/ckZf68patfeB64Lkj9ab520j+Y6NlytxZi6r6gHoRnidKRqiVP1Kg7Wp6nrou+Q8igfkSDv8VV+PSb9nIdPIdtbNWEir5Lx8vrk6A1QWghNTQY//14/a/YDC9Cr7HJ+kSDvHLMgwQsDPBeNW/gRYq/DGvoJpN6yYPWzo8w4fFe82fVm2GE+LYXQUCrlhzeVLVm4feRzeV7Oc7kz92nCnWcHyV9wMseXMKI/87U5U91Fl7xtJQex7Eq6sWQfB0hNPfjCbYmUvH9PQmrgv+2Cxp8hjCZtGmRask/Ekzu38sNlB9MDwRd8OO+ygMuelXgl8KdPRtL2dgne4m1jd0c1+U5Hoh/T1ilaruxdt1+l/mI8zCoQMDc33akxUt3ekST9u0/JZoFpqFuFxYBBvGlS76vwtnP9d/h9qB9UBemKkzY+PiLFcM94xAQgP0HO/8Qkfz9s14CEvcEQjVp5Wm+eWPX7uPyrKEVcR40JVwUb8A6AMGeSxUuzwoFAvX9jxvRyL+jPawY1K3bZJwHwSF4IEA2sCCyY+eu4/I76GeFz+nYkvL+3RGwmzJjA33HVgN+j1DQas4QIQNMUxn0IAcTCCbx0FXT6MDpo6wdY+UtpL2XjBOCaILL21PCs9Byrxf+46WG3+Atq6xN8RIIeY84iFI5PWXs9Duby//QrpMH6XzsBTp54Qxr1S3yHnO8PFETC3UHsovgAo81+PnWa5742iwMPmxfnJ5qmNfqM168WDUjkzQvvVD3c2oxa5CctnTvBjaxreMiPHCPbG08z0Kl2+IxdFDu7zL/f4waT+8nJj1eCeXqB7eZuWvpoNZxgsgNcwy+osucVINAf4aGGD56hartjEo1hos/i8GWMp2n5rYGTD3ROHYgEv4JaxoMPovYiKjXoHzsBpw94xxVhEn5FkigBiEI8QoP7pWrfIMT4yeucZm5KND6MONDBaLEOdhn1nEIuAJ4Rt7E0EAgzTsgiQJLJhUXENgfQEy7hYDPI8b4PvOqtUaK24PnjHrQru9nakhrAwTdvvq+kzwj3fYzLGx69fd0FUbh+m/Yrs/PzZ+lBcC9ivPjYtnRn31SITbMbaXN7uPSeYGv4AIqj+8omsw18JkIKVoWpstewbQ/R7cSYut6yWCCs1BoMLUP3fCzX3ymruznK+LilUwPslDYYXvp4dFzJ+lxaFcmstUu95friXnPwih9h5I0YvUMOsy+/7nY83Tj5g0WHKfp1abfiRUir3gSYmekD9oWoz2nPF+5rFFrcg+XdaD7cj9fY1uQAFuCyL10+S4JMoEo+sd9L5Pb9MVAQ7DHH0aOiXaZ9JD48Oveyeg+X/vtN7ymsRAMekoGqlUPAbP3eKBC4wZC/iLdpE3dPgg8jonsjZpsStoJgz4iCh8q1rBVkYItAx2HEFM5b1vHpB4IKpjK2s/FPcFSQAER0Y/y1fwHJqvVHuUSRCnT1qM3Wbh6C4NFS3ZIcE8LXkxVQphMnGIFfPyhYfN/PJ4DrlOLn40dQ0ew+2GzOvB8IWS9f7P4YM3BbfSQBLKUxub/H2RtOGHTAlXDE9dv3KD34LNWz+Ie+EyCX6IaqxoWhjPBIDD0e8nxP8zdLuwv+8OWo3uEQNDu0i5r2TQdf2VteUPVIJq9PVpIalkC1vXFDGdh8+uIpnTF4UWJv8KMr+h+DTOEUfJGX9M+1tb+cPjMcd+31fB9jl43W9VwRoLI3anHLB9/G1M67zLRMIAQzILP6wSYc6gPYkLDYfDXqDNazGXdFgZW/abj1RkWTp66QGmyNvGwFtCH7n3mqhr+8WPJvh6m+TPc/sjxK9VRC9fZh4Vpr/uB+8A5C5kgoaI3azf7cwEpqtR01jg63qAFJLT1x5+1FNNWNDcTEi5LtIOFAS34GZvauq/QsvC/vYmFWYjkNtcJn+FbB0PrDtO8yB1O1dgCsAOCDNe1B1Rxv8h9SAjwthUxcZl0GPiagINXTlU1fLHpyC66PyKnyyfVg35wtHtm4syl8/RCk29tAoCFBpOv9Jg2qoYzei9lc99uEfDncmPbqaMWmsyIFKGh64hA4usX6F9D1fDEqv2bRUi4+4t7/IhGrZ2lajgDAuV9CDF2N1z94et0X+wpcL2RIHKDuNovw0DM8ElziV6nZg0M8xekxZQOpknswFRMgR+7uQYcBs2U6etlvhxBFz1IkOU2aapnVlr/QYskkqzrgOQZcjWXRJXAuEnfFO7qqbl54I7x8ucx5Ya+ax8WwifjJy2CTp3ZAeLY7wMEcTKVAZDALghg1jZvN4WGDF8qPje0N9pycgswPQUNrzUyrlO38Th11ML6DQc9ZjMgRCCsIFSCoX6z8T7kRvDMDgiSH4r39LhfPD9cY/qs+KcLD4ieJBrN8p0tAv4S1UQddUa3RWM8yAXN9h+JSLvN2+bs54ZV8NSUmEO+ECSZpMiQ+kwgCBolNFjQ4L1qdiB4BpPadX12H5I3ykcnLsSoGp6AReEhMNjPztbD/Q52v2DfOl2HX30slN7LPBWgN+JN7piYC5Q2e1PXNBO0CxIXrlyOE0LDzBQfmgcTtLQdSGbQ/h8GNsxgEB7JE7o9CAuY2ocPn1FnWfiheC960RbocdLuTgA5JT7AZMV50MhoZ868LaqGBSSv2H1IDNpQtJzGufOxIgz0dWAqg3zez0Djl1L9PQJX0HpIs0WQBH679nkhiKZM36DOsjCE+/qMra8wtb1TdJFQAiGh64jZXDi0KcpfywzwIC3yBNCeNxDAfIEFtRYycC0wxw0XYd2GwIk0diB764P2JZQfygOXfdgn6+VmX9V/zAH4flBtpenVgOfzP2Y/WQN+92vsf3toPCZXBz/+u8alK7H0cvOCQlY5h4XGo3Vzy/vcNE5dPENPN/za5m+j7XTU0Y+pf+J8jETBdX1tZYxYM1PV8A9k4tn7YwmbjPTPRmd3RSPe5J63cJsMSpBE//DN2lgRxMIlmIDK/MXgmGGbQpo7fysPhBoyEDDQsnzRSiK18N/x/ZtqIEFYfP9zD3WWBWRUIYPqDSUANEFhLQQDouuwJLQGw/Wd/NOK4VE+Ue5e/eepo8GBgQ6tpZ+LmMrfdxJz3xvIVLMLSGg8ROl1NBnxAE1eTMthvt8+xYWIPBJNcBzC8P2PG9KRo57CsNAvPV2/BQrup13n6eqof8Dayv6lJaT1uRCko8d5ujEaiK4//VoVITbqQnDjulk+bymuVCiYu2OlaDHtuzqZwN44F3uBnm2cn8lim6dmctWe0lPVIJqxdZmQwNVuzRzyNtXjTLRAWLJnvZjP2keH0MjStbQ6amHK5sWqbXVtJt6LTb9ji8A5sAv/GGR21ed+P8f9P8v3EQwbDu8U/1+b84gD3Md/wy0JhHiTGxLcrjUQsNEktidVYDB17WX5XydOnhcTGqQGuV5iYiLYAyBzy26e4rMWFhpIHbUTB0E4aHunxBBv9Ow3z6N9+Kmf5/dMjoGQEatDRblhwsKkhV8ZKjp0nelxHRACmVtOEEEHAanqgsD52V1hD0IA7QpyQkCCLNDAA4culmOwkBCnQLAO54JIqGvH6TMX6SNYQzZrBc8vFGG4cvVeEUw6FoDfSzIF/SSgYIYAgTQIRvQV5+g+l6s6TNUKjIrjOrjMVZnKYvNzscry8gf4qXayJIP/yiREkEsjYlI3ITyOCylYgHzVt6o66h91p/b2PI8/15zcXR21ED6xq1ed9BK594fqUt9ukmekfP3D1dHA6LpwlOe57Fqkav2jzNsHQrzJXegXt3aG3weNCs0K2ImEH7tKTctE+aP8IBns+NFhlrdo6w54QEtDW+McFGjxWXM886ujRq8Q81TXARm8tbs/eLeP/jVoNkEdtQASg8x6QOuYQahpoACCV3ZNacUNnAdoy/beAq06NWk1SR21sGLlHiXQ6nF/aolwRH+2M8nEEmHSWedWo9Ydp6mzLGzddkSm0nQdS7s3okOHTqsa/gHBau8bnjVmLpwsEI39B07JjAeeoT4PAgWCf8HiIAKFBdlHnf9gYqopJ9aAMEFhGgfC570re/m72Sl5kwKi0TUsnziTuw4TJGKSJ0m9AVM+RcsfhEA4R4JefJ2pW+xrHG5Shs6lRLujTrJa3Ha1TB6CxRsFIiM8+8t9qTu1lzoaGFm6lxHB5T43HZUJEhAE4kVukPhDm3mMwQxfWAOrwaBtoJGgIQuX7E19IudLQAZmG45hoGgT8/Dh0+JfY/ChPZipyCo7yZreDpjH9gAPkmdKVx6ijvoHIs1IyXRpIf4fhFm1Zp+qYQFzuPYBDcFUgc30UAHLJHXWJq7nouMGh/j+nIDEGrsgAAlmegk0oHz1KJdQRDCxT+QCGjdhtauv/tyTVWv20mssELQrAhM7a+7WslIrEC7wcXv+An4zCGMI7WDAghctjPR9wYrDIpRAgImskz5k4PIgztmjvDrqjDkw47meNpvlPCaOXSsj+v6ud4SZCdV5of/pLwA+s4dG5n49xb415qc19p46RA8jMq6mpiBYsNAkkImdvUc5EQCudiunp64BpuI0FuxcJfcKiwbniWXDf8N1CIZ4kRtpnxiI+sfDILMHWrbvOGaZcVww8DDAU6WzNAiIC02P1UMaCAJBw+n2sPDDaYknBped3Ii2VwxhcUqxUv1FoOCct3Ee9/3n3/uqo278VtYzgIRrDRu5TB0NDqSn2s+XuEExZ8sCS1HtAg3PBILBKV0XSTyID6AOCla5fV2oi4t8CN7B/D7DZrgdIHeK993TVND0WP0Fkz4QOnX3nOKEUEibrWnIvjPm5e3PwRIqrXxmTexYd2i7lVCi/UnWtAUinaeSgLjrcZS6fXEZ4Jooch4Tsh6b0xrINHul2XeuIJSu02K2cyowgAw1BPKSYZ5dn8P9gZVgB7LkPPxn9r3z+Zn+0sjqTW7uS6sAfRGwVZMZiTR266PKR/RZ70qqQmDEi9xITHGRjMkLE3q2LeqMzLWPP0PE2O27atMQmgfTWXbAJ7WTFgOrvUPSyLBRyz3qQesVdSCpHZhS0v4/CiL40Cze88ZIg8VUno5yi3Bi4kFQhYqmrSdx/+xmdjXHVWAApvg8BBr3KdCa9ZbcDp4dtDcIDmJr3xZEcsr7xgo23IN+9hCsCOCdPu0/RoGlqjDlX+dr6L49lSI0ra0xd8FWEf46lgByI1EnkMWwbPd61q7Z3MEiNs/tEW9v/Dm6pRADPrYe8Fb0OAON2+Du6xU2r1O2KuIyr6VtJkmeftVUDU8cZ82cBkIDRGKt7DqHr4X5bDt+k3xy+xx4Omo/L7CygX8NIeA6hy2Nb9hUDwQIK7uvDSGYrHoWWrk/tGXBIZMbQR4sytCLKvSA8V6jXJA1lt3HhWkHYnoPQix8yFuos2v+WQd9dKDNjujVMK/dQR4MXGhif6uVMM+KJYg6Go3VVlgN1bilb742rgeC4fqoi/5gLj4YXD7oTZLVVvqeMbARLEPQzAmILnsLNATj/OHM2UuUkYUPppj0OfZzncgH8xoJMTrijT7BlRk91jnijZkDzF7gt8XvBeEBV+pbfg46ZRQZdRDOgfLssZjIPuOA54BgX6DpN2humL5acyermYv+w2XtwW2qhoW4a3FUcWw70ZiIND+EeWtFQjmXy5ajnnP4n/aq6KH1YMbfx2S3J7kA89n0fbfNTx7aWPoiwb3MtGCXO5sROepvtfnZJTSsXPZstPJAYMIJUSUzTbedix5kC2EuX9sJbeYMtqLx6rmIAKuQmupPC6zU7AiZ3Fgy+A7/8CAWfjh/GseeFomCqG7qLI3p8BHPqRoQE2TVeeWI7H6Uq7kMZm/oCLEWBCgYiBAOiO5iEchlNv0wSKExMagtDacit0wCDDIsYvAGLAW7KYprII6AQNDa9ftpw6aDtH7DAVq+cg/9M3mtZMSV+CtSstGAg/JcWKuq5+JP6AEY5HkL2gWaFQNYhAUvAaATW3Qf9blYoOMdP9DAIhLcF54B6sMyQTwDqaeY+8czxfTZYG4bS2fxe+JZoeAzFszYhSfyG97ke0SWIBa1QCgiBoP5/f0HYmQzC/uiGxQsBR02crlqwRlnL523VmHZTWH2k7H5wvA1M2XZ58AVkylDp98tYjPZoJELDqjlIhjOfZFNcO/ElJZs9mIFmE5gQREXgE31L9jUhhUgAoCFhZ4vf6TelxJEs9rNRi/I9Ja73fWHd9B9LIxclgb34XXuDyyFQICwwiyAPk+3/3SDPNRv2QTaf/oonWDrAevDvxtQU/oDQSfZeqjLxP5+YO2AgtIbIZMbmVZ2jYPPTvOm2IVFkwU+H4julMs8YjTacw9YSHl/SwuBwVFLZLBoDYtBCJJAKCDpIxeTH4ML1xPNzp9RFwGhr77r5HcJKhJwMJh1P3AezF+QDgLiDW4fVgCmh2Byor1HX6hIK1ZaWmLshNUezwVxA38uAzZygOmrs+DQd0TBvX1mb2DaDuml9n7Cj8aqOn/+LBarvKqeD+rjvuAuwcKAqYylnohwQ9NCGOI4BAHiGXiO3plsBw7FSKwAzxz3i+eB89EWIvFoB88N7eB6T7KlVPS3vn7X49vxI7LB2LR1kRDBKiYNtC4IAA0GYshnNtvn71hFxaIaucxcED5j51KqNTeOnTvJ5Mkrxz0IDm2LtiEs2HfH8QeY0NUndJJ5cB28AsEKMNHs6LpotOqrIii34Z3L7g/FkKEmi1ds98mCCffxWP0vZS243Cc0tj4OYSTErkVXHXLVAyFkckuGmBpcIA0G8dwFnqYTgGwqCbrxjwxtE+6VtqgBrY8cdD1YMWC857ftgMQqX22Y7FbiMre5wK/EQIfZqrWGmPg8iCFksPLKyRoA9u6zAlbeq9KsNqx4gS5iZXD72FXEvnkDtlCyL33FfTRt4zmtpQGtb7cS8DxDXXWG2AYIhH7gXHzG8wiEfgMXijCy5+3jfAgtWFT254hniEU5uQt0oI2bDqkW3Dh4+DSTlwWdOgft4G8RgDZtjWvBBYKAC+Tj2wFteD8GOQ9sOwmhQUE00Vzsoz7AhB+6xorJvNW6qFtzMwmRreaECRsXUDIWCKijN3uQtkFwaGw2lZ+q+wVN37ac+rAG9ZhPZhJjYwc7vmOSaS1v1Unvs7uKP2ArpbTIxGOyajcERe4PS0DFIlCChY9DcCTje8T2TQlBSORGauZ/nisvAxODF2YudiVdtMTXnNy05ZAEgLDlDzQqgmzeGMW+33+Su9tDwS6qtRsF3vHk2rUbksIK7YdrQMvAPMeAgkaB9kebCPRhEciYcc7+DIC2kGRx///KuvoQqKBdaCsIKz2VN4f9amxW8DT3RdfDfdRzSIvFyrW3Weg8/nIlV92H+BnkL9JV/PZQAHfgIf4dcC76ja2fgwFLQxFRh6CFHw2hh+eFe4FwQVvIIUBQEb5/rMM6dwDCFSvDYD3JM+ZnjfNRENjDdwgUwnfvE+J+c3ZEMWkfgZZiskE7i+aGFoN2ZZKla1/c5ftiPbfUAUmYXGF/vOGziYIds3dEU2aY9dD8lZnkSmM/y1r9jxFNaedJa0mufbpKzGfuz5Zj7vXpSDl9AEKh/IfWdbHpA2vZHcedXSMnnLp4looOqWdZDNgJhq8HN8S6XyWE+H5h+n/bP5yW7k34zr9ByY3AEfymzj1my3xwz35WwbTJ5i2++2tdYjOx36BF7MvO5OO+61Ox4AB7m3fyag/tT5nmmUPtD/CtYf6XKj9IMruQcZb3hy6Sr92ctf/8hdvoOpM3ELA5YTduA/uuD2eXI8pWhvL96n6hdO8zjwaxWwD/245/Jq+jDt1m+t6HVy44EL1yL7sxMzzqdus9l/3pZSFvRAhLo1P32XzuPDn32LHQNpFAxhoENBboIKnnCxZ8X7KrAquhFlsh4yetkecRCjAdCiH8S6l+VKREL2kDvwOsLuQ5IJiXUGw8upsqjGlDH3X8jTXzT/Qha7kfB9el4aumSSBL4+T5GGozZwi15tJm7lBqNaM/zdvhHCy0Y+3BrTSa/fihK6fS3O3RdOK8e6nwZr72A3ZfmsmH9dx2HD13glrOGui+Ln/GZhH2ZaChYtHutVR9fCfK2b0svdv2Z3q7TVFZUvpdZIRsAmnfZDGhCNksv5sRdzU0chhYgNVy43r8NGtiIw7+ZTy1/62g1KhWoo21qQwLoutCZ5fydgMLZ+Kuxc+fDgVJgtwGBreChaxF72ez2KW12f/Hii+Y4fcyDLkN/l9j45Fd9FKT/LZg3mfiSzeY7rzZyL0EQ26DJIl284ZR/UndaePhnZIA4w3kindbOIqerv+V+NdWQgwTu2omStWqiKyhvtdhyG2Q5HDp6mVZIRZW7n3JXEOwKk/falRiRDMqObKZrBZLDm2NqDmSZzSx2TR/sEY2WhzCoox7AYbcBkkOUaunC3Gt+eNc1jSTTlpBweeI7O45b9SrkoEejshB4zaE9kKIewGG3AZJCphfz9qtNIWVftua12bNrUmsC/6WLDXJDsO8d3rK2vkPig5xQca9AkNug6QFJvfMbcvo5yH1rSWfyEKrmlEILIknKCA0a/PnG+en7yMjaLTsPnp3Tw0mBIbcBkkWFy5fkAUb/7CpHbliEvVZ/g/1XTGR/l47m1bs20inL1o7CCVVGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRRGHIbGCRJEP0fYv2GdMyEcoMAAAAASUVORK5CYII="));
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

                    var imagen = httpContextService.ObtenerLogoImagen();

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

        public string GenerarZipPliego(int idSolp, string pathBase, out string mimeType)
        {
            Solp solp = repositorio.Obtener<Solp>(idSolp) ?? throw new ArgumentException("Invalid Solp ID");
            string middleFileName = solp.NroSolp ?? (solp.Pliego.NombreObra ?? "xxxx");
            string pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.pdf";
            string pdfFilePath = $"{pathBase}/{pdfFilename}";
            bool pdfPliegoDisponible = false;

            if (solp.TipoSolp?.Codigo == "CON_PLIEGO"
                || (solp.Urgencia == true && solp.TrabajoYaHecho != true)
                || solp.TipoSolpSap == (int)TipoSolpSap.Sap
                || solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento)
            {
                File.WriteAllBytes(pdfFilePath, GenerarSolpPdf(idSolp));
                pdfPliegoDisponible = true;
            }

            if (solp.Pliego.Archivos?.Any<Archivo>(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp) == true)
            {
                string filePath = AgregarArchivosAlZipPliego(solp.Pliego.Archivos, middleFileName, pathBase, pdfPliegoDisponible, pdfFilePath, pdfFilename);
                mimeType = CustomMediaTypeNames.Application.Zip;
                return filePath;
            }

            mimeType = CustomMediaTypeNames.Application.Pdf;
            return pdfFilePath;
        }

        public string AgregarArchivosAlZipPliego(IEnumerable<Archivo> archivos, string middleFileName, string pathBase, bool pdfPliegoDisponible, string pdfFilePath = null, string pdfFilename = null)
        {
            var zipFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (var archivoSubido in archivos)
                    {
                        if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp))
                        {
                            string fileName = Path.GetFileName(archivoSubido.Ruta);
                            archivo.CreateEntryFromFile(archivoSubido.Ruta, fileName);
                        }
                    }

                    if (pdfPliegoDisponible)
                    {
                        if (string.IsNullOrWhiteSpace(pdfFilename)) { throw new ArgumentNullException(nameof(pdfFilename)); }
                        if (string.IsNullOrWhiteSpace(pdfFilePath)) { throw new ArgumentNullException(nameof(pdfFilePath)); }
                        archivo.CreateEntryFromFile(pdfFilePath, pdfFilename);
                    }
                }
            }

            return filePath;
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

            if ((lista == null || !lista.Any()) && valor.Length >= 7 && valor.All(char.IsDigit))
            {
                string codServicio = valor.PadLeft(18, '0');
                this.ActualizarServicioSolpDadoCodigo(codServicio);
                var servicioAgregado = this.repositorio.Obtener<ServicioSolp>(s => s.Codigo == codServicio);
                if (servicioAgregado != null)
                    lista.Add(new ServicioSolpDto(servicioAgregado));
            }

            return lista;
        }

        /// <summary>
        /// Método utilizado para retornar los proveedores disponibles en BD - Utilizado en AutoComplete.
        /// </summary>
        /// <returns></returns>
        public List<ProveedorDto> AutocompleteProveedor(string valor)
        {
            List<ProveedorDto> lista;
            //Atributos minimos que seran utilizados en FE, pueden traerse mas de ser necesario (Ver Proveedor/ProveedorDto).
            //Ver si la busqueda es por Codigo de proveedor o por Razon Social.(tener en cuenta que hay codigos de proveedor que
            //empiezan con 'C').
            string entrada = valor;
            if (entrada.StartsWith("C"))
            {
                entrada = entrada.Substring(1);
            }
            double i;

            if (double.TryParse(entrada, out i))
            {
                //Buscar por código proveedor
                lista = repositorio.Listar<Proveedor, ProveedorDto>(x =>
                   new ProveedorDto
                   {
                       Id = x.Id,
                       CUIT = x.CUIT,
                       RazonSocial = x.RazonSocial,
                       CodigoProveedor = x.CodigoProveedor,
                       IdTipoProveedor = x.TipoProveedor.Id
                   }, e => e.CodigoProveedor.ToString().Contains(valor));
            }
            else
            {
                //Buscar por Razon Social
                lista = repositorio.Listar<Proveedor, ProveedorDto>(x =>
                   new ProveedorDto
                   {
                       Id = x.Id,
                       CUIT = x.CUIT,
                       RazonSocial = x.RazonSocial,
                       CodigoProveedor = x.CodigoProveedor,
                       IdTipoProveedor = x.TipoProveedor.Id
                   }, e => e.RazonSocial.ToString().Contains(valor));
            }

            // Filtra los que no cumplen con la forma.
            lista = lista.Where(p => p.CodigoProveedor.Substring(p.CodigoProveedor.Length - 8) == p.CUIT.Substring(2, 8)).ToList();

            // Filtra los Proveedores que sean Tipo Corredores o Clientes.
            lista = lista.Where(p => p.IdTipoProveedor != (int)TipoUsuarioEnum.Cliente && p.IdTipoProveedor != (int)TipoUsuarioEnum.Corredor).ToList();

            return lista;
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

            if (solp == null) { return; }

            var enviarMail = solp.SeEnvioMailLiberacion != true;
            solp.FechaLiberacionSap = fechaLiberacion;
            solp.EstadoSolpSap_Id = estadoSolpSapLiberada;
            solp.SeEnvioMailLiberacion = true;
            repositorio.GuardarCambios();
            var posicionesSolp = solp.Posiciones.Select(x => x.Id);
            var peticiones = repositorio.Listar<PeticionDeOferta>(peti => peti.Posiciones.Select(x => x.SolpPosicion.Id)
            .Any(posi => posicionesSolp.Contains(posi))).ToList();

            if (!peticiones.Any())
            {
                bool tieneAcuerdoMarco = solp.Posiciones.Any(x => !string.IsNullOrEmpty(x.NumeroContratoSuperior));

                if ((solp.TrabajoYaHecho == true || solp.ConPresupuesto) && !tieneAcuerdoMarco)
                {
                    CrearCotizacionConTrabajoYaHechoOPresupuestado(solp);
                }

                if (((solp.TrabajoYaHecho != true && !solp.ConPresupuesto && solp.Adicional == true) || solp.CondEspProveedorAsignado == true) && !tieneAcuerdoMarco)
                {
                    CrearPeticionAutomatica(solp, new List<int> { solp.ProveedorAsignado_Id.Value }, null, false);
                }
            }

            var cotizaciones = repositorio.Listar<Cotizacion>(coti => coti.CotizacionPosiciones.Any(posicion => posicion.PeticionDeOfertaSolpPosicion
                            .SolpPosicion.Solp.NroSolp == solp.NroSolp) && coti.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado);

            var esServicio = solp.Posiciones.Any() && solp.Posiciones.First().TipoPosicion.Codigo == "SERVICIO";

            if (esServicio && solp.Posiciones.Any(x => x.Peticiones.Any()))
            {
                var actualizarEstadoCotizacion = solp.TrabajoYaHecho != true && !solp.ConPresupuesto;
                ActualizarPeticionDeOfertaAlEditarSolp(solp, actualizarEstadoCotizacion);
                if (cotizaciones?.Count > 0 && solp.TieneModificaciones == true)
                {
                    if (solp.TrabajoYaHecho != true && !solp.ConPresupuesto)
                    {
                        solp.TieneModificaciones = false;
                        if (solp.EnvioCircularA != null && solp.EnvioCircularA != EnviarCircularEnum.NoEnviar)
                        {
                            EnviarCircularAutomatico(solp);
                        }
                    }
                    else
                    {
                        AgregarPosicionACotizacionTrabajoYaHechoOPresupuestado(cotizaciones, solp);
                    }
                }
            }

            if (solp.UsuarioCompras != null && esServicio && enviarMail &&
                (solp.Urgencia != true || solp.Urgencia == true && (solp.TrabajoYaHecho == true || solp.ConPresupuesto)))
            {
                try
                {
                    emailComprasService.EnviarMailSolpLiberada(solp);
                }
                catch (Exception e)
                {
                    Log.Error($"Error ActualizarFechaLiberacion.EnviarMailSolpLiberada Nro de SOLP: {solp.NroSolp}", e);
                }
            }

            if (DeboMarcarRevisionesTecnicasComoNoFinalizadas(solp))
            {
                MarcarRevisionesTecnicasComoNoFinalizadas(solp);
            }

            repositorio.GuardarCambios();
        }

        private static bool DeboMarcarRevisionesTecnicasComoNoFinalizadas(Solp solp)
        {
            return solp.EnvioCircularA != EnviarCircularEnum.NoEnviar && solp.EnvioCircularA != null;
        }

        private static void MarcarRevisionesTecnicasComoNoFinalizadas(Solp solp)
        {
            List<PeticionDeOfertaRevisionTecnica> revisionesTecnicas =
                    solp.Posiciones?
                    .Where(posicion => posicion.Peticiones?.Count > 0)
                    .SelectMany(posicion => posicion.Peticiones)
                    .Where(posicionPeticionDeOferta => posicionPeticionDeOferta.PeticionDeOferta?.RevisionTecnica != null)
                    .Select(x => x.PeticionDeOferta.RevisionTecnica)
                    .Distinct()
                    .ToList();

            if (revisionesTecnicas?.Count > 0)
            {
                foreach (PeticionDeOfertaRevisionTecnica revisionTecnica in revisionesTecnicas)
                {
                    revisionTecnica.Finalizada = false;
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

                if (!string.IsNullOrEmpty(solp?.Pliego.SupervisorTrabajo))
                {
                    //Supervisor
                    copia.Add(solp.Pliego.SupervisorTrabajo);
                    Log.Info($"Copia mail responsable de trabajo paso 2 {solp.Pliego.SupervisorTrabajo}");
                }

                var asunto = $"Nueva SOLP de urgencia Finalizada - {solp.NroSolp} - {usuarioCreacion.ObtenerRazonSocial()}";

                var usuariosComprasHabilitados = repositorio.Listar<UsuarioCompras>(x => x.Habilitado);

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

        public void ActualizarFechaLiberacionOC(string nroOc, DateTime fechaLiberacion)
        {
            try
            {
                var configuracion = repositorio.Obtener<Configuracion>(x => x.Code == "EnvioMailLiberacionOC");
                if (configuracion != null && configuracion.Value == "1")
                {
                    var adjudicaciones = repositorio.Listar<Adjudicacion>(a => a.NumeroOrdenDeCompra == nroOc);
                    Log.Info($"Encontradas {adjudicaciones.Count} adjudicaciones para la OC: {nroOc}");

                    foreach (var adjudicacionOC in adjudicaciones)
                    {
                        adjudicacionOC.FechaLiberacionSap = fechaLiberacion;
                    }
                    if (adjudicaciones.Count > 0)
                    {
                        repositorio.GuardarCambios();
                    }

                    //Se deja comentado este codigo hasta que se deshaiblite el envio de mail desde SAP.
                    //if (adjudicaciones.Count > 0)
                    //{
                    //    try
                    //    {
                    //        EnviarMailOrdenCompra(adjudicaciones.Last(), "");
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Log.Error($"Error al enviar mail ActualizarFechaLiberacionOC. Adjudicacion_Id: {adjudicaciones.Last().Id}", e);
                    //    }
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        EnviarMailOrdenCompraSAP(nroOc);
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Log.Error($"Error al enviar mail EnviarMailOrdenCompraSAP ActualizarFechaLiberacionOC. Nro OC: {nroOc}", e);
                    //    }
                    //}


                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public void EnviarMailOrdenCompraSAP(string nroOc)
        {
            var adjudicacionMail = ObtenerDatosParaEnviarMailOrdenCompraSAP(nroOc);
            var asunto = $"Nueva OC creada - {nroOc} - {adjudicacionMail.RazonSocial}";
            var pdf = comprasServiceSap.ObtenerPDFOrdenCompra(nroOc);
            emailService.EnviarMail(adjudicacionMail.EnviarA, asunto, "", adjudicacionMail.Copia, CuerpoMailOrdenCompra(nroOc, ""), pdf, $"Orden de Compra {nroOc}.pdf");
        }

        public AdjudicacionMailDto ObtenerDatosParaEnviarMailOrdenCompraSAP(string nroOc)
        {
            var ordenDeCompra = ObtenerOrdenDeCompra(nroOc);
            var nroSolps = ordenDeCompra.Posiciones.Select(x => x.NroSolp).ToList();
            var solps = repositorio.Listar<Solp>(x => nroSolps.Contains(x.NroSolp));
            var copia = new List<string> { };
            var enviarA = new List<string> { };

            var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList() ?? new List<string>();
            mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
            var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList() ?? new List<string>();

            copia.AddRange(mailPliego.Where(x => x != null));
            copia.AddRange(mailCreador.Where(x => x != null));

            enviarA.Add(ordenDeCompra.Cabecera.MailProveedor);

            var adjudicacionMail = new AdjudicacionMailDto
            {
                Copia = copia.Distinct().ToList(),
                EnviarA = enviarA?.Distinct().ToList(),
                RazonSocial = ordenDeCompra.Cabecera.RazonSocialProveedor,
            };

            return adjudicacionMail;
        }

        public void ActualizarServiciosSolp()
        {
            var servicios = comprasServiceSap.ObtenerServiciosSapRaw();
            ProcesarServiciosSolp(servicios);
        }

        public void ActualizarServicioSolpDadoCodigo(string codigo)
        {
            var servicios = comprasServiceSap.ObtenerServicioSapRawPorCodigo(codigo);
            ProcesarServiciosSolp(servicios);
        }

        private void ProcesarServiciosSolp(List<Servicio> servicios)
        {
            if (servicios == null || !servicios.Any())
            {
                return;
            }

            List<ServicioSolp> listaBase = repositorio.Listar<ServicioSolp>();
            int agregados = 0;
            int actualizados = 0;

            foreach (Servicio servicio in servicios)
            {
                if (int.TryParse(servicio.Codigo, out int codigoNum))
                {
                    var serv = listaBase.FirstOrDefault(x => x.CodigoSap == codigoNum);
                    if (serv == null)
                    {
                        repositorio.Agregar(new ServicioSolp
                        {
                            Codigo = servicio.Codigo,
                            CodigoSap = codigoNum,
                            Descripcion = servicio.Descripcion,
                            GrupoArticulos = int.TryParse(servicio.NroGrupo, out int grupoArticulos) ? grupoArticulos : (int?)null,
                            TipoServicio = servicio.Serv,
                            AmbitoServicio = servicio.Ser,
                            Edicion = int.TryParse(servicio.Edit, out int edicion) ? edicion : 0,
                            UnidadMedidaBase = servicio.Bas,
                            SSCItem = servicio.SSCItem
                        });
                        agregados++;
                    }
                    else
                    {
                        serv.Descripcion = servicio.Descripcion;
                        serv.GrupoArticulos = int.TryParse(servicio.NroGrupo, out int grupoArticulos) ? grupoArticulos : (int?)null;
                        serv.TipoServicio = servicio.Serv;
                        serv.AmbitoServicio = servicio.Ser;
                        serv.Edicion = int.TryParse(servicio.Edit, out int edicion) ? edicion : 0;
                        serv.UnidadMedidaBase = servicio.Bas;
                        serv.SSCItem = servicio.SSCItem;
                        actualizados++;
                    }
                }
            }
            Log.Info($"items agregados: {agregados}");
            Log.Info($"items actualizados: {actualizados}");
            repositorio.GuardarCambios();
        }

        public void ObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest)
        {
            try
            {
                Debug.WriteLine($"ObtenerSolpesDesdeSAPJob INICIO - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");
                Log.Info($"ObtenerSolpesDesdeSAPJob INICIO - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");

                if (string.IsNullOrEmpty(obtenerSolpRequest.NumeroSolp))
                    throw new ArgumentNullException(nameof(obtenerSolpRequest.NumeroSolp), "NumeroSolp no puede ser vacio");
                ObtenerSolpSAPResponse result = comprasServiceSap.ObtenerSolpSap(obtenerSolpRequest);
                Log.Info(JsonConvert.SerializeObject(result));

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
                     TablasSap.CentroBeneficio,
                     TablasSap.CuentasSolpSap,
                 };

                var tablaSap = repositorio.Listar<TablaSap>(x => tablasSapAConsultar.Contains(x.Tabla));

                if (result.TipoImputaciones.Count > 0)
                {
                    var listaSap = new List<TablaSapDto>();
                    var imputacionesTemp = result.TipoImputaciones.ToList();
                    foreach (var impTemp in imputacionesTemp)
                    {
                        if (!String.IsNullOrEmpty(impTemp.IdOrden))
                        {
                            var existeOrden = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.OrdenSolpSap && x.Codigo == impTemp.IdOrden).ToList();
                            if (existeOrden.Count == 0)
                            {
                                listaSap.AddRange(comprasServiceSap.ObtenerOrdenesSap(impTemp.IdOrden));
                            }
                            else
                            {
                                ordenes.AddRange(existeOrden);
                            }
                        }
                    }
                    // agrego el resultado a la lista de ordenes de ot para usar
                    ordenes.AddRange(tablaSapService.ActualizarTablaSap(listaSap, TablasSap.OrdenSolpSap));
                }

                IList<Solp> solpsFinales = new List<Solp>();

                List<TipoSolpPosicionSAP> tiposSolpPosicionSAP = repositorio.Listar<TipoSolpPosicionSAP>();
                List<SustitucionMOAModel.Entities.TipoImputacionSAP> tiposImputacionSAP = repositorio.Listar<SustitucionMOAModel.Entities.TipoImputacionSAP>();

                var materialesParaConsultar = result.Posiciones.Select(x => x.Material).ToList();
                List<MaterialSolp> materialesSap = new List<MaterialSolp>();
                if (result.Posiciones.Any(x => x.Tipo == "0"))
                {
                    materialesSap = repositorio.Listar<MaterialSolp>(x =>
                        materialesParaConsultar.Contains(x.Codigo)
                    ).ToList();
                }

                var usuarioParaConsultar = result.Posiciones.Select(x => x.UsuarioCreado).ToList();

                List<UsuarioDto> usuarios = repositorio.Listar<Usuario, UsuarioDto>(
                    a => new UsuarioDto { Id = a.Id, UsuarioSap = a.UsuarioSap },
                    a => a.UsuarioSap != null && a.UsuarioSap != "" && usuarioParaConsultar.Contains(a.UsuarioSap)).ToList();


                List<TablaSap> centros = tablaSap.Where(x => x.Tabla == TablasSap.Centro).ToList();
                List<TablaSap> monedas = tablaSap.Where(x => x.Tabla == TablasSap.Moneda).ToList();
                List<TablaSap> almacenes = tablaSap.Where(x => x.Tabla == TablasSap.Almacen).ToList();
                List<TablaSap> gruposCompras = tablaSap.Where(x => x.Tabla == TablasSap.GrupoCompras).ToList();
                List<TablaSap> gruposArticulos = tablaSap.Where(x => x.Tabla == TablasSap.GrupoArticulo).ToList();
                List<TablaSap> clasesDeDocumento = tablaSap.Where(x => x.Tabla == TablasSap.ClaseDocumento).ToList();
                List<TablaSap> centrosDeCosto = tablaSap.Where(x => x.Tabla == TablasSap.CecoSolpSap).ToList();
                List<TablaSap> centrosDeBeneficio = tablaSap.Where(x => x.Tabla == TablasSap.CentroBeneficio).ToList();
                List<TablaSap> cuentasSolpesSap = tablaSap.Where(x => x.Tabla == TablasSap.CuentasSolpSap).ToList();
                var listaEstadosSolpSap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.EstadoSolpSap).Select(x => new TablaSapDto(x)).ToList();
                List<ServicioSolp> listaServicioSolp = repositorio.Listar<ServicioSolp>();
                int? estadoIncompletoId = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == "INCOMPLETO")?.Id;
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
                            TipoSolpSap getTipoSolpSap()
                            {
                                if (tipoImputacion != null && !string.IsNullOrEmpty(tipoImputacion.IdOrden) && posicion.OrigenCreacion == "F")
                                {
                                    return TipoSolpSap.Mantenimiento;
                                }
                                if (posicion.OrigenCreacion == "B" || posicion.OrigenCreacion == "U")
                                {
                                    return TipoSolpSap.ReposicionAutomatica;
                                }
                                return TipoSolpSap.Sap;
                            }

                            solp = solpdsDB.SingleOrDefault(s => s.NroSolp == posicion.NumeroSolicitud) ??
                                    new Solp
                                    {
                                        FechaCreacion = DateTime.Now,
                                        EstadoDocumento_Id = estadoIncompletoId,
                                        NroSolp = posicion.NumeroSolicitud,
                                        ClaseDocumento_Id = clasesDeDocumento.SingleOrDefault(cd => cd.Codigo == posicion.TipoDocumento)?.Id,
                                        EstadoPasos = "0,0,0,0,1",
                                        TipoSolpSap = (int)getTipoSolpSap(),
                                        Pliego = new Pliego
                                        {
                                            SupervisorSector = string.Empty,
                                            SupervisorTrabajo = string.Empty,
                                            JornadaLaboralDias = string.Empty,
                                            Usuario_Id = usuarios.FirstOrDefault(u => u.UsuarioSap == posicion.UsuarioCreado)?.Id,
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
                                var usuario = usuarios.FirstOrDefault(a => a.UsuarioSap == posicion.UsuarioCreado);
                                if (usuario != null)
                                {
                                    solp.UsuarioCreacion_Id = usuario.Id;
                                }
                            }
                        }
                        var estadoSolpSap = listaEstadosSolpSap.FirstOrDefault(x => x.CodigoSap == posicion.EstadoSolpSap);
                        if (estadoSolpSap != null)
                        {
                            solp.EstadoSolpSap_Id = estadoSolpSap.Id;
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

                        var cotizaciones = repositorio.Listar<Cotizacion>(coti => coti.CotizacionPosiciones.Any(p => p.PeticionDeOfertaSolpPosicion
                                .SolpPosicion.Solp.NroSolp == solp.NroSolp) && coti.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado);

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
                            if (cotizaciones != null && cotizaciones.Count > 0)
                            {
                                ActualizarTieneModificaciones(solp);
                            }
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
                            var datosContratoMarco = comprasServiceSap.ObtenerContratoMarco(posicion.NumeroContratoMarco, posicion.CentroLogistico);
                            posicionEntity.NumeroContratoSuperior = posicion.NumeroContratoMarco;
                            posicionEntity.NumeroPosicionContratoSuperior = posicion.PosicionContratoMarco;
                            posicionEntity.ProveedorFijo = posicion.ProveedorFijo;
                            posicionEntity.NombreProveedor = datosContratoMarco.Any() && !string.IsNullOrEmpty(datosContratoMarco.First().NombreProveedor) ?
                                                             datosContratoMarco.First().NombreProveedor : posicion.ProveedorFijoRazonSocial;
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
                                CompletarTipoImputacion(ordenes, centrosDeCosto, centrosDeBeneficio, tipoImputacion, tipoImputacionPosicion, posicionEntity);
                            }

                            var material = materialesSap.Where(a => a.CodigoSap == posicion.Material && a.Centro_Id == posicionEntity.Centro_Id).FirstOrDefault();
                            posicionEntity.MaterialSolp_Id = material?.Id;

                            //posicionEntity.FechaLiberacion = posicion.FechaEstimadaLiberacionDate; es lo mismo estimada que no estimada??

                        }

                        IList<SuposicionServicioSAP> subPosicionesDeLaPosicion =
                            result.ServiciosSuposiciones.Where(x => x.NumeroPosicion == posicion.NumeroPosicion &&
                                                                    x.NumeroSolicitud == posicion.NumeroSolicitud).ToList();
                        var numerosExistentes = subPosicionesDeLaPosicion.Select(a => Int32.Parse(a.SumeroSubPosicion) / 10).ToList();

                        var eliminadas = posicionEntity.Subposiciones.Where(a => !numerosExistentes.Contains(a.Numero)).Select(a => a.Numero);

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
                                subPosicionEntity = posicionEntity.Subposiciones.Where(a => a.Numero == indiceSubPosicion).SingleOrDefault();
                            }

                            if (subPosicionEntity == null)
                            {
                                subPosicionEntity = new SolpSubposicion();
                                posicionEntity.Subposiciones.Add(subPosicionEntity);

                                if (cotizaciones != null && cotizaciones.Count > 0)
                                {
                                    ActualizarTieneModificaciones(solp);
                                }
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
                        {
                            repositorio.Agregar(solp);
                        }
                    }
                    catch (Exception e)
                    {
                        var asunto = $"Error al agregar la SOLP {posicion.NumeroSolicitud} - NumeroPosicion {posicion.NumeroPosicion}";
                        Log.Error(asunto, e);
                        ErroToMail(e, asunto);
                    }
                }


                if (subPosicionesBorradas.Any())
                {
                    IEnumerable<Expression<Func<SolpSubposicion, object>>> inc = new HashSet<Expression<Func<SolpSubposicion, object>>>
                    {
                        solpSubposicion => solpSubposicion.Cotizaciones,
                    };
                    List<SolpSubposicion> subposborradas = repositorio.Listar<SolpSubposicion>(x => subPosicionesBorradas.Contains(x.Id), includes: inc);

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
                SetNombreDePedido(solp);

                string observaciones = result.ObservacionesGeneracion;

                if (solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento && !string.IsNullOrEmpty(result.Posiciones.FirstOrDefault().NumeroRequerimientoInterno))
                {
                    ProcesarCondicionEspecial(result.Posiciones.FirstOrDefault(), solp, result.TipoImputaciones.FirstOrDefault(dir => dir.NumeroSolicitud == result.Posiciones.FirstOrDefault().NumeroSolicitud && dir.NumeroPosicion == result.Posiciones.FirstOrDefault().NumeroPosicion));

                    repositorio.GuardarCambios();

                    CompletarObservacionSegunCondEspOT(solp, observaciones);
                    GrabarArchivosSapEnPliego(solp, result.Archivos);

                    if ((result.Archivos.Count == 0 || solp.Pliego.Archivos == null) && ValidarCondicionEspecialArchivosYObservaciones(solp))
                    {
                        EnviarMailErrorCondicionEspecial(solp, $"Se generó la SOLP con condiciones especiales. " +
                            $"Recuerde ingresar un adjunto para completar la SOLP.");
                    }

                    if (string.IsNullOrEmpty(result.ObservacionesGeneracion) && ValidarCondicionEspecialArchivosYObservaciones(solp))
                    {
                        EnviarMailErrorCondicionEspecial(solp, $"Se generó la SOLP con condiciones especiales. " +
                            $"Recuerde ingresar la justificación para completar la SOLP.");
                    }
                    if (ValidarIncopatibilidadSeraUsadoEnPliegoMultipleConOtrasCondEsp(solp))
                    {
                        EnviarMailErrorCondicionEspecial(solp, $"Se generó la SOLP con condiciones especiales. " +
                            $"Recuerde marcar únicamente a usar en Pliego Múltiple o alguna condición especial para completar la SOLP");
                    }
                }
                else
                {
                    CompletarObservacionSegunCondEsp(solp, observaciones);
                }

                repositorio.GuardarCambios();
                ValidarSolpAnulada(obtenerSolpRequest.NumeroSolp);
                Log.Info($"ObtenerSolpesDesdeSAPJob FIN - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");

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
            catch (Exception e)
            {
                var asunto = $"ObtenerSolpesDesdeSAPJob ERROR - NumeroSolp: {obtenerSolpRequest.NumeroSolp}";
                Log.Error(asunto, e);
                throw;
            }
            Debug.WriteLine($"ObtenerSolpesDesdeSAPJob FIN - NumeroSolp: {obtenerSolpRequest.NumeroSolp}");
        }

        private void ErroToMail(Exception e, string asunto)
        {
            try
            {
                string errorMessage = $"Error: {e.Message}\nStack Trace: {e.StackTrace}\n";
                if (e.InnerException != null)
                {
                    errorMessage += $"Inner Exception: {e.InnerException.Message}\nInner Stack Trace: {e.InnerException.StackTrace}\n";
                }
                emailService.EnviarMail(new SustitucionMOAUtils.Email.EmailSenderData
                {
                    Asunto = asunto,
                    Cuerpo = errorMessage,
                    Mails = EmailEnvioErrores.Split(';').ToList()
                });
            }
            catch (Exception ex)
            {
                Log.Error("Error al enviar mail de error", ex);
            }
        }

        private void CompletarObservacionSegunCondEsp(Solp solp, string observaciones)
        {
            if (string.IsNullOrEmpty(observaciones) && ValidarCondicionEspecial(solp))
            {
                solp.Pliego.ObservacionesCotizacionCondEsp = observaciones;
            }
            else
            {
                solp.Pliego.ObservacionesGeneracion = observaciones;
            }
        }

        private void CompletarObservacionSegunCondEspOT(Solp solp, string observaciones)
        {
            if (!ValidarCondicionEspecial(solp))
            {
                solp.Pliego.ObservacionesGeneracion = observaciones;
            }
            else
            {
                solp.Pliego.ObservacionesCotizacionCondEsp = observaciones;
            }
        }

        private static void CompletarTipoImputacion(List<TablaSap> ordenes, List<TablaSap> centrosDeCosto, List<TablaSap> centrosDeBeneficio, SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacion, SustitucionMOAModel.Entities.TipoImputacionSAP tipoImputacionPosicion, SolpPosicion posicionEntity)
        {
            switch (tipoImputacionPosicion.Descripcion)
            {
                case "ordenDeOt":
                case "ordenDeInversion":
                    var orden = ordenes.Where(a => a.Codigo == tipoImputacion.IdOrden).FirstOrDefault();
                    posicionEntity.ValorTipoImputacion_Id = orden?.Id;
                    break;
                case "centroDeCosto": //Centro de costo
                    var centrodecost = centrosDeCosto.Where(a => a.Codigo == tipoImputacion.CentroDeCosto).FirstOrDefault();
                    posicionEntity.ValorTipoImputacion_Id = centrodecost?.Id;
                    break;
                case "siniestroBeneficio":
                    var centroBeneficio = centrosDeBeneficio.Where(a => a.Codigo == tipoImputacion.CentroDeBeneficio).FirstOrDefault();
                    posicionEntity.ValorTipoImputacion_Id = centroBeneficio?.Id;
                    break;
                default:
                    break;
            }
        }

        public void ReiniciarCondicionEspecial(Solp solp)
        {
            solp.Adicional = false;
            solp.CondEspProveedorAsignado = false;
            solp.TrabajoYaHecho = false;
            solp.CertificacionAutomatica = false;
            solp.THServicioPermanente = false;
            solp.THAjustePolinomica = false;
            solp.THProveedorDirecto = false;
            solp.Urgencia = false;
        }

        private static void CompletarPrefijoCondicionEspecial(Solp solp)
        {
            string prefijo = solp.ConfigurarPrefijos();

            foreach (var posicion in solp.Posiciones)
            {
                string textoOriginal = posicion.Tarea;
                string textoModificado = textoOriginal;
                if (PrefijoCondicionEspecial.TienePrefijo(textoModificado, out string prefijoAnterior))
                {
                    textoModificado = textoOriginal.Substring(prefijoAnterior.Length).Trim(); // no se usa replace ya que reemplaza todas las ocurrencias y no sólo la primera
                }

                posicion.Tarea = (prefijo + textoModificado);

                if (posicion.Tarea.Length > 40)
                {
                    posicion.Tarea = posicion.Tarea.Substring(0, 40);
                }
            }
        }

        private void ProcesarCondicionEspecial(PosicionSolpSAP posicion, Solp solp, SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacion)
        {
            CompletarCondicionEspecial(posicion.NumeroRequerimientoInterno, solp);

            try
            {
                if (!ValidarCondicionEspecial(solp)) { return; }

                CompletarPrefijoCondicionEspecial(solp);

                if (string.IsNullOrEmpty(posicion.ProveedorDeseado) && solp.Adicional != true)
                {
                    throw new WSCustomException("Debe ingresar un proveedor");
                }

                if (solp.Adicional != true)
                {
                    var proveedor = usuarioService.ObtenerYCrearProveedorCompras(posicion.ProveedorDeseado);
                    solp.ProveedorAsignado_Id = proveedor.Usuario_Id;
                }
                else
                {
                    ProcesarOrdenDeCompra(solp, tipoImputacion);
                }
            }
            catch (WSCustomException ex)
            {
                EnviarMailErrorCondicionEspecial(solp, ex.Message);
                ReiniciarCondicionEspecial(solp);
            }
        }

        private void ProcesarOrdenDeCompra(Solp solp, SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacion)
        {
            if (string.IsNullOrEmpty(tipoImputacion.NumeroOrdenDeCompra) && solp.Adicional == true)
            {
                EnviarMailErrorCondicionEspecial(solp, "Debe ingresar una orden de compra");
                ReiniciarCondicionEspecial(solp);
                return;
            }

            var ordenDeCompra = ObtenerOrdenDeCompra(tipoImputacion.NumeroOrdenDeCompra);

            if (!solp.Posiciones.All(x => x.Moneda.CodigoSap == ordenDeCompra.Cabecera.Moneda))
            {
                EnviarMailErrorCondicionEspecial(solp, $"La moneda de la OC {ordenDeCompra.Cabecera.Moneda} no es compatible con la moneda de la SOLP {solp.Posiciones.FirstOrDefault().Moneda.CodigoSap}");
                ReiniciarCondicionEspecial(solp);
                return;
            }

            solp.NroOrdenDeCompraAdicional = tipoImputacion.NumeroOrdenDeCompra;
            solp.ProveedorAsignado_Id = ordenDeCompra.Cabecera.Usuario_Id;
        }

        private bool ValidarCondicionEspecial(Solp solp)
        {
            bool isOk = solp.TrabajoYaHecho == true || solp.ConPresupuesto || solp.Adicional == true || solp.CondEspProveedorAsignado == true;

            if ((solp.TrabajoYaHecho == true) && (solp.Urgencia == true) && (solp.THAjustePolinomica == true))
            {
                throw new WSCustomException("Una SOLP \"Trabajo ya hecho\" con \"Urgencia\" no puede ser de tipo \"Ajuste Polinómica\"");
            }

            return isOk;
        }

        private bool ValidarCondicionEspecialArchivosYObservaciones(Solp solp)
        {
            return solp.TrabajoYaHecho == true || solp.ConPresupuesto || solp.Adicional == true || solp.CondEspProveedorAsignado == true || solp.Urgencia == true;
        }

        private bool ValidarIncopatibilidadSeraUsadoEnPliegoMultipleConOtrasCondEsp(Solp solp)
        {
            return !(solp.TrabajoYaHecho == true || solp.ConPresupuesto || solp.Adicional == true || solp.CondEspProveedorAsignado == true || solp.Urgencia == true) && solp.SeraUsadoEnPliegoMultiple;
        }

        private void GrabarArchivosSapEnPliego(Solp solp, List<ArchivoSolpDto> archivos)
        {
            try
            {
                if (solp.Pliego.Archivos == null)
                {
                    solp.Pliego.Archivos = new List<Archivo>();
                }
                else
                {
                    var archivosSap = solp.Pliego.Archivos.Where(a => a.ArchivoSap == true).ToList();
                    foreach (var archivo in archivosSap)
                    {
                        solp.Pliego.Archivos.Remove(archivo);
                    }
                }

                foreach (var item in archivos)
                {
                    byte[] archivoSAP = comprasServiceSap.TraerArchivosDeSAP(item.DocId);
                    string nombreArchivo = $"Solp_{solp.Id}/{item.Nombre}.{item.Tipo}";
                    string rutaArchivoGuardado = GuardarArchivoEnSistemaDeAlmacenamiento(archivoSAP, nombreArchivo, solp.Id);

                    Archivo nuevoArchivo = new Archivo
                    {
                        FileKey = FileKeys.AdjuntoCotizacionesSolpCondEsp,
                        Ruta = rutaArchivoGuardado,
                        ArchivoSap = true
                    };

                    solp.Pliego.Archivos.Add(nuevoArchivo);
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Logger.Log.Info($"ErrorCondicionEspecial {solp.NroSolp}");
                Logger.Log.Error(e);
            }
        }

        private string GuardarArchivoEnSistemaDeAlmacenamiento(byte[] archivo, string nombreArchivo, int solpId)
        {
            string rutaBase = $"C:/ArchivosCompras/Solp_{solpId}/";
            string rutaCompleta = Path.Combine(rutaBase, nombreArchivo);

            Directory.CreateDirectory(Path.GetDirectoryName(rutaCompleta));

            File.WriteAllBytes(rutaCompleta, archivo);

            return rutaCompleta;
        }


        private void EnviarMailErrorCondicionEspecial(Solp solp, string mensaje)
        {
            try
            {
                var asunto = $"Solp condición especial - {solp.NroSolp} ";
                var enviarA = new List<string>();
                Usuario usuario = null;

                if (solp?.UsuarioCreacion?.Mail != null)
                {
                    usuario = repositorio.Obtener<Usuario>(x => x.Mail == solp.UsuarioCreacion.Mail);
                }

                bool esAmbienteQA = ConfigurationManager.AppSettings["EmailAsuntoPrefijo"] == "[QA]";

                if (usuario == null || esAmbienteQA)
                {
                    enviarA.Add(ConfigurationManager.AppSettings["EmailMantenimiento"]);
                }
                if (usuario != null && !esAmbienteQA)
                {
                    enviarA.Add(usuario.Mail);
                }

                Logger.Log.Info($"Enviando mail a {enviarA.ToJson()}");

                emailService.EnviarMail(enviarA, asunto, "", null, CuerpoEnviarMailErrorCondicionEspecial(solp, mensaje));
            }
            catch (Exception e)
            {
                Logger.Log.Info($"EnviarMailErrorCondicionEspecial {solp?.NroSolp}");
                Logger.Log.Error(e);
            }
        }

        private AlternateView CuerpoEnviarMailErrorCondicionEspecial(Solp solp, string mensaje)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();

            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"Verique la SOLP {solp.NroSolp} y por favor actualice los datos de forma manual. <br />";
            htmlBody += $"Motivo: {mensaje}  <br />";
            htmlBody += $" <br/><br/> ";

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                  "<br/><br/>Saludos Cordiales<br/>" +
                  "Molinos Agro S.A. <br/><br/> " +
                   @"<img width='15%' src='cid:" + res.ContentId + @"'/>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private void CompletarCondicionEspecial(string condicion, Solp solp)
        {
            var listaCaracteresEspeciales = condicion.Select(c => c.ToString()).ToList();
            foreach (var caracter in listaCaracteresEspeciales)
            {
                switch (caracter.ToUpper())
                {
                    case "A":
                        solp.Adicional = true;
                        break;
                    case "P":
                        solp.CondEspProveedorAsignado = true;
                        break;
                    case "S":
                        solp.TrabajoYaHecho = true;
                        solp.CertificacionAutomatica = true;
                        solp.THServicioPermanente = true;
                        break;
                    case "L":
                        solp.TrabajoYaHecho = true;
                        solp.CertificacionAutomatica = true;
                        solp.THAjustePolinomica = true;
                        break;
                    case "D":
                        solp.TrabajoYaHecho = true;
                        solp.CertificacionAutomatica = true;
                        solp.THProveedorDirecto = true;
                        break;
                    case "C":
                        solp.ConPresupuesto = true;
                        break;
                    case "U":
                        solp.Urgencia = true;
                        break;
                    case "M":
                        solp.SeraUsadoEnPliegoMultiple = true;
                        break;
                    case "V":
                        solp.CertificacionAutomatica = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ActualizarTieneModificaciones(Solp solp)
        {
            if (solp != null && solp.TieneModificaciones != true)
            {
                solp.TieneModificaciones = true;
            }
        }

        static readonly object _lockObtenerSolpesDesdeSAPJob = new object();

        public void ExecuteObtenerSolpesDesdeSAPJob(ObtenerSolpRequest obtenerSolpRequest)
        {
            lock (_lockObtenerSolpesDesdeSAPJob)
            {
                ObtenerSolpesDesdeSAPJob(obtenerSolpRequest);
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
            ConsultaEstadoSolp(nroSolp, lista);
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
            var solp = comprasServiceSap.ObtenerSolpSap(filtros);
            if (solp.Posiciones.Any())
            {
                var codigoSap = listaTablaSap.FirstOrDefault(x => x.CodigoSap == solp.Posiciones[0].EstadoSolpSap);
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

        public List<MaterialSolpDto> AutocompleteMaterialSolp(string valor, int centroId)
        {
            string[] palabras = valor.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<MaterialSolpDto> lista = repositorio.Listar<MaterialSolp>(e =>
                palabras.All(p => e.Descripcion.Contains(p)) && e.Centro_Id == centroId && e.Estado, 0, null, DirOrden.Asc)
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

            if (solp.TipoSolp == null || (solp.TipoSolp.Descripcion == "SIN_PLIEGO" && solp.Pliego.Archivos.Count == 0))
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

        public ListaPaginada<SolpDto> ListarSolpComprador(int usuario_Id,
                                                          Paginacion paginacion,
                                                          string nroSolp,
                                                          string nombrePedido,
                                                          DateTime? desde,
                                                          DateTime? hasta,
                                                          bool sap,
                                                          bool mantenimiento,
                                                          bool web,
                                                          bool repoAutomatica,
                                                          EstadoListarTratamientoSolp listarPendiente,
                                                          bool contratoMarco,
                                                          List<int> usuarios = null,
                                                          List<int> estados = null,
                                                          List<int> centros = null,
                                                          List<int> grupoDeCompras = null,
                                                          List<int> claseDocumento = null,
                                                          List<string> tipoImputacion = null,
                                                          List<int> valorTipoImputacion = null,
                                                          TipoPliego tipoPliego = TipoPliego.All)
        {
            if (listarPendiente == EstadoListarTratamientoSolp.None)
            {
                listarPendiente = EstadoListarTratamientoSolp.Todas;
            }

            var solps = new List<string>();
            List<string> solpPendientesSap = new List<string>();
            if (listarPendiente != EstadoListarTratamientoSolp.Todas)
            {
                solpPendientesSap.AddRange(comprasServiceSap.ListarNumeroSolpPendientes());
            }

            if (!string.IsNullOrEmpty(nroSolp))
            {
                nroSolp = nroSolp.Trim();

                if (!string.IsNullOrEmpty(nroSolp) && !nroSolp.StartsWith("0"))
                {
                    nroSolp = "0" + nroSolp; /* TODO: no creo que haya que ponerle siempre un 0,
                                              * sino que se debería hacer algo del tipo PAD LEFT con X caracteres.
                                              * Por ahora lo dejo como estaba.
                                              * (fseckel, 2024-09-30)
                                              */
                }

                solps.Add(nroSolp);
            }

            string[] pedido = nombrePedido.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var todasLasSolp = repositorio.ListarConsultaPaginada(new ListarSolpConsulta(paginacion, solps, solpPendientesSap, pedido, desde, hasta, sap, mantenimiento, web, repoAutomatica, listarPendiente, contratoMarco, usuarios, estados, centros, grupoDeCompras, usuario_Id, claseDocumento, tipoImputacion, valorTipoImputacion, tipoPliego));

            if (todasLasSolp?.Any() == true)
            {
                var listId = todasLasSolp.Select(y => y.Id.Value).ToList();
                var solpsDB = repositorio.Listar<Solp>(x => listId.Contains(x.Id));
                todasLasSolp.First().ItemsTotales = todasLasSolp.ItemsTotales;
                todasLasSolp.First().ItemPorPagina = 10;

                foreach (var item in todasLasSolp.Items)
                {
                    item.CentroFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.CentroCodigo).GroupBy(x => x.CentroCodigo).Select(x => x.Key)) : "";
                    item.GrupoCompraFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.GrupoComprasCodigo).GroupBy(x => x.GrupoComprasCodigo).Select(x => x.Key)) : "";

                    if (item.VerPublicar && item.PosicionCompras.Any())
                    {
                        var solpDB = solpsDB.First(i => i.Id == item.Id);

                        if (solpDB.CondEspProveedorAsignado == true)
                        {
                            item.VerPublicar = false;
                        }

                        if (solpDB.TrabajoYaHecho == true && solpDB.Urgencia == true)
                        {
                            item.VerPublicar = false;
                        }

                        if (item.PosicionCompras.Any(p => !string.IsNullOrEmpty(p.NumeroContratoSuperior)))
                        {
                            item.VerPublicar = false;
                        }

                        item.VerPublicar &= comprasServiceSap.ObtenerPosicionesPendientesAdjudicar(item.NroSolp).Any();
                    }
                }
            }
            return todasLasSolp;
        }

        public ListaPaginada<PeticionDeOfertaDto> ListarPOProveedor(Paginacion paginacion, string nroSolp, string nroPo, string nombrePedido, string username, DateTime? desde, DateTime? hasta, int? estadoLicitacion, int? estadoCotizacion)
        {
            try
            {
                string cuitUsuario = repositorio.Obtener<Usuario>(a => a.Mail == username).CUITRegistro;
                string[] palabras = nombrePedido.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (!string.IsNullOrEmpty(nroSolp) && !nroSolp.StartsWith("0"))
                {
                    nroSolp = "0" + nroSolp;
                }

                ListaPaginada<PeticionDeOfertaDto> todasLasPO = repositorio.ListarConsultaPaginada(new ListarSolpPOConsulta(paginacion, nroSolp, nroPo, palabras, cuitUsuario, estadoCotizacion, estadoLicitacion, desde, hasta));
                IEnumerable<int> listId = todasLasPO.Select(y => y.Id);

                if (todasLasPO.Any())
                {
                    List<PeticionDeOferta> peticionesDeOferta = repositorio.Listar<PeticionDeOferta>(x => listId.Contains(x.Id));
                    todasLasPO.FirstOrDefault().ItemsTotales = todasLasPO.ItemsTotales;
                    foreach (var item in todasLasPO)
                    {
                        item.NroSolp = item.NrosSolp != null ? string.Join(", ", item.NrosSolp.Distinct()) : "";
                        if (peticionesDeOferta.Find(x => x.Id == item.Id)?.Posiciones.FirstOrDefault()?.SolpPosicion.Solp.Pliego != null)
                        {
                            item.VisitasMasivas = peticionesDeOferta.Find(x => x.Id == item.Id)?.Posiciones.FirstOrDefault()?.SolpPosicion.Solp.Pliego.VisitasMasivas.Select(x => x.FechaHora.HasValue ? x.FechaHora : null);
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
                var todasLasOfertas = repositorio.ObtenerConsultaEscalar(new ComparadorOfertasConsulta(PeticionOferta_Id));
                var posicionesId = todasLasOfertas.PeticionDeOfertaPosicion.Select(x => x.SolpPosicion_Id).ToList();

                if (todasLasOfertas.PeticionDeOfertaPosicion.First().Posicion.SolpTipo == "SERVICIO")
                {
                    var solpPosiciones = todasLasOfertas.PeticionDeOfertaPosicion.Select(x => new SolpPosicionDto { NroSolp = x.Posicion.NroSolp, Indice = x.Posicion.Indice }).ToList();
                    AgregarOrdenesCompraDeSap(todasLasOfertas, solpPosiciones);
                }

                var posicionesSap =
                    comprasServiceSap.ObtenerPosiciones(todasLasOfertas.NrosSolp);

                var esAdmin = usuario.Permisos.Exists(p => p == "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS");

                var noSolicitoVerPrecios = ValidarVisualizarPrecio(usuario.Id, PeticionOferta_Id);

                CompletarCotizacionEnVerOfertas(todasLasOfertas.Usuarios, posicionesId, PeticionOferta_Id);

                CompletarValoresPOPorProveedor(todasLasOfertas, esAdmin, noSolicitoVerPrecios);

                todasLasOfertas.VerBotonVerPrecio = noSolicitoVerPrecios && esAdmin && todasLasOfertas.Usuarios.Any(a => !a.VerImportes);

                var observacionesCotizacionCondEspBuilder = new StringBuilder();
                foreach (var ObservacionesCotizacionSolp in todasLasOfertas.SolpDto.ObservacionesCotizacionLista)
                {
                    observacionesCotizacionCondEspBuilder
                        .Append("Solp ")
                        .Append(ObservacionesCotizacionSolp.NroSolp)
                        .Append(": ")
                        .Append(ObservacionesCotizacionSolp.ObservacionesCotizacionCondEsp)
                        .Append("\n"); // no usar AppendNewLine
                }
                todasLasOfertas.SolpDto.ObservacionesCotizacionCondEsp = observacionesCotizacionCondEspBuilder.ToString();

                foreach (SolpPosicionDto posicion in todasLasOfertas.PeticionDeOfertaPosicion.Select(x => x.Posicion))
                {
                    var posicionSolpSAP = posicionesSap
                        .SingleOrDefault(sap =>
                            sap.NumeroSolicitud == posicion.NroSolp &&
                            int.Parse(sap.NumeroPosicion) == posicion.Indice)
                        ?? throw new ValidationCustomException($"No se encontró la posición en SAP (SOLP {posicion.NroSolp} Posición {posicion.Indice})");

                    posicion.CantidadAdjudicada = posicionSolpSAP.Ordered; //Cantidad que ya se adjudico
                    posicion.Cantidad = posicionSolpSAP.Cantidad;
                    posicion.CantidadPendiente = posicionSolpSAP.Cantidad - posicionSolpSAP.Ordered; //Cantidad Pendiente
                    posicion.CantidadAdjudicacion = posicion.CantidadPendiente; //Cantidad A Adjudicar 
                    posicion.AdjudicacionCompleta = posicion.CantidadPendiente <= 0;
                }

                return todasLasOfertas;
            }
            catch (Exception e)
            {
                Log.Error($"Error al listar ofertas comprador con PeticionOferta_Id={PeticionOferta_Id}", e);
                throw;
            }
        }

        private void CompletarCotizacionEnVerOfertas(List<PeticionDeOfertaUsuarioDto> usuarios, List<int> posicionesId, int peticionOferta_Id)
        {
            var peticionDeOfertaSolpPosicion = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => posicionesId.Contains(x.SolpPosicion_Id) && x.PeticionDeOferta_Id == peticionOferta_Id).ToList();

            foreach (var usuario in usuarios)
            {
                if (usuario.Cotizacion != null)
                {
                    foreach (var posicion in peticionDeOfertaSolpPosicion.Where(a => a.PeticionDeOferta_Id == peticionOferta_Id))
                    {
                        var posicionExistente = usuario.Cotizacion.CotizacionPosiciones.Find(posi => posi.PosicionId == posicion.SolpPosicion_Id);

                        if (posicionExistente == null)
                        {
                            var cotizacionPosicion = new CotizacionPosicionDto
                            {
                                Id = 0,// posicion.SolpPosicion_Id * -1,
                                PosicionId = posicion.SolpPosicion_Id,
                                Cantidad = 0,
                                Completado = false,
                                Adjudicado = posicion.SolpPosicion.ProveedorAdjudicado_Id != null,
                                EstaEliminado = !posicion.SolpPosicion.Estado,
                                NoDisponible = false,
                                UnidadDeMedida_Id = 0,
                                UnidadMedida = new TablaSapDto { },
                                UnidadMedidaDescripcion = "",
                                Moneda_Id = 0,
                                Moneda = new TablaSapDto { },
                                MonedaCodigo = "",
                                MonedaDescripcion = "",
                                PeticionDeOfertaSolpPosicion_Id = posicion.Id,
                                CotizacionSubPosiciones = posicion.SolpPosicion.Subposiciones
                                    .Select(sub => new CotizacionSubPosicionDto()
                                    {
                                        SolpSubPosicion_Id = sub.Id,
                                        Completado = false,
                                    })
                                    .ToList()
                            };

                            usuario.Cotizacion.CotizacionPosiciones.Add(cotizacionPosicion);
                        }
                        else
                        {
                            var subposicionesExistentes = posicionExistente.CotizacionSubPosiciones.Select(sub => sub.SolpSubPosicion_Id).ToList();

                            foreach (var subposicion in posicion.SolpPosicion.Subposiciones)
                            {
                                if (!subposicionesExistentes.Contains(subposicion.Id))
                                {
                                    var nuevaCotizacionSubposicion = new CotizacionSubPosicionDto
                                    {
                                        SolpSubPosicion_Id = subposicion.Id,
                                        Completado = false
                                    };

                                    posicionExistente.CotizacionSubPosiciones.Add(nuevaCotizacionSubposicion);
                                }
                            }
                        }
                    }
                }
            }
        }

        private decimal CalcularTipoDeCambio(Dictionary<int, decimal> tipodecambio, TablaSap destino, CotizacionPosicionDto cotizacionPosicion)
        {
            if (!tipodecambio.TryGetValue(cotizacionPosicion.Moneda_Id, out decimal cambio) && cotizacionPosicion.Moneda_Id > 0)
            {
                var tipoCambio = tipoCambioService.ObtenerTipoCambio(cotizacionPosicion.Moneda_Id, destino.Id, DateTime.Now);
                tipodecambio.Add(cotizacionPosicion.Moneda_Id, tipoCambio.TipoCambio);
                cambio = tipoCambio.TipoCambio;
            }

            if (cotizacionPosicion.CotizacionSubPosiciones != null)
            {
                foreach (var subpos in cotizacionPosicion.CotizacionSubPosiciones)
                {
                    if (subpos.Moneda_Id != null && !tipodecambio.TryGetValue(subpos.Moneda_Id.Value, out cambio) && subpos.Moneda_Id > 0)
                    {
                        var tipoCambio = tipoCambioService.ObtenerTipoCambio(subpos.Moneda_Id.Value, destino.Id, DateTime.Now);
                        tipodecambio.Add(subpos.Moneda_Id.Value, tipoCambio.TipoCambio);
                        cambio = tipoCambio.TipoCambio;
                    }

                    subpos.TotalARPSubPosCotizacion = cambio * subpos.PrecioTotalSubPosCotizacion;
                }
                cotizacionPosicion.TotalPosicionCotizacion = cotizacionPosicion.CotizacionSubPosiciones.Sum(x => x.PrecioTotalSubPosCotizacion);
            }

            return cambio;
        }

        private void CalcularCantidadYPrecioPorUnidadCotizada(PeticionDeOfertaDto todasLasOfertas, List<UnidadesDeMedida> unidadesDeMedidaSAP, CotizacionPosicionDto cotizacionPosicion)
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
            }
        }

        public SolpCompraDto ObtenerSolpCompras(int id)
        {
            try
            {
                var registrosInfo = new List<RegistroInfoDto>();
                var solp = repositorio.ObtenerConsultaEscalar(new ObtenerSolpCompras(id));
                var hoy = DateTime.Now.Date;
                var tablaSap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                var posicionesPendientes = comprasServiceSap.ObtenerPosicionesPendientesAdjudicar(solp.NroSolp);
                var consultaRegistro = solp.PosicionCompras.Where(a => !string.IsNullOrEmpty(a.MaterialComprasCodigo))
                    .GroupBy(x => new { Centro = x.Centro.CodigoSap, Material = x.MaterialComprasCodigo, GrupoDeCompras = x.GrupoCompras.CodigoSap });


                bool condicionEncontrarPendienteSap(PosicionSolpSAP x, SolpPosicionDto pos)
                    => int.Parse(x.NumeroPosicion) == pos.Indice;

                if (solp.EsTipoMaterial())
                {
                    solp.PosicionCompras.ForEach(posLocal =>
                    {
                        // Buscar las posiciones pendientes en SAP
                        var posPendiente = posicionesPendientes
                                .FirstOrDefault(posPendienteSap =>
                                    condicionEncontrarPendienteSap(posPendienteSap, posLocal)
                                );
                        // Si están pendientes, la cantidad real es la resta
                        // Si NO están pendientes (NO se encontró en la lista de pendientes), la cantidad real es 0
                        posLocal.Cantidad = posPendiente != null ? posPendiente.Cantidad - posPendiente.Ordered : 0;
                    });
                }

                if (solp.EsTipoServicio())
                {
                    // eliminar las posiciones NO pendientes en SAP
                    solp.PosicionCompras.RemoveAll(posLocal =>
                        !posicionesPendientes.Any(posPendienteSap =>
                            condicionEncontrarPendienteSap(posPendienteSap, posLocal)
                        )
                    );
                }

                foreach (var posicionAgrupada in consultaRegistro)
                {
                    var registros = registroInfoService.ObtenerRegistroInfoConsumer(posicionAgrupada.Key.Material, posicionAgrupada.Key.Centro, posicionAgrupada.Key.GrupoDeCompras, "");
                    if (registros != null)
                    {
                        CrearProveedor(registros.ConvertAll(x => x.Vendedor));
                        foreach (var posicion in posicionAgrupada)
                        {
                            foreach (var registroInfo in registros)
                            {
                                var i = 0;
                                var proveedor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == registroInfo.Vendedor && x.TipoProveedor.Id == (int)TipoUsuarioEnum.NoGranos);

                                var usuario = proveedor?.UsuariosAsociados.FirstOrDefault(a => a.Mail == proveedor.Mail && a.CUITRegistro == proveedor.CUIT && a.TipoUsuario.Id == proveedor.TipoProveedor.Id);

                                if (proveedor != null && usuario != null)
                                {
                                    decimal pendienteAdjudicar = 0;

                                    PosicionSolpSAP solpSAPPosicion = posicionesPendientes.FirstOrDefault(x => int.Parse(x.NumeroPosicion) == posicion.Indice);
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
                                            Deshabilitado = registroInfo.FechaFormateada != null && registroInfo.FechaFormateada < hoy,
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
                        usuarioService.ObtenerYCrearProveedorCompras(codigo);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error al crear el proveedor " + codigo, e);
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

                List<Expression<Func<SolpPosicion, object>>> inc = new List<Expression<Func<SolpPosicion, object>>>()
                {
                    x => x.Solp,
                };

                List<SolpPosicion> posiciones = repositorio
                    .Listar<SolpPosicion>(x => peticionDeOferta.PosIds.Contains(x.Id), includes: inc);

                if (!TodasLasPosicionesEstanPendientes(posiciones))
                {
                    throw new ValidationCustomException("La posición está completa");
                }

                var posicionesPeticion = posiciones.ConvertAll(x => new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = x.Id });
                if (registroInfo?.Count > 0)
                {
                    foreach (var pos in posicionesPeticion)
                    {
                        pos.NumeroRegistroInfo = registroInfo.First(a => a.PosicionId == pos.SolpPosicion_Id).Id;
                    }
                }
                DateTime? fechaOferta;
                if (posiciones[0].Solp.DebeGenerarPoAutomatica)
                {
                    if (posiciones[0].Solp.TrabajoYaHecho == true)
                    {
                        fechaOferta = DateTime.Today.AddDays(-1);
                    }
                    else
                    {
                        DateTime lastSecondTomorrow = DateTime.Today.AddDays(2).AddSeconds(-1);
                        if (posiciones[0].Solp.Pliego?.FechaHoraEntrega is null
                            || posiciones[0].Solp.Pliego?.FechaHoraEntrega <= lastSecondTomorrow)
                        {
                            fechaOferta = DateTime.Today.AddDays(10);
                        }
                        else
                        {
                            fechaOferta = posiciones[0].Solp.Pliego?.FechaHoraEntrega;
                        }
                    }
                }
                else
                {
                    if (peticionDeOferta.PlazoDeEntrega < DateTime.Today)
                    {
                        throw new ValidationCustomException("El plazo de entrega no puede ser anterior al día de hoy");
                    }
                    fechaOferta = peticionDeOferta.PlazoDeEntrega;
                }

                var usuarios = repositorio.Listar<Usuario>();

                List<PeticionDeOfertaUsuario> poUsuarios = new List<PeticionDeOfertaUsuario>();
                List<PeticionDeOfertaUsuarioAdicional> poUsuariosAdicionales = new List<PeticionDeOfertaUsuarioAdicional>();

                foreach (var proveedor in usuarios.Where(x => peticionDeOferta.UsuarioIds.Contains(x.Id)).GroupBy(a => a.CUITRegistro))
                {
                    poUsuarios.Add(new PeticionDeOfertaUsuario { Usuario_Id = proveedor.First().Id });

                    foreach (var adicionalId in proveedor.Select(a => a.Id))
                    {
                        if (adicionalId != proveedor.First().Id)
                        {
                            poUsuariosAdicionales.Add(new PeticionDeOfertaUsuarioAdicional { Usuario_Id = adicionalId });
                        }
                    }
                }

                var peticion = new PeticionDeOferta()
                {
                    UsuarioCreador_Id = peticionDeOferta.UsuarioActual.Id,
                    Usuario = usuarios.Find(x => x.Id == peticionDeOferta.UsuarioActual.Id),
                    FechaCreacion = DateTime.Now,
                    Observaciones = peticionDeOferta.Observacion ?? "",
                    Posiciones = posicionesPeticion,
                    PlazoDeOferta = fechaOferta ?? posiciones.OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).First().Value,
                    Usuarios = poUsuarios,
                    UsuariosAdicionales = poUsuariosAdicionales,
                    RegistroInfo = peticionDeOferta.RegistroInfo,
                    AdjuntoPliego = peticionDeOferta.AdjuntoPliego,
                    Agrupada = false
                };

                repositorio.Agregar(peticion);
                repositorio.GuardarCambios();

                if (adjuntos?.Count > 0)
                {
                    GuardarArchivosPeticionDeOferta(peticion, adjuntos);
                }
                repositorio.GuardarCambios();

                respuestaGuardarSOLP.IdEntidad = peticion.Id;

                if (enviarMail)
                {
                    try
                    {
                        EnviarMailPeticionDeOferta(peticion, peticion.Usuarios.ToList(), peticion.UsuariosAdicionales.ToList(), false);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Error al enviar mail GrabarPeticionDeOferta en petición: {peticion.Id}", e);
                    }
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public void DesvincularSolpDePOMultipleMaterial(int solpPosicionId, List<int> idsPOsADesvincular)
        {
            var peticionesDeOfertaADesvincular = ConsultarPeticionesDeOfertaADesvincular(idsPOsADesvincular, solpPosicionId: solpPosicionId);

            var solpPosicion = repositorio.Obtener<SolpPosicion>(sp => sp.Id == solpPosicionId, new Expression<Func<SolpPosicion, object>>[] { x => x.Peticiones });

            var pospsADesvincular = idsPOsADesvincular.Select(idPo => solpPosicion.Peticiones.First(x => x.PeticionDeOferta_Id == idPo)).ToList();
            repositorio.RemoverTodos(pospsADesvincular);

            repositorio.GuardarCambios();

            NotificarPOsModificadas(peticionesDeOfertaADesvincular);
        }

        public void DesvincularSolpDePOMultipleServicio(int solpId, List<int> idsPOsADesvincular)
        {
            var peticionesDeOfertaADesvincular = ConsultarPeticionesDeOfertaADesvincular(idsPOsADesvincular, solpId: solpId);

            var pospsADesvincular = peticionesDeOfertaADesvincular.SelectMany(po => po.Posiciones).Where(pos => pos.SolpPosicion.Solp_Id == solpId).ToList();

            repositorio.RemoverTodos(pospsADesvincular);

            repositorio.GuardarCambios();

            NotificarPOsModificadas(peticionesDeOfertaADesvincular);
        }

        public List<PeticionDeOfertaDesvincularDto> ObtenerPeticionesDeOfertaParaDesvincularMaterial(int solpPosicionId)
        {
            return repositorio.ListarPOsDesvinculablesDePosicionMaterial(solpPosicionId);
        }

        public List<PeticionDeOfertaDesvincularDto> ObtenerPeticionesDeOfertaParaDesvincularServicio(int solpId)
        {
            return repositorio.ListarPOsDesvinculablesDeSolpServicio(solpId);
        }

        private bool TodasLasPosicionesEstanPendientes(List<SolpPosicion> posiciones)
        {
            //tex:
            // sea $ posicionesPendientesSap $ las posiciones marcadas en SAP como pendientes
            IEnumerable<PosicionSolpSAP> posicionesPendientesSap =
                comprasServiceSap.ObtenerPosicionesPendientesAdjudicar(posiciones.Select(x => x.Solp.NroSolp));

            //tex:
            //se define que una posición $pos$ está pendiente de la siguiente forma:
            //$$ \{ \exists posPendieteSap \in posicionesPendientesSap \|
            //pos.Solp.NroSolp = posPendieteSap.NumeroSolicitud
            //\land pos.Indice = posPendieteSap.NumeroPosicion \} $$
            bool posicionPendiente(SolpPosicion pos) =>
                                posicionesPendientesSap.Any(posPendieteSap =>
                                            posPendieteSap.NumeroSolicitud == pos.Solp.NroSolp
                                            && int.Parse(posPendieteSap.NumeroPosicion) == pos.Indice);

            //tex:
            // Returns todas las posiciones recibidas están pendientes:
            // $$ \{\forall pos \in posiciones \| posicionPendiente\} $$
            return posiciones.TrueForAll(posicionPendiente);
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

        public ObtenerLegajoResponse ObtenerLegajo(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor, string mailUsuario, bool esSolicitante)
        {
            var legajo = new List<LegajoDto>();
            var usuarioDto = usuarioService.GetUsuario(mailUsuario);
            var peticion = repositorio.Obtener<PeticionDeOferta>(peticionDeOfertaId);

            var solps = peticion.Posiciones.Select(x => x.SolpPosicion.Solp).Distinct();

            // Primero, filtra las SOLP según la condición deseada
            var solpsAgrupadas = peticion.Posiciones
                .Where(x => x.PeticionDeOferta.Agrupada)
                .Select(x => x.SolpPosicion.Solp.NroSolp)
                .ToList();

            var solpsAgrupadasStr = string.Join(", ", solpsAgrupadas.Distinct());

            bool esMultipleSolp = solps.Count() > 1;
            bool ocultarArchivosPliego =
                esProveedor
                && (esMultipleSolp && !solps.Any(s => s.Posiciones.Any(p => p.TipoPosicion.Codigo == "SERVICIO"))) /* si es servicio, mostrar aún cuando es múltiple */;
            List<int> idPliegoUsados = new List<int>();
            foreach (Solp solp in solps)
            {
                ObtenerLegajoPliegoPdf(peticionDeOfertaId, legajo, ocultarArchivosPliego, solp);

                var tieneCondicionEspecial = solp.TrabajoYaHecho == true || solp.Urgencia == true || solp.Adicional == true || solp.CondEspProveedorAsignado == true;

                // buscar archivos de la solp y considerar condiciones especiales
                if (solp.Pliego != null && !idPliegoUsados.Contains(solp.Pliego.Id))
                {
                    idPliegoUsados.Add(solp.Pliego.Id);
                    ObtenerLegajoAdjuntosPliego(peticionDeOfertaId, esProveedor, legajo, solp, tieneCondicionEspecial);
                    ObtenerLegajoTextoCondicionesEspeciales(peticionDeOfertaId, esProveedor, legajo, solp, tieneCondicionEspecial);
                }

                ObtenerLegajoChatInterno(peticionDeOfertaId, esProveedor, legajo, solp);
                ObtenerLegajoChatExterno(peticionDeOfertaId, esProveedor, legajo, peticion, solp);
            }

            ObtenerLegajoSolpsAgrupadas(peticionDeOfertaId, legajo, peticion, solpsAgrupadasStr);
            ObtenerLegajoAdjuntosPeticionOferta(peticionDeOfertaId, idPeticionDeOfertaUsuario, esProveedor, legajo, peticion);
            ObtenerLegajoPdfPeticionOfertaMateriales(peticionDeOfertaId, idPeticionDeOfertaUsuario, esProveedor, legajo, usuarioDto, peticion);

            AgregarALegajoPeticionVisualizarPrecio(legajo, peticion, esProveedor);


            List<Circular> circulares = ObtenerLegajoCirculares(peticionDeOfertaId, idPeticionDeOfertaUsuario, legajo, peticion);


            ObtenerLegajoCierrePlazoOferta(peticionDeOfertaId, legajo, peticion, esProveedor);
            ObtenerLegajoRevisionTecnicaAnticipada(peticionDeOfertaId, legajo, peticion, esProveedor);


            if (!esProveedor)
            {
                AgregarALegajoDescargaRevisionTecnica(legajo, peticion);
                AgregarALegajoHistorialDeMovimientos(legajo, peticion, peticionDeOfertaId);

                if (!esSolicitante)
                {
                    AgregarALegajoDocumentosEnviadosPorProveedores(legajo, peticion, peticionDeOfertaId, usuarioDto);
                    AgregarALegajoDescargaHistorialDeCotizaciones(legajo, peticion, usuarioDto);
                }
            }

            var response = new ObtenerLegajoResponse
            {
                LegajoFilas = legajo.OrderByDescending(x => x.Fecha).ToList(),
                PuedeVerPrecios = PuedenVerseLosImportes(peticion, usuarioDto, circulares)
            };

            return response;
        }

        private static void ObtenerLegajoRevisionTecnicaAnticipada(int peticionDeOfertaId, List<LegajoDto> legajo, PeticionDeOferta peticion, bool esProveedor)
        {
            if (!esProveedor)
            {
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
                            SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                            Fecha = peticion.RevisionTecnica.Fecha,
                            FechaFormateado = peticion.RevisionTecnica.Fecha.ToString("dd/MM/yyyy"),
                            Usuario = new UsuarioDto { CUIT = peticion.RevisionTecnica.Usuario.CUITRegistro, Mail = peticion.RevisionTecnica.Usuario.Mail, Id = peticion.RevisionTecnica.Usuario.Id },
                            Tipo = TipoLegajo.RevisionTecnica
                        });
                    }

                    if (peticion.RevisionTecnica.Finalizada)
                    {
                        legajo.Add(new LegajoDto
                        {
                            ArchivoId = null,
                            Observacion = "Finalización revisión técnica",
                            PeticionDeOfertaId = peticionDeOfertaId,
                            SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                            Fecha = peticion.RevisionTecnica.Fecha,
                            FechaFormateado = peticion.RevisionTecnica.Fecha.ToString("dd/MM/yyyy"),
                            Usuario = new UsuarioDto { CUIT = peticion.RevisionTecnica.Usuario.CUITRegistro, Mail = peticion.RevisionTecnica.Usuario.Mail, Id = peticion.RevisionTecnica.Usuario.Id },
                            Tipo = TipoLegajo.RevisionTecnica
                        });
                    }
                }
            }
        }

        private void ObtenerLegajoCierrePlazoOferta(int peticionDeOfertaId, List<LegajoDto> legajo, PeticionDeOferta peticion, bool esProveedor)
        {
            if (!esProveedor)
            {
                //Cierres plazo de oferta
                var cierres = repositorio.Listar<PeticionDeOfertaCierre>(a => a.PeticionDeOferta_Id == peticionDeOfertaId);
                foreach (var cierre in cierres)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = null,
                        Observacion = cierre.Observacion,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                        Fecha = cierre.Fecha,
                        FechaFormateado = cierre.Fecha.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = cierre.Usuario.CUITRegistro, Mail = cierre.Usuario.Mail, Id = cierre.Usuario_Id },
                        Tipo = TipoLegajo.CierreOferta
                    });
                }
            }
        }

        private List<Circular> ObtenerLegajoCirculares(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, List<LegajoDto> legajo, PeticionDeOferta peticion)
        {
            //circular
            var peticionDeOfertaUsuarios_Id = peticion.Usuarios.Where(u => idPeticionDeOfertaUsuario == null || u.Id == idPeticionDeOfertaUsuario).Select(u => u.Id).ToList();
            var circulares = repositorio.Listar<Circular>(x => x.PeticionDeOfertaUsuarios.Any(a => peticionDeOfertaUsuarios_Id.Contains(a.PeticionDeOfertaUsuario_Id)));

            foreach (var circular in circulares)
            {
                bool noLeido = false;
                if (idPeticionDeOfertaUsuario.HasValue)
                {
                    noLeido = circular.PeticionDeOfertaUsuarios.Any(a => a.PeticionDeOfertaUsuario_Id == idPeticionDeOfertaUsuario && a.Leida != true);
                }

                //buscar archivos de la circular
                foreach (var item in circular.Archivos)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = item.Id,
                        Observacion = item.ObtenerNombre(item.Ruta),
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
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
                    SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
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
                            SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
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
                            SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                            Fecha = circular.FechaCreacion,
                            FechaFormateado = circular.FechaCreacion.ToString("dd/MM/yyyy"),
                            Leido = !noLeido,
                            Usuario = new UsuarioDto { CUIT = circular.Usuario.CUITRegistro, Mail = circular.Usuario.Mail, Id = circular.UsuarioCreador_Id },
                            Tipo = TipoLegajo.Circular
                        });
                    }
                }

                if (noLeido)
                {
                    foreach (var circularNoLeida in circular.PeticionDeOfertaUsuarios.Where(a => a.PeticionDeOfertaUsuario_Id == idPeticionDeOfertaUsuario && a.Leida != true))
                    {
                        circularNoLeida.Leida = true;
                        circularNoLeida.FechaLeida = DateTime.Now;
                    }
                    repositorio.GuardarCambios();
                }
            }

            return circulares;
        }

        private static void ObtenerLegajoPdfPeticionOfertaMateriales(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor, List<LegajoDto> legajo, UsuarioDto usuarioDto, PeticionDeOferta peticion)
        {
            // pdf petición de oferta materiales
            if (peticion.Posiciones?.FirstOrDefault()?.SolpPosicion?.TipoPosicion?.Codigo == "MATERIALES")
            {
                bool peticionesUsuario(PeticionDeOfertaUsuario u)
                {
                    if (!esProveedor) { return true; }
                    if (idPeticionDeOfertaUsuario == null && usuarioDto == null) { return true; }
                    if (idPeticionDeOfertaUsuario == u.Id) { return true; }
                    if (usuarioDto?.CUIT == u.Usuario.CUITRegistro) { return true; }
                    return false;
                }

                foreach (var peticionUsuario in peticion.Usuarios.Where(peticionesUsuario))
                {
                    var pdfPOUsuario = $"PO - {peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf";
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = peticionUsuario.Id * -1,//lo ponemos en negtivo para difernciarlo de los ids de archivos
                        Observacion = pdfPOUsuario,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                        Fecha = peticion.FechaCreacion,
                        FechaFormateado = peticion.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = peticion.Usuario.CUITRegistro, Mail = peticion.Usuario.Mail, Id = peticion.UsuarioCreador_Id },
                        Tipo = TipoLegajo.PeticionDeOferta
                    });
                }
            }
        }

        private static void ObtenerLegajoAdjuntosPeticionOferta(int peticionDeOfertaId, int? idPeticionDeOfertaUsuario, bool esProveedor, List<LegajoDto> legajo, PeticionDeOferta peticion)
        {
            //buscar archivos de la peticion ( menos lo de legajo cuando es un usuario proveedor)
            foreach (var item in peticion.Archivos.Where(a => !esProveedor || (esProveedor && a.Archivo.FileKey != FileKeys.PeticionDeOfertaLegajo)))
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = item.Archivo.Id,
                    Observacion = item.Archivo.ObtenerNombre(item.Archivo.Ruta),
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                    Fecha = item.Fecha,
                    FechaFormateado = item.Fecha.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Usuario.CUITRegistro, Mail = peticion.Usuario.Mail, Id = peticion.UsuarioCreador_Id },
                    Tipo = idPeticionDeOfertaUsuario == null ? TipoLegajo.Legajo : TipoLegajo.PeticionDeOferta
                });
            }
        }

        private static void ObtenerLegajoSolpsAgrupadas(int peticionDeOfertaId, List<LegajoDto> legajo, PeticionDeOferta peticion, string solpsAgrupadasStr)
        {
            // Agrupar po th
            if (peticion.Agrupada)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = "Solps agrupadas: " + solpsAgrupadasStr,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = 0,
                    Fecha = peticion.FechaCreacion,
                    FechaFormateado = peticion.FechaCreacion.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = peticion.Usuario.CUITRegistro, Mail = peticion.Usuario.Mail, Id = peticion.UsuarioCreador_Id },
                    Tipo = TipoLegajo.PeticionDeOfertaAgrupada
                });
            }
        }

        private static void ObtenerLegajoChatExterno(int peticionDeOfertaId, bool esProveedor, List<LegajoDto> legajo, PeticionDeOferta peticion, Solp solp)
        {
            if (peticion.Usuarios != null && !esProveedor)
            {
                foreach (var usuario in peticion.Usuarios.Where(x => x.ChatExterno.Count > 0))
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = 0,
                        Observacion = "Chat externo - Razon social: " + usuario.Usuario.ObtenerRazonSocial() + " - CUIT: " + usuario.PeticionDeOferta.Usuario.CUITRegistro,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = solp.Id,
                        Fecha = usuario.ChatExterno.First().FechaEnvio,
                        FechaFormateado = usuario.ChatExterno.First().FechaEnvio.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = usuario.ChatExterno.First().Usuario.CUITRegistro, Mail = usuario.ChatExterno.First().Usuario.Mail, Id = usuario.ChatExterno.First().Usuario_Id },
                        Tipo = TipoLegajo.ChatExterno
                    });
                }
            }
        }

        private static void ObtenerLegajoChatInterno(int peticionDeOfertaId, bool esProveedor, List<LegajoDto> legajo, Solp solp)
        {
            //Chat interno
            if (solp.ChatInternoCompras?.Count > 0 && !esProveedor)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = 0,
                    Observacion = "Chat interno",
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = solp.Id,
                    Fecha = solp.ChatInternoCompras.First().FechaEnvio,
                    FechaFormateado = solp.ChatInternoCompras.First().FechaEnvio.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = solp.ChatInternoCompras.First().Usuario.CUITRegistro, Mail = solp.ChatInternoCompras.First().Usuario.Mail, Id = solp.ChatInternoCompras.First().Usuario_Id },
                    Tipo = TipoLegajo.ChatInterno
                });
            }
        }

        private static void ObtenerLegajoTextoCondicionesEspeciales(int peticionDeOfertaId, bool esProveedor, List<LegajoDto> legajo, Solp solp, bool tieneCondicionEspecial)
        {
            //mostrar observación de condiciones especielas ingresada en el paso 4 
            if (solp.Pliego != null && !esProveedor && tieneCondicionEspecial)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = "Justificación de condición especial: " + solp.Pliego.ObservacionesCotizacionCondEsp,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = solp.Id,
                    Fecha = solp.FechaCreacion,
                    FechaFormateado = solp.FechaCreacion.ToString("dd/MM/yyyy"),
                    Usuario = new UsuarioDto { CUIT = solp.UsuarioCreacion?.CUITRegistro ?? "", Mail = solp.UsuarioCreacion?.Mail ?? "", Id = solp.UsuarioCreacion_Id ?? 0 },
                    Tipo = TipoLegajo.Solp
                });
            }
        }

        private static void ObtenerLegajoAdjuntosPliego(int peticionDeOfertaId, bool esProveedor, List<LegajoDto> legajo, Solp solp, bool tieneCondicionEspecial)
        {
            if (solp.Pliego?.Archivos?.Any(x => x.FileKey == FileKeys.AdjuntoSolp
                                                                            || x.FileKey == FileKeys.AdjuntoCotizacionesSolp
                                                                            || (x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp
                                                                                && (!esProveedor || !tieneCondicionEspecial))) == true)
            {
                foreach (var archivoSubido in solp.Pliego.Archivos)
                {
                    if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp
                        || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp
                        || (archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp
                        && (!esProveedor || !tieneCondicionEspecial))))
                    {
                        string fileName = Path.GetFileName(archivoSubido.Ruta);
                        legajo.Add(new LegajoDto
                        {
                            ArchivoId = archivoSubido.Id,
                            Observacion = fileName,
                            PeticionDeOfertaId = peticionDeOfertaId,
                            SolpId = solp.Id,
                            Fecha = solp.FechaCreacion,
                            FechaFormateado = solp.FechaCreacion.ToString("dd/MM/yyyy"),
                            Usuario = new UsuarioDto { CUIT = solp.UsuarioCreacion?.CUITRegistro ?? "", Mail = solp.UsuarioCreacion?.Mail ?? "", Id = solp.UsuarioCreacion_Id ?? 0 },
                            Tipo = TipoLegajo.SolpArchivos
                        });
                    }
                }
            }
        }

        private static void ObtenerLegajoPliegoPdf(int peticionDeOfertaId, List<LegajoDto> legajo, bool ocultarArchivosPliego, Solp solp)
        {
            if (!ocultarArchivosPliego)
            {
                bool tienePliego = (solp.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento ||
                    solp.TipoSolpSap == (int?)TipoSolpSap.Sap ||
                    solp.TipoSolpSap == (int?)TipoSolpSap.ReposicionAutomatica) && solp.EstadoDocumento.Codigo == "CREADO";

                if (tienePliego || solp.TipoSolp?.Codigo == "CON_PLIEGO")
                {
                    string middleFileName = solp.NroSolp ?? solp.Pliego?.NombreObra ?? "xxxx";
                    string pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.pdf";

                    //invento registro con id de archivo 0 para bajar el pliego
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = 0,
                        Observacion = pdfFilename,
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = solp.Id,
                        Fecha = solp.FechaCreacion,
                        FechaFormateado = solp.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = solp.UsuarioCreacion?.CUITRegistro ?? "", Mail = solp.UsuarioCreacion?.Mail ?? "", Id = solp.UsuarioCreacion_Id ?? 0 },
                        Tipo = TipoLegajo.Pliego
                    });
                }
            }
        }

        public Resultado GuardarAdjuntosPeticionDeOferta(int idPeticion, HttpFileCollectionBase files, UsuarioDto usuarioDto)
        {
            var ruta = ObtenerRutaArchivos(idPeticion, FileKeys.PeticionDeOferta);
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

        public string DescargarLegajo(int idPeticion, string pathBase, int? peticiondeOfertaUsuarioId, bool esProveedor, int? adjudicacionId, string mailUsuario, bool esSolicitante)
        {
            string zipFilename;
            string filePath;

            if (adjudicacionId > 0)
            {
                var nroOC = repositorio.Obtener<Adjudicacion, string>(x => x.Id == adjudicacionId, x => x.NumeroOrdenDeCompra);
                var idsPeticiones = repositorio.Listar<Adjudicacion, int>(x => x.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id, x => x.NumeroOrdenDeCompra == nroOC);
                zipFilename = $"OC-{nroOC}.zip";
                filePath = $"{pathBase}/{zipFilename}";

                using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        foreach (var idPO in idsPeticiones)
                        {
                            var peticion = repositorio.Obtener<PeticionDeOferta>(idPO);
                            var solps = peticion.Posiciones.Select(posi => posi.SolpPosicion.Solp).Distinct();
                            DescargarLegajoPO(pathBase, peticiondeOfertaUsuarioId, esProveedor, peticion, solps, archivo, mailUsuario, esSolicitante);
                        }
                    }
                }
            }
            else
            {
                var peticion = repositorio.Obtener<PeticionDeOferta>(idPeticion);
                var solps = peticion.Posiciones.Select(posi => posi.SolpPosicion.Solp).Distinct();
                zipFilename = $"PO-{peticion.Id}-{peticion.FechaCreacion:yyyyMMdd}.zip";
                filePath = $"{pathBase}/{zipFilename}";
                using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        DescargarLegajoPO(pathBase, peticiondeOfertaUsuarioId, esProveedor, peticion, solps, archivo, mailUsuario, esSolicitante);
                    }
                }
            }

            return filePath;
        }

        private void DescargarLegajoPO(string pathBase, int? peticiondeOfertaUsuarioId, bool esProveedor, PeticionDeOferta peticion, IEnumerable<Solp> solps, ZipArchive archivo, string mailUsuario, bool esSolicitante)
        {
            var usuarioDto = usuarioService.GetUsuario(mailUsuario);
            int idPeticion = peticion.Id;
            foreach (var solp in solps)
            {
                var middleFileName = solp.NroSolp == null ? (solp.Pliego.NombreObra == null ? "xxxx" : solp.Pliego.NombreObra) : solp.NroSolp;
                var pliegoFilename = $"PO-{idPeticion}-Solp-{middleFileName}-pliego-{DateTime.Now.ToString("yyyyMMdd")}.pdf";
                var pdfFilePath = $"{pathBase}/{pliegoFilename}";
                File.WriteAllBytes(pdfFilePath, GenerarSolpPdf(solp.Id));

                // Agregar archivos de pliego al zip
                archivo.CreateEntryFromFile(pdfFilePath, pliegoFilename);

                // Agregar archivos adjuntos del solp al zip
                if (solp.Pliego.Archivos != null)
                {
                    foreach (var archivoSubido in solp.Pliego.Archivos)
                    {
                        if (File.Exists(archivoSubido.Ruta) && (archivoSubido.FileKey == FileKeys.AdjuntoSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolp || archivoSubido.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp))
                        {
                            string fileName = Path.GetFileName(archivoSubido.Ruta);
                            archivo.CreateEntryFromFile(archivoSubido.Ruta, $"PO-{idPeticion}-" + fileName);
                        }
                    }
                }

                // Agregar archivos PDF de PeticionDeOfertaMateriales al zip
                if (solp.Posiciones.Any(a => a.TipoPosicion_Id != null && a.TipoPosicion.Codigo == "MATERIALES"))
                {
                    foreach (var peticionUsuario in peticion.Usuarios)
                    {
                        string codigoProveedor = peticionUsuario.Usuario.ObtenerProveedor().CodigoProveedor;
                        var pdf = GenerarPDFPeticionDeOferta(peticion, codigoProveedor);
                        var pdfFilePathUsuario = $"{pathBase}/PO-{peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf";
                        File.WriteAllBytes(pdfFilePathUsuario, pdf);
                        archivo.CreateEntryFromFile(pdfFilePathUsuario, $"PO-{idPeticion}-{peticionUsuario.Usuario.ObtenerProveedor().CUIT}.pdf");
                    }
                }

                // Agregar archivos de circulares al zip
                var peticionDeOfertaUsuarios_Id = peticion.Usuarios.Where(u => peticiondeOfertaUsuarioId == null || u.Id == peticiondeOfertaUsuarioId).Select(u => u.Id).ToList();
                var circulares = repositorio.Listar<Circular>(x => x.PeticionDeOfertaUsuarios.Any(a => peticionDeOfertaUsuarios_Id.Contains(a.PeticionDeOfertaUsuario_Id)));
                foreach (var circular in circulares)
                {
                    foreach (var item in circular.Archivos)
                    {
                        if ((peticionDeOfertaUsuarios_Id != null && item.FileKey != FileKeys.PeticionDeOfertaLegajo) || peticionDeOfertaUsuarios_Id == null)
                        {
                            string fileName = Path.GetFileName(item.Ruta);
                            archivo.CreateEntryFromFile(item.Ruta, $"PO-{idPeticion}-" + fileName);
                        }
                    }
                }

                // Agregar archivos de chat interno al zip
                if (solp.ChatInternoCompras != null && solp.ChatInternoCompras.Count > 0)
                {
                    var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                    Directory.CreateDirectory(path);
                    string rutaTxt = ExportarChatInternoAtexto(solp.Id, path, null);
                    string fileName = Path.GetFileName(rutaTxt);
                    archivo.CreateEntryFromFile(rutaTxt, $"PO-{idPeticion}-" + fileName);
                    Directory.Delete(path, true);
                }

                // Agregar archivos de chat externo al zip
                if (peticion.Usuarios != null)
                {
                    foreach (var usuario in peticion.Usuarios.Where(x => x.ChatExterno.Count > 0))
                    {
                        var path = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";
                        Directory.CreateDirectory(path);
                        string rutaTxt = ExportarChatInternoAtexto(solp.Id, path, usuario.Id);
                        string fileName = Path.GetFileName(rutaTxt);
                        archivo.CreateEntryFromFile(rutaTxt, $"PO-{idPeticion}-" + fileName);
                        Directory.Delete(path, true);
                    }
                }
            }

            // Agregar archivos de la petición de oferta al zip
            if (peticion.Archivos != null)
            {
                foreach (var archivoSubido in peticion.Archivos.Where(a => peticiondeOfertaUsuarioId == null || (peticiondeOfertaUsuarioId != null && a.Archivo.FileKey != FileKeys.PeticionDeOfertaLegajo)))
                {
                    if (File.Exists(archivoSubido.Archivo.Ruta))
                    {
                        string fileName = Path.GetFileName(archivoSubido.Archivo.Ruta);
                        archivo.CreateEntryFromFile(archivoSubido.Archivo.Ruta, $"PO-{idPeticion}-" + fileName);
                    }
                }
            }

            if (!esProveedor)
            {
                // Agregar revisión ténica al zip
                if (peticion?.RevisionTecnica != null)
                {
                    var revisionBytes = comprasArchivosService.GenerarExcelRevisionTecnica(peticion);
                    var rutaRevisionTecnica = $"{pathBase}/RevTec{peticion.Id}.xlsx";
                    File.WriteAllBytes(rutaRevisionTecnica, revisionBytes);
                    archivo.CreateEntryFromFile(rutaRevisionTecnica, $"PO-{idPeticion}-" + $"RevTec{peticion.Id}.xlsx");
                }

                // Agregar historial de movimientos al zip
                var excelBytes = GenerarExcelHistorialMovimientos(idPeticion);
                var zipEntry = archivo.CreateEntry($"PO-{idPeticion}-" + "Historial de Movimientos.xlsx", CompressionLevel.Fastest);
                using (var entryStream = zipEntry.Open())
                {
                    entryStream.Write(excelBytes, 0, excelBytes.Length);
                }

                if (!esSolicitante)
                {
                    // Agregar archivos de cotizaciones al zip
                    if (peticion.Usuarios != null)
                    {
                        foreach (var usuario in peticion.Usuarios.Where(x => x.Cotizaciones.Count > 0))
                        {
                            var cotizacionUsuario = usuario.Cotizaciones.First();

                            if (cotizacionUsuario.Archivos.Count > 0 && PuedenVerseLosImportesDeCotizacion(cotizacionUsuario, usuarioDto))
                            {
                                foreach (var item in cotizacionUsuario.Archivos)
                                {
                                    if ((item.FileKey == FileKeys.AdjuntoCotizacionRevisionEconomica || item.FileKey == FileKeys.AdjuntoCotizacionRevisionTecnica))
                                    {
                                        string fileName = Path.GetFileName(item.Ruta);
                                        archivo.CreateEntryFromFile(item.Ruta, $"PO-{idPeticion}-" + fileName);
                                    }
                                }
                            }
                        }
                    }

                    // Agregar historiales de cotización al zip
                    foreach (var cotizacion in GetCotizacionesDescargables(peticion, usuarioDto))
                    {
                        var historialBytes = GenerarHistorialCotizaciones(cotizacion.Id);
                        var rutaHistorial = $"{pathBase}/HC-{cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor().CUIT}.xlsx";
                        File.WriteAllBytes(rutaHistorial, historialBytes);
                        archivo.CreateEntryFromFile(rutaHistorial, $"PO-{idPeticion}-" + $"HC-{cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor().CUIT}.xlsx");
                    }
                }
            }
        }

        public byte[] GenerarHistorialCotizaciones(int cotizacionId)
        {
            var historialCotizaciones = ObtenerHistorial(cotizacionId);

            return comprasArchivosService.GenerarExcelHistorialCotizaciones(historialCotizaciones);
        }

        public byte[] GenerarArchivoRevisionTecnica(int peticionDeOfertaId)
        {
            var peticion = repositorio.Obtener<PeticionDeOferta>(peticionDeOfertaId);

            return comprasArchivosService.GenerarExcelRevisionTecnica(peticion);
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
            string cssTemplatePath = httpContextService.GetDirectory("Templates/cssTemplate.css");
            string css = File.ReadAllText(cssTemplatePath);
            string stylesHtml = $"<style>{css}</style>";

            var datosProveedor = new VendedorDetalleWSMOAResponse() { cabeceras = null };
            try
            {
                datosProveedor = vendedorService.GetDatosFiscales(codigoProveedor, codigoProveedor);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }

            StringBuilder posiciones = new StringBuilder();

            var listaPosiciones = peticion.Posiciones.Where(x => x.SolpPosicion.Estado && x.SolpPosicion.EsConcluido == true);
            var posicionesValoresSAP = comprasServiceSap.ObtenerPosicionesPendientesAdjudicar(peticion.Posiciones.Select(x => x.SolpPosicion.Solp.NroSolp));
            foreach (var item in listaPosiciones.Select(a => a.SolpPosicion))
            {
                var valorEnSAP = posicionesValoresSAP.FirstOrDefault(x => int.Parse(x.NumeroPosicion) == item.Indice && x.NumeroSolicitud == item.Solp.NroSolp);

                posiciones.Append("<tr class='border-top'>");
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", item.Indice);
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", item.MaterialSolp != null ? item.MaterialSolp.Codigo : "");
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", item.MaterialSolp != null ? item.MaterialSolp.Descripcion : item.Tarea);
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", valorEnSAP != null ? valorEnSAP.Cantidad - valorEnSAP.Ordered : 0);
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", item.Unidad.Descripcion);
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", peticion.PlazoDeOferta.ToString("dd.MM.yyyy"));
                posiciones.AppendFormat("<td style='font-size: 8px;'>{0}</td>", listaPosiciones.Select(x => x.SolpPosicion)
                    .OrderByDescending(x => x.FechaEntregaServicio)
                    .Select(x => x.FechaEntregaServicio)
                    .First().Value.ToString("dd.MM.yyyy"));
                posiciones.Append("</tr>");

                posiciones.AppendFormat("<tr><td colspan='7' style='font-size: 8px; text-align: justify'>{0}</td></tr>",
                    item.MaterialSolp != null ? item.MaterialSolp.TextoAmpliado : "");

                posiciones.AppendFormat("<tr class='border-bottom'><td colspan='7' style='font-size: 8px; text-align: justify'>Texto de Suministro: {0}</td></tr>",
                    item.TextoSuministro);
            }

            var posicion = listaPosiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.Id).FirstOrDefault();
            var localidad = repositorio.Obtener<Localidad>(x => x.ProvinciaId == posicion.ProvinciaId);
            var centro = repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == posicion.Centro.CodigoSap);
            var lugarEntrega = $"{posicion.NombreEntrega}, {posicion.CalleEntrega} - ({posicion.CpEntrega}) {localidad?.Nombre ?? ""} - {posicion.Provincia?.Nombre ?? ""}";

            xHtml = string.Format(xHtml, stylesHtml,
                peticion.Id,
                datosProveedor.cabeceras?.FirstOrDefault()?.cuit.Substring(2, 8),
                datosProveedor.cabeceras?.FirstOrDefault()?.descripcion,
                datosProveedor.cabeceras?.FirstOrDefault()?.calleFiscal,
                $"({datosProveedor.cabeceras?.FirstOrDefault()?.cpFiscal}) {datosProveedor.cabeceras?.FirstOrDefault()?.locaFiscal}",
                datosProveedor.cabeceras?.FirstOrDefault()?.provFiscal,
                "Argentina",
                peticion.PlazoDeOferta.ToString("dd.MM.yyyy"),
                listaPosiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value.ToString("dd.MM.yyyy"),
                lugarEntrega,
                peticion.FechaCreacion.ToString("dd.MM.yyyy"),
                "San Lorenzo",
                centro.CodigoSap,
                peticion.Usuario.UsuarioSap,
                posiciones.ToString()
                );

            return xHtml;

        }

        private void EnviarMailPeticionDeOferta(PeticionDeOferta peticion, List<PeticionDeOfertaUsuario> usuarios, List<PeticionDeOfertaUsuarioAdicional> usuariosAdicionales, bool esEdicionPO)
        {
            var archivosJson = repositorio
                .Obtener<Configuracion>(con => con.Code == "ListaArchivosMailPeticionDeOferta")
                .Value;
            var listaArchivosMailPO = JsonConvert.DeserializeObject<List<FileDto>>(archivosJson);

            var archivosAAdjuntar = ObtenerArchivosPeticionDeOferta(peticion);

            var solicitanteYComprador = new List<string> { peticion.Usuario.Mail };
            var solps = peticion.Posiciones.Select(x => x.SolpPosicion.Solp);

            var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList();
            mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
            var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList();

            solicitanteYComprador.AddRange(mailPliego);
            solicitanteYComprador.AddRange(mailCreador);

            var usuariosPO = new List<UsuarioDto>();
            foreach (var item in usuarios)
            {
                usuariosPO.Add(new UsuarioDto
                {
                    Mail = item.Usuario.Mail,
                    RazonSocial = item.Usuario.ObtenerRazonSocial(),
                    CUIT = item.Usuario.CUITRegistro,
                    CodigoProveedor = item.Usuario.ObtenerCodigoProveedor()
                });
            }
            foreach (var item in usuariosAdicionales)
            {
                usuariosPO.Add(new UsuarioDto
                {
                    Mail = item.Usuario.Mail,
                    RazonSocial = item.Usuario.ObtenerRazonSocial(),
                    CUIT = item.Usuario.CUITRegistro,
                    CodigoProveedor = item.Usuario.ObtenerCodigoProveedor()
                });
            }

            var proveedores = new List<string>();
            foreach (var prov in usuariosPO.GroupBy(a => a.CUIT))
            {
                proveedores.AddRange(prov.Select(a => a.RazonSocial + " - " + a.Mail).ToList());

                var enviarA = prov.Select(a => a.Mail).ToList();
                if (peticion.Posiciones.Select(x => x.SolpPosicion).Where(x => x.TipoPosicion_Id != null).FirstOrDefault().TipoPosicion.Codigo == "MATERIALES")
                {
                    var pdf = GenerarPDFPeticionDeOferta(peticion, prov.First().CodigoProveedor);
                    if (archivosAAdjuntar.ContainsKey("Peticion de Oferta.pdf"))
                        archivosAAdjuntar.Remove("Peticion de Oferta.pdf");
                    archivosAAdjuntar.Add("Peticion de Oferta.pdf", pdf);
                }

                var mailReqProv = new MailPeticionDeOfertaRequest
                {
                    ArchivosAdjuntos = archivosAAdjuntar,
                    Destinatarios = enviarA,
                    EsEdicionPO = esEdicionPO,
                    EsProveedor = true,
                    ListaArchivosParaMailPO = listaArchivosMailPO,
                    PeticionDeOferta = peticion,
                    Proveedores = null
                };
                emailComprasService.EnviarMailPeticionDeOferta(mailReqProv);
            }

            var mailReq = new MailPeticionDeOfertaRequest
            {
                ArchivosAdjuntos = new Dictionary<string, byte[]>(),
                Destinatarios = solicitanteYComprador,
                EsEdicionPO = esEdicionPO,
                EsProveedor = false,
                ListaArchivosParaMailPO = listaArchivosMailPO,
                PeticionDeOferta = peticion,
                Proveedores = proveedores
            };
            emailComprasService.EnviarMailPeticionDeOferta(mailReq);
        }

        private static Dictionary<string, byte[]> ObtenerArchivosPeticionDeOferta(PeticionDeOferta peticion)
        {
            var archs = new Dictionary<string, byte[]>();
            foreach (var p in peticion.Archivos.Select(a => a.Archivo))
            {
                byte[] b = File.ReadAllBytes(p.Ruta);
                archs.Add(p.ObtenerNombre(), b);
            }
            return archs;
        }

        public Pdf GenerarPeticionDeOfertaUsuarioPdf(int idPeticionDeOfertaUsuario)
        {
            var po = repositorio.Obtener<PeticionDeOfertaUsuario>(idPeticionDeOfertaUsuario);
            var pdf = GenerarPDFPeticionDeOferta(po.PeticionDeOferta, po.Usuario.ObtenerProveedor().CodigoProveedor);
            return new Pdf { data = pdf, name = "PO" + po.Usuario.ObtenerProveedor().CUIT + ".pdf" };
        }

        private void EnviarMailOrdenCompra(Adjudicacion adjudicacion, string mensaje = "")
        {
            try
            {
                Log.Info($"EnviarMailOrdenCompra Adjudicacion_Id: {adjudicacion.Id}");
                Log.Info($"Nueva OC liberada con número {adjudicacion.NumeroOrdenDeCompra} y fecha {adjudicacion.FechaLiberacionSap}");
                Log.Info($"Copia mail comprador: {adjudicacion.Usuario.Mail}");
                Log.Info($"Mail al proveedor adjudicado: {adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail}");
                var copia = new List<string> { adjudicacion.Usuario.Mail };

                var solps = adjudicacion.Posiciones.Select(x => x.Posicion.Solp);

                var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList();
                mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
                var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList();

                copia.AddRange(mailPliego);
                copia.AddRange(mailCreador);

                Log.Info($"Copia mail solicitante paso 1: {mailPliego.ToJson()}");
                Log.Info($"Copia mail solicitante: {mailCreador.ToJson()}");


                var asunto = $"Nueva OC creada - {adjudicacion.NumeroOrdenDeCompra} - {adjudicacion.Usuario.ObtenerRazonSocial()}";

                var enviarA = new List<string> { adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail };
                var adicionales = adjudicacion.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.UsuariosAdicionales;
                enviarA.AddRange(adicionales.Where(a => a.Usuario.CUITRegistro == adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.CUITRegistro).Select(a => a.Usuario.Mail).ToList());

                var pdf = comprasServiceSap.ObtenerPDFOrdenCompra(adjudicacion.NumeroOrdenDeCompra);

                emailService.EnviarMail(enviarA, asunto, "", copia.Distinct().ToList(), CuerpoMailOrdenCompra(adjudicacion.NumeroOrdenDeCompra, mensaje), pdf, $"Orden de Compra {adjudicacion.NumeroOrdenDeCompra}.pdf");
            }
            catch (Exception e)
            {
                Log.Info($"Error en EnviarMailOrdenCompra. Adjudicacion_Id {adjudicacion.Id} - NumeroOrdenDeCompra: {adjudicacion.NumeroOrdenDeCompra}");
                Log.Error(e);
            }
        }

        private AlternateView CuerpoMailOrdenCompra(string nroOc, string mensaje)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la nueva OC {nroOc} generada con Molinos Agro S.A. <br />";
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
            PeticionDeOfertaDto peticion;
            var usuarios = new List<PeticionDeOfertaUsuarioDto>();
            var peticionEntidad = repositorio.Obtener<PeticionDeOferta>(peticionId);
            var peticionDeOfertaUsuarios_Id = peticionEntidad.Usuarios.Select(u => u.Id).ToList();
            var cotizaciones = repositorio.Listar<Cotizacion>(x => peticionDeOfertaUsuarios_Id.Contains(x.PeticionDeOfertaUsuario_Id));
            var posicion = peticionEntidad.Posiciones.FirstOrDefault().SolpPosicion;
            var tipoPosicion = posicion.TipoPosicion.Codigo;
            var tieneVisitaMasiva = posicion.Solp.Pliego.TieneVisitaObraMasiva;
            var esServicio = tipoPosicion == "SERVICIO";

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

                var existeRevisionTecnicaFinalizada = u.PeticionDeOferta.RevisionTecnica != null && u.PeticionDeOferta.RevisionTecnica.Finalizada;

                var usuario = new PeticionDeOfertaUsuarioDto()
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
                    Deshabilitado = esServicio && u.RealizoVisita == true,
                };
                usuarios.Add(usuario);
            }

            peticion = ObtenerPeticionDeOfertaDto(peticionId);

            peticion.Usuarios = usuarios;
            peticion.UsuariosAdicionales = peticionEntidad.UsuariosAdicionales.Select(a => new PeticionDeOfertaUsuarioAdicionalDto(a)).ToList();

            peticion.Id = peticionEntidad.Id;
            peticion.PlazoDeOfertaEstado = peticionEntidad.PlazoDeOferta > DateTime.Now.Date ? "Abierto" : "Cerrado";
            peticion.TieneVisitaObraMasiva = tieneVisitaMasiva;
            peticion.TipoPosicionCodigo = tipoPosicion;

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
                            TipoPosicionCodigo = po.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo,
                            TieneVisitaObraMasiva = po.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.TieneVisitaObraMasiva,
                            RevisionTecnicaId = po.RevisionTecnica_Id
                        });

            var revisionTecnica = repositorio.Obtener<PeticionDeOfertaRevisionTecnica>(x => x.Id == peticionDeOfertaDto.RevisionTecnicaId);
            if (revisionTecnica != null)
            {
                peticionDeOfertaDto.RevisionTecnica = new PeticionDeOfertaRevisionTecnicaDto
                {
                    Id = revisionTecnica.Id,
                    Usuario_Id = revisionTecnica.Usuario_Id,
                    Fecha = revisionTecnica.Fecha,
                    RecotizacionEconomica = revisionTecnica.RecotizacionEconomica,
                    ModificacionSolp = revisionTecnica.ModificacionSolp,
                    ObservacionRecotizacion = revisionTecnica.ObservacionRecotizacion,
                    Finalizada = revisionTecnica.Finalizada
                };
                peticionDeOfertaDto.RevisionFinalizada = revisionTecnica.Finalizada;
            }

            return peticionDeOfertaDto;
        }

        private static bool ValidacionCircularSolicitante(PeticionDeOfertaUsuario u, CotizacionDto cotizacion)
        {
            if (cotizacion == null || cotizacion.CotizacionEstado_Id != (int)CotizacionEstadoEnum.Cotizado)
            {
                return false;
            }
            if (u.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo == "MATERIALES")
            {
                return cotizacion.RespetaMateriales == true || u.PropuestaTecnicaAprobada == true;
            }
            else
            {
                return u.PropuestaTecnicaAprobada == true &&
                    (u.RealizoVisita == true || u.PeticionDeOferta.Posiciones.FirstOrDefault()?.SolpPosicion.Solp.Pliego.TieneVisitaObraMasiva != true);
            }
        }
        public void EnviarCircularAutomatico(Solp solp)
        {
            var peticiones = solp.Posiciones.SelectMany(x => x.Peticiones.Select(y => y.PeticionDeOferta)).ToList();
            var peticionesId = peticiones.Select(peticion => peticion.Id);
            var peticionUsuarios = repositorio.Listar<PeticionDeOfertaUsuario>(petiUsuario =>
                                  peticionesId.Contains(petiUsuario.PeticionDeOferta_Id)).ToList();
            var proveedoresRealizaronVisita = repositorio.Listar<PeticionDeOfertaUsuario>(petiUsuario =>
                                  peticionesId.Contains(petiUsuario.PeticionDeOferta_Id) && petiUsuario.RealizoVisita == true).ConvertAll(x => x.Usuario_Id);
            var idsTodos = peticionUsuarios.ConvertAll(x => x.Usuario_Id);
            var fechaEntrega = solp.Posiciones.OrderByDescending(x => x.FechaEntregaServicio).FirstOrDefault()?.FechaEntregaServicio;

            DateTime plazoDeOferta = solp.FechaLimiteReenvioDocumentacionPorCambioCondiciones ?? DateTime.Today.AddDays(7);

            CircularDto circularDto = new CircularDto
            {
                UsuarioId = solp.UsuarioCreacion_Id.Value,
                Observacion = "Es necesario que se realice una cotización nuevamente",
                PlazoDeOferta = plazoDeOferta,
                RequiereCambioDeFecha = true,
                FechaEntrega = fechaEntrega,
                UsuarioIds = solp.EnvioCircularA == EnviarCircularEnum.EnviarRealizaronVisita ? proveedoresRealizaronVisita : idsTodos
            };

            GrabarCircular(circularDto, null, true, peticionUsuarios);
        }

        public RespuestaGuardarSOLP GrabarCircular(CircularDto circularDto, HttpFileCollectionBase adjuntos, bool esAutomatico = false, List<PeticionDeOfertaUsuario> usuarios = null)
        {
            try
            {
                ValidarCircular(circularDto);
                var peticion = repositorio.Obtener<PeticionDeOferta>(circularDto.PeticionDeOferta_Id);
                var usuario = repositorio.Obtener<Usuario>(circularDto.UsuarioId);
                var rolUsuario = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "COMPRADOR" : "SOLP";
                usuarios = usuarios != null ? usuarios : peticion.Usuarios.Where(x => circularDto.UsuarioIds.Contains(x.Usuario_Id)).ToList();

                ReiniciarRevisionTecnica(rolUsuario, usuarios, esAutomatico);

                var circular = new Circular()
                {
                    UsuarioCreador_Id = circularDto.UsuarioId,
                    Usuario = usuario,
                    FechaCreacion = DateTime.Now,
                    Observaciones = circularDto.Observacion,
                    PlazoDeOferta = circularDto.PlazoDeOferta?.ToLocalTime(),
                    FechaDeEntrega = circularDto.FechaEntrega,
                    RequiereCambioDeFechas = circularDto.RequiereCambioDeFecha,
                    PeticionDeOfertaUsuarios = usuarios.Where(x => circularDto.UsuarioIds.Contains(x.Usuario_Id))
                    .Select(a => new CircularPeticionDeOfertaUsuario
                    {
                        PeticionDeOfertaUsuario_Id = a.Id
                    }).ToList()
                };

                circular = repositorio.Agregar(circular);
                repositorio.GuardarCambios();

                if (adjuntos?.Count > 0)
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

        private static void ReiniciarRevisionTecnica(string rolUsuario, IEnumerable<PeticionDeOfertaUsuario> usuarios, bool esAutomatico = false)
        {
            var hoy = DateTime.Now;
            foreach (var proveedor in usuarios)
            {
                // Obtener el plazo de oferta original y el plazo de oferta circular
                var plazoDeOfertaOriginal = proveedor.PeticionDeOferta.PlazoDeOferta;
                var plazoDeOfertaCircular = usuarios.GroupBy(x => x)
                    .SelectMany(x => x.Key.Circulares)
                    .Where(x => x.Circular != null && x.Circular.RequiereCambioDeFechas == true && x.Circular.PlazoDeOferta.HasValue)
                    .OrderByDescending(x => x.Circular.Id)
                    .FirstOrDefault()?.Circular.PlazoDeOferta;

                // Obtener la fecha de creación de la última circular que requiere cambio de fechas
                var fechaCreacionCircular = usuarios.GroupBy(x => x)
                    .SelectMany(x => x.Key.Circulares)
                    .Where(x => x.Circular != null && x.Circular.RequiereCambioDeFechas == true)
                    .OrderByDescending(x => x.Circular.Id)
                    .FirstOrDefault()?.Circular.FechaCreacion;

                var plazoDeOfertaCierre = proveedor.PeticionDeOferta.Cierres.Any()
                    ? proveedor.PeticionDeOferta.Cierres.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha
                    : (DateTime?)null;

                if (plazoDeOfertaCierre.HasValue && plazoDeOfertaCircular.HasValue && plazoDeOfertaCierre < fechaCreacionCircular)
                {
                    plazoDeOfertaCierre = plazoDeOfertaCircular;
                }

                // Verificar las condiciones para reiniciar la propuesta
                bool reiniciarPropuesta = (plazoDeOfertaOriginal < hoy) &&
                                          (plazoDeOfertaCierre.HasValue && plazoDeOfertaCierre < hoy);


                if (!esAutomatico || (reiniciarPropuesta && esAutomatico))
                {
                    if (rolUsuario == "SOLP")
                    {
                        proveedor.PropuestaTecnicaAprobada = null;
                        proveedor.PropuestaTecnicaFecha = null;
                        proveedor.PropuestaTecnicaUsuario_Id = null;
                        proveedor.ObservacionNoCumple = "";
                        if (esAutomatico)
                        {
                            proveedor.PeticionDeOferta.RevisionTecnica.Finalizada = false;
                        }


                    }
                }
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

            var solps = circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Posiciones.Select(x => x.SolpPosicion.Solp);

            var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList();
            mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
            var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList();

            copia.AddRange(mailPliego);
            copia.AddRange(mailCreador);

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

                emailService.EnviarMail(enviarA, asunto, "", copia.Distinct().ToList(), CuerpoMailCircular(prov), null, null, null, null, archs);
            }
        }

        private Dictionary<string, byte[]> ObtenerArchivosCircular(Circular circular)
        {
            var archs = new Dictionary<string, byte[]>();
            foreach (var p in circular.Archivos)
            {
                byte[] b = File.ReadAllBytes(p.Ruta);
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
                    EnviarMailPeticionDeOferta(peticion, poUsuarios, poUsuariosAdicionales, false);
                }
                catch (Exception e)
                {
                    Log.Error($"Error al enviar mail GrabarProveedoresEnPeticionDeOferta en peticion: {peticion.Id}", e);
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception)
            {
                throw;
            }
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
                            if (cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo == "MATERIALES")
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

        public RespuestaGuardarSOLP GrabarRevisionTecnica(List<PeticionDeOfertaUsuarioDto> peticionDeOfertaUsuarioDto, int usuarioId, bool finalizar, PeticionDeOfertaRevisionTecnicaDto revision)
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
                }
                if (data.ObservacionNoCumple != null)
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
                peticiones.First().PeticionDeOferta.RevisionTecnica.FechaFinalizacion = DateTime.Now;

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
            respuesta.IdEntidad = peticiones.First().PeticionDeOferta_Id;

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
                peticionCotizacion.NroSolp = peticionCotizacion.NrosSolp != null ? string.Join(", ", peticionCotizacion.NrosSolp.Distinct()) : "";
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
                peticionCotizacion.Cotizacion.CotizacionesHorasOriginal = listaHoras;


                if (peticionCotizacion.TipoPosicionCodigo == "MATERIALES")
                {
                    var todasLasUM = comprasServiceSap.ObtenerTablaSap(TablasSap.Unidad);
                    var unidadesDeMedidaSAP = unidadMedidaService.ObtenerUnidadesDesdeServicioSap(peticionCotizacion.PeticionDeOfertaPosicion.Where(x => x.Posiciones.Codigo != null).Select(x => x.Posiciones.Codigo).ToList());
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

                peticionCotizacion.PeticionDeOfertaPosicion
                    = peticionCotizacion.PeticionDeOfertaPosicion.Where(a => a.Posiciones.Cantidad > 0);

                if (!peticionCotizacion.PeticionDeOfertaPosicion.Any())
                {
                    throw new WSCustomException("La petición de oferta no tiene posiciones pendientes, por favor contáctese con el área de compras.");
                }

                if (peticionCotizacion.ArchivosPaso4Cotizacion != null)
                {
                    peticionCotizacion.ArchivosPaso4Cotizacion = peticionCotizacion.ArchivosPaso4Cotizacion.Select(archivo => new ArchivoDto
                    {
                        Id = archivo.Id,
                        Nombre = Path.GetFileName(archivo.Ruta),
                    }).ToList();
                }

                return peticionCotizacion;
            }
            catch (Exception e)
            {
                Logger.Log.Error($"TraerCotizacion {e.Message}", e);
                Log.Error(e);
                throw;
            }
        }

        public RespuestaGuardarSOLP GrabarCotizacion(GuardarCotizacion cotizacionDto, HttpFileCollectionBase adjuntos, bool esFinalizado, bool enviarMail = true)
        {
            var respuestaGuardarSOLP = new RespuestaGuardarSOLP() { Errores = new List<string>() };
            var peticionUsuario = repositorio.Obtener<PeticionDeOfertaUsuario>(cotizacionDto.PeticionOfertaUsuarioId);
            var peticionDeOfertaPosicionIds = cotizacionDto.CotizacionPosiciones.Select(cotipos => cotipos.PeticionDeOfertaSolpPosicionId);
            var peticionDeOfertaSolpPosiciones = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => peticionDeOfertaPosicionIds.Contains(x.Id));
            var solpSubposicionesIds = cotizacionDto.CotizacionSubposiciones.Select(x => x.SolpSubPosicionId);
            var solpSubposiciones = repositorio.Listar<SolpSubposicion>(x => solpSubposicionesIds.Contains(x.Id));
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
                            Moneda_Id = x.MonedaId > 0 ? x.MonedaId : null,
                            Moneda = x.MonedaId > 0 ? info.Find(moneda => moneda.Id == x.MonedaId) : null,
                            Precio = x.Precio,
                            UnidadDeMedida_Id = x.UnidadDeMedidaId > 0 ? x.UnidadDeMedidaId : null,
                            UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == x.UnidadDeMedidaId) : null,
                            PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                            PeticionDeOfertaSolpPosicion = peticionDeOfertaSolpPosiciones.Find(peticion => peticion.Id == x.PeticionDeOfertaSolpPosicionId),
                            NoDisponible = x.NoDisponible,
                            FechaDeVigencia = x.FechaDeVigencia != null ? x.FechaDeVigencia.Value : (DateTime?)null,
                            CotizacionSubPosiciones = cotizacionDto.CotizacionSubposiciones.Count > 0 ? cotizacionDto.CotizacionSubposiciones
                            .Where(y => y.CotizacionPosicionId == x.PeticionDeOfertaSolpPosicionId).Select(sub => new CotizacionSubPosicion
                            {
                                Cantidad = sub.Cantidad,
                                Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : null,
                                Moneda = sub.MonedaId > 0 ? info.Find(moneda => moneda.Id == sub.MonedaId) : null,
                                Precio = sub.Precio,
                                UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : null,
                                UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == sub.UnidadDeMedidaId) : null,
                                CotizacionPosicion_Id = sub.CotizacionPosicionId,
                                SolpSubPosicion_Id = sub.SolpSubPosicionId,
                                SolpSubPosicion = solpSubposiciones.Find(subpo => subpo.Id == sub.SolpSubPosicionId)
                            }).ToList() : null,
                            PrimerPlazoDeOferta = x.PrimerPlazoDeOferta,
                            PrimeraCantidad = x.PrimeraCantidad,
                            SegundoPlazoDeOferta = x.SegundoPlazoDeOferta,
                            SegundaCantidad = x.SegundaCantidad,
                            TercerPlazoDeOferta = x.TercerPlazoDeOferta,
                            TerceraCantidad = x.TerceraCantidad,


                        }).ToList() : null,
                        UsuarioCreador = peticionUsuario.Usuario,
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
                cotizacion.CotizarNuevaPosicion = false;
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
            if (peticionUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo == "MATERIALES")
            {
                tieneUnidadDeMedidaNula = cotizacionDto.CotizacionPosiciones.Where(x => !(x.NoDisponible == true))?.Any(pos =>
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


            if (cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado && enviarMail)
            {
                emailComprasService.EnviarMailCotizacionCreada(cotizacion);
            }

            try
            {
                if (ValidarCreacionDeRegistroInfo(cotizacion) && !cotizacion.CotizacionPosiciones.All(x => x.NoDisponible == true && x.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp != null))
                {
                    var registros = CrearRegistroInfoDto(cotizacion);
                    if (registros.Exists(x => !x.EsModificar))
                    {
                        CrearRegistroInfo(cotizacion, registros.Where(x => !x.EsModificar).ToList());
                    }
                    registros.ForEach(x => x.EsModificar = true);
                    CrearRegistroInfo(cotizacion, registros);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error($"Error al generar registro info en GrabarCotizacion para la cotizacion: " + cotizacion.Id, e);
            }


            respuestaGuardarSOLP.IdEntidad = cotizacion.Id;
            repositorio.GuardarCambios();

            if (cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado)
            {
                GrabarLogCotizacion(cotizacion);
            }

            return respuestaGuardarSOLP;
        }

        private static bool ValidarCreacionDeRegistroInfo(Cotizacion cotizacion)
        {
            return cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.RegistroInfo != true && cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado
                                && cotizacion.CotizacionPosiciones.FirstOrDefault().PeticionDeOfertaSolpPosicion.SolpPosicion.TipoPosicion.Codigo == "MATERIALES";
        }

        private void CrearRegistroInfo(Cotizacion cotizacion, List<RegistroInfoDto> registros)
        {
            if (!ProveedorExisteEnSAP(cotizacion.UsuarioCreador.ObtenerCodigoProveedor()))
            {
                return;
            }
            var respuesta = registroInfoService.CrearOActualizarRegistrosInfoEnSap(registros);
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

        private void AgregarPosicionACotizacionTrabajoYaHechoOPresupuestado(List<Cotizacion> cotizaciones, Solp solp)
        {
            try
            {
                if (solp.TrabajoYaHecho == true || solp.ConPresupuesto)
                {
                    var posiciones = solp.Posiciones.Where(x => x.Estado);
                    var posicionesId = posiciones.Select(x => x.Id);
                    var peticionDeOfertaSolpPosicion = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => posicionesId.Contains(x.SolpPosicion_Id)).ToList();

                    var subposiciones = solp.Posiciones.SelectMany(posis => posis.Subposiciones);

                    foreach (var cotizacion in cotizaciones)
                    {
                        foreach (var posicion in posiciones)
                        {
                            var cotizacionPosicionEntidad = cotizacion.CotizacionPosiciones.FirstOrDefault(coti => coti.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id);
                            if (cotizacionPosicionEntidad == null)
                            {
                                var peticion = peticionDeOfertaSolpPosicion.FirstOrDefault(posi => posi.SolpPosicion_Id == posicion.Id);
                                cotizacionPosicionEntidad = new CotizacionPosicion
                                {
                                    Cantidad = posicion.Cantidad,
                                    Cotizacion = cotizacion,
                                    Cotizacion_Id = cotizacion.Id,
                                    FechaDeEntrega = posicion.FechaEntregaServicio,
                                    Moneda = posicion.Moneda,
                                    Moneda_Id = posicion.Moneda_Id,
                                    Precio = posicion.PrecioBruto,
                                    UnidadDeMedida = posicion.Unidad,
                                    UnidadDeMedida_Id = posicion.Unidad_Id,
                                    PeticionDeOfertaSolpPosicion_Id = peticion.Id,
                                    PeticionDeOfertaSolpPosicion = peticion,
                                };
                                cotizacion.CotizacionPosiciones.Add(cotizacionPosicionEntidad);
                            }
                            else
                            {
                                cotizacionPosicionEntidad.Cantidad = posicion.Cantidad;
                                cotizacionPosicionEntidad.FechaDeEntrega = posicion.FechaEntregaServicio;
                                cotizacionPosicionEntidad.Moneda = posicion.Moneda;
                                cotizacionPosicionEntidad.Moneda_Id = posicion.Moneda_Id;
                                cotizacionPosicionEntidad.Precio = posicion.PrecioBruto;
                                cotizacionPosicionEntidad.UnidadDeMedida = posicion.Unidad;
                                cotizacionPosicionEntidad.UnidadDeMedida_Id = posicion.Unidad_Id;
                            }

                            foreach (var subposicion in subposiciones.Where(subpoPosicion => subpoPosicion.SolpPosicion_Id == posicion.Id).ToList())
                            {
                                var cotizacionSubposicionEntidad = cotizacionPosicionEntidad.CotizacionSubPosiciones.FirstOrDefault(subpo => subpo.SolpSubPosicion_Id == subposicion.Id);
                                if (cotizacionSubposicionEntidad == null)
                                {
                                    var subpoNueva = new CotizacionSubPosicion
                                    {
                                        Cantidad = subposicion.Cantidad,
                                        Moneda = posicion.Moneda,
                                        Moneda_Id = posicion.Moneda_Id,
                                        UnidadDeMedida = subposicion.Unidad,
                                        UnidadDeMedida_Id = subposicion.Unidad_Id,
                                        Precio = subposicion.PrecioBruto,
                                        SolpSubPosicion = subposicion,
                                        SolpSubPosicion_Id = subposicion.Id,
                                        CotizacionPosicion = cotizacionPosicionEntidad,
                                        CotizacionPosicion_Id = cotizacionPosicionEntidad.Id
                                    };
                                    cotizacionPosicionEntidad.CotizacionSubPosiciones.Add(subpoNueva);
                                }
                                else
                                {
                                    cotizacionSubposicionEntidad.Cantidad = subposicion.Cantidad;
                                    cotizacionSubposicionEntidad.Moneda = posicion.Moneda;
                                    cotizacionSubposicionEntidad.Moneda_Id = posicion.Moneda_Id;
                                    cotizacionSubposicionEntidad.UnidadDeMedida = subposicion.Unidad;
                                    cotizacionSubposicionEntidad.UnidadDeMedida_Id = subposicion.Unidad_Id;
                                    cotizacionSubposicionEntidad.Precio = subposicion.PrecioBruto;
                                }
                            }
                        }

                        repositorio.GuardarCambios();
                        GrabarHistorialDeCotizaciones(cotizaciones);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error al AgregarPosicionACotizacionTrabajoYaHechoOPresupuestado", e);
            }
        }

        private void GuardarCotizacionPosicion(GuardarCotizacion cotizacionDto, Cotizacion cotizacion, List<TablaSap> info)
        {
            if (cotizacionDto == null) { throw new ArgumentNullException(nameof(cotizacionDto)); }
            if (cotizacion == null) { throw new ArgumentNullException(nameof(cotizacion)); }
            if (info == null) { throw new ArgumentNullException(nameof(info)); }

            List<CotizacionPosicion> posicionesDb = cotizacion.CotizacionPosiciones.Where(x => x.PeticionDeOfertaSolpPosicion.SolpPosicion.Estado).ToList();
            if (cotizacion.CotizacionPosiciones != null && posicionesDb.Count > 0)
            {
                foreach (CotizacionPosicion cotizacionPosicion in posicionesDb)
                {
                    GuardarCotizacionPosicionDto cotizacionPos = cotizacionDto.CotizacionPosiciones
                        .SingleOrDefault(x => x.PeticionDeOfertaSolpPosicionId == cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id);
                    if (cotizacionPos == default) { continue; }

                    cotizacionPosicion.Cantidad = cotizacionPos.Cantidad;
                    cotizacionPosicion.Precio = cotizacionPos.Precio;
                    cotizacionPosicion.FechaDeEntrega = cotizacionPos.FechaDeEntrega;
                    cotizacionPosicion.Moneda_Id = cotizacionPos.MonedaId > 0 ? cotizacionPos.MonedaId : null;
                    cotizacionPosicion.UnidadDeMedida_Id = cotizacionPos.UnidadDeMedidaId > 0 ? cotizacionPos.UnidadDeMedidaId : null;
                    cotizacionPosicion.Moneda = cotizacionPos.MonedaId > 0 && cotizacionPos.MonedaId != null ? info.Find(moneda => moneda.Id == cotizacionPos.MonedaId) : null;
                    cotizacionPosicion.UnidadDeMedida = cotizacionPos.UnidadDeMedidaId > 0 && cotizacionPos.UnidadDeMedidaId != null ? info.Find(unidad => unidad.Id == cotizacionPos.UnidadDeMedidaId) : null;
                    cotizacionPosicion.NoDisponible = cotizacionPos.NoDisponible;
                    cotizacionPosicion.FechaDeVigencia = cotizacionPos.FechaDeVigencia;
                    cotizacionPosicion.PrimerPlazoDeOferta = cotizacionPos.PrimerPlazoDeOferta;
                    cotizacionPosicion.PrimeraCantidad = cotizacionPos.PrimeraCantidad;
                    cotizacionPosicion.SegundoPlazoDeOferta = cotizacionPos.SegundoPlazoDeOferta;
                    cotizacionPosicion.SegundaCantidad = cotizacionPos.SegundaCantidad;
                    cotizacionPosicion.TercerPlazoDeOferta = cotizacionPos.TercerPlazoDeOferta;
                    cotizacionPosicion.TerceraCantidad = cotizacionPos.TerceraCantidad;

                    if (cotizacionPosicion.CotizacionSubPosiciones?.Count > 0)
                    {
                        foreach (var item in cotizacionPosicion.CotizacionSubPosiciones)
                        {
                            var sub = cotizacionDto.CotizacionSubposiciones.Find(x => x.CotizacionSubPosicionId == item.Id);
                            if (sub != null)
                            {
                                item.Cantidad = sub.Cantidad;
                                item.Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : null;
                                item.Moneda = sub.MonedaId > 0 ? info.Find(moneda => moneda.Id == sub.MonedaId) : null;
                                item.Precio = sub.Precio;
                                item.UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : null;
                                item.UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == sub.UnidadDeMedidaId) : null;
                                item.SolpSubPosicion_Id = sub.SolpSubPosicionId;
                            }
                        }
                    }

                    if (cotizacionDto.CotizacionSubposiciones.Exists(x => x.CotizacionSubPosicionId == 0 || x.CotizacionSubPosicionId == null))
                    {
                        foreach (var sub in cotizacionDto.CotizacionSubposiciones.Where(x => (x.CotizacionSubPosicionId == 0 || x.CotizacionSubPosicionId == null) &&
                        x.CotizacionPosicionId == cotizacionPosicion.PeticionDeOfertaSolpPosicion_Id))
                        {
                            cotizacionPosicion.CotizacionSubPosiciones.Add(new CotizacionSubPosicion
                            {
                                Cantidad = sub.Cantidad,
                                Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : null,
                                Moneda = sub.MonedaId > 0 ? info.Find(moneda => moneda.Id == sub.MonedaId) : null,
                                Precio = sub.Precio,
                                UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : null,
                                UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == sub.UnidadDeMedidaId) : null,
                                CotizacionPosicion_Id = sub.CotizacionPosicionId,
                                SolpSubPosicion_Id = sub.SolpSubPosicionId
                            });
                        }
                    }
                }
            }
            else
            {
                cotizacion.CotizacionPosiciones = cotizacionDto.CotizacionPosiciones.ConvertAll(x => new CotizacionPosicion
                {
                    Cantidad = x.Cantidad,
                    FechaDeEntrega = x.FechaDeEntrega != null ? x.FechaDeEntrega.Value : (DateTime?)null,
                    Moneda_Id = x.MonedaId > 0 ? x.MonedaId : null,
                    Precio = x.Precio,
                    UnidadDeMedida_Id = x.UnidadDeMedidaId > 0 ? x.UnidadDeMedidaId : null,
                    UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == x.UnidadDeMedidaId) : null,
                    PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                    Moneda = x.MonedaId > 0 ? info.Find(moneda => moneda.Id == x.MonedaId) : null,
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
                        Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : null,
                        Moneda = sub.MonedaId > 0 ? info.Find(moneda => moneda.Id == sub.MonedaId) : null,
                        Precio = sub.Precio,
                        UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : null,
                        UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Find(unidad => unidad.Id == sub.UnidadDeMedidaId) : null,
                        CotizacionPosicion_Id = sub.CotizacionPosicionId,
                        SolpSubPosicion_Id = sub.SolpSubPosicionId
                    }).ToList() : null,
                });
            }
            CompletarCotizacionPosicionYSubposicionNueva(cotizacionDto, cotizacion, info);
        }

        private void CompletarCotizacionPosicionYSubposicionNueva(GuardarCotizacion cotizacionDto, Cotizacion cotizacion, List<TablaSap> info)
        {
            AgregarNuevaCotizacionPosicion(cotizacionDto, cotizacion, info);
        }

        private void AgregarNuevaCotizacionPosicion(GuardarCotizacion cotizacionDto, Cotizacion cotizacion, List<TablaSap> info)
        {
            var peticionDeOfertaSolpPosicionExistente = cotizacion.CotizacionPosiciones.Select(x => x.PeticionDeOfertaSolpPosicion_Id).ToList();
            var cotizacionPosicionesNueva = cotizacionDto.CotizacionPosiciones.Where(x => !peticionDeOfertaSolpPosicionExistente.Contains(x.PeticionDeOfertaSolpPosicionId)).ToList();

            var peticionDeOfertaPosicionIds = cotizacionPosicionesNueva.Select(cotipos => cotipos.PeticionDeOfertaSolpPosicionId);
            var peticionDeOfertaSolpPosiciones = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => peticionDeOfertaPosicionIds.Contains(x.Id));
            var solpSubposicionesIds = cotizacionDto.CotizacionSubposiciones.Select(x => x.SolpSubPosicionId);
            var solpSubposiciones = repositorio.Listar<SolpSubposicion>(x => solpSubposicionesIds.Contains(x.Id));
            var posicionesCotizacion = cotizacionPosicionesNueva.Select(x => new CotizacionPosicion
            {
                Cantidad = x.Cantidad,
                FechaDeEntrega = x.FechaDeEntrega != null ? x.FechaDeEntrega.Value : peticionDeOfertaSolpPosiciones.Where(peticion => peticion.Id == x.PeticionDeOfertaSolpPosicionId).FirstOrDefault().SolpPosicion.FechaEntregaServicio,
                Moneda_Id = x.MonedaId > 0 ? x.MonedaId : null,
                Moneda = x.MonedaId > 0 ? info.Where(moneda => moneda.Id == x.MonedaId).FirstOrDefault() : null,
                Precio = x.Precio,
                PrimerPlazoDeOferta = x.PrimerPlazoDeOferta,
                UnidadDeMedida_Id = x.UnidadDeMedidaId > 0 ? x.UnidadDeMedidaId : null,
                UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == x.UnidadDeMedidaId).FirstOrDefault() : null,
                PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                PeticionDeOfertaSolpPosicion = peticionDeOfertaSolpPosiciones.Where(peticion => peticion.Id == x.PeticionDeOfertaSolpPosicionId).FirstOrDefault(),
                NoDisponible = x.NoDisponible,
                FechaDeVigencia = x.FechaDeVigencia != null ? x.FechaDeVigencia.Value : (DateTime?)null,
                CotizacionSubPosiciones = cotizacionDto.CotizacionSubposiciones.Count > 0 ? cotizacionDto.CotizacionSubposiciones
                                            .Where(y => y.CotizacionPosicionId == x.PeticionDeOfertaSolpPosicionId).Select(sub => new CotizacionSubPosicion
                                            {
                                                Cantidad = sub.Cantidad,
                                                Moneda_Id = sub.MonedaId > 0 ? sub.MonedaId : null,
                                                Moneda = sub.MonedaId > 0 ? info.Where(moneda => moneda.Id == sub.MonedaId).FirstOrDefault() : null,
                                                Precio = sub.Precio,
                                                UnidadDeMedida_Id = sub.UnidadDeMedidaId > 0 ? sub.UnidadDeMedidaId : null,
                                                UnidadDeMedida = sub.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == sub.UnidadDeMedidaId).FirstOrDefault() : null,
                                                CotizacionPosicion_Id = sub.CotizacionPosicionId,
                                                SolpSubPosicion_Id = sub.SolpSubPosicionId,
                                                SolpSubPosicion = solpSubposiciones.Where(subpo => subpo.Id == sub.SolpSubPosicionId).FirstOrDefault()
                                            }).ToList() : null
            });
            foreach (var posicion in posicionesCotizacion.ToList())
            {
                cotizacion.CotizacionPosiciones.Add(posicion);
            }

            repositorio.GuardarCambios();

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
                    }
                }
            }
        }

        private void EnviarMailAvisoDeErrorRegistroInfo(Cotizacion cotizacion)
        {
            var asunto = "";
            var enviarA = new List<string> { ConfigurationManager.AppSettings["EmailRegistroInfo"] };
            asunto += "Error al agregar registro info en cotizacion: " + cotizacion.Id;

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString("Se informa que al momento de finalizar una cotizacion, el registro info no se pudo generar, revisar los logs", null, "text/html");
            emailService.EnviarMail(enviarA, asunto, "", null, alternateView, null, null, null, null);
        }

        public GuardarCotizacion ObtenerPrecioTotalPosicionProveedor(GuardarCotizacion cotizacionDto)
        {
            try
            {
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
                                        var tipoCambio = tipoCambioService.ObtenerTipoCambio(subpos.MonedaId.Value, destino.Id, DateTime.Now);
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
                var tablasapMoneda = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                var numerosDePedido = new List<string>();
                var esMateriales = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo == "MATERIALES";

                if (adjudicacionDto.EsMonedaProveedor)
                {
                    var monedaProv = DevolverMonedaProveedor(adjudicacionDto.Proveedor).Moneda;
                    if (!string.IsNullOrEmpty(monedaProv))
                    {
                        adjudicacionDto.Moneda_Id = tablasapMoneda.FirstOrDefault(moneda => moneda.CodigoSap == monedaProv).Id;
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
                        adjudicacionDto.AdjudicacionPosiciones.ForEach(x => x.MonedaId = cotizacionPosiciones.FirstOrDefault()?.CotizacionSubPosiciones.FirstOrDefault()?.Moneda_Id);
                    }
                }

                var todasLasCotizacionPosiciones = cotizacion.CotizacionPosiciones.ToDictionary(x => x.Id);
                var peticionDeOfertaPosiciones = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion_Id);
                var todasLasSolpPosiciones = repositorio.Listar<SolpPosicion>(posi => peticionDeOfertaPosiciones.Contains(posi.Id)).ToDictionary(x => x.Id);

                var posicionesAgrupadas = adjudicacionDto.AdjudicacionPosiciones
                    .GroupBy(posicion =>
                    {
                        var solpPosicion = todasLasSolpPosiciones[posicion.SolpPosicion_Id];
                        var tipoSolp = solpPosicion.Solp.Adicional == true ? "ADICIONAL"
                                    : solpPosicion.Solp.TrabajoYaHecho == true ? "TRABAJOYAHECHO"
                                    : "OTRO";
                        return new { posicion.MonedaId, TipoSolp = tipoSolp };
                    })
                    .ToList();

                var regiones = repositorio.Listar<RegionSap>();

                foreach (var grupo in posicionesAgrupadas)
                {
                    var monedaKey = grupo.Key.MonedaId;
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
                                : x.PlazoDeEntrega.Value.AddDays(cotizacionPosicion?.PrimerPlazoDeOferta ?? 0)
                            };

                            if (!esMateriales)
                            {
                                monto = DevolverMontoServicio(ap.CotizacionPosicion.CotizacionSubPosiciones.ToList(), tablasapMoneda.FirstOrDefault(moneda => moneda.Id == monedaKey)?.Codigo);
                            }
                            else
                            {
                                monto = cotizacionPosicion?.Precio.Value *
                                tipoCambioService.ObtenerTipoCambio(cotizacionPosicion?.Moneda_Id ?? 0, monedaKey.Value, DateTime.Now).TipoCambio ?? 0;
                            }
                            ap.Monto = monto;
                            return ap;

                        }).ToList();

                        adjudicacion = new Adjudicacion
                        {
                            Cotizacion_Id = adjudicacionDto.Cotizacion_Id,
                            Moneda_Id = monedaKey.Value,
                            Moneda = tablasapMoneda.FirstOrDefault(moneda => moneda.Id == monedaKey.Value),
                            Cotizacion = cotizacion,
                            FechaCreacion = DateTime.Now,
                            Usuario = usuario,
                            UsuarioCreador_Id = usuario.Id,
                            MontoTotal = CalcularMontoTotal(adjudicacionDto, cotizacion, tablasapMoneda),
                            CondicionesDeEntrega = adjudicacionDto.CondicionesDeEntrega,
                            CondicionesDePago = adjudicacionDto.CondicionesDePago,
                            Garantias = adjudicacionDto.Garantias,
                            TextoDeCabecera = adjudicacionDto.TextoDeCabecera,
                            Posiciones = posiciones,
                            //Token = Guid.NewGuid().ToString(),
                            RegionSap = regiones.FirstOrDefault(c => c.Id == adjudicacionDto.RegionSap),
                            RegionSap_Id = adjudicacionDto.RegionSap,
                            NumeroOrdenDeCompra = "",
                            AdmiteCertificacionesParciales = esMateriales ? true : adjudicacionDto.AdmiteCertificacionesParciales
                        };
                        if (!esMateriales && adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.Adicional == true)
                        {
                            var NroOrdenDeCompraAdicional = adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.NroOrdenDeCompraAdicional;
                            var ordenesDeCompraAnteriores = repositorio.Listar<Adjudicacion>(a => a.NumeroOrdenDeCompra == NroOrdenDeCompraAdicional);
                            ordenesDeCompraAnteriores.ForEach(a => a.AdmiteCertificacionesParciales = adjudicacionDto.AdmiteCertificacionesParciales);
                        }
                        repositorio.Agregar(adjudicacion);
                        repositorio.GuardarCambios();
                        respuestaGuardarSOLP = comprasServiceSap.CrearOrdenDeCompra(adjudicacion, adjudicacionDto.CreadoAutomatico);

                        if (respuestaGuardarSOLP.Errores == null || !respuestaGuardarSOLP.Errores.Any())
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
                            Log.Info($"Error al enviar mail {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id} para el cierre de la cotización");
                        }
                    }
                }
                respuestaGuardarSOLP.NumerosDePedido = numerosDePedido;
                ActualizarDatosSolp(numerosDePedido, cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta);
                return respuestaGuardarSOLP;
            }
            catch (Exception)
            {
                if (adjudicacion != null && adjudicacion.Id > 0 && string.IsNullOrEmpty(adjudicacion.NumeroOrdenDeCompra))
                {
                    repositorio.Remover(adjudicacion);
                    repositorio.GuardarCambios();
                }
                throw;
            }
        }

        private decimal DevolverMontoServicio(List<CotizacionSubPosicion> cotizacionSubposiciones, string monedaCodigo)
        {
            decimal total = 0;
            var fecha = DateTime.Now;

            foreach (var item in cotizacionSubposiciones)
            {
                decimal tipoDeCambio = 1;
                if (item.Moneda.Codigo != monedaCodigo)
                {
                    tipoDeCambio = tipoCambioService.ObtenerTipoCambio(item.Moneda.Codigo, monedaCodigo, fecha.ToString("yyyy-MM-dd")).TipoCambio;
                }
                total += item.Cantidad.Value * item.Precio.Value * tipoDeCambio;
            }

            return total;
        }

        private decimal DevolverMontoServicioSolicitado(string monedaSolicitada, string monedaCotizada, List<SolpSubposicion> subpos)
        {
            decimal total = 0;
            var fecha = DateTime.Now;
            decimal tipoDeCambio = 1;

            if (monedaSolicitada != monedaCotizada)
            {
                tipoDeCambio = tipoCambioService.ObtenerTipoCambio(monedaSolicitada, monedaCotizada, fecha.ToString("yyyy-MM-dd")).TipoCambio;
            }

            foreach (var item in subpos)
            {
                total += item.Cantidad.Value * item.PrecioBruto.Value * tipoDeCambio;
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
            var adjudicacionPosicion = adjudicacionDto.AdjudicacionPosiciones.Select(y => y.CotizacionPosicion_Id);
            var cotizacionPosiciones = cotizacion.CotizacionPosiciones.Where(x => adjudicacionPosicion.Contains(x.Id));
            if (cotizacionPosiciones != null)
            {
                bool esServicios = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo != "MATERIALES";
                foreach (var cotizacionPosicion in cotizacionPosiciones)
                {
                    if (esServicios)
                    {
                        if (cotizacionPosicion.CotizacionSubPosiciones != null && cotizacionPosicion.CotizacionSubPosiciones.Any())
                        {
                            foreach (var subpos in cotizacionPosicion.CotizacionSubPosiciones)
                            {

                                if (subpos.Precio > 0 && subpos.Cantidad > 0 && subpos.Moneda_Id.Value > 0)
                                {
                                    decimal totalSub;
                                    cambio = 1;

                                    if ((subpos.Moneda_Id != null && !tipodecambio.TryGetValue(subpos.Moneda_Id.Value, out cambio)))
                                    {
                                        var tipoCambio = tipoCambioService.ObtenerTipoCambio(subpos.Moneda_Id.Value, info.First(moneda => moneda.CodigoSap == "ARP").Id, DateTime.Now);
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
                            var tipoCambio = tipoCambioService.ObtenerTipoCambio(cotizacionPosicion.Moneda_Id.Value, info.First(moneda => moneda.CodigoSap == "ARP").Id, DateTime.Now);
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

        public PeticionDeOferta CrearCotizacionConTrabajoYaHechoOPresupuestado(Solp solp)
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
                ObservacionRecotizacion = solp.ConPresupuesto ? "Con presupuesto" : "Trabajo ya hecho",
                Finalizada = true,
                FechaFinalizacion = DateTime.Now
            };
            peticionEntidad.PlazoDeOferta = DateTime.Now;
            repositorio.GuardarCambios();
            return peticionEntidad;
        }

        private RespuestaCrearOrdenDeCompra CrearOrdenDeCompraAutomatica(Solp solp, bool enviarMail, List<RegistroInfoDto> registroInfo, bool crearAdjudicacion, int usuarioActual)
        {
            var respuestaGuardarSOLP = new RespuestaCrearOrdenDeCompra();
            RespuestaGuardarSOLP respuestaCotizacion;
            Cotizacion cotizacionNueva;
            var solpPosicionIds = registroInfo.Select(registro => registro.PosicionId).ToList();
            var centroSolp = solp.Posiciones.First().Centro.CodigoSap;
            var centroRegion = repositorio.Obtener<CentroDireccion>(cr => cr.CodigoSap == centroSolp);
            var includes = new List<Expression<Func<SolpPosicion, object>>>();
            includes.Add(x => x.Solp);
            List<SolpPosicion> posiciones = repositorio.Listar<SolpPosicion>(a => solpPosicionIds.Contains(a.Id), includes: includes);
            //Crear Peticion 
            var usuariosIds = new List<int>();
            int proveedorId = registroInfo.Select(x => x.ProveedorId).First();
            usuariosIds.Add(proveedorId);
            PeticionDeOferta peticionEntidad = CrearPeticionAutomatica(solp, usuariosIds, posiciones, true, registroInfo, usuarioActual);
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

        private void CrearCotizacionAutomatica(Solp solp, bool enviarMail, PeticionDeOferta peticionEntidad, out RespuestaGuardarSOLP respuestaCotizacion, out Cotizacion cotizacionNueva, List<SolpPosicion> solpPosiciones = null, List<RegistroInfoDto> registroInfo = null)
        {
            var posiciones = solpPosiciones != null ? solpPosiciones : solp.Posiciones;
            GuardarCotizacion cotizacion = CrearCotizacionDto(peticionEntidad, registroInfo, posiciones);
            respuestaCotizacion = GrabarCotizacion(cotizacion, null, true, enviarMail);
            cotizacionNueva = repositorio.Obtener<Cotizacion>(respuestaCotizacion.IdEntidad);
        }

        private static GuardarCotizacion CrearCotizacionDto(PeticionDeOferta peticionEntidad, List<RegistroInfoDto> registroInfo, ICollection<SolpPosicion> posiciones)
        {
            var cotizacion = new GuardarCotizacion
            {
                RespetaMateriales = true,
                RespetaServicios = true,
                FechaDeEntrega = DateTime.Today.AddDays(-1),
                PeticionOfertaUsuarioId = peticionEntidad.Usuarios.Select(x => x.Id).FirstOrDefault(),
                CotizacionPosiciones = posiciones.Select(x => new GuardarCotizacionPosicionDto
                {
                    PeticionDeOfertaSolpPosicionId = peticionEntidad.Posiciones.Where(posicion => posicion.SolpPosicion_Id == x.Id).First().Id,
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
                            Cantidad = (decimal)subposicion.Cantidad,
                            UnidadDeMedidaId = subposicion.Unidad_Id,
                            SolpSubPosicionId = subposicion.Id,
                            CotizacionPosicionId = peticionEntidad.Posiciones.Where(pos => pos.SolpPosicion_Id == posicion.Id).FirstOrDefault().Id,
                            MonedaId = posicion.Moneda_Id
                        };
                        cotizacion.CotizacionSubposiciones.Add(subpos);
                    }
                }
            }

            return cotizacion;
        }

        private PeticionDeOferta CrearPeticionAutomatica(Solp solp, List<int> usuariosIds, List<SolpPosicion> solpPosicions = null, bool esRegistroInfo = false, List<RegistroInfoDto> registroInfoLista = null, int? usuarioCreacionId = null)
        {
            int usuarioCreacionPOId = usuarioCreacionId ?? ObtenerCompradorCondicionesEspeciales(solp);
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
            var enviarMail = solp.TrabajoYaHecho != true && !solp.ConPresupuesto && (solp.Adicional == true || solp.ProveedorAsignado_Id > 0);
            var resultado = GrabarPeticionDeOferta(peticion, null, enviarMail, registroInfoLista);
            var peticionEntidad = repositorio.Obtener<PeticionDeOferta>(resultado.IdEntidad);

            return peticionEntidad;
        }

        private int ObtenerCompradorCondicionesEspeciales(Solp solp)
        {
            int usuarioCreadorPOId = solp.UsuarioCreacion.Id;
            if (solp.UsuarioCompras_Id.HasValue)
            {
                int? usuarioId = repositorio.Obtener<Usuario, int?>(a => a.Mail == solp.UsuarioCompras.Mail, a => a.Id);
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
                        var adjudicacionDto = comprasServiceSap.ObtenerOrdenDeCompraAdjudicacion(solp.NroOrdenDeCompraAdicional);
                        if (adjudicacionDto != null && adjudicacionDto.UsuarioCreador_Id != 0)
                            usuarioCreadorPOId = adjudicacionDto.UsuarioCreador_Id;
                    }
                }
            }

            return usuarioCreadorPOId;
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            var result = comprasServiceSap.ObtenerOrdenDeCompra(nroOC);
            Log.Info("ObtenerOrdenDeCompra obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra" + result.ToJson());
            if (result.Error == null || string.IsNullOrEmpty(result.Error.Mensaje))
            {
                try
                {
                    ProveedorComprasDto proveedor = usuarioService.ObtenerYCrearProveedorCompras(result.Cabecera.CodigoProveedor);
                    Log.Info("ObtenerOrdenDeCompra ObtenerProveedorCompras" + proveedor.ToJson());

                    result.Cabecera.RazonSocialProveedor = proveedor.RazonSocial;
                    result.Cabecera.CUITProveedor = proveedor.CUIT;
                    result.Cabecera.CodigoProveedor = proveedor.CodigoProveedor;
                    result.Cabecera.Usuario_Id = proveedor.Usuario_Id;
                    result.Cabecera.MailProveedor = proveedor.Mail;
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
            return result;
        }

        public List<RespuestaCrearOrdenDeCompra> CrearOrdenDeCompraConRegistroInfo(List<RegistroInfoDto> registros, int usuarioActualId)
        {
            if (registros.Count == 0)
                throw new ValidationCustomException("Tiene que seleccionar al menos un registro");
            if (registros.Exists(x => x.CantidadAdjudicacion == 0))
                throw new ValidationCustomException("Todos los registros seleccionados tienen que tener la cantidad ingresada.");


            var resultado = new List<RespuestaCrearOrdenDeCompra>();
            var solpPosicionIds = registros.Select(registro => registro.PosicionId).ToList();
            var posicionesSolp = repositorio.Listar<SolpPosicion>(x => solpPosicionIds.Contains(x.Id));
            foreach (var item in registros.GroupBy(a => new { a.ProveedorId, a.Moneda }))
            {
                RespuestaCrearOrdenDeCompra r = CrearOrdenDeCompraAutomatica(posicionesSolp[0].Solp, false, item.ToList(), true, usuarioActualId);
                r.Proveedor = item.First().NombreProveedor;
                resultado.Add(r);
            }

            return resultado;

        }

        private List<RegistroInfoDto> CrearRegistroInfoDto(Cotizacion cotizacion)
        {
            var registros = new List<RegistroInfoDto>();
            var solpPosiciones = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion).Where(x => x.MaterialSolp != null);
            var unidadesDeMedidaSAP = new List<UnidadesDeMedida>();
            if (solpPosiciones.Any() && solpPosiciones.First().TipoPosicion.Codigo == "MATERIALES")
            {
                unidadesDeMedidaSAP = unidadMedidaService.ObtenerUnidadesDesdeServicioSap(solpPosiciones.Select(x => x.MaterialSolp.Codigo).ToList());
            }
            foreach (var cotizacionPosicion in cotizacion.CotizacionPosiciones.Where(x => x.NoDisponible != true && x.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp != null)) //excluye no catalogados
            {
                var solpPosicion = solpPosiciones.First(p => p.Id == cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id);
                var registro = CrearRegistroInfoDto(cotizacionPosicion, solpPosicion, unidadesDeMedidaSAP, cotizacion);
                registros.Add(registro);
            }

            return registros;
        }

        private RegistroInfoDto CrearRegistroInfoDto(
            CotizacionPosicion cotizacionPosicion,
            SolpPosicion solpPosicion,
            IEnumerable<UnidadesDeMedida> unidadesDeMedidaSap,
            Cotizacion cotizacion = null)
        {
            var organizacionDeCompra = solpPosicion.Solp.OrganizacionDeCompra_Id;

            RegistroInfoDto ultimoRegistroInfoSap = registroInfoService.ObtenerUltimoRegistroPorMaterialYProveedor(
                cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo,
                cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.Centro.Codigo,
                organizacionDeCompra,
                cotizacionPosicion.Cotizacion.UsuarioCreador.ObtenerCodigoProveedor());

            var registro = new RegistroInfoDto
            {
                Cantidad = cotizacionPosicion.Cantidad.Value,
                MaterialCodigo = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo,
                Cuit = cotizacion != null ? cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor()
                        : cotizacionPosicion.Cotizacion.UsuarioCreador.ObtenerCodigoProveedor(),
                Unidad = cotizacionPosicion.UnidadDeMedida.Codigo,
                OrganizacionDeCompra = organizacionDeCompra,
                Centro = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.Centro.Codigo,
                Moneda = cotizacionPosicion.Moneda.Codigo,
                Precio = cotizacionPosicion.Precio.Value,
                FechaVigencia = cotizacionPosicion.FechaDeVigencia.HasValue ? cotizacionPosicion.FechaDeVigencia.Value.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(15).Date.ToString("yyyy-MM-dd"),
                FechaVigenciaFormateada = cotizacionPosicion.FechaDeVigencia ?? DateTime.Now.AddDays(15).Date,
                GrupoDeCompras = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.GrupoCompras.Codigo,
                EsModificar = ultimoRegistroInfoSap.EsModificar
            };

            TablaSap unidadDeMedidaRegInfo = repositorio.Obtener<TablaSap>(a => a.Tabla == "Unidad" && a.CodigoSap == ultimoRegistroInfoSap.Unidad);

            var unidadesDelMaterial = unidadesDeMedidaSap.Where(x => x.CodigoMaterial == solpPosicion.MaterialSolp.Codigo).ToList();
            var unidadCotizada = unidadesDelMaterial.First(x => x.UnidadDeMedida == cotizacionPosicion.UnidadDeMedida.Codigo);
            if (ultimoRegistroInfoSap.EsModificar)
            {
                if (unidadDeMedidaRegInfo.Id != cotizacionPosicion.UnidadDeMedida.Id)
                {
                    var unidadDelRegistroInfo = unidadesDelMaterial.First(x => x.UnidadDeMedida == unidadDeMedidaRegInfo.Codigo);
                    AdjustUnitPriceAndQuantity(registro, cotizacionPosicion.Cantidad.Value, cotizacionPosicion.Precio.Value, unidadCotizada, unidadDelRegistroInfo);
                }
            }
            else
            {

                var unidadBase = unidadesDelMaterial.First(x => x.UnidadDeMedida == solpPosicion.MaterialSolp.UnidadMedidaBase.Codigo);
                AdjustUnitPriceAndQuantity(registro, cotizacionPosicion.Cantidad.Value, cotizacionPosicion.Precio.Value, unidadCotizada, unidadBase);
            }
            return registro;
        }

        private static void AdjustUnitPriceAndQuantity(RegistroInfoDto registro, decimal cantidad, decimal precio, UnidadesDeMedida unidadActual, UnidadesDeMedida unidadObjetivo)
        {
            // Convertir la cantidad a la unidad base (UNI)
            decimal cantidadEnUnidadBase = cantidad * unidadActual.Numerador / unidadActual.Denominador;

            // Convertir la cantidad de la unidad base a la nueva unidad
            decimal nuevaCantidad = cantidadEnUnidadBase * unidadObjetivo.Denominador / unidadObjetivo.Numerador;

            // Como la nueva cantidad sea 1, ajustamos el precio proporcionalmente
            decimal nuevoPrecio = precio * (cantidad / nuevaCantidad);

            // Asignar los valores ajustados al registro
            registro.Unidad = unidadObjetivo.UnidadDeMedida;
            registro.Cantidad = 1;
            registro.Precio = Math.Round(nuevoPrecio, 2);
        }

        void IComprasService.AdjustUnitPriceAndQuantity(RegistroInfoDto registro, decimal cantidad, decimal precio, UnidadesDeMedida unidadActual, UnidadesDeMedida unidadObjetivo)
            => AdjustUnitPriceAndQuantity(registro, cantidad, precio, unidadActual, unidadObjetivo);

        private void EnviarMailResultadoAdjudicacion(List<string> mails, PeticionDeOferta peticion)
        {
            try
            {
                var asunto = "";
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

        public LegajoExternoDto ObtenerLegajoParaExternos(int adjudicacionId, string token, string mailUsuario)
        {
            LegajoExternoDto resultado = new LegajoExternoDto();
            var adjudicacion = repositorio.Obtener<Adjudicacion>(x => x.Id == adjudicacionId && x.Token == token);
            if (adjudicacion != null)
            {
                var cotizacion = adjudicacion.Cotizacion;
                resultado.NroOrdenDeCompra = adjudicacion.NumeroOrdenDeCompra;
                List<string> nrosSolp = adjudicacion.Posiciones.Select(x => x.Posicion.Solp).Select(x => x.NroSolp).ToList();
                resultado.Proveedor = new UsuarioDto(adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario);
                resultado.FechaAdjudicacionFormateado = adjudicacion.FechaCreacion.ToString("dd/MM/yyyy");

                resultado.ListaLegajos = ObtenerLegajo(cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id, null, false, mailUsuario, true).LegajoFilas;

                var adjudicacionesMismaOC = repositorio.Listar<Adjudicacion>(x => x.NumeroOrdenDeCompra == adjudicacion.NumeroOrdenDeCompra && x.Id != adjudicacion.Id);
                foreach (var adjudicacionMismaOC in adjudicacionesMismaOC)
                {
                    resultado.ListaLegajos.AddRange(ObtenerLegajo(adjudicacionMismaOC.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id, null, false, mailUsuario, true).LegajoFilas);
                    nrosSolp.AddRange(adjudicacionMismaOC.Posiciones.Select(x => x.Posicion.Solp).Select(x => x.NroSolp));
                }
                resultado.NroSolp = string.Join(", ", nrosSolp.Distinct());
            }
            return resultado;
        }

        private void ValidarSolpAnulada(string nroSolp)
        {
            var hayPeticionPosicion = repositorio.Existe<PeticionDeOfertaSolpPosicion>(posi => posi.SolpPosicion.Solp.NroSolp.Contains(nroSolp));
            var hayAdjudicacionPosicion = repositorio.Existe<AdjudicacionPosicion>(posi => posi.Posicion.Solp.NroSolp.Contains(nroSolp));

            var solp = repositorio.Obtener<Solp>(x => x.NroSolp == nroSolp && x.TrabajoYaHecho != true
            && hayPeticionPosicion && !hayAdjudicacionPosicion && x.SeEnvioMailAnulacion != true);
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
                var peticiones = repositorio.Listar<PeticionDeOferta>(peti =>
                peti.Posiciones.Select(posi => posi.SolpPosicion.Solp.NroSolp).Contains(solp.NroSolp));

                var peticionesDeOfertaUsuario = repositorio.Listar<PeticionDeOfertaUsuario>(x => peticiones.Select(peti => peti.Id).Contains(x.PeticionDeOferta_Id));
                foreach (var peticion in peticiones)
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

        public ChatsDto ObtenerChat(int solpId, int usuarioActualId)
        {
            ChatComprasDto chat = new ChatComprasDto();
            ChatsDto chats = new ChatsDto();

            var solp = repositorio.Obtener<Solp>(solpId);

            chat.Solp_Id = solp.Id;
            chat.FechaCreacion = solp.FechaCreacion.ToString("dd-MM-yyyy HH-mm-ss");
            chat.FechaCreacionDate = solp.FechaCreacion;
            chat.UsuarioActualId = usuarioActualId;
            chat.RazonSocialComprador = solp.UsuarioCreacion.ObtenerRazonSocial();

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

            chats.ChatCompras = chat;

            var usuario = repositorio.Obtener<Usuario>(usuarioActualId);

            if (usuario.Roles.Any(r => r.Codigo == "SOLP"))
            {
                List<ChatProveedoresDto> chatProveedores = new List<ChatProveedoresDto>();

                foreach (var item in solp.Posiciones.SelectMany(po => po.Peticiones))
                {
                    if (!chatProveedores.Any(cp => cp.PeticionDeOferta_Id == item.PeticionDeOferta_Id))
                    {
                        foreach (var poUsuario in item.PeticionDeOferta.Usuarios)
                        {
                            ChatProveedoresDto chatProveedor = new ChatProveedoresDto();
                            chatProveedor.PeticionDeOferta_Id = item.PeticionDeOferta_Id;
                            chatProveedor.FechaCreacion = item.PeticionDeOferta.FechaCreacion.ToString("dd-MM-yyyy HH-mm-ss");
                            chatProveedor.FechaCreacionDate = item.PeticionDeOferta.FechaCreacion;
                            chatProveedor.UsuarioActualId = usuarioActualId;
                            chatProveedor.RazonSocialProveedor = poUsuario.Usuario.ObtenerRazonSocial();
                            chatProveedor.CuitProveedor = poUsuario.Usuario.CUITRegistro;
                            chatProveedor.PeticionDeOfertaUsuario_Id = poUsuario.Id;

                            chatProveedor.Mensajes = poUsuario.ChatExterno.Select(m => new ChatExternoComprasDto
                            {
                                Id = m.Id,
                                FechaEnvio = m.FechaEnvio.ToString("dd-MM-yyyy HH-mm-ss"),
                                FechaEnvioDate = m.FechaEnvio,
                                FechaDiaEnvio = m.FechaEnvio.ToString("ddd, d MMM"),
                                Leido = m.Leido,
                                Mail = m.Usuario.Mail,
                                Mensaje = m.Mensaje,
                                PeticionDeOferta_Id = m.PeticionDeOferta_Id,
                                RolUsuario = m.Usuario.Roles.Any(r => r.Codigo == "SOLP") ? "SOLP" : "PROVEEDOR",
                                Usuario_Id = m.Usuario_Id,
                                PeticionDeOfertaUsuario_Id = m.PeticionDeOfertaUsuario_Id
                            }).ToList();

                            chatProveedores.Add(chatProveedor);
                        }
                    }
                }
                repositorio.GuardarCambios();
                chats.ChatCompras = chat;
                chats.ChatProveedores = chatProveedores;
            }
            return chats;
        }

        public void MarcarChatProveedorComoLeido(ChatProveedoresDto proveedor)
        {
            var chatProveedor = repositorio.Listar<ChatExternoCompras>(x => x.PeticionDeOfertaUsuario_Id == proveedor.PeticionDeOfertaUsuario_Id && x.PeticionDeOferta_Id == proveedor.PeticionDeOferta_Id);

            if (chatProveedor != null)
            {
                chatProveedor.ForEach(m => m.Leido = true);
                repositorio.GuardarCambios();
            }
        }

        public ChatsDto ObtenerChatProveedor(int peticionDeOfertaUsuarioId, int usuarioActualId)
        {
            ChatsDto chats = new ChatsDto();
            ChatComprasDto chat = new ChatComprasDto();
            List<ChatProveedoresDto> chatProveedores = new List<ChatProveedoresDto>();

            ChatProveedoresDto chatProveedor = new ChatProveedoresDto();
            var peticiondeOfertaUsuario = repositorio.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId);

            foreach (var item in peticiondeOfertaUsuario.ChatExterno.Where(l => l.Usuario_Id != usuarioActualId))
            {
                item.Leido = true;
            }
            repositorio.GuardarCambios();

            chatProveedor.PeticionDeOferta_Id = peticiondeOfertaUsuario.PeticionDeOferta.Id;
            chatProveedor.FechaCreacion = peticiondeOfertaUsuario.PeticionDeOferta.FechaCreacion.ToString("dd-MM-yyyy HH-mm-ss");
            chatProveedor.FechaCreacionDate = peticiondeOfertaUsuario.PeticionDeOferta.FechaCreacion;
            chatProveedor.UsuarioActualId = usuarioActualId;
            chatProveedor.RazonSocialProveedor = peticiondeOfertaUsuario.Usuario.ObtenerRazonSocial();
            chatProveedor.CuitProveedor = peticiondeOfertaUsuario.Usuario.CUITRegistro;
            chatProveedor.PeticionDeOfertaUsuario_Id = peticiondeOfertaUsuario.Id;


            var mensajes = peticiondeOfertaUsuario.ChatExterno.Select(m => new ChatExternoComprasDto
            {
                Id = m.Id,
                FechaEnvio = m.FechaEnvio.ToString("dd-MM-yyyy HH-mm-ss"),
                FechaEnvioDate = m.FechaEnvio,
                FechaDiaEnvio = m.FechaEnvio.ToString("ddd, d MMM"),
                Leido = m.Leido,
                Mail = m.Usuario.Mail,
                Mensaje = m.Mensaje,
                PeticionDeOferta_Id = m.PeticionDeOferta_Id,
                RolUsuario = m.Usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "COMPRADOR" : "PROVEEDOR",
                Usuario_Id = m.Usuario_Id,
                PeticionDeOfertaUsuario_Id = m.PeticionDeOfertaUsuario_Id
            }).ToList();

            chats.ChatCompras = chat;
            chatProveedor.Mensajes = mensajes;
            chatProveedores.Add(chatProveedor);
            chats.ChatProveedores = chatProveedores;
            repositorio.GuardarCambios();

            return chats;
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

        public Resultado GrabarMensajeChatExterno(ChatExternoComprasDto mensaje)
        {
            try
            {
                var resultado = new Resultado();
                var chatExternoCompras = new ChatExternoCompras()
                {
                    FechaEnvio = DateTime.Now,
                    Leido = false,
                    Mensaje = mensaje.Mensaje,
                    PeticionDeOferta_Id = mensaje.PeticionDeOferta_Id,
                    Usuario_Id = mensaje.Usuario_Id,
                    PeticionDeOfertaUsuario_Id = mensaje.PeticionDeOfertaUsuario_Id
                };

                repositorio.Agregar(chatExternoCompras);
                repositorio.GuardarCambios();

                resultado.IdEntidad = chatExternoCompras.Id;
                return resultado;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string ExportarChatInternoAtexto(int solpId, string rutaArchivo, int? peticionDeOfertaUsuarioId)
        {
            try
            {
                var txtFilename = "";
                var txtFilePath = "";

                if (solpId == 0 && peticionDeOfertaUsuarioId > 0)
                {
                    var peticion = repositorio.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId);
                    txtFilename = $"Chat-PO-{peticion.PeticionDeOferta_Id}-CUIT-{peticion.PeticionDeOferta.Usuario.CUITRegistro}-{DateTime.Now.ToString("yyyyMMdd")}.txt";
                    txtFilePath = $"{rutaArchivo}/{txtFilename}";

                    using (StreamWriter sw = new StreamWriter(txtFilePath))
                    {
                        var solpNros = string.Join(", ", peticion.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp.NroSolp).Distinct().ToList());
                        // Escribir información general del chat
                        sw.WriteLine($"SOLP : {solpNros}");
                        sw.WriteLine($"PO : {peticion.PeticionDeOferta_Id}");
                        sw.WriteLine($"Fecha creacion: {peticion.PeticionDeOferta.FechaCreacion}");

                        // Escribir mensajes del chat
                        sw.WriteLine();
                        sw.WriteLine("Mensajes:");
                        foreach (var mensaje in peticion.ChatExterno)
                        {
                            sw.WriteLine($"{mensaje.FechaEnvio.ToString("dd-MM-yyyy HH:mm")} - {mensaje.Usuario.Mail} - {mensaje.Mensaje}");
                        }
                    }
                }
                else
                {
                    var solp = repositorio.Obtener<Solp>(solpId);

                    txtFilename = $"Chat-SOLP-{solp.NroSolp}-{DateTime.Now.ToString("yyyyMMdd")}.txt";
                    txtFilePath = $"{rutaArchivo}/{txtFilename}";

                    if (!peticionDeOfertaUsuarioId.HasValue)
                    {
                        using (StreamWriter sw = new StreamWriter(txtFilePath))
                        {
                            // Escribir información general del chat
                            sw.WriteLine($"SOLP : {solp.Id}");
                            sw.WriteLine($"Fecha creacion: {solp.FechaCreacion}");

                            // Escribir mensajes del chat
                            sw.WriteLine();
                            sw.WriteLine("Mensajes:");
                            foreach (var mensaje in solp.ChatInternoCompras)
                            {
                                sw.WriteLine($"{mensaje.FechaEnvio.ToString("dd-MM-yyyy HH:mm")} - {mensaje.Usuario.Mail} - {mensaje.Mensaje}");
                            }
                        }
                    }
                    else
                    {
                        var peticion = repositorio.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId);
                        txtFilename = $"Chat-SOLP-{solp.NroSolp}-CUIT-{peticion.PeticionDeOferta.Usuario.CUITRegistro}-{DateTime.Now.ToString("yyyyMMdd")}.txt";
                        txtFilePath = $"{rutaArchivo}/{txtFilename}";

                        using (StreamWriter sw = new StreamWriter(txtFilePath))
                        {
                            // Escribir información general del chat
                            sw.WriteLine($"SOLP : {solp.Id}");
                            sw.WriteLine($"PO : {peticion.PeticionDeOferta_Id}");
                            sw.WriteLine($"Fecha creacion: {solp.FechaCreacion}");

                            // Escribir mensajes del chat
                            sw.WriteLine();
                            sw.WriteLine("Mensajes:");
                            foreach (var mensaje in peticion.ChatExterno)
                            {
                                sw.WriteLine($"{mensaje.FechaEnvio.ToString("dd-MM-yyyy HH:mm")} - {mensaje.Usuario.Mail} - {mensaje.Mensaje}");
                            }
                        }
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
            var proveedorMoa = usuarioService.ObtenerProveedorSap(codigoProveedor);
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
            try
            {
                return !comprasServiceSap.ObtenerPosicionesPendientesAdjudicar(nroSolp).Any();
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

        private void ActualizarDatosSolp(List<string> nroOrdenDeCompra, PeticionDeOferta peticionDeOferta)
        {
            try
            {
                IEnumerable<Solp> solps =
                    peticionDeOferta.Posiciones
                    .Select(x => x.SolpPosicion.Solp)
                    .DistinctBy(x => x.Id);
                foreach (Solp solp in solps)
                {
                    RespuestaGuardarSOLP respuestaGuardarSOLP = new RespuestaGuardarSOLP { Solp = new SolpDto { NroSolp = solp.NroSolp } };
                    List<Usuario> proveedores = repositorio.Listar<Usuario>();
                    if (nroOrdenDeCompra.Count > 0)
                    {
                        foreach (string nro in nroOrdenDeCompra)
                        {
                            OrdenDeCompraSAPDto ordenDeCompra = ObtenerOrdenDeCompra(nro);
                            Log.Info("ActualizarDatosSolp ObtenerOrdenDeCompra" + ordenDeCompra.ToJson());
                            ProveedorComprasDto proveedor = usuarioService.ObtenerYCrearProveedorCompras(ordenDeCompra.Cabecera.CodigoProveedor);
                            foreach (OrdenDeCompraSAPPosicion posicionOCSap in ordenDeCompra.Posiciones.Where(x => x.NroSolp == solp.NroSolp))
                            {
                                SolpPosicion posicionSolp = solp.Posiciones.FirstOrDefault(x => x.Indice == int.Parse(posicionOCSap.IndiceSolp));
                                posicionSolp.ProveedorAdjudicado_Id = proveedor.Usuario_Id;
                                posicionSolp.ProveedorAdjudicado = proveedores.FirstOrDefault(x => x.Id == proveedor.Usuario_Id);
                                posicionSolp.RegistroInfoNro = posicionOCSap.RegistroInfo;
                                posicionSolp.OrganizacionDeComprasCodigo = ordenDeCompra.Cabecera.OrganizacionDeComprasCodigo;
                            }
                        }

                        FinalizarSolp(solp, respuestaGuardarSOLP, false);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(new Exception("ActualizarDatosSolp"));
                Log.Error(e);
            }
        }

        private void SetNombreDePedido(Solp solp)
        {
            if (solp is null) { throw new ArgumentNullException(nameof(solp)); }

#pragma warning disable S2589 // Boolean expressions should not be gratuitous <-- se usa esto para que el código sea más legible
            bool mustSetNombreDePedido = false;
            // no usar |= ya que ese operador no respeta el cortocircuito
            mustSetNombreDePedido = mustSetNombreDePedido || solp.Pliego is null;
            mustSetNombreDePedido = mustSetNombreDePedido || string.IsNullOrWhiteSpace(solp.Pliego.NombreObra);
            mustSetNombreDePedido = mustSetNombreDePedido || solp.TipoSolpSap is null;
            mustSetNombreDePedido = mustSetNombreDePedido || solp.TipoSolpSap != (int?)TipoSolpSap.Web;
            mustSetNombreDePedido = mustSetNombreDePedido || solp.TipoSolp is null;
            mustSetNombreDePedido = mustSetNombreDePedido || solp.TipoSolp.Codigo == "SIN_PLIEGO";

            bool canSetNombreDePedido = true;
            canSetNombreDePedido = canSetNombreDePedido && solp.Posiciones != null;
            canSetNombreDePedido = canSetNombreDePedido && (solp.Posiciones.Count > 0);
#pragma warning restore S2589 // Boolean expressions should not be gratuitous

            if (mustSetNombreDePedido && canSetNombreDePedido)
            {
                string nombre = solp.Posiciones.Count > 2 ? string.Join(" + ", solp.Posiciones.Take(2).Select(x => x.Tarea)) + " + Otros" :
                                string.Join(" + ", solp.Posiciones.Select(x => x.Tarea));
                solp.Pliego.NombreObra = nombre;
            }
        }

        public bool TieneCondicionEspecial(Solp solp)
        {
            return solp.TrabajoYaHecho == true || solp.ConPresupuesto || solp.Adicional == true || solp.Urgencia == true || solp.CondEspProveedorAsignado == true;
        }

        public bool TieneCondicionEspecial(SolpDto solp)
        {
            return solp.TrabajoYaHecho == true || solp.ConPresupuesto || solp.Adicional == true || solp.Urgencia == true || solp.CondEspProveedorAsignado == true;
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
                    .OrderBy(x => x.TipoSolp)
                    .ToList());

                startDate = startDate.AddMonths(1);
            }
            MemoryStream streamExcel = ExcelExport.CreateExcelFileMs(resultadoFinal, new string[] { "Origen", "Usuario", "Cantidad", "Periodo" });

            var nombreArchivoXls = $"Reporte SOLPs {DateTime.Today:dd-MM-yyyy}.xlsx";

            EnviarMailReporteSolp(streamExcel.ToArray(), nombreArchivoXls);
        }

        public void EnviarReporteTrabajoYaHecho()
        {
            var fechaDesde = DateTime.Today.AddDays(-7);
            var ordenesDeCompraUltimaSemana = comprasServiceSap.ObtenerOrdenesDeCompra(fechaDesde);

            var nrosOcs = new HashSet<string>(ordenesDeCompraUltimaSemana.Select(x => x.Id.ToString()));
            var nrosSolps = new HashSet<string>();
            var detallesOCs = new List<OrdenDeCompraSAPDto>();

            foreach (var nroOc in nrosOcs)
            {
                var ordenDeCompra = comprasServiceSap.ObtenerOrdenDeCompra(nroOc);
                if (ordenDeCompra.Posiciones?.Any() ?? false)
                {
                    ordenDeCompra.Posiciones?.ForEach(p => nrosSolps.Add(p.NroSolp));
                    detallesOCs.Add(ordenDeCompra);
                }
            }

            var solpsTrabajosHechos = repositorio.ObtenerSolpsReporteTrabajoYaHecho(nrosSolps);

            if (!ordenesDeCompraUltimaSemana.Any() || !solpsTrabajosHechos.Any())
            {
                Log.Info($"No se encontraron registros para reportar Trabajo ya hecho. OCs: {ordenesDeCompraUltimaSemana.Count}. SOLPs: {solpsTrabajosHechos.Count}");
                return;
            }

            var fechasLiberacionPorOc = repositorio.ObtenerFechasLiberacionOcs(nrosOcs);

            var trabajosHechosAReportar = new List<TrabajoYaHechoReporte>();

            foreach (var detalleOc in detallesOCs)
            {
                foreach (var nroSolp in detalleOc.Posiciones?.Select(x => x.NroSolp).Distinct())
                {
                    var solpTh = solpsTrabajosHechos.FirstOrDefault(s => s.SolpNro == nroSolp);
                    if (solpTh != null)
                    {
                        trabajosHechosAReportar.Add(new TrabajoYaHechoReporte
                        {
                            SolpNro = solpTh.SolpNro,
                            SolpCreador = solpTh.SolpCreador,
                            SolpFecha = solpTh.SolpFecha,
                            OrdenCompraNro = detalleOc.Cabecera.OrdenDeCompra,
                            OrdenCompraCreador = detalleOc.Cabecera.UsuarioComprasSAP,
                            OrdenCompraFecha = detalleOc.Cabecera.FechaCreacion.ToString("dd/MM/yyyy"),
                            OrdenCompraFechaLiberacion = fechasLiberacionPorOc.TryGetValue(detalleOc.Cabecera.OrdenDeCompra, out DateTime fechaLiberacionOc) ? fechaLiberacionOc.ToString("dd/MM/yyyy") : null
                        });
                    }
                }
            }

            Log.Info("Trabajos hechos a reportar: " + trabajosHechosAReportar.Count);
            var excelMemStream = ExcelExport.CreateExcelFileMs(trabajosHechosAReportar, new string[] { "Nro solp", "Creador solp", "Fecha solp", "Liberación OC", "Nro OC", "Creador OC", "Fecha OC" });
            var nombreArchivoXls = $"Reporte OCs trabajos ya hechos {DateTime.Today:yyyy-MM-dd}.xlsx";

            emailComprasService.EnviarMailReporteTrabajoYaHecho(excelMemStream.ToArray(), nombreArchivoXls);
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

        public List<int> ListarClaseDocumento(int usuarioId)
        {
            return repositorio.Listar<Solp>(x => x.UsuarioCreacion_Id == usuarioId && x.ClaseDocumento_Id.HasValue)
                .Select(x => x.ClaseDocumento_Id.Value).Distinct().ToList();
        }

        public Resultado ActualizarProveedorVisibleEnSolicitante(int peticionDeOfertaUsuarioId, bool esVisible)
        {
            try
            {
                Resultado resultado = new Resultado();
                var peticionUsuario = repositorio.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId);
                peticionUsuario.VisibleSolicitante = esVisible;
                repositorio.GuardarCambios();
                resultado.Mensaje = "OK ";
                resultado.IdEntidad = peticionDeOfertaUsuarioId;
                return resultado;
            }
            catch (Exception e)
            {
                Log.Info($"Error al ActualizarProveedorVisibleEnSolicitante");
                Log.Error(e);
                throw;
            }
        }
        public CotizacionHistorial GrabarLogCotizacion(Cotizacion cotizacion)
        {
            var cotizacionHistorial = new CotizacionHistorial()
            {
                Cotizacion_Id = cotizacion.Id,
                FechaFinalizacion = DateTime.Now,
                Log = new LogCotizacionDto()
                {
                    Id = cotizacion.Id,
                    UsuarioCreador_Id = cotizacion.UsuarioCreador_Id,
                    CotizacionEstadoDescripcion = cotizacion.CotizacionEstado != null ? cotizacion.CotizacionEstado.Descripcion : "",
                    PeticionDeOfertaUsuario_Id = cotizacion.PeticionDeOfertaUsuario_Id,
                    RespetaMateriales = cotizacion.RespetaMateriales == true ? "Si" : "No",
                    RespetaServicios = cotizacion.RespetaServicios == true ? "Si" : "No",
                    ObservacionTecnica = cotizacion.ObservacionTecnica,
                    ObservacionEconomica = cotizacion.ObservacionEconomica,
                    Revision = cotizacion.Revision,
                    PorcentajeDeHoras = cotizacion.PorcentajeDeHoras,
                    FechaCreacion = cotizacion.FechaCreacion != null ? cotizacion.FechaCreacion.ToString("dd-MM-yyyy") : "",
                    Archivos = cotizacion.Archivos.Select(x => new ArchivoDto { Id = x.Id, FileKey = x.FileKey, Ruta = x.ObtenerNombre(x.Ruta) }).ToList(),
                    CotizacionPosiciones = cotizacion.CotizacionPosiciones.Select(cotPos => new LogCotizacionPosicionDto
                    {
                        Id = cotPos.Id,
                        Cotizacion_Id = cotPos.Cotizacion_Id,
                        PeticionDeOfertaSolpPosicion_Id = cotPos.PeticionDeOfertaSolpPosicion_Id,
                        Cantidad = cotPos.Cantidad != null ? cotPos.Cantidad : 0,
                        Precio = cotPos.Precio != null ? cotPos.Precio : 0,
                        FechaDeEntrega = cotPos.FechaDeEntrega != null ? cotPos.FechaDeEntrega.Value.ToString("dd-MM-yyyy") : "",
                        MonedaCodigo = cotPos.Moneda != null ? cotPos.Moneda.Codigo : "",
                        UnidadMedidaDescripcion = cotPos.UnidadDeMedida != null ? cotPos.UnidadDeMedida.Descripcion : "",
                        PrecioTotal = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.TipoPosicion.Codigo == "MATERIALES" ? (cotPos.Cantidad.HasValue && cotPos.Precio.HasValue ? cotPos.Cantidad.Value * cotPos.Precio.Value : 0) : cotPos.CotizacionSubPosiciones?.Sum(x => x.Precio * x.Cantidad),
                        NoDisponible = cotPos.NoDisponible == true ? "No disponible" : "Disponible",
                        FechaDeVigencia = cotPos.FechaDeVigencia != null ? cotPos.FechaDeVigencia.Value.ToString("dd-MM-yyyy") : "",
                        PrimerPlazoDeOferta = cotPos.PrimerPlazoDeOferta != null ? cotPos.PrimerPlazoDeOferta : 0,
                        PrimeraCantidad = cotPos.PrimeraCantidad != null ? cotPos.PrimeraCantidad : 0,
                        SegundoPlazoDeOferta = cotPos.SegundoPlazoDeOferta != null ? cotPos.SegundoPlazoDeOferta : 0,
                        SegundaCantidad = cotPos.SegundaCantidad != null ? cotPos.SegundaCantidad : 0,
                        TercerPlazoDeOferta = cotPos.TercerPlazoDeOferta != null ? cotPos.TercerPlazoDeOferta : 0,
                        TerceraCantidad = cotPos.TerceraCantidad != null ? cotPos.TerceraCantidad : 0,
                        EstaEliminado = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.Estado ? "" : "Esta eliminado",
                        Indice = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.Indice,
                        IdPosicion = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.Id,
                        Descripcion = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.Tarea != null ? cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.Tarea : "",
                        Codigo = cotPos.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp?.Id,
                        CotizacionSubPosiciones = cotPos.CotizacionSubPosiciones?.Select(cotSubPos => new LogCotizacionSubPosicionDto
                        {
                            CotizacionSubPosicionId = cotSubPos.Id,
                            SolpSubPosicionId = cotSubPos.SolpSubPosicion_Id,
                            Cantidad = cotSubPos.Cantidad != null ? cotSubPos.Cantidad : 0,
                            UnidadDeMedidaDescripcion = cotSubPos.UnidadDeMedida != null ? cotSubPos.UnidadDeMedida.Descripcion : "",
                            Precio = cotSubPos.Precio != null ? cotSubPos.Precio : 0,
                            MonedaCodigo = cotSubPos.Moneda != null ? cotSubPos.Moneda.Codigo : "",
                            PrecioTotal = cotSubPos.Cantidad.HasValue && cotSubPos.Precio.HasValue ? cotSubPos.Cantidad.Value * cotSubPos.Precio.Value : 0,
                            NroSubPosicion = cotSubPos.SolpSubPosicion.Numero,
                            IdSubPosicion = cotSubPos.SolpSubPosicion_Id,
                            Descripcion = cotSubPos.SolpSubPosicion.Tarea,
                            Codigo = cotSubPos.SolpSubPosicion.ServicioSolp != null ? cotSubPos.SolpSubPosicion.ServicioSolp?.Id : 0
                        }).ToList(),

                    }).ToList(),
                    CotizacionesHoras = cotizacion.CotizacionesHoras?.Select(cotHs => new LogCotizacionHorasDto
                    {
                        Id = cotHs.Id,
                        Cotizacion_Id = cotHs.Cotizacion_Id,
                        Categoria = cotHs.Categoria,
                        CantidadPersonas = cotHs.CantidadPersonas,
                        HorasNormales = cotHs.HorasNormales,
                        HorasNocturnas = cotHs.HorasNocturnas,
                        Gremio = cotHs.Gremio,
                    }).ToList(),
                }.ToJson(),
                Usuario_Id = cotizacion.UsuarioCreador_Id,
            };

            repositorio.Agregar(cotizacionHistorial);
            repositorio.GuardarCambios();
            return cotizacionHistorial;
        }

        public List<CotizacionHistorialDto> ObtenerHistorial(int id)
        {
            var historialEntities = repositorio.Listar<CotizacionHistorial>(x => x.Cotizacion_Id == id);

            var historial = historialEntities.Select(x => new CotizacionHistorialDto
            {
                Id = x.Id,
                Cotizacion_Id = x.Cotizacion_Id,
                FechaFinalizacion = x.FechaFinalizacion.ToString("dd-MM-yyyy HH:mm:ss") + "hs",
                Log = x.Log,
                Usuario_Id = x.Usuario_Id,
                UsuarioRazonSocial = x.Usuario.ObtenerRazonSocial()
            }).ToList();

            foreach (var item in historial)
            {
                item.Cotizacion = JsonConvert.DeserializeObject<LogCotizacionDto>(item.Log);
            }

            return historial;
        }

        private static Expression<Func<SolpPosicion, bool>> ListarPosicionesPOMultipleCommonFilter
            (List<string> solps,
             DateTime? desde,
             DateTime? hasta,
             bool sap,
             bool mantenimiento,
             bool web,
             bool repoAutomatica,
             bool? tratada,
             List<int> centros = null,
             List<int> grupoDeCompras = null,
             List<int> claseDocumento = null,
             List<string> tipoImputacion = null,
             List<int> valorTipoImputacion = null)
        {
            Expression<Func<SolpPosicion, bool>> ListarPosicionesPOMultipleCommonFilter =
                pos =>
                    solps.Contains(pos.Solp.NroSolp)
                    &&
                        (
                            (!(sap || mantenimiento || web || repoAutomatica))
                            || (sap && pos.Solp.TipoSolpSap == (int)TipoSolpSap.Sap)
                            || (mantenimiento && pos.Solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento)
                            || (repoAutomatica && pos.Solp.TipoSolpSap == (int)TipoSolpSap.ReposicionAutomatica)
                            || (web && (pos.Solp.TipoSolpSap == null || pos.Solp.TipoSolpSap == (int)TipoSolpSap.Web))
                        )
                    &&
                        (
                            desde == null || pos.Solp.FechaCreacion >= desde.Value
                        )
                    &&
                        (
                            hasta == null || pos.Solp.FechaCreacion <= hasta.Value
                        )
                    &&
                        (
                            !centros.Any() || pos.Solp.Posiciones.Any(c => centros.Contains(c.Centro_Id))
                        )
                    &&
                        (
                            (
                                pos.TipoPosicion.Codigo == "MATERIALES"
                                    && (!grupoDeCompras.Any() || grupoDeCompras.Contains((int)pos.GrupoCompras_Id))
                                    && (!tipoImputacion.Any() || tipoImputacion.Contains(pos.TipoImputacion.Codigo))
                            )
                            ||
                            (
                                pos.TipoPosicion.Codigo == "SERVICIO"
                                    && (!grupoDeCompras.Any() || pos.Solp.Posiciones.Any(gc => grupoDeCompras.Contains((int)gc.GrupoCompras_Id)))
                                    && (!tipoImputacion.Any() || pos.Solp.Posiciones.Any(c => tipoImputacion.Contains(c.TipoImputacion.Codigo)))
                            )
                        )
                    &&
                        (!claseDocumento.Any() || pos.Solp.EstadoSolpSap_Id != null && claseDocumento.Contains((int)pos.Solp.ClaseDocumento_Id)) &&
                        (string.IsNullOrEmpty(pos.NumeroContratoSuperior))
                    &&
                        (
                            !valorTipoImputacion.Any()
                            || pos.Solp.Posiciones.Any(p => valorTipoImputacion.Contains((int)pos.ValorTipoImputacion_Id))
                            || pos.Solp.Posiciones.Any(p => p.Subposiciones.Any(sp => valorTipoImputacion.Contains((int)sp.TipoImputacion_Id)))
                        )
                    &&
                        (tratada == null || pos.Peticiones.Any() == tratada) &&
                            pos.Solp.TrabajoYaHecho != true &&
                            !pos.Solp.ConPresupuesto &&
                            pos.Solp.Adicional != true &&
                            pos.Solp.CondEspProveedorAsignado != true &&
                            (pos.Solp.EstadoSolpSap.CodigoSap == "05" ||
                            pos.Solp.EstadoSolpSap.CodigoSap == "02")
                     && (pos.Solp.OrganizacionDeCompra_Id == OrganizacionDeCompraEnum.Estrategica);

            return ListarPosicionesPOMultipleCommonFilter;
        }

        public List<POPosicionDto> ListarPosicionesPOMultiple(DateTime? desde,
                                                              DateTime? hasta,
                                                              bool sap,
                                                              bool mantenimiento,
                                                              bool web,
                                                              bool repoAutomatica,
                                                              bool? tratada,
                                                              bool contratoMarco,
                                                              List<int> centros = null,
                                                              List<int> grupoDeCompras = null,
                                                              List<int> claseDocumento = null,
                                                              List<string> tipoImputacion = null,
                                                              List<int> valorTipoImputacion = null,
                                                              int? numeroPo = null)
        {
            try
            {
                DateTime? fechaHasta = hasta != null ? hasta.Value.AddDays(1) : (DateTime?)null;

                var posicionPendientesSap = new List<PosicionPendienteDto>();

                posicionPendientesSap.AddRange(comprasServiceSap.ListarSolpPendientes());
                List<string> solps = posicionPendientesSap.Select(a => a.NroSolp).Distinct().ToList();
                var sinSolps = !solps.Any();

                if (sinSolps)
                {
                    return new List<POPosicionDto>();
                }

                Expression<Func<SolpPosicion, bool>> commonFilter = ListarPosicionesPOMultipleCommonFilter(solps, desde, fechaHasta, sap, mantenimiento, web, repoAutomatica, tratada, centros, grupoDeCompras, claseDocumento, tipoImputacion, valorTipoImputacion);
                Expression<Func<SolpPosicion, bool>> filtroMaterial = pos => pos.TipoPosicion.Codigo == "MATERIALES";
                List<Expression<Func<SolpPosicion, bool>>> filtros = new List<Expression<Func<SolpPosicion, bool>>>()
                {
                    filtroMaterial,
                    commonFilter,
                };

                var posicionMaterial = repositorio.ListarIntersecar<SolpPosicion, POPosicionDto>(pos => new POPosicionDto
                {
                    Id = pos.Id,
                    NroSolp = pos.Solp.NroSolp,
                    Indice = pos.Indice,
                    Codigo = pos.MaterialSolp.Codigo,
                    Tarea = pos.Tarea,
                    CentroComprasDescripcion = pos.Centro.Descripcion,
                    AlmacenComprasDescripcion = pos.Almacen.Descripcion,
                    TextoSuministro = pos.TextoSuministro,
                    Modelo = pos.Modelo,
                    GrupoComprasDescripcion = pos.GrupoCompras.Descripcion,
                    Cantidad = pos.Cantidad,
                    UnidadComprasDescripcion = pos.Unidad.Descripcion,
                    MonedaSolpDescripcion = pos.Moneda.Descripcion,
                    FechaEntregaServicio = pos.FechaEntregaServicio,
                    PlazoEntrega = pos.PlazoEntrega,
                    FechaOferta = pos.Solp.Pliego_Id != null ? pos.Solp.Pliego.FechaHoraEntrega : null,
                    TieneCotizacion = pos.Peticiones.Any(),
                    SeraUsadoEnPliegoMultiple = pos.Solp.SeraUsadoEnPliegoMultiple,
                },
                    filtros
                );

                posicionMaterial = posicionMaterial.Where(pm => posicionPendientesSap
                        .Exists(pp => pp.NroSolp == pm.NroSolp && pp.NumeroPosicion == pm.Indice)).ToList();

                foreach (var posicion in posicionMaterial)
                {
                    if (posicion.TieneCotizacion)
                    {
                        posicion.ListaPO = repositorio.Obtener<SolpPosicion, IEnumerable<string>>(
                            po => po.Id == posicion.Id,
                            po => po.Peticiones.Select(p => p.PeticionDeOferta_Id.ToString())).ToList();
                    }
                }

                if (numeroPo != null)
                {
                    return posicionMaterial
                        .Where(posicion => posicion.ListaPO?.Any(x => x == numeroPo.ToString()) == true)
                        .ToList();
                }

                return posicionMaterial;
            }
            catch (Exception e)
            {
                Log.Error("Error al ListarPosicionesPOMultiple", e);
                throw;
            }
        }

        public List<SolpCrearPoMultipleDto> ListarPosicionesPOMultipleServicio(DateTime? desde,
                                                                               DateTime? hasta,
                                                                               bool sap,
                                                                               bool mantenimiento,
                                                                               bool web,
                                                                               bool repoAutomatica,
                                                                               bool? tratada,
                                                                               bool contratoMarco,
                                                                               List<int> centros = null,
                                                                               List<int> grupoDeCompras = null,
                                                                               List<int> claseDocumento = null,
                                                                               List<string> tipoImputacion = null,
                                                                               List<int> valorTipoImputacion = null,
                                                                               int? numeroPo = null,
                                                                               string nombrePliego = null,
                                                                               TipoPliego tipoPliego = TipoPliego.All)
        {
            try
            {
                DateTime? fechaHasta = hasta != null ? hasta.Value.AddDays(1) : (DateTime?)null;

                var posicionPendientesSap = comprasServiceSap.ListarSolpPendientes(); //new List<PosicionPendienteDto>();

                //posicionPendientesSap.AddRange(comprasServiceSap.ListarSolpPendientes());
                var nrosSolpsPendientes = posicionPendientesSap.Select(a => a.NroSolp).Distinct().ToList();
                var noHaySolpsPendientes = !nrosSolpsPendientes.Any();

                if (noHaySolpsPendientes)
                {
                    return new List<SolpCrearPoMultipleDto>();
                }

                Expression<Func<SolpPosicion, bool>> commonFilter = ListarPosicionesPOMultipleCommonFilter(nrosSolpsPendientes, desde, fechaHasta, sap, mantenimiento, web, repoAutomatica, tratada, centros, grupoDeCompras, claseDocumento, tipoImputacion, valorTipoImputacion);
#pragma warning disable RCS1155 // Use StringComparison when comparing strings -> No se puede usar StringComparison porque linq to entity no lo soporta. Lo mismo con IsNullOrWhitespace.
                Expression<Func<SolpPosicion, bool>> filtroServicio =
                    pos =>
                        pos.TipoPosicion.Codigo == "SERVICIO" &&
                        (string.IsNullOrEmpty(nombrePliego) ||
                            string.IsNullOrEmpty(nombrePliego.Trim()) ||
                            pos.Solp.Pliego.NombreObra.Trim().ToLower().Contains(nombrePliego.Trim().ToLower())
                        ) &&
                        (tipoPliego == TipoPliego.All ||
                            (tipoPliego == TipoPliego.PliegoUnico && !pos.Solp.Pliego.Multiple) ||
                            (tipoPliego == TipoPliego.PliegoMultiple && pos.Solp.Pliego.Multiple));
#pragma warning restore RCS1155 // Use StringComparison when comparing strings
                List<Expression<Func<SolpPosicion, bool>>> filtros = new List<Expression<Func<SolpPosicion, bool>>>()
                {
                    filtroServicio,
                    commonFilter,
                };

                var solpIds = repositorio.ListarIntersecar<SolpPosicion, int>(pos => pos.Solp_Id, filtros).Distinct();

                var solpsPoMultiple = repositorio.Listar<Solp, SolpCrearPoMultipleDto>(
                    solp => new SolpCrearPoMultipleDto
                    {
                        Id = solp.Id,
                        NroSolp = solp.NroSolp,
                        Nombre = solp.Pliego.NombreObra,
                        FechaCreacion = solp.FechaCreacion,
                        FechaLiberacion = solp.FechaLiberacionSap,
                        Solicitante = solp.UsuarioCreacion.Mail,
                        GrupoDeCompras = solp.Posiciones.FirstOrDefault() != null ? solp.Posiciones.FirstOrDefault().GrupoCompras.Descripcion : null,
                        Centro = solp.Posiciones.FirstOrDefault() != null ? solp.Posiciones.FirstOrDefault().Centro.Descripcion : null,
                        Tipo = solp.TipoSolp.Descripcion,
                        MultipleFinalizado = solp.Pliego.MultipleFinalizado,
                        SeraUsadoEnPliegoMultiple = solp.SeraUsadoEnPliegoMultiple,
                        TieneCotizacion = solp.Posiciones.Any(pos => pos.Peticiones.Any())
                    },
                    solp =>
                        solpIds.Contains(solp.Id) &&
                        (solp.Pliego.MultipleFinalizado == true || solp.Pliego.Multiple == false)
                );

                foreach (var solpPoMultiple in solpsPoMultiple)
                {
                    if (solpPoMultiple.TieneCotizacion)
                    {
                        var peticionesIds = repositorio.ListarProyeccion<SolpPosicion, IEnumerable<int>>(
                            pos => pos.Peticiones.Select(pet => pet.PeticionDeOferta_Id),
                            pos => pos.Solp_Id == solpPoMultiple.Id);

                        solpPoMultiple.ListaPO = peticionesIds.SelectMany(x => x).Distinct().ToList().ConvertAll(x => x.ToString());
                    }
                }

                return solpsPoMultiple;
            }
            catch (Exception e)
            {
                Log.Error($"Error al ListarPosicionesPOMultiple", e);
                throw;
            }
        }

        public IEnumerable<PosicionCrearPoMultipleDto> ListarPosicionesPOMultipleSolpId(int idSolp)
        {
            try
            {
                var posicionPendientesSap = new List<PosicionPendienteDto>();
                posicionPendientesSap.AddRange(comprasServiceSap.ListarSolpPendientes());

                List<PosicionCrearPoMultipleDto> posiciones = repositorio.Listar<SolpPosicion, PosicionCrearPoMultipleDto>(
                    posicion => new PosicionCrearPoMultipleDto
                    {
                        Id = posicion.Id,
                        nroSolp = posicion.Solp.NroSolp,
                        NroPosicion = posicion.Indice,
                        Descripcion = posicion.Tarea,
                        TipoImputacion = posicion.TipoImputacion.Descripcion,
                        Centro = posicion.Centro.Descripcion,
                        Almacen = posicion.Almacen.Descripcion,
                        Moneda = posicion.Moneda.Descripcion,
                        ValorTotal = posicion.Subposiciones.Sum(sub => sub.PrecioBruto * sub.Cantidad),
                    }
                    , posicion => posicion.Solp_Id == idSolp);

                return posiciones
                    .Where(posicion =>
                        posicionPendientesSap
                            .Exists(pendiente => pendiente.NroSolp == posicion.nroSolp && pendiente.NumeroPosicion == posicion.NroPosicion));
            }
            catch (Exception e)
            {
                Log.Info($"Error al ListarPosicionesPOMultipleSolpId");
                Log.Error(e);
                throw;
            }
        }

        public IEnumerable<SubPosicionCrearPoMultipleDto> ListarSubPosicionesPOMultipleSolpId(int idPosicion)
        {
            try
            {
                return repositorio.Listar<SolpSubposicion, SubPosicionCrearPoMultipleDto>(
                    subPosicion => new SubPosicionCrearPoMultipleDto
                    {
                        NroSubPosicion = subPosicion.Numero,
                        CodigoServicio = subPosicion.ServicioSolp.Codigo,
                        Tarea = subPosicion.Tarea,
                        Cantidad = subPosicion.Cantidad,
                        UnidadMedida = subPosicion.Unidad.Descripcion,
                        PrecioBruto = subPosicion.PrecioBruto,
                        ValorNeto = subPosicion.PrecioBruto * subPosicion.Cantidad,
                        CuentaMayor = subPosicion.CuentaMayorSap.Descripcion,
                        Imputacion = subPosicion.TipoImputacionSap.Descripcion,
                    }
                    , subPosicion => subPosicion.SolpPosicion_Id == idPosicion);
            }
            catch (Exception e)
            {
                Log.Info($"Error al ListarPosicionesPOMultipleSolpId");
                Log.Error(e);
                throw;
            }
        }

        public MemoryStream DescargarPosicionesPOMultiple(DateTime? desde,
                                                          DateTime? hasta,
                                                          bool sap,
                                                          bool mantenimiento,
                                                          bool web,
                                                          bool repoAutomatica,
                                                          bool? tratada,
                                                          bool contratoMarco,
                                                          List<int> centros = null,
                                                          List<int> grupoDeCompras = null,
                                                          List<int> claseDocumento = null,
                                                          List<string> tipoImputacion = null,
                                                          List<int> valorTipoImputacion = null,
                                                          int? numeroPo = null)
        {
            List<POPosicionDto> data = this.ListarPosicionesPOMultiple(desde,
                                                       hasta,
                                                       sap,
                                                       mantenimiento,
                                                       web,
                                                       repoAutomatica,
                                                       tratada,
                                                       contratoMarco,
                                                       centros,
                                                       grupoDeCompras,
                                                       claseDocumento,
                                                       tipoImputacion,
                                                       valorTipoImputacion,
                                                       numeroPo);

            /* las siguientes 2 líneas no son necesarias si se usa la configuración predeterminada
             * ya que estas mismas llamadas se hacen dentro del método CreateColumnsFromObject
             * cuando no se especifican los parámetros.
             * Sin embargo, como esta invocación seguro va a ser usada como ejemplo,
             * especifico acá las configuraciones
             */
            var columnas = ExcelExport.CreateColumnsFromObject(data.GetType().GetGenericArguments()[0]);
            var styleSheet = ExcelExport.DefaultMoaStyleSheet();

            MemoryStream stream = ExcelExport.ExportDtoToSingleStandardExcelSheet(data, true, styleSheet, columnas);
            return stream;
        }

        public MemoryStream DescargarPosicionesPOMultipleServicio(DateTime? desde,
                                                          DateTime? hasta,
                                                          bool sap,
                                                          bool mantenimiento,
                                                          bool web,
                                                          bool repoAutomatica,
                                                          bool? tratada,
                                                          bool contratoMarco,
                                                          List<int> centros = null,
                                                          List<int> grupoDeCompras = null,
                                                          List<int> claseDocumento = null,
                                                          List<string> tipoImputacion = null,
                                                          List<int> valorTipoImputacion = null,
                                                          int? numeroPo = null,
                                                          string nombrePliego = null)
        {
            List<SolpCrearPoMultipleDto> data = this.ListarPosicionesPOMultipleServicio(desde,
                                                       hasta,
                                                       sap,
                                                       mantenimiento,
                                                       web,
                                                       repoAutomatica,
                                                       tratada,
                                                       contratoMarco,
                                                       centros,
                                                       grupoDeCompras,
                                                       claseDocumento,
                                                       tipoImputacion,
                                                       valorTipoImputacion,
                                                       numeroPo,
                                                       nombrePliego);

            /* las siguientes 2 líneas no son necesarias si se usa la configuración predeterminada
             * ya que estas mismas llamadas se hacen dentro del método CreateColumnsFromObject
             * cuando no se especifican los parámetros.
             * Sin embargo, como esta invocación seguro va a ser usada como ejemplo,
             * especifico acá las configuraciones
             */
            var columnas = ExcelExport.CreateColumnsFromObject(data.GetType().GetGenericArguments()[0]);
            var styleSheet = ExcelExport.DefaultMoaStyleSheet();

            MemoryStream stream = ExcelExport.ExportDtoToSingleStandardExcelSheet(data, true, styleSheet, columnas);
            return stream;
        }

        public SolpCompraDto ObtenerPosicionesMultipleCompras(List<int> listaId)
        {
            try
            {
                var registrosInfo = new List<RegistroInfoDto>();
                var solp = repositorio.ObtenerConsultaEscalar(new ObtenerPosicionesMultipleComprasConsulta(listaId));

                var posiciones = solp.PosicionCompras.ToList();
                var consultaRegistro = posiciones.Where(a => !string.IsNullOrEmpty(a.MaterialComprasCodigo))
                    .GroupBy(x => new { Centro = x.Centro.CodigoSap, Material = x.MaterialComprasCodigo, GrupoDeCompras = x.GrupoCompras.CodigoSap });

                foreach (var posicionAgrupada in consultaRegistro)
                {
                    var registros = registroInfoService.ObtenerRegistroInfoConsumer(posicionAgrupada.Key.Material, posicionAgrupada.Key.Centro, posicionAgrupada.Key.GrupoDeCompras, "");
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

                                    var hoy = DateTime.Now.Date;
                                    var tablaSap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                                    DateTime fechaDesde = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaInicioConsultaSolp"].ToString());
                                    DateTime fechaHasta = Convert.ToDateTime(ConfigurationManager.AppSettings["FechaFinConsultaSolp"].ToString());
                                    var filtros = new ObtenerSolpRequest
                                    {
                                        FechaDesde = fechaDesde,
                                        FechaHasta = fechaHasta,
                                        NumeroSolp = posicion.NroSolp,
                                    };
                                    var solpSAPResponse = comprasServiceSap.ObtenerSolpSap(filtros);

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
                                            Deshabilitado = registroInfo.FechaFormateada != null && registroInfo.FechaFormateada < hoy,
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

        public HistorialDeFechaDto ListarHistorialDeFechas(int peticionDeOfertaId)
        {
            try
            {
                var peticionDeOferta = repositorio.Obtener<PeticionDeOferta>(peticionDeOfertaId);
                var nroSolps = peticionDeOferta.Posiciones.Select(posicion => posicion.SolpPosicion.Solp.Id);
                var peticionUsuarioIds = peticionDeOferta.Usuarios.Select(pu => pu.Usuario_Id);
                var cotizacionFechas = repositorio.Listar<CotizacionHistorial>(ch => ch.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta_Id == peticionDeOfertaId &&
                                    peticionUsuarioIds.Contains(ch.Usuario_Id));
                var cierreDePlazos = repositorio.Listar<PeticionDeOfertaCierre, DateTime>(x => x.Fecha, x => x.PeticionDeOferta_Id == peticionDeOfertaId);

                var solps = repositorio.Listar<Solp, SolpDto>(solp => new SolpDto
                {
                    NroSolp = solp.NroSolp,
                    FechaCreacionFormateada = SqlFunctions.DateName("day", solp.FechaCreacion) + "/" +
                    SqlFunctions.DatePart("month", solp.FechaCreacion) + "/" + SqlFunctions.DateName("year", solp.FechaCreacion) + " " +
                    SqlFunctions.DateName("hour", solp.FechaCreacion) + ":" + SqlFunctions.DateName("minute", solp.FechaCreacion) + "hs",
                    FechaLiberacionSapFormateada = solp.FechaLiberacionSap != null ? SqlFunctions.DateName("day", solp.FechaLiberacionSap.Value) + "/" +
                    SqlFunctions.DatePart("month", solp.FechaLiberacionSap.Value) + "/"
                    + SqlFunctions.DateName("year", solp.FechaLiberacionSap.Value) + " " +
                    SqlFunctions.DateName("hour", solp.FechaLiberacionSap.Value) + ":"
                    + SqlFunctions.DateName("minute", solp.FechaLiberacionSap.Value) + "hs" : "",
                }, solp => nroSolps.Contains(solp.Id));

                var proveedores = new List<HistorialPorProveedorDto>();
                foreach (var item in peticionDeOferta.Usuarios)
                {
                    var cotizacion = cotizacionFechas.Where(coti => item.Cotizaciones.Any() && coti.Cotizacion_Id == item.Cotizaciones.FirstOrDefault().Id)
                        .Select(x => x.Cotizacion).FirstOrDefault();

                    var proveedor = new HistorialPorProveedorDto
                    {
                        RazonSocial = item.Usuario.ObtenerRazonSocial(),
                        FechasCirculares = item.Circulares.Select(circular => circular.Circular.FechaCreacion).ToList(),
                        UsuariosCirculares = item.Circulares.Select(circular => circular.Circular.Usuario).ToList(),
                        FechasCotizaciones = cotizacionFechas.Where(cotiH => cotiH.Cotizacion.PeticionDeOfertaUsuario_Id == item.Id) != null ?
                        cotizacionFechas.Where(cotiH => cotiH.Cotizacion.PeticionDeOfertaUsuario_Id == item.Id).Select(coti => coti.FechaFinalizacion).ToList() : new List<DateTime>(),
                        FechaOrdenDeCompraCreacion = cotizacion != null ? cotizacion.Adjudicaciones.Select(x => x.FechaCreacion).ToList() : new List<DateTime>().ToList(),
                        FechaOrdenDeCompraLiberacion = cotizacion != null ? cotizacion.Adjudicaciones.Select(x => x.FechaLiberacionSap).Where(fecha => fecha.HasValue)
                        .Select(fecha => fecha.Value).ToList() : new List<DateTime>()
                    };
                    proveedores.Add(proveedor);
                }

                var cuerpo = ConstruirTabla(proveedores, cierreDePlazos, peticionDeOferta.RevisionTecnica?.FechaFinalizacion);
                var historial = new HistorialDeFechaDto
                {
                    ListaSolp = solps,
                    FechaCreacionPOFormateada = peticionDeOferta.FechaCreacion.ToString("dd/MM/yyyy"),
                    Cuerpo = cuerpo
                };

                return historial;


            }
            catch (Exception e)
            {
                Log.Info($"Error al ListarHistorialDeFechas");
                Log.Error(e);
                throw;
            }
        }
        private List<List<string>> ConstruirTabla(List<HistorialPorProveedorDto> proveedores, List<DateTime> cierres, DateTime? fechaRevisionTecnica)
        {
            List<List<string>> tabla = new List<List<string>>();

            // Obtener lista de razones sociales de los proveedores
            var razonesSociales = proveedores.Select(p => p.RazonSocial).ToList();

            // Encabezado de la tabla
            List<string> encabezado = new List<string> { };
            encabezado.AddRange(razonesSociales);
            tabla.Add(encabezado);

            // Obtener todas las fechas únicas de circulares y cotizaciones
            var fechasUnicas = proveedores.SelectMany(p => p.FechasCirculares)
                                          .Union(proveedores.SelectMany(p => p.FechasCotizaciones))
                                          .Union(proveedores.Where(p => p.FechaOrdenDeCompraCreacion.Any())
                                           .SelectMany(p => p.FechaOrdenDeCompraCreacion))
                                          .Union(proveedores.Where(p => p.FechaOrdenDeCompraLiberacion.Any())
                                           .SelectMany(p => p.FechaOrdenDeCompraLiberacion))
                                          .Union(cierres)
                                          .Select(f => f.Date)
                                          .Distinct()
                                          .OrderBy(f => f)
                                          .ToList();

            if (fechaRevisionTecnica.HasValue)
            {
                fechasUnicas.Add(fechaRevisionTecnica.Value.Date);
            }

            // Construir filas de la tabla
            foreach (var fecha in fechasUnicas.Distinct().OrderByDescending(x => x))
            {
                var fila = new List<string>();
                foreach (var proveedor in proveedores)
                {
                    var fechaCircular = proveedor.FechasCirculares.Where(x => x.Date == fecha.Date).ToList();
                    if (fechaCircular != null && fechaCircular.Any())
                    {
                        var rolUsuario = ObtenerRol(proveedor.UsuariosCirculares.FirstOrDefault());
                        string fechaHoraCirculares = string.Join(", ", fechaCircular.OrderBy(x => x).Select(x => x.ToString("HH:mm") + "hs"));
                        fila.Add($"{fecha.ToString("dd/MM/yyyy")} {fechaHoraCirculares} - Circular {rolUsuario}");
                    }
                    else
                    {
                        fila.Add("");
                    }

                }
                if (fila.Any()) tabla.Add(fila);
                fila = new List<string>();
                foreach (var proveedor in proveedores)
                {
                    var fechaCotizacion = proveedor.FechasCotizaciones.Where(x => x.Date == fecha.Date).ToList();
                    if (fechaCotizacion != null && fechaCotizacion.Any())
                    {
                        string fechaHoraCotizacion = string.Join(", ", fechaCotizacion.OrderBy(x => x).Select(x => x.ToString("HH:mm") + "hs"));
                        fila.Add($"{fecha.ToString("dd/MM/yyyy")} {fechaHoraCotizacion} - Cotización");
                    }
                    else
                    {
                        fila.Add("");
                    }

                }
                if (fila.Any()) tabla.Add(fila);


                fila = new List<string>();
                foreach (var proveedor in proveedores)
                {
                    if (proveedor.FechaOrdenDeCompraLiberacion.Any())
                    {
                        var fechaOrdenDeCompraLiberacion = proveedor.FechaOrdenDeCompraLiberacion.Where(x => x.Date == fecha.Date).ToList();
                        if (fechaOrdenDeCompraLiberacion != null && fechaOrdenDeCompraLiberacion.Any())
                        {
                            string fechaHoraOrdenLiberada = string.Join(", ", fechaOrdenDeCompraLiberacion.OrderBy(x => x).Select(x => x.ToString("HH:mm") + "hs"));
                            fila.Add($"{fecha.ToString("dd/MM/yyyy")} {fechaHoraOrdenLiberada} - Orden de compra liberada");
                        }
                        else
                        {
                            fila.Add("");
                        }
                    }
                    else
                    {
                        fila.Add("");
                    }

                }
                if (fila.Any()) tabla.Add(fila);

                fila = new List<string>();
                foreach (var proveedor in proveedores)
                {
                    if (proveedor.FechaOrdenDeCompraCreacion.Any())
                    {
                        var fechaOrdenDeCompraCreacion = proveedor.FechaOrdenDeCompraCreacion.Where(x => x.Date == fecha.Date).ToList();
                        if (fechaOrdenDeCompraCreacion != null && fechaOrdenDeCompraCreacion.Any())
                        {
                            string fechaHoraOrdenCreada = string.Join(", ", fechaOrdenDeCompraCreacion.OrderBy(x => x).Select(x => x.ToString("HH:mm") + "hs"));
                            fila.Add($"{fecha.ToString("dd/MM/yyyy")} {fechaHoraOrdenCreada} - Orden de compra creada");
                        }
                        else
                        {
                            fila.Add("");
                        }
                    }
                    else
                    {
                        fila.Add("");
                    }

                }
                if (fila.Any()) tabla.Add(fila);


                fila = new List<string>();
                foreach (var proveedor in proveedores)
                {
                    if (cierres.Any())
                    {
                        var cierre = cierres.Where(x => x.Date == fecha.Date).ToList();
                        if (cierre != null && cierre.Any())
                        {
                            string fechaHoraCierre = string.Join(", ", cierre.OrderBy(x => x).Select(x => x.ToString("HH:mm") + "hs"));
                            fila.Add($"{fecha.ToString("dd/MM/yyyy")} {fechaHoraCierre} - Cierre de cotización");
                        }
                        else
                        {
                            fila.Add("");
                        }
                    }

                }
                if (fila.Any()) tabla.Add(fila);


                fila = new List<string>();
                if (fechaRevisionTecnica.HasValue && fecha.Date == fechaRevisionTecnica.Value.Date)
                {
                    foreach (var proveedor in proveedores)
                    {
                        fila.Add(fechaRevisionTecnica.Value.ToString("dd/MM/yyyy HH:mm") + "hs - Revisión Técnica");
                    }

                }
                if (fila.Any()) tabla.Add(fila);
            }
            tabla = tabla.Where(fila => !fila.All(celda => string.IsNullOrEmpty(celda))).ToList();
            return tabla;
        }

        private string ObtenerRol(Usuario usuario)
        {
            return usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "Comprador" : "Solicitante";
        }

        public List<SolpDto> ListarSolpCondicionEspecial(FiltroDto filtroDto)
        {
            var filtro = ConvertirAFiltroServiceDto(filtroDto);
            var todasLasSolp = repositorio.ListarConsulta(new ListarSolpCondicionEspecialConsulta(filtro));
            return todasLasSolp;
        }

        private FiltroServiceDto ConvertirAFiltroServiceDto(FiltroDto filtro)
        {
            return new FiltroServiceDto
            {
                Paginacion = new Paginacion(filtro.Columna, filtro.Orden == "ASC" ? DirOrden.Asc : DirOrden.Desc,
                    filtro.Pagina ?? 0, filtro.ItemsPorPagina ?? 0),
                CodigoProveedor = filtro.CodigoProveedor,
                NroSolp = !string.IsNullOrEmpty(filtro.NroSolp) ? filtro.NroSolp.Trim() : "",
                FechaDesde = filtro.FechaDesde,
                FechaHasta = filtro.FechaHasta,
                Sap = filtro.Sap ?? false,
                Mantenimiento = filtro.Mantenimiento ?? false,
                Web = filtro.Web ?? false,
                RepoAutomatica = filtro.RepoAutomatica ?? false,
                ListarPendiente = filtro.ListarPendiente ?? false,
                ContratoMarco = filtro.ContratoMarco,
                Usuarios = ConvertirStringAListaInt(filtro.Usuarios),
                Estados = ConvertirStringAListaInt(filtro.Estados),
                Centros = ConvertirStringAListaInt(filtro.Centros),
                GrupoDeCompras = ConvertirStringAListaInt(filtro.GrupoDeCompras),
                ClaseDocumento = ConvertirStringAListaInt(filtro.ClaseDocumento),
                TipoImputacion = ConvertirStringAListaString(filtro.TipoImputacion),
                ValorTipoImputacion = ConvertirStringAListaInt(filtro.ValorTipoImputacion),
                EsServicio = filtro.EsServicio ?? false,
                NombrePedido = filtro.NombrePedido?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0],
                EstadoLicitacion = filtro.EstadoLicitacion,
                EstadoCotizacion = filtro.EstadoCotizacion,
                Agrupada = filtro.Agrupada,
                NroPo = !string.IsNullOrEmpty(filtro.NroPo) ? filtro.NroPo.Trim() : "",
                OrganizacionDeCompra_Id = OrganizacionDeCompraEnum.Estrategica
            };
        }

        private List<int> ConvertirStringAListaInt(string cadena)
        {
            if (string.IsNullOrEmpty(cadena))
                return new List<int>();
            return cadena.Split(',').Select(x => int.Parse(x)).ToList();
        }

        private List<string> ConvertirStringAListaString(string cadena)
        {
            if (string.IsNullOrEmpty(cadena))
                return new List<string>();
            return cadena.Split(',').ToList();
        }

        public Resultado AgruparPeticionesDeOferta(int usuarioId, string ids)
        {
            try
            {
                var resultado = new Resultado();
                var cotizaciones = new List<Cotizacion>();
                var peticionesDeOfertaIdViejas = ConvertirStringAListaInt(ids);
                var peticionesViejas = repositorio.Listar<PeticionDeOferta>(pet => peticionesDeOfertaIdViejas.Contains(pet.Id));
                var peticionDeOfertaUsuarioIds = peticionesViejas.SelectMany(p => p.Usuarios).Select(x => x.Id).ToList();

                if (peticionesViejas.Count > 0)
                {
                    PeticionDeOferta peticionNueva = CrearNuevaPOAgrupada(usuarioId, peticionesViejas);
                    CrearCotizacionEnTH(usuarioId, cotizaciones, peticionNueva);

                    //AgregarPosicionesCotizacionPOAgrupada(peticion);
                    AgregarPosicionesCotizacionPOAgrupada(peticionNueva, peticionDeOfertaUsuarioIds);

                    EliminarPOTrabajoHecho(peticionesViejas);

                    resultado.IdEntidad = peticionNueva.Id;

                }

                return resultado;
            }
            catch (Exception ex)
            {
                Log.Info($"Error al AgruparPeticionesDeOferta");
                Log.Error(ex);
                throw;
            }
        }

        private void AgregarPosicionesCotizacionPOAgrupada(PeticionDeOferta peticion, List<int> peticionDeOfertaUsuarioIds)
        {
            var cotizacionesGuardadas = repositorio.Listar<Cotizacion>(coti => peticionDeOfertaUsuarioIds.Contains(coti.PeticionDeOfertaUsuario_Id));
            var cotizacion = peticion.Usuarios.First().Cotizaciones.FirstOrDefault();
            var listaCotizaciones = new List<Cotizacion> { cotizacion };
            if (cotizacion != null)
            {
                foreach (var cotizacionVieja in cotizacionesGuardadas)
                {
                    foreach (var cotizacionPosicion in cotizacionVieja.CotizacionPosiciones)
                    {
                        cotizacionPosicion.Cotizacion_Id = cotizacion.Id;
                    }
                }
                repositorio.GuardarCambios();
                GrabarHistorialDeCotizaciones(listaCotizaciones);
            }
        }

        private void EliminarPOTrabajoHecho(List<PeticionDeOferta> peticiones)
        {
            try
            {
                repositorio.RemoverTodos(peticiones);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }

        private PeticionDeOferta CrearNuevaPOAgrupada(int usuarioId, List<PeticionDeOferta> peticiones)
        {
            var peticion = new PeticionDeOferta
            {
                Observaciones = string.Join(Environment.NewLine, peticiones.Select(peti => peti.Observaciones)),
                FechaCreacion = DateTime.Now,
                Agrupada = true,
                Posiciones = peticiones.SelectMany(x => x.Posiciones).GroupBy(y => y.SolpPosicion_Id).Select(group => group.First()).ToList(),
                UsuarioCreador_Id = usuarioId,
                AdjuntoPliego = peticiones.Any(x => x.Archivos.Any()),
                Archivos = peticiones.SelectMany(x => x.Archivos).ToList(),
                PlazoDeOferta = peticiones.Select(peti => peti.PlazoDeOferta).OrderBy(f => f).FirstOrDefault(),
                Usuarios = peticiones.SelectMany(x => x.Usuarios).GroupBy(y => y.Usuario_Id).Select(group => new PeticionDeOfertaUsuario { Usuario_Id = group.First().Usuario_Id }).ToList(),
                UsuariosAdicionales = peticiones.SelectMany(x => x.UsuariosAdicionales).ToList()
            };
            CrearRevisionTecnicaParaPO(usuarioId, peticion);
            repositorio.Agregar(peticion);
            repositorio.GuardarCambios();
            return peticion;
        }

        private static void CrearRevisionTecnicaParaPO(int usuarioId, PeticionDeOferta peticion)
        {
            peticion.RevisionTecnica = new PeticionDeOfertaRevisionTecnica
            {
                Fecha = DateTime.Now,
                FechaFinalizacion = DateTime.Now,
                Usuario_Id = usuarioId,
                Finalizada = true,
            };
        }

        public Resultado DesagruparPO(string nroSolp, string po)
        {
            try
            {
                var resultado = new Resultado();
                var posiciones = repositorio.Listar<SolpPosicion>(x => x.Solp.NroSolp == nroSolp);
                var posicionesIds = posiciones.Select(x => x.Id);
                var peticionDeOfertaSolpPosicion = repositorio.Listar<PeticionDeOfertaSolpPosicion>(x => x.PeticionDeOferta_Id.ToString() == po && posicionesIds.Contains(x.SolpPosicion_Id));
                repositorio.RemoverTodos(peticionDeOfertaSolpPosicion);
                repositorio.GuardarCambios();
                var peticion = CrearCotizacionConTrabajoYaHechoOPresupuestado(posiciones.FirstOrDefault().Solp);
                resultado.IdEntidad = peticion.Id;
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Info($"Error al DesagruparPO");
                Log.Error(ex);
                throw;
            }
        }

        private void CrearCotizacionEnTH(int usuarioId, List<Cotizacion> cotizaciones, PeticionDeOferta peticion)
        {
            foreach (var usuario in peticion.Usuarios.Distinct())
            {
                var cotizacion = new Cotizacion
                {
                    PeticionDeOfertaUsuario_Id = usuario.Id,
                    FechaCreacion = DateTime.Now,
                    UsuarioCreador_Id = usuarioId,
                    CotizacionEstado_Id = (int)CotizacionEstadoEnum.Cotizado,
                    RespetaMateriales = true,
                    RespetaServicios = true,
                };
                cotizaciones.Add(cotizacion);
                repositorio.Agregar(cotizacion);
            }

            repositorio.GuardarCambios();

        }

        private void GrabarHistorialDeCotizaciones(List<Cotizacion> cotizaciones)
        {
            foreach (var cotizacion in cotizaciones)
            {
                GrabarLogCotizacion(cotizacion);
            }
            repositorio.GuardarCambios();
        }

        public RespuestaCrearOrdenDeCompra ValidarPrecioCotizado(AdjudicacionDto adjudicacionDto)
        {
            var respuestaGuardarSOLP = new RespuestaCrearOrdenDeCompra();
            var posicionesId = adjudicacionDto.AdjudicacionPosiciones.Select(x => x.SolpPosicion_Id);
            var cotizacionPosicionId = adjudicacionDto.AdjudicacionPosiciones.Select(x => x.CotizacionPosicion_Id);
            var posicionesSolp = repositorio.Listar<SolpPosicion>(x => posicionesId.Contains(x.Id));
            var cotizacionPosiciones = repositorio.Listar<CotizacionPosicion>(x => cotizacionPosicionId.Contains(x.Id));
            var esServicios = cotizacionPosiciones.FirstOrDefault().Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo != "MATERIALES";
            var trabajoYaHecho = cotizacionPosiciones.FirstOrDefault().Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.TrabajoYaHecho == true;

            if (esServicios && !trabajoYaHecho)
            {
                respuestaGuardarSOLP.Errores = new List<String>();
                var tablasap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);
                var monedaCodigo = "";
                if (adjudicacionDto.EsMonedaProveedor)
                {
                    var monedaProv = DevolverMonedaProveedor(adjudicacionDto.Proveedor).Moneda;
                    if (!string.IsNullOrEmpty(monedaProv))
                    {
                        monedaCodigo = tablasap.Find(moneda => moneda.CodigoSap == monedaProv)?.CodigoSap;
                    }
                }

                foreach (var posicion in posicionesSolp)
                {
                    var cotizacionPosicion = cotizacionPosiciones.Find(x => x.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id);
                    decimal monto = 0;
                    var moneda = !string.IsNullOrEmpty(monedaCodigo) ? monedaCodigo : tablasap.Find(m => m.Id == posicion.Moneda_Id)?.CodigoSap;

                    monto = DevolverMontoServicioSolicitado(moneda, cotizacionPosicion.CotizacionSubPosiciones.First().Moneda.CodigoSap, posicion.Subposiciones.ToList());

                    var totalSolicitado = monto;
                    var totalCotizado = cotizacionPosicion.CotizacionSubPosiciones.Sum(cp => cp.Precio.Value * cp.Cantidad) ?? 0;
                    var monedaAdjudicada = moneda;
                    var monedaCotizada = cotizacionPosicion.CotizacionSubPosiciones.First().Moneda.Codigo;

                    if (monedaAdjudicada != monedaCotizada)
                    {
                        respuestaGuardarSOLP.Errores.Add($"Algunas posiciones no tienen el mismo precio que lo solicitado." +
                            $" La moneda solicitada ({monedaAdjudicada}) no coincide con la moneda cotizada ({monedaCotizada})");
                        return respuestaGuardarSOLP;
                    }

                    if (totalSolicitado != totalCotizado)
                    {
                        respuestaGuardarSOLP.Errores.Add($"Algunas posiciones no tienen el mismo precio que lo solicitado. El monto solicitado ({monedaAdjudicada} {totalSolicitado.ToString("n2")}) no coincide con el monto cotizado ({monedaCotizada} {totalCotizado.ToString("n2")}).");
                        return respuestaGuardarSOLP;
                    }
                }
            }
            return respuestaGuardarSOLP;
        }

        public AdjuntosSolpDto ObtenerAdjuntosSolpAgrupar(string nroSolp)
        {
            var solp = repositorio.Obtener<Solp>(a => a.NroSolp == nroSolp);

            var middleFileName = solp.NroSolp ?? solp.Pliego.NombreObra ?? "xxxx";
            var pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now:yyyyMMdd}.pdf";

            var adjuntosSolpDto = new AdjuntosSolpDto
            {
                Pliego = solp.TipoSolp.Codigo == "CON_PLIEGO" ? pdfFilename : null,
                PDF = Convert.ToBase64String(GenerarSolpPdf(solp.Id)),
                ArchivosEspecificacionesTecnicas = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),
                ArchivosCotizacion = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),
                ArchivosCondicionesEspeciales = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),
                ObservacionCondicionesEspeciales = "Justificación de condición especial: " + solp.Pliego.ObservacionesCotizacionCondEsp
            };
            return adjuntosSolpDto;
        }
        private bool ProveedorExisteEnSAP(string codigoProveedor)
        {
            return usuarioService.ObtenerProveedorSap(codigoProveedor) != null;
        }

        public Resultado GuardarEnvioCircularProveedor(int id, EnviarCircularEnum envioCircularA, DateTime? fechaLimite)
        {
            Resultado resultado = new Resultado();
            Solp solp = repositorio.Obtener<Solp>(x => x.Id == id);
            solp.EnvioCircularA = envioCircularA;
            if (fechaLimite != null)
            {
                solp.FechaLimiteReenvioDocumentacionPorCambioCondiciones = new DateTime(fechaLimite.Value.Year, fechaLimite.Value.Month, fechaLimite.Value.Day, 23, 59, 59, DateTimeKind.Local);
            }
            else
            {
                solp.FechaLimiteReenvioDocumentacionPorCambioCondiciones = null;
            }
            repositorio.GuardarCambios();
            resultado.IdEntidad = solp.Id;
            resultado.Mensaje = "Se grabó con éxito";
            return resultado;
        }

        public ValidarFechaVigenciaRegistroInfoResDto ValidarFechaVigenciaRegistroInfo(
            ValidarFechaVigenciaRegistroInfoReqDto request)
        {
            var cotizacionPosicion = repositorio.Obtener<CotizacionPosicion>(request.CotizacionPosicionId);
            if (cotizacionPosicion == null)
            {
                throw new ValidationCustomException("La posicion seleccionada no fue cotizada.");
            }
            var ultimoRegistroInfo = registroInfoService.ObtenerUltimoRegistroPorMaterialYProveedor(
                request.MaterialCodigoSap, request.CentroCodigoSap, request.GrupoComprasCodigoSap,
                cotizacionPosicion.Cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerCodigoProveedor()
                );

            return new ValidarFechaVigenciaRegistroInfoResDto
            {
                CotizacionPosicionId = request.CotizacionPosicionId,
                EstaVigente = DateTime.Parse(ultimoRegistroInfo.FechaVigencia).Date >= DateTime.Now.Date,
                FechaVigencia = ultimoRegistroInfo?.FechaVigencia
            };
        }

        public void ActualizarFechaVigenciaRegistroInfo(ActualizarFechaVigenciaRegistroInfoDto datos)
        {
            if (datos.NuevaFechaVigencia.Date < DateTime.Now.Date)
            {
                throw new ValidationCustomException("No puede seleccionarse una fecha anterior al día de hoy.");
            }
            var idCotizacionesPosiciones = datos.RegistrosInfo.Select(r => r.CotizacionPosicion_Id);
            var idSolpPosiciones = datos.RegistrosInfo.Select(r => r.SolpPosicion_Id);
            var cotizacionPosiciones = repositorio.Listar<CotizacionPosicion>(x => idCotizacionesPosiciones.Contains(x.Id));
            var solpPosiciones = repositorio.Listar<SolpPosicion>(x => idSolpPosiciones.Contains(x.Id));
            var unidadesDeMedidaSap = unidadMedidaService.ObtenerUnidadesDesdeServicioSap(solpPosiciones.Select(x => x.MaterialSolp.Codigo).ToList());
            var registros = new List<RegistroInfoDto>();

            foreach (var registro in datos.RegistrosInfo)
            {
                var cotizacionPosicion = cotizacionPosiciones.FirstOrDefault(x => x.Id == registro.CotizacionPosicion_Id);
                var solpPosicion = ObtenerSolpPosicion(solpPosiciones, cotizacionPosicion);
                var registroInfo = CrearRegistroInfoDto(cotizacionPosicion, solpPosicion, unidadesDeMedidaSap);
                registroInfo.FechaVigencia = datos.NuevaFechaVigencia.ToString("yyyy-MM-dd");
                registros.Add(registroInfo);
            }
            if (registros.Any(x => !x.EsModificar))
            {
                registroInfoService.CrearOActualizarRegistrosInfoEnSap(registros.Where(x => !x.EsModificar).ToList());
            }
            registros.ForEach(x => x.EsModificar = true);

            var respuesta = registroInfoService.CrearOActualizarRegistrosInfoEnSap(registros);
            var errores = respuesta.Errores != null ? respuesta.Errores.Where(x => x.Tipo == "E") : null;

            if (errores != null && errores.Any())
            {
                throw new WSCustomException(string.Join(" - ", errores.Select(x => x.Mensaje)));
            }
        }
        private SolpPosicion ObtenerSolpPosicion(IEnumerable<SolpPosicion> solpPosiciones, CotizacionPosicion cotizacionPosicion)
        {
            return solpPosiciones.FirstOrDefault(p => p.MaterialSolp.Codigo == cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.MaterialSolp.Codigo);
        }

        private void AgregarALegajoDescargaHistorialDeCotizaciones(List<LegajoDto> legajo, PeticionDeOferta peticion, UsuarioDto usuarioDto)
        {
            var cotizaciones = GetCotizacionesDescargables(peticion, usuarioDto)
                .Where(cotizacion => PuedenVerseLosImportesDeCotizacion(cotizacion, usuarioDto));

            foreach (var cotizacion in cotizaciones)
            {
                var fechaCotizacion = cotizacion.FechaCreacion;
                legajo.Add(new LegajoDto
                {
                    ArchivoId = cotizacion.Id,
                    Fecha = fechaCotizacion,
                    FechaFormateado = fechaCotizacion.ToString("dd/MM/yyyy"),
                    Leido = false,
                    Observacion = "Cotización proveedor " + cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerRazonSocial(),
                    PeticionDeOfertaId = peticion.Id,
                    SolpId = 0,
                    Tipo = TipoLegajo.Cotizacion,
                    Usuario = new UsuarioDto
                    {
                        CUIT = cotizacion.PeticionDeOfertaUsuario.Usuario.CUITRegistro,
                        Mail = cotizacion.PeticionDeOfertaUsuario.Usuario.Mail,
                        Id = cotizacion.PeticionDeOfertaUsuario.Usuario_Id
                    },
                    UsuarioId = 0
                });
            }
        }

        private void AgregarALegajoDescargaRevisionTecnica(List<LegajoDto> legajo, PeticionDeOferta peticion)
        {
            if (peticion?.RevisionTecnica != null)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = peticion.Id,
                    Fecha = peticion.RevisionTecnica.Fecha,
                    FechaFormateado = peticion.RevisionTecnica.Fecha.ToString("dd/MM/yyyy"),
                    Leido = false,
                    Observacion = "Revisión técnica",
                    PeticionDeOfertaId = peticion.Id,
                    SolpId = 0,
                    Tipo = TipoLegajo.RevisionTecnica,
                    Usuario = new UsuarioDto
                    {
                        Mail = peticion.RevisionTecnica.Usuario.Mail,
                        CUIT = peticion.RevisionTecnica.Usuario.CUITRegistro,
                        Id = peticion.RevisionTecnica.Usuario_Id,
                    },
                    UsuarioId = 0
                });
            }
        }

        private void AgregarALegajoPeticionVisualizarPrecio(List<LegajoDto> legajo, PeticionDeOferta peticionOferta, bool esProveedor)
        {
            if (!esProveedor)
            {
                var peticionVisualizarPrecio = repositorio.Obtener<PeticionDeOfertaVisualizacionPrecio>(x => x.PeticionDeOferta_Id == peticionOferta.Id);
                if (peticionVisualizarPrecio != null)
                {
                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = peticionVisualizarPrecio.Archivo?.Id,
                        Observacion = "Justificación de visualización de precios: " + peticionVisualizarPrecio.Observaciones,
                        PeticionDeOfertaId = peticionOferta.Id,
                        SolpId = peticionOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                        Fecha = peticionVisualizarPrecio.FechaCreacion,
                        FechaFormateado = peticionVisualizarPrecio.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = peticionVisualizarPrecio.Usuario.CUITRegistro, Mail = peticionVisualizarPrecio.Usuario.Mail, Id = peticionVisualizarPrecio.UsuarioCreador_Id },
                        Tipo = TipoLegajo.PeticionDeOfertaVisualizacionPrecio
                    });
                }
            }
        }

        private void AgregarALegajoHistorialDeMovimientos(List<LegajoDto> legajo, PeticionDeOferta peticion, int peticionDeOfertaId)
        {
            legajo.Add(new LegajoDto
            {
                ArchivoId = 0,
                Observacion = "Excel con historial de movimientos",
                PeticionDeOfertaId = peticionDeOfertaId,
                SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                Fecha = DateTime.Now,
                FechaFormateado = DateTime.Now.ToString("dd/MM/yyyy"),
                Usuario = new UsuarioDto { CUIT = "", Mail = "---" },
                Tipo = TipoLegajo.HistorialMovimientos
            });
        }

        private void AgregarALegajoDocumentosEnviadosPorProveedores(List<LegajoDto> legajo, PeticionDeOferta peticion, int peticionDeOfertaId, UsuarioDto usuarioDto)
        {
            // Buscar archivos de la cotizacion 
            if (peticion.Usuarios == null)
            {
                return;
            }
            var proveedoresProcesados = new HashSet<int>();

            foreach (var usuario in peticion.Usuarios.Where(x => x.Cotizaciones.Count > 0))
            {
                var cotizacionUsuario = usuario.Cotizaciones.First();

                if (cotizacionUsuario.Archivos.Count > 0 &&
                    !proveedoresProcesados.Contains(cotizacionUsuario.UsuarioCreador_Id))
                {
                    proveedoresProcesados.Add(cotizacionUsuario.UsuarioCreador_Id);

                    legajo.Add(new LegajoDto
                    {
                        ArchivoId = cotizacionUsuario.Archivos.First().Id,
                        Observacion = cotizacionUsuario.UsuarioCreador.ObtenerRazonSocial() + ": Descargar adjuntos",
                        PeticionDeOfertaId = peticionDeOfertaId,
                        SolpId = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id,
                        Fecha = cotizacionUsuario.FechaCreacion,
                        FechaFormateado = cotizacionUsuario.FechaCreacion.ToString("dd/MM/yyyy"),
                        Usuario = new UsuarioDto { CUIT = cotizacionUsuario.UsuarioCreador.CUITRegistro, Mail = cotizacionUsuario.UsuarioCreador.Mail, Id = cotizacionUsuario.UsuarioCreador_Id },
                        Tipo = TipoLegajo.CotizacionAdjunto,
                        PeticionDeOfertaUsuarioId = cotizacionUsuario.PeticionDeOfertaUsuario_Id
                    });

                }
            }
        }

        private IEnumerable<Cotizacion> GetCotizacionesDescargables(PeticionDeOferta peticion, UsuarioDto usuarioActual)
        {
            var estaLiberado = peticion.Posiciones.Select(x => x.SolpPosicion.Solp).All(solp => solp.EstadoSolpSap.CodigoSap == "05" || solp.EstadoSolpSap.CodigoSap == "02");

            var esMateriales = peticion.Posiciones.First().SolpPosicion.TipoPosicion.Codigo == "MATERIALES";

            if (estaLiberado)
            {
                foreach (var peticionOfertaUsuario in peticion.Usuarios)
                {
                    var circulares = peticionOfertaUsuario.Circulares.Select(cp => cp.Circular).ToList();
                    var fechaFinPlazo = CalcularFechaFinDePlazo(peticion, circulares);

                    var esUrgencia = peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp.Urgencia ?? false;
                    var plazoOfertaSinFinalizar = fechaFinPlazo >= DateTime.Now && !esUrgencia;

                    var cotizacion = peticionOfertaUsuario.Cotizaciones
                        .Where(c => c.CotizacionEstado_Id != (int)CotizacionEstadoEnum.Incompleta)
                        .OrderBy(c => c.FechaCreacion)
                        .FirstOrDefault();

                    var revisionTecnicaFinalizada = true;
                    if (cotizacion != null)
                    {
                        if (esMateriales && cotizacion.RespetaMateriales == false && (peticion.RevisionTecnica == null || !peticion.RevisionTecnica.Finalizada))
                        {
                            revisionTecnicaFinalizada = false;
                        }
                        if (!esMateriales && (peticion.RevisionTecnica == null || !peticion.RevisionTecnica.Finalizada))
                        {
                            revisionTecnicaFinalizada = false;
                        }

                        if ((!plazoOfertaSinFinalizar && revisionTecnicaFinalizada) || PuedenVerseLosImportesDeCotizacion(cotizacion, usuarioActual))
                        {
                            yield return cotizacion;
                        }
                    }
                }
            }
        }

        private DateTime CalcularFechaFinDePlazo(PeticionDeOferta peticion, List<Circular> circulares)
        {
            var plazoDeOfertaCierre = peticion.Cierres.Any() ? peticion.Cierres.Max(p => p.Fecha) : (DateTime?)null;
            var plazoDeOfertaOriginal = peticion.PlazoDeOferta;

            var ultimaCircularConFecha = circulares
                .Where(c => c.RequiereCambioDeFechas == true && c.PlazoDeOferta.HasValue)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();

            var fechaCircular = ultimaCircularConFecha?.FechaCreacion;
            var plazoDeOfertaCircular = ultimaCircularConFecha?.PlazoDeOferta;

            DateTime fechaFinPlazo;
            if (plazoDeOfertaCierre == null && fechaCircular == null)
            {
                fechaFinPlazo = plazoDeOfertaOriginal;
            }
            else
            {
                if (plazoDeOfertaCierre == null)
                {
                    fechaFinPlazo = plazoDeOfertaCircular.Value;
                }
                else
                {
                    if (fechaCircular == null)
                    {
                        fechaFinPlazo = plazoDeOfertaCierre.Value;
                    }
                    else
                    {
                        if (plazoDeOfertaCierre.Value > fechaCircular)
                        {
                            fechaFinPlazo = plazoDeOfertaCierre.Value;
                        }
                        else
                        {
                            fechaFinPlazo = plazoDeOfertaCircular.Value;
                        }
                    }
                }
            }
            return fechaFinPlazo;
        }

        public byte[] GenerarExcelHistorialMovimientos(int peticionDeOfertaId)
        {
            var historial = ListarHistorialDeFechas(peticionDeOfertaId);
            return ExcelExport.ComprasHistorialMovimientosToExcel(historial);
        }

        public string DescargarAdjuntosProveedores(int idPeticion, string pathBase, int? peticiondeOfertaUsuarioId)
        {
            var peticion = repositorio.Obtener<PeticionDeOferta>(idPeticion);
            var zipFilename = $"Adjuntos-{peticion.FechaCreacion.ToString("yyyyMMdd")}.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    // Agregar archivos de cotizaciones al zip
                    if (peticion.Usuarios != null)
                    {
                        foreach (var usuario in peticion.Usuarios.Where(x => x.Cotizaciones.Count > 0 && (peticiondeOfertaUsuarioId == null || x.Id == peticiondeOfertaUsuarioId)))
                        {
                            var cotizacionUsuario = usuario.Cotizaciones.First();

                            if (cotizacionUsuario.Archivos.Count > 0)
                            {
                                foreach (var item in cotizacionUsuario.Archivos)
                                {
                                    if ((item.FileKey == FileKeys.AdjuntoCotizacionRevisionEconomica || item.FileKey == FileKeys.AdjuntoCotizacionRevisionTecnica))
                                    {
                                        string fileName = Path.GetFileName(item.Ruta);
                                        archivo.CreateEntryFromFile(item.Ruta, fileName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return filePath;
        }

        private bool PuedenVerseLosImportesDeCotizacion(Cotizacion cotizacion, UsuarioDto usuarioDto)
        {
            if (cotizacion == null || cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Incompleta)
            {
                return false;
            }

            var peticionOfertaUsuario = cotizacion.PeticionDeOfertaUsuario;
            var peticionDeOferta = peticionOfertaUsuario.PeticionDeOferta;

            var estaLiberado = peticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp).All(solp => solp.EstadoSolpSap.CodigoSap == "05" || solp.EstadoSolpSap.CodigoSap == "02");
            var esPeticionDeMateriales = peticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo == "MATERIALES";
            var respetaMateriales = cotizacion.RespetaMateriales ?? true;
            var revisionEstaFinalizada = peticionDeOferta.RevisionTecnica != null && peticionDeOferta.RevisionTecnica.Finalizada;
            var revisionAprobada = peticionOfertaUsuario.PropuestaTecnicaAprobada == true;

            if (!estaLiberado)
            {
                return false;
            }

            var puedenVerseImportes = true;
            var circulares = peticionOfertaUsuario.Circulares.Select(cp => cp.Circular).ToList();
            var fechaFinPlazo = CalcularFechaFinDePlazo(peticionDeOferta, circulares);
            var esUrgencia = peticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Urgencia ?? false;
            var esAdmin = usuarioDto?.Permisos.Exists(p => p == "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS") ?? false;

            if (fechaFinPlazo >= DateTime.Now && !esUrgencia)
            {
                puedenVerseImportes = false;
            }
            if (esPeticionDeMateriales && !respetaMateriales && (!revisionEstaFinalizada || !revisionAprobada))
            {
                puedenVerseImportes = false;
            }
            if (!esPeticionDeMateriales && (!revisionEstaFinalizada || !revisionAprobada))
            {
                puedenVerseImportes = false;
            }

            if (!puedenVerseImportes && esAdmin)
            {
                var haSolicitadoVerPrecios = !ValidarVisualizarPrecio(usuarioDto.Id, peticionDeOferta.Id);
                puedenVerseImportes = haSolicitadoVerPrecios;
            }

            return puedenVerseImportes;
        }

        private bool PuedenVerseLosImportes(PeticionDeOferta peticionDeOferta, UsuarioDto usuarioDto, List<Circular> circulares)
        {
            var estaLiberado = peticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp).All(solp => solp.EstadoSolpSap.CodigoSap == "05" || solp.EstadoSolpSap.CodigoSap == "02");
            //var respetaMateriales = cotizacion.RespetaMateriales ?? true;
            var revisionEstaFinalizada = peticionDeOferta.RevisionTecnica != null && peticionDeOferta.RevisionTecnica.Finalizada;

            if (!estaLiberado)
            {
                return false;
            }

            var puedenVerseImportes = true;
            var fechaFinPlazo = CalcularFechaFinDePlazo(peticionDeOferta, circulares);
            var esUrgencia = peticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.Solp.Urgencia ?? false;
            var esAdmin = usuarioDto?.Permisos.Exists(p => p == "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS") ?? false;

            if (fechaFinPlazo >= DateTime.Now && !esUrgencia)
            {
                puedenVerseImportes = false;
            }

            if (!revisionEstaFinalizada)
            {
                puedenVerseImportes = false;
            }

            if (!puedenVerseImportes && esAdmin)
            {
                var haSolicitadoVerPrecios = !ValidarVisualizarPrecio(usuarioDto.Id, peticionDeOferta.Id);
                puedenVerseImportes = haSolicitadoVerPrecios;
            }

            return puedenVerseImportes;
        }

        public List<KeyValuePair<EstadoListarTratamientoSolp, string>> ListarPendienteListComboOptions()
        {
            return Enum.GetValues(typeof(EstadoListarTratamientoSolp))
                .Cast<EstadoListarTratamientoSolp>()
                .Where(x => x != EstadoListarTratamientoSolp.None)
                .Select(x => new KeyValuePair<EstadoListarTratamientoSolp, string>(x, x.GetDescription()))
                .ToList();
        }

        public ProcesarPrecargaSolpResponse ProcesarArchivoPrecargaSolp(HttpPostedFileBase archivo, int tipoSolpId)
        {
            var tiposImputaciones = ObtenerTiposImputaciones();
            var monedas = ObtenerMonedas();
            var gruposCompras = ObtenerGrupoCompras();
            var gruposArticulos = ObtenerGrupoArticulos();
            var centros = ObtenerCentros();
            var almacenes = ObtenerAlmacenes();
            var unidades = ObtenerUnidades();
            var tiposPosicion = ObtenerTiposPosicionSolp();
            var tipoPosicion = tiposPosicion.Single(x => x.Id == tipoSolpId);

            var cuentasMayor = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.CuentasSolpSap).Select(x => new TablaSapDto(x)).ToList();
            var imputaciones = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.CecoSolpSap || x.Tabla == TablasSap.OrdenSolpSap || x.Tabla == TablasSap.CentroBeneficio).Select(x => new TablaSapDto(x)).ToList();

            return comprasArchivosImportService.ProcesarArchivoPrecargaSolp(archivo, tiposImputaciones, monedas, gruposCompras, gruposArticulos, centros, almacenes, unidades, cuentasMayor, imputaciones, tipoPosicion);
        }

        public void GuardarCertificacionesParciales(List<AdjudicacionDto> adjudicaciones)
        {
            var ordenesDeCompra = adjudicaciones.Select(a => a.NumeroOrdenDeCompra).Distinct();

            var adjudicacionesBD = repositorio.Listar<Adjudicacion>(a => ordenesDeCompra.Contains(a.NumeroOrdenDeCompra));

            adjudicacionesBD.ForEach(x =>
            {
                x.AdmiteCertificacionesParciales = adjudicaciones.First(a => a.NumeroOrdenDeCompra == x.NumeroOrdenDeCompra).AdmiteCertificacionesParciales;
            });

            repositorio.GuardarCambios();
        }

        public AdjudicacionDto ObtenerAdjudicacion(string nroOC)
        {
            var adjudicacionSap = comprasServiceSap.ObtenerAdjudicacion(nroOC);

            var adjudicacionBD = repositorio.Obtener<Adjudicacion>(a => a.NumeroOrdenDeCompra == adjudicacionSap.NumeroOrdenDeCompra);

            if (adjudicacionBD != null)
            {
                adjudicacionSap.AdmiteCertificacionesParciales = adjudicacionBD.AdmiteCertificacionesParciales;
            }

            return adjudicacionSap;
        }

        private void CompletarValoresPOPorProveedor(PeticionDeOfertaDto peticionDeOferta, bool esAdmin, bool noSolicitoVerPrecios)
        {
            var tiposDeCambio = new Dictionary<int, decimal>();
            var hoy = DateTime.Now;
            var monedaDestinoPesos = repositorio.Obtener<TablaSap>(x => x.Codigo == "ARP" && x.Tabla == TablasSap.Moneda);

            var unidadesDeMedidaSAP = peticionDeOferta.EsMateriales() ?
                unidadMedidaService.ObtenerUnidadesDesdeServicioSap(peticionDeOferta.PeticionDeOfertaPosicion.Select(x => x.Posicion.CodigoMaterialSap.Codigo).ToList())
                : new List<UnidadesDeMedida>();

            foreach (var usuarioPO in peticionDeOferta.Usuarios)
            {
                var respetaMateriales = true;
                if (usuarioPO.Cotizacion?.CotizacionPosiciones != null)
                {
                    if (usuarioPO.Cotizacion.RespetaMateriales == false) { respetaMateriales = false; }

                    foreach (var cotizacionPosicion in usuarioPO.Cotizacion.CotizacionPosiciones.Where(x => x.NoDisponible != true))
                    {
                        if (cotizacionPosicion.Completado)
                        {
                            CalcularCantidadYPrecioPorUnidadCotizada(peticionDeOferta, unidadesDeMedidaSAP, cotizacionPosicion);
                        }
                        decimal cambio = CalcularTipoDeCambio(tiposDeCambio, monedaDestinoPesos, cotizacionPosicion);
                        if (!cotizacionPosicion.EstaEliminado)
                        {

                            cotizacionPosicion.TotalPesos = cambio * cotizacionPosicion.PrecioTotal;
                            cotizacionPosicion.TotalARPCotizacionPosicion = cotizacionPosicion.CotizacionSubPosiciones.Sum(x => x.TotalARPSubPosCotizacion);
                        }

                        cotizacionPosicion.TodasTotalPesos = cambio * cotizacionPosicion.PrecioTotal;
                        cotizacionPosicion.TodasTotalARPCotizacionPosicion = cotizacionPosicion.CotizacionSubPosiciones.Sum(x => x.TotalARPSubPosCotizacion);

                    }
                    usuarioPO.Cotizacion.TotalGlobal = usuarioPO.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalPesos);
                    usuarioPO.Cotizacion.TotalGlobalSubPos = usuarioPO.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalARPCotizacionPosicion);
                    usuarioPO.Cotizacion.TodasTotalGlobal = usuarioPO.Cotizacion.CotizacionPosiciones.Sum(x => x.TodasTotalPesos);
                    usuarioPO.Cotizacion.TodasTotalGlobalSubPos = usuarioPO.Cotizacion.CotizacionPosiciones.Sum(x => x.TodasTotalARPCotizacionPosicion);
                }

                var mensaje = "Adjudicar";
                var verAdjudicar = true;
                usuarioPO.VerImportes = true;

                if (usuarioPO.Cotizacion == null)
                {
                    mensaje = "Sin Cotizar";
                    verAdjudicar = false;
                    usuarioPO.VerImportes = false;
                }
                if (usuarioPO.Cotizacion?.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Incompleta)
                {
                    mensaje = "Oferta sin finalizar";
                    verAdjudicar = false;
                    usuarioPO.VerImportes = false;
                }
                if (usuarioPO.Cotizacion != null && usuarioPO.PropuestaTecnicaAprobada == false)
                {
                    mensaje = "Propuesta técnica Rechazada";
                    verAdjudicar = false;
                    usuarioPO.VerImportes = false;
                }
                if (!peticionDeOferta.EstaLiberado)
                {
                    mensaje = "SOLP Sin liberar";
                    verAdjudicar = false;
                    usuarioPO.VerImportes = false;
                }

                var fechaFinPlazo = usuarioPO.GetFechaFinPlazo();

                if (usuarioPO.Cotizacion != null && fechaFinPlazo >= hoy && peticionDeOferta.Urgencia != true)
                {
                    mensaje = "Plazo de oferta sin finalizar";
                    verAdjudicar = false;
                    usuarioPO.VerImportes = false;
                }

                if (!usuarioPO.EstaHabilitado)
                {
                    mensaje = "Proveedor deshabilitado";
                    verAdjudicar = false;
                }

                switch (usuarioPO.ProveedorEstadoAprobacion)
                {
                    case EstadoAprobacion.Aprobado:
                        //do nothing
                        break;
                    case EstadoAprobacion.AltaIncompleta:
                        mensaje = "Completar alta";
                        verAdjudicar = false;
                        break;
                    default:
                        mensaje = "Proveedor NO habilitado";
                        verAdjudicar = false;
                        break;
                }

                if (peticionDeOferta.EsMateriales())
                {
                    if (!respetaMateriales && !peticionDeOferta.RevisionFinalizada)
                    {
                        mensaje = "Revisión técnica sin finalizar.";
                        verAdjudicar = false;
                        usuarioPO.VerImportes = false;
                    }
                }
                else
                {
                    if (!peticionDeOferta.RevisionFinalizada)
                    {
                        mensaje = "Revisión técnica sin finalizar.";
                        verAdjudicar = false;
                        usuarioPO.VerImportes = false;
                    }

                    if (usuarioPO.Cotizacion?.CotizacionPosiciones
                        .Exists(x => !x.EstaEliminado && !x.Adjudicado && x.CotizacionSubPosiciones.Exists(y => !y.Completado)) == true)
                    {
                        mensaje = "La cotización tiene subposiciones sin cotizar";
                        verAdjudicar = false;
                    }
                }

                if (verAdjudicar) // Lo siguiente se hace unicamente en caso que todavía esté habilitada la adjudicación, ya que es una consulta costosa.
                {
                    var fechasSap = CommonUtil.toDateList(DateTime.Now.AddYears(-5).ToShortDateString(), DateTime.Now.ToShortDateString());
                    var vendedoresMoa = usuarioService.ObtenerVendedorSap(usuarioPO.CodigoProveedor, fechasSap);
                    if (vendedoresMoa == null || vendedoresMoa.vendedores == null || vendedoresMoa.vendedores.Count == 0)
                    {
                        mensaje = "No existe un proveedor con ese código.";
                        verAdjudicar = false;
                    }
                }

                if (esAdmin && !noSolicitoVerPrecios)
                {
                    usuarioPO.VerImportes = true;
                }

                usuarioPO.MensajeAdjudicar = mensaje;
                usuarioPO.VerAdjudicar = verAdjudicar;

                if (usuarioPO.Cotizacion != null && !usuarioPO.Cotizacion.CotizacionPosiciones.TrueForAll(d => d.MonedaDescripcion == "ARP"))
                {
                    if (peticionDeOferta.EsMateriales())
                    {
                        usuarioPO.TotalesPorMoneda = usuarioPO.Cotizacion.CotizacionPosiciones
                            .Where(x => x.Completado)
                            .GroupBy(x => x.MonedaDescripcion)
                            .Select(grupo => new MonedaTotalDto
                            {
                                Moneda = grupo.Key,
                                Total = grupo.Sum(x => x.PrecioTotal).ToString("N2")
                            })
                            .ToList();
                    }
                    else
                    {
                        usuarioPO.TotalesPorMoneda = usuarioPO.Cotizacion.CotizacionPosiciones
                            .Where(x => x.Completado)
                            .SelectMany(pos => pos.CotizacionSubPosiciones)
                            .Where(x => x.Completado)
                            .GroupBy(sub => sub.MonedaDescripcion)
                            .Select(grupo => new MonedaTotalDto
                            {
                                Moneda = grupo.Key,
                                Total = grupo.Sum(sub => sub.PrecioTotalSubPos).ToString("N2")
                            })
                            .ToList();
                    }
                }
            }
        }

        private void AgregarOrdenesCompraDeSap(PeticionDeOfertaDto peticionOferta, List<SolpPosicionDto> solpPosiciones)
        {
            List<TablaSap> tablaSapMoneda = null;
            RegionSap regionSapSantaFe = null;

            var ordenesCompraSap = comprasServiceSap.ObtenerOrdenesCompraSapParaSolpPosicion(solpPosiciones);

            var adjudicacionesAGrabar = new List<Adjudicacion>();

            foreach (var poUsuario in peticionOferta.Usuarios.Where(pou => pou.Cotizacion != null))
            {
                var proveedorCodigo = poUsuario.CodigoProveedor;
                var cotizacionId = poUsuario.Cotizacion.Id;
                var regionSapId = poUsuario.Cotizacion.Adjudicaciones.FirstOrDefault()?.RegionSap;

                var ordenesSapNoEnWeb = ordenesCompraSap.Where(ocs =>
                    ocs.Cabecera.CodigoProveedor == proveedorCodigo &&
                    !poUsuario.Cotizacion.Adjudicaciones.Any(a => a.NumeroOrdenDeCompra == ocs.Cabecera.OrdenDeCompra));

                foreach (var nuevaOCSap in ordenesSapNoEnWeb)
                {
                    tablaSapMoneda = tablaSapMoneda ?? tablaSapService.ObtenerMonedas();
                    var moneda = tablaSapMoneda.First(m => m.CodigoSap == nuevaOCSap.Cabecera.Moneda);

                    regionSapSantaFe = regionSapSantaFe ?? repositorio.Obtener<RegionSap>(r => r.CodigoSap == "12");

                    var nuevaAdjudicacion = new Adjudicacion
                    {
                        AdmiteCertificacionesParciales = false,
                        Cotizacion_Id = cotizacionId,
                        FechaCreacion = nuevaOCSap.Cabecera.FechaCreacion,
                        Moneda = moneda,
                        Moneda_Id = moneda.Id,
                        MontoTotal = nuevaOCSap.Cabecera.MontoTotal,
                        NumeroOrdenDeCompra = nuevaOCSap.Cabecera.OrdenDeCompra,
                        RegionSap_Id = regionSapId ?? regionSapSantaFe.Id,
                        UsuarioCreador_Id = peticionOferta.UsuarioCreador_Id
                    };

                    adjudicacionesAGrabar.Add(nuevaAdjudicacion);

                    poUsuario.Cotizacion.Adjudicaciones.Add(new AdjudicacionDto
                    {
                        AdmiteCertificacionesParciales = nuevaAdjudicacion.AdmiteCertificacionesParciales,
                        Cotizacion_Id = nuevaAdjudicacion.Cotizacion_Id,
                        FechaCreacion = nuevaAdjudicacion.FechaCreacion,
                        MontoTotal = nuevaAdjudicacion.MontoTotal,
                        NumeroOrdenDeCompra = nuevaAdjudicacion.NumeroOrdenDeCompra,
                        UsuarioCreador_Id = nuevaAdjudicacion.UsuarioCreador_Id
                    });
                }

                poUsuario.Cotizacion.Adjudicaciones = poUsuario.Cotizacion.Adjudicaciones.Where(a =>
                ordenesCompraSap.Select(b => b.Cabecera.OrdenDeCompra).Contains(a.NumeroOrdenDeCompra)
                    ).ToList();
            }

            if (adjudicacionesAGrabar.Any())
            {
                adjudicacionesAGrabar.ForEach(a => repositorio.Agregar(a));
                repositorio.GuardarCambios();
            }
        }

        private List<PeticionDeOferta> ConsultarPeticionesDeOfertaADesvincular(List<int> idsPOsADesvincular, int? solpId = null, int? solpPosicionId = null)
        {
            var includes = new List<Expression<Func<PeticionDeOferta, object>>>
            {
                po => po.Posiciones
            };

            var peticionesDeOfertaADesvincular = repositorio.Listar(x => idsPOsADesvincular.Contains(x.Id), includes: includes);

            foreach (var po in peticionesDeOfertaADesvincular)
            {
                if (po.Posiciones.Count == 1 &&
                    (solpId == null || po.Posiciones.First().SolpPosicion.Solp_Id == solpId) &&
                    (solpPosicionId == null || po.Posiciones.First().SolpPosicion_Id == solpPosicionId))
                {
                    throw new ValidationCustomException($"La PO {po.Id} no puede quedar sin posiciones vinculadas");
                }
            }

            return peticionesDeOfertaADesvincular;
        }

        private void NotificarPOsModificadas(List<PeticionDeOferta> peticionesDeOferta)
        {
            foreach (var po in peticionesDeOferta)
            {
                try
                {
                    EnviarMailPeticionDeOferta(po, po.Usuarios.ToList(), po.UsuariosAdicionales.ToList(), true);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error al enviar mail PO desvinculada. PO {po.Id}", ex);
                }
            }
        }
    }
}