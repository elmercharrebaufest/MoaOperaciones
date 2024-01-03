using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionPosicionDto
    {   
        public int Id { get; set; }
        public int Adjudicacion_Id { get; set; }
        public int CotizacionPosicion_Id { get; set; }
        public decimal Cantidad { get; set; }
        public int SolpPosicion_Id { get; set; }
        public List<SolpSubposicionDto> SubposicionesCompras { get; set; }
        public string MaterialComprasCodigo { get; set; }
        public int? Indice { get; set; }
        public string Tarea { get; set; }
        public string CentroComprasDescripcion { get; set; }
        public string TextoSuministro { get; set; }
        public string Modelo { get; set; }
        public string UnidadDescripcion { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal? PrecioUnidad { get; set; }
        public int? MonedaId { get; set; }
        public decimal? PrecioTotal { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public int? PlazoDeOferta { get; set; }
        public string MaterialComprasDescripcion { get; set; }
        public string MonedaCodigo { get; set; }
        public string CentroComprasCodigo { get; set; }
        public string MaterialTextoAmpliado { get; set; }
        public string FechaEntregaServicioFormateado { get; set; }
        public DateTime? PlazoDeEntrega { get; set; }
    }
}
