using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class CotizacionDto
    {
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public int CotizacionEstado_Id { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public int Revision { get; set; }
        public string CotizacionEstadoDescripcion { get; set; }
        public List<LegajoDto> Archivos { get; set; } = new List<LegajoDto>();
        public bool TieneObservacionTecnica { get; set; }
        public List<CotizacionPosicionDto> CotizacionPosiciones { get; set; } = new List<CotizacionPosicionDto>();
        public bool TieneAdjuntos { get; set; }
        public IList<ArchivoDto> ArchivosCotizacion { get; set; }
        public string FechaCreacionFormateada { get; set; }
        public decimal TotalGlobal { get; set; }
        public decimal TotalGlobalSubPos { get; set; }
        public int TotalPesos { get; set; }
        public List<CotizacionHorasDto> CotizacionesHoras { get; set; }
        public string RespetaMaterialesColor { get; set; }
        public string RespetaMaterialesDescripcion { get; set; }
        public bool TieneObservacionEconomica { get; set; }
        public decimal? PorcentajeDeHoras { get; set; }
    }

    public class CotizacionPosicionDto
    {
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int PeticionDeOfertaSolpPosicion_Id { get; set; }
        public decimal Cantidad { get; set; }
        public int UnidadDeMedida_Id { get; set; }
        public int Moneda_Id { get; set; }
        public decimal Precio { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public string FechaDeEntregaFormateada { get; set; }
        public int ItemPorPagina { get; set; }
        public int Pagina { get; set; }
        public int ItemsTotales { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string TextoSuministro { get; set; }
        public decimal? CantidadSolp { get; set; }
        public TablaSapDto UnidadMedida { get; set; }
        public TablaSapDto Moneda { get; set; }
        public string MonedaDescripcion { get; set; }
        public List<CotizacionSubPosicionDto> CotizacionSubPosiciones { get; set; } = new List<CotizacionSubPosicionDto>();
        public string UnidadMedidaDescripcion { get; set; }
        public int PlazoDeEntrega { get; set; }
        public decimal PrecioTotal { get; set; }
        public string MonedaCodigo { get; set; }
        public string FechaDeEntregaFormateado { get; set; }
        public DateTime? FechaOriginal { get; set; }
        public decimal TotalARPCotizacionPosicion { get; set; }
        public int CantidadPendiente { get; set; }
        public decimal TotalPesos { get; set; }
        public decimal TotalPosicionCotizacion { get; set; }
        public bool? NoDisponible { get; set; }
        public DateTime? FechaDeVigencia { get; set; }
        public string FechaDeVigenciaFormateado { get; set; }
    }

    public class CotizacionSubPosicionDto
    {
        public int Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public int SolpSubPosicion_Id { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadDeMedida_Id { get; set; }
        public int? Moneda_Id { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal PrecioTotalSubPosCotizacion { get; set; }
        public TablaSapDto UnidadMedida { get; set; }
        public decimal TotalARPSubPosCotizacion { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioTotalSubPos { get; set; }
        public decimal TotalPesos { get; set; }
    }

    public class GuardarCotizacionPosicionDto
    {
        public int PeticionDeOfertaSolpPosicionId { get; set; }
        public decimal? Precio { get; set; }
        public int? MonedaId { get; set; }
        public int? UnidadDeMedidaId { get; set; }
        public decimal? Cantidad { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public decimal PrecioTotal { get; set; }
        public decimal TotalPesos { get; set; }
        public bool? NoDisponible { get; set; }
        public DateTime? FechaDeVigencia { get; set; }
    }

    public class GuardarCotizacion
    {
        public int PeticionOfertaUsuarioId { get; set; }
        public List<GuardarCotizacionPosicionDto> CotizacionPosiciones { get; set; } = new List<GuardarCotizacionPosicionDto>();
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public List<ArchivoDto> ArchivosNuevos { get; set; } = new List<ArchivoDto>();
        public List<ArchivoDto> ArchivosGuardados { get; set; } = new List<ArchivoDto>();
        public int CotizacionId { get; set; }
        public bool EsFinalizado { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public List<CotizacionHorasDto> CotizacionesHoras { get; set; } = new List<CotizacionHorasDto>();
        public int? MonedaId { get; set; }
        public int? UnidadDeMedidaId { get; set; }
        public int? Cantidad { get; set; }
        public DateTime? FechaDeEntrega { get; set; }
        public List<CotizacionSubposicionesDto> CotizacionSubposiciones { get; set; } = new List<CotizacionSubposicionesDto>();
        public bool ConfigurarHora { get; set; }
        public DateTime? FechaDeVigencia { get; set; }
        public decimal? PorcentajeDeHoras { get; set; }
    }


    public class CotizacionSubposicionesDto
    {
        public int CotizacionSubPosicionId { get; set; }
        public decimal Precio { get; set; }
        public int? MonedaId { get; set; }
        public int CotizacionPosicionId { get; set; }
        public int? UnidadDeMedidaId { get; set; }
        public decimal Cantidad { get; set; }
        public int SolpSubPosicionId { get; set; }
        public decimal PrecioTotal { get; set; }
    }

    public class CotizacionHorasDto
    {
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public string Categoria { get; set; }
        public int CantidadPersonas { get; set; }
        public int HorasNormales { get; set; }
        public int HorasNocturnas { get; set; }
        public int HorasExtras { get; set; }
        public string Gremio { get; set; }
        public bool Fila { get; set; }
        public bool? ConfigurarHora { get; set; }
    }

}