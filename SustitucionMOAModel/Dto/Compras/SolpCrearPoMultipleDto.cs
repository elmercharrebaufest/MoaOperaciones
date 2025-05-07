// Ignore Spelling: Solp Nro
using SustitucionMOAModel.Attributes;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class SolpCrearPoMultipleDto
    {
        [ExcelIgnore]
        public int Id { get; set; }

        [ExcelColumnName("Nro SOLP")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string NroSolp { get; set; }

        [ExcelColumnName("Nombre SOLP/Pliego")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(1)]
        [ExcelColumnWidth(45)]
        public string Nombre { get; set; }

        [ExcelColumnName("Fecha Creación")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(2)]
        [ExcelColumnWidth(18)]
        public DateTime FechaCreacion { get; set; }

        [ExcelColumnName("Fecha Liberación")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(3)]
        [ExcelColumnWidth(18)]
        public DateTime? FechaLiberacion { get; set; }

        [ExcelColumnName("Solicitante")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(4)]
        [ExcelColumnWidth(15)]
        public string Solicitante { get; set; }

        [ExcelColumnName("Grupo de compras")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(5)]
        [ExcelColumnWidth(20)]
        public string GrupoDeCompras { get; set; }

        [ExcelColumnName("Centro")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(6)]
        [ExcelColumnWidth(20)]
        public string Centro { get; set; }

        [ExcelColumnName("Tipo (Sin Doc. / C/Doc. Pliego)")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(7)]
        [ExcelColumnWidth(28)]
        public string Tipo { get; set; }

        [ExcelIgnore]
        public bool MultipleFinalizado { get; set; }

        [ExcelIgnore]
        public bool SeraUsadoEnPliegoMultiple { get; set; }

        [ExcelIgnore]
        public bool TieneCotizacion { get; set; }

        [ExcelIgnore]
        public IEnumerable<string> ListaPO { get; set; }
    }
}