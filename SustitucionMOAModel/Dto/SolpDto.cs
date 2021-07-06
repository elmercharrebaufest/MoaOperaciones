using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class SolpDto
    {
        public UsuarioDto UsuarioActual { get; set; }
        public int? Id { get; set; }
        public string NombreDeObra { get; set; }
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
        public int? EstadoSolpSap_Id { get; set; }
        public int? EstadoDocumento_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TablaEstadoDto EstadoDocumento { get; set; }
        public bool VincularPliego { get; set; }
        public TablaSapDto EstadoSolpSap { get; set; }

        public List<SolpPosicionDto> Posiciones { get; set; }

        public SolpDto() {}
    }

    public class VisitaObraDto
    {
        public string Codigo { get; set; }
        public DateTime FechaHora { get; set; }

        public VisitaObraDto() {}
    }

    public class SolpPosicionDto
    {
        public string Codigo { get; set; }
        public int? TipoPosicion_Id { get; set; }
        public int? TipoImputacion_Id { get; set; }
        public string TextoGenerico { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public int? PlazoEntrega { get; set; }
        public bool? EsConcluido { get; set; }
        public bool? EsFijacion { get; set; }
        public int? Centro_Id { get; set; }
        public int? Almacen_Id { get; set; }
        public string NombreEntrega { get; set; }
        public string CalleEntrega { get; set; }
        public string NumeroEntrega { get; set; }
        public string CpEntrega { get; set; }
        public string PaisEntrega { get; set; }
        public int? GrupoCompras_Id { get; set; }
        public string Solicitante { get; set; }
        public string NroNecesidad { get; set; }
        public int? GrupoArticulo_Id { get; set; }
        public string CodigosProveedores { get; set; }
        public int? Moneda_Id { get; set; }

        public TablaGeneralDto TipoPosicion { get; set; }
        public TablaGeneralDto TipoImputacion { get; set; }
        public TablaSapDto Centro { get; set; }
        public TablaSapDto Almacen { get; set; }
        public TablaSapDto GrupoCompras { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }
        public TablaSapDto Moneda { get; set; }

        public List<SolpSubposicionDto> Subposiciones { get; set; }
        public List<SolpProveedorDto> Proveedores { get; set; }
    }

    public class SolpSubposicionDto
    {
        public string Codigo { get; set; }
        public int Numero { get; set; }
        public int? CodigoServicioSap_Id { get; set; }
        public string Tarea { get; set; }
        public string CuentaMayor { get; set; }
        public decimal? Cantidad { get; set; }
        public int? Unidad_Id { get; set; }
        public decimal? PrecioBruto { get; set; }
        public string TipoImputacionValor { get; set; }

        public SolpPosicionDto SolpPosicion { get; set; }
        public TablaSapDto CodigoServicioSap { get; set; }
        public TablaSapDto Unidad { get; set; }
    }

    public class SolpProveedorDto
    {
        public string Codigo { get; set; }
        public int? Proveedor_Id { get; set; }
        public string RazonSocial { get; set; }
        public int TipoFiltroProveedorSolp_Id { get; set; }

        public virtual ProveedorDto Proveedor { get; set; }
        public virtual TablaGeneralDto TipoFiltroProveedorSolp { get; set; }
    }
}
