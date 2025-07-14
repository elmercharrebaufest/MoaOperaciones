// Ignore Spelling: Solp Nro
using SustitucionMOAModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class POPosicionDto
    {
        [ExcelIgnore]
        public int Id { get; set; }

        [ExcelColumnName("Nro SOLP")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string NroSolp { get; set; }

        [ExcelColumnName("POS.")]
        [ExcelColumnType(ExcelCellType.Number)]
        [ExcelColumnOrder(1)]
        [ExcelColumnWidth(7)]
        public int? Indice { get; set; }

        [ExcelColumnName("Código")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(2)]
        [ExcelColumnWidth(20)]
        public string Codigo { get; set; }

        [ExcelColumnName("Descripción")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(3)]
        [ExcelColumnWidth(45)]
        public string Tarea { get; set; }

        [ExcelColumnName("Centro")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(4)]
        [ExcelColumnWidth(20)]
        public string CentroComprasDescripcion { get; set; }

        [ExcelColumnName("Almacén")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(5)]
        [ExcelColumnWidth(12)]
        public string AlmacenComprasDescripcion { get; set; }

        [ExcelColumnName("Texto de suministro")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(6)]
        [ExcelColumnWidth(45)]
        public string TextoSuministro { get; set; }

        [ExcelColumnName("Modelo")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(7)]
        [ExcelColumnWidth(12)]
        public string Modelo { get; set; }

        [ExcelColumnName("Grupo de compras")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(8)]
        [ExcelColumnWidth(20)]
        public string GrupoComprasDescripcion { get; set; }

        [ExcelColumnName("Ctd.")]
        [ExcelColumnType(ExcelCellType.Number)]
        [ExcelColumnOrder(9)]
        [ExcelColumnWidth(6)]
        public decimal? Cantidad { get; set; }

        [ExcelColumnName("Um")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(10)]
        [ExcelColumnWidth(6)]
        public string UnidadComprasDescripcion { get; set; }

        [ExcelColumnName("Mon.")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(11)]
        [ExcelColumnWidth(16)]
        public string MonedaSolpDescripcion { get; set; }

        [ExcelColumnName("Fecha de entrega")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(12)]
        [ExcelColumnWidth(22)]
        public DateTime? FechaEntregaServicio { get; set; }

        [ExcelColumnName("Plazo de entrega (días)")]
        [ExcelColumnType(ExcelCellType.Number)]
        [ExcelColumnOrder(13)]
        [ExcelColumnWidth(22)]
        public int? PlazoEntrega { get; set; }

        [ExcelColumnName("Plazo de oferta")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(14)]
        [ExcelColumnWidth(22)]
        public DateTime? FechaOferta { get; set; }

        [ExcelIgnore]
        public bool TieneCotizacion { get; set; }

        [ExcelIgnore]
        public IEnumerable<string> ListaPO { get; set; }
        
        [ExcelIgnore]
        public bool SeraUsadoEnPliegoMultiple { get; set; }

        [ExcelColumnName("Tiene PO")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(15)]
        [ExcelColumnWidth(60)]
        public string ListaPoFormateada
        {
            get
            {
                if (ListaPO == null) { return "-"; }
                if (!ListaPO.Any()) { return "-"; }
                return string.Join(" - ", ListaPO);
            }
        }
    }
}