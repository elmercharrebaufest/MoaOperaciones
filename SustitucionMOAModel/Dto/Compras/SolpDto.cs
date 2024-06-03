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
        public string SupervisorTrabajo { get; set; }
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
        public int? ClaseDocumento_Id { get; set; }
        public List<ArchivoDto> Adjuntos { get; set; }
        public string NroSolp { get; set; }
        public string NroPedido { get; set; }
        public int? EstadoSolpSap_Id { get; set; }
        public int? EstadoDocumento_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TablaEstadoDto EstadoDocumento { get; set; }
        public bool VincularPliego { get; set; }
        public TablaSapDto EstadoSolpSap { get; set; }
        public TablaGeneralDto TipoSolp { get; set; }

        public List<SolpPosicionDto> Posiciones { get; set; }
        public string PosicionesSolpId { get; set; }
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
        public int? ProveedorAsignado_Id { get; set; }
        public string ProveedorAsignado { get; set; }
        public bool VerPublicar { get; set; }
        public bool VerCircular { get; set; }
        public bool? Adicional { get; set; }
        public bool? Urgencia { get; set; }
        public string NroOrdenDeCompraAdicional { get; set; }
        public bool DeshabilitarAdicional { get; set; }
        public int? ProveedorIdAdicional { get; set; }
        public string ProveedorRazonSocialAdicional { get; set; }
        public string MonedaOC { get; set; }
        public decimal MontoTotalOC { get; set; }
        public string FechaCreacionOC { get; set; }
        public bool SolpConAdjuntos { get; set; }
        public List<LiberadorSapSolpDto> LiberadoresSapSolp { get; set; } = new List<LiberadorSapSolpDto>();
        public bool ChatSinLeer { get; set; }
        public bool TodasLasPosicionesBorradas { get; set; }
        public bool? CondEspProveedorAsignado { get; set; }
        public bool EditarCondicionesEspeciales { get; set; }
        public int? Pliego_Id { get; set; }
        public bool? THProveedorDirecto { get; set; }
        public bool? THAjustePolinomica { get; set; }
        public bool? THServicioPermanente { get; set; }
        public string TipoDeSolp { get; set; }
        public string UsuarioCreadorMail { get; set; }
        public string Periodo { get; set; }
        public int Cantidad { get; set; }
        public bool VerEditarOC { get; set; }
        public IEnumerable<int> PosicionesId { get; set; }
        public bool TieneMensajesChatInterno { get; set; }
        public bool TieneMensajesChatExterno { get; set; }
        public int NroPeticionDeOferta { get; set; }
        public bool Agrupada { get; set; }
        public string ProveedorAsignadoCuit { get; set; }
        public string ProveedorAsignadoRazonSocial { get; set; }
        public int? ProveedorAdicional_Id { get; set; }
        public IEnumerable<string> ObservacionesCotizacionLista { get; set; }

        public SolpDto() { }
        public SolpDto(Solp entity)
        {
            UsuarioActual = new UsuarioDto(entity.UsuarioCreacion);
            Id = entity.Id;
            NombreDeObra = entity.Pliego.NombreObra; //nombre de pedido
            FiscalContrato = entity.Pliego.FiscalContrato;
            Telefono = entity.Pliego.Telefono;
            Email = entity.Pliego.Email;
            FechaHoraEntrega = entity.Pliego.FechaHoraEntrega;
            SupervisorSector = entity.Pliego.SupervisorSector.Split(',').ToList();
            SupervisorTrabajo = entity.Pliego.SupervisorTrabajo;
            VisitasObraMasiva = new List<VisitaObraDto>();
            TieneVisitaObra = entity.Pliego.TieneVisitaObra.HasValue && entity.Pliego.TieneVisitaObra.Value;
            TieneVisitaObraMasiva = entity.Pliego.TieneVisitaObraMasiva.HasValue && entity.Pliego.TieneVisitaObraMasiva.Value;
            TieneObradores = entity.Pliego.TieneObradores.HasValue && entity.Pliego.TieneObradores.Value;
            TieneMedioElevacion = entity.Pliego.TieneMedioElevacion.HasValue && entity.Pliego.TieneMedioElevacion.Value;
            TieneAndamio = entity.Pliego.TieneAndamio.HasValue && entity.Pliego.TieneAndamio.Value;
            TieneTecnicoSeguridad = entity.Pliego.TieneTecnicoSeguridad.HasValue && entity.Pliego.TieneTecnicoSeguridad.Value;
            TieneGrillaPersonal = entity.Pliego.TieneGrillaPersonal.HasValue && entity.Pliego.TieneGrillaPersonal.Value;
            TieneFabricacionTallerExterno = entity.Pliego.TieneFabricacionTallerExterno.HasValue && entity.Pliego.TieneFabricacionTallerExterno.Value;
            TieneDescripcionTecnica = entity.Pliego.TieneDescripcionTecnica.HasValue && entity.Pliego.TieneDescripcionTecnica.Value;
            TieneDocumentacionTecnica = entity.Pliego.TieneDocumentacionTecnica.HasValue && entity.Pliego.TieneDocumentacionTecnica.Value;
            FechaHoraLimiteConsulta = entity.Pliego.FechaHoraLimiteConsulta;
            ObservacionesGeneracion = entity.Pliego.ObservacionesGeneracion;
            JornadaLaboral = new List<DayOfWeek>();
            //JornadaLaboralDesde = entity.Pliego.JornadaLaboralDesde;
            //JornadaLaboralHasta = entity.Pliego.JornadaLaboralHasta;
            ClaseDocumento = new TablaSapDto(entity.ClaseDocumento);
            ClaseDocumento_Id = entity.ClaseDocumento.Id;
            Adjuntos = new List<ArchivoDto>();
            NroSolp = entity.NroSolp;
            EstadoSolpSap_Id = entity.EstadoSolpSap_Id;
            EstadoDocumento_Id = entity.EstadoDocumento_Id;
            FechaCreacion = entity.FechaCreacion;
            EstadoDocumento = new TablaEstadoDto(entity.EstadoDocumento);
            //VincularPliego = entity.VincularPliego.HasValue && entity.VincularPliego.Value;
            EstadoSolpSap = new TablaSapDto(entity.EstadoSolpSap);
            TipoSolp = new TablaGeneralDto(entity.TipoSolp);
            Posiciones = new List<SolpPosicionDto>();
            TieneCondicionesGenerales = entity.Pliego.TieneCondicionesGenerales.HasValue ? entity.Pliego.TieneCondicionesGenerales : true;
            PasoCompletado = entity.PasoCompletado;
            EstadoPasos = entity.EstadoPasos;
            RevisadoPor = entity.Pliego.RevisadoPor;
            UsuarioCompras = new UsuarioComprasDto(entity.UsuarioCompras);
            TipoSolpSap = entity.TipoSolpSap;
            ProveedorAsignado_Id = entity.ProveedorAsignado_Id;
            TrabajoYaHecho = entity.TrabajoYaHecho;
            LiberadoresSapSolp = new List<LiberadorSapSolpDto>();
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
        public ProvinciaDto Provincia { get; set; }
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
        public List<TablaSapDto> UnidadesDeMedida { get; set; }
        public string NroSolp { get; set; }
        public string TipoPosicionCodigo { get; set; }
        public string CentroCodigoSap { get; set; }
        public string GrupoComprasCodigoSap { get; set; }
        public string MaterialDescripcion { get; set; }

        public SolpPosicionDto() { }

        public SolpPosicionDto(SolpPosicion entity)
        {
            if (entity != null)
            {
                Codigo = entity.Codigo;
                TipoPosicionId = entity.TipoPosicion_Id;
                TipoImputacionId = entity.TipoImputacion_Id;
                FechaEntregaServicio = entity.FechaEntregaServicio;
                FechaLiberacion = entity.FechaLiberacion;
                PlazoEntrega = entity.PlazoEntrega;
                CentroId = entity.Centro_Id;
                AlmacenId = entity.Almacen_Id;
                NombreEntrega = entity.NombreEntrega;
                CalleEntrega = entity.CalleEntrega;
                NumeroEntrega = entity.NumeroEntrega;
                CpEntrega = entity.CpEntrega;
                PaisEntrega = entity.PaisEntrega;
                GrupoComprasId = entity.GrupoCompras_Id;
                Solicitante = entity.Solicitante;
                NroNecesidad = entity.NroNecesidad;
                TextoSuministro = entity.TextoSuministro;
                Motivo = entity.Motivo;
                Modelo = entity.Modelo;
                GrupoArticuloId = entity.GrupoArticulo_Id;
                MonedaId = entity.Moneda_Id;
                TipoPosicion = new TablaGeneralDto(entity.TipoPosicion);
                TipoImputacion = new TablaGeneralDto(entity.TipoImputacion);
                Centro = new TablaSapDto(entity.Centro);
                Almacen = new TablaSapDto(entity.Almacen);
                GrupoCompras = new TablaSapDto(entity.GrupoCompras);
                GrupoArticulo = new TablaSapDto(entity.GrupoArticulo);
                Moneda = new TablaSapDto(entity.Moneda);
                Subposiciones = new List<SolpSubposicionDto>();
                Proveedores = new List<SolpProveedorDto>();
                Estado = entity.Estado;
                Indice = entity.Indice;
                EsConcluido = entity.EsConcluido;
                CodigoServicioSap = entity.ServicioSolp != null ? new ServicioSolpDto(entity.ServicioSolp) : null;
                CodigoMaterialSap = entity.MaterialSolp != null ? new MaterialSolpDto(entity.MaterialSolp) : null;
                Tarea = entity.Tarea;
                Cantidad = entity.Cantidad;
                UnidadId = entity.Unidad_Id;
                Unidad = entity.Unidad != null ? new TablaSapDto(entity.Unidad) : null;
                PrecioBruto = entity.PrecioBruto;
                CuentaMayor = entity.CuentaMayorSap != null ? new TablaSapDto(entity.CuentaMayorSap) : null;
                TipoImputacionValor = entity.TipoImputacionSap != null ? new TablaSapDto(entity.TipoImputacionSap) : null;
                Provincia = entity.ProvinciaId != null ? new ProvinciaDto(entity.Provincia) : null;

                //Contrato Marco
                NumeroContratoSuperior = entity.NumeroContratoSuperior;
                NombreProveedor = entity.NombreProveedor;
                OrganizacionCompras = entity.OrganizacionCompras;
                ProveedorFijo = entity.ProveedorFijo;
                NumeroPosicionContratoSuperior = entity.NumeroPosicionContratoSuperior;
                NumeroPedido = entity.NumeroPedido;

                if (entity.Subposiciones != null)
                {
                    foreach (var subpos in entity.Subposiciones)
                    {
                        Subposiciones.Add(new SolpSubposicionDto(subpos));
                    }
                }

                if (entity.Proveedores != null)
                {
                    foreach (var proveedor in entity.Proveedores)
                    {
                        Proveedores.Add(new SolpProveedorDto(proveedor));
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
        public bool Eliminado { get; set; }
        public decimal? ValorNeto { get; set; }


        public SolpSubposicionDto() { }

        public SolpSubposicionDto(SolpSubposicion entity)
        {
            if (entity != null)
            {
                Codigo = entity.Codigo;
                Numero = entity.Numero;
                CodigoServicioSap = entity.ServicioSolp != null ? new ServicioSolpDto(entity.ServicioSolp) : null;
                Tarea = entity.Tarea;
                CuentaMayor = entity.CuentaMayorSap != null ? new TablaSapDto(entity.CuentaMayorSap) : null;
                Cantidad = entity.Cantidad;
                UnidadId = entity.Unidad_Id;
                PrecioBruto = entity.PrecioBruto;
                TipoImputacionValor = entity.TipoImputacionSap != null ? new TablaSapDto(entity.TipoImputacionSap) : null; //se corregira luego el campo en base
                Unidad = entity.Unidad != null ? new TablaSapDto(entity.Unidad) : null;
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
                Codigo = entity.Id.ToString();
                ProveedorId = entity.Proveedor_Id;
                RazonSocial = entity.RazonSocial;
                TipoFiltroProveedorSolpId = entity.TipoFiltroProveedorSolp_Id;
                TipoFiltroProveedorSolp = new TablaGeneralDto(entity.TipoFiltroProveedorSolp);
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
        public List<string> NumerosDePedido { get; set; }
        public bool MostrarModalMoneda { get; set; }
    }

    public class SolpMailDto
    {

        public string TipoSolp { get; set; }
        public string UsuarioCreadorMail { get; set; }
        public string Cantidad { get; set; }
        public string Periodo { get; set; }
    
    }
}