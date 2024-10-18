using System;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class POPosicionDto
    {    
        public int Id { get; set; }
        public string NroSolp { get; set; }
        public int? Indice { get; set; }
        public string Codigo { get; set; }
        public string Tarea { get; set; }
        public string CentroComprasDescripcion { get; set; }
        public string AlmacenComprasDescripcion { get; set; }
        public string TextoSuministro { get; set; }
        public string Modelo { get; set; }
        public string GrupoComprasDescripcion { get; set; }
        public decimal? Cantidad { get; set; }
        public string UnidadComprasDescripcion { get; set; }
        public string MonedaSolpDescripcion { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public int? PlazoEntrega { get; set; }
        public DateTime? FechaOferta { get; set; }
        public bool TieneCotizacion { get; set; }
        public IEnumerable<string> ListaPO { get; set; }
    }
}