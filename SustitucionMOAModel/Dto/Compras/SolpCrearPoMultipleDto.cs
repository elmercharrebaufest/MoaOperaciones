// Ignore Spelling: Solp Nro
using SustitucionMOAModel.Attributes;
using System;

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
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string Nombre { get; set; }

        [ExcelColumnName("Fecha Creación")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public DateTime FechaCreacion { get; set; }

        [ExcelColumnName("Fecha Liberación")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public DateTime? FechaLiberacion { get; set; }

        [ExcelColumnName("Solicitante")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string Solicitante { get; set; }

        [ExcelColumnName("Grupo de compras")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string GrupoDeCompras { get; set; }

        [ExcelColumnName("Centro")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string Centro { get; set; }

        [ExcelColumnName("Tipo (Sin Doc. / C/Doc. Pliego)")]
        [ExcelColumnType(ExcelCellType.Text)]
        [ExcelColumnOrder(0)]
        [ExcelColumnWidth(11)]
        public string Tipo { get; set; }
    }
}