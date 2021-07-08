using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string SupervisorSector { get; set; }
        public string SupervisorTrabajo { get; set; }
        public List<VisitaObraDto> VisitasObraMasiva { get; set; }
        public bool TieneVisitaObra { get; set; }
        public bool TieneVisitaObraMasiva { get; set; }
        public bool TieneObradores { get; set; }
        public bool TieneMedioElevacion { get; set; }
        public bool TieneTecnicoSeguridad { get; set; }
        public bool TieneDescripcionTecnica { get; set; }
        public bool TieneDocumentacionTecnica { get; set; }
        public DateTime? FechaHoraLimiteConsulta { get; set; }
        public string ObservacionesGeneracion { get; set; }
        public string EspecificacionesTecnicas { get; set; }
        public int? DiasEjecucion { get; set; }
        public string ObservacionesCotizacion { get; set; }
        public List<DayOfWeek> JornadaLaboral { get; set; }
        public DateTime? JornadaLaboralDesde { get; set; } 
        public DateTime? JornadaLaboralHasta { get; set; }
        public TablaSapDto ClaseDocumento { get; set; }
        public int? ClaseDocumentoId { get; set; }
        public List<ArchivoDto> Adjuntos { get; set; }
        public string NroSolp { get; set; }
        public int? EstadoSolpSapId { get; set; }
        public int? EstadoDocumentoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TablaEstadoDto EstadoDocumento { get; set; }
        public bool VincularPliego { get; set; }
        public TablaSapDto EstadoSolpSap { get; set; }

        public List<SolpPosicionDto> Posiciones { get; set; }

        public SolpDto() {}
        public SolpDto(Solp entity) 
        {
            this.UsuarioActual = new UsuarioDto(entity.UsuarioCreacion);
            this.Id = entity.Id;
            this.NombreDeObra = entity.Pliego.NombreObra; //nombre de pedido
            this.FiscalContrato = entity.Pliego.FiscalContrato;
            this.Telefono = entity.Pliego.Telefono;
            this.Email = entity.Pliego.Email;
            this.FechaHoraEntrega = entity.Pliego.FechaHoraEntrega;
            this.SupervisorSector = entity.Pliego.SupervisorSector;
            this.SupervisorTrabajo = entity.Pliego.SupervisorTrabajo;
            this.VisitasObraMasiva = new List<VisitaObraDto>();
            this.TieneVisitaObra = entity.Pliego.TieneVisitaObra.HasValue && entity.Pliego.TieneVisitaObra.Value;
            this.TieneVisitaObraMasiva = entity.Pliego.TieneVisitaObraMasiva.HasValue && entity.Pliego.TieneVisitaObraMasiva.Value;
            this.TieneObradores = entity.Pliego.TieneObradores.HasValue && entity.Pliego.TieneObradores.Value;
            this.TieneMedioElevacion = entity.Pliego.TieneMedioElevacion.HasValue && entity.Pliego.TieneMedioElevacion.Value;
            this.TieneTecnicoSeguridad = entity.Pliego.TieneTecnicoSeguridad.HasValue && entity.Pliego.TieneTecnicoSeguridad.Value;
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
            this.Posiciones = new List<SolpPosicionDto>();


        }
    }

    public class VisitaObraDto
    {
        public string Codigo { get; set; }
        public DateTime FechaHora { get; set; }

        public VisitaObraDto() {}

        public VisitaObraDto(PliegoVisita entity) {
            this.Codigo = entity.Codigo;
            this.FechaHora = entity.FechaHora.HasValue ? entity.FechaHora.Value : DateTime.MinValue;
        }

    }

    public class SolpPosicionDto
    {
        public string Codigo { get; set; }
        public int? TipoPosicionId { get; set; }
        public int? TipoImputacionId { get; set; }
        public string TextoGenerico { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public int? PlazoEntrega { get; set; }
        public bool? EsConcluido { get; set; }
        public bool? EsFijacion { get; set; }
        public int? CentroId { get; set; }
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
        public string CodigosProveedores { get; set; }
        public int? MonedaId { get; set; }

        public TablaGeneralDto TipoPosicion { get; set; }
        public TablaGeneralDto TipoImputacion { get; set; }
        public TablaSapDto Centro { get; set; }
        public TablaSapDto Almacen { get; set; }
        public TablaSapDto GrupoCompras { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }
        public TablaSapDto Moneda { get; set; }

        public List<SolpSubposicionDto> Subposiciones { get; set; }
        public List<SolpProveedorDto> Proveedores { get; set; }

        public SolpPosicionDto() { }

        public SolpPosicionDto(SolpPosicion entity)
        {
            if(entity != null)
            {
                this.Codigo = entity.Id.ToString();
                this.TipoPosicionId = entity.TipoPosicion_Id;
                this.TipoImputacionId = entity.TipoImputacion_Id;
                this.TextoGenerico = entity.TextoGenerico;
                this.FechaEntregaServicio = entity.FechaEntregaServicio;
                this.FechaLiberacion = entity.FechaLiberacion;
                this.PlazoEntrega = entity.PlazoEntrega;
                this.EsConcluido = entity.EsConcluido;
                this.EsFijacion = entity.EsFijacion;
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
                this.GrupoArticuloId = entity.GrupoArticulo_Id;
                this.CodigosProveedores = entity.CodigosProveedores;
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
        public string Codigo { get; set; }
        public int Numero { get; set; }
        public int? CodigoServicioSapId { get; set; }
        public string Tarea { get; set; }
        public string CuentaMayor { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadId { get; set; }
        public decimal? PrecioBruto { get; set; }
        public string TipoImputacionValor { get; set; }

        public TablaSapDto CodigoServicioSap { get; set; }
        public TablaSapDto Unidad { get; set; }

        public SolpSubposicionDto() { }

        public SolpSubposicionDto(SolpSubposicion entity)
        {
            if(entity != null)
            {
                this.Codigo = entity.Id.ToString();
                this.Numero = entity.Numero;
                this.CodigoServicioSapId = entity.CodigoServicioSap_Id;
                this.Tarea = entity.Tarea;
                this.CuentaMayor = entity.CuentaMayor;
                this.Cantidad = entity.Cantidad;
                this.UnidadId = entity.Unidad_Id;
                this.PrecioBruto = entity.PrecioBruto;
                this.TipoImputacionValor = entity.CentroCosto; //se corregira luego el campo en base
                this.CodigoServicioSap = new TablaSapDto(entity.CodigoServicioSap);
                this.Unidad = new TablaSapDto(entity.Unidad);
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

        public SolpProveedorDto(){}

        public SolpProveedorDto(SolpProveedor entity)
        {
            if(entity != null)
            {
                this.Codigo = entity.Id.ToString();
                this.ProveedorId = entity.Proveedor_Id;
                this.RazonSocial = entity.RazonSocial;
                this.TipoFiltroProveedorSolpId = entity.TipoFiltroProveedorSolp_Id;
                this.Proveedor = new ProveedorDto(entity.Proveedor);
                this.TipoFiltroProveedorSolp = new TablaGeneralDto(entity.TipoFiltroProveedorSolp);
            }
        }
    }
}
