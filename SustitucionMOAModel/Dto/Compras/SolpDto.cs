using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class SolpDto
    {
        public UsuarioDto UsuarioActual { get; set; }
        public int? Id { get; set; }
        public string NombreDeObra { get; set; } //nombre de pedido
        public string FiscalContrato { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime? FechaHoraEntrega { get; set; }
        public List<string> SupervisorSector { get; set; }
        public List<string> SupervisorTrabajo { get; set; }
        public List<VisitaObraDto> VisitasObraMasiva { get; set; }
        public bool TieneVisitaObra { get; set; }
        public bool TieneVisitaObraMasiva { get; set; }
        public bool TieneObradores { get; set; }
        public bool TieneMedioElevacion { get; set; }
        public bool TieneAndamio { get; set; }
        public bool TieneTecnicoSeguridad { get; set; }
        public bool TieneGrillaPersonal { get; set; }
        public bool TieneFabricacionTallerExterno { get; set; }
        public bool TieneDescripcionTecnica { get; set; }
        public bool TieneDocumentacionTecnica { get; set; }
        public DateTime? FechaHoraLimiteConsulta { get; set; }
        public string ObservacionesGeneracion { get; set; }
        public string EspecificacionesTecnicas { get; set; }
        public int? DiasEjecucion { get; set; }
        public string ObservacionesCotizacion { get; set; }
        public List<DayOfWeek> JornadaLaboral { get; set; }
        public DateTimeOffset? JornadaLaboralDesde { get; set; }
        public DateTimeOffset? JornadaLaboralHasta { get; set; }
        public TablaSapDto ClaseDocumento { get; set; }
        public int? ClaseDocumentoId { get; set; }
        public List<ArchivoDto> Adjuntos { get; set; }
        public string NroSolp { get; set; }
        public string NroPedido { get; set; }
        public int? EstadoSolpSapId { get; set; }
        public int? EstadoDocumentoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TablaEstadoDto EstadoDocumento { get; set; }
        public bool VincularPliego { get; set; }
        public TablaSapDto EstadoSolpSap { get; set; }
        public TablaGeneralDto TipoSolp { get; set; }

        public List<SolpPosicionDto> Posiciones { get; set; }
        public string Pdf { get; set; }
        public bool Finalizar { get; set; }
        public bool? TieneCondicionesGenerales { get; set; }
        public int? PasoCompletado { get; set; }
        public string EstadoPasos { get; set; }
        public bool CargaCotizacionesConArchivo { get; set; }
        public string RevisadoPor { get; set; }
        public UsuarioComprasDto UsuarioCompras { get; set; }
        public int? TipoSolpSap { get; set; }
        public bool PosicionesEstado { get; set; }
        public Guid? EmailLinkToken { get; set; }
        public int ItemPorPagina { get; set; }
        public int Pagina { get; set; }
        public int ItemsTotales { get; set; }
        public string EstadoSolpDescripcion { get; set; }
        public string SolicitanteMail { get; set; }
        public IEnumerable<SolpPosicionDto> PosicionCompras { get; set; }
        public IEnumerable<SolpPosicionDto> GrupoCompras { get; set; }
        public IEnumerable<SolpPosicionDto> CentroCompras { get; set; }
        public string GrupoCompraFormateado { get; set; }
        public string FechaCreacionFormateada { get; set; }
        public string CentroFormateado { get; set; }
        public DateTime? FechaLiberacionSap { get; set; }
        public string FechaLiberacionSapFormateada { get; set; }
        public IEnumerable<PeticionDeOfertaDto> PeticionesDeOferta { get; set; } = new List<PeticionDeOfertaDto>();
        public List<PeticionDeOfertaDto> Peticiones { get; set; } = new List<PeticionDeOfertaDto>();
        public string TipoPosicionCodigo { get; set; }
        public bool TienePeticionDeOferta { get; set; }
        public IQueryable<AdjudicacionDto> OrdenesDeCompra { get; set; }
        public List<AdjudicacionDto> OrdenesDeCompraSolicitante { get; set; }
        public bool? TrabajoYaHecho { get; set; }
        public int? ProveedorAsignadoId { get; set; }
        public string ProveedorAsignado { get; set; }
        public bool VerPublicar { get; set; }
        public bool VerCircular { get; set; }
        public bool? Adicional { get; set; }
        public string NroOrdenDeCompraAdicional { get; set; }
        public bool DeshabilitarAdicional { get; set; }
        public int? ProveedorIdAdicional { get; set; }
        public string ProveedorRazonSocialAdicional { get; set; }
        public string MonedaOC { get; set; }
        public decimal MontoTotalOC { get; set; }
        public string FechaCreacionOC { get; set; }

        public SolpDto() { }
        public SolpDto(Solp entity)
        {
            this.UsuarioActual = new UsuarioDto(entity.UsuarioCreacion);
            this.Id = entity.Id;
            this.NombreDeObra = entity.Pliego.NombreObra; //nombre de pedido
            this.FiscalContrato = entity.Pliego.FiscalContrato;
            this.Telefono = entity.Pliego.Telefono;
            this.Email = entity.Pliego.Email;
            this.FechaHoraEntrega = entity.Pliego.FechaHoraEntrega;
            this.SupervisorSector = entity.Pliego.SupervisorSector.Split(',').ToList();
            this.SupervisorTrabajo = entity.Pliego.SupervisorTrabajo.Split(',').ToList();
            this.VisitasObraMasiva = new List<VisitaObraDto>();
            this.TieneVisitaObra = entity.Pliego.TieneVisitaObra.HasValue && entity.Pliego.TieneVisitaObra.Value;
            this.TieneVisitaObraMasiva = entity.Pliego.TieneVisitaObraMasiva.HasValue && entity.Pliego.TieneVisitaObraMasiva.Value;
            this.TieneObradores = entity.Pliego.TieneObradores.HasValue && entity.Pliego.TieneObradores.Value;
            this.TieneMedioElevacion = entity.Pliego.TieneMedioElevacion.HasValue && entity.Pliego.TieneMedioElevacion.Value;
            this.TieneAndamio = entity.Pliego.TieneAndamio.HasValue && entity.Pliego.TieneAndamio.Value;
            this.TieneTecnicoSeguridad = entity.Pliego.TieneTecnicoSeguridad.HasValue && entity.Pliego.TieneTecnicoSeguridad.Value;
            this.TieneGrillaPersonal = entity.Pliego.TieneGrillaPersonal.HasValue && entity.Pliego.TieneGrillaPersonal.Value;
            this.TieneFabricacionTallerExterno = entity.Pliego.TieneFabricacionTallerExterno.HasValue && entity.Pliego.TieneFabricacionTallerExterno.Value;
            this.TieneDescripcionTecnica = entity.Pliego.TieneDescripcionTecnica.HasValue && entity.Pliego.TieneDescripcionTecnica.Value;
            this.TieneDocumentacionTecnica = entity.Pliego.TieneDocumentacionTecnica.HasValue && entity.Pliego.TieneDocumentacionTecnica.Value;
            this.FechaHoraLimiteConsulta = entity.Pliego.FechaHoraLimiteConsulta;
            this.ObservacionesGeneracion = entity.Pliego.ObservacionesGeneracion;
            this.JornadaLaboral = new List<DayOfWeek>();
            //this.JornadaLaboralDesde = entity.Pliego.JornadaLaboralDesde;
            //this.JornadaLaboralHasta = entity.Pliego.JornadaLaboralHasta;
            this.ClaseDocumento = new TablaSapDto(entity.ClaseDocumento);
            this.ClaseDocumentoId = entity.ClaseDocumento.Id;
            this.Adjuntos = new List<ArchivoDto>();
            this.NroSolp = entity.NroSolp;
            this.EstadoSolpSapId = entity.EstadoSolpSap_Id;
            this.EstadoDocumentoId = entity.EstadoDocumento_Id;
            this.FechaCreacion = entity.FechaCreacion;
            this.EstadoDocumento = new TablaEstadoDto(entity.EstadoDocumento);
            //this.VincularPliego = entity.VincularPliego.HasValue && entity.VincularPliego.Value;
            this.EstadoSolpSap = new TablaSapDto(entity.EstadoSolpSap);
            this.TipoSolp = new TablaGeneralDto(entity.TipoSolp);
            this.Posiciones = new List<SolpPosicionDto>();
            this.TieneCondicionesGenerales = entity.Pliego.TieneCondicionesGenerales.HasValue ? entity.Pliego.TieneCondicionesGenerales : true;
            this.PasoCompletado = entity.PasoCompletado;
            this.EstadoPasos = entity.EstadoPasos;
            this.RevisadoPor = entity.Pliego.RevisadoPor;
            this.UsuarioCompras = new UsuarioComprasDto(entity.UsuarioCompras);
            this.TipoSolpSap = entity.TipoSolpSap;
            this.ProveedorAsignadoId = entity.ProveedorAsignado_Id;
            this.TrabajoYaHecho = entity.TrabajoYaHecho;
        }
    }

    public class VisitaObraDto
    {
        public string Codigo { get; set; }
        public DateTime FechaHora { get; set; }
        public VisitaObraDto() { }
        public VisitaObraDto(PliegoVisita entity)
        {
            Codigo = entity.Codigo;
            FechaHora = entity.FechaHora ?? DateTime.MinValue;
        }

    }


    public class RespuestaGuardarSOLP
    {
        public SolpDto Solp { get; set; }
        //public enum TipoSolp { get; set; } (Para diferenciar tipo de solp a la hora de mandar a sap)
        public List<string> Errores { get; set; }
        public string Mensaje { get; set; }
        public int IdEntidad { get; set; }
    }


    public class SolpPosicionDto
    {
        public string Codigo { get; set; }
        public int? TipoPosicionId { get; set; }
        public int? TipoImputacionId { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public int? PlazoEntrega { get; set; }
        public int? CentroId { get; set; }
        public string CentroCodigo { get; set; }
        public int? AlmacenId { get; set; }
        public string NombreEntrega { get; set; }
        public string CalleEntrega { get; set; }
        public string NumeroEntrega { get; set; }
        public string CpEntrega { get; set; }
        public string PaisEntrega { get; set; }
        public int? GrupoComprasId { get; set; }
        public string Solicitante { get; set; }
        public string NroNecesidad { get; set; }
        public int? GrupoArticuloId { get; set; }
        public string TextoSuministro { get; set; }
        public string Motivo { get; set; }
        public string Modelo { get; set; }
        public int? MonedaId { get; set; }
        public bool Estado { get; set; }
        public int? Indice { get; set; }
        public int? CodigoServicioSapId { get; set; }
        public int? CodigoMaterialSapId { get; set; }

        public string Tarea { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadId { get; set; }
        public decimal? PrecioBruto { get; set; }

        public bool? EsConcluido { get; set; }

        //Contrato Marco
        public string NumeroContratoSuperior { get; set; }
        public string NumeroPosicionContratoSuperior { get; set; }
        public string NombreProveedor { get; set; }
        public string ProveedorFijo { get; set; }
        public string OrganizacionCompras { get; set; }
        public string NumeroPedido { get; set; }


        public TablaSapDto Unidad { get; set; }
        public TablaGeneralDto TipoPosicion { get; set; }
        public TablaGeneralDto TipoImputacion { get; set; }
        public TablaSapDto Centro { get; set; }
        public TablaSapDto Almacen { get; set; }
        public TablaSapDto GrupoCompras { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }
        public TablaSapDto Moneda { get; set; }
        public ServicioSolpDto CodigoServicioSap { get; set; }
        public MaterialSolpDto CodigoMaterialSap { get; set; }

        public TablaSapDto CuentaMayor { get; set; }
        public TablaSapDto TipoImputacionValor { get; set; }

        public List<SolpSubposicionDto> Subposiciones { get; set; }
        public List<SolpProveedorDto> Proveedores { get; set; }
        public ProvinciaDTO Provincia { get; set; }
        public string GrupoComprasDescripcion { get; set; }
        public string CentroComprasDescripcion { get; set; }
        public bool TieneCotizacion { get; set; }
        public string AlmacenComprasDescripcion { get; set; }
        public string UnidadComprasDescripcion { get; set; }
        public string MonedaSolpDescripcion { get; set; }
        public IEnumerable<SolpProveedorDto> ProveedoresCompras { get; set; }
        public IEnumerable<SolpSubposicionDto> SubposicionesCompras { get; set; }
        public string MaterialComprasCodigo { get; set; }
        public int Id { get; set; }
        public DateTime? FechaOferta { get; set; }
        public CotizacionPosicionDto CotizacionPosicion { get; set; }
        public decimal? CantidadPendiente { get; set; }
        public decimal? CantidadAdjudicacion { get; set; }
        public bool AdjudicacionCompleta { get; set; }
        public AdjudicacionPosicion AdjudicacionPosicion { get; set; }
        public string SolpTipo { get; set; }
        public int Solp_Id { get; set; }
        public decimal? CantidadAdjudicada { get; set; }
        public string GrupoComprasCodigo { get; set; }

        public SolpPosicionDto() { }

        public SolpPosicionDto(SolpPosicion entity)
        {
            if (entity != null)
            {
                this.Codigo = entity.Codigo;
                this.TipoPosicionId = entity.TipoPosicion_Id;
                this.TipoImputacionId = entity.TipoImputacion_Id;
                this.FechaEntregaServicio = entity.FechaEntregaServicio;
                this.FechaLiberacion = entity.FechaLiberacion;
                this.PlazoEntrega = entity.PlazoEntrega;
                this.CentroId = entity.Centro_Id;
                this.AlmacenId = entity.Almacen_Id;
                this.NombreEntrega = entity.NombreEntrega;
                this.CalleEntrega = entity.CalleEntrega;
                this.NumeroEntrega = entity.NumeroEntrega;
                this.CpEntrega = entity.CpEntrega;
                this.PaisEntrega = entity.PaisEntrega;
                this.GrupoComprasId = entity.GrupoCompras_Id;
                this.Solicitante = entity.Solicitante;
                this.NroNecesidad = entity.NroNecesidad;
                this.TextoSuministro = entity.TextoSuministro;
                this.Motivo = entity.Motivo;
                this.Modelo = entity.Modelo;
                this.GrupoArticuloId = entity.GrupoArticulo_Id;
                this.MonedaId = entity.Moneda_Id;
                this.TipoPosicion = new TablaGeneralDto(entity.TipoPosicion);
                this.TipoImputacion = new TablaGeneralDto(entity.TipoImputacion);
                this.Centro = new TablaSapDto(entity.Centro);
                this.Almacen = new TablaSapDto(entity.Almacen);
                this.GrupoCompras = new TablaSapDto(entity.GrupoCompras);
                this.GrupoArticulo = new TablaSapDto(entity.GrupoArticulo);
                this.Moneda = new TablaSapDto(entity.Moneda);
                this.Subposiciones = new List<SolpSubposicionDto>();
                this.Proveedores = new List<SolpProveedorDto>();
                this.Estado = entity.Estado;
                this.Indice = entity.Indice;
                this.EsConcluido = entity.EsConcluido;

                this.CodigoServicioSap = entity.ServicioSolp != null ? new ServicioSolpDto(entity.ServicioSolp) : null;

                this.CodigoMaterialSap = entity.MaterialSolp != null ? new MaterialSolpDto(entity.MaterialSolp) : null;

                this.Tarea = entity.Tarea;
                this.Cantidad = entity.Cantidad;
                this.UnidadId = entity.Unidad_Id;
                this.Unidad = entity.Unidad != null ? new TablaSapDto(entity.Unidad) : null;
                this.PrecioBruto = entity.PrecioBruto;

                this.CuentaMayor = entity.CuentaMayorSap != null ? new TablaSapDto(entity.CuentaMayorSap) : null;
                this.TipoImputacionValor = entity.TipoImputacionSap != null ? new TablaSapDto(entity.TipoImputacionSap) : null;

                this.Provincia = entity.ProvinciaId != null ? new ProvinciaDTO(entity.Provincia) : null;

                //Contrato Marco
                this.NumeroContratoSuperior = entity.NumeroContratoSuperior;
                this.NombreProveedor = entity.NombreProveedor;
                this.OrganizacionCompras = entity.OrganizacionCompras;
                this.ProveedorFijo = entity.ProveedorFijo;
                this.NumeroPosicionContratoSuperior = entity.NumeroPosicionContratoSuperior;
                this.NumeroPedido = entity.NumeroPedido;

                if (entity.Subposiciones != null)
                {
                    foreach (var subpos in entity.Subposiciones)
                    {
                        this.Subposiciones.Add(new SolpSubposicionDto(subpos));
                    }
                }

                if (entity.Proveedores != null)
                {
                    foreach (var proveedor in entity.Proveedores)
                    {
                        this.Proveedores.Add(new SolpProveedorDto(proveedor));
                    }
                }
            }
        }
    }

    public class SolpSubposicionDto
    {
        public int Indice { get; set; }
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int Numero { get; set; }
        public int? CodigoServicioSapId { get; set; }
        public string Tarea { get; set; }
        public TablaSapDto CuentaMayor { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadId { get; set; }
        public decimal? PrecioBruto { get; set; }
        public TablaSapDto TipoImputacionValor { get; set; }

        public ServicioSolpDto CodigoServicioSap { get; set; }

        public TablaSapDto Unidad { get; set; }
        public string UnidadComprasDescripcion { get; set; }
        public string UnidadDescripcion { get; set; }
        public decimal? CantidadCotizacion { get; set; }
        public string UnidadCotizacionDescripcion { get; set; }
        public int? UnidadCotizacionId { get; set; }
        public string MonedaCotizacionDescripcion { get; set; }
        public int? MonedaCotizacionId { get; set; }
        public decimal PrecioSubPosicion { get; set; }
        public decimal PrecioTotalSubPosicion { get; set; }
        public string MonedaCotizacionCodigo { get; set; }
        public int? CotizacionSubPosicionId { get; set; }
        public int? CodigoSolpServicioSap { get; set; }
        public int? ServicioSolpCodigo { get; set; }
        public TablaSapDto MonedaCotizacion { get; set; }
        public TablaSapDto UnidadMedidaCotizacion { get; set; }
        public int? CodigoSolp { get; set; }

        public SolpSubposicionDto() { }

        public SolpSubposicionDto(SolpSubposicion entity)
        {
            if (entity != null)
            {
                this.Codigo = entity.Codigo;
                this.Numero = entity.Numero;
                this.CodigoServicioSap = entity.ServicioSolp != null ? new ServicioSolpDto(entity.ServicioSolp) : null;
                this.Tarea = entity.Tarea;
                this.CuentaMayor = entity.CuentaMayorSap != null ? new TablaSapDto(entity.CuentaMayorSap) : null;
                this.Cantidad = entity.Cantidad;
                this.UnidadId = entity.Unidad_Id;
                this.PrecioBruto = entity.PrecioBruto;
                this.TipoImputacionValor = entity.TipoImputacionSap != null ? new TablaSapDto(entity.TipoImputacionSap) : null; //se corregira luego el campo en base
                this.Unidad = entity.Unidad != null ? new TablaSapDto(entity.Unidad) : null;
            }
        }
    }

    public class SolpProveedorDto
    {
        public string Codigo { get; set; }
        public int? ProveedorId { get; set; }
        public string RazonSocial { get; set; }
        public int TipoFiltroProveedorSolpId { get; set; }

        public virtual ProveedorDto Proveedor { get; set; }
        public virtual TablaGeneralDto TipoFiltroProveedorSolp { get; set; }
        public int SolpPosicionId { get; set; }
        public string TipoFiltroProveedorSolpCodigo { get; set; }

        public SolpProveedorDto() { }

        public SolpProveedorDto(SolpProveedor entity)
        {
            if (entity != null)
            {
                this.Codigo = entity.Id.ToString();
                this.ProveedorId = entity.Proveedor_Id;
                this.RazonSocial = entity.RazonSocial;
                this.TipoFiltroProveedorSolpId = entity.TipoFiltroProveedorSolp_Id;
                this.TipoFiltroProveedorSolp = new TablaGeneralDto(entity.TipoFiltroProveedorSolp);
            }
        }
    }

    public class RespuestaCrearOrdenDeCompra
    {
        public List<string> Errores { get; set; }
        public string Mensaje { get; set; }
        public int IdEntidad { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroSolp { get; set; }
        public string Proveedor { get; set; }
    }
}