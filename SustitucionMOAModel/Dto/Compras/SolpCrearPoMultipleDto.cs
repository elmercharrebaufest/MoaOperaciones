// Ignore Spelling: Solp Nro
using iTextSharp.text;
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
        public List<List<string>> Posiciones { get; set; } = new List<List<string>>()
        {
            new List<string>() {"f1c1","f1c2"},
            new List<string>() {"f2c1","f2c2"},
        };
    }
}