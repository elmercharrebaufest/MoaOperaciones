using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Services
{
    public class ComprasArchivosService : IComprasArchivosService
    {
        public byte[] GenerarExcelHistorialCotizaciones(List<CotizacionHistorialDto> historialCotizaciones)
        {
            var memStream = new MemoryStream();
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(memStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                var nroHoja = 1U;

                foreach (var historialCotizacion in historialCotizaciones.OrderByDescending(x => x.FechaFinalizacion))
                {
                    var rowIndex = 1U;
                    var cotizacion = historialCotizacion.Cotizacion;
                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet();
                    
                    var sheet = new Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = new UInt32Value(nroHoja),
                        Name = new StringValue($"Cotización {historialCotizaciones.Count + 1 - nroHoja++}")
                    };
                    sheets.Append(sheet);
                    var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());
                    
                    var celdasTitulo = new OpenXmlElement[] { CrearCelda(historialCotizacion.UsuarioRazonSocial, 2U), CrearCelda(""), CrearCelda(historialCotizacion.FechaFinalizacion, 2U) };
                    sheetData.Append(new Row(celdasTitulo) { RowIndex = new UInt32Value(rowIndex) });
                    worksheetPart.Worksheet.InsertAfter(new MergeCells(new OpenXmlElement[] { new MergeCell { Reference = new StringValue("A1:B1") }, new MergeCell { Reference = new StringValue("C1:D1") } }), sheetData);
                    rowIndex++;

                    AgregarTabla(sheetData, "",
                        new List<string> { "Fecha creación", "Estado", "Respeta materiales", "Respeta servicios", "Obs económicas", "Obs técnicas", "Porcentaje de horas" },
                        new List<List<string>> { new List<string> { cotizacion.FechaCreacion, cotizacion.CotizacionEstadoDescripcion, cotizacion.RespetaMateriales, cotizacion.RespetaServicios, cotizacion.ObservacionEconomica, cotizacion.ObservacionTecnica, cotizacion.PorcentajeDeHoras.ToString() } },
                        ref rowIndex);
                    rowIndex += 2;

                    AgregarTabla(sheetData, "",
                        new List<string> { "Categoría", "Gremio", "Ctd Pers", "Hs Nor", "Hs Noc" },
                        cotizacion.CotizacionesHoras.Select(x => new List<string> { x.Categoria, x.Gremio, x.CantidadPersonas.ToString(), x.HorasNormales.ToString(), x.HorasNocturnas.ToString() }).ToList(),
                        ref rowIndex);
                    rowIndex += 3;

                    foreach (var posicion in cotizacion.CotizacionPosiciones)
                    {
                        ObtenerDatosTablaPosiciones(posicion, out List<string> cabeceras, out List<string> valoresFila);
                        AgregarTabla(sheetData, $"Nro posición: {posicion.Indice}", cabeceras, new List<List<string>> { valoresFila }, ref rowIndex);
                        rowIndex++;

                        if (posicion.CotizacionSubPosiciones != null && posicion.CotizacionSubPosiciones.Any())
                        {
                            ObtenerDatosTablaSubposiciones(posicion.CotizacionSubPosiciones, out List<string> cabecerasSubp, out List<List<string>> valoresSubp);
                            AgregarTabla(sheetData, "", cabecerasSubp, valoresSubp, ref rowIndex);
                        }
                        rowIndex += 3;
                    }

                    var columnas = AjustarColumnas(sheetData);
                    worksheetPart.Worksheet.InsertAt(columnas, 0);
                }

                var workbookStylesPart1 = workbookPart.AddNewPart<WorkbookStylesPart>("rId3");
                GenerateWorkbookStylesPartContent(workbookStylesPart1);
            }
            return memStream.ToArray();
        }

        private void AgregarTabla(SheetData sheetData, string titulo, List<string> cabeceras, List<List<string>> valores, ref uint rowIndex)
        {
            if (!string.IsNullOrEmpty(titulo))
            {
                sheetData.Append(new Row(new OpenXmlElement[] { CrearCelda(titulo, 2U) }) { RowIndex = new UInt32Value(rowIndex) });
            }

            sheetData.Append(new Row(cabeceras.Select(x => CrearCelda(x, 2U))) { RowIndex = new UInt32Value(++rowIndex) });

            foreach (var filaValores in valores)
            {
                var rowDatos = new Row { RowIndex = new UInt32Value(++rowIndex) };

                rowDatos.Append( filaValores.Select(x => CrearCelda(x)) );
                sheetData.Append(rowDatos);
            }
        }

        private void ObtenerDatosTablaPosiciones(LogCotizacionPosicionDto posicion, out List<string> cabeceras, out List<string> valoresFila)
        {
            cabeceras = new List<string> { "Descripción" };
            valoresFila = new List<string> { posicion.Descripcion };

            if (posicion.NoDisponible == "Disponible")
            {
                if (posicion.CotizacionSubPosiciones == null || !posicion.CotizacionSubPosiciones.Any())
                {
                    cabeceras.Add("Cantidad");
                    valoresFila.Add($"{posicion.Cantidad} - {posicion.UnidadMedidaDescripcion}");
                }
                if (posicion.Precio > 0)
                {
                    cabeceras.Add("Precio");
                    valoresFila.Add($"{posicion.Precio} - {posicion.MonedaCodigo}");
                }

                cabeceras.AddRange(new string[] { "Total", "Fecha de entrega" });
                valoresFila.AddRange(new string[] { $"{posicion.PrecioTotal}" + (string.IsNullOrEmpty(posicion.MonedaCodigo) ? "" : $" - {posicion.MonedaCodigo}"), posicion.FechaDeEntrega });

                if (!string.IsNullOrEmpty(posicion.FechaDeVigencia))
                {
                    cabeceras.Add("Fecha de vigencia");
                    valoresFila.Add(posicion.FechaDeVigencia);
                }

                if (posicion.PrimerPlazoDeOferta > 0)
                {
                    cabeceras.Add("Plazo de oferta");
                    valoresFila.Add($"{posicion.PrimerPlazoDeOferta}" + (posicion.PrimeraCantidad > 0 ? $" - Ctd {posicion.PrimeraCantidad}" : ""));
                }

                if (posicion.SegundoPlazoDeOferta > 0)
                {
                    cabeceras.Add("Segundo plazo de oferta");
                    valoresFila.Add($"{posicion.SegundoPlazoDeOferta}" + (posicion.SegundaCantidad > 0 ? $" - Ctd {posicion.SegundaCantidad}" : ""));
                }

                if (posicion.TercerPlazoDeOferta > 0)
                {
                    cabeceras.Add("Tercer plazo de oferta");
                    valoresFila.Add($"{posicion.TercerPlazoDeOferta}" + (posicion.TerceraCantidad > 0 ? $" - Ctd {posicion.TerceraCantidad}" : ""));
                }

                if (!string.IsNullOrEmpty(posicion.EstaEliminado))
                {
                    cabeceras.Add("");
                    valoresFila.Add($"{posicion.EstaEliminado}");
                }
            }
        }

        private void ObtenerDatosTablaSubposiciones(List<LogCotizacionSubPosicionDto> subposiciones, out List<string> cabeceras, out List<List<string>> valoresFila)
        {
            cabeceras = new List<string> { "Nro subposición", "Descripción", "Cantidad", "Precio", "Total" };
            valoresFila = subposiciones.Select(
                x => new List<string>
                {
                    $"{x.NroSubPosicion}",
                    x.Descripcion,
                    $"{x.Cantidad} - {x.UnidadDeMedidaDescripcion}",
                    $"{x.Precio} - {x.MonedaCodigo}",
                    $"{x.PrecioTotal} - {x.MonedaCodigo}"
                }).ToList();
        }

        private Cell CrearCelda(string valor)
        {
            return new Cell
            {
                DataType = ResolverTipoCelda(valor),
                CellValue = new CellValue(valor)
            };
        }
        
        private Cell CrearCelda(string valor, uint styleIndex)
        {
            var cell = CrearCelda(valor);
            cell.StyleIndex = styleIndex;
            return cell;
        }

        private EnumValue<CellValues> ResolverTipoCelda(string text)
        {
            return (int.TryParse(text, out _) || double.TryParse(text, out _))
                ? CellValues.Number
                : CellValues.String;
        }

        private void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            Font font2 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append(bold1);
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color3);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color4);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color5);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color6 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color6);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)3U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        private Columns AjustarColumnas(SheetData sheetData)
        {
            var columnas = new Columns();
            var anchosMaximos = sheetData.Elements<Row>()
                .SelectMany(r => r.Elements<Cell>().Select((c, i) => new { Index = i, Width = GetCellText(c).Length, Cell = c }))
                .Where(c => !EsCeldaAgrupada(c.Cell))
                .GroupBy(c => c.Index)
                .Select(g => new { Index = g.Key, MaxWidth = g.Max(c => c.Width) });

            foreach (var col in anchosMaximos)
            {
                double ancho = col.MaxWidth + 2; // Margen
                columnas.Append(new Column() { Min = (uint)(col.Index + 1), Max = (uint)(col.Index + 1), Width = ancho, CustomWidth = true });
            }

            return columnas;
        }

        private string GetCellText(Cell cell)
        {
            return cell.CellValue?.Text ?? string.Empty;
        }

        private bool EsCeldaAgrupada(Cell cell)
        {
            if (cell.CellReference != null)
            {
                string cellRef = cell.CellReference.Value;
                if (cellRef.Contains(":"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
