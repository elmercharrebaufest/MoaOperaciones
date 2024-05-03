using System;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;

namespace SustitucionMOAModel.Dto
{
    public class CotizacionHistorialDto 
    {
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public string Log { get; set; }
        public string FechaFinalizacion { get; set; }
        public int Usuario_Id { get; set; }
        public string UsuarioRazonSocial { get; set; }
        public LogCotizacionDto Cotizacion { get; set; }
    }

    public class LogCotizacionDto
    {
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public string CotizacionEstadoDescripcion { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public string RespetaMateriales { get; set; }
        public string RespetaServicios { get; set; }
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public int Revision { get; set; }
        public List<ArchivoDto> Archivos { get; set; } = new List<ArchivoDto>();
        public string FechaCreacion { get; set; }
        public decimal? PorcentajeDeHoras { get; set; }
        public List<LogCotizacionPosicionDto> CotizacionPosiciones { get; set; } = new List<LogCotizacionPosicionDto>();
        public List<LogCotizacionHorasDto> CotizacionesHoras { get; set; }
    }

    public class LogCotizacionPosicionDto
    {
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int PeticionDeOfertaSolpPosicion_Id { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public string FechaDeEntrega { get; set; }
        public string FechaDeVigencia { get; set; }
        public string MonedaCodigo { get; set; }
        public string UnidadMedidaDescripcion { get; set; }
        public decimal? PrecioTotal { get; set; }
        public string NoDisponible { get; set; }
        public int? PrimerPlazoDeOferta { get; set; }
        public decimal? PrimeraCantidad { get; set; }
        public int? SegundoPlazoDeOferta { get; set; }
        public decimal? SegundaCantidad { get; set; }
        public int? TercerPlazoDeOferta { get; set; }
        public decimal? TerceraCantidad { get; set; }
        public string EstaEliminado { get; set; }
        public List<LogCotizacionSubPosicionDto> CotizacionSubPosiciones { get; set; } = new List<LogCotizacionSubPosicionDto>();
        public int? Indice { get; set; }
        public int IdPosicion { get; set; }
        public string Descripcion { get; set; }
        public int? Codigo { get; set; }

    }

    public class LogCotizacionSubPosicionDto
    {
        public int? CotizacionSubPosicionId { get; set; }
        public decimal? Precio { get; set; }
        public string MonedaCodigo { get; set; }
        public string UnidadDeMedidaDescripcion { get; set; }
        public decimal? Cantidad { get; set; }
        public int SolpSubPosicionId { get; set; }
        public decimal PrecioTotal { get; set; }
        public int NroSubPosicion { get; set; }
        public int IdSubPosicion { get; set; }
        public string Descripcion { get; set; }
        public int? Codigo { get; set; }

    }

    public class LogCotizacionHorasDto
    {
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public string Categoria { get; set; }
        public int CantidadPersonas { get; set; }
        public int HorasNormales { get; set; }
        public int HorasNocturnas { get; set; }
        public string Gremio { get; set; }
    }
}