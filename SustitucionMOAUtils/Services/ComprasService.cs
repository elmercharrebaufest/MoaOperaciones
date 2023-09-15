using HandlebarsDotNet;
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
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
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
            IEmailService emailService)
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
        }

        public RespuestaGuardarSOLP GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
        {

            Solp solpEntity = null;
            Pliego pliegoEntity = null;
            SolpPosicion postEntitySubPosicionesEliminadas = null;

            if (solp.Id.HasValue)
            {
                //es la forma de decirle a entity framework que tambine me traiga todas estas cosas
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
                    if (solpEntity.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || solpEntity.TipoSolpSap == (int?)TipoSolpSap.Sap)
                    {
                        solpEntity.UsuarioCreacion_Id = solp.UsuarioActual.Id;
                    }
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
                solpEntity.Adicional = solp.Adicional;
                solpEntity.NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional;
                solpEntity.ProveedorAsignado_Id = solp.ProveedorAsignadoId;
                pliegoEntity = solpEntity.Pliego;

                solpEntity.NroSolp = solp.NroSolp;

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
                solpEntity.Adicional = solp.Adicional;
                solpEntity.NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional;
                solpEntity.ProveedorAsignado_Id = solp.ProveedorAsignadoId;
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
                    respuestaGuardarSOLP = FinalizarSolp(solp, solpEntity, postEntitySubPosicionesEliminadas, respuestaGuardarSOLP);
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
                            subposEntity.ServicioSolp = repositorio.Obtener<ServicioSolp>(x => x.CodigoSap == subpos.CodigoServicioSap.Codigo);

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

        private RespuestaGuardarSOLP FinalizarSolp(SolpDto solp, Solp solpEntity, SolpPosicion postEntitySubPosicionesEliminadas, RespuestaGuardarSOLP respuestaGuardarSOLP)
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
                    foreach (var posiciones in solpEntity.Posiciones)
                    {
                        posiciones.EsConcluido = true;
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
                    var mensaje = "No se pudo procesar la SOLP";
                    respuestaGuardarSOLP.Errores.Add(mensaje);
                    break;
                }
                if (respuestaGuardarSOLP.Errores.Count == 0)
                {
                    respuestaGuardarSOLP.Mensaje = "OK";
                    if (solpEntity.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento || solpEntity.TipoSolpSap == (int?)TipoSolpSap.Sap)
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
                }
                else
                {
                    //revertir los cambios si da error
                    ObtenerSolpesDesdeSAPJob(new ObtenerSolpRequest { NumeroSolp = solpEntity.NroSolp, FechaDesde = new DateTime(2010, 01, 01), FechaHasta = DateTime.Now.Date.AddDays(1) });
                    respuestaGuardarSOLP.Solp = TraerSolpId(solp.Id.Value);
                }
                repositorio.GuardarCambios();
            }
            respuestaGuardarSOLP.IdEntidad = solp.Id.Value;
            return respuestaGuardarSOLP;

            //TODO: Esto de crear pedido queda comentado por que todavia falta las definiciones del requerimiento.
            //NO BORRAR EL CODIGO COMENTADO EN EL BLOQUE DE ABAJO

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
            var tablaSolp = repositorio.Listar<TablaSap>(x => x.Tabla == tabla).Select(x => new TablaSapDto(x)).ToList();
            if (tabla == TablasSap.EstadoSolpSap)
            {
                tablaSolp.Add(new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" });
            }
            return tablaSolp;
        }

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x => new CentroDireccionDto(x)).ToList();
        }

        public ListaPaginada<SolpDto> ListarSolp(UsuarioDto usuarioActual, Paginacion paginacion, string nroSolp, DateTime? desde, DateTime? hasta, bool? sap, bool? mantenimiento, bool? web, int? usuarioId, List<int> estados = null)
        {
            try
            {
                var fechaHasta = hasta != null ? hasta.Value.AddDays(1) : (DateTime?)null;
                var hoy = DateTime.Now.Date;
                var usuariosCompras = repositorio.Listar<UsuarioCompras>();
                var usuariosComprasRelacion = repositorio.Listar<UsuarioComprasRelacionConUsuarios>(x => x.Usuario_Id == usuarioActual.Id);
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
                    EstadoSolpSapId = x.NroSolp != null && x.Posiciones.All(p => p.Estado == false) ? -1 : (x.EstadoSolpSap != null ? x.EstadoSolpSap_Id : 0),
                    EstadoSolpSap = new TablaSapDto { Descripcion = x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : "", Id = x.EstadoSolpSap != null ? x.EstadoSolpSap.Id : 0 },
                    EstadoSolpDescripcion = x.NroSolp != null && x.Posiciones.All(p => p.Estado == false) ? "Borrado en SAP" : (x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : ""),
                    TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "" },
                    VincularPliego = !x.Pliego_Id.HasValue,
                    TieneCondicionesGenerales = x.Pliego == null ? (bool?)null : x.Pliego.TieneCondicionesGenerales,
                    RevisadoPor = x.Pliego == null ? "" : x.Pliego.RevisadoPor,
                    TipoSolpSap = x.TipoSolpSap,
                    EstadoPasos = x.EstadoPasos,
                    PosicionesEstado = x.Posiciones.All(p => p.Estado == false),
                    ItemPorPagina = paginacion.ItemsPorPagina,
                    Pagina = paginacion.Pagina,
                    TipoPosicionCodigo = x.Posiciones.Select(posiciones => posiciones.TipoPosicion.Codigo).FirstOrDefault(),
                },
                paginacion,
                x => x.FechaBorrado == null && (string.IsNullOrEmpty(nroSolp) || x.NroSolp.ToUpper().StartsWith(nroSolp.ToUpper())) &&
                (!estados.Any() || (x.EstadoSolpSap_Id != null && estados.Contains((int)x.EstadoSolpSap_Id)) || (estados.Any(y => y == -1) && x.NroSolp != null && x.Posiciones.All(p => p.Estado == false))) &&
                (usuarioId == null || (x.UsuarioCreacion_Id != null && usuarioId == x.UsuarioCreacion_Id)) &&
                (sap == true && x.TipoSolpSap == 3 ||
                mantenimiento == true && x.TipoSolpSap == 2 ||
                (web == true && (x.TipoSolpSap == null || x.TipoSolpSap == 1))
                || (sap == false && mantenimiento == false && web == false)) &&
                (desde == null || x.FechaCreacion >= desde.Value) && (fechaHasta == null || x.FechaCreacion <= fechaHasta.Value));
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
                ProveedorAsignadoId = solp.ProveedorAsignado_Id,
                TrabajoYaHecho = solp.TrabajoYaHecho,
                Adicional = solp.Adicional,
                NroOrdenDeCompraAdicional = solp.NroOrdenDeCompraAdicional,
                DeshabilitarAdicional = solp.Adjudicacions.Any(),

                Adjuntos = solp.Pliego.Archivos.Where(a => a.FileKey == FileKeys.AdjuntoSolp || a.FileKey == FileKeys.AdjuntoCotizacionesSolp).Select(s => new ArchivoDto
                {
                    Id = s.Id,
                    Nombre = s.ObtenerNombre(s.Ruta),
                    FileKey = s.FileKey,
                }).ToList(),

                EspecificacionesTecnicas = solp.Pliego.Archivos.FirstOrDefault(a => a.FileKey == FileKeys.EspecificacionesTecnicasPliego)?.Ruta,

                TieneCondicionesGenerales = solp.Pliego.TieneCondicionesGenerales ?? true,

                RevisadoPor = solp.Pliego.RevisadoPor,

                EstadoSolpSapId = solp.EstadoSolpSap_Id,
                EstadoDocumentoId = solp.EstadoDocumento_Id,

                //Posiciones = (x.TipoSolpSap == (int)TipoSolpSap.Sap || x.TipoSolpSap == (int)TipoSolpSap.Mantenimiento) ? 
                //                x.Posiciones.Select(p => new SolpPosicionDto(p)).ToList() : 
                //                x.Posiciones.Where(p => !p.FechaBaja.HasValue).Select(p => new SolpPosicionDto(p)).ToList(),

                Posiciones = solp.Posiciones.Select(p => new SolpPosicionDto(p)).ToList(),
                PasoCompletado = solp.PasoCompletado,
                EstadoPasos = solp.EstadoPasos,
                EmailLinkToken = solp.EmailLinkToken
            };
            if (solpDevuelta.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || solpDevuelta.TipoSolpSap == (int)TipoSolpSap.Sap)
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

            if (solpDevuelta.ProveedorAsignadoId != null)
            {
                var usuario = repositorio.Obtener<Usuario>(solpDevuelta.ProveedorAsignadoId);
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

                subposiciones.AppendLine($"<tr><th class='posicion' colspan='6'> {pos.Indice} - {pos.Tarea}</th></tr>{texto}<tr><td colspan='6'>&nbsp;</td></tr>");
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

                    var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor

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
            string[] palabras = valor.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<ServicioSolpDto> lista = repositorio.Listar<ServicioSolp>()
            .FindAll(e => palabras.All(p => e.Descripcion.ToLower().Contains(p)))
            .Select(s => new ServicioSolpDto(s)).ToList();

            return lista;
        }

        public List<ServicioSolpDto> AutocompleteCodigoServicioSolp(string valor)
        {
            List<ServicioSolpDto> lista = repositorio.Listar<ServicioSolp>()
                .FindAll(e => e.CodigoSap.ToString().ToLower().Contains(valor.ToLower()))
                .Select(s => new ServicioSolpDto(s)).ToList();

            return lista;
        }

        //private void EnviarMailSolpLiberada(Solp solp, Usuario usuario) //-- No borrar por las dudas
        //{
        //    try
        //    {
        //        var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_SOLP);
        //        string asunto = "MOA COMPRAS - Solp Liberada";

        //        var cuerpo = string.Format(cuerpoTemplate, solp.NroSolp, usuario.Mail);
        //        var Destinatario = usuario.Mail;



        //        emailService.EnviarMail(new List<string> { Destinatario }, asunto, cuerpo, null, null, null, null);
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}

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
                solp.FechaLiberacionSap = fechaLiberacion;
                solp.EstadoSolpSap_Id = estadoSolpSapLiberada;
                repositorio.GuardarCambios();

                if (!solp.PeticionesDeOferta.Any())
                {
                    if (solp.TrabajoYaHecho == true)
                    {
                        CrearCotizacionConTrabajoYaHecho(solp);
                    }

                    if (solp.TrabajoYaHecho != true && solp.Adicional == true)
                    {
                        CrearPeticionAutomatica(solp, new List<int> { solp.ProveedorAsignado_Id.Value }, null, false);
                    }
                }
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

                List<TablaSap> centro = tablaSap.Where(x => x.Tabla == TablasSap.Centro).ToList();
                List<TablaSap> grupoArticulo = tablaSap.Where(x => x.Tabla == TablasSap.GrupoArticulo).ToList();
                List<TablaSap> unidad = tablaSap.Where(x => x.Tabla == TablasSap.Unidad).ToList();
                List<TablaSap> grupoCompras = tablaSap.Where(x => x.Tabla == TablasSap.GrupoCompras).ToList();
                List<TablaSap> cuentas = tablaSap.Where(x => x.Tabla == TablasSap.CuentasSolpSap).ToList();

                var contador = 0;
                foreach (var material in Materiales)
                {
                    try
                    {
                        var centroId = centro.FirstOrDefault(x => x.CodigoSap == material.CentroLogistico)?.Id;
                        var item = listaBase.FirstOrDefault(x => x.CodigoSap == material.NroMaterial && x.Centro_Id == centroId);

                        contador += 1;
                        if (item == null)
                        {
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
                Logger.Log.Info($"ObtenerSolpesDesdeSAPJob numero{obtenerSolpRequest.NumeroSolp}");
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

                List<SustitucionMOAModel.Entities.TipoSolpPosicionSAP> tiposSolpPosicionSAP = repositorio.Listar<SustitucionMOAModel.Entities.TipoSolpPosicionSAP>();
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

                List<string> numeroSolicitudes = result.Posiciones.Select(a => a.NumeroSolicitud).Distinct().ToList();
                var solpdsDB = repositorio.Listar<Solp>(s => numeroSolicitudes.Contains(s.NroSolp));
                foreach (var posicion in result.Posiciones)
                {
                    try
                    {
                        SustitucionMOAWS.WSConsumers.TipoImputacionSAP tipoImputacion = result.TipoImputaciones
                        .FirstOrDefault(dir => dir.NumeroSolicitud == posicion.NumeroSolicitud &&
                                                dir.NumeroPosicion == posicion.NumeroPosicion);

                        Solp solp = solpsFinales.SingleOrDefault(x => x.NroSolp == posicion.NumeroSolicitud);

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
                                        TipoSolpSap = tipoImputacion != null && !string.IsNullOrEmpty(tipoImputacion.IdOrden) && posicion.OrigenCreacion == "F" ? (int?)TipoSolpSap.Mantenimiento : (int?)TipoSolpSap.Sap,
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

                            solpsFinales.Add(solp);
                        }
                        if (solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento || solp.TipoSolpSap == (int)TipoSolpSap.Sap)
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
                            solp.Posiciones.Add(posicionEntity);
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
                        Logger.Log.Info($"ObtenerSolpesDesdeSAPJob numero{obtenerSolpRequest.NumeroSolp}, pos: {posicion.NumeroPosicion}, estado:{posicion.EstadoPosicion}.");

                        posicionEntity.Tarea = posicion.TextoPosicion;
                        posicionEntity.NroNecesidad = posicion.NumeroRequerimientoInterno;
                        posicionEntity.EsConcluido = true;

                        if (posicionEntity.TipoPosicion_Id == 10)
                        {
                            posicionEntity.Cantidad = posicion.Cantidad;
                            TablaSap unidadMedidapos = unidadesDeMedida.FirstOrDefault(um => um.Codigo == posicion.UnidadMedida);
                            posicionEntity.Unidad_Id = unidadMedidapos?.Id;
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

                            TablaSap unidadMedida = unidadesDeMedida.FirstOrDefault(um => um.Codigo == subPosicion.UnidadDeMedida);

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
                            subPosicionEntity.Unidad_Id = unidadMedida?.Id;
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
                        Logger.Log.Info($"Error al agregar la SOLP {posicion.NumeroSolicitud}  NumeroPosicion {posicion.NumeroPosicion}");
                        Logger.Log.Error(e);
                        continue;
                    }
                }

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
                Logger.Log.Info($"ObtenerSolpesDesdeSAPJob fin");

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
            var usuariosCompras = repositorio.Listar<UsuarioComprasRelacionConUsuarios>(x => x.Usuario_Id == usuarioActual.Id)
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
                palabras.All(p => e.Descripcion.Contains(p)) && e.Centro_Id == centroId, 0, null, DirOrden.Asc)
                .Select(s => new MaterialSolpDto(s)).ToList();

            return lista;
        }

        public List<MaterialSolpDto> AutocompleteCodigoMaterialSolp(string valor, int centroId)
        {
            List<MaterialSolpDto> lista = repositorio.Listar<MaterialSolp>(e =>
                (e.Descripcion.Contains(valor) || e.CodigoSap.ToString().Contains(valor)) && e.Centro_Id == centroId, 0, null, SustitucionMOAModel.Consultas.DirOrden.Asc)
                .Select(s => new MaterialSolpDto(s)).ToList();

            return lista;
        }

        public void EnviarEmailSolp(EmailComposeDto emailCompose)
        {
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

        public List<ProvinciaDTO> ListarProvincia()
        {
            List<ProvinciaDTO> lista = repositorio.Listar<Provincia>()
                  .Select(s => new ProvinciaDTO(s)).ToList();
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

        public ListaPaginada<SolpDto> ListarSolpComprador(Paginacion paginacion, string nroSolp, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null)
        {
            var todasLasSolp = repositorio.ListarConsultaPaginada(new ListarSolpConsulta(paginacion, nroSolp, usuarios, estados, centros, grupoDeCompras));
            if (todasLasSolp != null && todasLasSolp.Count() > 0)
            {
                todasLasSolp.FirstOrDefault().ItemsTotales = todasLasSolp.ItemsTotales;
            }
            foreach (var item in todasLasSolp.Items)
            {
                item.CentroFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.CentroCodigo).GroupBy(x => x.CentroCodigo).Select(x => x.Key)) : "";
                item.GrupoCompraFormateado = item.PosicionCompras != null ? string.Join(", ", item.PosicionCompras.OrderBy(x => x.GrupoComprasCodigo).GroupBy(x => x.GrupoComprasCodigo).Select(x => x.Key)) : "";
            }

            return todasLasSolp;
        }

        public ListaPaginada<PeticionDeOfertaDto> ListarPOProveedor(Paginacion paginacion, string nroSolp, string username)
        {
            try
            {
                var userId = repositorio.Obtener<Usuario>(a => a.Mail == username).Id;
                var todasLasPO = repositorio.ListarConsultaPaginada(new ListarSolpPOConsulta(paginacion, nroSolp, userId));
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
            catch (Exception e)
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
                var solp = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(filtros);

                foreach (var item in todasLasOfertas.Usuarios)
                {
                    if (item.Cotizacion != null && item.Cotizacion.CotizacionPosiciones != null)
                    {
                        foreach (var item2 in item.Cotizacion.CotizacionPosiciones)
                        {
                            decimal cambio = 0;
                            if (!tipodecambio.TryGetValue(item2.Moneda_Id, out cambio) && item2.Moneda_Id > 0)
                            {
                                var tipoCambio = ObtenerTipoCambio(item2.Moneda_Id, destino.Id, DateTime.Now);
                                tipodecambio.Add(item2.Moneda_Id, tipoCambio.TipoCambio);
                                cambio = tipoCambio.TipoCambio;
                            }

                            if (item2.CotizacionSubPosiciones != null)
                            {
                                foreach (var subpos in item2.CotizacionSubPosiciones)
                                {
                                    if (subpos.Moneda_Id != null && !tipodecambio.TryGetValue(subpos.Moneda_Id.Value, out cambio) && subpos.Moneda_Id > 0)
                                    {
                                        var tipoCambio = ObtenerTipoCambio(subpos.Moneda_Id.Value, destino.Id, DateTime.Now);
                                        tipodecambio.Add(subpos.Moneda_Id.Value, tipoCambio.TipoCambio);
                                        cambio = tipoCambio.TipoCambio;
                                    }

                                    subpos.TotalARPSubPosCotizacion = cambio * subpos.PrecioTotalSubPosCotizacion;
                                }
                                item2.TotalPosicionCotizacion = item2.CotizacionSubPosiciones.Sum(x => x.PrecioTotalSubPosCotizacion);
                            }
                            item2.TotalPesos = cambio * item2.PrecioTotal;
                            item2.TotalARPCotizacionPosicion = item2.CotizacionSubPosiciones.Sum(x => x.TotalARPSubPosCotizacion);
                        }
                        item.Cotizacion.TotalGlobal = item.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalPesos);
                        item.Cotizacion.TotalGlobalSubPos = item.Cotizacion.CotizacionPosiciones.Sum(x => x.TotalARPCotizacionPosicion);
                    }
                    //  item.VerAdjudicar = item.Cotizacion == null ? false : item.Cotizacion != null && item.PlazoDeOferta.Date <= hoy && item.Cotizacion.CotizacionEstadoDescripcion == "Cotizado" ? false : item.EstaHabilitado ? false : todasLasOfertas.EstaLiberado ? false: true;

                    var esAdmin = usuario.Permisos.Any(p => p == "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS");
                    var mensaje = "Adjudicar";
                    var verAdjudicar = true;
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
                    if (!todasLasOfertas.EstaLiberado)
                    {
                        mensaje = "SOLP Sin liberar";
                        verAdjudicar = false;
                    }
                    if (!esAdmin)
                    {
                        var fecha = (item.PlazoDeOfertaCierre == null && item.FechaCircular == null) ? item.PlazoDeOfertaOriginal :
                  item.PlazoDeOfertaCierre == null ? item.PlazoDeOfertaCircular.Value :
                  item.FechaCircular == null ? item.PlazoDeOfertaCierre.Value :
                  item.PlazoDeOfertaCierre.Value > item.FechaCircular.Value ? item.PlazoDeOfertaCierre.Value : item.PlazoDeOfertaCircular.Value;

                        if (item.Cotizacion != null && fecha >= hoy)
                        {
                            mensaje = "Plazo de oferta sin finalizar";
                            verAdjudicar = false;
                        }
                    }
                    if (!item.EstaHabilitado)
                    {
                        mensaje = "Proveedor desahabilitado";
                        verAdjudicar = false;
                    }
                    item.MensajeAdjudicar = mensaje;
                    item.VerAdjudicar = verAdjudicar;
                }
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

            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
                bool eliminarPosicion = false;
                bool eliminarSubPosicion = false;
                if (posicion.TipoPosicion.Codigo.ToLower() == "servicios")
                {
                    eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                    eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                }
                eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;
                numeroPosicion++;

                preqItem = $"{numeroPosicion:00000}";
                docItem = preqItem;
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";

                var IM_PRITEM = new ZMPES5700();

                //Nombre: ZBAPIMEREQITEMIMP Denominación:	Posición de SOLPED
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

                //Estos datos se envian en el caso de que la posicion sea de materiales
                if (posicion.TipoPosicion.Codigo.ToLower() == "materiales")
                {
                    IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
                    IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad; //QUANTITY BAMNG   Cantidad solicitud de pedido
                    IM_PRITEM.QUANTITYSpecified = true;
                    IM_PRITEM.UNIT = posicion.Unidad.CodigoSap.ToString(); //UNIT BAMEI   Unidad de medida de solicitud pedido
                                                                           //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
                    IM_PRITEM.PREQ_PRICE = (Decimal)posicion.PrecioBruto; //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
                    IM_PRITEM.PREQ_PRICESpecified = true;
                    //IM_PRITEM.PRICE_UNIT = null; //PRICE_UNIT EPEIN   Cantidad base  
                    //IM_PRITEM.PRICE_UNITSpecified = true;
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
                IM_PRITEM.FIXED_VEND = posicion.ProveedorFijo; //FIXED_VEND FLIEF   Proveedor fijo
                IM_PRITEM.PURCH_ORG = posicion.OrganizacionCompras; //PURCH_ORG EKORG   Organización de compras        
                IM_PRITEM.AGREEMENT = posicion.NumeroContratoSuperior; //AGREEMENT   KONNR Número del contrato superior
                IM_PRITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; //AGMT_ITEM   KTPNR Número de posición del contrato superior
                                                                               //IM_PRITEM.INFO_REC = null; //INFO_REC    INFNR Número del registro info de compras
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
                    //FIXED_VEND = "X",
                    //PURCH_ORG = "X",
                    //AGREEMENT = "X",
                    //AGMT_ITEM = "X",
                    //INFO_REC = "X",
                    //CLOSED = "X",
                    CURRENCY = "X",
                    //CURRENCY_ISO = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X",
                    DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : "",
                });

                //Estos datos de imputacion se envian solo para materiales por que en servicio va a nivel de subposicion
                if (posicion.TipoPosicion.Codigo == "MATERIALES")
                {
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

                //Desde aca empiezan las subposiciones
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
                        IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea; //SHORT_TEXT SH_TEXT1    Texto breve

                    IM_SERVICELINE.QUANTITY = (decimal)subPosicion.Cantidad.Value; //QUANTITY MENGEV  Cantidad con signo +/ -
                    IM_SERVICELINE.QUANTITYSpecified = true;
                    IM_SERVICELINE.UOM = subPosicion.Unidad.CodigoSap; //UOM MEINS Unidad de medida base
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
                        SERVICE = (subPosicion.ServicioSolp != null) ? "X" : "",
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

                //Estos son los metodos con los que se nos fijamos si se edito algun campo de la direccion de entrega. Si no edito ninguno no 
                //hace falta enviar a SAP pero si se edito por lo menos uno tenemos que enviar todos los campos 
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
                    var datosPosicion = AutocompleteMaterialSolp(p.CodigoMaterialSap.Codigo, p.Centro.Id);
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
                var unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();
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
                    var registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(posicionAgrupada.Key.Material, posicionAgrupada.Key.Centro, posicionAgrupada.Key.GrupoDeCompras);
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
                                        var codigoUnidad = unidadMedidaSap.Where(a =>
                                        a.Tecnica == registroInfo.Unidad ||
                                        a.UM == registroInfo.Unidad ||
                                        a.Comercial == registroInfo.Unidad ||
                                        a.TextoUM == registroInfo.Unidad ||
                                        a.TextoUM2 == registroInfo.Unidad).Single().Comercial;

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
                                            UnidadId = tablaSap.Where(x => x.CodigoSap == codigoUnidad).FirstOrDefault().Id,
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
            catch (Exception e)
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
                    catch (Exception e)
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
                var peticion = new PeticionDeOferta()
                {
                    UsuarioCreador_Id = peticionDeOferta.UsuarioActual.Id,
                    Usuario = usuarios.Where(x => x.Id == peticionDeOferta.UsuarioActual.Id).FirstOrDefault(),
                    FechaCreacion = DateTime.Now,
                    Solp_Id = peticionDeOferta.SolpId,
                    Observaciones = peticionDeOferta.Observacion ?? "",
                    Posiciones = posicionesPeticion,
                    PlazoDeOferta = fechaOferta ?? posiciones.OrderByDescending(x => x.FechaEntregaServicio).Select(x => x.FechaEntregaServicio).FirstOrDefault().Value,
                    Usuarios = usuarios.Where(x => peticionDeOferta.UsuarioIds.Contains(x.Id)).Select(a => new PeticionDeOfertaUsuario { Usuario_Id = a.Id }).ToList(),
                    RegistroInfo = peticionDeOferta.RegistroInfo
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
                    EnviarMailPeticionDeOferta(peticion, peticion.Usuarios.ToList());
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception e)
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
            var middleFileName = peticion.Solp.NroSolp == null ? (peticion.Solp.Pliego.NombreObra == null ? "xxxx" : peticion.Solp.Pliego.NombreObra) : peticion.Solp.NroSolp;
            var pdfFilename = $"Solp-{middleFileName}-pliego-{DateTime.Now.ToString("yyyyMMdd")}.pdf";

            //invento registro con id de archivo 0 para bajar el pliego
            legajo.Add(new LegajoDto
            {
                ArchivoId = 0,
                Observacion = pdfFilename,
                PeticionDeOfertaId = peticionDeOfertaId,
                SolpId = peticion.Solp_Id,
                Fecha = peticion.Solp.FechaCreacion,
                FechaFormateado = peticion.Solp.FechaCreacion.ToString("dd/MM/yyyy")
            });

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
                            FechaFormateado = peticion.Solp.FechaCreacion.ToString("dd/MM/yyyy")
                        });
                    }
                }
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
                    FechaFormateado = item.Fecha.ToString("dd/MM/yyyy")
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
                        FechaFormateado = peticion.FechaCreacion.ToString("dd/MM/yyyy")
                    });
                }
            }

            //circular
            var peticionDeOfertaUsuarios_Id = peticion.Usuarios.Where(u => peticiondeOfertaUsuarioId == null || u.Id == peticiondeOfertaUsuarioId).Select(u => u.Id).ToList();
            var circulares = repositorio.Listar<Circular>(x => x.PeticionDeOfertaUsuarios.Any(a => peticionDeOfertaUsuarios_Id.Contains(a.PeticionDeOfertaUsuario_Id)));

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
                        Leido = !noLeido
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
                    Leido = !noLeido
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
                            Leido = !noLeido
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
                            Leido = !noLeido
                        });
                    }
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
            foreach (var cierre in circulares)
            {
                legajo.Add(new LegajoDto
                {
                    ArchivoId = null,
                    Observacion = "Cierre Cotizacion: " + cierre.Observaciones,
                    PeticionDeOfertaId = peticionDeOfertaId,
                    SolpId = peticion.Solp_Id,
                    Fecha = cierre.FechaCreacion,
                    FechaFormateado = cierre.FechaCreacion.ToString("dd/MM/yyyy")
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
            catch (Exception ex)
            {
                throw ex;
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
                var centroPlanta = repositorio.Obtener<TablaSap>(x => x.CodigoSap == posicion.Centro.CodigoSap);
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
            catch (Exception e)
            {
                throw;
            }
        }

        private void EnviarMailPeticionDeOferta(PeticionDeOferta peticion, List<PeticionDeOfertaUsuario> usuarios)
        {
            var archs = ObtenerArchivosPeticionDeOferta(peticion);
            var copia = new List<string> { peticion.Usuario.Mail };
            if (!string.IsNullOrEmpty(peticion.Solp?.UsuarioCreacion?.Mail))
            {
                copia.Add(peticion.Solp.UsuarioCreacion.Mail);
            }
            var asunto = "";
            
            foreach (var prov in usuarios)
            {
                asunto = "";
                var enviarA = new List<string> { prov.Usuario.Mail };
                asunto += $"MOA - Pedido de Oferta {peticion.Id}: {peticion.Solp.Pliego.NombreObra}";
                if (peticion.Posiciones.Select(x => x.SolpPosicion).Where(x => x.TipoPosicion_Id != null).FirstOrDefault().TipoPosicion.Codigo == "MATERIALES")
                {
                    var pdf = GenerarPDFPeticionDeOferta(peticion, prov.Usuario.ObtenerCodigoProveedor());
                    if (archs.ContainsKey("Peticion de Oferta.pdf"))
                        archs.Remove("Peticion de Oferta.pdf");
                    archs.Add("Peticion de Oferta.pdf", pdf);
                }
                var cuerpo = CuerpoMailPeticionDeOferta(peticion);
                emailService.EnviarMail(enviarA, asunto, "", copia, cuerpo, null, null, null, null, archs);
            }
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

        private AlternateView CuerpoMailPeticionDeOferta(PeticionDeOferta peticion)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la nueva PO {peticion.Id} generada con Molinos Agro S.A <br />";
            if (!string.IsNullOrEmpty(peticion.Observaciones))
            {
                string observacionesFormatted = peticion.Observaciones.Replace("\n", "<br />");

                htmlBody += $"<br />Observaciones: {observacionesFormatted} <br /><br /><br />";
            }

            if (peticion.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() == "SERVICIO" && peticion.Solp.TipoSolp.Codigo != "SIN_PLIEGO")
            {
                var downloadLinkUrl = ConfigurationManager.AppSettings["ida:RedirectUri"] + "/api/compras/DescargarPliegoDesdeLink?solpId=" + peticion.Solp.Id + "&token=" + peticion.Solp.EmailLinkToken;

                htmlBody += "<p" +
                           "style = 'line-height: 24px; font-size: 16px; margin: 0;'" +
                           "align = 'center' >" +
                           " Para descargar el legajo haga clic en el siguiente enlace: " +
                           $"<a href = '{downloadLinkUrl}' download rel='noopener noreferrer'>" +
                           "Descargar Legajo" +
                           "</a></p> <br /><br />";
            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
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
            return new Pdf { data = pdf, name = "PO" + po.Usuario.ObtenerProveedor().CUIT + ".pdf" };
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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception e)
            {
                throw;
            }
        }

        private void EnviarMailOrdenCompra(Adjudicacion adjudicacion, string mensaje = "")
        {
            try
            {
                Logger.Log.Info($"EnviarMailOrdenCompra numero{adjudicacion.Id}");
                Logger.Log.Info($"copia mail comprador {adjudicacion.Usuario.Mail}");
                Logger.Log.Info($"mail al proveedor adjudicado {adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail}");
                Logger.Log.Info($"Nueva OC creada - {adjudicacion.NumeroOrdenDeCompra}");
                Logger.Log.Info($"fecha {DateTime.Now}");

                var copia = new List<string> { adjudicacion.Usuario.Mail };
                if (!string.IsNullOrEmpty(adjudicacion.Solp?.UsuarioCreacion?.Mail))
                {
                    copia.Add(adjudicacion.Solp.UsuarioCreacion.Mail);
                    Log.Info($"copia mail solicitante {adjudicacion.Solp.UsuarioCreacion.Mail}");
                }
                var asunto = "";
               

                var enviarA = new List<string> { adjudicacion.Cotizacion.PeticionDeOfertaUsuario.Usuario.Mail };
                asunto += $"Nueva OC creada - {adjudicacion.NumeroOrdenDeCompra} - {adjudicacion.Usuario.ObtenerRazonSocial()}";
                var pdf = GenerarPDFOrdenCompra(adjudicacion, adjudicacion.Usuario.ObtenerCodigoProveedor());

                emailService.EnviarMail(enviarA, asunto, "", copia, CuerpoMailOrdenCompra(adjudicacion, mensaje), pdf, "Orden de Compra.pdf");
            }
            catch (Exception e)
            {
                Logger.Log.Info($"Error al enviar mail {adjudicacion.Id}  NumeroOrdenDeCompra {adjudicacion.NumeroOrdenDeCompra}");
                Logger.Log.Error(e);
            }
        }

        private AlternateView CuerpoMailOrdenCompra(Adjudicacion adjudicacion, string mensaje)
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/header/logo_.png");
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
                    Archivos = c.Archivos/*.Where(x => x.FileKey == FileKeys.AdjuntoCotizacionRevisionTecnica)*/.Select(archivo => new LegajoDto
                    {

                        ArchivoId = archivo.Id,
                        Observacion = archivo.ObtenerNombre(archivo.Ruta),
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
                    ObservacionNoCumple = u.ObservacionNoCumple
                };
                usuarios.Add(usuario);
            }
            peticion.Usuarios = usuarios;
            peticion.TipoPosicionCodigo = peticionEntidad.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault();
            peticion.Id = peticionEntidad.Id;
            peticion.PlazoDeOfertaEstado = peticionEntidad.PlazoDeOferta > DateTime.Now.Date ? "Abierto" : "Cerrado";

            var fechaEntrega = peticionEntidad.Posiciones.Select(x => x.SolpPosicion).OrderByDescending(x => x.FechaEntregaServicio).FirstOrDefault()?.FechaEntregaServicio;
            peticion.FechaEntregaFormateado = fechaEntrega != null ? fechaEntrega.Value.ToString("yyyy-MM-dd") : "";
            return peticion;
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
                    proveedor.PropuestaTecnicaAprobada = null;
                    proveedor.PropuestaTecnicaFecha = null;
                    proveedor.PropuestaTecnicaUsuario_Id = null;
                    proveedor.ObservacionNoCumple = "";

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
                EnviarMailCircular(circular);
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
            if (!string.IsNullOrEmpty(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Usuario?.Mail))
            {
                copia.Add(circular.PeticionDeOfertaUsuarios?.First().PeticionDeOfertaUsuario?.PeticionDeOferta?.Usuario?.Mail);
            }
            foreach (var prov in circular.PeticionDeOfertaUsuarios)
            {
                var enviarA = new List<string> { prov.PeticionDeOfertaUsuario.Usuario.Mail };
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
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/header/logo_.png");
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

                var peticion = repositorio.Obtener<PeticionDeOferta>(x => x.Id == peticionId);
                var nuevosUsuarios = usuarios.Where(x => usuariosId.Contains(x.Id)).Select(a => new PeticionDeOfertaUsuario { Usuario_Id = a.Id, PeticionDeOferta_Id = peticion.Id, Usuario = usuarios.Where(y => y.Id == a.Id).FirstOrDefault(), PeticionDeOferta = peticion }).ToList();
                peticion.Usuarios = nuevosUsuarios;
                repositorio.GuardarCambios();
                respuestaGuardarSOLP.IdEntidad = peticion.Id;
                EnviarMailPeticionDeOferta(peticion, nuevosUsuarios);
                return respuestaGuardarSOLP;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private RespuestaCrearOrdenDeCompra CrearOrdenDeCompra(Adjudicacion AdjudicacionEntity)
        {
            var respuesta = new RespuestaCrearOrdenDeCompra();
            CrearPedidoConsumerMOAResponse resultadoCrearPedido = new CrearPedidoConsumerMOAResponse();
            if (AdjudicacionEntity.Cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Adicional == true)
            {
                resultadoCrearPedido = modificarOrdenDeCompraConsumerMOA.Request(AdjudicacionEntity);
            }
            else
            {
                resultadoCrearPedido = crearPedidoConsumerMOA.Request(AdjudicacionEntity);
            }

            respuesta.Errores = new List<string>();
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

        public string DescargarAdjuntosCotizacion(int idCotizacion, string pathBase)
        {
            var cotizacion = repositorio.Obtener<Cotizacion>(idCotizacion);

            var zipFilename = $"Cotizacion-{cotizacion.Id}-{cotizacion.FechaCreacion.ToString("yyyyMMdd")}.zip";
            var filePath = $"{pathBase}/{zipFilename}";

            using (FileStream zipToOpen = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (ZipArchive archivo = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    //peticion de oferta
                    if (cotizacion.Archivos != null)
                    {
                        foreach (var archivoSubido in cotizacion.Archivos)
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

        public RespuestaGuardarSOLP GrabarRevisionTecnica(List<PeticionDeOfertaUsarioDto> revision, int usuarioId)
        {
            RespuestaGuardarSOLP respuesta = new RespuestaGuardarSOLP();
            var ids = revision.Select(a => a.Id);
            var peticiones = repositorio.Listar<PeticionDeOfertaUsuario>(a => ids.Contains(a.Id));
            foreach (var peticion in peticiones)
            {
                var data = revision.Where(a => a.Id == peticion.Id).Single();

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
                var esModificar = false;
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
                                }).ToList() : null

                            }).ToList() : null,
                            UsuarioCreador = usuario,
                            FechaCreacion = DateTime.Now
                        };

                        if (cotizacionDto.RespetaServicios == true && cotizacionDto.RespetaMateriales == true)
                        {
                            cotizacion.PeticionDeOfertaUsuario.PropuestaTecnicaAprobada = true;
                        }
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
                    esModificar = cotizacion.CotizacionEstado_Id == (int)CotizacionEstadoEnum.Cotizado;

                    if (cotizacionDto.RespetaServicios == true && cotizacionDto.RespetaMateriales == true)
                    {
                        cotizacion.PeticionDeOfertaUsuario.PropuestaTecnicaAprobada = true;
                    }
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
                    && cotizacion.CotizacionPosiciones.FirstOrDefault()
                    .PeticionDeOfertaSolpPosicion.SolpPosicion.TipoPosicion.Codigo == "MATERIALES")
                {
                    var registros = CrearRegistroInfoDto(cotizacion);
                    var respuesta = agregarRegistroInfoConsumerMOA.AgregarRegistroInfo(registros, esModificar);
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
                respuestaGuardarSOLP.IdEntidad = cotizacion.Id;
                repositorio.GuardarCambios();
                return respuestaGuardarSOLP;

            }
            catch (Exception e)
            {
                throw;
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

                    if (cotizacionDto.CotizacionSubposiciones.Where(x => x.CotizacionSubPosicionId == 0).Count() > 0)
                    {
                        foreach (var sub in cotizacionDto.CotizacionSubposiciones.Where(x => x.CotizacionSubPosicionId == 0 &&
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
                    PeticionDeOfertaSolpPosicion_Id = x.PeticionDeOfertaSolpPosicionId,
                    Moneda = x.MonedaId > 0 ? info.Where(moneda => moneda.Id == x.MonedaId).FirstOrDefault() : null,
                    UnidadDeMedida = x.UnidadDeMedidaId > 0 ? info.Where(unidad => unidad.Id == x.UnidadDeMedidaId).FirstOrDefault() : null,
                    NoDisponible = x.NoDisponible,
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
            var cotizacionesHoraNuevo = new List<CotizacionHora>();

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
                        cotizacionesHoraNuevo.Add(cotiH);
                    }
                }
                repositorio.AgregarTodos(cotizacionesHoraNuevo);
            }
        }

        private ObtenerTipoCambioConsumerMOAResponse ObtenerTipoCambio(int MonedaOrigen_Id, int MonedaDestino_Id, DateTime Fecha)
        {
            var origen = repositorio.Obtener<TablaSap>(MonedaOrigen_Id);
            var destino = repositorio.Obtener<TablaSap>(MonedaDestino_Id);
            ObtenerTipoCambioConsumerMOAResponse result = obtenerTipoCambioConsumerMOA.Request(Fecha.ToString("yyyy-MM-dd"), destino.Codigo, origen.Codigo);

            return result;
        }

        public void EnviarMailCotizacion(Cotizacion cotizacion)
        {
            var peticion = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta;
            var asunto = "";
            var enviarA = new List<string> { peticion.Usuario.Mail };
            if (!string.IsNullOrEmpty(peticion.Solp?.UsuarioCreacion?.Mail))
            {
                enviarA.Add(peticion.Solp.UsuarioCreacion.Mail);
            }
            asunto += "NUEVA cotización creada - SOLP " + peticion.Solp.NroSolp;
            emailService.EnviarMail(enviarA, asunto, "", null, CuerpoMailCotizacion(cotizacion), null, null, null, null);
        }


        public void EnviarMailAvisoDeErrorRegistroInfo(Cotizacion cotizacion)
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
            var proveedor = cotizacion.UsuarioCreador.ObtenerProveedor();
            string htmlBody = "";
            htmlBody += $"En el presente mail, se informa la cotización realizada para SOLP " +
                $"{cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.NroSolp} y la PO {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id} generada por el proveedor {proveedor.RazonSocial} ({proveedor.CUIT}) <br /> <br/>";

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
            catch (Exception e)
            {
                throw;
            }
        }

        public RespuestaCrearOrdenDeCompra GrabarAdjudicacion(AdjudicacionDto adjudicacionDto, int usuarioActualId, string mensaje = "")
        {
            try
            {
                var respuestaGuardarSOLP = new RespuestaCrearOrdenDeCompra();
                var usuario = repositorio.Obtener<Usuario>(usuarioActualId);
                var cotizacion = repositorio.Obtener<Cotizacion>(adjudicacionDto.Cotizacion_Id);
                var tablasap = repositorio.Listar<TablaSap>(x => x.Tabla == TablasSap.Moneda || x.Tabla == TablasSap.Unidad);

                adjudicacionDto.Moneda_Id = tablasap.Where(moneda => moneda.CodigoSap == "ARP").FirstOrDefault().Id;
                var adjudicacion = new Adjudicacion()
                {
                    Cotizacion_Id = adjudicacionDto.Cotizacion_Id,
                    Moneda_Id = adjudicacionDto.Moneda_Id,
                    Moneda = tablasap.FirstOrDefault(moneda => moneda.Id == adjudicacionDto.Moneda_Id),
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
                    Posiciones = adjudicacionDto.AdjudicacionPosiciones.Select(x => new AdjudicacionPosicion
                    {
                        Cantidad = x.Cantidad,
                        CotizacionPosicion_Id = x.CotizacionPosicion_Id,
                        CotizacionPosicion = cotizacion.CotizacionPosiciones.Where(y => y.Id == x.CotizacionPosicion_Id).FirstOrDefault(),
                        Posicion = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones
                          .Where(y => y.Id == x.SolpPosicion_Id).FirstOrDefault(),
                        SolpPosicion_Id = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Solp.Posiciones
                          .Where(y => y.Id == x.SolpPosicion_Id).FirstOrDefault().Id
                    }).ToList(),
                };

                repositorio.Agregar(adjudicacion);

                respuestaGuardarSOLP = CrearOrdenDeCompra(adjudicacion);

                if (respuestaGuardarSOLP.Errores == null || respuestaGuardarSOLP.Errores.Count == 0)
                {
                    adjudicacion.NumeroOrdenDeCompra = respuestaGuardarSOLP.NumeroPedido;
                    repositorio.GuardarCambios();
                    //EnviarMailOrdenCompra(adjudicacion, mensaje);
                }

                return respuestaGuardarSOLP;
            }
            catch (Exception e)
            {
                throw;
            }
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
            var listaResultado = respuestaSAP.Select(adjudicacion => new AdjudicacionDto()
            {
                Id = 0,
                Solp_Id = solpId,
                TipoPosicionCodigo = adjudicacion.Cabecera.Tipo,
                NumeroOrdenDeCompra = adjudicacion.Cabecera.OrdenDeCompra,
                FechaCreacion = adjudicacion.Cabecera.FechaCreacion,
                Proveedor = adjudicacion.Cabecera.RazonSocialProveedor,
                MonedaDescripcion = adjudicacion.Cabecera.Moneda,
                PrecioFinal = adjudicacion.Cabecera.MontoTotal
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
            var adjudicar = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraAdjudicacion(nroOC);
            var monedaPesos = repositorio.Obtener<TablaSap>(a => a.Tabla == "Moneda" && a.CodigoSap == "ARP");
            if (adjudicar.Moneda_Id != monedaPesos.Id)
            {
                var monedaAdjudicacion = repositorio.Obtener<TablaSap>(a => a.Tabla == "Moneda" && a.Id == adjudicar.Moneda_Id);
                var tipoCambio = ObtenerTipoCambio(monedaAdjudicacion.Id, monedaPesos.Id, adjudicar.FechaCreacion);
                adjudicar.PrecioFinal = adjudicar.PrecioFinal * tipoCambio.TipoCambio;
            }
            return adjudicar;
        }

        public void CrearCotizacionConTrabajoYaHecho(Solp solp, bool enviarMail = true)
        {
            try
            {
                RespuestaGuardarSOLP respuestaCotizacion;
                Cotizacion cotizacionNueva;
                //Crear Peticion 
                var usuariosIds = new List<int>();
                usuariosIds.Add(solp.ProveedorAsignado_Id.Value);
                PeticionDeOferta peticionEntidad = CrearPeticionAutomatica(solp, usuariosIds);
                //Crear Cotizacion
                CrearCotizacionAutomatica(solp, enviarMail, peticionEntidad, out respuestaCotizacion, out cotizacionNueva, null);
            }
            catch (Exception e)
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
                            SolpPosicion_Id = x.PeticionDeOfertaSolpPosicion.SolpPosicion_Id
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

                    };
                    string mensaje = "Orden de compra generada a partir de las órdenes: " + string.Join(", ", registroInfo.Select(a => a.NumeroOrdenDeCompra));

                    var resultado = GrabarAdjudicacion(adjudicacion, usuarioActual, mensaje);
                    return resultado;
                }
                return respuestaGuardarSOLP;
            }
            catch (Exception e)
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
            var peticion = new GuardarPeticionDeOfertaDto()
            {
                Observacion = "",
                PosIds = solpPosicions != null ? solpPosicions.Select(x => x.Id).ToList() : solp.Posiciones.Select(x => x.Id).ToList(),
                SolpId = solp.Id,
                UsuarioIds = usuariosIds,
                UsuarioActual = new UsuarioDto
                {
                    Id = solp.UsuarioCreacion.Id
                },
                Adjuntos = null,
                RegistroInfo = esRegistroInfo
            };
            var resultado = GrabarPeticionDeOferta(peticion, null, solp.TrabajoYaHecho != true && solp.Adicional == true, registroInfoLista);
            var peticionEntidad = repositorio.Obtener<PeticionDeOferta>(resultado.IdEntidad);

            return peticionEntidad;
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            var result = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra(nroOC);
            if (result.Error == null || string.IsNullOrEmpty(result.Error.Mensaje))
            {
                try
                {
                    ProveedorComprasDto proveedor = ObtenerProveedorCompras(result.Cabecera.CodigoProveedor);

                    result.Cabecera.RazonSocialProveedor = proveedor.RazonSocial;
                    result.Cabecera.CUITProveedor = proveedor.CUIT;
                    result.Cabecera.CodigoProveedor = proveedor.CodigoProveedor;
                    result.Cabecera.Usuario_Id = proveedor.Usuario_Id;
                }
                catch (Exception e)
                {
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

        private ProveedorComprasDto ObtenerProveedorCompras(string codigoProveedor)
        {
            var proveedorMoa = obtenerProveedorConsumerMOA.ObtenerProveedor(codigoProveedor);
            if (proveedorMoa == null)
            {
                throw new WSCustomException("No existe un proveedor con ese codigo");
            }
            List<SustitucionMOAModel.Models.FechaWS> fechas = CommonService.toDateList(DateTime.Now.AddYears(-5).ToShortDateString(), DateTime.Now.ToShortDateString());
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
            catch (Exception e)
            {
                throw;
            }
        }

        private List<RegistroInfoDto> CrearRegistroInfoDto(Cotizacion cotizacion)
        {
            var registros = new List<RegistroInfoDto>();
            foreach (var cotizacionPosicion in cotizacion.CotizacionPosiciones)
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
                    GrupoDeCompras = cotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion.GrupoCompras.Codigo
                };
                registros.Add(registro);
            }

            return registros;
        }

        public DatosUltimaSolpDto ObtenerUltimaSolp(int usuarioId) 
        {
            var ultimaSolp = repositorio.Listar<Solp>(a => a.UsuarioCreacion_Id == usuarioId).OrderByDescending(a => a.Id).FirstOrDefault();


            if (ultimaSolp == null)
                return null; 

            DatosUltimaSolpDto result = new DatosUltimaSolpDto();

            result.FiscalContrato = ultimaSolp.Pliego?.FiscalContrato;
            result.Telefono = ultimaSolp.Pliego?.Telefono;

            result.ClaseDocumento = ultimaSolp.ClaseDocumento != null ? new TablaSapDto { Id = ultimaSolp.ClaseDocumento.Id,
                                    Tabla = ultimaSolp.ClaseDocumento.Tabla,
                                    Codigo = ultimaSolp.ClaseDocumento.Codigo,
                                    CodigoSap = ultimaSolp.ClaseDocumento.CodigoSap,
                                    Descripcion = ultimaSolp.ClaseDocumento.Descripcion,
                                    IdPadre = ultimaSolp.ClaseDocumento.Padre_id
                                    }
                                    : null;

            result.GrupoCompras = ultimaSolp.Posiciones.FirstOrDefault()?.GrupoCompras != null
                                ? new TablaSapDto { Id = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Id,
                                                    Tabla = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Tabla,
                                                    Codigo = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Codigo,
                                                    CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.CodigoSap,
                                                    Descripcion = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Descripcion,
                                                    IdPadre = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Padre_id
                                }
                                : null;

            result.CuentaMayor = ultimaSolp.Posiciones.FirstOrDefault()?.CuentaMayorSap != null
                                ? new TablaSapDto { Id = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Id,
                                                    Tabla = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Tabla,
                                                    Codigo = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Codigo,
                                                    CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.CodigoSap,
                                                    Descripcion = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                                                    IdPadre = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Padre_id
                                }
                                : null;

            result.Almacen = ultimaSolp.Posiciones.FirstOrDefault()?.Almacen != null
                            ? new TablaSapDto { Id = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Id,
                                                Tabla = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Tabla,
                                                Codigo = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Codigo,
                                                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().Almacen.CodigoSap,
                                                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Descripcion,
                                                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Padre_id
                            }
                            : null;

            result.TipoPosicion = ultimaSolp.Posiciones.FirstOrDefault()?.TipoPosicion != null
                                ? new TablaGeneralDto { Id = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Id,
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
                                   Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.CodigoSap + " - " +  ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                                   IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Padre_id
                               }
                               : null;


            return result; 
        }
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